﻿using Aequus;
using Aequus.Common.DataSets;
using Aequus.Items.Misc.Spawners;
using Aequus.Tiles.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Items.Materials.PearlShards {
    public class PearlShardWhite : ModItem {
        public const int AmountPerPearl = 3;
        public const int InWorldVerticalFrames = 3;
        public virtual int PearlItem => ItemID.WhitePearl;
        public virtual int PearlTile => ModContent.TileType<PearlsTileWhite>();
        public virtual Texture2D DroppedSprite => AequusTextures.PearlShardWhite_Dropped;

        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 25;
        }

        public override void SetDefaults() {
            Item.CloneDefaults(PearlItem);
            Item.width = 6;
            Item.height = 6;
            Item.value /= AmountPerPearl;
            Item.rare = Math.Clamp(Item.rare - 1, 0, ItemRarityID.Count);

            Item.consumable = true;
            Item.useTime = 10;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;
            Item.createTile = PearlTile;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
            var texture = DroppedSprite;
            var frame = texture.Frame(verticalFrames: InWorldVerticalFrames, frameY: whoAmI % InWorldVerticalFrames);

            spriteBatch.Draw(
                texture,
                Item.Bottom + new Vector2(0f, -3f) - Main.screenPosition,
                frame,
                lightColor,
                rotation,
                frame.Size() / 2f,
                scale, SpriteEffects.None, 0f
            );
            return false;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed) {
            if (Main.netMode == NetmodeID.Server) {
                return;
            }

            var clr = Helper.GetColor(Item.Center).ToVector3();
            int chance = Math.Clamp((int)(clr.Length() * 25f), 0, 99);
            if (chance < 4) {
                return;
            }

            if (Main.rand.NextBool(100 - chance)) {
                var d = Dust.NewDustDirect(
                    Item.TopLeft,
                    Item.width, Item.height,
                    DustID.SilverFlame,
                    newColor: Color.White,
                    Scale: Main.rand.NextFloat(0.4f, 0.6f)
                );
                d.velocity = Vector2.Zero;
                d.noGravity = true;
                d.fadeIn = d.scale + Main.rand.NextFloat(0.4f, 1f);
            }
        }

        public override void AddRecipes() {
            Recipe.Create(PearlItem)
                .AddIngredient(Type, AmountPerPearl)
                .AddTile(TileID.GlassKiln)
                .Register()
                .DisableDecraft();
        }
    }

    public class PearlShardBlack : PearlShardWhite {
        public override int PearlItem => ItemID.BlackPearl;
        public override int PearlTile => ModContent.TileType<PearlsTileBlack>();
        public override Texture2D DroppedSprite => AequusTextures.PearlShardBlack_Dropped;
    }

    public class PearlShardPink : PearlShardWhite {
        public override int PearlItem => ItemID.PinkPearl;
        public override int PearlTile => ModContent.TileType<PearlsTilePink>();
        public override Texture2D DroppedSprite => AequusTextures.PearlShardPink_Dropped;
    }

    public abstract class PearlsTile : BaseGemTile {
        internal abstract Color MapColor { get; }
        internal abstract LocalizedText MapKey { get; }
        public abstract float Weight { get; }

        public override void SetStaticDefaults() {
            base.SetStaticDefaults();
            TileSets.OceanPearlsToGenerate.Add(this);
            Main.tileObsidianKill[Type] = true;
            Main.tileShine[Type] = 400;
            Main.tileShine2[Type] = true;
            Main.tileOreFinderPriority[Type] = 110;
            Main.tileSpelunker[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileLighted[Type] = true;

            AddMapEntry(MapColor, MapKey);
            DustType = DustID.Glass;
            HitSound = SoundID.Shatter;
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
            r = 0.12f;
            g = 0.12f;
            b = 0.12f;
        }

        public static bool TryGrow(int i, int j) {
            int gemX = i + WorldGen.genRand.Next(-1, 2);
            int gemY = j + WorldGen.genRand.Next(-1, 2);
            if (Framing.GetTileSafely(gemX, gemY).HasTile || TileHelper.ScanTilesSquare(gemX, gemY, 8, (i, j) => TileLoader.GetTile(Main.tile[i, j].TileType) is PearlsTile)) {
                return false;
            }

            PearlsTile tileInstance;
            if (!Main.getGoodWorld || WorldGen.genRand.NextBool(3)) {
                do {
                    tileInstance = WorldGen.genRand.Next(TileSets.OceanPearlsToGenerate);
                }
                while (tileInstance.Weight <= WorldGen.genRand.NextFloat());
            }
            else {
                tileInstance = ModContent.GetInstance<PearlsTileHypnotic>();
            }
            if (tileInstance == null || !tileInstance.CanPlace(gemX, gemY)) {
                return false;
            }

            WorldGen.PlaceTile(gemX, gemY, tileInstance.Type, mute: true);
            if (Main.tile[gemX, gemY].TileType == tileInstance.Type && Main.netMode != NetmodeID.SinglePlayer) {
                NetMessage.SendTileSquare(-1, gemX, gemY);
            }
            return true;
        }
    }

    [LegacyName("PearlsTile")]
    public class PearlsTileWhite : PearlsTile {
        internal override Color MapColor => new Color(190, 200, 222);
        internal override LocalizedText MapKey => Lang.GetItemName(ItemID.WhitePearl);
        public override float Weight => 1f;
    }

    public class PearlsTileBlack : PearlsTile {
        internal override Color MapColor => new Color(124, 128, 172);
        internal override LocalizedText MapKey => Lang.GetItemName(ItemID.BlackPearl);
        public override float Weight => 0.5f;
    }

    public class PearlsTilePink : PearlsTile {
        internal override Color MapColor => new Color(212, 136, 205);
        internal override LocalizedText MapKey => Lang.GetItemName(ItemID.PinkPearl);
        public override float Weight => 0.2f;
    }

    public class PearlsTileHypnotic : PearlsTile {
        internal override Color MapColor => new Color(105, 186, 220);
        internal override LocalizedText MapKey => TextHelper.GetItemName<HypnoticPearl>();
        public override float Weight => 0.2f;

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
            g += (float)Math.Sin(Main.GameUpdateCount / 30f) * 0.33f;
            b += (float)Math.Sin(Main.GameUpdateCount / 30f + MathHelper.Pi) * 0.33f;
            if (g < 0.3f)
                g = 0.3f;
            if (b < 0.3f)
                b = 0.3f;
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j) {
            return new List<Item>() { new(ModContent.ItemType<HypnoticPearl>()) };
        }
    }
}