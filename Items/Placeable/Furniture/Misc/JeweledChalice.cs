﻿using Aequus.Content.Town.ExporterNPC.Quest;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Items.Placeable.Furniture.Misc
{
    public class JeweledChalice : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 5;
            ExporterQuestSystem.QuestItems.Add(Type, new DefaultThieveryItemInfo());
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<JeweledChaliceTile>());
            Item.width = 16;
            Item.height = 24;
            Item.rare = ItemRarityID.White;
            Item.value = Item.buyPrice(gold: 1);
            Item.maxStack = 9999;
        }
    }

    public class JeweledChaliceTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileShine[Type] = 5000;
            Main.tileShine2[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.StyleOnTable1x1);
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.addTile(Type);
            AddMapEntry(Color.Gold * 1.25f, TextHelper.GetText("ItemName.JeweledChalice"));
            HitSound = SoundID.Dig;

            ExporterQuestSystem.TilePlacements.Add(Type, new PlacementSolidTop());
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 0;
        }

        public override bool Drop(int i, int j)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 16, ModContent.ItemType<JeweledChalice>());
            return true;
        }
    }
}