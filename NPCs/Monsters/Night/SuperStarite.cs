using Aequus.Buffs.Debuffs;
using Aequus.Items.Consumables.Foods;
using Aequus.Items.Placeable.Banners;
using Aequus.Projectiles.Monster;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Monsters.Night
{
    public class SuperStarite : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 2;

            NPCID.Sets.TrailingMode[Type] = 7;
            NPCID.Sets.TrailCacheLength[Type] = 15;
            NPCID.Sets.DebuffImmunitySets.Add(Type, new Terraria.DataStructures.NPCDebuffImmunityData()
            {
                SpecificallyImmuneTo = new int[]
                {
                    BuffID.Confused,
                    BuffID.OnFire,
                    BuffID.OnFire3,
                    BuffID.ShadowFlame,
                    BuffID.Frostburn,
                    BuffID.Frostburn2,
                    BuffID.Ichor,
                    BuffID.Poisoned,
                    BuffID.Venom,
                    BuffID.Bleeding,
                    BuffID.Weak,
                    BuffID.Stinky,
                    BuffID.Lovestruck,
                    BuffID.Wet,
                    BuffID.Slimed,
                    ModContent.BuffType<CrimsonHellfire>(),
                    ModContent.BuffType<CorruptionHellfire>(),
                },
            });
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            this.CreateLoot(npcLoot)
                .Add<NeutronYogurt>(chance: 2, stack: 1)
                .Add(ItemID.Nazar, chance: 2, stack: 1);
        }

        public override void SetDefaults()
        {
            NPC.width = 80;
            NPC.height = 80;
            NPC.lifeMax = 115;
            NPC.damage = 25;
            NPC.defense = 9;
            NPC.HitSound = SoundID.NPCHit5;
            NPC.DeathSound = SoundID.NPCDeath55;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(silver: 30);
            NPC.npcSlots = 2f;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<SuperStariteBanner>();
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            float x = NPC.velocity.X.Abs() * hitDirection;
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 35; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 15);
                    Main.dust[d].velocity.X += x;
                    Main.dust[d].velocity.Y = -Main.rand.NextFloat(2f, 6f);
                }
                for (int i = 0; i < 50; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 57 + Main.rand.Next(2));
                    Main.dust[d].velocity.X += x;
                    Main.dust[d].velocity.Y = -Main.rand.NextFloat(2f, 6f);
                }
                for (int i = 0; i < 8; i++)
                {
                    Gore.NewGore(NPC.GetSource_OnHurt(null), NPC.Center, new Vector2(Main.rand.NextFloat(-5f, 5f) + x, Main.rand.NextFloat(-5f, 5f)), 16 + Main.rand.Next(2));
                }
            }
            else
            {
                for (int i = 0; i < 6; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 15);
                    Main.dust[d].velocity.X += x;
                    Main.dust[d].velocity.Y = -Main.rand.NextFloat(5f, 12f);
                }
                int d1 = Dust.NewDust(NPC.position, NPC.width, NPC.height, 57 + Main.rand.Next(2));
                Main.dust[d1].velocity.X += x;
                Main.dust[d1].velocity.Y = -Main.rand.NextFloat(2f, 6f);
                if (Main.rand.NextBool())
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, new Vector2(Main.rand.NextFloat(-4f, 4f) + x * 0.75f, Main.rand.NextFloat(-4f, 4f)), 16 + Main.rand.Next(2));
            }
        }

        private bool playerCheck()
        {
            NPC.TargetClosest(faceTarget: false);
            if (!NPC.HasValidTarget || Main.player[NPC.target].dead)
            {
                NPC.ai[0] = -1f;
                return false;
            }
            else
            {
                return true;
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

            if (Main.rand.NextBool(20))
            {
                int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 15);
                Main.dust[d].velocity = NPC.velocity * 0.01f;
            }
            if (Main.rand.NextBool(40))
            {
                int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 58);
                Main.dust[d].velocity.X = Main.rand.NextFloat(-4f, 4f);
                Main.dust[d].velocity.Y = Main.rand.NextFloat(-4f, 4f);
            }
            if (Main.rand.NextBool(40))
            {
                int g = Gore.NewGore(NPC.GetSource_FromThis(), NPC.position + new Vector2(Main.rand.Next(NPC.width - 4), Main.rand.Next(NPC.height - 4)), new Vector2(Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f)), 16 + Main.rand.Next(2));
                Main.gore[g].scale *= 0.6f;
            }
            Lighting.AddLight(NPC.Center, new Vector3(0.8f, 0.8f, 0.45f));

            Vector2 center = NPC.Center;
            const float collisionMult = 0.75f;
            bool collisonEffects = false;
            if (NPC.collideX)
            {
                if (NPC.oldVelocity.X.Abs() > 2f)
                {
                    NPC.velocity.X = -NPC.oldVelocity.X * collisionMult;
                    collisonEffects = true;
                }
            }
            if (NPC.collideY)
            {
                if (NPC.oldVelocity.Y.Abs() > 2f)
                {
                    NPC.velocity.Y = -NPC.oldVelocity.Y * collisionMult;
                    collisonEffects = true;
                }
            }
            if (collisonEffects)
            {
                for (int i = 0; i < 10f; i++)
                {
                    int d = Dust.NewDust(center + NPC.oldVelocity, NPC.width, NPC.height, DustID.MagicMirror);
                    Main.dust[d].velocity = NPC.velocity * 0.65f;
                }
            }

            if (NPC.ai[0] == -1f)
            {
                NPC.noTileCollide = true;
                NPC.velocity.X *= 0.965f;
                if (NPC.velocity.Y > 0f)
                    NPC.velocity.Y *= 0.985f;
                NPC.velocity.Y -= 0.055f;
                if (NPC.timeLeft > 100)
                    NPC.timeLeft = 100;
                return;
            }
            if (NPC.ai[0] == 0f)
            {
                NPC.TargetClosest(faceTarget: false);
                if (NPC.HasValidTarget)
                {
                    if (Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height) || NPC.life < NPC.lifeMax)
                    {
                        NPC.ai[0] = 1f;
                        NPC.ai[1] = 0f;
                    }
                    else
                    {
                        NPC.ai[1]++;
                        if (NPC.ai[1] >= 1200f)
                        {
                            NPC.timeLeft = 0;
                            NPC.ai[0] = -1f;
                        }
                        NPC.velocity *= 0.96f;
                        return;
                    }
                }
                else
                {
                    if (Main.player[NPC.target].dead)
                    {
                        NPC.ai[0] = -1f;
                        NPC.ai[1] = -0f;
                    }
                    NPC.ai[1]++;
                    if (NPC.ai[1] >= 1200f)
                    {
                        NPC.timeLeft = 0;
                        NPC.ai[0] = -1f;
                    }
                    NPC.velocity *= 0.96f;
                    return;
                }
            }
            if (NPC.ai[0] == 1f)
            {
                NPC.ai[1]++;
                NPC.velocity.Y -= 0.35f;
                if (NPC.velocity.Y < -8f || NPC.ai[1] > 50f)
                {
                    NPC.ai[0] = 2f;
                    NPC.ai[1] -= 50f;
                }
                return;
            }
            if (NPC.ai[0] == 2f)
            {
                NPC.ai[1]++;
                NPC.velocity.Y += 0.15f;
                if (NPC.velocity.Y >= 0f || NPC.ai[1] > 50f)
                {
                    if (playerCheck())
                    {
                        NPC.velocity.Y = 0f;
                        NPC.ai[0] = 3f;
                        NPC.ai[1] = 0f;
                    }
                }
                return;
            }
            Player player = Main.player[NPC.target];
            Vector2 plrCenter = player.Center;
            bool doRotate = true;
            if (NPC.ai[0] == 3f)
            {
                NPC.ai[1]++;
                float oldSpeed = NPC.velocity.Length();
                if (NPC.ai[2] != 0f && NPC.velocity.X != NPC.ai[2])
                {
                    NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, NPC.ai[2], 0.035f);
                    if ((NPC.velocity.X - NPC.ai[2]).Abs() < 0.1f)
                    {
                        NPC.velocity.X = NPC.ai[2];
                        NPC.ai[2] = 0f;
                    }
                }
                if (NPC.ai[3] != 0f && NPC.velocity.Y != NPC.ai[3])
                {
                    NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, NPC.ai[3], 0.035f);
                    if ((NPC.velocity.Y - NPC.ai[3]).Abs() < 0.1f)
                    {
                        NPC.velocity.Y = NPC.ai[3];
                        NPC.ai[3] = 0f;
                    }
                }
                if (NPC.velocity.Length() < oldSpeed)
                {
                    NPC.velocity = Vector2.Normalize(NPC.velocity) * oldSpeed;
                }
                if (NPC.ai[1] > 50f)
                {
                    if (playerCheck())
                    {
                        Vector2 difference = plrCenter + new Vector2(0f, player.height * -5f) - center;
                        float length = difference.Length();
                        Vector2 velocity = Vector2.Normalize(difference) * Main.rand.NextFloat(4f, 8f);
                        NPC.netUpdate = true;
                        if (length < 500f)
                        {
                            NPC.ai[1] = -125f;
                        }
                        else if (length < 800f)
                        {
                            NPC.ai[1] = -100f;
                        }
                        else if (length < 1500f)
                        {
                            NPC.ai[1] = -50f;
                        }
                        else if (length < 2000f)
                        {
                            NPC.timeLeft = 0;
                        }
                        NPC.ai[2] = velocity.X;
                        NPC.ai[3] = velocity.Y;
                    }
                }
                if (NPC.ai[1] >= 0f && NPC.ai[1] % 50 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item84.WithVolume(0.25f).WithPitch(0.2f), NPC.Center);
                    const float twoPiOver5 = MathHelper.TwoPi / 5f;
                    int damage = Main.hardMode && Main.expertMode ? 45 : 20;
                    int type = ModContent.ProjectileType<SuperStariteBullet>();
                    float length = (float)Math.Sqrt(NPC.width * NPC.width + NPC.height * NPC.height) / 2f;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            Vector2 normal = (twoPiOver5 * i + NPC.rotation).ToRotationVector2();
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), center + normal * length, normal * 9f, type, damage, 1f, Main.myPlayer);
                        }
                    }
                }
                NPC.velocity *= 0.995f;
                NPC.rotation += 0.0314f;
            }
            if (doRotate)
                NPC.rotation += NPC.velocity.Length() * 0.0157f;
        }

        public override bool? CanFallThroughPlatforms()
        {
            return true;
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

        public override void OnKill()
        {
            if (!AequusWorld.downedEventCosmic)
            {
                AequusWorld.downedEventCosmic = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var texture = TextureAssets.Npc[Type].Value;
            var origin = NPC.frame.Size() / 2f;
            var offset = new Vector2(NPC.width / 2f, NPC.height / 2f);
            float mult = 1f / NPCID.Sets.TrailCacheLength[NPC.type];
            float rotation = NPC.rotation - MathHelper.PiOver4 / 2.5f;
            float rotationOffset = MathHelper.PiOver2;
            float armsOffset = 28f * NPC.scale;
            var circular = AequusHelpers.CircularVector(5, rotation);
            var coreFrame = new Rectangle(NPC.frame.X, NPC.frame.Y + NPC.frame.Height, NPC.frame.Width, NPC.frame.Height);
            if (!NPC.IsABestiaryIconDummy)
            {
                for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                {
                    var trailColor = new Color(70, 30, 30, 0) * (mult * (NPCID.Sets.TrailCacheLength[NPC.type] - i));
                    for (int j = 0; j < circular.Length; j++)
                    {
                        Main.spriteBatch.Draw(texture, NPC.oldPos[i] + offset + circular[j] * armsOffset - screenPos, NPC.frame, trailColor, NPC.oldRot[i] + rotationOffset - MathHelper.PiOver4 / 2.5f + MathHelper.TwoPi / 5f * j, origin, NPC.scale, SpriteEffects.None, 0f);
                    }
                    Main.spriteBatch.Draw(texture, NPC.oldPos[i] + offset - screenPos, coreFrame, trailColor, 0f, origin, NPC.scale, SpriteEffects.None, 0f);
                }
            }

            for (int j = 0; j < circular.Length; j++)
                Main.spriteBatch.Draw(texture, NPC.position + offset + circular[j] * armsOffset - screenPos, NPC.frame, new Color(255, 255, 255, 255), rotation + rotationOffset + MathHelper.TwoPi / 5f * j, origin, NPC.scale, SpriteEffects.None, 0f);

            var bloom = TextureCache.Bloom[0].Value;
            Main.spriteBatch.Draw(bloom, NPC.position + offset - screenPos, null, Color.Yellow * 0.5f, 0f, bloom.Size() / 2f, NPC.scale * 0.6f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(bloom, NPC.position + offset - screenPos, null, Color.Yellow * 0.25f, 0f, bloom.Size() / 2f, NPC.scale * 0.9f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, NPC.position + offset - screenPos, coreFrame, new Color(255, 255, 255, 255), 0f, origin, NPC.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}