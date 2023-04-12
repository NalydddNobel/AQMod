using Aequus;
using Aequus.Content.Boss.Crabson.Misc;
using Aequus.Tiles.Base;
using Aequus.Tiles.CrabCrevice;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Materials.Gems {
    public class PearlShardWhite : ModItem {
        public const int AmountPerPearl = 3;
        public const int InWorldVerticalFrames = 3;
        public virtual int PearlItem => ItemID.WhitePearl;
        public virtual int PearlTileStyle => PearlsTile.STYLE_WHITE;
        public virtual Texture2D DroppedSprite => AequusTextures.PearlShardWhite_Dropped;

        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 25;
        }

        public override void SetDefaults() {
            Item.CloneDefaults(PearlItem);
            Item.width = 6;
            Item.height = 6;
            Item.useTime = Item.WALL_PLACEMENT_USETIME;
            Item.useAnimation = 15;
            Item.createTile = ModContent.TileType<PearlsTile>();
            Item.placeStyle = PearlTileStyle;
            Item.value /= AmountPerPearl;
            Item.rare = Math.Clamp(Item.rare - 1, 0, ItemRarityID.Count);
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
                .Register();
        }
    }

    public class PearlShardBlack : PearlShardWhite {
        public override int PearlItem => ItemID.BlackPearl;
        public override int PearlTileStyle => PearlsTile.STYLE_BLACK;
        public override Texture2D DroppedSprite => AequusTextures.PearlShardBlack_Dropped;
    }

    public class PearlShardPink : PearlShardWhite {
        public override int PearlItem => ItemID.PinkPearl;
        public override int PearlTileStyle => PearlsTile.STYLE_PINK;
        public override Texture2D DroppedSprite => AequusTextures.PearlShardPink_Dropped;
    }
}

namespace Aequus.Tiles.CrabCrevice {
    public class PearlsTile : BaseGemTile {
        public const int STYLE_WHITE = 0;
        public const int STYLE_BLACK = 1;
        public const int STYLE_PINK = 2;
        public const int STYLE_BOSS_SPAWNER = 3;

        public override void SetStaticDefaults() {
            base.SetStaticDefaults();
            Main.tileObsidianKill[Type] = true;
            Main.tileShine[Type] = 400;
            Main.tileShine2[Type] = true;
            Main.tileOreFinderPriority[Type] = 110;
            Main.tileSpelunker[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileLighted[Type] = true;

            AddMapEntry(new Color(190, 200, 222), TextHelper.GetText("MapObject.Pearl"));
            AddMapEntry(new Color(105, 186, 220), TextHelper.GetText("MapObject.HypnoticPearl"));
            DustType = DustID.Glass;
            HitSound = SoundID.Shatter;
        }

        public override ushort GetMapOption(int i, int j) {
            return (ushort)(Main.tile[i, j].TileFrameX / 18 == STYLE_BOSS_SPAWNER ? 1 : 0);
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j) {
            if (Main.tile[i, j].TileFrameX / 18 == STYLE_BOSS_SPAWNER) {
                return new Item[] { new(ModContent.ItemType<HypnoticPearl>()) };
            }
            return base.GetItemDrops(i, j);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
            r = 0.12f;
            g = 0.12f;
            b = 0.12f;
            if (Main.tile[i, j].TileFrameX >= 18 * 3) {
                g += (float)Math.Sin(Main.GameUpdateCount / 30f) * 0.33f;
                b += (float)Math.Sin(Main.GameUpdateCount / 30f + MathHelper.Pi) * 0.33f;
                if (g < 0.3f)
                    g = 0.3f;
                if (b < 0.3f)
                    b = 0.3f;
            }
        }
    }
}