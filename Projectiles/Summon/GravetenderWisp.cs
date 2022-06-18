using Aequus.Buffs.Minion;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Summon
{
    public class GravetenderWisp : MinionBase
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 5;
            this.SetTrail(8);
        }

        public sealed override void SetDefaults()
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
            if (!AequusHelpers.UpdateProjActive<GravetenderMinionBuff>(Projectile))
            {
                return;
            }

            Projectile.LoopingFrame(7);

            Projectile.rotation = Projectile.velocity.X * 0.2f;
            var aequus = Main.player[Projectile.owner].Aequus();
            var gotoPosition = aequus.setGravetenderGhost != -1 ?
                Main.npc[aequus.setGravetenderGhost].Center + new Vector2(0f, -Main.npc[aequus.setGravetenderGhost].height - Projectile.height)
                : DefaultIdlePosition();

            var diff = gotoPosition - Projectile.Center;
            if ((Main.player[Projectile.owner].Center - Projectile.Center).Length() > 2000f)
            {
                Projectile.Center = Main.player[Projectile.owner].Center;
                Projectile.velocity *= 0.1f;
                diff = Vector2.UnitY;
            }
            var ovalDiff = new Vector2(diff.X, diff.Y *= 3f);
            float ovalLength = ovalDiff.Length();
            if (ovalLength > 40f)
            {
                var velocity = diff / 80f;
                if (velocity.Length() < 4f)
                {
                    velocity = Vector2.Normalize(velocity).UnNaN() * 4f;
                }
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, velocity, 0.015f);
            }

            Lighting.AddLight(Projectile.Center, new Vector3(0.4f, 0.05f, 0.1f));

            if (aequus.setGravetenderGhost != -1)
            {
                Projectile.spriteDirection = Math.Sign(Main.npc[aequus.setGravetenderGhost].velocity.X);
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

            Main.instance.PrepareDrawnEntityDrawing(Projectile, 0);

            var c = Projectile.GetAlpha(lightColor) * Projectile.Opacity * Projectile.scale;

            var bloom = Images.Bloom[0].Value;
            var bloomOrigin = bloom.Size() / 2f;
            off -= Main.screenPosition;
            Main.spriteBatch.Draw(bloom, Projectile.position + off, null, Color.Black * Projectile.Opacity, 0f, bloomOrigin, Projectile.scale * 0.3f, SpriteEffects.None, 0f);

            for (int i = 0; i < trailLength; i++)
            {
                float p = AequusHelpers.CalcProgress(trailLength, i);
                Main.spriteBatch.Draw(bloom, Projectile.oldPos[i] + off, null, Color.Black * Projectile.Opacity * p * (0.8f + 0.2f * p), 0f, bloomOrigin, Projectile.scale * 0.3f * (0.8f + 0.2f * p), SpriteEffects.None, 0f);
            }

            origin.Y += 6f;
            var effects = Projectile.GetSpriteEffect();
            Main.instance.PrepareDrawnEntityDrawing(Projectile, Main.player[Projectile.owner].cHead);
            for (int i = 0; i < trailLength; i++)
            {
                float p = AequusHelpers.CalcProgress(trailLength, i);
                Main.EntitySpriteDraw(t, Projectile.oldPos[i] + off, frame, c * 0.6f * p * p, Projectile.oldRot[i], origin, Projectile.scale * (0.8f + 0.2f * p), effects, 0);
            }
            foreach (var v in AequusHelpers.CircularVector(4))
            {
                float f = AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 2.5f, 2f, 4f);
                Main.EntitySpriteDraw(t, Projectile.position + off + v * (f - 2f), frame, c * 0.1f, Projectile.rotation, origin, Projectile.scale, effects, 0);
                Main.EntitySpriteDraw(t, Projectile.position + off + v * f, frame, c * 0.1f, Projectile.rotation, origin, Projectile.scale, effects, 0);
            }
            Main.EntitySpriteDraw(t, Projectile.position + off, frame, c, Projectile.rotation, origin, Projectile.scale, effects, 0);
            return false;
        }
    }
}