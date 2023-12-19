using Aequus.Core;
using Aequus.Core.Graphics.Animations;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using Terraria.Utilities;

namespace Aequus.Content.Fishing.CrabPots;

public class TECrabPot : ModTileEntity {
    public static int ChancePerTick { get; set; } = 100000;

    /// <summary>
    /// Multiplier to <see cref="ChancePerTick"/> for crab pots inside of lava. Defaults to 2 (chance is twice as rare)
    /// </summary>
    public static double LavaEffectiveness { get; set; } = 2.0;

    /// <summary>
    /// Determines how effective bait is. Defaults to 0.5 (50% effectiveness)
    /// <para>Bait divides <see cref="ChancePerTick"/> by its bait power multiplied by this value. So a 50 bait power bait would divide <see cref="ChancePerTick"/> by 25 by default.</para>
    /// </summary>
    public static double BaitEffectiveness { get; set; } = 0.5;

    public Item item = new();
    /// <summary>
    /// Determines whether or not the item was from a catch.
    /// This prevents some catches, like Jellyfishes being used as bait or being indicated as bait.
    /// </summary>
    public bool caught;

    public CrabPotBiomeData biomeData;

    public static int WaterStyle => LiquidsSystem.WaterStyle;

    public override bool IsTileValidForEntity(int x, int y) {
        return Main.tile[x, y].HasTile && Main.tile[x, y].TileFrameX % 36 == 0 && Main.tile[x, y].TileFrameY == 0 && TileLoader.GetTile(Main.tile[x, y].TileType) is BaseCrabPot;
    }

    public static void PlacementEffects(int x, int y) {
        if (Main.netMode != NetmodeID.Server) {
            AnimationSystem.GetValueOrAddDefault<AnimationPlaceCrabPot>(x, y);
        }
    }

    public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate) {
        var tileData = TileObjectData.GetTileData(type, style, alternate);
        int x = i - tileData.Origin.X;
        int y = j - tileData.Origin.Y;
        var biomeData = new CrabPotBiomeData(GetWaterStyle(x, y));
        if (Main.netMode == NetmodeID.MultiplayerClient) {
            NetMessage.SendTileSquare(Main.myPlayer, x, y, tileData.Width, tileData.Height);
            ModContent.GetInstance<PacketCrabPotPlacement>().Send(x, y, biomeData.LiquidStyle);
            return -1;
        }

        int id = Place(x, y);
        ((TECrabPot)ByID[id]).biomeData = biomeData;
        PlacementEffects(x, y);
        return id;
    }

    public override void OnNetPlace() {
        NetMessage.SendData(MessageID.TileEntitySharing, number: ID, number2: Position.X, number3: Position.Y);
    }

    public override void NetSend(BinaryWriter writer) {
        writer.Write(caught);
        ItemIO.Send(item, writer, writeStack: true);
    }

    public override void NetReceive(BinaryReader reader) {
        caught = reader.ReadBoolean();
        ItemIO.Receive(item, reader, readStack: true);
    }

    public override void SaveData(TagCompound tag) {
        tag["biome"] = biomeData.Save();
        tag["caught"] = caught;
        if (!item.IsAir) {
            tag["item"] = item;
        }
    }

    public override void LoadData(TagCompound tag) {
        biomeData = CrabPotBiomeData.Load(tag.Get<TagCompound>("biome"));
        caught = tag.GetBool("caught");
        if (tag.TryGet<Item>("item", out var savedItem)) {
            item = savedItem.Clone();
        }
    }

    private void RollFish(UnifiedRandom random) {
        if (!CrabPotLootTable.Table.TryGetValue(biomeData.LiquidStyle, out var rules)) {
            return;
        }

        for (int i = 0; i < 5; i++) {
            int fishChoice = random.Next(rules.Count);
            var rule = rules[fishChoice];
            if (!rule.PassesCondition(Position.X, Position.Y) || !rule.RollChance(random)) {
                continue;
            }

            int stack = rule.RollStack(random);
            item = new(rule.ItemId, stack);
            caught = true;
            return;
        }
    }

    public override void Update() {
        if (!item.IsAir && item.bait <= 0) {
            return;
        }

        caught = false;
        int chance = ChancePerTick;
        if (biomeData.LiquidStyle == WaterStyleID.Lava) {
            chance = (int)Math.Max(chance * LavaEffectiveness, 1f);
        }
        if (!item.IsAir && item.bait > 0) {
            chance /= (int)Math.Max(item.bait * BaitEffectiveness, 1f);
        }

        if (Main.rand.NextBool(chance)) {
            RollFish(Main.rand);
            NetMessage.SendData(MessageID.TileEntitySharing, number: ID, number2: Position.X, number3: Position.Y);
        }
    }

    public void ClearItem() {
        item.TurnToAir();
        caught = false;
    }

    public override void OnKill() {
        if (!item.IsAir && Main.netMode != NetmodeID.MultiplayerClient) {
            Item.NewItem(new EntitySource_TileBreak(Position.X, Position.Y), new Vector2(Position.X, Position.Y) * 16f, 32, 32, item.Clone());
            item = null;
        }
    }

    public static int GetWaterStyle(int x, int y) {
        return Framing.GetTileSafely(x, y).LiquidType switch {
            LiquidID.Shimmer => 14,
            LiquidID.Honey => WaterStyleID.Honey,
            LiquidID.Lava => WaterStyleID.Lava,
            _ => LiquidsSystem.WaterStyle
        };
    }
}