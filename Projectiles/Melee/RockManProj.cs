using Aequus.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Melee
{
    public class RockManProj : ModProjectile
    {
        public override string Texture => AequusHelpers.GetPath<RockMan>();

        public float stabLength;

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.alpha = 0;
            Projectile.hide = true;
            Projectile.ownerHitCheck = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 24;
            Projectile.manualDirectionChange = true;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            var player = Main.player[Projectile.owner];
            float speedMultiplier = Main.player[Projectile.owner].GetAttackSpeed(DamageClass.Melee);
            if (stabLength == 0f)
            {
                stabLength = 32f * speedMultiplier;
                Projectile.netUpdate = true;
            }
            var playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
            Projectile.direction = player.direction;
            player.heldProj = Projectile.whoAmI;
            player.itemTime = player.itemAnimation;
            Projectile.position.X = playerCenter.X - Projectile.width / 2;
            Projectile.position.Y = playerCenter.Y - Projectile.height / 2;
            if (!player.frozen && !player.stoned)
            {
                if ((int)Projectile.ai[0] == 0)
                {
                    Projectile.ai[0] = 25f;
                    Projectile.velocity = Vector2.Normalize(Projectile.velocity).UnNaN() * Projectile.ai[0];
                    AequusHelpers.MeleeScale(Projectile);
                    Projectile.netUpdate = true;
                }
                Projectile.ai[0] = 1f + (float)Math.Sin(player.itemAnimation / (float)player.itemAnimationMax * MathHelper.Pi) * stabLength;
                Projectile.ai[0] += 25;
                //if (player.itemAnimation < player.itemAnimationMax / 3f)
                //{
                //    Projectile.ai[0] = MathHelper.Lerp(Projectile.ai[0], 0f, MathHelper.Clamp(lerpAmount + 0.55f, 0.8f, 1f));
                //}
                //else
                //{
                //    Projectile.ai[0] = MathHelper.Lerp(Projectile.ai[0], stabLength, MathHelper.Clamp(lerpAmount + 0.55f, 0.8f, 1f));

                //    var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(175, 200, 220, 80) * Main.rand.NextFloat(0.6f, 1f), 0.8f);
                //    d.velocity *= 0.1f;
                //    d.velocity += Vector2.Normalize(-Projectile.velocity) * 1.35f;
                //}
            }
            if (player.itemAnimation == 0)
                Projectile.Kill();
            Projectile.velocity = Vector2.Normalize(Projectile.velocity).UnNaN() * Projectile.ai[0];
            Projectile.direction = Projectile.velocity.X <= 0f ? -1 : 1;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            player.ChangeDir(Projectile.direction);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Type].Value;
            var drawColor = Projectile.GetAlpha(lightColor);
            var swordTip = Projectile.Center;
            var origin = new Vector2(texture.Width, 0f);
            var effects = SpriteEffects.None;
            Main.EntitySpriteDraw(texture, swordTip - Main.screenPosition, null, drawColor, Projectile.rotation, origin, Projectile.scale, effects, 0);
            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(stabLength);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            stabLength = reader.ReadSingle();
        }
    }
}