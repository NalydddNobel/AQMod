using AQMod.Assets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Items.Armor.Arachnotron
{
    public class ArachnotronLegs : ModProjectile
    {
        public override string Texture => "AQMod/" + OldTextureCache.None;

        public Vector2[] arms;
        public Vector2[] armRest;
        public float[] rotations;
        public float[] rotationRest;
        public int[] directions;

        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 2;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.aiStyle = -1;
            projectile.penetrate = -1;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 12;
            arms = new Vector2[4];
            rotations = new float[8];
            arms[0] = new Vector2(-32f, -32f);
            rotations[0] = -MathHelper.PiOver4;
            rotations[1] = -MathHelper.PiOver4 * 3f + 0.1f;
            arms[1] = new Vector2(32f, -32f);
            rotations[2] = MathHelper.PiOver4;
            rotations[3] = MathHelper.PiOver4 * 3f + 0.1f;
            arms[2] = new Vector2(-32f, 32f);
            rotations[4] = -MathHelper.PiOver4 * 3f;
            rotations[5] = -MathHelper.PiOver4 - 0.1f;
            arms[3] = new Vector2(32f, 32f);
            rotations[6] = MathHelper.PiOver4 * 3f;
            rotations[7] = MathHelper.PiOver4 + 0.1f;
            armRest = new Vector2[4];
            for (int i = 0; i < 4; i++)
            {
                armRest[i] = arms[i];
            }
            rotationRest = new float[8];
            for (int i = 0; i < 8; i++)
            {
                rotationRest[i] = rotations[i];
            }
            directions = new int[4];
            directions[0] = -1;
            directions[1] = 1;
            directions[2] = -1;
            directions[3] = 1;
        }

        private void rest(int i)
        {
            if (arms[i] != armRest[i])
            {
                arms[i] = Vector2.Lerp(arms[i], armRest[i], 0.1f);
                rotations[i * 2] = MathHelper.Lerp(rotations[i * 2], rotationRest[i * 2], 0.1f);
                rotations[i * 2 + 1] = MathHelper.Lerp(rotations[i * 2 + 1], rotationRest[i * 2 + 1], 0.1f);
            }
        }

        public override void AI()
        {
            var plr = Main.player[projectile.owner];
            if (plr.dead)
                plr.GetModPlayer<AQPlayer>().arachnotron = false;
            if (plr.GetModPlayer<AQPlayer>().arachnotron)
                projectile.timeLeft = 2;
            var gotoPosition = plr.position;
            if (Main.player[projectile.owner].gravDir == -1)
            {
                gotoPosition += new Vector2(plr.width / 2f, plr.height - 15f);
            }
            else
            {
                gotoPosition += new Vector2(plr.width / 2f, 16f);
            }
            var center = projectile.Center = Vector2.Lerp(projectile.Center, gotoPosition, 0.25f);
            int target = -1;
            float closestDistance = 640f;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].CanBeChasedBy())
                {
                    float distance = (Main.npc[i].Center - center).Length();
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        target = i;
                    }
                }
            }
            if (target != -1)
            {
                var targetCenter = Main.npc[target].Center;
                projectile.ai[0] += 0.314f;
                const float lerpValue = 0.075f;
                const int seperationX = 12;
                const int seperationY = 12;
                arms[0] = Vector2.Lerp(center + arms[0], targetCenter + new Vector2(Main.npc[target].width + seperationX, -seperationY), lerpValue) - center;
                arms[2] = Vector2.Lerp(center + arms[2], targetCenter + new Vector2(Main.npc[target].width + seperationX, seperationY), lerpValue) - center;
                arms[1] = Vector2.Lerp(center + arms[1], targetCenter + new Vector2(-Main.npc[target].width - seperationX, -seperationY), lerpValue) - center;
                arms[3] = Vector2.Lerp(center + arms[3], targetCenter + new Vector2(-Main.npc[target].width - seperationX, seperationY), lerpValue) - center;
                for (int i = 0; i < arms.Length; i++)
                {
                    rotations[i * 2] = rotationRest[i * 2] + (float)Math.Sin(projectile.ai[0]);
                }
            }
            else
            {
                projectile.ai[0] = 0f;
                for (int i = 0; i < arms.Length; i++)
                {
                    rest(i);
                }
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            var center = projectile.Center;
            var targetPos = targetHitbox.TopLeft();
            var targetSize = targetHitbox.Size();
            for (int i = 0; i < arms.Length; i++)
            {
                float rot = rotations[i * 2];
                var pos = center + arms[i];
                float _ = float.NaN;
                var normal = new Vector2(0f, -1f).RotatedBy(rot);
                var offset = normal * 4;
                Vector2 end = pos + offset + normal * 36;
                if (Collision.CheckAABBvLineCollision(targetPos, targetSize, pos + offset, end, 10f * projectile.scale, ref _))
                    return true;
                pos += normal * 34f;
                rot = rotations[i * 2 + 1];
                _ = float.NaN;
                normal = new Vector2(0f, -1f).RotatedBy(rot);
                offset = normal * 4;
                end = pos + offset + normal * 36;
                if (Collision.CheckAABBvLineCollision(targetPos, targetSize, pos + offset, end, 10f * projectile.scale, ref _))
                    return true;
            }
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var texture = OldTextureCache.ArachnotronArms[ArachnotronLegsTextureType.LegInitial];
            var texture2 = OldTextureCache.ArachnotronArms[ArachnotronLegsTextureType.Leg2ndPiece];
            var texture3 = OldTextureCache.ArachnotronArms[ArachnotronLegsTextureType.LegInitialGlow];
            var texture4 = OldTextureCache.ArachnotronArms[ArachnotronLegsTextureType.Leg2ndPieceGlow];
            projectile.gfxOffY = (float)Math.Sin(Main.GlobalTime * 2f);
            var drawOrig = projectile.position + new Vector2(projectile.width / 2f, projectile.height + projectile.gfxOffY);
            var orig = new Vector2(texture.Width / 2f, texture.Height);
            var orig2 = new Vector2(texture2.Width / 2f, texture2.Height);
            var clr = Lighting.GetColor((int)(drawOrig.X / 16f), (int)(drawOrig.Y / 16f));
            drawOrig -= Main.screenPosition;
            var effects = SpriteEffects.FlipHorizontally;
            if (Main.player[projectile.owner].gravDir == -1)
            {
                drawOrig.Y -= 8f;
                effects = SpriteEffects.None;
            }
            var clr2 = new Color(255, 255, 255, 0);
            for (int i = 0; i < arms.Length; i++)
            {
                float armRot = rotations[i * 2];
                float armRot2 = rotations[i * 2 + 1];
                var drawPos = drawOrig + arms[i];
                var drawPos2 = drawOrig + arms[i] + new Vector2(0f, -orig.Y * projectile.scale + 2f).RotatedBy(armRot);
                Main.spriteBatch.Draw(texture, drawPos, null, clr, armRot, orig, projectile.scale, effects, 0f);
                Main.spriteBatch.Draw(texture2, drawPos2, null, clr, armRot2, orig2, projectile.scale, effects, 0f);
                Main.spriteBatch.Draw(texture3, drawPos, null, clr2, armRot, orig, projectile.scale, effects, 0f);
                Main.spriteBatch.Draw(texture4, drawPos2, null, clr2, armRot2, orig2, projectile.scale, effects, 0f);
            }
            return false;
        }
    }
}