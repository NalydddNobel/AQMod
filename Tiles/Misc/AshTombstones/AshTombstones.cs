using Aequus.Projectiles.Base;
using Aequus.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using System;
using Aequus.Particles;
using Terraria.Audio;

namespace Aequus.Tiles.Misc.AshTombstones {
    public class AshTombstone : ModItem {

        public virtual int Style => AshTombstonesTile.Style_AshTombstone;
        public virtual int RecipeItem => ItemID.Tombstone;

        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 2;
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<AshTombstonesTile>(), Style);
            Item.rare = ItemRarityID.Green;
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(RecipeItem)
                .AddIngredient(ItemID.AshBlock, 10)
                .AddTile(TileID.Hellforge)
                .Register();
        }
    }
    public class AshGraveMarker : AshTombstone {
        public override int Style => AshTombstonesTile.Style_AshGraveMarker;
        public override int RecipeItem => ItemID.GraveMarker;
    }
    public class AshCrossGraveMarker : AshTombstone {
        public override int Style => AshTombstonesTile.Style_AshCrossGraveMarker;
        public override int RecipeItem => ItemID.CrossGraveMarker;
    }
    public class AshHeadstone : AshTombstone {
        public override int Style => AshTombstonesTile.Style_AshHeadstone;
        public override int RecipeItem => ItemID.Headstone;
    }
    public class AshGravestone : AshTombstone {
        public override int Style => AshTombstonesTile.Style_AshGravestone;
        public override int RecipeItem => ItemID.Gravestone;
    }
    public class AshObelisk : AshTombstone {
        public override int Style => AshTombstonesTile.Style_AshObelisk;
        public override int RecipeItem => ItemID.Obelisk;
    }

    public class AshTombstoneProj : TombstoneProjBase {
        public override string Texture => AequusTextures.AshTombstone.Path;

        public override void SetStaticDefaults() {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            AequusProjectile.HellTombstones.Add(Type);
        }

        public override int TileType => ModContent.TileType<AshTombstonesTile>();
        public override int TileStyle => AshTombstonesTile.Style_AshTombstone;

        public string AshTombstoneText() {
            return TextHelper.GetTextValue("Deaths.AshTombstone." + Main.rand.Next(13));
        }

        public override string GetTombstoneText() {
            return base.GetTombstoneText() + "\n" + AshTombstoneText();
        }

        public override void SetDefaults() {
            base.SetDefaults();
            Projectile.friendly = true;
        }

        public override void AI() {
            if (Aequus.GetFixedBoi) {
                Projectile.damage = Math.Max(Projectile.damage, 300);
                Projectile.hostile = true;
            }
            var tileCoords = Projectile.Center.ToTileCoordinates();
            if (WorldGen.InWorld(tileCoords.X, tileCoords.Y, 5) && Main.tile[tileCoords].LiquidAmount > 0) {
                Projectile.velocity.Y -= 0.7f;
            }
            if ((int)Projectile.ai[0] == 0) {
                Projectile.ai[0] = 6f;
            }
            base.AI();
        }

        public override bool? CanDamage() {
            return Projectile.velocity.Length() > 1f ? null : false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            if (Math.Abs(Projectile.velocity.Y) < 0.2f) {
                Projectile.ai[0] = -1f;
                return base.OnTileCollide(oldVelocity);
            }
            Projectile.ai[0]--;
            if (Projectile.ai[0] <= 0f) {
                Projectile.ai[0] = -1f;
            }
            if (Main.myPlayer == Projectile.owner) {
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center, 
                    Vector2.Normalize(Projectile.velocity) * 0.1f,
                    ModContent.ProjectileType<AshTombstoneExplosion>(),
                    Projectile.damage,
                    Projectile.knockBack,
                    Projectile.owner
                );
            }
            if (Projectile.velocity.X != oldVelocity.X) {
                Projectile.velocity.X = oldVelocity.X * -0.95f;
            }
            if (Projectile.velocity.Y != oldVelocity.Y) {
                Projectile.velocity.Y = oldVelocity.Y * -0.95f;
            }
            if (Main.player[Projectile.owner].active) {
                var toPlayer = Main.player[Projectile.owner].Center - Projectile.Center;
                Projectile.velocity.X = Math.Abs(Projectile.velocity.X) * Math.Sign(toPlayer.X);
            }
            return false;
        }

        public void DrawGrave(Vector2 drawCoordinates, Texture2D tileTexture, float rotation, Color color) {
            for (int i = 0; i < 2; i++) {
                for (int j = 0; j < 2; j++) {
                    Rectangle frame = new(18 * (TileStyle * 2 + i), 18 * j, 16, j == 1 ? 18 : 16);
                    var tileSegmentCoords = drawCoordinates + new Vector2(frame.Width * (i - 1), frame.Height * (j - 1)).RotatedBy(rotation);
                    Main.spriteBatch.Draw(
                        tileTexture,
                        tileSegmentCoords,
                        frame,
                        color,
                        rotation,
                        Vector2.Zero,
                        Projectile.scale,
                        SpriteEffects.None, 0f
                    );
                }
            }
        }

        public override void PostDraw(Color lightColor) {
            var tileTexture = AequusTextures.AshTombstonesTile.Value;
            var glowTexture = AequusTextures.AshTombstonesTile_Glow.Value;
            int trailLength = ProjectileID.Sets.TrailCacheLength[Type];
            var positionOffset = Projectile.Size / 2f;
            for (int i = 0; i < trailLength; i++) {
                float progress = Helper.CalcProgress(trailLength, i);
                DrawGrave(Projectile.oldPos[i] + positionOffset - Main.screenPosition, tileTexture, Projectile.oldRot[i], Color.Orange with { A = 0 } * progress);
            }
            var drawCoordinates = Projectile.Center - Main.screenPosition;
            DrawGrave(drawCoordinates, tileTexture, Projectile.rotation, lightColor);
            DrawGrave(drawCoordinates, glowTexture, Projectile.rotation, Color.White);
        }
    }
    public class AshTombstoneExplosion : ModProjectile {
        public override string Texture => AequusTextures.Explosion1.Path;

        public override void SetStaticDefaults() {
            Main.projFrames[Type] = 7;
        }

        public override void SetDefaults() {
            Projectile.DefaultToExplosion(90, DamageClass.Default, 20);
        }

        public override Color? GetAlpha(Color lightColor) {
            return new Color(255, 128, 60, 100);
        }

        public override void AI() {
            if (Projectile.frame == 0 && Main.netMode != NetmodeID.Server) {
                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);
                ScreenShake.SetShake(40f, 0.85f, Projectile.Center);
                for (int i = 0; i < 20; i++) {
                    var v = Main.rand.NextVector2Unit();
                    ParticleSystem.New<BloomParticle>(ParticleLayer.BehindPlayers).Setup(
                        Projectile.Center + v * Main.rand.NextFloat(16f), 
                        v * Main.rand.NextFloat(3f, 12f),
                        new Color(255, 128, 100, 60), 
                        new Color(60, 10, 0, 0), 
                        1.25f,
                        0.2f
                    );
                }
            }
            if (Aequus.GetFixedBoi) {
                Projectile.hostile = true;
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 2) {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Type]) {
                    Projectile.hide = true;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            Projectile.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out int _);
            Main.spriteBatch.Draw(texture, Projectile.position + offset - Main.screenPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
    public class AshGraveMarkerProj : AshTombstoneProj {
        public override int TileStyle => AshTombstonesTile.Style_AshGraveMarker;
    }
    public class AshCrossGraveMarkerProj : AshTombstoneProj {
        public override int TileStyle => AshTombstonesTile.Style_AshCrossGraveMarker;
    }
    public class AshHeadstoneProj : AshTombstoneProj {
        public override int TileStyle => AshTombstonesTile.Style_AshHeadstone;
    }
    public class AshGravestoneProj : AshTombstoneProj {
        public override int TileStyle => AshTombstonesTile.Style_AshGravestone;
    }
    public class AshObeliskProj : AshTombstoneProj {
        public override int TileStyle => AshTombstonesTile.Style_AshObelisk;
    }

    [LegacyName("Tombstones", "AshTombstones")]
    public class AshTombstonesTile : ModTile {
        public const int Style_AshTombstone = 0;
        public const int Style_AshGraveMarker = 1;
        public const int Style_AshCrossGraveMarker = 2;
        public const int Style_AshHeadstone = 3;
        public const int Style_AshGravestone = 4;
        public const int Style_AshObelisk = 5;

        public static int numAshTombstones;

        public override void SetStaticDefaults() {
            Main.tileSign[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.CoordinateHeights = new int[2] { 16, 18 };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(100, 20, 10, 255), TextHelper.GetText("MapObject.AshTombstone"));
            DustType = 37;
            AdjTiles = new int[] { TileID.Tombstones };
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) {
            return true;
        }

        public override bool RightClick(int i, int j) {
            return true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num) {
            num = fail ? 1 : 3;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY) {
            Sign.KillSign(i, j);
        }

        public override void MouseOverFar(int i, int j) {
            MouseOver(i, j);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
            r = 0.04f;
            g = 0.02f;
            b = 0.001f;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch) {
            var frame = new Rectangle(Main.tile[i, j].TileFrameX, Main.tile[i, j].TileFrameY, 16, 16);
            if (Main.tile[i, j].TileFrameY >= 18) {
                frame.Y = 18;
            }
            Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture + "_Glow", AssetRequestMode.ImmediateLoad).Value, new Vector2(i * 16f - Main.screenPosition.X, j * 16f - Main.screenPosition.Y) + Helper.TileDrawOffset,
                frame, new Color(200, 100, 100, 0) * Helper.Wave(Main.GlobalTimeWrappedHourly * 3f, 0.2f, 0.5f), 0f, new Vector2(0f, 0f), 1f, SpriteEffects.None, 0f);
        }
    }
}