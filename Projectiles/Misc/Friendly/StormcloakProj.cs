using Aequus.Graphics;
using Aequus.Graphics.Primitives;
using Aequus.Particles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;

namespace Aequus.Projectiles.Misc.Friendly
{
    public class StormcloakProj : PumpinatorProj
    {
        public override string Texture => "Aequus/Assets/Explosion1";

        public override bool OnlyPushHostilePlayers => true;
        public override bool OnlyPushHostileProjectiles => true;
        public override bool PushUIObjects => false;
        public override bool PushItems => false;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 800;
            Projectile.height = 800;
            Projectile.hide = true;
            Projectile.timeLeft = 120;
        }

        public override Vector2 GetWindVelocity(Vector2 entityLocation)
        {
            var v = Projectile.DirectionTo(entityLocation).UnNaN() * 12f;
            return Vector2.Lerp(v, new Vector2(Math.Sign(v.X) * v.Length(), 0f), 0.5f);
        }

        public void PushEffects(Entity entity, Vector2 position, int width, int height)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }
            Vector2 widthMethod(float p) => new Vector2(4f) * (float)Math.Sin(p * MathHelper.Pi);
            Color colorMethod(float p) => Color.White.UseA(150) * 0.45f * (float)Math.Sin(p * MathHelper.Pi);

            var center = new Vector2(position.X + width / 2f, position.Y + height / 2f);
            var windVelocity = GetWindVelocity(center);
            for (int i = 0; i < 1; i++)
            {
                if (!Main.rand.NextBool(30) && Projectile.timeLeft < 118 && Projectile.timeLeft % 10 != 0)
                    continue;
                var prim = new TrailRenderer(TextureCache.Trail[4].Value, TrailRenderer.DefaultPass, widthMethod, colorMethod);
                float rotation = Main.rand.NextFloat(-0.1f, 0.1f);
                var v = windVelocity.RotatedBy(rotation);
                var particle = new StormcloakTrailParticle(prim, center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(-10f, 10f) - v * 5f, v,
                    scale: Main.rand.NextFloat(0.85f, 1.5f), trailLength: 10, drawDust: false);
                particle.StretchTrail(windVelocity / -4f);
                particle.rotationValue = rotation / 4f;
                particle.prim.GetWidth = (p) => widthMethod(p) * particle.Scale;
                particle.prim.GetColor = (p) => colorMethod(p) * particle.Rotation * Math.Min(particle.Scale, 1.5f);
                EffectsSystem.ParticlesAbovePlayers.Add(particle);
            }
        }

        public override void OnPushNPC(NPC npc)
        {
            PushEffects(npc, npc.position, npc.width, npc.height);
        }

        public override void OnPushProj(Projectile proj)
        {
            PushEffects(proj, proj.position, proj.width, proj.height);
        }

        public override void OnPushItem(Item item)
        {
            PushEffects(item, item.position, item.width, item.height);
        }

        public override void OnPushPlayer(Player player)
        {
            PushEffects(player, player.position, player.width, player.height);
        }
    }
}