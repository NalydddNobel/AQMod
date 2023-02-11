using Aequus.Buffs.Debuffs;
using Aequus.Buffs.Necro;
using Aequus.Common;
using Aequus.Common.ItemDrops;
using Aequus.Common.ModPlayers;
using Aequus.Content.Necromancy;
using Aequus.Graphics;
using Aequus.Items;
using Aequus.Items.Accessories;
using Aequus.Items.Accessories.Vanity.Cursors;
using Aequus.Items.Consumables.Foods;
using Aequus.Items.Consumables.Permanent;
using Aequus.Items.Misc.Energies;
using Aequus.Items.Misc.Festive;
using Aequus.Items.Misc.Materials;
using Aequus.Items.Placeable;
using Aequus.NPCs.GlobalNPCs;
using Aequus.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Aequus.Items.Weapons.Magic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.NPCs
{
    public class AequusNPC : GlobalNPC, IPostSetupContent
    {
        public static FieldInfo NPC_waterMovementSpeed { get; private set; }
        public static FieldInfo NPC_lavaMovementSpeed { get; private set; }
        public static FieldInfo NPC_honeyMovementSpeed { get; private set; }

        public static HashSet<int> HeatDamage { get; private set; }

        public override bool InstancePerEntity => true;

        public bool heatDamage;
        public bool noHitEffect;
        public bool disabledContactDamage;

        public int oldLife;
        public float statAttackDamage;
        public byte mindfungusStacks;
        public byte corruptionHellfireStacks;
        public byte crimsonHellfireStacks;
        public byte locustStacks;
        public byte nightfallStacks;
        public float nightfallSpeed;
        public bool noAITest;
        public bool childNPC;
        public bool tempHide;
        public byte tempDontTakeDamage;

        public override void Load()
        {
            NPC_waterMovementSpeed = typeof(NPC).GetField("waterMovementSpeed", BindingFlags.NonPublic | BindingFlags.Instance);
            NPC_lavaMovementSpeed = typeof(NPC).GetField("lavaMovementSpeed", BindingFlags.NonPublic | BindingFlags.Instance);
            NPC_honeyMovementSpeed = typeof(NPC).GetField("honeyMovementSpeed", BindingFlags.NonPublic | BindingFlags.Instance);
            HeatDamage = new HashSet<int>();

            if (!Main.dedServ)
            {
                NPCID.Sets.TrailingMode[NPCID.Mimic] = 7;
            }

            AddHooks();
        }

        public void PostSetupContent(Aequus mod)
        {
            var contentFile = new ContentArrayFile("HeatDamage", NPCID.Search);
            contentFile.AddToHashSet("NPCs", HeatDamage);
        }

        public override void Unload()
        {
            if (!Main.dedServ)
            {
                NPCID.Sets.TrailingMode[NPCID.Mimic] = -1;
            }
            HeatDamage?.Clear();
            NPC_waterMovementSpeed = null;
            NPC_lavaMovementSpeed = null;
            NPC_honeyMovementSpeed = null;
        }

        public override void ModifyGlobalLoot(GlobalLoot globalLoot)
        {
            globalLoot.Add(ItemDropRule.ByCondition(new VictorsReward.DropCondition(), ModContent.ItemType<VictorsReward>()));
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            switch (npc.type)
            {
                case NPCID.Everscream:
                case NPCID.SantaNK1:
                case NPCID.IceQueen:
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.FromCertainWaveAndAbove(15), ModContent.ItemType<XmasEnergy>()));
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsChristmas(), ModContent.ItemType<XmasEnergy>()));
                    break;

                case NPCID.SlimeRibbonWhite:
                case NPCID.SlimeRibbonYellow:
                case NPCID.SlimeRibbonGreen:
                case NPCID.SlimeRibbonRed:
                case NPCID.ZombieXmas:
                case NPCID.ZombieSweater:
                case NPCID.MisterStabby:
                case NPCID.SnowmanGangsta:
                case NPCID.SnowBalla:
                case NPCID.ZombieElf:
                case NPCID.ZombieElfBeard:
                case NPCID.ZombieElfGirl:
                case NPCID.ElfArcher:
                case NPCID.Nutcracker:
                case NPCID.NutcrackerSpinning:
                case NPCID.Yeti:
                case NPCID.ElfCopter:
                case NPCID.Krampus:
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsChristmas(), ModContent.ItemType<XmasEnergy>()));
                    break;

                case NPCID.Scarecrow1:
                case NPCID.Scarecrow2:
                case NPCID.Scarecrow3:
                case NPCID.Scarecrow4:
                case NPCID.Scarecrow5:
                case NPCID.Scarecrow6:
                case NPCID.Scarecrow7:
                case NPCID.Scarecrow8:
                case NPCID.Scarecrow9:
                case NPCID.Scarecrow10:
                    npcLoot.Add(ItemDropRule.ByCondition(new IsHalloweenCondition(), ModContent.ItemType<HalloweenEnergy>()));
                    npcLoot.Add(new NameTagDropRule(new ItemDrop(ItemID.Ale, 1), "Press B.", new NameTagCondition("birdy", "beardy")));
                    break;

                case NPCID.Bunny:
                case NPCID.ExplosiveBunny:
                case NPCID.BunnyXmas:
                case NPCID.BunnySlimed:
                    npcLoot.Add(new NameTagDropRule(new ItemDrop(ModContent.ItemType<RabbitsFoot>(), 1), "You're a Monster.", new NameTagCondition("toast")));
                    break;

                case NPCID.Moth:
                case NPCID.Mothron:
                case NPCID.MothronSpawn:
                    npcLoot.Add(ItemDropRule.ByCondition(new NameTagCondition("cata", "cataclysmic", "armageddon", "cataclysmicarmageddon", "cataclysmic armageddon"), ModContent.ItemType<MothmanMask>()));
                    break;

                case NPCID.Unicorn:
                    npcLoot.Add(new NameTagDropRule(new ItemDrop(ModContent.ItemType<RabbitsFoot>(), 1), "Tattered Pegasus Wings", new NameTagCondition("pegasus")));
                    break;

                case NPCID.Crab:
                    npcLoot.Add(new NameTagDropRule(new ItemDrop(ItemID.GoldCoin, 1), "Me first dollar!", new NameTagCondition("mr krabs", "krabs", "mr. krabs")));
                    break;

                case NPCID.Pumpking:
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.FromCertainWaveAndAbove(15), ModContent.ItemType<HalloweenEnergy>()));
                    npcLoot.Add(ItemDropRule.ByCondition(new IsHalloweenCondition(), ModContent.ItemType<HalloweenEnergy>()));
                    break;
                case NPCID.MourningWood:
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.FromCertainWaveAndAbove(15), ModContent.ItemType<HalloweenEnergy>(), 5));
                    npcLoot.Add(ItemDropRule.ByCondition(new IsHalloweenCondition(), ModContent.ItemType<HalloweenEnergy>()));
                    break;

                case NPCID.SlimeMasked:
                case NPCID.ZombieDoctor:
                case NPCID.ZombieSuperman:
                case NPCID.ZombiePixie:
                case NPCID.DemonEyeOwl:
                case NPCID.DemonEyeSpaceship:
                case NPCID.Raven:
                case NPCID.SkeletonTopHat:
                case NPCID.SkeletonAstonaut:
                case NPCID.SkeletonAlien:
                case NPCID.Ghost:
                case NPCID.HoppinJack:
                case NPCID.Splinterling:
                case NPCID.Hellhound:
                case NPCID.HeadlessHorseman:
                case NPCID.Poltergeist:
                    npcLoot.Add(ItemDropRule.ByCondition(new IsHalloweenCondition(), ModContent.ItemType<HalloweenEnergy>()));
                    break;

                case NPCID.Demon:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DemonCursor>(), 100));
                    break;

                case NPCID.Pixie:
                    //npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PixieCandle>(), 100));
                    break;

                case NPCID.BloodNautilus:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BloodyTearFragment>(), minimumDropped: 6, maximumDropped: 12));
                    break;

                case NPCID.BloodEelHead:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BloodyTearFragment>(), minimumDropped: 3, maximumDropped: 6));
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SpicyEel>(), 10));
                    break;
                case NPCID.GoblinShark:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BloodyTearFragment>(), minimumDropped: 3, maximumDropped: 6));
                    break;

                case NPCID.Clown:
                case NPCID.EyeballFlyingFish:
                case NPCID.ZombieMerman:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BloodyTearFragment>(), minimumDropped: 1, maximumDropped: 3));
                    break;

                case NPCID.BloodZombie:
                case NPCID.Drippler:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BloodyTearFragment>(), 25));
                    //npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BloodMoonCandle>(), 100));
                    break;

                case NPCID.DevourerHead:
                case NPCID.GiantWormHead:
                case NPCID.BoneSerpentHead:
                case NPCID.TombCrawlerHead:
                case NPCID.DiggerHead:
                case NPCID.DuneSplicerHead:
                case NPCID.SeekerHead:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SpicyEel>(), 10));
                    break;

                case NPCID.MoonLordCore:
                    if (GameplayConfig.Instance.EarlyGravityGlobe)
                        npcLoot.RemoveWhere((itemDrop) => itemDrop is ItemDropWithConditionRule dropRule && dropRule.itemId == ItemID.GravityGlobe);
                    if (GameplayConfig.Instance.EarlyPortalGun)
                        npcLoot.RemoveWhere((itemDrop) => itemDrop is ItemDropWithConditionRule dropRule && dropRule.itemId == ItemID.PortalGun);
                    break;

                case NPCID.CultistBoss:
                    npcLoot.Add(ItemDropRule.ByCondition(DropRulesBuilder.FlawlessCondition, ModContent.ItemType<MothmanMask>()));
                    break;

                case NPCID.Plantera:
                    npcLoot.Add(ItemDropRule.ByCondition(DropRulesBuilder.NotExpertCondition, ModContent.ItemType<OrganicEnergy>(), 1, 3, 3));
                    break;

                case NPCID.WallofFlesh:
                    npcLoot.Add(ItemDropRule.ByCondition(new FuncConditional(() => !AequusWorld.downedEventDemon, "DemonSiege", "Mods.Aequus.DropCondition.NotBeatenDemonSiege"), ModContent.ItemType<GoreNest>()));
                    break;
            }
        }

        public override void SetDefaults(NPC npc)
        {
            if (HeatDamage.Contains(npc.type))
            {
                heatDamage = true;
            }
            statAttackDamage = 1f;
            noAITest = false;
            tempHide = false;
        }

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (AequusHelpers.HereditarySource(source, out var ent))
            {
                childNPC = true;
                NecromancyNPC.InheritFromParent(npc, ent);
            }
        }

        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            return !disabledContactDamage;
        }

        public override bool? CanHitNPC(NPC npc, NPC target)
        {
            return disabledContactDamage ? false : null;
        }

        public override bool? CanBeHitByItem(NPC npc, Player player, Item item)
        {
            return tempDontTakeDamage > 0 ? false : null;
        }

        public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile)
        {
            return tempDontTakeDamage > 0 ? false : null;
        }

        public override void ResetEffects(NPC npc)
        {
            disabledContactDamage = false;
            statAttackDamage = 1f;
            if (nightfallStacks > 0 && !npc.HasBuff<NightfallDebuff>())
            {
                nightfallStacks = 0;
                nightfallSpeed = 0f;
            }
            if (tempDontTakeDamage > 0)
                tempDontTakeDamage--;
        }

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (tempHide)
            {
                tempHide = false;
                return false;
            }
            if (npc.type == NPCID.Mimic && npc.frame.Y >= npc.frame.Height * 6 && npc.frame.Y < npc.frame.Height * 12)
            {
                var texture = ModContent.Request<Texture2D>($"{this.GetNoNamePath()}/Vanilla/AdamantiteMimic");
                var frame = texture.Value.Frame(verticalFrames: 6, frameY: npc.frame.Y / npc.frame.Height % 6);
                int trailLength = Math.Min(NPCID.Sets.TrailCacheLength[npc.type], 6);
                var offset = npc.Size / 2f + new Vector2(0f, -7f);
                var origin = frame.Size() / 2f;
                var spriteDirection = (-npc.spriteDirection).ToSpriteEffect();
                for (int i = 0; i < trailLength; i++)
                {
                    if (i < trailLength - 1 && (npc.oldPos[i] - npc.oldPos[i + 1]).Length() < 1f)
                    {
                        continue;
                    }
                    spriteBatch.Draw(texture.Value, (npc.oldPos[i] - screenPos + offset).Floor(), frame,
                        AequusHelpers.GetColor(npc.oldPos[i] + offset) * AequusHelpers.CalcProgress(trailLength, i) * 0.4f, npc.rotation, origin, npc.scale, spriteDirection, 0f);
                }
                spriteBatch.Draw(texture.Value, (npc.position - screenPos+ offset).Floor(), frame, 
                    drawColor, npc.rotation, origin, npc.scale, spriteDirection, 0f);
                return false;
            }
            return true;
        }

        public override bool PreAI(NPC npc)
        {
            if (npc.type == NPCID.Probe)
            {
                npc.buffImmune[ModContent.BuffType<OsirisDebuff>()] = false;
                npc.buffImmune[ModContent.BuffType<InsurgentDebuff>()] = false;
            }
            if (nightfallStacks > 0 && !npc.noTileCollide && npc.knockBackResist > 0f)
            {
                if (npc.velocity.Y != npc.oldVelocity.Y && npc.oldVelocity.Y >= 1f && (npc.velocity.Y.Abs() < 0.5f || npc.collideY))
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float diff = nightfallSpeed;
                        if (diff > 1f)
                        {
                            int damage = (int)diff * 2;
                            if (damage > 25)
                            {
                                damage -= 25;
                                damage /= 2;
                                damage += 25;
                            }
                            if (damage > 35)
                            {
                                damage -= 35;
                                damage /= 2;
                                damage += 35;
                            }
                            if (damage > 50)
                            {
                                damage -= 50;
                                damage /= 2;
                                damage += 50;
                            }
                            npc.StrikeNPCNoInteraction(damage, 0f, 0);
                            npc.velocity *= 0.2f;
                            npc.netUpdate = true;
                            if (Main.netMode == NetmodeID.Server)
                            {
                                NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, npc.whoAmI, damage);
                            }
                        }
                    }
                    if (nightfallSpeed > 1f)
                    {
                        if (Main.netMode != NetmodeID.Server)
                        {
                            for (int i = 0; i < npc.width / 2; i++)
                            {
                                var d = Dust.NewDustDirect(npc.position + new Vector2(-8f, npc.height - 4), npc.width + 16, 4, DustID.RainbowMk2, 0f, -5f, newColor: new Color(128, 128, 128, 0), Scale: Main.rand.NextFloat(1f, 1.5f));
                                d.fadeIn = d.scale + 0.2f;
                                d.noGravity = true;
                            }
                        }
                        SoundEngine.PlaySound(SoundID.Item14.WithVolume(0.5f), npc.Center);
                    }
                    nightfallSpeed = 0f;
                }

                if (npc.velocity.Y > 0.5f)
                {
                    if (Main.netMode != NetmodeID.Server && Main.GameUpdateCount % 3 == 0)
                    {
                        var d = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.AncientLight, 0f, -npc.velocity.Y * 0.4f, 100, Scale: Main.rand.NextFloat(1f, 1.5f));
                        d.fadeIn = d.scale + 0.05f;
                        d.noGravity = true;
                    }

                    float amt = npc.knockBackResist * 0.15f * nightfallStacks + npc.velocity.Y * 0.01f;
                    nightfallSpeed += amt;
                    if (npc.velocity.Y < 10f)
                    {
                        npc.velocity.Y += amt;
                        if (npc.velocity.Y > 10f)
                        {
                            npc.velocity.Y = 10f;
                        }
                    }
                }
                if (nightfallSpeed < 0f || npc.velocity.Y < 0f)
                    nightfallSpeed = 0f;
            }
            return !noAITest;
        }
        public void DebuffEffects(NPC npc)
        {
            if (npc.HasBuff<AethersWrath>())
            {
                var colors = new Color[] { new Color(80, 180, 255, 10), new Color(255, 255, 255, 10), new Color(100, 255, 100, 10), };
                int amt = (int)(npc.Size.Length() / 16f);
                for (int i = 0; i < amt; i++)
                {
                    var spawnLocation = Main.rand.NextCircularFromRect(npc.getRect()) + Main.rand.NextVector2Unit() * 8f;
                    spawnLocation.Y -= 4f;
                    var color = AequusHelpers.LerpBetween(colors, spawnLocation.X / 32f + Main.GlobalTimeWrappedHourly * 4f);
                    EffectsSystem.ParticlesBehindPlayers.Add(new BloomParticle(spawnLocation, -npc.velocity * 0.1f + new Vector2(Main.rand.NextFloat(-1f, 1f), -Main.rand.NextFloat(2f, 6f)),
                        color, color.UseA(0).HueAdd(Main.rand.NextFloat(0.02f)) * 0.1f, Main.rand.NextFloat(1f, 2f), 0.2f, Main.rand.NextFloat(MathHelper.TwoPi)));
                    if (i == 0 && Main.GameUpdateCount % 12 == 0)
                    {
                        var velocity = -npc.velocity * 0.1f + new Vector2(Main.rand.NextFloat(-6f, 6f), -Main.rand.NextFloat(4f, 7f));
                        float scale = Main.rand.NextFloat(1.2f, 2.2f);
                        float rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                        EffectsSystem.ParticlesBehindPlayers.Add(new AethersWrathParticle(spawnLocation, velocity, color, scale * 1.2f, rotation));
                    }
                }
            }
            if (npc.HasBuff<BlueFire>())
            {
                int amt = (int)(npc.Size.Length() / 16f);
                for (int i = 0; i < amt; i++)
                    EffectsSystem.ParticlesBehindPlayers.Add(new BloomParticle(Main.rand.NextCircularFromRect(npc.getRect()) + Main.rand.NextVector2Unit() * 8f, -npc.velocity * 0.1f + new Vector2(Main.rand.NextFloat(-1f, 1f), -Main.rand.NextFloat(2f, 6f)),
                        new Color(60, 100, 160, 10) * 0.5f, new Color(5, 20, 40, 10), Main.rand.NextFloat(1f, 2f), 0.2f, Main.rand.NextFloat(MathHelper.TwoPi)));
            }
            if (npc.HasBuff<CorruptionHellfire>())
            {
                int amt = (int)(npc.Size.Length() / 16f);
                for (int i = 0; i < amt; i++)
                    EffectsSystem.ParticlesBehindPlayers.Add(new BloomParticle(Main.rand.NextCircularFromRect(npc.getRect()) + Main.rand.NextVector2Unit() * 8f, -npc.velocity * 0.1f + new Vector2(Main.rand.NextFloat(-1f, 1f), -Main.rand.NextFloat(2f, 6f)),
                        CorruptionHellfire.FireColor, CorruptionHellfire.BloomColor * 0.6f, Main.rand.NextFloat(1f, 2f), 0.2f, Main.rand.NextFloat(MathHelper.TwoPi)));
            }
            if (npc.HasBuff<CrimsonHellfire>())
            {
                int amt = (int)(npc.Size.Length() / 16f);
                for (int i = 0; i < amt; i++)
                    EffectsSystem.ParticlesBehindPlayers.Add(new BloomParticle(Main.rand.NextCircularFromRect(npc.getRect()) + Main.rand.NextVector2Unit() * 8f, -npc.velocity * 0.1f + new Vector2(Main.rand.NextFloat(-1f, 1f), -Main.rand.NextFloat(2f, 6f)),
                        CrimsonHellfire.FireColor, CrimsonHellfire.BloomColor * 0.2f, Main.rand.NextFloat(1f, 2f), 0.2f, Main.rand.NextFloat(MathHelper.TwoPi)));
            }
        }
        public override void PostAI(NPC npc)
        {
            oldLife = npc.life;

            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }
            DebuffEffects(npc);
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (npc.HasBuff<SnowgraveDebuff>())
            {
                if (Main.GameUpdateCount % 9 == 0)
                {
                    var d = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.IceRod, Scale: Main.rand.NextFloat(0.6f, 1f));
                    d.velocity = npc.velocity * 0.5f;
                    d.noGravity = true;
                    d.fadeIn = d.scale + 0.5f;
                }
            }
            if (npc.HasBuff<BoneRingWeakness>())
            {
                byte a = drawColor.A;
                drawColor = (drawColor * 0.8f).UseA(a);
                if (Main.GameUpdateCount % 10 == 0)
                {
                    var d = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.Slush, Alpha: 160, Scale: Main.rand.NextFloat(0.6f, 1f));
                    d.velocity = npc.velocity.SafeNormalize(Vector2.Zero);
                    d.noGravity = true;
                    d.fadeIn = d.scale + 0.5f;
                }
                if (npc.life >= 0 && Main.GameUpdateCount % 30 == 0)
                {
                    npc.HitEffect(0, 10);
                }
                if (!npc.noTileCollide)
                {
                    int x = (int)((npc.position.X + npc.width / 2f) / 16f);
                    int checkTilesTop = (int)((npc.position.Y + npc.height) / 16f);
                    int checkTilesBottom = (int)(npc.position.Y / 16f);
                    for (int j = checkTilesBottom; j <= checkTilesTop; j++)
                    {
                        if (Main.tile[x, j].IsFullySolid())
                        {
                            var d = Dust.NewDustPerfect(new Vector2(npc.position.X + Main.rand.NextFloat(npc.width), npc.position.Y + npc.height), DustID.Slush, Vector2.Zero, 160, Scale: Main.rand.NextFloat(0.6f, 1f));
                            d.noGravity = true;
                            d.fadeIn = d.scale + 0.5f;
                            break;
                        }
                    }
                }
            }
            if (npc.HasBuff<BattleAxeBleeding>())
            {
                if (Main.GameUpdateCount % 10 == 0)
                {
                    bool foodParticle = Main.rand.NextBool();
                    var d = Dust.NewDustDirect(npc.position, npc.width, npc.height, foodParticle ? DustID.Blood : DustID.FoodPiece, newColor: foodParticle ? new Color(200, 20, 30, 100) : default);
                    d.velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(1f, 2f);
                    d.velocity += npc.velocity * 0.5f;
                }
                if (npc.life >= 0 && Main.GameUpdateCount % 50 == 0)
                {
                    npc.HitEffect(0, 10);
                }
            }
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            bool applyAequusOiled = false;
            if (npc.HasBuff<AethersWrath>())
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= 50;
                applyAequusOiled = true;
                damage = Math.Max(damage, 10);
            }
            if (npc.HasBuff<MindfungusDebuff>())
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= 16 + 8 * mindfungusStacks;
                damage = Math.Max(damage, 5 + mindfungusStacks);
            }
            else
            {
                mindfungusStacks = 0;
            }
            if (npc.HasBuff<BlueFire>())
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= 30;
                applyAequusOiled = true;
                damage = Math.Max(damage, 6);
            }
            if (npc.HasBuff<BattleAxeBleeding>())
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= 16;
            }
            if (npc.HasBuff<WindFanDebuff>())
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= 10;
                damage = Math.Max(damage, 2);
            }
            bool debuff = npc.HasBuff<CorruptionHellfire>();
            applyAequusOiled |= debuff;
            UpdateDebuffStack(npc, debuff, ref corruptionHellfireStacks, ref damage, 20, 1f);
            debuff = npc.HasBuff<CrimsonHellfire>();
            applyAequusOiled |= debuff;
            UpdateDebuffStack(npc, debuff, ref crimsonHellfireStacks, ref damage, 20, 1.1f);

            UpdateDebuffStack(npc, npc.HasBuff<LocustDebuff>(), ref locustStacks, ref damage, 20, 1f);

            if (npc.oiled && applyAequusOiled && !(npc.onFire || npc.onFire2 || npc.onFire3 || npc.onFrostBurn || npc.onFrostBurn2 || npc.shadowFlame))
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= 50;
                damage = Math.Max(damage, 10);
            }
        }
        public void UpdateDebuffStack(NPC npc, bool has, ref byte stacks, ref int twoSecondsDamageNumbers, byte cap = 20, float dotMultiplier = 1f)
        {
            if (!has)
            {
                stacks = 0;
            }
            else
            {
                stacks = Math.Min(stacks, cap);
                int dot = (int)(stacks * dotMultiplier);

                if (dot >= 0)
                {
                    npc.AddRegen(-dot * 8);
                    twoSecondsDamageNumbers = Math.Max(twoSecondsDamageNumbers, (int)(dot * dotMultiplier));
                }
            }
        }

        public override bool SpecialOnKill(NPC npc)
        {
            if (npc.TryGetGlobalNPC<NecromancyNPC>(out var zombie) && zombie.isZombie)
            {
                return true;
            }

            if (Main.netMode != NetmodeID.Server)
            {
                if (npc.HasBuff<SnowgraveDebuff>() && SnowgraveCorpse.CanFreezeNPC(npc))
                {
                    EffectsSystem.ParticlesBehindAllNPCs.Add(new SnowgraveCorpse(npc.Center, npc));
                }
            }
            return false;
        }

        public override void OnKill(NPC npc)
        {
            if (npc.SpawnedFromStatue || npc.friendly || npc.lifeMax < 5)
                return;

            if (npc.TryGetGlobalNPC<NecromancyNPC>(out var zombie))
            {
                zombie.CreateGhost(npc);
            }

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (npc.playerInteraction[i])
                {
                    var info = new EnemyKillInfo(npc);
                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Main.player[i].Aequus().OnKillEffect(info);
                        continue;
                    }

                    var p = Aequus.GetPacket(PacketType.OnKillEffect);
                    p.Write(i);
                    info.WriteData(p);
                    p.Send(toClient: i);
                }
            }
        }

        public override bool PreChatButtonClicked(NPC npc, bool firstButton)
        {
            if (npc.type == NPCID.Angler)
            {
                if (firstButton)
                {
                    var inv = Main.LocalPlayer.inventory;
                    for (int i = 0; i < Main.InventoryItemSlotsCount; i++)
                    {
                        if (!inv[i].IsAir && AequusItem.LegendaryFishIDs.Contains(inv[i].type))
                        {
                            if (Main.npcChatCornerItem != inv[i].type)
                            {
                                Main.npcChatCornerItem = inv[i].type;
                                Main.npcChatText = AequusText.GetText("Chat.Angler.LegendaryFish");
                                return false;
                            }
                            Main.npcChatCornerItem = 0;
                            Main.npcChatText = AequusText.GetText("Chat.Angler.LegendaryFishReward");
                            Main.LocalPlayer.GetModPlayer<AnglerQuestRewards>().LegendaryFishRewards(npc, inv[i], i);
                            inv[i].stack--;
                            if (inv[i].stack <= 0)
                            {
                                inv[i].TurnToAir();
                            }
                            SoundEngine.PlaySound(SoundID.Grab);
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            var bb = new BitsByte(locustStacks > 0, corruptionHellfireStacks > 0, crimsonHellfireStacks > 0, mindfungusStacks > 0, nightfallStacks > 0, childNPC, tempDontTakeDamage > 0);
            binaryWriter.Write(bb);
            if (bb[0])
            {
                binaryWriter.Write(locustStacks);
            }
            if (bb[1])
            {
                binaryWriter.Write(corruptionHellfireStacks);
            }
            if (bb[2])
            {
                binaryWriter.Write(crimsonHellfireStacks);
            }
            if (bb[3])
            {
                binaryWriter.Write(mindfungusStacks);
            }
            if (bb[4])
            {
                binaryWriter.Write(nightfallStacks);
                binaryWriter.Write(nightfallSpeed);
            }
            if (bb[6])
            {
                binaryWriter.Write(tempDontTakeDamage);
            }
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            var bb = (BitsByte)binaryReader.ReadByte();
            if (bb[0])
            {
                locustStacks = binaryReader.ReadByte();
            }
            if (bb[1])
            {
                corruptionHellfireStacks = binaryReader.ReadByte();
            }
            if (bb[2])
            {
                crimsonHellfireStacks = binaryReader.ReadByte();
            }
            if (bb[3])
            {
                mindfungusStacks = binaryReader.ReadByte();
            }
            if (bb[4])
            {
                nightfallStacks = binaryReader.ReadByte();
                nightfallSpeed = binaryReader.ReadSingle();
            }
            childNPC = bb[5];
            if (bb[6])
            {
                tempDontTakeDamage = binaryReader.ReadByte();
            }
        }

        #region Hooks
        private static void AddHooks()
        {
            On.Terraria.NPC.Transform += NPC_Transform;
            On.Terraria.NPC.UpdateNPC_Inner += NPC_UpdateNPC_Inner; // fsr detouring NPC.Update(int) doesn't work, but this does
            On.Terraria.NPC.VanillaHitEffect += Hook_PreHitEffect;
        }

        private static void NPC_Transform(On.Terraria.NPC.orig_Transform orig, NPC npc, int newType)
        {
            string nameTag = null;
            if (npc.TryGetGlobalNPC<NPCNameTag>(out var nameTagNPC))
            {
                nameTag = nameTagNPC.NameTag;
                switch (npc.type)
                {
                    case NPCID.Bunny:
                    case NPCID.BunnySlimed:
                    case NPCID.BunnyXmas:
                    case NPCID.ExplosiveBunny:
                        if (nameTagNPC.HasNameTag && nameTagNPC.NameTag.ToLower() == "toast")
                        {
                            return;
                        }
                        break;
                }
            }

            var info = GhostSyncInfo.GetInfo(npc);

            orig(npc, newType);

            if (info.IsZombie)
            {
                info.SetZombieNPCInfo(npc, npc.GetGlobalNPC<NecromancyNPC>());
            }
            if (npc.TryGetGlobalNPC(out nameTagNPC))
            {
                nameTagNPC.NameTag = nameTag;
            }
        }

        private static void NPC_UpdateNPC_Inner(On.Terraria.NPC.orig_UpdateNPC_Inner orig, NPC self, int i)
        {
            if (self.TryGetGlobalNPC<BitCrushedGlobalNPC>(out var bitCrushed) && !bitCrushed.CheckUpdateNPC(self, i))
                return;
            if (AequusHelpers.iterations == 0 && self.TryGetGlobalNPC<NecromancyNPC>(out var zombie) && zombie.isZombie)
            {
                var aequus = Main.player[zombie.zombieOwner].Aequus();
                if (aequus.ghostShadowDash > 0)
                {
                    if (zombie.shadowDashTimer > 240 - aequus.ghostShadowDash * 10)
                    {
                        AequusHelpers.iterations++;
                        for (int k = 0; k < 3; k++)
                        {
                            for (int l = 0; l < 2; l++)
                            {
                                var d = Dust.NewDustDirect(self.position, self.width, self.height, DustID.Smoke, Alpha: 100, newColor: Color.Black, Scale: Main.rand.NextFloat(1f, 1.5f));
                                d.velocity *= 0.25f;
                                d.velocity -= self.velocity * 0.25f;
                            }
                            if (self.velocity.Length() < 3f)
                                self.velocity = Vector2.Normalize(self.velocity).UnNaN() * 3f;
                            var v = self.velocity;
                            orig(self, i);
                            self.velocity = Vector2.Lerp(v, self.velocity, 0.2f);
                            if (Vector2.Distance(self.oldPosition, self.position).UnNaN() < 1f)
                            {
                                break;
                            }
                        }
                        AequusHelpers.iterations = 0;
                    }
                }
                else
                {
                    zombie.shadowDashTimer = 0;
                }
            }
            orig(self, i);
        }

        private static void Hook_PreHitEffect(On.Terraria.NPC.orig_VanillaHitEffect orig, NPC self, int hitDirection, double dmg)
        {
            try
            {
                if (self.TryGetGlobalNPC<AequusNPC>(out var aequus))
                {
                    if (aequus.noHitEffect)
                    {
                        return;
                    }
                }

                if (Main.netMode == NetmodeID.Server)
                {
                    goto Orig;
                }

                if (self.HasBuff<BattleAxeBleeding>())
                {
                    int amt = (int)Math.Min(4.0 + dmg / 20.0, 20.0);
                    for (int i = 0; i < amt; i++)
                    {
                        bool foodParticle = Main.rand.NextBool();
                        var d = Dust.NewDustDirect(self.position, self.width, self.height, foodParticle ? DustID.Blood : DustID.FoodPiece, newColor: foodParticle ? new Color(200, 20, 30, 100) : default);
                        d.velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(2f, 5f);
                        d.velocity += self.velocity * 0.5f;
                        if (Main.rand.NextBool(3))
                        {
                            d.noGravity = true;
                        }
                    }
                }

                if (self.life <= 0 && self.HasBuff<SnowgraveDebuff>()
                    && SnowgraveCorpse.CanFreezeNPC(self))
                {
                    SoundEngine.PlaySound(SoundID.Item30, self.Center);
                    return;
                }
            }
            catch
            {
            }
        Orig:
            orig(self, hitDirection, dmg);
        }
        #endregion
    }
}