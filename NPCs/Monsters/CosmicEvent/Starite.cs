using AQMod.Buffs.Debuffs;
using AQMod.Common;
using AQMod.Common.Utilities;
using AQMod.Content.WorldEvents.CosmicEvent;
using AQMod.Items.BuffItems.Foods;
using AQMod.Items.Materials.Energies;
using AQMod.Items.Placeable.Banners;
using AQMod.Items.Vanities.Dyes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs.Monsters.CosmicEvent
{
    public class Starite : ModNPC
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailingMode[npc.type] = 7;
            NPCID.Sets.TrailCacheLength[npc.type] = 12;
        }

        public override void SetDefaults()
        {
            npc.width = 20;
            npc.height = 20;
            npc.lifeMax = 45;
            npc.damage = 45;
            npc.defense = 3;
            npc.HitSound = SoundID.NPCHit39;
            npc.DeathSound = SoundID.NPCDeath55;
            npc.aiStyle = -1;
            npc.noGravity = true;
            npc.knockBackResist = 1.1f;
            npc.value = Item.buyPrice(silver: 2);
            npc.buffImmune[BuffID.OnFire] = true;
            banner = npc.type;
            bannerItem = ModContent.ItemType<StariteBanner>();
        }

        public override void AI()
        {
            Vector2 center = npc.Center;
            const float collisionMult = 0.8f;
            if (npc.collideX)
            {
                if (npc.oldVelocity.X.Abs() > 2f)
                    npc.velocity.X = -npc.oldVelocity.X * collisionMult;
                npc.ai[2] *= -collisionMult;
            }
            if (npc.collideY)
            {
                if (npc.oldVelocity.Y.Abs() > 2f)
                    npc.velocity.Y = -npc.oldVelocity.Y * collisionMult;
                npc.ai[3] *= -collisionMult;
            }
            if (npc.ai[0] == 0f)
            {
                npc.TargetClosest(faceTarget: false);
                if (npc.HasValidTarget && !Main.player[npc.target].dead && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                {
                    npc.ai[0] = 1f;
                    npc.velocity = new Vector2(Main.rand.NextFloat(4f, 6f), 0f).RotatedBy((Main.player[npc.target].Center - center).ToRotation());
                }
                else
                {
                    npc.velocity *= 0.985f;
                    return;
                }
            }
            var player = Main.player[npc.target];
            var plrCenter = player.Center;
            if (npc.ai[0] == 1f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (npc.ai[1] == 0f)
                    {
                        npc.ai[1] = Main.rand.Next(60, 100);
                        npc.netUpdate = true;
                    }
                    if (npc.ai[1] == 1f)
                    {
                        npc.ai[0] = 2f;
                        npc.netUpdate = true;
                    }
                }
                float turnSpeed = MathHelper.Clamp((npc.ai[1] + npc.localAI[0]) / 100000f, 0f, 1f); // don't even ask...
                npc.ai[1]--;
                if (npc.localAI[0] > 0)
                    npc.localAI[0]--;
                if (turnSpeed != 0f)
                {
                    float length = npc.velocity.Length();
                    Vector2 difference = plrCenter - center;
                    npc.velocity = Vector2.Normalize(Vector2.Lerp(npc.velocity, difference, turnSpeed)) * length;
                }
            }
            if (npc.ai[0] == 2f)
            {
                if (npc.ai[2] == 0f && npc.ai[3] == 0f)
                {
                    npc.TargetClosest(faceTarget: false);
                    if (npc.HasValidTarget && !Main.player[npc.target].dead && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                    {
                        float min;
                        float max;
                        float add;
                        if (Main.expertMode)
                        {
                            min = 4f;
                            max = 6.5f;
                            add = 2f;
                        }
                        else
                        {
                            min = 2f;
                            max = 3f;
                            add = 1.5f;
                        }
                        var gotoVelo = new Vector2(Main.rand.NextFloat(min, max) + add * (1f - npc.life / npc.lifeMax), 0f).RotatedBy((Main.player[npc.target].Center - center).ToRotation());
                        npc.ai[2] = gotoVelo.X;
                        npc.ai[3] = gotoVelo.Y;
                    }
                    else
                    {
                        npc.ai[0] = 0f;
                        return;
                    }
                }
                else
                {
                    var gotoVelo = new Vector2(npc.ai[2], npc.ai[3]);
                    float length = gotoVelo.Length();
                    npc.velocity = Vector2.Normalize(Vector2.Lerp(npc.velocity, gotoVelo, 0.05f)) * length;
                    bool xCloseEnough = (npc.velocity.X - gotoVelo.X).Abs() < 0.1f;
                    bool yCloseEnough = (npc.velocity.Y - gotoVelo.Y).Abs() < 0.1f;
                    if (Main.netMode != NetmodeID.MultiplayerClient && xCloseEnough && yCloseEnough)
                    {
                        npc.velocity.X = gotoVelo.X;
                        npc.velocity.Y = gotoVelo.Y;
                        npc.ai[0] = 1f;
                        npc.ai[2] = 0f;
                        npc.ai[3] = 0f;
                        npc.netUpdate = true;
                    }
                }
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
                int g = Gore.NewGore(npc.position + new Vector2(Main.rand.Next(npc.width - 4), Main.rand.Next(npc.height - 4)), new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f)), 16 + Main.rand.Next(2));
                Main.gore[g].scale *= 0.6f;
            }
            Lighting.AddLight(npc.Center, new Vector3(0.4f, 0.4f, 0.2f));
            npc.rotation += npc.velocity.Length() * 0.0157f;
            if (GlimmerEvent.CheckStariteDeath(npc))
            {
                npc.life = -1;
                npc.HitEffect();
                npc.active = false;
            }
        }

        private void OnHit(int plr, int damage)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                npc.ai[1] = MathHelper.Clamp(npc.ai[1] + Main.rand.Next(30, 50), 0f, 160f);
                npc.localAI[0] = MathHelper.Clamp(damage * 8, 0f, 800f);
                if (plr > 0 && plr < 255)
                    npc.target = plr;
                npc.netUpdate = true;
            }
        }

        public override void OnHitByItem(Player player, Item item, int damage, float knockback, bool crit)
        {
            OnHit(player.whoAmI, damage);
        }

        public override void OnHitByProjectile(Projectile projectile, int damage, float knockback, bool crit)
        {
            OnHit(projectile.owner, damage);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Main.expertMode)
            {
                target.AddBuff(ModContent.BuffType<BlueFire>(), 60);
                target.AddBuff(BuffID.Blackout, 300);
            }
            else
            {
                if (Main.rand.NextBool(4))
                    target.AddBuff(BuffID.OnFire, 60);
                if (Main.rand.NextBool())
                    target.AddBuff(BuffID.Darkness, 300);
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
                    Main.dust[d].velocity.Y = -Main.rand.NextFloat(2f, 6f);
                }
                for (int i = 0; i < 30; i++)
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, 57 + Main.rand.Next(2));
                    Main.dust[d].velocity.X += x;
                    Main.dust[d].velocity.Y = -Main.rand.NextFloat(2f, 6f);
                }
                for (int i = 0; i < 4; i++)
                {
                    Gore.NewGore(npc.Center, new Vector2(Main.rand.NextFloat(-5f, 5f) + x, Main.rand.NextFloat(-5f, 5f)), 16 + Main.rand.Next(2));
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

        public override void NPCLoot()
        {
            if (Main.rand.NextBool(50))
                Item.NewItem(npc.getRect(), ModContent.ItemType<Items.Vanities.CelesitalEightBall>());
            if (NPC.downedBoss1 && !AQNPC.NoEnergyDrops)
            {
                if (!AQMod.CosmicEvent.IsActive || Main.rand.NextBool(10))
                    Item.NewItem(npc.getRect(), ModContent.ItemType<CosmicEnergy>());
            }
            if (Main.rand.NextBool(5))
                Item.NewItem(npc.getRect(), ModContent.ItemType<NeutronJuice>());
            if (Main.rand.NextBool(50))
                Item.NewItem(npc.getRect(), ModContent.ItemType<HypnoDye>());
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = Main.npcTexture[npc.type];
            var offset = new Vector2(npc.width / 2f, npc.height / 2f);
            Vector2 origin = npc.frame.Size() / 2f;
            Vector2 drawPos = npc.Center - Main.screenPosition;
            float mult = 1f / NPCID.Sets.TrailCacheLength[npc.type];
            for (int i = 0; i < NPCID.Sets.TrailCacheLength[npc.type]; i++)
            {
                Main.spriteBatch.Draw(texture, npc.oldPos[i] + offset - Main.screenPosition, npc.frame, new Color(80, 80, 80, 0) * (mult * (NPCID.Sets.TrailCacheLength[npc.type] - i)), npc.oldRot[i], origin, npc.scale, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.Draw(texture, drawPos, npc.frame, new Color(255, 255, 255, 255), npc.rotation, origin, npc.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, drawPos, npc.frame, new Color(20, 20, 20, 0), npc.rotation, origin, npc.scale + 0.1f, SpriteEffects.None, 0f);
            return false;
        }
    }
}