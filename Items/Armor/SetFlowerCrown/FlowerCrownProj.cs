using Aequus;
using Aequus.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Armor.SetFlowerCrown {
    public class FlowerCrownProj : ModProjectile {
        public override void SetStaticDefaults() {
            Main.projFrames[Projectile.type] = 3;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            PushableEntities.AddProj(Type);
        }

        public override void SetDefaults() {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.scale = 0.8f;
            Projectile.timeLeft = 600;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override bool? CanCutTiles() {
            return false;
        }

        public override bool? CanHitNPC(NPC target) {
            return NPCID.Sets.CountsAsCritter[target.type] ? false : null;
        }

        public override void AI() {
            if ((int)Projectile.ai[0] == 0) {
                Projectile.ai[0] = 1f;
                Projectile.timeLeft += Main.rand.Next(-60, 60);
                Projectile.scale += Main.rand.NextFloat(-0.1f, 0.05f);
                Projectile.rotation = Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi);
                Projectile.netUpdate = true;
            }
            if ((int)Projectile.ai[0] == 2) {
                if (Projectile.timeLeft > 240) {
                    Projectile.timeLeft = 240;
                }
                if (Projectile.timeLeft < 85) {
                    Projectile.scale -= 0.0055555f;
                    Projectile.gfxOffY += 0.01f;
                }
            }
            else {
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            Projectile.velocity = Vector2.Zero;
            Projectile.damage = 0;
            Projectile.tileCollide = false;
            Projectile.ai[0] = 2f;
            return false;
        }

        public override bool PreDraw(ref Color lightColor) {
            var texture = TextureAssets.Projectile[Type].Value;
            var frame = texture.Frame(verticalFrames: Main.projFrames[Projectile.type], frameY: Projectile.frame);
            var origin = frame.Size() / 2f;
            var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f + Projectile.gfxOffY) - Main.screenPosition;

            int trailLength = ProjectileID.Sets.TrailCacheLength[Projectile.type];
            for (int i = 0; i < trailLength; i++) {
                float progress = 1f / trailLength * i;
                Main.EntitySpriteDraw(texture, Projectile.oldPos[i] + offset, frame, lightColor * (1f - progress) * 0.5f, Projectile.rotation, origin, Projectile.scale * (1f - progress), SpriteEffects.None, 0);
            }
            Main.EntitySpriteDraw(texture, Projectile.position + offset, frame, lightColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}