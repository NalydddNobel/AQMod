using Aequus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Misc.Pets
{
    public class SpaceSquidPet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.manualDirectionChange = true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            AequusHelpers.UpdateProjActive(Projectile, ref player.GetModPlayer<AequusPlayer>().spaceSquidPet);
            Vector2 gotoPos = GetIdlePosition();
            Projectile.direction = player.direction;
            var center = Projectile.Center;
            float distance = (center - gotoPos).Length();

            float snapLength = 0.1f;
            if ((int)Projectile.ai[0] == 1)
            {
                if (distance > snapLength)
                {
                    Projectile.ai[0] = 0f;
                }
                else
                {
                    snapLength = 24f;
                }
            }
            if (distance < snapLength)
            {
                Projectile.velocity = Vector2.Zero;
                Projectile.Center = gotoPos;
                Projectile.ai[0] = 1f;
            }
            else
            {
                Projectile.velocity = (center - gotoPos) / -16f;
            }

            Projectile.LoopingFrame(6);

            Projectile.rotation = Projectile.velocity.X * 0.1f;
        }
        private Vector2 GetIdlePosition()
        {
            int dir = Main.player[Projectile.owner].direction;
            float y = -20f;
            var counts = Main.player[Projectile.owner].ownedProjectileCounts;
            if (counts[ProjectileID.SuspiciousTentacle] > 0 || counts[ProjectileID.DD2PetGhost] > 0
                || counts[ProjectileID.MagicLantern] > 0 || counts[ProjectileID.PumpkingPet] > 0)
            {
                dir = -dir;
            }
            if (counts[ProjectileID.GolemPet] > 0)
            {
                y -= 36;
            }
            return Main.player[Projectile.owner].Center + new Vector2((Main.player[Projectile.owner].width + 16f) * dir, y);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Type].Value;
            var frame = texture.Frame(verticalFrames: Main.projFrames[Type], frameY: Projectile.frame);
            Vector2 center = Projectile.Center;
            var drawCoordinates = center - Main.screenPosition;
            var origin = frame.Size() / 2f;
            var effects = Projectile.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            var circular = AequusHelpers.CircularVector(3, Main.GlobalTimeWrappedHourly);
            for (int i = 0; i < circular.Length; i++)
            {
                Main.EntitySpriteDraw(texture, drawCoordinates + circular[i] * Projectile.scale * AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 2.5f, 2f, 6f), frame,
                    Color.Lerp(Color.Blue.UseA(0) * 0.3f, Color.Cyan.UseA(0) * 0.5f, AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 6f + i * MathHelper.TwoPi / 3f, 0f, 1f)), Projectile.rotation, origin, Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture, drawCoordinates, frame, Color.White, Projectile.rotation, origin, Projectile.scale, effects, 0);
            return false;
        }
    }
}