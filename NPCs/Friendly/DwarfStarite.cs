using AQMod.Assets;
using AQMod.Common.WorldGeneration;
using AQMod.Content.WorldEvents.GlimmerEvent;
using AQMod.Items.Vanities.Critters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs.Friendly
{
    public class DwarfStarite : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 6;
            Main.npcCatchable[npc.type] = true;
            NPCID.Sets.TrailingMode[npc.type] = 7;
            NPCID.Sets.TrailCacheLength[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.width = 6;
            npc.height = 6;
            npc.aiStyle = -1;
            npc.damage = 0;
            npc.defense = 0;
            npc.lifeMax = 5;
            npc.HitSound = SoundID.NPCHit5;
            npc.DeathSound = SoundID.NPCDeath55;
            npc.npcSlots = 0.5f;
            npc.noGravity = true;
            npc.catchItem = (short)ModContent.ItemType<DwarfStariteItem>();
            for (int i = 0; i < npc.buffImmune.Length; i++)
            {
                npc.buffImmune[i] = true;
            }
        }

        public override void AI()
        {
            if (GlimmerEvent.ShouldKillStar(npc))
            {
                npc.life = -1;
                npc.HitEffect();
                npc.active = false;
                return;
            }
            if ((int)npc.ai[1] == -1)
            {
                if (npc.ai[3] > 0f)
                    npc.ai[3] = 0f;
                npc.ai[3]--;
                if (npc.ai[3] < -60f)
                {
                    npc.life = -33333;
                    npc.HitEffect();
                    npc.checkDead();
                }
                return;
            }
            int tileHeight = 0;
            int tileX = ((int)npc.position.X + npc.width) / 16;
            int tileY = ((int)npc.position.Y + npc.height) / 16;
            for (int i = 0; i < 10; i++)
            {
                if (AQWorldGen.ActiveAndSolid(tileX, tileY + i))
                {
                    tileHeight = i + 1;
                    break;
                }
            }
            if (tileHeight == 10)
            {
                npc.ai[0] = 0.5f;
            }
            else
            {
                if ((int)npc.ai[1] <= 0)
                {
                    npc.ai[0] = Main.rand.NextFloat(-1f, 1f);
                    npc.ai[1] = Main.rand.Next(20, 80);
                }
                else
                {
                    npc.ai[1]--;
                    if (npc.collideX)
                    {
                        npc.ai[0] = -npc.ai[0];
                        npc.velocity.Y = npc.oldVelocity.Y * 0.8f;
                    }
                }
            }
            if ((int)npc.ai[3] <= 0)
            {
                npc.ai[2] = Main.rand.NextFloat(-2f, 2f);
                npc.ai[3] = Main.rand.Next(120, 600);
            }
            else
            {
                npc.ai[3]--;
                if (npc.collideX)
                {
                    npc.ai[2] = -npc.ai[2];
                    npc.velocity.X = npc.oldVelocity.X * 0.8f;
                }
            }
            npc.velocity.X = MathHelper.Lerp(npc.velocity.X, npc.ai[2], 0.05f);
            npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, npc.ai[0], 0.05f);
            npc.rotation += npc.velocity.X * 0.004f;
            if ((int)npc.frameCounter == 0 && Main.rand.NextBool(400))
                npc.frameCounter = 1.0;
        }

        public override void PostAI()
        {
            if ((int)npc.frameCounter != 0)
            {
                if (Main.rand.NextBool(10))
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, 15);
                    Main.dust[d].velocity = npc.velocity * 0.01f;
                }
                if (Main.rand.NextBool(20))
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, 58);
                    Main.dust[d].velocity.X = Main.rand.NextFloat(-4f, 4f);
                    Main.dust[d].velocity.Y = Main.rand.NextFloat(-4f, 4f);
                }
                if (Main.rand.NextBool(20))
                {
                    int g = Gore.NewGore(npc.position + new Vector2(Main.rand.Next(npc.width - 4), Main.rand.Next(npc.height - 4)), new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f)), 16 + Main.rand.Next(2));
                    Main.gore[g].scale *= 0.6f;
                }
            }
            else
            {
                if (Main.rand.NextBool(20))
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, 15);
                    Main.dust[d].velocity = npc.velocity * 0.1f;
                    Main.dust[d].scale = Main.rand.NextFloat(0.4f, 0.8f);
                }
                if (Main.rand.NextBool(120))
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, 58);
                    Main.dust[d].velocity.X = Main.rand.NextFloat(-1f, 1f);
                    Main.dust[d].velocity.Y = Main.rand.NextFloat(-1f, 1f);
                    Main.dust[d].scale = Main.rand.NextFloat(0.4f, 0.8f);
                }
                if (Main.rand.NextBool(120))
                {
                    int g = Gore.NewGore(npc.position + new Vector2(Main.rand.Next(npc.width - 4), Main.rand.Next(npc.height - 4)), new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-0.5f, 0.5f)), 16 + Main.rand.Next(2));
                    Main.gore[g].scale *= 0.3f;
                }
            }
            Lighting.AddLight(npc.Center, new Vector3(0.1f, 0.1f, 0.01f));
        }

        public override bool CheckDead()
        {
            if (npc.ai[1] == -1f)
                return true;
            npc.ai[0] = 0f;
            npc.ai[1] = -1f;
            npc.ai[2] = 0f;
            npc.ai[3] = 0f;
            npc.velocity = new Vector2(0f, 0f);
            npc.dontTakeDamage = true;
            npc.life = npc.lifeMax;
            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            if ((int)npc.frameCounter != 0)
            {
                npc.frameCounter++;
                if (npc.frameCounter > 3.0)
                {
                    npc.frameCounter = 1.0;
                    npc.frame.Y += frameHeight;
                    if (npc.frame.Y >= frameHeight * Main.npcFrameCount[npc.type])
                    {
                        npc.frame.Y = 0;
                        npc.frameCounter = 0.0;
                    }
                }
            }
            else
            {
                npc.frame.Y = 0;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            float x = npc.velocity.X.Abs() * hitDirection;
            if (npc.life <= 0)
            {
                for (int i = 0; i < 20; i++)
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, 55);
                    Main.dust[d].velocity.X += x;
                    Main.dust[d].velocity.Y = -Main.rand.NextFloat(1f, 3f);
                }
                for (int i = 0; i < 15; i++)
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, 57 + Main.rand.Next(2));
                    Main.dust[d].velocity.X += x;
                    Main.dust[d].velocity.Y = -Main.rand.NextFloat(1f, 3f);
                }
                for (int i = 0; i < 3; i++)
                {
                    Gore.NewGore(npc.Center, new Vector2(Main.rand.NextFloat(-2f, 2f) + x, Main.rand.NextFloat(-2f, 2f)), 16 + Main.rand.Next(2));
                }
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, 55);
                    Main.dust[d].velocity.X += x;
                    Main.dust[d].velocity.Y = -Main.rand.NextFloat(5f, 12f);
                }
                if (Main.rand.NextBool())
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, 57 + Main.rand.Next(2));
                    Main.dust[d].velocity.X += x;
                    Main.dust[d].velocity.Y = -Main.rand.NextFloat(2f, 6f);
                }
                if (Main.rand.NextBool())
                    Gore.NewGore(npc.Center, new Vector2(Main.rand.NextFloat(-4f, 4f) + x * 0.75f, Main.rand.NextFloat(-4f, 4f)), 16 + Main.rand.Next(2));
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (GlimmerEvent.IsActive && spawnInfo.player.position.Y < Main.worldSurface * 16f)
                return 1f;
            return 0f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = Main.npcTexture[npc.type];
            var offset = new Vector2(npc.width / 2f, npc.height / 2f);
            var screenPos = Main.screenPosition;
            Vector2 origin = npc.frame.Size() / 2f;
            Vector2 drawPos = npc.Center - screenPos;
            float mult = 1f / NPCID.Sets.TrailCacheLength[npc.type];
            for (int i = 0; i < NPCID.Sets.TrailCacheLength[npc.type]; i++)
            {
                Main.spriteBatch.Draw(texture, npc.oldPos[i] + offset - Main.screenPosition, npc.frame, new Color(80, 80, 80, 0) * (mult * (NPCID.Sets.TrailCacheLength[npc.type] - i)), npc.oldRot[i], origin, npc.scale, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.Draw(texture, drawPos, npc.frame, new Color(255, 255, 255, 255), npc.rotation, origin, npc.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, drawPos, npc.frame, new Color(20, 20, 20, 0), npc.rotation, origin, npc.scale + 0.1f, SpriteEffects.None, 0f);
            if ((int)npc.ai[1] == -1)
            {
                var spotlightTexture = AQTextures.Lights[LightTex.Spotlight20x20];
                var spotlightOrigin = spotlightTexture.Size() / 2f;
                float scale = npc.scale * (-npc.ai[3] / 44f);
                scale *= scale;
                Main.spriteBatch.Draw(spotlightTexture, drawPos, null, new Color(250, 250, 250, 0), 0f, spotlightOrigin, scale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(spotlightTexture, drawPos, null, new Color(60, 60, 60, 0), 0f, spotlightOrigin, scale * 2f, SpriteEffects.None, 0f);
            }
            return false;
        }
    }
}