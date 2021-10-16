using AQMod.Assets;
using AQMod.Common.Config;
using AQMod.Common.IO;
using AQMod.Common.Skies;
using AQMod.Common.Utilities;
using AQMod.Content;
using AQMod.Content.CursorDyes;
using AQMod.Content.Dusts;
using AQMod.Content.WorldEvents.Glimmer;
using AQMod.Effects.Screen;
using AQMod.Items;
using AQMod.Items.Accessories.Amulets;
using AQMod.Items.Accessories.FishingSeals;
using AQMod.Items.Armor.Arachnotron;
using AQMod.Items.Fishing;
using AQMod.Items.Fishing.Bait;
using AQMod.Items.Fishing.QuestFish;
using AQMod.Items.Placeable;
using AQMod.Items.Placeable.Walls;
using AQMod.Items.TagItems.Starbyte;
using AQMod.Items.Weapons.Summon;
using AQMod.Projectiles;
using AQMod.Projectiles.Pets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace AQMod.Common
{
    public sealed class AQPlayer : ModPlayer
    {
        public const int MaxCelesteTorusOrbs = 5;
        public const int MAX_ARMOR = 20;
        public const int DYE_WRAP = MAX_ARMOR / 2;
        public const int FRAME_HEIGHT = 56;
        public const int FRAME_COUNT = 20;
        public const float CELESTE_Z_MULT = 0.0157f;
        public const int ARACHNOTRON_OLD_POS_LENGTH = 8;

        public static int oldPosLength;
        public static Vector2[] oldPosVisual;
        public static bool arachnotronHeadTrail;
        public static bool arachnotronBodyTrail;
        internal static int _moneyTroughHackIndex = -1;
        internal static ISuperClunkyMoneyTroughTypeThing _moneyTroughHack;

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
        public ushort shieldLife;
        public bool crimsonHands;
        public bool chomper;
        public bool cosmicMap;
        public bool dungeonMap;
        public bool lihzahrdMap;
        public bool retroMap;
        public bool showCosmicMap = true;
        public bool showDungeonMap = true;
        public bool showLihzahrdMap = true;
        public bool showRetroMap = true;
        public byte nearGlobe;
        public ushort globeX;
        public ushort globeY;
        public bool hasMinionCarry;
        public int headMinionCarryX;
        public int headMinionCarryY;
        public int headMinionCarryXOld;
        public int headMinionCarryYOld;
        public bool mothmanMaskSpecial;
        public Color cataEyeColor;
        public byte monoxiderCarry;
        public int headOverlay = -1;
        public int mask = -1;
        public int cHeadOverlay;
        public int cMask;
        public int cCelesteTorus;
        public bool heartMoth;

        public byte ClosestEnemy { get; private set; }
        public float ClosestEnemyDistance { get; private set; }
        public int PopperType { get; private set; }
        public int PopperBaitPower { get; private set; }
        public int FishingPowerCache { get; private set; }
        public int ExtractinatorCount { get; set; }
        public int CursorDyeID { get; private set; } = CursorDyeLoader.ID.None;
        public string CursorDye { get; private set; } = "";

        public void SetCursorDye(int type)
        {
            if (type <= CursorDyeLoader.ID.None || type > AQMod.CursorDyes.Count)
            {
                CursorDyeID = CursorDyeLoader.ID.None;
                CursorDye = "";
            }
            else
            {
                CursorDyeID = type;
                var cursorDye = AQMod.CursorDyes.GetContent(type);
                CursorDye = AQStringCodes.EncodeName(cursorDye.Mod, cursorDye.Name);
            }
        }

        public void SetMinionCarryPos(int x, int y)
        {
            hasMinionCarry = true;
            headMinionCarryX = x;
            headMinionCarryY = y;
        }

        public Vector2 GetHeadCarryPosition()
        {
            int x;
            if (headMinionCarryX != 0)
            {
                x = headMinionCarryX;
            }
            else if (headMinionCarryXOld != 0)
            {
                x = headMinionCarryXOld;
            }
            else
            {
                x = (int)player.position.X + (player.width / 2);
            }
            int y;
            if (headMinionCarryY != 0)
            {
                y = headMinionCarryY;
            }
            else if (headMinionCarryYOld != 0)
            {
                y = headMinionCarryYOld;
            }
            else
            {
                y = (int)player.position.Y + (player.height / 2);
            }
            return new Vector2(x, y);
        }

        /// <summary>
        /// Item is the item, int is the index.
        /// </summary>
        /// <param name="action"></param>
        public void ArmorAction(Action<Item, int> action)
        {
            for (int i = 0; i < 3; i++)
            {
                action(player.armor[i], i);
            }
        }

        /// <summary>
        /// Item is the item, bool is the hide flag, int is the index.
        /// </summary>
        /// <param name="action"></param>
        public void AccessoryAction(Action<Item, bool, int> action)
        {
            for (int i = 3; i < 8 + player.extraAccessorySlots; i++)
            {
                action(player.armor[i], player.hideVisual[i], i);
            }
        }

        public static void Setup()
        {
            On.Terraria.Player.FishingLevel += GetFishingLevel;
        }

        public void HeadMinionSummonCheck(int type)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].type != type && AQProjectile.Sets.HeadMinion[Main.projectile[i].type])
                {
                    Main.projectile[i].Kill();
                }
            }
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
                float playerPercent = player.statLife / (float)player.statLifeMax2;
                celesteTorusMaxRadius = GetCelesteTorusMaxRadius(playerPercent);
                celesteTorusRadius = MathHelper.Lerp(celesteTorusRadius, celesteTorusMaxRadius, 0.1f);
                celesteTorusDamage = GetCelesteTorusDamage();
                celesteTorusKnockback = GetCelesteTorusKnockback();

                celesteTorusScale = 1f + (celesteTorusRadius * 0.006f) + (celesteTorusDamage * 0.009f) + (celesteTorusKnockback * 0.0015f);

                var type = ModContent.ProjectileType<CelesteTorusCollider>();
                if (player.ownedProjectileCounts[type] <= 0)
                {
                    Projectile.NewProjectile(player.Center, Vector2.Zero, type, celesteTorusDamage, celesteTorusKnockback, player.whoAmI);
                }
                else
                {
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        if (Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI && Main.projectile[i].type == ModContent.ProjectileType<CelesteTorusCollider>())
                        {
                            Main.projectile[i].damage = celesteTorusDamage;
                            Main.projectile[i].knockBack = celesteTorusKnockback;
                            break;
                        }
                    }
                }
                var center = player.Center;
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
            return 6.5f + (player.velocity.Length() * 0.8f);
        }

        public Vector3[] celesteTorusDrawOffsets;

        public int GetOldPosCountMaxed(int maxCount)
        {
            int count = 0;
            for (; count < maxCount; count++)
            {
                if (oldPosVisual[count] == default(Vector2))
                    break;
            }
            return count;
        }

        public static bool ShouldDrawOldPos(Player player)
        {
            if (player.mount.Active || player.frozen || player.stoned || player.GetModPlayer<AQPlayer>().mask >= 0)
            {
                return false;
            }
            return true;
        }

        public override void Initialize()
        {
            omoriDeathTimer = 1;
            arachnotron = false;
            spoiled = 0;
            sparkling = false;
            nearGlobe = 0;
            headMinionCarryX = 0;
            headMinionCarryY = 0;
            headMinionCarryXOld = 0;
            headMinionCarryYOld = 0;
            headOverlay = -1;
            mask = -1;
            CursorDyeID = 0;
            cHeadOverlay = 0;
            cMask = 0;
            cCelesteTorus = 0;
            monoxiderCarry = 0;
            cataEyeColor = new Color(50, 155, 255, 0);
            showCosmicMap = true;
            showDungeonMap = true;
            showLihzahrdMap = true;
            showRetroMap = true;
            oldPosLength = 0;
            oldPosVisual = null;
            arachnotronHeadTrail = false;
            arachnotronBodyTrail = false;
            _moneyTroughHack = null;
            _moneyTroughHackIndex = -1;
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
                ["CursorDye"] = CursorDye,
            };
        }

        public override void Load(TagCompound tag)
        {
            ExtractinatorCount = tag.GetInt("extractinatorCount");
            string dyeKey = tag.GetString("CursorDye");
            if (!string.IsNullOrEmpty(dyeKey) && AQStringCodes.DecodeName(dyeKey, out string cursorDyeMod, out string cursorDyeName))
            {
                SetCursorDye(AQMod.CursorDyes.GetContentID(cursorDyeMod, cursorDyeName));
            }
            else
            {
                SetCursorDye(CursorDyeLoader.ID.None);
            }
        }

        public override void UpdateBiomeVisuals()
        {
            if (_moneyTroughHack == null)
                _moneyTroughHackIndex = -1;
            if (_moneyTroughHackIndex > -1)
            {
                if (player.flyingPigChest >= 0 || player.chest != -3 || !Main.projectile[_moneyTroughHackIndex].active || Main.projectile[_moneyTroughHackIndex].type != ModContent.ProjectileType<Projectiles.Pets.ATM>())
                {
                    _moneyTroughHackIndex = -1;
                    _moneyTroughHack = null;
                }
                else
                {
                    player.chestX = ((int)Main.projectile[_moneyTroughHackIndex].position.X + Main.projectile[_moneyTroughHackIndex].width / 2) / 16;
                    player.chestY = ((int)Main.projectile[_moneyTroughHackIndex].position.Y + Main.projectile[_moneyTroughHackIndex].height / 2) / 16;
                    if (!player.IsInTileInteractionRange(player.chestX, player.chestY))
                    {
                        if (player.chest != -1)
                            _moneyTroughHack.OnClose();
                        player.flyingPigChest = -1;
                        _moneyTroughHackIndex = -1;
                        player.chest = -1;
                        Recipe.FindRecipes();
                    }
                    else
                    {
                        player.flyingPigChest = _moneyTroughHackIndex;
                        player.chest = -2;
                        Main.projectile[_moneyTroughHackIndex].type = ProjectileID.FlyingPiggyBank;
                    }
                }
            }
            if (!Main.gamePaused && Main.instance.IsActive)
            {
                GameScreenManager.Update();
            }
            AQUtils.UpdateSky((AQMod.glimmerEvent.IsActive || OmegaStariteSceneManager.OmegaStariteIndexCache != -1) && player.position.Y < Main.worldSurface * 16f + Main.screenHeight, GlimmerEventSky.Name);
            //if (AQConfigClient.Instance.ScreenDistortShader)
            //    player.ManageSpecialBiomeVisuals(VisualsManager.DistortX, OmegaStarite.DistortShaderActive());
        }

        public override void ResetEffects()
        {
            if (Main.myPlayer == player.whoAmI)
            {
                if (_moneyTroughHackIndex > -1)
                {
                    player.flyingPigChest = -1;
                    player.chest = _moneyTroughHack.ChestType;
                    Main.projectile[_moneyTroughHackIndex].type = _moneyTroughHack.ProjectileType;
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
            shieldLife = 0;
            crimsonHands = false;
            chomper = false;
            cosmicMap = false;
            dungeonMap = false;
            lihzahrdMap = false;
            retroMap = false;
            headMinionCarryXOld = headMinionCarryX;
            headMinionCarryYOld = headMinionCarryY;
            headMinionCarryX = 0;
            headMinionCarryY = 0;
            headOverlay = -1;
            mask = -1;
            cHeadOverlay = 0;
            cMask = 0;
            cCelesteTorus = 0;
            monoxiderCarry = 0;
            cataEyeColor = new Color(50, 155, 255, 0);
            heartMoth = false;
            if (nearGlobe > 0)
            {
                nearGlobe--;
            }
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

        public override Texture2D GetMapBackgroundImage()
        {
            if (!player.ZoneCorrupt && !player.ZoneCrimson && !player.ZoneHoly && !player.ZoneDesert && !player.ZoneJungle)
            {
                if (player.position.Y < Main.worldSurface * 16f)
                {
                    if (AQMod.glimmerEvent.IsActive)
                        return TextureCache.MapBGGlimmer.Value;
                }
            }
            return null;
        }

        public static bool InVanitySlot(Player player, int type)
        {
            for (int i = DYE_WRAP; i < MAX_ARMOR; i++)
            {
                if (player.armor[i].type == type)
                {
                    return true;
                }
            }
            return false;
        }


        public override void UpdateDead()
        {
            omori = false;
            blueSpheres = false;
            sparkling = false;
            monoxiderCarry = 0;
            if (Main.myPlayer == player.whoAmI)
            {
                oldPosLength = 0;
                oldPosVisual = null;
            }
        }

        public override bool Shoot(Item item, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (chloroTransfer && type == ProjectileID.Bullet && Main.rand.NextBool(8))
                type = ProjectileID.ChlorophyteBullet;
            return true;
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

        public override void PostUpdateBuffs()
        {
            monoxiderCarry = 0;
            var monoxider = ModContent.ProjectileType<MonoxiderMinion>();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.type == monoxider && p.ai[0] > 0f)
                    monoxiderCarry++;
            }
        }

        public override void UpdateVanityAccessories()
        {
            for (int i = 0; i < MAX_ARMOR; i++)
            {
                if (player.armor[i].type <= Main.maxItemTypes)
                    continue;
                bool hidden = i < 10 && player.hideVisual[i];
                if (player.armor[i].modItem is IUpdateEquipVisuals update && !hidden)
                    update.UpdateEquipVisuals(player, this, i);
            }
            if (player.GetModPlayer<AQPlayer>().monoxiderBird)
                headOverlay = (int)PlayerHeadOverlayID.MonoxideHat;
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
                    int type = ModContent.ProjectileType<ArachnotronLegs>();
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

                    case NPCID.TargetDummy:
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

        public void DoHyperCrystalChannel(NPC target, int damage, float knockback, Vector2 center, Vector2 targCenter)
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
                int length = (int)(Main.npc[boss].Center - targCenter).Length();
                if (Main.myPlayer == player.whoAmI && AQMod.TonsofScreenShakes)
                {
                    if (length < 800)
                    {
                        GameScreenManager.AddEffect(new ScreenShake(12, AQMod.MultIntensity((800 - length) / 128)));
                    }
                }
                int dustLength = length / size;
                const float offset = MathHelper.TwoPi / 3f;
                for (int i = 0; i < dustLength; i++)
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
                        DoHyperCrystalChannel(target, damage, knockback, center, targCenter);
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
                        DoHyperCrystalChannel(target, damage, knockback, center, targCenter);
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
                if (AQMod.glimmerEvent.IsActive)
                {
                    if (player.position.Y < Main.worldSurface * 16f)
                    {
                        if (player.ZoneCorrupt && Main.rand.NextBool(5))
                        {
                            caughtType = ModContent.ItemType<Fizzler>();
                        }
                        else if (((int)(player.position.X / 16f + player.width / 2) - AQMod.glimmerEvent.tileX).Abs() < GlimmerEvent.UltraStariteDistance && Main.rand.NextBool(7))
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

        private Vector2 getCataDustSpawnPos(int gravityOffset, int headFrame)
        {
            var spawnPos = new Vector2((int)(player.position.X + player.width / 2) - 3f, (int)(player.position.Y + 12f + gravityOffset) + Main.OffsetsPlayerHeadgear[headFrame].Y) + player.headPosition;
            if (player.direction == -1)
                spawnPos.X -= 4f;
            spawnPos.X -= 0.6f;
            spawnPos.Y -= 0.6f;
            return spawnPos;
        }

        private void CataEyeDust(Vector2 spawnPos)
        {
            int d = Dust.NewDust(spawnPos + new Vector2(0f, -6f), 6, 6, ModContent.DustType<MonoDust>(), 0, 0, 0, cataEyeColor);
            if (Main.rand.NextBool(600))
            {
                Main.dust[d].velocity = player.velocity.RotatedBy(Main.rand.NextFloat(-0.025f, 0.025f)) * 2;
                Main.dust[d].velocity.X += Main.windSpeed * 20f + player.velocity.X / -2f;
                Main.dust[d].velocity.Y -= Main.rand.NextFloat(8f, 16f);
                Main.dust[d].scale *= Main.rand.NextFloat(0.65f, 2f);
            }
            else
            {
                Main.dust[d].velocity = player.velocity * 1.1f;
                Main.dust[d].velocity.X += Main.windSpeed * 2.5f + player.velocity.X / -2f;
                Main.dust[d].velocity.Y -= Main.rand.NextFloat(4f, 5.65f);
                Main.dust[d].scale *= Main.rand.NextFloat(0.95f, 1.4f);
            }
            Main.dust[d].shader = GameShaders.Armor.GetSecondaryShader(cMask, player);
            Main.playerDrawDust.Add(d);
        }

        public override void DrawEffects(PlayerDrawInfo drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (Main.myPlayer == drawInfo.drawPlayer.whoAmI)
            {
                oldPosLength = 0;
                arachnotronHeadTrail = false;
                arachnotronBodyTrail = false;
            }
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
            if (drawInfo.shadow == 0f)
            {
                for (int i = 0; i < DYE_WRAP; i++)
                {
                    if (player.armor[i].type > Main.maxItemTypes && !player.hideVisual[i] && player.armor[i].modItem is IUpdateEquipVisuals updateVanity)
                        updateVanity.UpdateEquipVisuals(player, this, i);
                }
                for (int i = DYE_WRAP; i < MAX_ARMOR; i++)
                {
                    if (player.armor[i].type > Main.maxItemTypes && player.armor[i].modItem is IUpdateEquipVisuals updateVanity)
                        updateVanity.UpdateEquipVisuals(player, this, i);
                }
                int gravityOffset = 0;
                int headFrame = player.bodyFrame.Y / FRAME_HEIGHT;
                if (player.gravDir == -1)
                    gravityOffset = 8;
                switch ((PlayerMaskID)mask)
                {
                    case PlayerMaskID.CataMask:
                    {
                        if (cMask > 0)
                            cataEyeColor = new Color(100, 100, 100, 0);
                        if (!player.mount.Active && !player.merman && !player.wereWolf && player.statLife == player.statLifeMax2)
                        {
                            mothmanMaskSpecial = true;
                            float dustAmount = (Main.rand.Next(2, 3) + 1) * ModContent.GetInstance<AQConfigClient>().EffectQuality;
                            if (dustAmount < 1f)
                            {
                                if (Main.rand.NextFloat(dustAmount) > 0.1f)
                                    CataEyeDust(getCataDustSpawnPos(gravityOffset, headFrame));
                            }
                            else
                            {
                                var spawnPos = getCataDustSpawnPos(gravityOffset, headFrame);
                                for (int i = 0; i < dustAmount; i++)
                                {
                                    CataEyeDust(spawnPos);
                                }
                            }
                        }
                    }
                    break;
                }
            }
            var aQPlayer = drawInfo.drawPlayer.GetModPlayer<AQPlayer>();
            var drawPlayer = drawInfo.drawPlayer.GetModPlayer<AQPlayer>();
            if (aQPlayer.blueSpheres)
            {
                celesteTorusDrawOffsets = new Vector3[MaxCelesteTorusOrbs];
                for (int i = 0; i < MaxCelesteTorusOrbs; i++)
                {
                    celesteTorusDrawOffsets[i] = aQPlayer.GetCelesteTorusPositionOffset(i);
                }
            }
            if (!aQPlayer.chomper && aQPlayer.monoxiderBird)
            {
                aQPlayer.headOverlay = (byte)PlayerHeadOverlayID.MonoxideHat;
            }
        }

        public override void ModifyDrawLayers(List<PlayerLayer> layers)
        {
            int i = layers.FindIndex((p) => p.mod.Equals("Terraria") && p.Name.Equals("Head"));
            if (i != -1)
            {
                PlayerDrawLayerInstances.postDrawHead.visible = true;
                layers.Insert(i + 1, PlayerDrawLayerInstances.postDrawHead);
            }
            i = layers.FindIndex((p) => p.mod.Equals("Terraria") && p.Name.Equals("Body"));
            if (i != -1)
            {
                PlayerDrawLayerInstances.postDrawBody.visible = true;
                layers.Insert(i + 1, PlayerDrawLayerInstances.postDrawBody);
            }
            i = layers.FindIndex((p) => p.mod.Equals("Terraria") && p.Name.Equals("HeldItem"));
            if (i != -1)
            {
                PlayerDrawLayerInstances.postDrawHeldItem.visible = true;
                layers.Insert(i + 1, PlayerDrawLayerInstances.postDrawHeldItem);
            }
            i = layers.FindIndex((p) => p.mod.Equals("Terraria") && p.Name.Equals("Wings"));
            if (i != -1)
            {
                PlayerDrawLayerInstances.postDrawWings.visible = true;
                layers.Insert(i + 1, PlayerDrawLayerInstances.postDrawWings);
            }
            PlayerDrawLayerInstances.preDraw.visible = true;
            layers.Insert(0, PlayerDrawLayerInstances.preDraw);
            PlayerDrawLayerInstances.postDraw.visible = true;
            layers.Add(PlayerDrawLayerInstances.postDraw);
        }

        public override void ModifyDrawHeadLayers(List<PlayerHeadLayer> layers)
        {
            layers.Add(PlayerDrawLayerInstances.postDrawHeadHead);
        }

        public override void ModifyScreenPosition()
        {
            GameScreenManager.ModifyScreenPosition();
        }

        public static bool PlayerCrit(int critChance, UnifiedRandom rand)
        {
            if (critChance >= 100)
                return true;
            if (critChance <= 0)
                return false;
            return rand.NextBool(100 - critChance);
        }

        public static bool CloseMoneyTrough()
        {
            if (_moneyTroughHack != null)
            {
                _moneyTroughHack.OnClose();
                Main.LocalPlayer.chest = -1;
                Recipe.FindRecipes();
                return true;
            }
            return false;
        }

        public static bool OpenMoneyTrough(ISuperClunkyMoneyTroughTypeThing moneyTrough, int index)
        {
            if (_moneyTroughHack == null)
            {
                _moneyTroughHack = moneyTrough;
                _moneyTroughHackIndex = index;
                var plr = Main.LocalPlayer;
                plr.chest = moneyTrough.ChestType;
                plr.chestX = (int)(Main.projectile[index].Center.X / 16f);
                plr.chestY = (int)(Main.projectile[index].Center.Y / 16f);
                plr.talkNPC = -1;
                Main.npcShop = 0;
                Main.playerInventory = true;
                moneyTrough.OnOpen();
                Recipe.FindRecipes();
                return true;
            }
            return false;
        }
    }
}