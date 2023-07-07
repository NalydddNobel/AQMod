using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Misc.CrownOfBlood {
    internal class ShinyStoneHealingAura : ModProjectile {
        public override void SetDefaults() {
            Projectile.DefaultToActor();
            Projectile.width = 240;
            Projectile.height = 240;
            Projectile.alpha = 255;
        }

        public override void AI() {
            Projectile.timeLeft = 2;
            if ((int)Projectile.ai[0] == 1) {
                Projectile.Opacity -= 0.03f;
                if (Projectile.Opacity <= 0f) {
                    Projectile.Kill();
                }
                return;
            }

            Projectile.velocity = Vector2.Zero;
            Projectile.Opacity += 0.03f;
            if (Main.player[Projectile.owner].velocity != Vector2.Zero) {
                Projectile.ai[0] = 1f;
                Projectile.netUpdate = true;
                return;
            }
            else {
                Projectile.Center = Main.player[Projectile.owner].Center;
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) {
            behindNPCsAndTiles.Add(index);
            overPlayers.Add(index);
            Projectile.hide = false;
        }

        public override bool PreDraw(ref Color lightColor) {
            var drawColor = Color.Red with { A = 0 } * Projectile.Opacity;
            if (Projectile.hide) {
                var texture = TextureAssets.Projectile[Type].Value;
                var drawPosition = Projectile.Center + new Vector2(0f, Main.player[Projectile.owner].height / 2f) - Main.screenPosition;
                var origin = texture.Size() / 2f;
                Main.EntitySpriteDraw(texture, drawPosition, null, drawColor, 0f, origin, 1f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(texture, drawPosition, null, drawColor * (1f - Main.GlobalTimeWrappedHourly % 1f), 0f, origin, new Vector2(1f + Main.GlobalTimeWrappedHourly % 1f * 3f, 1f + Main.GlobalTimeWrappedHourly % 1f), SpriteEffects.None, 0);
                return false;
            }

            var bloom = AequusTextures.Bloom6;
            Main.EntitySpriteDraw(bloom, Projectile.Center - Main.screenPosition, null, drawColor * Helper.Wave(Main.GlobalTimeWrappedHourly * 5f, 0.2f, 0.5f), 0f, bloom.Size() / 2f, 2f, SpriteEffects.None, 0);
            Projectile.hide = true;
            return false;
        }
    }
}
