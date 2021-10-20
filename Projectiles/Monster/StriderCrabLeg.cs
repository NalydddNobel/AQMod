using AQMod.Assets;
using AQMod.Common.Utilities;
using AQMod.NPCs.Monsters.AquaticEvent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Monster
{
    public sealed class StriderCrabLeg : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.hostile = true;
            projectile.timeLeft *= 10;
            projectile.manualDirectionChange = true;
        }

        public Vector2 getKneecapPositionOffset()
        {
            return new Vector2(projectile.width / 2f, projectile.height - StriderCrab.LEG_2_KNEECAP_Y_OFFSET);
        }

        public override void AI()
        {
            int npcOwner = (int)projectile.ai[1] - 1;
            if (npcOwner == -1 || !Main.npc[npcOwner].active)
            {
                //Main.NewText("bo");
                projectile.Kill();
                return;
            }
            int legID = (int)projectile.ai[0];
            var strider = Main.npc[npcOwner].modNPC as StriderCrab;
            Vector2 connection = strider.GetLegPosition(legID, Main.npc[npcOwner].direction);
            var kneecapPositionOffset = getKneecapPositionOffset();
            var kneecapPosition = projectile.position + kneecapPositionOffset;
            var differenceFromConnection = kneecapPosition - connection;
            if (differenceFromConnection.Length() > StriderCrab.LEG_1_LENGTH)
            {
                float speed = Math.Max(differenceFromConnection.Length() / 16f, 6f);
                var normal = Vector2.Normalize(-differenceFromConnection);
                projectile.velocity = normal * speed;
                projectile.velocity.X *= 0.75f;
                strider.npc.velocity += -projectile.velocity / 8f;
                if (projectile.localAI[0] < 120f)
                    projectile.localAI[0] += 1.5f;
            }
            if (projectile.tileCollide && projectile.velocity.Y.Abs() <= 0.01f)
            {
                if (strider.npc.velocity.X.Abs() > 1.5f)
                {
                    projectile.localAI[0] -= 18f;
                }
                else if (strider.npc.velocity.X.Abs() > 0.1f)
                {
                    projectile.localAI[0] -= 1f;
                }
            }
            projectile.velocity.Y += 0.2f;
            if (projectile.velocity.Y > 3f)
                projectile.velocity.Y = 3f;
            if (projectile.localAI[0] <= 0f)
            {
                projectile.localAI[0] = 120f - Main.rand.Next(30);
                projectile.velocity.X = projectile.direction * 4.5f;
                projectile.velocity.Y = -2f;
                strider.npc.velocity += projectile.velocity / 7f;
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            int npcOwner = (int)projectile.ai[1] - 1;
            fallThrough = true;
            if (Main.npc[npcOwner].HasValidTarget)
            {
                var target = Main.player[Main.npc[npcOwner].target];
                fallThrough = Main.npc[npcOwner].position.Y + Main.npc[npcOwner].height + 180f < target.position.Y;
            }
            if (fallThrough)
            {
                if ((Main.npc[npcOwner].Center - projectile.Center).Length() < 10f || Main.npc[npcOwner].position.Y + Main.npc[npcOwner].height > projectile.position.Y)
                    fallThrough = true;
            }
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.velocity.Y >= 0f)
                projectile.velocity *= 0.1f;
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var texture = this.GetTexture();
            var orig = new Vector2(texture.Width / 2f, texture.Height - 4f);
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, lightColor, projectile.rotation, orig, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}