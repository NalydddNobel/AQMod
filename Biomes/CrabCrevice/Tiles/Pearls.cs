using Aequus.Items.Boss.Summons;
using Aequus.Tiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Biomes.CrabCrevice.Tiles
{
    public class PearlShardWhite : ModItem 
    {
        public const int AmountPerPearl = 5;
        public virtual int PearlItem => ItemID.WhitePearl;

        public override void SetDefaults()
        {
            Item.CloneDefaults(PearlItem);
            Item.width = 16;
            Item.height = 16;
            Item.value /= AmountPerPearl;
            Item.rare = Math.Clamp(Item.rare - 1, 0, ItemRarityID.Count);
        }

        public override void AddRecipes()
        {
            Recipe.Create(PearlItem)
                .AddIngredient(Type, AmountPerPearl)
                .AddTile(TileID.GlassKiln)
                .Register();
        }
    }

    public class PearlShardBlack : PearlShardWhite 
    {
        public override int PearlItem => ItemID.BlackPearl;
    }

    public class PearlShardPink : PearlShardWhite 
    {
        public override int PearlItem => ItemID.PinkPearl;
    }

    public class PearlsTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;
            Main.tileShine[Type] = 400;
            Main.tileShine2[Type] = true;
            Main.tileOreFinderPriority[Type] = 110;
            Main.tileSpelunker[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileLighted[Type] = true;

            TileID.Sets.DisableSmartCursor[Type] = true;

            AddMapEntry(new Color(190, 200, 222), CreateMapEntryName("Pearl"));
            AddMapEntry(new Color(105, 186, 220), CreateMapEntryName("HypnoticPearl"));
            DustType = DustID.Glass;
            ItemDrop = ModContent.ItemType<PearlShardWhite>();
            HitSound = SoundID.Shatter;
        }

        public override ushort GetMapOption(int i, int j)
        {
            return (ushort)(Main.tile[i, j].TileFrameX / 18 == 3 ? 1 : 0);
        }

        public override bool CanPlace(int i, int j)
        {
            var top = Framing.GetTileSafely(i, j - 1);
            if (top.HasTile && !top.BottomSlope && top.TileType >= 0 && Main.tileSolid[top.TileType] && !Main.tileSolidTop[top.TileType])
            {
                return true;
            }
            var bottom = Framing.GetTileSafely(i, j + 1);
            if (bottom.HasTile && !bottom.IsHalfBlock && !bottom.TopSlope && bottom.TileType >= 0 && (Main.tileSolid[bottom.TileType] || Main.tileSolidTop[bottom.TileType]))
            {
                return true;
            }
            var left = Framing.GetTileSafely(i - 1, j);
            if (left.HasTile && left.TileType >= 0 && Main.tileSolid[left.TileType] && !Main.tileSolidTop[left.TileType])
            {
                return true;
            }
            var right = Framing.GetTileSafely(i + 1, j);
            if (right.HasTile && right.TileType >= 0 && Main.tileSolid[right.TileType] && !Main.tileSolidTop[right.TileType])
            {
                return true;
            }
            return false;
        }

        public override bool Drop(int i, int j)
        {
            switch (Main.tile[i, j].TileFrameX / 18)
            {
                case 0:
                    Item.NewItem(new EntitySource_TileBreak(i, j), new Rectangle(i * 16, j * 16, 16, 16), ModContent.ItemType<PearlShardWhite>(), Main.rand.Next(2) + 1);
                    return false;
                case 1:
                    Item.NewItem(new EntitySource_TileBreak(i, j), new Rectangle(i * 16, j * 16, 16, 16), ModContent.ItemType<PearlShardBlack>(), Main.rand.Next(2) + 1);
                    return false;
                case 2:
                    Item.NewItem(new EntitySource_TileBreak(i, j), new Rectangle(i * 16, j * 16, 16, 16), ModContent.ItemType<PearlShardPink>(), Main.rand.Next(2) + 1);
                    return false;
                case 3:
                    Item.NewItem(new EntitySource_TileBreak(i, j), new Rectangle(i * 16, j * 16, 16, 16), ModContent.ItemType<HypnoticPearl>());
                    return false;
            }
            return true;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.12f;
            g = 0.12f;
            b = 0.12f;
            if (Main.tile[i, j].TileFrameX >= 18 * 3)
            {
                g += (float)Math.Sin(Main.GameUpdateCount / 30f) * 0.33f;
                b += (float)Math.Sin(Main.GameUpdateCount / 30f + MathHelper.Pi) * 0.33f;
                if (g < 0.3f)
                    g = 0.3f;
                if (b < 0.3f)
                    b = 0.3f;
            }
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            AequusTile.GemFrame(i, j);
            return false;
        }
    }
}