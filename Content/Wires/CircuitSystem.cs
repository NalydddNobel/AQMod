using Aequus.Common.Wires;
using Aequus.Content.Wires.Conductive;
using Aequus.Core.Networking;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Wires;

public class CircuitSystem : ModSystem {
    public readonly Queue<ElectricCircuit> ActiveCircuits = new(64);
    private readonly Dictionary<Point, ElectricCircuit> NextCircuits = new(64);

    private static readonly int[,] _wireDataCache = new int[5, 5];

    public int MaxSplits { get; set; } = 12;
    public int MaxTurns { get; set; } = 60;

    public float MechCooldownMultiplier { get; set; } = 0.5f;

    public float TickRate { get; set; } = 0.5f;

    private float _tick;

    public void UpdateCircuits() {
        _tick += TickRate;

        WiringSystem.MechCooldownMultiplier = MechCooldownMultiplier;
        while (_tick > 1f) {
            _tick -= 1f;
            UpdateCircuitsInner();
        }
        WiringSystem.MechCooldownMultiplier = 1f;
    }

    private void UpdateCircuitsInner() {
        while (ActiveCircuits.TryDequeue(out var circuit)) {
            if (!WorldGen.InWorld(circuit.Position.X, circuit.Position.Y, 10)) {
                continue;
            }

            var tile = Framing.GetTileSafely(circuit.Position).TileType;
            if (TileLoader.GetTile(tile) is not ConductiveBlock) {
                continue;
            }

            HitWires(circuit.Position.X, circuit.Position.Y);

            StepCircuit(tile, circuit);
        }

        ActiveCircuits.EnsureCapacity(NextCircuits.Count);
        foreach (var c in NextCircuits.Where(c => c.Value.Direction != ElectricCircuit.Dead)) {
            ActiveCircuits.Enqueue(c.Value);
        }
        NextCircuits.Clear();
    }

    public void HitWires(int x, int y) {
        if (Main.netMode == NetmodeID.MultiplayerClient) {
            return;
        }

        for (int i = -2; i < 3; i++) {
            for (int j = -2; j < 3; j++) {
                var tile = Framing.GetTileSafely(x + i, y + j);
                ref var data = ref tile.Get<TileWallWireStateData>();
                _wireDataCache[i + 2, j + 2] = data.WireData;
                data.WireData = 0;
            }
        }

        for (int i = -1; i < 2; i++) {
            for (int j = -1; j < 2; j++) {
                var tile = Framing.GetTileSafely(x + i, y + j);
                tile.Get<TileWallWireStateData>().WireData = TileLoader.GetTile(tile.TileType) is not ConductiveBlock ? 1 : 0;
            }
        }

        try {
            for (int i = -1; i < 2; i++) {
                for (int j = -1; j < 2; j++) {
                    Wiring.TripWire(x + i, y + j, 1, 1);
                }
            }
        }
        catch (Exception ex) {
            Mod.Logger.Error(ex);
        }

        for (int i = -2; i < 3; i++) {
            for (int j = -2; j < 3; j++) {
                Framing.GetTileSafely(x + i, y + j).Get<TileWallWireStateData>().WireData = _wireDataCache[i + 2, j + 2];
            }
        }
    }

    private void StepCircuit(ushort tile, in ElectricCircuit circuit) {
        var forward = circuit.PosForward;
        // If you can move forward, just go forward
        if (CanMoveTo(tile, Main.tile[forward])) {
            PlaceCircuitStep(circuit with { Position = forward, });
            return;
        }

        // Otherwise we need to turn, dont continue if you've turned too much (possible infinite loop)
        if (circuit.TurnCounts > MaxTurns) {
            return;
        }

        var left = circuit.PosLeft;
        var right = circuit.PosRight;
        bool moveLeft = CanMoveTo(tile, Main.tile[left]);
        bool moveRight = CanMoveTo(tile, Main.tile[right]);

        var outputCircuit = circuit with { TurnCounts = (byte)(circuit.TurnCounts + 1) };

        // If we can turn both directions, split
        if (moveLeft && moveRight) {
            if (circuit.SplitCount < MaxSplits) {
                outputCircuit = outputCircuit with { SplitCount = (byte)(outputCircuit.SplitCount + 1) };
                PlaceCircuitStep(outputCircuit with { Position = left, Direction = outputCircuit.DirLeft, });
                PlaceCircuitStep(outputCircuit with { Position = right, Direction = outputCircuit.DirRight, });
            }
        }
        // Otherwise turn in any potential direction
        else if (moveLeft) {
            PlaceCircuitStep(outputCircuit with { Position = left, Direction = outputCircuit.DirLeft, });
        }
        else if (moveRight) {
            PlaceCircuitStep(outputCircuit with { Position = right, Direction = outputCircuit.DirRight, });
        }

        // Current dies if it reaches a dead end without any conductors to the left or right
    }

    private void PlaceCircuitStep(ElectricCircuit circuit) {
        LegacyConductiveSystem.ActivationPoints[circuit.Position] = new() { timeActive = 0, intensity = (1f - Math.Max(circuit.SplitCount / (float)MaxSplits, circuit.TurnCounts / (float)MaxTurns)) * 0.9f + 0.1f, };

        if (NextCircuits.TryGetValue(circuit.Position, out var competingCircuit)) {
            if (competingCircuit.SplitCount < circuit.SplitCount) {
                return;
            }

            if (circuit.SplitCount > 2 && competingCircuit.SplitCount == circuit.SplitCount) {
                // Collision
                NextCircuits[circuit.Position] = circuit with { Direction = ElectricCircuit.Dead };
                return;
            }
        }

        NextCircuits[circuit.Position] = circuit;
    }

    private static bool CanMoveTo(ushort wantedTile, Tile nextTile) {
        return wantedTile == nextTile.TileType;
    }

    public override void PreUpdateEntities() {
        UpdateCircuits();
    }

    public void HitCircuit(int x, int y, bool quiet = false) {
        if (!quiet && Main.netMode != NetmodeID.SinglePlayer) {
            PacketSystem.Get<HitCircuitPacket>().Send(x, y);

            if (Main.netMode == NetmodeID.MultiplayerClient) {
                return;
            }
        }

        LegacyConductiveSystem.ActivationPoints[new(x, y)] = new() { timeActive = 0, intensity = 1f, };
        for (byte i = 0; i < ElectricCircuit.DirectionCount; i++) {
            ActiveCircuits.Enqueue(new(new(x, y), i));
        }
    }

    public override void ClearWorld() {
        NextCircuits.Clear();
        ActiveCircuits.Clear();
    }

    public class HitCircuitPacket : PacketHandler {
        public void Send(int x, int y) {
            var packet = GetPacket();
            packet.Write((ushort)x);
            packet.Write((ushort)y);
            packet.Send();
        }

        public override void Receive(BinaryReader reader, int sender) {
            var x = reader.ReadUInt16();
            var y = reader.ReadUInt16();
            ModContent.GetInstance<CircuitSystem>().HitCircuit(x, y);

            if (Main.netMode == NetmodeID.Server) {
                Send(x, y);
            }
        }
    }
}