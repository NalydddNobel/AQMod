using AQMod.Assets;
using AQMod.Assets.Textures;
using AQMod.Buffs.Debuffs;
using AQMod.Common;
using AQMod.Common.Config;
using AQMod.Common.Utilities;
using AQMod.Content.WorldEvents.CosmicEvent;
using AQMod.Items.Accessories;
using AQMod.Items.BuffItems.Foods;
using AQMod.Items.Materials.Energies;
using AQMod.Items.Placeable.Banners;
using AQMod.Items.Tools.MapMarkers;
using AQMod.Items.Vanities.Dyes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs.Monsters.CosmicEvent
{
    public class HyperStarite : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 2;
            NPCID.Sets.TrailingMode[npc.type] = 7;
            NPCID.Sets.TrailCacheLength[npc.type] = 15;
        }

        public static readonly Color SpotlightColor = new Color(100, 100, 10, 0);

        public override void SetDefaults()
        {
            npc.width = 50;
            npc.height = 50;
            npc.lifeMax = 225;
            npc.damage = 80;
            npc.defense = 8;
            npc.HitSound = SoundID.NPCHit5;
            npc.DeathSound = SoundID.NPCDeath55;
            npc.aiStyle = -1;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(gold: 2, silver: 50);
            npc.buffImmune[BuffID.OnFire] = true;
            npc.buffImmune[BuffID.CursedInferno] = true;
            npc.buffImmune[BuffID.Ichor] = true;
            npc.buffImmune[BuffID.BetsysCurse] = true;
            npc.buffImmune[ModContent.BuffType<Buffs.Debuffs.Sparkling>()] = true;
            npc.npcSlots = 4f;
            banner = npc.type;
            bannerItem = ModContent.ItemType<HyperStariteBanner>();
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            if (Main.hardMode)
            {
                npc.lifeMax = (int)(npc.lifeMax * 1.5f);
                npc.damage = (int)(npc.damage * 1.2f);
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            float x = npc.velocity.X.Abs() * hitDirection;
            if (npc.life <= 0)
            {
                for (int i = 0; i < 50; i++)
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, 15);
                    Main.dust[d].velocity.X += x;
                    Main.dust[d].velocity.Y = -Main.rand.NextFloat(2f, 6f);
                }
                for (int i = 0; i < 70; i++)
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, 57 + Main.rand.Next(2));
                    Main.dust[d].velocity.X += x;
                    Main.dust[d].velocity.Y = -Main.rand.NextFloat(2f, 6f);
                }
                for (int i = 0; i < 16; i++)
                {
                    Gore.NewGore(npc.Center, new Vector2(Main.rand.NextFloat(-5f, 5f) + x, Main.rand.NextFloat(-5f, 5f)), 16 + Main.rand.Next(2));
                }
            }
            else
            {
                for (int i = 0; i < 7; i++)
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, 15);
                    Main.dust[d].velocity.X += x;
                    Main.dust[d].velocity.Y = -Main.rand.NextFloat(5f, 12f);
                }
                int d1 = Dust.NewDust(npc.position, npc.width, npc.height, 57 + Main.rand.Next(2));
                Main.dust[d1].velocity.X += x;
                Main.dust[d1].velocity.Y = -Main.rand.NextFloat(2f, 6f);
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

        private float Speed() => Main.expertMode ? 25f : 18f;

        public override void AI()
        {
            if (GlimmerEvent.CheckStariteDeath(npc))
            {
                npc.life = -1;
                npc.HitEffect();
                npc.active = false;
                return;
            }
            if (Main.rand.NextBool(8))
            {
                int d = Dust.NewDust(npc.position, npc.width, npc.height, 58);
                Main.dust[d].velocity.X = Main.rand.NextFloat(-4f, 4f);
                Main.dust[d].velocity.Y = Main.rand.NextFloat(-4f, 4f);
            }
            if (Main.rand.NextBool(10))
            {
                int g = Gore.NewGore(npc.position + new Vector2(Main.rand.Next(npc.width - 4), Main.rand.Next(npc.height - 4)), new Vector2(Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f)), 16);
                Main.gore[g].scale *= 0.6f;
            }
            Lighting.AddLight(npc.Center, new Vector3(1.2f, 1.2f, 0.5f));
            Vector2 center = npc.Center;
            if (npc.ai[0] == -1f)
            {
                npc.noTileCollide = true;
                npc.velocity.X *= 0.95f;
                if (npc.velocity.Y > 0f)
                    npc.velocity.Y *= 0.96f;
                npc.velocity.Y -= 0.075f;
                if (npc.timeLeft > 100)
                    npc.timeLeft = 100;
                npc.rotation += npc.velocity.Length() * 0.0157f;
                return;
            }
            Player player = Main.player[npc.target];
            Vector2 plrCenter = player.Center;
            switch (npc.ai[0])
            {
                case 0f:
                {
                    npc.TargetClosest(faceTarget: false);
                    if (npc.HasValidTarget)
                    {
                        if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height) || npc.life < npc.lifeMax)
                        {
                            npc.ai[0] = 1f;
                            npc.ai[1] = 0f;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int i = 0; i < 5; i++)
                                {
                                    int damage = Main.expertMode ? 45 : 75;
                                    int p = Projectile.NewProjectile(center, new Vector2(0f, 0f), ModContent.ProjectileType<Projectiles.Monster.HyperStarite>(), damage, 1f, default, npc.whoAmI, i);
                                    Main.projectile[p].netUpdate = true;
                                }
                            }
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
                break;

                case 1f:
                {
                    npc.ai[1]++;
                    npc.velocity.Y -= 0.45f;
                    if (npc.ai[1] > 20f)
                    {
                        if (playerCheck())
                        {
                            if (Main.rand.NextBool())
                            {
                                npc.ai[0] = 6f;
                                npc.ai[1] = 2f;
                            }
                            else
                            {
                                npc.ai[0] = 2f;
                            }
                        }
                    }
                }
                break;

                case 2f:
                {
                    npc.ai[1]++;
                    if (npc.ai[1] < 180f)
                    {
                        Vector2 difference = plrCenter - center;
                        var normal = Vector2.Normalize(difference);
                        npc.velocity = Vector2.Lerp(npc.velocity, normal.RotatedBy(MathHelper.PiOver2) * 2f, 0.0165f);
                        npc.ai[3] = MathHelper.Lerp(npc.ai[3], 0f, 0.1f);
                        npc.TargetClosest(faceTarget: false);
                    }
                    else if (npc.ai[1] < 240f)
                    {
                        if (npc.ai[2] == 0f && playerCheck())
                        {
                            var target = Main.player[npc.target];
                            var difference = target.Center - center;
                            float speed = Speed();
                            int mult2 = (int)(difference.Length() / speed) * 2;
                            var gotoPos = target.Center + target.velocity * mult2;
                            npc.ai[2] = (center - gotoPos).ToRotation();
                            npc.netUpdate = true;
                        }
                        npc.velocity *= 0.99985f;
                    }
                    else if (playerCheck())
                    {
                        Main.PlaySound(SoundID.Item14.WithVolume(0.6f).WithPitchVariance(2.5f), center);
                        npc.velocity = new Vector2(-Speed(), 0f).RotatedBy(npc.ai[2]);
                        npc.ai[3] = 0f;
                        npc.ai[2] = 0f;
                        npc.ai[1] = 0f;
                        npc.ai[0] = 3f;
                        npc.netUpdate = true;
                    }
                    npc.rotation += npc.velocity.Length() * 0.0157f;
                }
                break;

                case 3f:
                {
                    npc.velocity = Vector2.Lerp(npc.velocity, Vector2.Normalize(plrCenter - center) * npc.velocity.Length(), 0.018f);
                    npc.velocity *= 0.9925f;
                    npc.rotation += npc.velocity.Length() * 0.0157f;
                    npc.ai[1]++;
                    if (npc.ai[1] > 480f || npc.velocity.Length() < 0.5f)
                    {
                        if (playerCheck())
                        {
                            if ((plrCenter - center).Length() < 1250f && Main.rand.NextBool())
                            {
                                npc.ai[0] = 4f;
                                npc.ai[1] = 0f;
                            }
                            else
                            {
                                npc.ai[0] = 6f;
                                npc.ai[1] = 4f;
                            }
                        }
                    }
                }
                break;

                case 4f:
                {
                    if (npc.velocity.Length() > 0.1f)
                    {
                        npc.velocity *= 0.975f;
                        npc.rotation += npc.velocity.Length() * 0.0157f;
                    }
                    else
                    {
                        npc.rotation += 0.0628f;
                        npc.ai[3] = MathHelper.Lerp(npc.ai[3], 350f, 0.0125f);
                        if (npc.ai[3] > 340f)
                            npc.ai[0] = 5f;
                    }
                }
                break;

                case 5f:
                {
                    npc.rotation += 0.0628f;
                    npc.ai[3] -= 2.5f;
                    if (npc.ai[3] <= 4f && playerCheck())
                    {
                        if ((plrCenter - center).Length() < 1250f && Main.rand.NextBool())
                        {
                            npc.ai[0] = 2f;
                        }
                        else
                        {
                            npc.ai[0] = 6f;
                            npc.ai[1] = 2f;
                        }
                        npc.ai[3] = 0f;
                    }
                }
                break;

                case 6f:
                {
                    if ((plrCenter - center).Length() > 500f)
                    {
                        npc.velocity = Vector2.Lerp(npc.velocity, Vector2.Normalize(plrCenter - center) * npc.velocity.Length(), 0.025f);
                        if (npc.velocity.Length() < Speed() / 2f)
                            npc.velocity *= 1.035f;
                    }
                    else if (playerCheck())
                    {
                        npc.ai[0] = npc.ai[1];
                        npc.ai[1] = 0f;
                    }
                    npc.rotation += npc.velocity.Length() * 0.0157f;
                }
                break;
            }
            if (npc.velocity.Length() < 1.5f && center.Y + 160f > plrCenter.Y && Collision.SolidCollision(npc.position, npc.width, npc.height))
                npc.velocity.Y -= 0.35f;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Main.expertMode)
            {
                target.AddBuff(ModContent.BuffType<BlueFire>(), 360);
                target.AddBuff(BuffID.Blackout, 1800);
                if (Main.rand.NextBool(4))
                    target.AddBuff(BuffID.Cursed, 120);
            }
            else
            {
                if (Main.rand.NextBool(4))
                    target.AddBuff(BuffID.OnFire, 360);
                if (Main.rand.NextBool())
                    target.AddBuff(BuffID.Darkness, 1800);
                if (Main.rand.NextBool(12))
                    target.AddBuff(BuffID.Cursed, 120);
            }
        }

        public override void NPCLoot()
        {
            WorldDefeats.DownedGlimmer = true;
            if (AQNPC.CanDropEnergy)
                Item.NewItem(npc.getRect(), ModContent.ItemType<CosmicEnergy>());
            for (int i = 0; i < 2; i++)
                Item.NewItem(npc.getRect(), ModContent.ItemType<NeutronJuice>());
            if (Main.rand.NextBool(4))
            {
                var drops = new List<int>()
                {
                    ModContent.ItemType<MoonShoes>(),
                    ModContent.ItemType<RetroGoggles>(),
                };
                if (NPC.downedBoss2)
                    drops.Add(ModContent.ItemType<Ultranium>());
                Item.NewItem(npc.getRect(), drops[Main.rand.Next(drops.Count)]);
            }
            if (Main.rand.NextBool(30))
                Item.NewItem(npc.getRect(), ItemID.Nazar);
            if (Main.rand.NextBool(50))
                Item.NewItem(npc.getRect(), ModContent.ItemType<ScrollDye>());
        }

        public override int SpawnNPC(int tileX, int tileY)
        {
            return NPC.NewNPC(tileX * 16 + 8, tileY * 16 - 80, npc.type);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = Main.npcTexture[npc.type];
            Vector2 origin = npc.frame.Size() / 2f;
            var offset = new Vector2(npc.width / 2f, npc.height / 2f);
            float mult = 1f / NPCID.Sets.TrailCacheLength[npc.type];
            var frame = new Rectangle(npc.frame.X, npc.frame.Y + npc.frame.Height, npc.frame.Width, npc.frame.Height);
            var armLength = new Vector2(npc.height * npc.scale + npc.ai[3] + 18f, 0f);
            var texture1 = TextureCache.Lights[LightTex.Spotlight66x66];
            var frame1 = new Rectangle(0, 0, texture1.Width, texture1.Height);
            var origin1 = frame1.Size() / 2f;
            Main.spriteBatch.Draw(texture1, new Vector2((int)(npc.position.X + offset.X - Main.screenPosition.X), (int)(npc.position.Y + offset.Y - Main.screenPosition.Y)), frame1, SpotlightColor, 0f, origin1, npc.scale * 2, SpriteEffects.None, 0f);
            for (int i = 0; i < NPCID.Sets.TrailCacheLength[npc.type]; i++)
            {
                Color color = new Color(10, 10, 30, 0) * (mult * (NPCID.Sets.TrailCacheLength[npc.type] - i)) * ModContent.GetInstance<AQConfigClient>().EffectIntensity;
                Main.spriteBatch.Draw(texture, new Vector2((int)(npc.oldPos[i].X + offset.X - Main.screenPosition.X), (int)(npc.oldPos[i].Y + offset.Y - Main.screenPosition.Y)), npc.frame, color, 0f, origin, npc.scale, SpriteEffects.None, 0f);
                color = new Color(25, 25, 80, 4) * (mult * (NPCID.Sets.TrailCacheLength[npc.type] - i)) * ModContent.GetInstance<AQConfigClient>().EffectIntensity * 0.6f;
                for (int j = 0; j < 5; j++)
                {
                    float rotation = npc.oldRot[i] + MathHelper.TwoPi / 5f * j;
                    var armPos = npc.position + offset + armLength.RotatedBy(rotation - MathHelper.PiOver2) - Main.screenPosition;
                    Main.spriteBatch.Draw(texture, new Vector2((int)armPos.X, (int)armPos.Y), frame, color, rotation, origin, npc.scale, SpriteEffects.None, 0f);
                }
            }
            for (int i = 0; i < 5; i++)
            {
                float rotation = npc.rotation + MathHelper.TwoPi / 5f * i;
                var armPos = npc.position + offset + armLength.RotatedBy(rotation - MathHelper.PiOver2) - Main.screenPosition;
                Main.spriteBatch.Draw(texture, new Vector2((int)armPos.X, (int)armPos.Y), frame, new Color(255, 255, 255, 255), rotation, origin, npc.scale, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.Draw(texture, new Vector2((int)(npc.position.X + offset.X - Main.screenPosition.X), (int)(npc.position.Y + offset.Y - Main.screenPosition.Y)), npc.frame, new Color(255, 255, 255, 255), 0f, origin, npc.scale, SpriteEffects.None, 0f);
            if (npc.ai[0] == 2f)
            {
                var texture3 = TextureCache.Lights[LightTex.Spotlight33x24];
                float rotation;
                if (npc.ai[2] != 0f)
                {
                    rotation = npc.ai[2] + MathHelper.Pi;
                }
                else
                {
                    var target = Main.player[npc.target];
                    var difference = target.Center - (npc.position + offset);
                    float speed = Speed();
                    int mult2 = (int)(difference.Length() / speed) * 2;
                    var gotoPos = target.Center + target.velocity * mult2;
                    rotation = (gotoPos - (npc.position + offset)).ToRotation();
                }
                var drawPos = npc.position + offset + new Vector2(npc.height * 0.45f, 0f).RotatedBy(rotation) - Main.screenPosition;
                drawPos.X = (int)drawPos.X;
                drawPos.Y = (int)drawPos.Y;
                Main.spriteBatch.Draw(texture3, drawPos, null, new Color(255, 255, 25, 150), rotation + MathHelper.PiOver2, new Vector2(texture3.Width / 2f, texture3.Height), new Vector2(npc.scale * 1.5f, npc.scale * 2f + 2f), SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(texture3, drawPos, null, new Color(255, 255, 25, 150) * 0.2f, rotation + MathHelper.PiOver2, new Vector2(texture3.Width / 2f, texture3.Height), new Vector2(npc.scale * 0.5f, npc.scale * 2f + 40f), SpriteEffects.None, 0f);
            }
            Main.spriteBatch.Draw(texture, new Vector2((int)(npc.position.X + offset.X - Main.screenPosition.X), (int)(npc.position.Y + offset.Y - Main.screenPosition.Y)), npc.frame, new Color(60, 60, 60, 0) * AQMod.EffectIntensity, 0f, origin, npc.scale + 0.3f, SpriteEffects.None, 0f);
            return false;
        }
    }
}