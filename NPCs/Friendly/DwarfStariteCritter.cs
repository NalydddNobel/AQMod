using Aequus.Biomes;
using Aequus.Items.Consumables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Friendly
{
    public class DwarfStariteCritter : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 6;
            Main.npcCatchable[Type] = true;
            NPCID.Sets.TrailingMode[Type] = 7;
            NPCID.Sets.TrailCacheLength[Type] = 4;
            NPCID.Sets.DebuffImmunitySets.Add(Type, new Terraria.DataStructures.NPCDebuffImmunityData()
            {
                ImmuneToAllBuffsThatAreNotWhips = true,
            });
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Position = new Vector2(0f, 23f),
                PortraitPositionYOverride = 42,
            });
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            this.CreateEntry(database, bestiaryEntry)
                .AddMainSpawn(BestiaryBuilder.NightTime);
        }

        public override void SetDefaults()
        {
            NPC.width = 6;
            NPC.height = 6;
            NPC.aiStyle = -1;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 5;
            NPC.HitSound = SoundID.NPCHit5;
            NPC.DeathSound = SoundID.NPCDeath55;
            NPC.npcSlots = 0.5f;
            NPC.noGravity = true;
            NPC.catchItem = (short)ModContent.ItemType<DwarfStarite>();

            this.SetBiome<GlimmerInvasion>();
        }

        public override void AI()
        {
            if (Main.dayTime)
            {
                NPC.life = -1;
                NPC.HitEffect();
                NPC.active = false;
                return;
            }
            if ((int)NPC.ai[1] == -1)
            {
                if (NPC.ai[3] > 0f)
                    NPC.ai[3] = 0f;
                NPC.ai[3]--;
                if (NPC.ai[3] < -60f)
                {
                    NPC.life = -33333;
                    NPC.HitEffect();
                    NPC.checkDead();
                }
                return;
            }
            int tileHeight = 0;
            int tileX = ((int)NPC.position.X + NPC.width) / 16;
            int tileY = ((int)NPC.position.Y + NPC.height) / 16;
            for (int i = 0; i < 10; i++)
            {
                if (WorldGen.InWorld(tileX, tileY + i, 10) && AequusHelpers.IsSolid(Main.tile[tileX, tileY + i]))
                {
                    tileHeight = i + 1;
                    break;
                }
            }
            if (tileHeight == 10)
            {
                NPC.ai[0] = 0.5f;
            }
            else
            {
                if ((int)NPC.ai[1] <= 0)
                {
                    NPC.ai[0] = Main.rand.NextFloat(-1f, 1f);
                    NPC.ai[1] = Main.rand.Next(20, 80);
                }
                else
                {
                    NPC.ai[1]--;
                    if (NPC.collideX)
                    {
                        NPC.ai[0] = -NPC.ai[0];
                        NPC.velocity.Y = NPC.oldVelocity.Y * 0.8f;
                    }
                }
            }
            if ((int)NPC.ai[3] <= 0)
            {
                NPC.ai[2] = Main.rand.NextFloat(-2f, 2f);
                NPC.ai[3] = Main.rand.Next(120, 600);
            }
            else
            {
                NPC.ai[3]--;
                if (NPC.collideX)
                {
                    NPC.ai[2] = -NPC.ai[2];
                    NPC.velocity.X = NPC.oldVelocity.X * 0.8f;
                }
            }
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, NPC.ai[2], 0.05f);
            NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, NPC.ai[0], 0.05f);
            NPC.rotation += NPC.velocity.X * 0.004f;
            if ((int)NPC.frameCounter == 0 && Main.rand.NextBool(400))
                NPC.frameCounter = 1.0;
            if ((int)NPC.frameCounter != 0)
            {
                if (Main.rand.NextBool(10))
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.MagicMirror);
                    Main.dust[d].velocity = NPC.velocity * 0.01f;
                }
                if (Main.rand.NextBool(20))
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Enchanted_Pink);
                    Main.dust[d].velocity.X = Main.rand.NextFloat(-4f, 4f);
                    Main.dust[d].velocity.Y = Main.rand.NextFloat(-4f, 4f);
                }
                if (Main.rand.NextBool(20))
                {
                    int g = Gore.NewGore(NPC.GetSource_FromThis(), NPC.position + new Vector2(Main.rand.Next(NPC.width - 4), Main.rand.Next(NPC.height - 4)), new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f)), 16 + Main.rand.Next(2));
                    Main.gore[g].scale *= 0.6f;
                }
            }
            else
            {
                if (Main.rand.NextBool(20))
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.MagicMirror);
                    Main.dust[d].velocity = NPC.velocity * 0.1f;
                    Main.dust[d].scale = Main.rand.NextFloat(0.4f, 0.8f);
                }
                if (Main.rand.NextBool(120))
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Enchanted_Pink);
                    Main.dust[d].velocity.X = Main.rand.NextFloat(-1f, 1f);
                    Main.dust[d].velocity.Y = Main.rand.NextFloat(-1f, 1f);
                    Main.dust[d].scale = Main.rand.NextFloat(0.4f, 0.8f);
                }
                if (Main.rand.NextBool(120))
                {
                    int g = Gore.NewGore(NPC.GetSource_FromThis(), NPC.position + new Vector2(Main.rand.Next(NPC.width - 4), Main.rand.Next(NPC.height - 4)), new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-0.5f, 0.5f)), 16 + Main.rand.Next(2));
                    Main.gore[g].scale *= 0.3f;
                }
            }
            Lighting.AddLight(NPC.Center, new Vector3(0.1f, 0.1f, 0.01f));
        }

        public override bool CheckDead()
        {
            if (NPC.ai[1] == -1f)
                return true;
            NPC.ai[0] = 0f;
            NPC.ai[1] = -1f;
            NPC.ai[2] = 0f;
            NPC.ai[3] = 0f;
            NPC.velocity = new Vector2(0f, 0f);
            NPC.dontTakeDamage = true;
            NPC.life = NPC.lifeMax;
            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            if ((int)NPC.frameCounter != 0)
            {
                NPC.frameCounter++;
                if (NPC.frameCounter > 3.0)
                {
                    NPC.frameCounter = 1.0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[NPC.type])
                    {
                        NPC.frame.Y = 0;
                        NPC.frameCounter = 0.0;
                    }
                }
            }
            else
            {
                NPC.frame.Y = 0;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            float x = NPC.velocity.X.Abs() * hitDirection;
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 20; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 55);
                    Main.dust[d].velocity.X += x;
                    Main.dust[d].velocity.Y = -Main.rand.NextFloat(1f, 3f);
                }
                for (int i = 0; i < 15; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 57 + Main.rand.Next(2));
                    Main.dust[d].velocity.X += x;
                    Main.dust[d].velocity.Y = -Main.rand.NextFloat(1f, 3f);
                }
                for (int i = 0; i < 3; i++)
                {
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center, new Vector2(Main.rand.NextFloat(-2f, 2f) + x, Main.rand.NextFloat(-2f, 2f)), 16 + Main.rand.Next(2));
                }
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 55);
                    Main.dust[d].velocity.X += x;
                    Main.dust[d].velocity.Y = -Main.rand.NextFloat(5f, 12f);
                }
                if (Main.rand.NextBool())
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 57 + Main.rand.Next(2));
                    Main.dust[d].velocity.X += x;
                    Main.dust[d].velocity.Y = -Main.rand.NextFloat(2f, 6f);
                }
                if (Main.rand.NextBool())
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, new Vector2(Main.rand.NextFloat(-4f, 4f) + x * 0.75f, Main.rand.NextFloat(-4f, 4f)), 16 + Main.rand.Next(2));
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (!Main.dayTime && spawnInfo.Player.position.Y < Main.worldSurface * 16f)
                return 0.0005f;
            return 0f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var texture = TextureAssets.Npc[NPC.type].Value;
            var offset = new Vector2(NPC.width / 2f, NPC.height / 2f);
            var origin = NPC.frame.Size() / 2f;
            var drawPos = NPC.Center - screenPos;
            float mult = 1f / NPCID.Sets.TrailCacheLength[NPC.type];
            for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
            {
                Main.spriteBatch.Draw(texture, NPC.oldPos[i] + offset - Main.screenPosition, NPC.frame, new Color(80, 80, 80, 0) * (mult * (NPCID.Sets.TrailCacheLength[NPC.type] - i)), NPC.oldRot[i], origin, NPC.scale, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.Draw(texture, drawPos, NPC.frame, new Color(255, 255, 255, 255), NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, drawPos, NPC.frame, new Color(20, 20, 20, 0), NPC.rotation, origin, NPC.scale + 0.1f, SpriteEffects.None, 0f);
            if ((int)NPC.ai[1] == -1)
            {
                var spotlightTexture = ModContent.Request<Texture2D>(Aequus.AssetsPath + "Bloom_20x20").Value;
                var spotlightOrigin = spotlightTexture.Size() / 2f;
                float scale = NPC.scale * (-NPC.ai[3] / 44f);
                scale *= scale;
                Main.spriteBatch.Draw(spotlightTexture, drawPos, null, new Color(250, 250, 250, 0), 0f, spotlightOrigin, scale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(spotlightTexture, drawPos, null, new Color(60, 60, 60, 0), 0f, spotlightOrigin, scale * 2f, SpriteEffects.None, 0f);
            }
            return false;
        }
    }
}