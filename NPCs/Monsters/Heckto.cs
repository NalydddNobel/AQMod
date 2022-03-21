using AQMod.Dusts;
using AQMod.Items.Accessories.Summon;
using AQMod.Items.Potions.Foods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs.Monsters
{
    public class Heckto : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 5;
            NPCID.Sets.TrailingMode[npc.type] = 7;
            NPCID.Sets.TrailCacheLength[npc.type] = 16;
        }

        public override void SetDefaults()
        {
            npc.width = 20;
            npc.height = 20;
            npc.damage = 70;
            npc.defense = 30;
            npc.lifeMax = 250;
            npc.knockBackResist = 0.1f;
            npc.HitSound = SoundID.NPCHit36;
            npc.DeathSound = SoundID.NPCDeath39;
            npc.value = 510f;
            npc.noTileCollide = true;
            npc.noGravity = true;
            for (int b = 0; b < npc.buffImmune.Length; b++)
            {
                npc.buffImmune[b] = true;
            }
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            if (AQMod.calamityMod.IsActive)
            {
                npc.lifeMax = (int)(npc.lifeMax * 1.5f);
                npc.defense *= 2;
            }
        }

        public override void AI()
        {
            npc.TargetClosest();
            Vector2 center = npc.Center;
            var target = Main.player[npc.target];
            Vector2 trueDifference = target.Center - center;
            Vector2 difference = trueDifference;
            float speedMult = (float)Math.Sqrt(difference.X * difference.X + difference.Y * difference.Y);
            var tile = Framing.GetTileSafely(npc.Center.ToTileCoordinates());
            float maxSpeed = 18f;
            bool inTiles = tile.active() && Main.tileSolid[tile.type] && !Main.tileSolidTop[tile.type];
            if (inTiles)
                maxSpeed = 5f;
            maxSpeed += 10 * (1f - npc.life / (float)npc.lifeMax);
            speedMult = maxSpeed / speedMult;
            difference *= speedMult;
            if (!inTiles && trueDifference.X.Abs() > 100f && trueDifference.Y.Abs() < 120f)
            {
                npc.ai[0] = 1f;
                npc.direction = difference.X < 0f ? -1 : 1;
                npc.velocity.X += npc.direction * 0.5f;
                if (npc.velocity.X > maxSpeed)
                {
                    npc.velocity.X = maxSpeed;
                }
                else if (npc.velocity.X < -maxSpeed)
                {
                    npc.velocity.X = -maxSpeed;
                }
                npc.directionY = difference.Y < 0 ? -1 : 1;
                npc.velocity.Y += npc.directionY * 0.1f;
                if (npc.velocity.Y < 0f && npc.directionY == 1 || npc.velocity.Y > 0f && npc.directionY == -1)
                    npc.velocity.Y += npc.directionY * 0.1f;
            }
            else
            {
                npc.ai[0] = 0f;
                npc.velocity.X = (npc.velocity.X * 100f + difference.X) / 101f;
                npc.velocity.Y = (npc.velocity.Y * 100f + difference.Y) / 101f;
            }
            npc.rotation = (float)Math.Atan2(difference.Y, difference.X) - 1.57f;
            int d = Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(240, 90, 100, 0));
            Main.dust[d].velocity *= 0.1f;
            Main.dust[d].scale = 1.3f;
            Main.dust[d].noGravity = true;
            if (Main.netMode == NetmodeID.Server)
                return;
            if (Main.rand.Next(400) == 0)
                Main.PlaySound(SoundID.Zombie, (int)npc.position.X, (int)npc.position.Y, Main.rand.Next(53, 55));
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            if (npc.frameCounter > 6)
            {
                npc.frameCounter = 0;
                npc.frame.Y += frameHeight;
                if (npc.frame.Y >= frameHeight * Main.npcFrameCount[npc.type])
                    npc.frame.Y = 0;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                for (int i = 0; i < 50; i++)
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<MonoDust>(), npc.velocity.X, npc.velocity.Y, 0, new Color(240, 90, 100, 0));
                    Main.dust[d].velocity *= 2f;
                    Main.dust[d].noGravity = true;
                    Main.dust[d].scale = 1.4f;
                }
            }
        }

        public override void NPCLoot()
        {
            Item.NewItem(npc.getRect(), ItemID.Ectoplasm, Main.rand.Next(3) + 1);
            if (Main.rand.NextBool(10))
                Item.NewItem(npc.getRect(), ModContent.ItemType<RedLicorice>());
            if (Main.rand.NextBool(15))
                Item.NewItem(npc.getRect(), ModContent.ItemType<Dreadsoul>());
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            var screenPos = Main.screenPosition;
            var texture = Main.npcTexture[npc.type];
            Vector2 drawPos = npc.position - screenPos;
            var orig = new Vector2(texture.Width / 2, texture.Height / Main.npcFrameCount[npc.type] / 2);
            var offset = -(new Vector2(texture.Width, texture.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f) + orig * npc.scale + new Vector2(0f, npc.gfxOffY + 2f);
            offset += new Vector2(npc.width / 2f, npc.height / 2f);
            var spriteEffects = npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            if ((int)npc.ai[0] == 1)
            {
                int trailLength = NPCID.Sets.TrailCacheLength[npc.type];
                for (int i = 0; i < trailLength; i++)
                {
                    if (npc.oldPos[i] == Vector2.Zero)
                        break;
                    float progress = 1f - 1f / trailLength * i;
                    progress += (float)Math.Sin(-i * 0.314f + Main.GlobalTime * 20) * 0.1f;
                    Main.spriteBatch.Draw(texture, npc.oldPos[i] + offset - screenPos, npc.frame, new Color(88, 38, 20, 2) * progress, npc.rotation, orig, npc.scale * progress, spriteEffects, 0f);
                }
            }
            else
            {
                npc.oldPos[0] = Vector2.Zero;
            }
            drawPos += offset;
            Main.spriteBatch.Draw(texture, drawPos, npc.frame, new Color(200, 200, 200, 0), npc.rotation, orig, npc.scale, spriteEffects, 0f);
            Main.spriteBatch.Draw(texture, drawPos, npc.frame, new Color(120, 40, 40, 0), npc.rotation, orig, npc.scale * 1.1f, spriteEffects, 0f);
            return false;
        }
    }
}