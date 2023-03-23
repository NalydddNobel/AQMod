using Aequus.Common.ItemDrops;
using Aequus.Content;
using Aequus.Content.Events;
using Aequus.Items.Tools;
using Aequus.Projectiles.Monster;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Aequus.NPCs.Monsters.Sky
{
    public class Meteor : ModNPC
    {
        public static SoundStyle CRUNCHSonicBreakingSound { get; private set; }

        public float GetExplosionIntent()
        {
            float lifeRatio = NPC.life / (float)NPC.lifeMax;
            float threshold = 0.5f;
            float amount = 0f;
            for (int i = 0; i < 10; i++)
            {
                if (lifeRatio > threshold)
                {
                    return amount;
                }
                amount += 0.1f;
                threshold /= 2f;
                continue;
            }
            return amount;
        }

        public override void Load()
        {
            if (!Main.dedServ)
            {
                CRUNCHSonicBreakingSound = Aequus.GetSound("sonicMeteor", 0.6f);
            }
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5;
            PushableEntities.NPCIDs.Add(Type);
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Position = new Vector2(0f, 8f),
            });
            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData()
            {
                ImmuneToAllBuffsThatAreNotWhips = true,
                ImmuneToWhips = true,
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 30;
            NPC.height = 30;
            NPC.lifeMax = 100;
            NPC.damage = 20;
            NPC.defense = 32;
            NPC.HitSound = SoundID.NPCHit7;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(silver: 2);
            NPC.npcSlots = 1f;
            NPC.Aequus().noGravityDrops = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            this.CreateEntry(database, bestiaryEntry)
                .AddMainSpawn(BestiaryBuilder.SkyBiome)
                .QuickUnlock();
        }

        public override void AI()
        {
            if ((int)NPC.ai[0] == 0)
            {
                NPC.ai[0] = 1f;
                NPC.velocity = new Vector2(Main.rand.NextFloat(1f, 2.5f), 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi));
                NPC.localAI[0] = Main.rand.Next(Main.npcFrameCount[NPC.type]) + 1f;
            }
            if (!GaleStreamsBiomeManager.IsThisSpace(NPC.position.Y))
            {
                NPC.noGravity = false;
                if (NPC.collideX || NPC.collideY)
                {
                    NPC.TargetClosest(faceTarget: false);
                    NPC.defense = 0;
                    NPC.ai[0] = 2f;
                    if (NPC.HasValidTarget)
                    {
                        Main.player[NPC.target].ApplyDamageToNPC(NPC, NPC.lifeMax, NPC.velocity.Length(), 0, false);
                    }
                    else
                    {
                        NPC.life = -1;
                        NPC.HitEffect();
                        NPC.active = false;
                    }
                    NPC.life = -1;
                    var p = NPC.Center.ToTileCoordinates();
                    SoundEngine.PlaySound(SoundID.NPCDeath14);
                    for (int i = 0; i < 80; i++)
                    {
                        int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Torch, 0f, 0f, 0, default(Color), Main.rand.NextFloat(0.8f, 2f));
                        Main.dust[d].noGravity = true;
                        Main.dust[d].velocity = (Main.dust[d].position - NPC.Center) / 8f;
                    }
                    if (Main.netMode != NetmodeID.MultiplayerClient && NPC.downedBoss2 && NPC.oldVelocity.Length() > 7.5f)
                    {
                        GaleStreamsBiomeManager.CrashMeteor(p.X, p.Y, 24, scatter: 1, scatterAmount: 4, scatterChance: 10, holeSizeDivider: 3, doEffects: true, tileType: TileID.Meteorite);
                    }
                }
            }
            else if (!GaleStreamsBiomeManager.IsThisSpace(NPC.position.Y + 600f))
            {
                NPC.velocity.Y -= 0.01f;
            }
            else if (NPC.position.Y < 200)
            {
                NPC.velocity.Y += 0.05f;
            }
            float lifeRatio = (NPC.life / (float)NPC.lifeMax);
            if (NPC.velocity.Length() > 3f * lifeRatio)
            {
                NPC.velocity *= 0.99f;
            }
            NPC.defense = (int)Math.Min(NPC.defense, NPC.defDefense * lifeRatio);
            NPC.rotation += NPC.velocity.Length() * 0.00157f;

            if (NPC.life < NPC.lifeMax / 2)
            {
                int amt = (int)(10 * (1f - NPC.life / (float)NPC.lifeMax * 2f));
                for (int i = 0; i < amt; i++)
                {
                    if (Main.rand.NextBool(24 - amt * 2))
                    {
                        var d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Torch, Scale: 2f);
                        d.velocity = (NPC.Center - d.position) / 8f;
                        d.position -= d.velocity * (24f + amt);
                        d.velocity *= 2f;
                        d.noGravity = true;
                    }
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            int frame = (int)(NPC.localAI[0] - 1f);
            if (NPC.IsABestiaryIconDummy)
            {
                frame = Helper.TimedBasedOn((int)Main.GameUpdateCount, 30, Main.npcFrameCount[NPC.type]);
            }
            NPC.frame.Y = frameHeight * frame;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            this.CreateLoot(npcLoot)
                .Add<Pumpinator>(chance: 15, stack: 1)
                .Add(new OneFromOptionsWithStackWithCondition(new OreTierCondition(0, true), new ItemDrop(ItemID.CopperOre, 1, 3), new ItemDrop(ItemID.CopperBar, 1)))
                .Add(new OneFromOptionsWithStackWithCondition(new OreTierCondition(0, false), new ItemDrop(ItemID.TinOre, 1, 3), new ItemDrop(ItemID.TinBar, 1)))
                .Add(new OneFromOptionsWithStackWithCondition(new OreTierCondition(1, true), new ItemDrop(ItemID.IronOre, 1, 3), new ItemDrop(ItemID.IronBar, 1)))
                .Add(new OneFromOptionsWithStackWithCondition(new OreTierCondition(1, false), new ItemDrop(ItemID.LeadOre, 1, 3), new ItemDrop(ItemID.LeadBar, 1)))
                .Add(new OneFromOptionsWithStackWithCondition(new OreTierCondition(2, true), new ItemDrop(ItemID.SilverOre, 1, 4), new ItemDrop(ItemID.SilverBar, 1)))
                .Add(new OneFromOptionsWithStackWithCondition(new OreTierCondition(2, false), new ItemDrop(ItemID.TungstenOre, 1, 4), new ItemDrop(ItemID.TungstenBar, 1)))
                .Add(new OneFromOptionsWithStackWithCondition(new OreTierCondition(3, true), new ItemDrop(ItemID.GoldOre, 1, 4), new ItemDrop(ItemID.GoldBar, 1)))
                .Add(new OneFromOptionsWithStackWithCondition(new OreTierCondition(3, false), new ItemDrop(ItemID.PlatinumOre, 1, 4), new ItemDrop(ItemID.PlatinumBar, 1)))
                .Add(new FuncConditional(() => NPC.downedBoss2, "Evil Bosses", "Mods.Aequus.DropCondition.Boss2"), ItemID.Meteorite, chance: 1, stack: (3, 12));
        }

        public override void UpdateLifeRegen(ref int damage)
        {
            if (NPC.lifeRegen > 0)
            {
                NPC.lifeRegen = 0;
            }
            NPC.lifeRegen -= (int)(80f * GetExplosionIntent());
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            float intensity = NPC.life * 2 < NPC.lifeMax ? 1f - MathF.Pow(NPC.life / (NPC.lifeMax / 2f), 2f) : 0f;

            var texture = TextureAssets.Npc[Type].Value;
            var drawCoords = NPC.Center - screenPos;
            drawColor = Color.Lerp(NPC.GetNPCColorTintedByBuffs(drawColor), Color.Orange * 1.2f, intensity);
            if (intensity > 0f)
            {
                spriteBatch.Draw(AequusTextures.Bloom0, drawCoords, null, Color.Lerp(Color.Red, Color.Yellow, intensity).UseA(0) * 0.2f * intensity, 0f, AequusTextures.Bloom0.Size() / 2f, NPC.scale, SpriteEffects.None, 0f);

                foreach (var v in Helper.CircularVector(4, NPC.rotation + Main.GlobalTimeWrappedHourly * 0.1f))
                {
                    spriteBatch.Draw(texture, drawCoords + v * 4f * intensity, NPC.frame, Color.Orange.UseA(0) * intensity, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0f);
                }

                var rand = new FastRandom(NPC.whoAmI * 3);
                var rayTexture = AequusTextures.LightRay2;
                var rayOrigin = new Vector2(rayTexture.Value.Width / 2f, rayTexture.Value.Height);
                var rayColor = Color.Orange.UseA(0) * intensity;
                int amt = 4;
                float rot = MathHelper.TwoPi / amt;
                for (int i = 0; i < amt * 2; i++)
                {
                    float rayOpacity = Helper.Wave(Main.GlobalTimeWrappedHourly * rand.Float(0.5f, 0.6f + i % amt * 0.4f), 0.2f, 1f);
                    float rayScale = rand.Float(0.5f, 0.8f) + Helper.Wave(Main.GlobalTimeWrappedHourly * rand.Float(0.5f, 0.6f + i % amt * 0.4f), -0.1f, 0.1f);
                    float rayRotation = rot * i + rand.Float(rot) + Main.GlobalTimeWrappedHourly * rand.Float(0.5f + i % amt * 0.3f, 0.6f + i % amt * 0.4f) * (i >= amt ? -1 : 1) + NPC.rotation;
                    spriteBatch.Draw(rayTexture.Value, drawCoords, null, rayColor.UseA(100) * rayOpacity * rayScale, rayRotation, rayOrigin, new Vector2(rayScale * rand.Float(1f, 2f) * 0.3f, rayScale * 1.2f * intensity), SpriteEffects.None, 0f);
                }
            }

            spriteBatch.Draw(texture, drawCoords, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0f);

            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 25; i++)
                {
                    var d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.t_Meteor, Scale: Main.rand.NextFloat(1f, 1.5f));
                    d.noGravity = GaleStreamsBiomeManager.IsThisSpace(NPC.position.Y);
                    d.velocity = (d.position - NPC.Center) / 4f;
                }
                for (int i = 0; i < 60; i++)
                {
                    var d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Torch, Scale: Main.rand.NextFloat(0.8f, 2f));
                    d.noGravity = true;
                    d.velocity = (d.position - NPC.Center) / 2f;
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    ScreenShake.SetShake(19f, where: NPC.Center);
                    SoundEngine.PlaySound(CRUNCHSonicBreakingSound, NPC.Center);
                }
            }
            else
            {
                int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.t_Meteor, 0f, 0f, 0, default(Color), Main.rand.NextFloat(0.5f, 1f));
                Main.dust[d].noGravity = GaleStreamsBiomeManager.IsThisSpace(NPC.position.Y);
                Main.dust[d].velocity = (Main.dust[d].position - NPC.Center) / 8f;
            }
        }

        public override void OnKill()
        {
            Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.UnitY, ModContent.ProjectileType<MeteorRubble>(), 50, 1f, Main.myPlayer);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
        }
    }
}