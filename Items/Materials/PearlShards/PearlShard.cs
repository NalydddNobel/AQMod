using Aequus;
using Aequus.Tiles.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
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
        internal abstract string MapKey { get; }

        public override void SetStaticDefaults() {
            base.SetStaticDefaults();
            AequusTile.PearlsToGenerate.Add(this);
            Main.tileObsidianKill[Type] = true;
            Main.tileShine[Type] = 400;
            Main.tileShine2[Type] = true;
            Main.tileOreFinderPriority[Type] = 110;
            Main.tileSpelunker[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileLighted[Type] = true;

            AddMapEntry(MapColor, TextHelper.GetText(MapKey));
            DustType = DustID.Glass;
            HitSound = SoundID.Shatter;
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
            r = 0.12f;
            g = 0.12f;
            b = 0.12f;
        }

        public static void GrowPearl(int i, int j) {
            if (!WorldGen.genRand.NextBool(2200)) {
                return;
            }

            List<Point> p = new();
            if (!Main.tile[i + 1, j].HasTile && WorldGen.genRand.NextBool(4)) {
                p.Add(new(i + 1, j));
            }
            if (!Main.tile[i - 1, j].HasTile && WorldGen.genRand.NextBool(4)) {
                p.Add(new(i - 1, j));
            }
            if (!Main.tile[i, j + 1].HasTile && WorldGen.genRand.NextBool(4)) {
                p.Add(new(i, j + 1));
            }
            if (!Main.tile[i, j - 1].HasTile) {
                p.Add(new(i, j - 1));
            }

            if (p.Count > 0) {
                for (int k = -17; k <= 17; k++) {
                    for (int l = -20; l <= 20; l++) {
                        if (WorldGen.InWorld(i + k, j + l) && Main.tile[i + k, j + l].HasTile && TileLoader.GetTile(Main.tile[i + k, j + l].TileType) is PearlsTile) {
                            return;
                        }
                    }
                }
                var chosen = WorldGen.genRand.Next(p);
                var tileInstance = Main.rand.Next(AequusTile.PearlsToGenerate);
                if (tileInstance.CanPlace(chosen.X, chosen.Y)) {
                    WorldGen.PlaceTile(chosen.X, chosen.Y, tileInstance.Type, mute: true);
                    if (Main.tile[chosen].TileType == tileInstance.Type) {
                        int frame = 0;
                        if (WorldGen.genRand.NextBool(3)) {
                            frame = 1;
                        }
                        else if (WorldGen.genRand.NextBool(12)) {
                            frame = 2;
                        }
                        else if (WorldGen.genRand.NextBool(6)) {
                            frame = 3;
                        }
                        Main.tile[chosen.X, chosen.Y].TileFrameX = (short)(frame * 18);
                        if (Main.netMode != NetmodeID.SinglePlayer) {
                            NetMessage.SendTileSquare(-1, chosen.X, chosen.Y);
                        }
                    }
                }
            }
        }
    }
    [LegacyName("PearlsTile")]
    public class PearlsTileWhite : PearlsTile {
        internal override Color MapColor => new Color(190, 200, 222);
        internal override string MapKey => "MapObject.Pearl";
    }
    public class PearlsTileBlack : PearlsTile {
        internal override Color MapColor => new Color(124, 128, 172);
        internal override string MapKey => "MapObject.Pearl";
    }
    public class PearlsTilePink : PearlsTile {
        internal override Color MapColor => new Color(212, 136, 205);
        internal override string MapKey => "MapObject.Pearl";
    }
    public class PearlsTileHypnotic : PearlsTile {
        internal override Color MapColor => new Color(105, 186, 220);
        internal override string MapKey => "Items.HypnoticPearl.DisplayName";
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
            g += (float)Math.Sin(Main.GameUpdateCount / 30f) * 0.33f;
            b += (float)Math.Sin(Main.GameUpdateCount / 30f + MathHelper.Pi) * 0.33f;
            if (g < 0.3f)
                g = 0.3f;
            if (b < 0.3f)
                b = 0.3f;
        }
    }
}