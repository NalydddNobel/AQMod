using AQMod.Buffs.Debuffs;
using AQMod.Common;
using AQMod.Content.WorldEvents.CosmicEvent;
using AQMod.Items.Foods;
using AQMod.Items.Materials.Energies;
using AQMod.Items.Placeable.Banners;
using AQMod.Items.Vanities.Dyes;
using AQMod.Items.Weapons.Ranged;
using AQMod.Projectiles.Monster;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs.Monsters.CosmicEvent
{
    public class SuperStarite : ModNPC, IDecideFallThroughPlatforms
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailingMode[npc.type] = 7;
            NPCID.Sets.TrailCacheLength[npc.type] = 15;
        }

        public override void SetDefaults()
        {
            npc.width = 80;
            npc.height = 80;
            npc.lifeMax = 115;
            npc.damage = 25;
            npc.defense = 9;
            npc.HitSound = SoundID.NPCHit5;
            npc.DeathSound = SoundID.NPCDeath55;
            npc.aiStyle = -1;
            npc.noGravity = true;
            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(silver: 40);
            npc.npcSlots = 2f;
            Starite.BuffImmunities(npc);
            banner = npc.type;
            bannerItem = ModContent.ItemType<SuperStariteBanner>();
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            float x = npc.velocity.X.Abs() * hitDirection;
            if (npc.life <= 0)
            {
                for (int i = 0; i < 35; i++)
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, 15);
                    Main.dust[d].velocity.X += x;
                    Main.dust[d].velocity.Y = -Main.rand.NextFloat(2f, 6f);
                }
                for (int i = 0; i < 50; i++)
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, 57 + Main.rand.Next(2));
                    Main.dust[d].velocity.X += x;
                    Main.dust[d].velocity.Y = -Main.rand.NextFloat(2f, 6f);
                }
                for (int i = 0; i < 8; i++)
                {
                    Gore.NewGore(npc.Center, new Vector2(Main.rand.NextFloat(-5f, 5f) + x, Main.rand.NextFloat(-5f, 5f)), 16 + Main.rand.Next(2));
                }
            }
            else
            {
                for (int i = 0; i < 6; i++)
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, 15);
                    Main.dust[d].velocity.X += x;
                    Main.dust[d].velocity.Y = -Main.rand.NextFloat(5f, 12f);
                }
                int d1 = Dust.NewDust(npc.position, npc.width, npc.height, 57 + Main.rand.Next(2));
                Main.dust[d1].velocity.X += x;
                Main.dust[d1].velocity.Y = -Main.rand.NextFloat(2f, 6f);
                if (Main.rand.NextBool())
                    Gore.NewGore(npc.Center, new Vector2(Main.rand.NextFloat(-4f, 4f) + x * 0.75f, Main.rand.NextFloat(-4f, 4f)), 16 + Main.rand.Next(2));
            }
        }

        private bool playerCheck()
        {
            npc.TargetClosest(faceTarget: false);
            if (!npc.HasValidTarget || Main.player[npc.target].dead)
            {
                npc.ai[0] = -1f;
                return false;
            }
            else
            {
                return true;
            }
        }

        public override void AI()
        {
            Vector2 center = npc.Center;
            const float collisionMult = 0.75f;
            bool collisonEffects = false;
            if (npc.collideX)
            {
                if (npc.oldVelocity.X.Abs() > 2f)
                {
                    npc.velocity.X = -npc.oldVelocity.X * collisionMult;
                    collisonEffects = true;
                }
            }
            if (npc.collideY)
            {
                if (npc.oldVelocity.Y.Abs() > 2f)
                {
                    npc.velocity.Y = -npc.oldVelocity.Y * collisionMult;
                    collisonEffects = true;
                }
            }
            if (collisonEffects)
            {
                for (int i = 0; i < 10f; i++)
                {
                    int d = Dust.NewDust(center + npc.oldVelocity, npc.width, npc.height, 15);
                    Main.dust[d].velocity = npc.velocity * 0.65f;
                }
            }
            if (npc.ai[0] == -1f)
            {
                npc.noTileCollide = true;
                npc.velocity.X *= 0.965f;
                if (npc.velocity.Y > 0f)
                    npc.velocity.Y *= 0.985f;
                npc.velocity.Y -= 0.055f;
                if (npc.timeLeft > 100)
                    npc.timeLeft = 100;
                return;
            }
            if (npc.ai[0] == 0f)
            {
                npc.TargetClosest(faceTarget: false);
                if (npc.HasValidTarget)
                {
                    if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height) || npc.life < npc.lifeMax)
                    {
                        npc.ai[0] = 1f;
                        npc.ai[1] = 0f;
                    }
                    else
                    {
                        npc.ai[1]++;
                        if (npc.ai[1] >= 1200f)
                        {
                            npc.timeLeft = 0;
                            npc.ai[0] = -1f;
                        }
                        npc.velocity *= 0.96f;
                        return;
                    }
                }
                else
                {
                    if (Main.player[npc.target].dead)
                    {
                        npc.ai[0] = -1f;
                        npc.ai[1] = -0f;
                    }
                    npc.ai[1]++;
                    if (npc.ai[1] >= 1200f)
                    {
                        npc.timeLeft = 0;
                        npc.ai[0] = -1f;
                    }
                    npc.velocity *= 0.96f;
                    return;
                }
            }
            if (npc.ai[0] == 1f)
            {
                npc.ai[1]++;
                npc.velocity.Y -= 0.35f;
                if (npc.velocity.Y < -8f || npc.ai[1] > 50f)
                {
                    npc.ai[0] = 2f;
                    npc.ai[1] -= 50f;
                }
                return;
            }
            if (npc.ai[0] == 2f)
            {
                npc.ai[1]++;
                npc.velocity.Y += 0.15f;
                if (npc.velocity.Y >= 0f || npc.ai[1] > 50f)
                {
                    if (playerCheck())
                    {
                        npc.velocity.Y = 0f;
                        npc.ai[0] = 3f;
                        npc.ai[1] = 0f;
                    }
                }
                return;
            }
            Player player = Main.player[npc.target];
            Vector2 plrCenter = player.Center;
            bool doRotate = true;
            if (npc.ai[0] == 3f)
            {
                npc.ai[1]++;
                if (npc.ai[2] != 0f && npc.velocity.X != npc.ai[2])
                {
                    npc.velocity.X = MathHelper.Lerp(npc.velocity.X, npc.ai[2], 0.1f);
                    if ((npc.velocity.X - npc.ai[2]).Abs() < 0.1f)
                    {
                        npc.velocity.X = npc.ai[2];
                        npc.ai[2] = 0f;
                    }
                }
                if (npc.ai[3] != 0f && npc.velocity.Y != npc.ai[3])
                {
                    npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, npc.ai[3], 0.1f);
                    if ((npc.velocity.Y - npc.ai[3]).Abs() < 0.1f)
                    {
                        npc.velocity.Y = npc.ai[3];
                        npc.ai[3] = 0f;
                    }
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (npc.ai[1] > 50f)
                    {
                        if (playerCheck())
                        {
                            Vector2 difference = plrCenter + new Vector2(0f, player.height * -5f) - center;
                            float length = difference.Length();
                            Vector2 velocity = Vector2.Normalize(difference) * Main.rand.NextFloat(4f, 8f);
                            npc.netUpdate = true;
                            if (length < 500f)
                            {
                                npc.ai[1] = -125f;
                            }
                            else if (length < 800f)
                            {
                                npc.ai[1] = -100f;
                            }
                            else if (length < 1500f)
                            {
                                npc.ai[1] = -20f;
                            }
                            else if (length < 2000f)
                            {
                                npc.timeLeft = 0;
                            }
                            npc.ai[2] = velocity.X;
                            npc.ai[3] = velocity.Y;
                        }
                    }
                    if (npc.ai[1] % 50 == 0)
                    {
                        const float twoPiOver5 = MathHelper.TwoPi / 5f;
                        int damage = Main.hardMode && Main.expertMode ? 45 : 20;
                        int type = ModContent.ProjectileType<Projectiles.Monster.SuperStarite>();
                        float length = (float)Math.Sqrt(npc.width * npc.width + npc.height * npc.height) / 2f;
                        for (int i = 0; i < 5; i++)
                        {
                            Vector2 normal = new Vector2(1f, 0f).RotatedBy(twoPiOver5 * i + npc.rotation);
                            Projectile.NewProjectile(center + normal * length, normal * 9f, type, damage, 1f, Main.myPlayer);
                        }
                    }
                }
                npc.velocity *= 0.995f;
                npc.rotation += 0.0314f;
            }
            if (doRotate)
                npc.rotation += npc.velocity.Length() * 0.0157f;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Main.expertMode)
            {
                target.AddBuff(ModContent.BuffType<BlueFire>(), 120);
                target.AddBuff(BuffID.Blackout, 600);
                if (Main.rand.NextBool(4))
                    target.AddBuff(BuffID.Cursed, 120);
            }
            else
            {
                if (Main.rand.NextBool(4))
                    target.AddBuff(BuffID.OnFire, 120);
                if (Main.rand.NextBool())
                    target.AddBuff(BuffID.Darkness, 600);
                if (Main.rand.NextBool(12))
                    target.AddBuff(BuffID.Cursed, 120);
            }
        }

        public override void PostAI()
        {
            if (Main.rand.NextBool(20))
            {
                int d = Dust.NewDust(npc.position, npc.width, npc.height, 15);
                Main.dust[d].velocity = npc.velocity * 0.01f;
            }
            if (Main.rand.NextBool(40))
            {
                int d = Dust.NewDust(npc.position, npc.width, npc.height, 58);
                Main.dust[d].velocity.X = Main.rand.NextFloat(-4f, 4f);
                Main.dust[d].velocity.Y = Main.rand.NextFloat(-4f, 4f);
            }
            if (Main.rand.NextBool(40))
            {
                int g = Gore.NewGore(npc.position + new Vector2(Main.rand.Next(npc.width - 4), Main.rand.Next(npc.height - 4)), new Vector2(Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f)), 16 + Main.rand.Next(2));
                Main.gore[g].scale *= 0.6f;
            }
            Lighting.AddLight(npc.Center, new Vector3(0.8f, 0.8f, 0.45f));
            if (GlimmerEvent.CheckStariteDeath(npc))
            {
                npc.life = -1;
                npc.HitEffect();
                npc.active = false;
            }
        }

        public override void NPCLoot()
        {
            if (Main.rand.NextBool(4))
            {
                Item.NewItem(npc.getRect(), ModContent.ItemType<SpaceShot>());
            }
            WorldDefeats.DownedGlimmer = true;
            if (NPC.downedBoss1 && Main.rand.NextBool(5))
                Item.NewItem(npc.getRect(), ModContent.ItemType<CosmicEnergy>());

            if (Main.rand.NextBool())
                Item.NewItem(npc.getRect(), ModContent.ItemType<NeutronJuice>());
            if (Main.rand.NextBool(50))
                Item.NewItem(npc.getRect(), ItemID.Nazar);
            if (Main.rand.NextBool(50))
                Item.NewItem(npc.getRect(), ModContent.ItemType<OutlineDye>());
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = Main.npcTexture[npc.type];
            Vector2 origin = npc.frame.Size() / 2f;
            var offset = new Vector2(npc.width / 2f, npc.height / 2f);
            float mult = 1f / NPCID.Sets.TrailCacheLength[npc.type];
            for (int i = 0; i < NPCID.Sets.TrailCacheLength[npc.type]; i++)
            {
                Main.spriteBatch.Draw(texture, npc.oldPos[i] + offset - Main.screenPosition, npc.frame, new Color(70, 70, 70, 0) * (mult * (NPCID.Sets.TrailCacheLength[npc.type] - i)), npc.oldRot[i], origin, npc.scale, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.Draw(texture, npc.position + offset - Main.screenPosition, npc.frame, new Color(255, 255, 255, 255), npc.rotation, origin, npc.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, npc.position + offset - Main.screenPosition, npc.frame, new Color(20, 20, 20, 0) * ModContent.GetInstance<AQConfigClient>().EffectIntensity, npc.rotation, origin, npc.scale + 0.3f, SpriteEffects.None, 0f);
            return false;
        }

        bool IDecideFallThroughPlatforms.Decide() => true;
    }
}