using AQMod.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs.Monsters.AtmosphericEvent
{
    public class Vraine : ModNPC
    {
        public const int FramesX = 2;

        private bool _setupFrame; // no need to sync this since find frame stuff is client only (I think)

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 16;

            NPCID.Sets.TrailingMode[npc.type] = 7;
            NPCID.Sets.TrailCacheLength[npc.type] = 12;
        }

        public override void SetDefaults()
        {
            npc.width = 46;
            npc.height = 36;
            npc.lifeMax = 345;
            npc.damage = 45;
            npc.defense = 3;
            npc.HitSound = SoundID.DD2_GoblinHurt;
            npc.DeathSound = SoundID.NPCDeath56;
            npc.aiStyle = -1;
            npc.noGravity = true;
            npc.knockBackResist = 0.1f;
            npc.value = Item.buyPrice(silver: 20);
            npc.buffImmune[BuffID.OnFire] = true;
            //banner = npc.type;
            //bannerItem = ModContent.ItemType<StariteBanner>();
        }

        public override void AI() // ai[0] attack stuff, -1 means it's fleeing.
                                  // ai[1] is temperature (1 = hot, 2 = cold)
        {
            if (npc.ai[1] == 0f)
            {
                npc.ai[1] = Main.rand.Next(2) + 1f; // whether to be a hot or cold enemy
                npc.netUpdate = true;
            }
            bool hot = (int)npc.ai[1] == 1;
            if ((int)npc.ai[0] == -1)
            {
                npc.velocity.X *= 0.97f;
                if (npc.velocity.Y < 0f)
                {
                    npc.velocity.Y *= 0.97f;
                }
                npc.velocity.Y -= 0.1f;
                return;
            }
            npc.TargetClosest(faceTarget: false);
            npc.spriteDirection = 1;
            if (!npc.HasValidTarget)
            {
                npc.ai[0] = -1f;
                return;
            }

            Player target = Main.player[npc.target];
            float targetY = target.position.Y;
            if (hot)
            {
                targetY -= npc.height * 6f;
            }
            else
            {
                targetY += npc.width * 4f;
            }
            float diffY = targetY - npc.position.Y;
            if (targetY < npc.position.Y)
            {
                if (npc.velocity.Y > -4f)
                {
                    npc.velocity.Y -= 0.1f;
                }
            }
            else
            {
                if (diffY > 240f)
                {
                    if (npc.velocity.Y < 3.5f)
                    {
                        npc.velocity.Y += 0.15f;
                    }
                }
                else
                {
                    if (npc.ai[1] == 1f)
                    {
                        if (npc.velocity.Y < 2f)
                        {
                            npc.velocity.Y += 0.05f;
                        }
                    }
                    else
                    {
                        if (npc.velocity.Y < 1.5f)
                        {
                            npc.velocity.Y += 0.025f;
                        }
                    }
                }
            }
            float targetX = target.position.X + target.width / 2f;
            if (npc.velocity.X < 0f)
            {
                targetX -= npc.width * 3f;
            }
            else
            {
                targetX += npc.width * 3f;
            }
            float diffX = targetX - (npc.position.X + npc.width / 2f);
            if (targetX < npc.position.X) // moving left
            {
                if (npc.ai[1] == 1f)
                {
                    if (npc.velocity.X > -4f)
                    {
                        npc.velocity.X -= 0.15f;
                    }
                }
                else
                {
                    if (npc.velocity.X > -2f)
                    {
                        npc.velocity.X -= 0.075f;
                    }
                }
            }
            else // moving right
            {
                if (npc.ai[1] == 1f)
                {
                    if (npc.velocity.X < 4f)
                    {
                        npc.velocity.X += 0.15f;
                    }
                }
                else
                {
                    if (npc.velocity.X < 2f)
                    {
                        npc.velocity.X += 0.075f;
                    }
                }
            }
            // attack stuff
            if (diffY.Abs() < 40f)
            {
                npc.ai[0]++;
                if (diffX.Abs() < 100f)
                {
                    npc.ai[0]++; // x2 faster when closer to the player
                }
                if ((int)npc.ai[0] > 60)
                {
                    npc.ai[0] = 0f;
                    npc.netUpdate = true;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(npc.Center, new Vector2(0f, 12f), ProjectileID.SmokeBomb, 30, 1f, Main.myPlayer);
                    }
                }
            }
            else
            {
                if (npc.ai[0] > 0f)
                {
                    npc.ai[0] -= 0.5f;
                }
            }

            DoParticles();
            DoCollision();
            Lighting.AddLight(npc.Center, new Vector3(-1f, -1f, -1f));
        }

        private void DoAttacks()
        {
        }

        private void DoParticles()
        {
            var center = npc.Center;
            float yMult = npc.height / (float)npc.width;
            for (int i = 0; i < 10 * AQMod.EffectQuality; i++)
            {
                int d = Dust.NewDust(npc.position, npc.width, npc.height, 16);
                Main.dust[d].position = center;
                var offset = new Vector2(Main.rand.NextFloat(npc.width - 12f) / 2f, 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi));
                offset.Y *= yMult;
                Main.dust[d].alpha = 165;
                Main.dust[d].position += offset;
                Main.dust[d].velocity *= 0.1f;
                Main.dust[d].velocity += npc.velocity * 0.8f;
                Main.dust[d].noGravity = true;
                Main.dust[d].scale = Main.rand.NextFloat(0.2f, 2.25f);
            }
        }

        private void DoCollision()
        {
            var rect = npc.getRect();
            var center = npc.Center;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (i != npc.whoAmI && Main.npc[i].active && Main.npc[i].type == npc.type && Main.npc[i].getRect().Intersects(rect)) // if there are multiple temperature balloons colliding
                {
                    var normal = Vector2.Normalize(center - Main.npc[i].Center);
                    npc.velocity += normal;
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (!_setupFrame) // sets up the width of the frame since this NPC has horizontal frames
            {
                _setupFrame = true;
                npc.frame.Width /= FramesX;
            }
            npc.frame.X = (int)(npc.frame.Width * (npc.ai[1] - 1));
            if (npc.velocity.X > 0f)
            {
                npc.frameCounter += 1.0d;
            }
            else
            {
                npc.frameCounter -= 1.0d;
            }
            if (npc.frameCounter > 5.0d)
            {
                npc.frameCounter = 0.0d;
                if (npc.frame.Y > 0)
                {
                    npc.frame.Y -= frameHeight;
                }
            }
            else if (npc.frameCounter < -5.0d)
            {
                npc.frameCounter = 0.0d;
                if (npc.frame.Y < frameHeight * (Main.npcFrameCount[npc.type] - 1))
                {
                    npc.frame.Y += frameHeight;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            //HotAndColdCurrentLayer.AddToColdCurrentList(new testdraw(ModContent.GetTexture("AQMod/Assets/Textures/debugtextures/coldcur")));
            //HotAndColdCurrentLayer.AddToHotCurrentList(new testdraw(ModContent.GetTexture("AQMod/Assets/Textures/debugtextures/hotcur")));
            Texture2D texture = Main.npcTexture[npc.type];
            var offset = new Vector2(npc.width / 2f, npc.height / 2f);
            Vector2 origin = npc.frame.Size() / 2f;
            Vector2 drawPos = npc.Center - Main.screenPosition;
            float mult = 1f / NPCID.Sets.TrailCacheLength[npc.type];
            for (int i = 0; i < NPCID.Sets.TrailCacheLength[npc.type]; i++)
            {
                Main.spriteBatch.Draw(texture, npc.oldPos[i] + offset - Main.screenPosition, npc.frame, drawColor * 0.3f * (mult * (NPCID.Sets.TrailCacheLength[npc.type] - i)), npc.oldRot[i], origin, npc.scale, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.Draw(texture, drawPos, npc.frame, new Color(255, 255, 255, 255), npc.rotation, origin, npc.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, drawPos, npc.frame, new Color(20, 20, 20, 0), npc.rotation, origin, npc.scale + 0.1f, SpriteEffects.None, 0f);
            return false;
        }
    }
}