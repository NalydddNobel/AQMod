using Aequus.Events.Glimmer;
using Aequus.Graphics;
using Aequus.Items.Misc.Critters;
using Aequus.Particles;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Friendly.Critter
{
    public class DwarfStariteCritter : ModNPC
    {
        public float rotationSpeed;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 2;
            Main.npcCatchable[Type] = true;
            NPCID.Sets.CountsAsCritter[Type] = true;
            NPCID.Sets.TrailingMode[Type] = 7;
            NPCID.Sets.TrailCacheLength[Type] = 14;
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
            this.CreateEntry(database, bestiaryEntry);
        }

        public override void SetDefaults()
        {
            NPC.width = 12;
            NPC.height = 12;
            NPC.aiStyle = -1;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 5;
            NPC.HitSound = SoundID.NPCHit5;
            NPC.DeathSound = SoundID.NPCDeath55;
            NPC.npcSlots = 0.1f;
            NPC.noGravity = true;
            NPC.catchItem = (short)ModContent.ItemType<DwarfStarite>();

            this.SetBiome<GlimmerBiome>();
        }

        public override Color? GetAlpha(Color drawColor)
        {
            return new Color(255, 255, 255, 0);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            float x = NPC.velocity.X.Abs() * hitDirection;
            if (NPC.life <= 0)
            {
                if (NPC.life == -33333)
                {
                    for (int i = 0; i < 60; i++)
                    {
                        var d = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Unit() * Main.rand.Next(2, 32), ModContent.DustType<MonoDust>(), newColor: Color.Lerp(Color.Yellow.UseB(128), Color.White, Main.rand.NextFloat(0.2f, 1f)).UseA(0));
                        d.velocity *= 0.2f;
                        d.velocity += d.position - NPC.Center;
                    }
                    for (int i = 0; i < 50; i++)
                    {
                        var b = ParticleSystem.Fetch<BloomParticle>().Setup(NPC.Center + Main.rand.NextVector2Unit() * Main.rand.Next(2, 12), Vector2.Zero, Color.White.UseA(0), new Color(25, 25, 40, 0), Main.rand.NextFloat(0.8f, 1.45f), 0.33f);
                        b.Velocity += (b.Position - NPC.Center) / 2f;
                        ParticleSystem.GetLayer(ParticleLayer.AboveDust).Add(b);
                    }
                    for (int i = 0; i < 20; i++)
                    {
                        var b = ParticleSystem.Fetch<BloomParticle>().Setup(NPC.Center + Main.rand.NextVector2Unit() * Main.rand.Next(10, 42), Vector2.Zero, Color.White.UseA(0), new Color(25, 25, 40, 0), Main.rand.NextFloat(0.8f, 1.45f), 0.33f);
                        b.Velocity += (b.Position - NPC.Center) / 3f;
                        ParticleSystem.GetLayer(ParticleLayer.AboveDust).Add(b);
                    }
                    for (int i = 0; i < 25; i++)
                    {
                        var d = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Unit() * Main.rand.Next(2, 32), DustID.Enchanted_Gold + Main.rand.Next(2), newColor: Color.White.UseA(0));
                        d.velocity *= 0.1f;
                        d.velocity += (d.position - NPC.Center) / 2f;
                    }
                }
                for (int i = 0; i < 15; i++)
                {
                    var d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, 57 + Main.rand.Next(2));
                    d.velocity *= 0.1f;
                    d.velocity += d.position - NPC.Center;
                }
                for (int i = 0; i < 3; i++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, new Vector2(Main.rand.NextFloat(-2f, 2f) + x, Main.rand.NextFloat(-2f, 2f)), 16 + Main.rand.Next(2));
                }
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Pixie);
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
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center, new Vector2(Main.rand.NextFloat(-4f, 4f) + x * 0.75f, Main.rand.NextFloat(-4f, 4f)), 16 + Main.rand.Next(2));
            }
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
                NPC.ai[3] -= 0.66f;
                if (Main.rand.NextBool())
                {
                    var d = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Unit() * NPC.ai[3] / 2f, ModContent.DustType<MonoDust>(), newColor: Color.Lerp(Color.Yellow.UseB(128), Color.White, Math.Min(Main.rand.NextFloat(0.5f, 1f) - NPC.ai[3] / 60f, 1f)).UseA(0));
                    d.velocity *= 0.2f;
                    d.velocity += (NPC.Center - d.position) / 16f;
                }
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
                if (WorldGen.InWorld(tileX, tileY + i, 10) && Main.tile[tileX, tileY + i].IsSolid())
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
            {
                rotationSpeed = 1f;
            }
            if (rotationSpeed <= 0.01f)
            {
                rotationSpeed = 0f;
            }
            else
            {
                rotationSpeed *= 0.95f;
                NPC.rotation += rotationSpeed * 0.1f;
            }
            if (rotationSpeed > 0.1f)
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

        public override void FindFrame(int frameHeight)
        {
            if ((int)NPC.ai[1] == -1)
            {
                return;
            }

            NPC.frameCounter++;
            if (NPC.frameCounter >= 10.0)
            {
                NPC.frameCounter = 0.0;
                NPC.frame.Y = (NPC.frame.Y + frameHeight) % (frameHeight * Main.npcFrameCount[Type]);
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return 0f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var texture = TextureAssets.Npc[NPC.type].Value;
            var offset = new Vector2(NPC.width / 2f, NPC.height / 2f);
            var origin = NPC.frame.Size() / 2f;
            var drawPos = NPC.Center - screenPos;
            for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
            {
                float p = AequusHelpers.CalcProgress(NPCID.Sets.TrailCacheLength[NPC.type], i);
                Main.spriteBatch.Draw(texture, NPC.oldPos[i] + offset - Main.screenPosition, NPC.frame, new Color(200, 200, 200, 0) * p, NPC.oldRot[i], origin, NPC.scale * p, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.Draw(texture, drawPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, drawPos, NPC.frame, new Color(20, 20, 20, 0), NPC.rotation, origin, NPC.scale + 0.1f, SpriteEffects.None, 0f);
            if ((int)NPC.ai[1] == -1)
            {
                float scale = (float)Math.Pow(Math.Min(NPC.scale * (-NPC.ai[3] / 60f), 1f), 3f) * 1.25f;
                var shineColor = new Color(120, 120, 180, 0) * scale * NPC.Opacity;

                var lightRay = ModContent.Request<Texture2D>(Aequus.AssetsPath + "LightRay", AssetRequestMode.ImmediateLoad).Value;
                var lightRayOrigin = lightRay.Size() / 2f;

                int i = 0;
                foreach (float f in AequusHelpers.Circular(8, Main.GlobalTimeWrappedHourly * 0.8f + (int)(NPC.position.X * 2f + NPC.position.Y * 2f)))
                {
                    var rayScale = new Vector2(AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 0.8f + (int)(NPC.position.X + NPC.position.Y) + i * (int)NPC.position.Y, 0.3f, 1f));
                    rayScale.X *= 0.5f;
                    rayScale.X *= (float)Math.Pow(scale, Math.Min(rayScale.Y, 1f));
                    Main.spriteBatch.Draw(lightRay, drawPos, null, shineColor * scale * NPC.Opacity, f, lightRayOrigin, scale * rayScale, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(lightRay, drawPos, null, shineColor * 0.5f * scale * NPC.Opacity, f, lightRayOrigin, scale * rayScale * 2f, SpriteEffects.None, 0f);
                    i++;
                }

                var spotlightTexture = ModContent.Request<Texture2D>(Aequus.AssetsPath + "Bloom_20x20", AssetRequestMode.ImmediateLoad).Value;
                var spotlightOrigin = spotlightTexture.Size() / 2f;
                Main.spriteBatch.Draw(spotlightTexture, drawPos, null, shineColor * scale * NPC.Opacity, 0f, spotlightOrigin, scale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(spotlightTexture, drawPos, null, shineColor * 0.5f * scale * NPC.Opacity, 0f, spotlightOrigin, scale * 2f, SpriteEffects.None, 0f);

                Main.instance.LoadProjectile(ProjectileID.RainbowCrystalExplosion);
                var shine = TextureAssets.Projectile[ProjectileID.RainbowCrystalExplosion].Value;
                var shineOrigin = shine.Size() / 2f;
                Main.EntitySpriteDraw(shine, drawPos, null, shineColor, 0f, shineOrigin, new Vector2(NPC.scale * 0.5f, NPC.scale) * scale, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(shine, drawPos, null, shineColor, MathHelper.PiOver2, shineOrigin, new Vector2(NPC.scale * 0.5f, NPC.scale * 2f) * scale, SpriteEffects.None, 0);
            }
            return false;
        }
    }
}