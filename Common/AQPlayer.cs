using AQMod.Assets.Textures;
using AQMod.Common.Config;
using AQMod.Common.Utilities;
using AQMod.Content.Dusts;
using AQMod.Content.Skies;
using AQMod.Content.WorldEvents;
using AQMod.Effects.Screen;
using AQMod.Items;
using AQMod.Items.Accessories.Amulets;
using AQMod.Items.Accessories.FishingSeals;
using AQMod.Items.Fishing;
using AQMod.Items.Fishing.Bait;
using AQMod.Items.Fishing.QuestFish;
using AQMod.Items.Placeable.Walls;
using AQMod.Items.Placeable.WorldInteractors;
using AQMod.Items.TagItems.Starbyte;
using AQMod.Items.Weapons.Melee;
using AQMod.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace AQMod.Common
{
    public sealed class AQPlayer : ModPlayer
    {
        internal const int MaxArmor = 20;
        internal const int DyeWrap = MaxArmor / 2;

        public const int MaxCelesteTorusOrbs = 5;

        public int ExtractinatorCount { get; set; }

        public bool bossrush;
        public float discountPercentage;
        public bool blueSpheres;
        public bool bossChanneling;
        public bool monoxiderBird;
        public bool sparkling;
        public bool chloroTransfer;
        public bool opposingForce;
        public bool spectreSoulCollector;
        public bool moonShoes;
        public bool extractinator;
        public bool copperSeal;
        public bool silverSeal;
        public bool goldSeal;
        public bool canDash;
        public bool dartHead;
        public int dartHeadType;
        public int dartHeadDelay;
        public int dartTrapHatTimer;
        public int extraFlightTime;
        public int thunderbirdJumpTimer;
        public int thunderbirdLightningTimer;
        public int flyingSafe = -1;
        public bool dreadsoul;
        public bool arachnotron;
        public bool primeTime;
        public bool omori;
        public int omoriDeathTimer;
        public int spelunkerEquipTimer;
        public bool microStarite;
        public byte spoiled;
        public bool wyvernAmulet;
        public bool voodooAmulet;
        public bool ghostAmulet;
        public bool spiritAmulet;
        public bool extractinatorVisible;
        public float celesteTorusX;
        public float celesteTorusY;
        public float celesteTorusZ;
        public float celesteTorusRadius;
        public int celesteTorusDamage;
        public float celesteTorusKnockback;
        public int celesteTorusMaxRadius;
        public float celesteTorusSpeed;
        public float celesteTorusScale;
        public bool unityMirror;
        public bool stariteMinion;
        public bool spicyEel;
        public bool striderPalms;
        public bool striderPalms2;
        public bool wyvernAmuletHeld;
        public bool voodooAmuletHeld;
        public bool ghostAmuletHeld;
        public bool spiritAmuletHeld;
        public bool[] veinmineTiles;
        public bool degenerationRing;
        public byte weaponShootTag;
        public ushort shieldLife;
        public bool burnterizerCursor;

        public byte ClosestEnemy { get; private set; }
        public float ClosestEnemyDistance { get; private set; }

        public int PopperType { get; private set; }
        public int PopperBaitPower { get; private set; }
        public int FishingPowerCache { get; private set; }

        public bool TagProjectile(Projectile projectile)
        {
            if (!AQProjectile.Sets.UntaggableProjectile[projectile.type] && projectile.damage > 0 && !projectile.bobber && !projectile.noEnchantments && !projectile.counterweight && !projectile.minion)
            {
                var aQProj = projectile.GetGlobalProjectile<AQProjectile>();
                aQProj.projectileTag = weaponShootTag;
                return true;
            }
            return false;
        }

        public static void Setup()
        {
            On.Terraria.Player.FishingLevel += GetFishingLevel;
        }

        private static int GetFishingLevel(On.Terraria.Player.orig_FishingLevel orig, Player player)
        {
            int regularLevel = orig(player);
            if (regularLevel <= 0)
                return regularLevel;
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            Item baitItem = null;
            for (int j = 0; j < 58; j++)
            {
                if (player.inventory[j].stack > 0 && player.inventory[j].bait > 0)
                {
                    baitItem = player.inventory[j];
                    break;
                }
            }
            if (baitItem.modItem is PopperBait popper)
            {
                int popperPower = popper.GetExtraFishingPower(player, aQPlayer);
                if (popperPower > 0)
                {
                    aQPlayer.PopperType = baitItem.type;
                    aQPlayer.PopperBaitPower = popperPower;
                }
                else
                {
                    aQPlayer.PopperType = 0;
                    aQPlayer.PopperBaitPower = 0;
                }
            }
            else
            {
                aQPlayer.PopperType = 0;
                aQPlayer.PopperBaitPower = 0;
            }
            aQPlayer.FishingPowerCache = regularLevel + aQPlayer.PopperBaitPower;
            return aQPlayer.FishingPowerCache;
        }

        public Vector3 GetCelesteTorusPositionOffset(int i)
        {
            return Vector3.Transform(new Vector3(celesteTorusRadius, 0f, 0f), Matrix.CreateFromYawPitchRoll(celesteTorusX, celesteTorusY, celesteTorusZ + MathHelper.TwoPi / 5 * i));
        }

        public void UpdateCelesteTorus()
        {
            if (blueSpheres)
            {
                var type = ModContent.ProjectileType<CelesteTorusProjectile>();
                if (player.ownedProjectileCounts[type] <= 0)
                    Projectile.NewProjectile(player.Center, Vector2.Zero, type, celesteTorusDamage, celesteTorusKnockback, player.whoAmI);
                var center = player.Center;
                float playerPercent = player.statLife / (float)player.statLifeMax2;
                bool danger = false;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].IsntFriendly() && Vector2.Distance(Main.npc[i].Center, center) < 2000f)
                    {
                        danger = true;
                        break;
                    }
                }

                if (danger)
                {
                    celesteTorusSpeed = 0.04f + (1f - playerPercent) * 0.0314f;
                    celesteTorusX = celesteTorusX.AngleLerp(0f, 0.01f);
                    celesteTorusY = celesteTorusY.AngleLerp(0f, 0.0075f);
                    celesteTorusZ += celesteTorusSpeed;
                }
                else
                {
                    celesteTorusSpeed = 0.0314f;
                    celesteTorusX += 0.0157f;
                    celesteTorusY += 0.01f;
                    celesteTorusZ += celesteTorusSpeed;
                }

                celesteTorusMaxRadius = GetCelesteTorusMaxRadius(playerPercent);
                celesteTorusRadius = MathHelper.Lerp(celesteTorusRadius, celesteTorusMaxRadius, 0.1f);
                celesteTorusDamage = GetCelesteTorusDamage();
                celesteTorusKnockback = GetCelesteTorusKnockback();

                celesteTorusScale = 1f + celesteTorusRadius * 0.006f + celesteTorusDamage * 0.009f + celesteTorusKnockback * 0.0015f;
            }
            else
            {
                celesteTorusDamage = 0;
                celesteTorusKnockback = 0f;
                celesteTorusMaxRadius = 0;
                celesteTorusRadius = 0f;
                celesteTorusScale = 1f;
                celesteTorusSpeed = 0f;
                celesteTorusX = 0f;
                celesteTorusY = 0f;
                celesteTorusZ = 0f;
            }
        }

        public int GetCelesteTorusMaxRadius(float playerPercent)
        {
            return (int)((float)Math.Sqrt(player.width * player.height) + 20f + player.wingTimeMax * 0.15f + player.wingTime * 0.15f + (1f - playerPercent) * 90f + player.statDefense);
        }

        public int GetCelesteTorusDamage()
        {
            return 25 + (int)(player.statDefense / 1.5f + player.endurance * 80f);
        }

        public float GetCelesteTorusKnockback()
        {
            return 6.5f + player.velocity.Length() * 0.8f;
        }

        public override void Initialize()
        {
            omoriDeathTimer = 1;
            flyingSafe = -1;
            arachnotron = false;
            spoiled = 0;
            sparkling = false;
            weaponShootTag = 0;
            shieldLife = 0;
        }

        public override void OnEnterWorld(Player player)
        {
            if (!Main.dayTime)
                GlimmerEventSky.InitNight();
        }

        public override TagCompound Save()
        {
            return new TagCompound()
            {
                ["extractinatorCount"] = ExtractinatorCount,
            };
        }

        public override void Load(TagCompound tag)
        {
            ExtractinatorCount = tag.GetInt("extractinatorCount");
        }

        public override void UpdateBiomeVisuals()
        {
            if (flyingSafe > -1)
            {
                if (player.flyingPigChest >= 0 || player.chest != -3 || !Main.projectile[flyingSafe].active || Main.projectile[flyingSafe].type != ModContent.ProjectileType<Projectiles.ATM>())
                {
                    flyingSafe = -1;
                }
                else
                {
                    player.chestX = ((int)Main.projectile[flyingSafe].position.X + Main.projectile[flyingSafe].width / 2) / 16;
                    player.chestY = ((int)Main.projectile[flyingSafe].position.Y + Main.projectile[flyingSafe].height / 2) / 16;
                    if (!player.IsInTileInteractionRange(player.chestX, player.chestY))
                    {
                        if (player.chest != -1)
                            Main.PlaySound(SoundID.Item97);
                        player.flyingPigChest = -1;
                        flyingSafe = -1;
                        player.chest = -1;
                        Recipe.FindRecipes();
                    }
                    else
                    {
                        player.flyingPigChest = flyingSafe;
                        player.chest = -2;
                        Main.projectile[flyingSafe].type = ProjectileID.FlyingPiggyBank;
                    }
                }
            }
            if (!Main.gamePaused && Main.instance.IsActive)
            {
                GameScreenManager.Update();
            }
            AQUtils.UpdateSky((GlimmerEvent.IsActive || AQMod.omegaStariteIndexCache != -1) && player.position.Y < Main.worldSurface * 16f + Main.screenHeight, GlimmerEventSky.Name);
            //if (AQConfigClient.Instance.ScreenDistortShader)
            //    player.ManageSpecialBiomeVisuals(VisualsManager.DistortX, OmegaStarite.DistortShaderActive());
        }

        public override Texture2D GetMapBackgroundImage()
        {
            if (!player.ZoneCorrupt && !player.ZoneCrimson && !player.ZoneHoly && !player.ZoneDesert && !player.ZoneJungle)
            {
                if (player.position.Y < Main.worldSurface * 16f)
                {
                    if (GlimmerEvent.IsActive)
                        return DrawUtils.Textures.Extras[ExtraID.GlimmerMapBackground];
                }
            }
            return null;
        }

        public static bool InVanitySlot(Player player, int type)
        {
            for (int i = GraphicsPlayer.DYE_WRAP; i < GraphicsPlayer.MAX_ARMOR; i++)
            {
                if (player.armor[i].type == type)
                {
                    return true;
                }
            }
            return false;
        }

        public override void ResetEffects()
        {
            if (Main.myPlayer == player.whoAmI)
            {
                if (flyingSafe > -1)
                {
                    player.flyingPigChest = -1;
                    player.chest = -3;
                    Main.projectile[flyingSafe].type = ModContent.ProjectileType<Projectiles.ATM>();
                }
            }
            bossrush = false;
            blueSpheres = false;
            discountPercentage = 0.8f;
            bossChanneling = false;
            monoxiderBird = false;
            sparkling = false;
            moonShoes = false;
            canDash = !(player.setSolar || player.mount.Active);
            copperSeal = false;
            silverSeal = false;
            goldSeal = false;
            extraFlightTime = 0;
            dreadsoul = false;
            spectreSoulCollector = false;
            arachnotron = false;
            primeTime = false;
            omori = false;
            microStarite = false;
            spoiled = 0;
            wyvernAmulet = false;
            voodooAmulet = false;
            ghostAmulet = false;
            spiritAmulet = false;
            extractinatorVisible = false;
            opposingForce = false;
            unityMirror = false;
            stariteMinion = false;
            spicyEel = false;
            striderPalms2 = striderPalms;
            striderPalms = false;
            ghostAmuletHeld = InVanitySlot(player, ModContent.ItemType<GhostAmulet>());
            spiritAmuletHeld = InVanitySlot(player, ModContent.ItemType<SpiritAmulet>());
            voodooAmuletHeld = InVanitySlot(player, ModContent.ItemType<VoodooAmulet>());
            wyvernAmuletHeld = InVanitySlot(player, ModContent.ItemType<WyvernAmulet>());
            veinmineTiles = new bool[TileLoader.TileCount];
            weaponShootTag = AQProjectile.TagID.None;
            shieldLife = 0;
            if (burnterizerCursor)
            {
                if (Main.myPlayer == player.whoAmI)
                {
                    int sightsID = ModContent.ProjectileType<BurnterizerSights>();
                    if (AQProjectile.CountProjectiles(sightsID) <= 0)
                    {
                        Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, sightsID, 0, 0f, player.whoAmI);
                        player.ownedProjectileCounts[sightsID]++;
                    }
                }
            }
            burnterizerCursor = false;
            if (!dartHead)
                dartTrapHatTimer = 240;
            dartHead = false;
            if (thunderbirdJumpTimer > 0)
            {
                canDash = false;
                thunderbirdJumpTimer--;
            }
            if (thunderbirdLightningTimer > 0)
                thunderbirdLightningTimer--;
            if (canDash)
            {
                for (int i = 3; i < 8 + player.extraAccessorySlots; i++)
                {
                    Item item = player.armor[i];
                    if (item.type == ItemID.EoCShield || item.type == ItemID.MasterNinjaGear || item.type == ItemID.Tabi)
                    {
                        canDash = false;
                        break;
                    }
                }
            }
        }

        public override void UpdateDead()
        {
            omori = false;
            blueSpheres = false;
            sparkling = false;
            burnterizerCursor = false;
        }

        public override bool Shoot(Item item, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (chloroTransfer && type == ProjectileID.Bullet && Main.rand.NextBool(8))
                type = ProjectileID.ChlorophyteBullet;
            return true;
        }

        public override void PostItemCheck()
        {
            weaponShootTag = 0;
        }

        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (omori)
            {
                if (omoriDeathTimer <= 0)
                {
                    Main.PlaySound(SoundID.Item60, player.position);
                    player.statLife = 1;
                    player.immune = true;
                    player.immuneTime = 120;
                    omoriDeathTimer = 18000;
                    return false;
                }
            }
            return true;
        }

        public override void UpdateEquips(ref bool wallSpeedBuff, ref bool tileSpeedBuff, ref bool tileRangeBuff)
        {
            for (int i = 0; i < Chest.maxItems; i++)
            {
                if (player.bank.item[i].type > Main.maxItemTypes && player.bank.item[i].modItem is IUpdatePiggybank update)
                    update.UpdatePiggyBank(player, i);
                if (player.bank2.item[i].type > Main.maxItemTypes && player.bank2.item[i].modItem is IUpdatePlayerSafe update2)
                    update2.UpdatePlayerSafe(player, i);
            }
            UpdateCelesteTorus();
            if (player.wingsLogic > 0)
                player.wingTimeMax += extraFlightTime;
        }

        public override void PostUpdateEquips()
        {
            if (dartHead)
            {
                if (player.velocity.Y == 0f)
                    dartTrapHatTimer--;
                if (dartTrapHatTimer <= 0)
                {
                    dartTrapHatTimer = dartHeadDelay;
                    int damage = player.GetWeaponDamage(player.armor[0]);
                    var spawnPosition = player.gravDir == -1
                        ? player.position + new Vector2(player.width / 2f + 8f * player.direction, player.height)
                        : player.position + new Vector2(player.width / 2f + 8f * player.direction, 0f);
                    int p = Projectile.NewProjectile(spawnPosition, new Vector2(10f, 0f) * player.direction, dartHeadType, damage, player.armor[0].knockBack * player.minionKB, player.whoAmI);
                    Main.projectile[p].hostile = false;
                    Main.projectile[p].friendly = true;
                }
            }
            if (arachnotron)
            {
                if (Main.myPlayer == player.whoAmI)
                {
                    int type = ModContent.ProjectileType<MechaSpiderLegs>();
                    if (player.ownedProjectileCounts[type] <= 0)
                    {
                        int p = Projectile.NewProjectile(player.Center, Vector2.Zero, type, 33, 1f, player.whoAmI);
                        Main.projectile[p].netUpdate = true;
                    }
                }
            }
            if (omori)
            {
                if (omoriDeathTimer > 0)
                {
                    omoriDeathTimer--;
                    if (omoriDeathTimer == 0 && Main.myPlayer == player.whoAmI)
                        Main.PlaySound(SoundID.MaxMana, (int)player.position.X, (int)player.position.Y, 1, 0.85f, -6f);
                }
                int type = ModContent.ProjectileType<Friend>();
                if (player.ownedProjectileCounts[type] < 3)
                {
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        if (Main.projectile[i].active && Main.projectile[i].type == type && Main.projectile[i].owner == player.whoAmI)
                            Main.projectile[i].Kill();
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        Projectile.NewProjectile(player.Center, Vector2.Zero, type, 66, 4f, player.whoAmI, 1f + i);
                    }
                }
            }
            else
            {
                if (omoriDeathTimer <= 0)
                    omoriDeathTimer = 1;
            }
            if (spicyEel)
            {
                player.accRunSpeed *= 1.1f;
                player.moveSpeed *= 1.1f;
            }
            var center = player.Center;
            ClosestEnemy = byte.MaxValue;
            ClosestEnemyDistance = float.MaxValue;
            for (byte i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].IsntFriendly())
                {
                    float distance = (Main.npc[i].Center - center).Length();
                    if (distance < ClosestEnemyDistance)
                    {
                        ClosestEnemyDistance = distance;
                        ClosestEnemy = i;
                    }
                }
            }
        }

        public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
        {
            switch (proj.type)
            {
                case ProjectileID.SiltBall:
                case ProjectileID.SlushBall:
                {
                    if (extractinator)
                        damage /= 4;
                }
                break;
            }
        }

        public static bool CanBossChannel(NPC npc)
        {
            if (npc.boss)
            {
                return true;
            }
            else
            {
                switch (npc.type)
                {
                    default:
                    return false;

                    case NPCID.EaterofWorldsHead:
                    case NPCID.EaterofWorldsBody:
                    case NPCID.EaterofWorldsTail:
                    case NPCID.TheDestroyerBody:
                    case NPCID.TheDestroyerTail:
                    case NPCID.MoonLordHand:
                    case NPCID.MoonLordHead:
                    case NPCID.GolemFistLeft:
                    case NPCID.GolemFistRight:
                    case NPCID.SkeletronHand:
                    case NPCID.PrimeCannon:
                    case NPCID.PrimeLaser:
                    case NPCID.PrimeSaw:
                    case NPCID.PrimeVice:
                    case NPCID.LunarTowerNebula:
                    case NPCID.LunarTowerSolar:
                    case NPCID.LunarTowerStardust:
                    case NPCID.LunarTowerVortex:
                    case NPCID.PirateShipCannon:
                    case NPCID.GoblinSummoner:
                    case NPCID.MourningWood:
                    case NPCID.Pumpking:
                    case NPCID.Everscream:
                    case NPCID.SantaNK1:
                    case NPCID.IceQueen:
                    case NPCID.DD2DarkMageT1:
                    case NPCID.DD2DarkMageT3:
                    case NPCID.DD2OgreT2:
                    case NPCID.DD2OgreT3:
                    case NPCID.DD2Betsy:
                    case NPCID.WyvernHead:
                    case NPCID.WyvernBody:
                    case NPCID.WyvernBody2:
                    case NPCID.WyvernBody3:
                    case NPCID.WyvernLegs:
                    case NPCID.WyvernTail:
                    case NPCID.Paladin:
                    case NPCID.BigMimicCrimson:
                    case NPCID.BigMimicCorruption:
                    case NPCID.BigMimicHallow:
                    case NPCID.BigMimicJungle:
                    case NPCID.MartianSaucerTurret:
                    case NPCID.MartianSaucerCannon:
                    case NPCID.PlanterasTentacle:
                    return true;
                }
            }
        }

        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            var center = player.Center;
            var targCenter = target.Center;
            if (item.melee)
            {
                if (bossChanneling)
                {
                    target.AddBuff(ModContent.BuffType<Buffs.Debuffs.Sparkling>(), 120);
                    if (!target.SpawnedFromStatue && !CanBossChannel(target) && crit)
                    {
                        int boss = -1;
                        float closestDist = 4000f;
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            NPC npc = Main.npc[i];
                            if (npc.active && CanBossChannel(npc) && !npc.dontTakeDamage)
                            {
                                float dist = (npc.Center - center).Length();
                                if (dist < closestDist)
                                {
                                    boss = i;
                                    closestDist = dist;
                                }
                            }
                        }
                        if (boss != -1)
                        {
                            int dmg = damage > target.lifeMax ? target.lifeMax : damage;
                            Vector2 normal = Vector2.Normalize(Main.npc[boss].Center - targCenter);
                            int size = 4;
                            var type = ModContent.DustType<MonoDust>();
                            Vector2 position = target.Center - new Vector2(size / 2);
                            int length = (int)(Main.npc[boss].Center - targCenter).Length() / size;
                            const float offset = MathHelper.TwoPi / 3f;
                            for (int i = 0; i < length; i++)
                            {
                                Vector2 pos = position + normal * (i * size);
                                for (int j = 0; j < 6; j++)
                                {
                                    int d = Dust.NewDust(pos, size, size, type);
                                    float positionLength = Main.dust[d].position.Length() / 32f;
                                    Main.dust[d].color = new Color(
                                        (float)Math.Sin(positionLength) + 1f,
                                        (float)Math.Sin(positionLength + offset) + 1f,
                                        (float)Math.Sin(positionLength + offset * 2f) + 1f,
                                        0.5f);
                                }
                            }
                            for (int i = 0; i < 8; i++)
                            {
                                Vector2 normal2 = new Vector2(1f, 0f).RotatedBy(MathHelper.PiOver4 * i);
                                for (int j = 0; j < 4; j++)
                                {

                                    float positionLength1 = (targCenter + normal2 * (j * 8f)).Length() / 32f;
                                    Color color = new Color(
                                        (float)Math.Sin(positionLength1) + 1f,
                                        (float)Math.Sin(positionLength1 + offset) + 1f,
                                        (float)Math.Sin(positionLength1 + offset * 2f) + 1f,
                                        0.5f);
                                    int d = Dust.NewDust(targCenter, 1, 1, type, default, default, default, color);
                                    Main.dust[d].velocity = normal2 * (j * 3.5f);
                                }
                            }
                            Projectile.NewProjectile(Main.npc[boss].Center, Vector2.Zero, ModContent.ProjectileType<UltraExplosion>(), dmg * 2, knockback * 2, player.whoAmI);
                        }
                    }
                }
                if (primeTime)
                {
                    if (player.potionDelay <= 0)
                    {
                        player.AddBuff(ModContent.BuffType<Buffs.PrimeTime>(), 600);
                        player.AddBuff(BuffID.PotionSickness, player.potionDelayTime);
                    }
                }
            }
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            var center = player.Center;
            var targCenter = target.Center;
            if (proj.melee && proj.whoAmI == player.heldProj && proj.aiStyle != 99)
            {
                if (bossChanneling)
                {
                    target.AddBuff(ModContent.BuffType<Buffs.Debuffs.Sparkling>(), 120);
                    if (!target.SpawnedFromStatue && !CanBossChannel(target) && crit)
                    {
                        int boss = -1;
                        float closestDist = 4000f;
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            NPC npc = Main.npc[i];
                            if (npc.active && CanBossChannel(npc) && !npc.dontTakeDamage)
                            {
                                float dist = (npc.Center - center).Length();
                                if (dist < closestDist)
                                {
                                    boss = i;
                                    closestDist = dist;
                                }
                            }
                        }
                        if (boss != -1)
                        {
                            int dmg = damage > target.lifeMax ? target.lifeMax : damage;
                            Vector2 normal = Vector2.Normalize(Main.npc[boss].Center - targCenter);
                            int size = 4;
                            var type = ModContent.DustType<MonoDust>();
                            Vector2 position = target.Center - new Vector2(size / 2);
                            int length = (int)(Main.npc[boss].Center - targCenter).Length() / size;
                            const float offset = MathHelper.TwoPi / 3f;
                            for (int i = 0; i < length; i++)
                            {
                                Vector2 pos = position + normal * (i * size);
                                for (int j = 0; j < 6; j++)
                                {
                                    int d = Dust.NewDust(pos, size, size, type);
                                    float positionLength = Main.dust[d].position.Length() / 32f;
                                    Main.dust[d].color = new Color(
                                        (float)Math.Sin(positionLength) + 1f,
                                        (float)Math.Sin(positionLength + offset) + 1f,
                                        (float)Math.Sin(positionLength + offset * 2f) + 1f,
                                        0.5f);
                                }
                            }
                            for (int i = 0; i < 8; i++)
                            {
                                Vector2 normal2 = new Vector2(1f, 0f).RotatedBy(MathHelper.PiOver4 * i);
                                for (int j = 0; j < 4; j++)
                                {

                                    float positionLength1 = (targCenter + normal2 * (j * 8f)).Length() / 32f;
                                    Color color = new Color(
                                        (float)Math.Sin(positionLength1) + 1f,
                                        (float)Math.Sin(positionLength1 + offset) + 1f,
                                        (float)Math.Sin(positionLength1 + offset * 2f) + 1f,
                                        0.5f);
                                    int d = Dust.NewDust(targCenter, 1, 1, type, default, default, default, color);
                                    Main.dust[d].velocity = normal2 * (j * 3.5f);
                                }
                            }
                            Projectile.NewProjectile(Main.npc[boss].Center, Vector2.Zero, ModContent.ProjectileType<UltraExplosion>(), dmg * 2, knockback * 2, player.whoAmI);
                        }
                    }
                }
                if (primeTime)
                {
                    if (player.potionDelay <= 0)
                    {
                        player.AddBuff(ModContent.BuffType<Buffs.PrimeTime>(), 600);
                        player.AddBuff(BuffID.PotionSickness, player.potionDelayTime);
                    }
                }
            }
        }

        public override void UpdateBadLifeRegen()
        {
            if (sparkling)
            {
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;
                player.lifeRegenTime = 0;
                player.lifeRegen -= 40;
            }
        }

        public override void AnglerQuestReward(float rareMultiplier, List<Item> rewardItems)
        {
            if (Main.myPlayer == player.whoAmI && AQConfigClient.Instance.ShowCompletedQuestsCount)
                CombatText.NewText(player.getRect(), Color.Aqua, player.anglerQuestsFinished);
            var item = new Item();
            if (player.anglerQuestsFinished == 2)
            {
                item.SetDefaults(ModContent.ItemType<CopperSeal>());
                rewardItems.Add(item.Clone());
                item = new Item();
            }
            else if (player.anglerQuestsFinished == 10)
            {
                item.SetDefaults(ModContent.ItemType<SilverSeal>());
                rewardItems.Add(item.Clone());
                item = new Item();
            }
            else if (player.anglerQuestsFinished == 20)
            {
                item.SetDefaults(ModContent.ItemType<GoldSeal>());
                rewardItems.Add(item.Clone());
                item = new Item();
            }
        }

        public override void CatchFish(Item fishingRod, Item bait, int power, int liquidType, int poolSize, int worldLayer, int questFish, ref int caughtType, ref bool junk)
        {
            if (liquidType == Tile.Liquid_Water)
            {
                if (questFish == ModContent.ItemType<WaterFisg>() && Main.rand.NextBool(8))
                {
                    caughtType = ModContent.ItemType<WaterFisg>();
                }
                else if (questFish == ModContent.ItemType<Crabdaughter>() && player.ZoneBeach && Main.rand.NextBool(4))
                {
                    caughtType = ModContent.ItemType<Crabdaughter>();
                }
                if (GlimmerEvent.ActuallyActive)
                {
                    if (player.position.Y < Main.worldSurface * 16f)
                    {
                        if (player.ZoneCorrupt && Main.rand.NextBool(5))
                        {
                            caughtType = ModContent.ItemType<Fizzler>();
                        }
                        else if (((int)(player.position.X / 16f + player.width / 2) - GlimmerEvent.X).Abs() < GlimmerEvent.UltraStariteDistance && Main.rand.NextBool(7))
                        {
                            caughtType = ModContent.ItemType<UltraEel>();
                        }
                        else if (Main.rand.NextBool(6))
                        {
                            caughtType = ModContent.ItemType<Nessie>();
                        }
                        else if (Main.rand.NextBool(8))
                        {
                            caughtType = ModContent.ItemType<Blobfish>();
                        }
                        else if (Main.rand.NextBool(6))
                        {
                            caughtType = ModContent.ItemType<GlimmeringStatue>();
                        }
                        else if (Main.rand.NextBool(6))
                        {
                            caughtType = ModContent.ItemType<MoonlightWall>();
                        }
                        else
                        {
                            if (caughtType == ItemID.Bass || caughtType == ItemID.NeonTetra || caughtType == ItemID.Salmon)
                                caughtType = ModContent.ItemType<Molite>();
                        }
                    }
                }
            }
            if (liquidType == Tile.Liquid_Honey)
            {
                if (Main.rand.NextBool(3))
                {
                    caughtType = ModContent.ItemType<Combfish>();
                }
                else if (Main.rand.NextBool(5))
                {
                    caughtType = ModContent.ItemType<LarvaEel>();
                }
            }
        }

        public override void DrawEffects(PlayerDrawInfo drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (sparkling)
            {
                if (drawInfo.shadow == 0f)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        int d = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, ModContent.DustType<UltimaDust>(), player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default(Color), Main.rand.NextFloat(0.45f, 1f));
                        Main.dust[d].velocity *= 2.65f;
                        Main.dust[d].velocity.Y -= 2f;
                        Main.playerDrawDust.Add(d);
                    }
                }
                float positionLength = drawInfo.drawPlayer.position.Length() / 128f;
                const float offset = MathHelper.TwoPi / 3f;
                r *= (float)Math.Sin(positionLength) + 1f;
                g *= (float)Math.Sin(positionLength + offset) + 1f;
                b *= (float)Math.Sin(positionLength + offset * 2f) + 1f;
                Lighting.AddLight(player.Center, r * 0.25f, g * 0.25f, b * 0.25f);
            }
        }

        public static bool PlayerCrit(int critChance, UnifiedRandom rand)
        {
            if (critChance >= 100)
                return true;
            if (critChance <= 0)
                return false;
            return rand.NextBool(100 - critChance);
        }

        public override void ModifyScreenPosition()
        {
            GameScreenManager.ModifyScreenPosition();
        }
    }
}