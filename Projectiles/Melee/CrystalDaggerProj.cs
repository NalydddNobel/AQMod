using Aequus.Items.Weapons.Melee;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Melee
{
    public class CrystalDaggerProj : ModProjectile
    {
        public override string Texture => AequusHelpers.GetPath<CrystalDagger>();

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
            Projectile.idStaticNPCHitCooldown = 8;
            Projectile.manualDirectionChange = true;
            Projectile.ignoreWater = true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return lightColor.MaxRGBA(120);
        }

        public override void AI()
        {
            var player = Main.player[Projectile.owner];
            float speedMultiplier = 1f + (1f - player.meleeSpeed);
            if (stabLength == 0f)
                stabLength = 145f * speedMultiplier;
            var playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
            Projectile.direction = player.direction;
            player.heldProj = Projectile.whoAmI;
            player.itemTime = player.itemAnimation;
            Projectile.position.X = playerCenter.X - Projectile.width / 2;
            Projectile.position.Y = playerCenter.Y - Projectile.height / 2;
            float lerpAmount = MathHelper.Clamp(1f - speedMultiplier, 0.1f, 1f);
            if (!player.frozen && !player.stoned)
            {
                if ((int)Projectile.ai[0] == 0)
                {
                    Projectile.ai[0] = 25f;
                    Projectile.ai[1] = Main.rand.NextFloat(-0.3f, 0.3f);
                    if (Main.myPlayer == player.whoAmI && lerpAmount > 0f)
                        Projectile.velocity = Vector2.Normalize(Main.MouseWorld - playerCenter).RotatedBy(Projectile.ai[1]) * Projectile.ai[0];
                    Projectile.netUpdate = true;
                }
                if (player.itemAnimation < player.itemAnimationMax / 3f)
                {
                    Projectile.ai[0] = MathHelper.Lerp(Projectile.ai[0], 0f, MathHelper.Clamp(lerpAmount + 0.55f, 0.8f, 1f));
                }
                else
                {
                    var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(175, 200, 220, 80) * Main.rand.NextFloat(0.6f, 1f), 0.8f);
                    d.velocity *= 0.1f;
                    d.velocity += Vector2.Normalize(-Projectile.velocity) * 1.35f;
                    Projectile.ai[0] = MathHelper.Lerp(Projectile.ai[0], stabLength, MathHelper.Clamp(lerpAmount + 0.55f, 0.8f, 1f));
                }
            }
            if (player.itemAnimation == 0)
                Projectile.Kill();
            if (Main.myPlayer == player.whoAmI && lerpAmount > 0f)
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(Main.MouseWorld - playerCenter).RotatedBy(Projectile.ai[1]) * Projectile.ai[0], lerpAmount * 0.4f);
            Projectile.direction = Projectile.velocity.X <= 0f ? -1 : 1;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            player.ChangeDir(Projectile.direction);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Type].Value;
            var drawColor = Projectile.GetAlpha(lightColor);
            var swordTip = Projectile.Center/* - Vector2.Normalize(Projectile.velocity) * (Projectile.Size.Length() / 2f)*/;
            var origin = new Vector2(texture.Width, 0f);
            var effects = SpriteEffects.None;

            foreach (var v in AequusHelpers.CircularVector(4, Main.GlobalTimeWrappedHourly))
            {
                Main.EntitySpriteDraw(texture, swordTip + v * 2f - Main.screenPosition, null, new Color(10, 60, 100, 0), Projectile.rotation, origin, Projectile.scale, effects, 0);
            }
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