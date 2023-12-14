using Aequus.Common.Wires;
using Aequus.Content.Wires.Conductive;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.Wires;

public class CircuitSystem : ModSystem {
    public readonly Queue<ElectricCircuit> ActiveCircuits = new(64);
    private readonly Dictionary<Point, ElectricCircuit> NextCircuits = new(64);

    private static readonly int[,] _wireDataCache = new int[5, 5];

    public static int MaxSplits { get; set; } = 12;

    private void CheckCircuitDirection(ElectricCircuit circuit, ushort wantedTileType, byte direction) {
        var directionPosition = circuit.Position + ElectricCircuit.DirectionIds[direction];
        var nextTile = Framing.GetTileSafely(directionPosition);
        if (nextTile.TileType == wantedTileType) {
            ConductiveSystem.ActivationPoints[directionPosition] = new() { timeActive = 0, intensity = 1f - (circuit.SplitCount / (MaxSplits * 2)), };

            if (NextCircuits.TryGetValue(directionPosition, out var competingCircuit)) {
                if (competingCircuit.SplitCount < circuit.SplitCount) {
                    return;
                }

                if (circuit.SplitCount > 2 && competingCircuit.SplitCount == circuit.SplitCount) {
                    // Self Collision
                    NextCircuits[directionPosition] = circuit with { Direction = ElectricCircuit.Dead };
                    return;
                }
            }

            if (circuit.Direction == direction) {
                NextCircuits[directionPosition] = new(directionPosition, direction, circuit.SplitCount);
            }
            else if (circuit.SplitCount < MaxSplits) {
                NextCircuits[directionPosition] = new(directionPosition, direction, (byte)Math.Min(circuit.SplitCount + 1, 100));
            }
        }
    }

    public void UpdateCircuits() {
        if (Main.GameUpdateCount % 2 != 0) {
            return;
        }

        WiringSystem.MechCooldownMultiplier = 0.05f;

        while (ActiveCircuits.TryDequeue(out var circuit)) {
            if (!WorldGen.InWorld(circuit.Position.X, circuit.Position.Y, 10)) {
                continue;
            }

            var tile = Framing.GetTileSafely(circuit.Position).TileType;
            if (TileLoader.GetTile(tile) is not ConductiveBlock) {
                continue;
            }

            HitWires(circuit.Position.X, circuit.Position.Y);
            CheckCircuitDirection(circuit, tile, circuit.Direction);
            CheckCircuitDirection(circuit, tile, ElectricCircuit.LeftTransform[circuit.Direction]);
            CheckCircuitDirection(circuit, tile, ElectricCircuit.RightTransform[circuit.Direction]);
        }

        ActiveCircuits.EnsureCapacity(NextCircuits.Count);
        foreach (var c in NextCircuits.Where(c => c.Value.Direction != ElectricCircuit.Dead)) {
            ActiveCircuits.Enqueue(c.Value);
        }
        NextCircuits.Clear();

        WiringSystem.MechCooldownMultiplier = 1f;
    }

    public void HitWires(int x, int y) {
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

    public void HitCircuit(int x, int y) {
        ConductiveSystem.ActivationPoints[new(x, y)] = new() { timeActive = 0, intensity = 1f, };
        for (byte i = 0; i < ElectricCircuit.DirectionCount; i++) {
            ActiveCircuits.Enqueue(new(new(x, y), i));
        }
    }
}