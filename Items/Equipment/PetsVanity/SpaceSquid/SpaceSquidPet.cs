﻿using Aequus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.PetsVanity.SpaceSquid {
    public class SpaceSquidPet : ModProjectile {
        public static Asset<Texture2D> Glow { get; private set; }

        public override void Load() {
            if (!Main.dedServ) {
                Glow = ModContent.Request<Texture2D>(this.GetPath() + "_Glow", AssetRequestMode.ImmediateLoad);
            }
        }

        public override void Unload() {
            Glow = null;
        }

        public override void SetStaticDefaults() {
            Main.projFrames[Projectile.type] = 5;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.CharacterPreviewAnimations[Type] = ProjectileID.Sets.SimpleLoop(0, 5, 6).WithOffset(new(0f, -14f));
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
            Helper.UpdateProjActive<SpaceSquidBuff>(Projectile);
            var gotoPos = GetIdlePosition();
            Projectile.direction = player.direction;
            var center = Projectile.Center;
            float distance = (center - gotoPos).Length();

            if (Projectile.Distance(Main.player[Projectile.owner].Center) > 2000f) {
                Projectile.Center = Main.player[Projectile.owner].Center;
                Projectile.velocity *= 0.1f;
            }

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
                Projectile.velocity = (center - gotoPos) / -16f;
            }

            Projectile.LoopingFrame(6);

            Projectile.rotation = Projectile.velocity.X * 0.1f;
        }
        private Vector2 GetIdlePosition() {
            int dir = Main.player[Projectile.owner].direction;
            float y = -20f;
            var counts = Main.player[Projectile.owner].ownedProjectileCounts;
            if (counts[ProjectileID.SuspiciousTentacle] > 0 || counts[ProjectileID.DD2PetGhost] > 0
                || counts[ProjectileID.MagicLantern] > 0 || counts[ProjectileID.PumpkingPet] > 0) {
                dir = -dir;
            }
            if (counts[ProjectileID.GolemPet] > 0) {
                y -= 36;
            }
            return Main.player[Projectile.owner].Center + new Vector2((Main.player[Projectile.owner].width + 16f) * dir, y);
        }

        public override bool PreDraw(ref Color lightColor) {
            var texture = TextureAssets.Projectile[Type].Value;
            var frame = texture.Frame(verticalFrames: Main.projFrames[Type], frameY: Projectile.frame);
            var drawCoordinates = Projectile.Center - Main.screenPosition;
            var origin = frame.Size() / 2f;
            var effects = Projectile.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            var color1 = Color.Blue with { A = 0 } * 0.3f;
            var color2 = Color.Cyan with { A = 0 } * 0.5f;
            for (int i = 0; i < 3; i++) {
                var v = (i * MathHelper.TwoPi / 3f + Main.GlobalTimeWrappedHourly).ToRotationVector2();
                Main.EntitySpriteDraw(
                    texture,
                    drawCoordinates + v * Projectile.scale * Helper.Wave(Main.GlobalTimeWrappedHourly * 2.5f, 2f, 6f),
                    frame,
                    Color.Lerp(color1, color2, Helper.Wave(Main.GlobalTimeWrappedHourly * 6f + i * MathHelper.TwoPi / 3f, 0f, 1f)),
                    Projectile.rotation,
                    origin,
                    Projectile.scale,
                    effects,
                    0
                );
            }

            Main.EntitySpriteDraw(texture, drawCoordinates, frame, lightColor.MaxRGBA(24), Projectile.rotation, origin, Projectile.scale, effects, 0);
            Main.EntitySpriteDraw(Glow.Value, drawCoordinates, frame, Color.White, Projectile.rotation, origin, Projectile.scale, effects, 0);
            return false;
        }
    }
}