using Aequus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Pets.Drone {
    public class DronePet : ModProjectile {
        public override string Texture => AequusTextures.PhysicistPet_PhysicistNPC.Path;

        public override void SetStaticDefaults() {
            Main.projFrames[Type] = 4;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }

        public override void SetDefaults() {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.manualDirectionChange = true;
        }

        public override void AI() {
            Player player = Main.player[Projectile.owner];
            Helper.UpdateProjActive<DroneBuff>(Projectile);
            var gotoPos = GetIdlePosition();
            Projectile.direction = player.direction;
            var center = Projectile.Center;
            float distance = (center - gotoPos).Length();

            if (Projectile.Distance(Main.player[Projectile.owner].Center) > 2000f) {
                Projectile.Center = Main.player[Projectile.owner].Center;
                Projectile.velocity *= 0.1f;
            }

            Projectile.localAI[0]++;
            float snapLength = 0.1f;
            if ((int)Projectile.ai[0] == 1) {
                if (distance > snapLength) {
                    Projectile.ai[0] = 0f;
                }
                else {
                    snapLength = 24f;
                }
            }
            if (distance < snapLength) {
                Projectile.velocity = Vector2.Zero;
                Projectile.Center = gotoPos;
                Projectile.ai[0] = 1f;
            }
            else {
                Projectile.velocity = (center - gotoPos) / -32f;
            }

            Projectile.LoopingFrame(6);

            Projectile.rotation = Projectile.velocity.X * 0.1f;
            Lighting.AddLight(Projectile.Center, new Vector3(0.6f, 1.33f, 1f));
        }
        private Vector2 GetIdlePosition() {
            int dir = Main.player[Projectile.owner].direction;
            float y = -20f;
            return Main.player[Projectile.owner].Center + new Vector2((Main.player[Projectile.owner].width + 16f) * dir, y);
        }

        public override bool PreDraw(ref Color lightColor) {
            if (Aequus.GameWorldActive && Projectile.localAI[0] > 2f) {
                Projectile.localAI[0] = 0f;
                ScanRareTiles();
                ScanEnemies();
            }
            var texture = TextureAssets.Projectile[Type].Value;
            var frame = texture.Frame(verticalFrames: Main.projFrames[Type], frameY: Projectile.frame);
            Vector2 center = Projectile.Center;
            var drawCoordinates = center - Main.screenPosition;
            var origin = frame.Size() / 2f;
            var effects = Projectile.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Main.EntitySpriteDraw(texture, drawCoordinates, frame, lightColor.MaxRGBA(24), Projectile.rotation, origin, Projectile.scale, effects, 0);
            Main.EntitySpriteDraw(AequusTextures.PhysicistPet_Glow_PhysicistNPC, drawCoordinates, frame, Color.White, Projectile.rotation, origin, Projectile.scale, effects, 0);
            return false;
        }

        public void ScanEnemies() {
            for (int i = 0; i < Main.maxNPCs; i++) {
                if (Main.npc[i].active && Main.npc[i].rarity > 0) {
                    var p = Main.npc[i].getRect().ClosestPointInRect(Projectile.Center);
                    if (Projectile.Distance(p) < 256f * 2f) {
                        Projectile.localAI[1] = Main.npc[i].rarity * 200f;
                        for (int k = 0; k <= Main.npc[i].width; k += 4) {
                            SearchDust(new Vector2(Main.npc[i].position.X + k, Main.npc[i].position.Y), 0, -1);
                            SearchDust(new Vector2(Main.npc[i].position.X + k, Main.npc[i].position.Y + Main.npc[i].height), 0, 1);
                        }
                        for (int k = 0; k <= Main.npc[i].height; k += 4) {
                            SearchDust(new Vector2(Main.npc[i].position.X, Main.npc[i].position.Y + k), -1, 0);
                            SearchDust(new Vector2(Main.npc[i].position.X + Main.npc[i].width, Main.npc[i].position.Y + k), 1, 0);
                        }
                    }
                }
            }
        }

        public void ScanRareTiles() {
            int sizeHalf = 16;
            var r = Helper.TileRectangle(Projectile.Center, sizeHalf, sizeHalf).Fluffize(10);
            for (int i = r.X; i < r.X + r.Width; i++) {
                for (int j = r.Y; j < r.Y + r.Height; j++) {
                    var tile = Main.tile[i, j];
                    if (tile.HasTile) {
                        if (Main.tileOreFinderPriority[tile.TileType] > 0) {
                            Projectile.localAI[1] = Math.Min(Main.tileOreFinderPriority[tile.TileType], 800f);
                        }
                        else if (Main.IsTileSpelunkable(i, j)) {
                            Projectile.localAI[1] = 550f;
                        }
                        else {
                            continue;
                        }
                        var clr = Color.Lerp(Color.White, Color.Yellow, Projectile.localAI[1] / 800f).ToVector3();
                        clr *= 1f - Vector2.Distance(Projectile.Center, new Vector2(i * 16f + 8f, j * 16f + 8f)) / (sizeHalf * 16f);

                        Lighting.AddLight(i, j, clr.X, clr.Y, clr.Z);
                        if (!MatchesMe(tile, Main.tile[i, j - 1])) {
                            for (int k = 0; k <= 4; k++) {
                                SearchDust(new Vector2(i * 16f + k * 4f, j * 16f), 0, -1);
                            }
                        }
                        if (!MatchesMe(tile, Main.tile[i, j + 1])) {
                            for (int k = 0; k <= 4; k++) {
                                SearchDust(new Vector2(i * 16f + k * 4f, j * 16f + 16f), 0, 1);
                            }
                        }
                        if (!MatchesMe(tile, Main.tile[i + 1, j])) {
                            for (int k = 0; k <= 4; k++) {
                                SearchDust(new Vector2(i * 16f + 16f, j * 16f + k * 4f), 1, 0);
                            }
                        }
                        if (!MatchesMe(tile, Main.tile[i - 1, j])) {
                            for (int k = 0; k <= 4; k++) {
                                SearchDust(new Vector2(i * 16f, j * 16f + k * 4f), -1, 0);
                            }
                        }
                    }
                }
            }
        }
        public void SearchDust(Vector2 where, int x, int y) {
            if (!Main.rand.NextBool(3 + (int)Vector2.Distance(Projectile.Center, where)))
                return;
            var v = new Vector2(x, y) * Main.rand.Next(8, 16);
            var d = Dust.NewDustPerfect(where, DustID.MinecartSpark, -v * 0.025f, newColor: Color.Lerp(Color.White, Color.Yellow, Projectile.localAI[1] / 800f).UseA(100));
            d.frame.Y = 59;
            d.noGravity = true;
            d.fadeIn = d.scale + 0.33f;
            d.noLight = true;
            d.noLightEmittence = true;
        }
        public bool MatchesMe(Tile tile, Tile tile2) {
            return tile2.HasTile && tile2.TileType == tile.TileType;
        }
    }
}