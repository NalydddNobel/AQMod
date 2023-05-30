using Aequus.Buffs.Minion;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Summon.Misc {
    public class GravetenderWisp : MinionBase
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 5;
            this.SetTrail(8);
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 30;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, 40);
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void AI()
        {
            if (!Helper.UpdateProjActive<GravetenderMinionBuff>(Projectile))
            {
                return;
            }

            Projectile.LoopingFrame(7);

            Projectile.rotation = Projectile.velocity.X * 0.075f;
            var aequus = Main.player[Projectile.owner].Aequus();
            var gotoPosition = aequus.gravetenderGhost > -1 ?
                Main.npc[aequus.gravetenderGhost].Center + new Vector2(0f, -Main.npc[aequus.gravetenderGhost].height - Projectile.height)
                : DefaultIdlePosition();
            var velocityMin = 4f;
            var diff = gotoPosition - Projectile.Center;
            if ((Main.player[Projectile.owner].Center - Projectile.Center).Length() > 2000f)
            {
                Projectile.Center = Main.player[Projectile.owner].Center;
                Projectile.velocity *= 0.1f;
                aequus.gravetenderGhost = -1;
                diff = Vector2.UnitY;
            }
            var ovalDiff = new Vector2(diff.X, diff.Y * 3f);
            float ovalLength = ovalDiff.Length();
            if (ovalLength > 28f)
            {
                var velocity = diff / 50f;
                if (velocity.Length() < velocityMin)
                {
                    velocity = Vector2.Normalize(velocity).UnNaN() * 4f;
                }
                if (Math.Sign(velocity.X) != Math.Sign(Projectile.velocity.X))
                {
                    Projectile.velocity.X *= 0.99f;
                }

                Projectile.velocity = Vector2.Lerp(Projectile.velocity, velocity, 0.01f);
            }
            else
            {
                Projectile.velocity *= 0.95f;
            }
            if (Projectile.numUpdates == -1)
            {
                Projectile.position += (aequus.gravetenderGhost > -1 ? Main.npc[aequus.gravetenderGhost].velocity * Main.npc[aequus.gravetenderGhost].GetSpeedStats() : Main.player[Projectile.owner].velocity) * 0.97f;
            }

            Lighting.AddLight(Projectile.Center, new Vector3(0.2f, 0f, 0.05f));

            if (aequus.gravetenderGhost != -1)
            {
                Projectile.spriteDirection = Math.Sign(Main.npc[aequus.gravetenderGhost].velocity.X);
            }
            else
            {
                Projectile.spriteDirection = Main.player[Projectile.owner].direction;
            }
        }

        public override Vector2 IdlePosition(Player player, int leader, int minionPos, int count)
        {
            return base.IdlePosition(player, leader, minionPos, count) + new Vector2((-40f + player.width / 2f) * player.direction, -16f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetDrawInfo(out var t, out var off, out var frame, out var origin, out int trailLength);

            var c = Projectile.GetAlpha(lightColor) * Projectile.Opacity * Projectile.scale;

            off -= Main.screenPosition;
            origin.Y += 6f;
            var effects = Projectile.GetSpriteEffect();
            Main.instance.PrepareDrawnEntityDrawing(Projectile, Main.player[Projectile.owner].cHead, null);
            for (int i = 0; i < trailLength; i++)
            {
                float p = Helper.CalcProgress(trailLength, i);
                Main.EntitySpriteDraw(t, Projectile.oldPos[i] + off, frame, c * 0.6f * p * p, Projectile.oldRot[i], origin, Projectile.scale * (0.8f + 0.2f * p), effects, 0);
            }
            foreach (var v in Helper.CircularVector(4))
            {
                float f = Helper.Wave(Main.GlobalTimeWrappedHourly * 2.5f, 2f, 4f);
                Main.EntitySpriteDraw(t, Projectile.position + off + v * (f - 2f), frame, c * 0.1f, Projectile.rotation, origin, Projectile.scale, effects, 0);
                Main.EntitySpriteDraw(t, Projectile.position + off + v * f, frame, c * 0.1f, Projectile.rotation, origin, Projectile.scale, effects, 0);
            }
            Main.EntitySpriteDraw(t, Projectile.position + off, frame, c, Projectile.rotation, origin, Projectile.scale, effects, 0);
            return false;
        }
    }
}