using Aequus.Common.Preferences;
using Aequus.Graphics.Primitives;
using Aequus.NPCs.Friendly.Drones;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Misc
{
    public class GunnerDroneProj : ModProjectile
    {
        public override string Texture => Aequus.BlankTexture;

        public TrailRenderer prim;
        public TrailRenderer smokePrim;

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.timeLeft *= 5;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.manualDirectionChange = true;
        }

        public override void AI()
        {
            Projectile.alpha += 20;
            if (Projectile.alpha > 255)
                Projectile.Kill();
            var npc = Main.npc[(int)Projectile.ai[0]];
            if (!npc.active)
            {
                Projectile.Kill();
                return;
            }
            Projectile.Center = npc.Center;
            Projectile.rotation = npc.rotation;
            Projectile.direction = npc.spriteDirection;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = float.NaN;
            var normal = new Vector2(1f, 0f).RotatedBy(Projectile.rotation);
            var end = Projectile.Center + normal * 800f * Projectile.direction;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, end, 10f * Projectile.scale, ref _);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Projectile.damage = (int)(Projectile.damage * 0.8f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var npc = Main.npc[(int)Projectile.ai[0]];
            if (!npc.active || npc.ModNPC is not TownDroneBase townDrone)
            {
                return false;
            }

            var drawPos = Projectile.Center - Main.screenPosition;
            var drawColor = new Color(10, 200, 80, 0);
            var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f);
            var n = Projectile.rotation.ToRotationVector2();
            var arr = new Vector2[] {
                    drawPos,
                    drawPos + n * 800f * Projectile.direction,
                    drawPos + n * 800f * 2f * Projectile.direction, };

            if (prim == null)
            {
                prim = new TrailRenderer(Textures.Trail[0].Value, TrailRenderer.DefaultPass, (p) => new Vector2(10f), (p) => townDrone.GetPylonColor().UseA(0) * 1.4f * (float)Math.Pow(1f - p, 2f) * Projectile.Opacity, obeyReversedGravity: false, worldTrail: false);
            }

            if (smokePrim == null)
            {
                smokePrim = new ForceCoordTrailRenderer(Textures.Trail[3].Value, TrailRenderer.DefaultPass, (p) => new Vector2(4f), (p) => townDrone.GetPylonColor().UseA(0) * (1f - p) * 0.4f * Projectile.Opacity, obeyReversedGravity: false, worldTrail: false)
                {
                    coord1 = 0f,
                    coord2 = 1f
                };
            }
            if (Main.LocalPlayer.gravDir == -1)
            {
                AequusHelpers.ScreenFlip(arr);
            }
            var smokeLineColor = drawColor * ((float)Math.Sin(Main.GlobalTimeWrappedHourly * 12f) + 2f);
            int amount = (int)(5 * (ClientConfig.Instance.HighQuality ? 1f : 0.5f));
            var initialArr = new Vector2[amount];
            var center = Projectile.Center;

            initialArr[0] = arr[0];
            for (int i = 1; i < amount; i++)
            {
                initialArr[i] = drawPos + new Vector2(60f / amount * i * -Projectile.direction, 0f);
            }
            if (Main.LocalPlayer.gravDir == -1)
            {
                AequusHelpers.ScreenFlip(initialArr);
            }

            prim.Draw(arr);
            prim.Draw(arr);
            smokePrim.Draw(arr);
            return false;
        }
    }
}