﻿using Aequus.Core;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using Terraria.Utilities;

namespace Aequus.Content.Tiles.CrabPots;

public class TECrabPot : ModTileEntity {
    public static int ChancePerTick { get; set; } = 100000;
    public static float BaitEffectiveness { get; set; } = 0.5f;

    public Item item = new();

    public CrabPotBiomeData biomeData;

    public static int WaterStyle => LiquidsSystem.WaterStyle;

    public override bool IsTileValidForEntity(int x, int y) {
        return Main.tile[x, y].HasTile && Main.tile[x, y].TileFrameX % 36 == 0 && Main.tile[x, y].TileFrameY == 0 && Main.tile[x, y].TileType == ModContent.TileType<CrabPot>();
    }

    public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate) {
        TileObjectData tileData = TileObjectData.GetTileData(type, style, alternate);
        int x = i - tileData.Origin.X;
        int y = j - tileData.Origin.Y;
        var biomeData = new CrabPotBiomeData(WaterStyle);
        if (Main.netMode == NetmodeID.MultiplayerClient) {
            NetMessage.SendTileSquare(Main.myPlayer, x, y, tileData.Width, tileData.Height);
            ModContent.GetInstance<PacketCrabPotPlacement>().Send(x, y, WaterStyle);
            return -1;
        }

        int id = Place(x, y);
        ((TECrabPot)ByID[id]).biomeData = biomeData;
        return id;
    }

    public override void OnNetPlace() {
        NetMessage.SendData(MessageID.TileEntitySharing, number: ID, number2: Position.X, number3: Position.Y);
    }

    public override void NetSend(BinaryWriter writer) {
        ItemIO.Send(item, writer, writeStack: true);
    }

    public override void NetReceive(BinaryReader reader) {
        ItemIO.Receive(item, reader, readStack: true);
    }

    public override void SaveData(TagCompound tag) {
        tag["biome"] = biomeData.Save();
        if (!item.IsAir) {
            tag["item"] = item;
        }
    }

    public override void LoadData(TagCompound tag) {
        biomeData = CrabPotBiomeData.Load(tag.Get<TagCompound>("biome"));
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
            return;
        }
    }

    public override void Update() {
        if (!item.IsAir && item.bait <= 0) {
            return;
        }

        int chance = ChancePerTick;
        if (!item.IsAir && item.bait > 0) {
            chance /= (int)Math.Max(item.bait * BaitEffectiveness, 1f);
        }

        if (Main.rand.NextBool(chance)) {
            RollFish(Main.rand);
            NetMessage.SendData(MessageID.TileEntitySharing, number: ID, number2: Position.X, number3: Position.Y);
        }
    }

    public override void OnKill() {
        if (!item.IsAir && Main.netMode != NetmodeID.MultiplayerClient) {
            Item.NewItem(new EntitySource_TileBreak(Position.X, Position.Y), new Vector2(Position.X, Position.Y) * 16f, 32, 32, item.Clone());
            item = null;
        }
    }
}