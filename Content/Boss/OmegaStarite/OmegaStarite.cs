using Aequus.Biomes;
using Aequus.Biomes.Glimmer;
using Aequus.Buffs.Debuffs;
using Aequus.Common.ItemDrops;
using Aequus.Content.Boss.OmegaStarite.Rewards;
using Aequus.Content.Boss.OmegaStarite.Misc;
using Aequus.Items.Accessories.Passive;
using Aequus.Items.Materials.Energies;
using Aequus.Items.Placeable.Furniture.Paintings;
using Aequus.Items.Vanity.Pets.Light;
using Aequus.Items.Weapons.Melee;
using Aequus.NPCs;
using Aequus.Particles;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;

namespace Aequus.Content.Boss.OmegaStarite
{
    [AutoloadBossHead()]
    public partial class OmegaStarite : OmegaStariteBase
    {
        public const float BossProgression = 6.99f;

        public const float DEATHTIME = MathHelper.PiOver4 * 134;

        #region Load / Unload
        public override void Load()
        {
            LoadSounds();
            LoadResists();

            if (!Main.dedServ)
            {
                music = new ConfiguredMusicData(MusicID.Boss5, MusicID.OtherworldlyLunarBoss);
            }
        }

        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailingMode[NPC.type] = 7;
            NPCID.Sets.TrailCacheLength[NPC.type] = 60;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Position = new Vector2(0f, 2f),
            });
            NPCID.Sets.DebuffImmunitySets[NPC.type] = new NPCDebuffImmunityData()
            {
                SpecificallyImmuneTo = new int[]
                {
                    BuffID.Confused,
                    BuffID.OnFire,
                    BuffID.OnFire3,
                    BuffID.Poisoned,
                    BuffID.Frostburn,
                    BuffID.Frostburn2,
                    BuffID.Bleeding,
                    ModContent.BuffType<BlueFire>(),
                    ModContent.BuffType<BattleAxeBleeding>(),
                },
            };
            Main.npcFrameCount[NPC.type] = 14;

            SnowgraveCorpse.NPCBlacklist.Add(Type);
        }

        public override void Unload()
        {
            music = null;
            UnloadResists();
            UnloadSounds();
        }
        #endregion

        public static ConfiguredMusicData music { get; private set; }

        public float starDamageMultiplier;

        public override void SetDefaults()
        {
            base.SetDefaults();
            starDamageMultiplier = 0.8f;

            if (Main.getGoodWorld)
            {
                NPC.scale *= 0.5f;
                starDamageMultiplier *= 0.5f;
            }

            if (!Main.dedServ && music != null)
            {
                Music = music.GetID();
                SceneEffectPriority = SceneEffectPriority.BossLow;
                if (AprilFools.CheckAprilFools())
                {
                    NPC.GivenName = "Omega Starite, Living Galaxy the Omega Being";
                    Music = MusicID.WindyDay;
                    SceneEffectPriority = SceneEffectPriority.BossHigh;
                }
            }
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            base.ScaleExpertStats(numPlayers, bossLifeScale);
            if (Main.expertMode)
            {
                starDamageMultiplier *= 0.8f;
            }
        }

        #region Damage Resist
        public static HashSet<int> StarResistCatalogue { get; private set; }

        private void LoadResists()
        {
            StarResistCatalogue = new HashSet<int>()
            {
                ProjectileID.FallingStar,
                ProjectileID.StarCannonStar,
                ProjectileID.SuperStar,
                ProjectileID.StarCloakStar,
                ProjectileID.Starfury,
                ProjectileID.StarVeilStar,
                ProjectileID.StarWrath,
                ProjectileID.BeeCloakStar,
                ProjectileID.HallowStar,
                ProjectileID.ManaCloakStar,
                ProjectileID.SuperStarSlash,
            };
        }

        private void UnloadResists()
        {
            StarResistCatalogue?.Clear();
            StarResistCatalogue = null;
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (StarResistCatalogue.Contains(projectile.type))
            {
                damage = (int)(damage * starDamageMultiplier);
            }
        }
        #endregion

        #region Daytime 
        public void AI_CheckDaytime()
        {
            if (Main.dayTime)
            {
                NPC.Aequus().noOnKill = true;
            }
        }

        public override void UpdateLifeRegen(ref int damage)
        {
            if (Main.dayTime)
            {
                NPC.lifeRegen = -10000;
                damage = 100;
            }
        }
        #endregion

        #region Misc
        public void PassivelyKillFallenStars()
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].type == ProjectileID.FallingStar && NPC.Distance(Main.projectile[i].Center) < 2000f)
                {
                    Main.projectile[i].damage = 0;
                    Main.projectile[i].noDropItem = true;
                    Main.projectile[i].Kill();
                }
            }
        }

        public override Color? GetAlpha(Color drawColor)
        {
            return new Color(255, 255, 255, 240);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (Main.netMode == NetmodeID.Server || NPC.life <= 0)
            {
                return;
            }
            SoundEngine.PlaySound(Hit, NPC.Center);
            float x = NPC.velocity.X.Abs() * hitDirection;
            if (Main.rand.NextBool())
            {
                int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Enchanted_Pink);
                Main.dust[d].velocity.X += x;
                Main.dust[d].velocity.Y = Main.rand.NextFloat(2f, 6f);
            }
            if (Main.rand.NextBool(7))
            {
                Gore.NewGore(new EntitySource_HitEffect(NPC), NPC.Center, new Vector2(Main.rand.NextFloat(-4f, 4f) + x * 0.75f, Main.rand.NextFloat(-4f, 4f)), 16 + Main.rand.Next(2));
            }

            hitShake = Math.Max(hitShake, (byte)MathHelper.Clamp((int)(damage / 7), 5, 15));
        }

        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                PrepareOrbRenders();
            }

            if (NPC.ai[0] != ACTION_DEAD)
            {
                NPC.frameCounter++;
                if (NPC.frameCounter >= 6)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[NPC.type])
                        NPC.frame.Y = 0;
                }
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            this.CreateEntry(database, bestiaryEntry);
        }

        public override void OnKill()
        {
            GlimmerSystem.EndEvent();
            AequusWorld.MarkAsDefeated(ref AequusWorld.downedOmegaStarite, Type);
        }

        public bool IsUltimateRayActive()
        {
            return NPC.ai[0] == ACTION_LASER_ORBITAL_1 && NPC.ai[2] < 1200f;
        }
        #endregion

        public override bool CheckDead()
        {
            return true;
            if (Action == ACTION_DEAD || Main.dayTime)
            {
                NPC.lifeMax = -33333;
                return true;
            }
            //NPC.GetGlobalNPC<NoHitting>().preventNoHitCheck = true;
            NPC.ai[0] = ACTION_DEAD;
            NPC.ai[1] = 0f;
            NPC.ai[2] = 0f;
            NPC.ai[3] = 0f;
            NPC.velocity = new Vector2(0f, 0f);
            NPC.dontTakeDamage = true;
            NPC.life = NPC.lifeMax;
            return false;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Main.expertMode)
            {
                if (Main.rand.NextBool())
                    target.AddBuff(ModContent.BuffType<BlueFire>(), 120);
                if (Main.rand.NextBool())
                    target.AddBuff(BuffID.Blackout, 360);
            }
            else
            {
                if (Main.rand.NextBool())
                    target.AddBuff(BuffID.OnFire, 120);
                if (Main.rand.NextBool())
                    target.AddBuff(BuffID.Darkness, 120);
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return NPC.ai[0] != -1;
        }

        public override bool PreKill()
        {
            if (!NPC.Aequus().noOnKill)
            {
                NPC.NewNPCDirect(NPC.GetSource_Death(), NPC.Center, ModContent.NPCType<OmegaStariteDefeat>(), NPC.whoAmI, target: NPC.target);
            }
            return false;
        }

        #region Net Code
        public override void SendExtraAI(BinaryWriter writer)
        {
            if (rings == null)
            {
                writer.Write(0);
                return;
            }
            writer.Write(rings.Count);
            for (int i = 0; i < rings.Count; i++)
            {
                rings[i].SendNetPackage(writer);
            }
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            if (rings == null)
            {
                rings = new List<OmegaStariteRing>();
            }
            int amt = reader.ReadInt32();
            if (rings.Count != amt)
            {
                rings.Clear();
                SetupRings();
            }
            for (int i = 0; i < rings.Count; i++)
            {
                if (rings.Count < i || rings[i] == null)
                {
                    SetupRings();
                }
                rings[i].RecieveNetPackage(reader);
            }
        }
        #endregion
    }

    public abstract partial class OmegaStariteBase : AequusBoss
    {
        public const float DIAMETER = 120;
        public const float RADIUS = DIAMETER / 2f;

        public List<OmegaStariteRing> rings;
        public byte hitShake;

        #region Stats
        public override void SetDefaults()
        {
            NPC.width = 120;
            NPC.height = 120;
            NPC.lifeMax = 12000;
            NPC.damage = 45;
            NPC.defense = 18;
            NPC.DeathSound = SoundID.NPCDeath55;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(gold: 6);
            NPC.boss = true;
            NPC.npcSlots = 10f;
            NPC.noTileCollide = true;
            NPC.trapImmune = true;
            NPC.lavaImmune = true;

            this.SetBiome<GlimmerBiome>();
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.7f * bossLifeScale);
        }
        #endregion

        public void SetupRings()
        {
            var center = NPC.Center;
            rings = new List<OmegaStariteRing>();
            if (Main.expertMode)
            {
                rings.Add(new OmegaStariteRing(OmegaStariteRing.SEGMENTS_1, DIAMETER, OmegaStariteRing.SCALE_1));
                if (!Main.getGoodWorld)
                {
                    rings.Add(new OmegaStariteRing(OmegaStariteRing.SEGMENTS_2, DIAMETER * OmegaStariteRing.DIAMETERMULT_2_EXPERT, OmegaStariteRing.SCALE_2_EXPERT));
                }
                else
                {
                    rings.Add(new OmegaStariteRing(OmegaStariteRing.SEGMENTS_2, DIAMETER * OmegaStariteRing.DIAMETERMULT_2_EXPERT, OmegaStariteRing.SCALE_2_EXPERT));
                    rings.Add(new OmegaStariteRing(OmegaStariteRing.SEGMENTS_3, DIAMETER * OmegaStariteRing.DIAMETERMULT_3, OmegaStariteRing.RING_3_SCALE));
                }
            }
            else
            {
                rings.Add(new OmegaStariteRing(OmegaStariteRing.SEGMENTS_1, DIAMETER * 0.75f, OmegaStariteRing.SCALE_1));
                rings.Add(new OmegaStariteRing(OmegaStariteRing.SEGMENTS_2, DIAMETER * OmegaStariteRing.DIAMETERMULT_2, OmegaStariteRing.SCALE_2));
            }
            for (int i = 0; i < rings.Count; i++)
            {
                rings[i].MultScale(NPC.scale);
                rings[i].Update(center);
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.HealingPotion;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            this.CreateLoot(npcLoot)
                .Add<SupernovaFruit>(new LegacyFuncConditional(() => AequusWorld.downedOmegaStarite, "OmegaStarite"), chance: 1, stack: 1)
                .AddBossLoot<OmegaStariteTrophy, OmegaStariteRelic, OmegaStariteBag, DragonBall>()
                .AddFlawless<OriginPainting>()
                .ExpertDropForCrossModReasons<CelesteTorus>()
                .AddPerPlayer<CosmicEnergy>(chance: 1, stack: 3)
                
                .SetCondition(new Conditions.NotExpert())
                .Add<OmegaStariteMask>(chance: 7, stack: 1)
                .Add<UltimateSword>(chance: 1, stack: 1)
                .RegisterCondition();
        }
    }
}