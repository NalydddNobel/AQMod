using AQMod.Assets;
using AQMod.Assets.Graphics.Particles;
using AQMod.Assets.Graphics.ParticlesLayers;
using AQMod.Assets.Graphics.PlayerLayers;
using AQMod.Buffs.Debuffs;
using AQMod.Common;
using AQMod.Common.Config;
using AQMod.Common.NetCode;
using AQMod.Common.Skies;
using AQMod.Common.Utilities;
using AQMod.Content.CursorDyes;
using AQMod.Content.Dusts;
using AQMod.Content.WorldEvents.CosmicEvent;
using AQMod.Effects;
using AQMod.Effects.ScreenEffects;
using AQMod.Items;
using AQMod.Items.Accessories.Amulets;
using AQMod.Items.Accessories.FishingSeals;
using AQMod.Items.Armor.Arachnotron;
using AQMod.Items.BuffItems;
using AQMod.Items.BuffItems.Foods;
using AQMod.Items.Materials.Fish;
using AQMod.Items.Placeable;
using AQMod.Items.Placeable.Wall;
using AQMod.Items.Quest.Angler;
using AQMod.Projectiles;
using AQMod.Projectiles.Pets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace AQMod
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

        public Vector3[] celesteTorusOffsetsForDrawing;

        public float discountPercentage;
        public bool blueSpheres;
        public bool bossChanneling;
        public bool monoxiderBird;
        public bool sparkling;
        public bool chloroTransfer;
        public bool altEvilDrops;
        public bool breadsoul;
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
        public bool striderPalmsOld;
        public bool wyvernAmuletHeld;
        public bool voodooAmuletHeld;
        public bool ghostAmuletHeld;
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
        public Color cataEyeColor;
        public byte monoxiderCarry;
        public int headOverlay = -1;
        public int mask = -1;
        public int cHeadOverlay;
        public int cMask;
        public int cCelesteTorus;
        public bool heartMoth;
        public bool notFrostburn;
        public bool bossrush;
        public bool bossrushOld;
        public float grabReachMult; // until 1.4 comes
        public bool grapePhanta;
        public bool neutronYogurt;
        public bool mothmanMask;
        public byte mothmanExplosionDelay;

        public bool NetUpdateKillCount;
        public int[] CurrentEncoreKillCount { get; private set; }
        public int[] EncoreBossKillCountRecord { get; private set; }
        public int PopperType { get; set; }
        public int PopperBaitPower { get; set; }
        public int FishingPowerCache { get; set; }
        public int ExtractinatorCount { get; set; }
        public int CursorDyeID { get; private set; } = CursorDyeLoader.ID.None;
        public string CursorDye { get; private set; } = "";
        public sbyte Temperature;
        public byte TemperatureStruck;

        public const float AtmosphericCurrentsWindSpeed = 30f;
        public static bool IsQuickBuffing { get; internal set; }

        public bool AtmosphericCurrentsEvent => player.ZoneSkyHeight && Main.windSpeed > 30f;

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
            notFrostburn = false;
            bossrush = false;
            bossrushOld = false;
            CurrentEncoreKillCount = new int[NPCLoader.NPCCount];
            EncoreBossKillCountRecord = new int[NPCLoader.NPCCount];
            grabReachMult = 1f;
            Temperature = 0;
        }

        public override void OnEnterWorld(Player player)
        {
            if (!Main.dayTime && Main.netMode != NetmodeID.MultiplayerClient && Main.myPlayer == player.whoAmI)
                GlimmerEventSky.InitNight();
        }

        public byte[] SerializeBossKills()
        {
            var writer = new BinaryWriter(new MemoryStream(1024));
            if (EncoreBossKillCountRecord == null)
            {
                writer.Write(false);
                return ((MemoryStream)writer.BaseStream).GetBuffer();
            }
            writer.Write(true);
            writer.Write((byte)0);
            for (int i = 0; i < EncoreBossKillCountRecord.Length; i++)
            {
                if (EncoreBossKillCountRecord[i] != 0)
                {
                    writer.Write(true);
                    if (i >= Main.maxNPCTypes)
                    {
                        writer.Write(true);
                        var ModNPC = NPCLoader.GetNPC(i);
                        writer.Write(ModNPC.mod.Name);
                        writer.Write(ModNPC.Name);
                        writer.Write(EncoreBossKillCountRecord[i]);
                    }
                    else
                    {
                        writer.Write(false);
                        writer.Write(i);
                        writer.Write(EncoreBossKillCountRecord[i]);
                    }
                }
            }
            writer.Write(false);
            return ((MemoryStream)writer.BaseStream).GetBuffer();
        }

        public void DeserialzeBossKills(byte[] buffer)
        {
            var reader = new BinaryReader(new MemoryStream(buffer));
            if (!reader.ReadBoolean())
                return;
            byte save = reader.ReadByte();
            while (reader.ReadBoolean())
            {
                if (reader.ReadBoolean())
                {
                    string mod = reader.ReadString();
                    string name = reader.ReadString();
                    int kills = reader.ReadInt32();
                    try
                    {
                        var Mod = ModLoader.GetMod(mod);
                        if (Mod == null)
                            continue;
                        int type = Mod.NPCType(name);
                        if (type != -1)
                            EncoreBossKillCountRecord[type] = kills;
                    }
                    catch
                    {
                    }
                }
                else
                {
                    int type = reader.ReadInt32();
                    int kills = reader.ReadInt32();
                    EncoreBossKillCountRecord[type] = kills;
                }
            }
        }

        public override TagCompound Save()
        {
            return new TagCompound()
            {
                ["extractinatorCount"] = ExtractinatorCount,
                ["CursorDye"] = CursorDye,
                ["bosskills"] = SerializeBossKills(),
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
            byte[] buffer = tag.GetByteArray("bosskills");
            if (buffer == null || buffer.Length == 0)
                return;
            DeserialzeBossKills(buffer);
        }

        public override void UpdateBiomeVisuals()
        {
            if (_moneyTroughHack == null)
                _moneyTroughHackIndex = -1;
            if (_moneyTroughHackIndex > -1)
            {
                if (player.flyingPigChest >= 0 || player.chest != -3 || !Main.projectile[_moneyTroughHackIndex].active || Main.projectile[_moneyTroughHackIndex].type != ModContent.ProjectileType<ATM>())
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
                ScreenShakeManager.Update();
            AQUtils.UpdateSky((AQMod.CosmicEvent.IsActive || OmegaStariteScene.OmegaStariteIndexCache != -1) && player.position.Y < Main.worldSurface * 16f + Main.screenHeight, GlimmerEventSky.Name);
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
            breadsoul = false;
            arachnotron = false;
            primeTime = false;
            omori = false;
            microStarite = false;
            spoiled = 0;
            wyvernAmulet = false;
            voodooAmulet = false;
            ghostAmulet = false;
            extractinatorVisible = false;
            altEvilDrops = false;
            unityMirror = false;
            stariteMinion = false;
            spicyEel = false;
            striderPalmsOld = striderPalms;
            striderPalms = false;
            ghostAmuletHeld = InVanitySlot(player, ModContent.ItemType<GhostAmulet>());
            voodooAmuletHeld = InVanitySlot(player, ModContent.ItemType<VoodooAmulet>());
            wyvernAmuletHeld = InVanitySlot(player, ModContent.ItemType<WyvernAmulet>());
            veinmineTiles = new bool[TileLoader.TileCount];
            shieldLife = 0;
            crimsonHands = false;
            chomper = false;
            dungeonMap = false;
            lihzahrdMap = false;
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
            notFrostburn = false;
            grabReachMult = 1f;
            grapePhanta = false;
            mothmanMask = false;
            if (TemperatureStruck > 0)
                TemperatureStruck--;
            else
            {
                if (Temperature < 0f)
                {

                }
            }
            if (mothmanExplosionDelay > 0)
                mothmanExplosionDelay--;
            if (bossrushOld != bossrush)
            {
                if (bossrush)
                {

                }
                else
                {
                    CurrentEncoreKillCount = new int[NPCLoader.NPCCount];
                }
            }
            bossrushOld = bossrush;
            bossrush = false;
            if (nearGlobe > 0)
                nearGlobe--;
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
                    if (AQMod.CosmicEvent.IsActive)
                        return TextureCache.MapBGGlimmer.Value;
                }
            }
            return null;
        }

        public override void clientClone(ModPlayer clientClone)
        {
            var clone = (AQPlayer)clientClone;
            clone.celesteTorusX = celesteTorusX;
            clone.celesteTorusY = celesteTorusY;
            clone.celesteTorusZ = celesteTorusZ;
            clone.CurrentEncoreKillCount = CurrentEncoreKillCount;
            clone.EncoreBossKillCountRecord = EncoreBossKillCountRecord;
            clone.breadsoul = breadsoul;
            clone.dreadsoul = dreadsoul;
            clone.dartHead = dartHead;
            clone.dartHeadType = dartHeadType;
            clone.arachnotron = arachnotron;
            clone.blueSpheres = blueSpheres;
        }

        public override void PostItemCheck()
        {
            if (player.itemAnimation < 1 && player.inventory[player.selectedItem].modItem is ISpecialFood)
            {
                player.inventory[player.selectedItem].buffType = BuffID.WellFed;
            }
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            var clone = (AQPlayer)clientPlayer;
            if (clone.bossrush)
            {
                NetUpdateKillCount = true;
                SyncPlayer(-1, player.whoAmI, true);
            }
            else
            {
                if (clone.blueSpheres)
                {
                    Sync_CelesteTorus(toWho: -1, fromWho: -1);
                }
            }
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            ModPacket packet = mod.GetPacket();
            if (NetUpdateKillCount)
            {
                packet.Write(NetType.UpdateAQPlayerEncoreKills);
                packet.Write((byte)player.whoAmI);
                byte[] buffer = SerializeBossKills();
                packet.Write(buffer, 0, buffer.Length);
                packet.Send(toWho, fromWho);
                NetUpdateKillCount = false;
                return;
            }
            Sync_CelesteTorus();
        }

        private void Sync_CelesteTorus(int toWho = -1, int fromWho = -1)
        {
            var packet = mod.GetPacket();
            packet.Write(NetType.UpdateAQPlayerCelesteTorus);
            packet.Write((byte)player.whoAmI);
            packet.Write(celesteTorusX);
            packet.Write(celesteTorusY);
            packet.Write(celesteTorusZ);
            packet.Send(toWho, fromWho);
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
            var monoxider = ModContent.ProjectileType<Projectiles.Summon.Monoxider>();
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
            if (Main.myPlayer == player.whoAmI)
            {
                for (int i = 0; i < Chest.maxItems; i++)
                {
                    if (player.bank.item[i].type > Main.maxItemTypes && player.bank.item[i].modItem is IUpdatePiggybank update)
                        update.UpdatePiggyBank(player, i);
                    if (player.bank2.item[i].type > Main.maxItemTypes && player.bank2.item[i].modItem is IUpdatePlayerSafe update2)
                        update2.UpdatePlayerSafe(player, i);
                }
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
            if (npc.chaseable || npc.dontTakeDamage)
            {
                return false;
            }
            return npc.boss || AQNPC.Sets.BossRelatedEnemy[npc.type];
        }

        public void DoHyperCrystalChannel(NPC target, int damage, float knockback, Vector2 center, Vector2 targCenter)
        {
            if (target.SpawnedFromStatue || target.type == NPCID.TargetDummy || CanBossChannel(target))
                return;
            int boss = -1;
            float closestDist = 1200f;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && CanBossChannel(npc))
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
                var normal = Vector2.Normalize(Main.npc[boss].Center - targCenter);
                int size = 4;
                var type = ModContent.DustType<MonoDust>();
                Vector2 position = target.Center - new Vector2(size / 2);
                int length = (int)(Main.npc[boss].Center - targCenter).Length();
                if (Main.myPlayer == player.whoAmI && AQMod.TonsofScreenShakes)
                {
                    if (length < 800)
                        ScreenShakeManager.AddEffect(new BasicScreenShake(12, AQMod.MultIntensity((800 - length) / 128)));
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
                        var color = new Color(
                            (float)Math.Sin(positionLength1) + 1f,
                            (float)Math.Sin(positionLength1 + offset) + 1f,
                            (float)Math.Sin(positionLength1 + offset * 2f) + 1f,
                            0.5f);
                        int d = Dust.NewDust(targCenter, 1, 1, type, default, default, default, color);
                        Main.dust[d].velocity = normal2 * (j * 3.5f);
                    }
                }
                Projectile.NewProjectile(Main.npc[boss].Center, Vector2.Zero, ModContent.ProjectileType<HyperCrystalExplosion>(), dmg * 2, knockback * 2, player.whoAmI);
            }
        }


        public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
        {
            switch (npc.type)
            {
                case NPCID.Mothron:
                case NPCID.MothronSpawn:
                case NPCID.MothronEgg:
                case NPCID.CultistBoss:
                case NPCID.CultistBossClone:
                case NPCID.CultistDragonBody1:
                case NPCID.CultistDragonBody2:
                case NPCID.CultistDragonBody3:
                case NPCID.CultistDragonBody4:
                case NPCID.CultistDragonHead:
                case NPCID.CultistDragonTail:
                case NPCID.AncientCultistSquidhead:
                {
                    if (mothmanMask)
                        damage /= 2;
                }
                break;
            }
        }

        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            var center = player.Center;
            var targetCenter = target.Center;
            if (item.melee)
            {
                if (bossChanneling)
                {
                    target.AddBuff(ModContent.BuffType<Sparkling>(), 120);
                    if (crit)
                        DoHyperCrystalChannel(target, damage, knockback, center, targetCenter);
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
            HitNPCEffects(target, targetCenter, damage, knockback, crit);
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            var center = player.Center;
            var targetCenter = target.Center;
            if (proj.melee && proj.whoAmI == player.heldProj && proj.aiStyle != 99)
            {
                if (bossChanneling)
                {
                    target.AddBuff(ModContent.BuffType<Sparkling>(), 120);
                    if (crit)
                        DoHyperCrystalChannel(target, damage, knockback, center, targetCenter);
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
            HitNPCEffects(target, targetCenter, damage, knockback, crit);
        }

        private void HitNPCEffects(NPC target, Vector2 targetCenter, int damage, float knockback, bool crit)
        {
            if (mothmanMask && mothmanExplosionDelay == 0 && player.statLife >= player.statLifeMax2 && crit && !target.buffImmune[ModContent.BuffType<BlueFire>()] && target.type != NPCID.TargetDummy)
            {
                target.AddBuff(ModContent.BuffType<BlueFire>(), 480);
                if (Main.myPlayer == player.whoAmI)
                {
                    Main.PlaySound(SoundID.Item74, targetCenter);
                    int amount = (int)(25 * AQMod.EffectIntensity);
                    if (AQMod.EffectQuality < 1f)
                    {
                        amount = (int)(amount * AQMod.EffectQuality);
                    }
                    var pos = target.position - new Vector2(2f, 2f);
                    var rect = new Rectangle((int)pos.X, (int)pos.Y, target.width + 4, target.height + 4);
                    for (int i = 0; i < amount; i++)
                    {
                        var dustPos = new Vector2(Main.rand.Next(rect.X, rect.X + rect.Width), Main.rand.Next(rect.Y, rect.Y + rect.Height));
                        var velocity = new Vector2(Main.rand.NextFloat(-8f, 8f), Main.rand.NextFloat(-10f, 2f).Abs());
                        ParticleLayers.AddParticle_PostDrawPlayers(
                            new MonoParticleEmber(dustPos, velocity,
                            new Color(0.5f, Main.rand.NextFloat(0.2f, 0.6f), Main.rand.NextFloat(0.8f, 1f), 0f), Main.rand.NextFloat(0.8f, 1.1f)));
                        ParticleLayers.AddParticle_PostDrawPlayers(
                            new MonoParticleEmber(dustPos, velocity,
                            new Color(0.5f, Main.rand.NextFloat(0.2f, 0.6f), Main.rand.NextFloat(0.8f, 1f), 0f) * 0.2f, 1.5f));
                    }
                    amount = (int)(120 * AQMod.EffectIntensity);
                    if (AQMod.EffectQuality < 1f)
                    {
                        amount = (int)(amount * AQMod.EffectQuality);
                    }
                    if (AQMod.Screenshakes)
                    {
                        ScreenShakeManager.AddEffect(new BasicScreenShake(16, 8));
                    }
                    mothmanExplosionDelay = 60;
                    int p = Projectile.NewProjectile(targetCenter, Vector2.Normalize(targetCenter - player.Center), ModContent.ProjectileType<MothmanCritExplosion>(), damage * 2, knockback * 1.5f, player.whoAmI, 0f, target.whoAmI);
                    var size = Main.projectile[p].Size;
                    float radius = size.Length() / 5f;
                    for (int i = 0; i < amount; i++)
                    {
                        var offset = new Vector2(Main.rand.NextFloat(radius), 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi));
                        var normal = Vector2.Normalize(offset);
                        var dustPos = targetCenter + offset;
                        var velocity = normal * Main.rand.NextFloat(6f, 12f);
                        ParticleLayers.AddParticle_PostDrawPlayers(
                            new MonoParticleEmber(dustPos, velocity,
                            new Color(0.5f, Main.rand.NextFloat(0.2f, 0.6f), Main.rand.NextFloat(0.8f, 1f), 0f), Main.rand.NextFloat(0.8f, 1.1f)));
                        ParticleLayers.AddParticle_PostDrawPlayers(
                            new MonoParticleEmber(dustPos, velocity,
                            new Color(0.5f, Main.rand.NextFloat(0.2f, 0.6f), Main.rand.NextFloat(0.8f, 1f), 0f) * 0.2f, 1.5f));
                        if (Main.rand.NextBool(14))
                        {
                            var sparkleClr = new Color(0.5f, Main.rand.NextFloat(0.2f, 0.6f), Main.rand.NextFloat(0.8f, 1f), 0f);
                            ParticleLayers.AddParticle_PostDrawPlayers(
                                new SparkleParticle(dustPos, velocity,
                                sparkleClr, 1.5f));
                            ParticleLayers.AddParticle_PostDrawPlayers(
                                new SparkleParticle(dustPos, velocity,
                                sparkleClr * 0.5f, 1f)
                                { rotation = MathHelper.PiOver4 });
                        }
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
            if (notFrostburn)
            {
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;
                player.lifeRegenTime = 0;
                player.lifeRegen -= 10;
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
                if (questFish > Main.maxItems && ItemLoader.GetItem(questFish) is AnglerQuestItem anglerQuestItem)
                {
                    if (anglerQuestItem.FishingLocation.CatchFish(player, fishingRod, bait, power, liquidType, poolSize, worldLayer))
                    {
                        caughtType = questFish;
                        return;
                    }
                }
                if (AQMod.CosmicEvent.IsActive)
                {
                    if (player.position.Y < Main.worldSurface * 16f)
                    {
                        if (player.ZoneCorrupt && Main.rand.NextBool(5))
                        {
                            caughtType = ModContent.ItemType<Fizzler>();
                        }
                        else if (((int)(player.position.X / 16f + player.width / 2) - AQMod.CosmicEvent.tileX).Abs() < GlimmerEvent.UltraStariteDistance && Main.rand.NextBool(7))
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
                Main.dust[d].velocity = player.velocity.RotatedBy(Main.rand.NextFloat(-0.025f, 0.025f)) * 1.5f;
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
            if (drawInfo.shadow == 0f)
            {
                if (sparkling)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        int d = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, ModContent.DustType<UltimaDust>(), player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default(Color), Main.rand.NextFloat(0.45f, 1f));
                        Main.dust[d].velocity *= 2.65f;
                        Main.dust[d].velocity.Y -= 2f;
                        Main.playerDrawDust.Add(d);
                    }
                    Lighting.AddLight(player.Center, 1f, 1f, 1f);
                    fullBright = true;
                }
                if (notFrostburn)
                {
                    if (Main.netMode != NetmodeID.Server && AQMod.GameWorldActive)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            var pos = drawInfo.position - new Vector2(2f, 2f);
                            var rect = new Rectangle((int)pos.X, (int)pos.Y, player.width + 4, player.height + 4);
                            var dustPos = new Vector2(Main.rand.Next(rect.X, rect.X + rect.Width), Main.rand.Next(rect.Y, rect.Y + rect.Height));
                            ParticleLayers.AddParticle_PostDrawPlayers(
                                new MonoParticleEmber(dustPos, new Vector2((player.velocity.X + Main.rand.NextFloat(-3f, 3f)) * 0.3f, ((player.velocity.Y + Main.rand.NextFloat(-3f, 3f)) * 0.4f).Abs() - 2f),
                                new Color(0.5f, Main.rand.NextFloat(0.2f, 0.6f), Main.rand.NextFloat(0.8f, 1f), 0f), Main.rand.NextFloat(0.2f, 1.2f)));
                        }
                    }
                    Lighting.AddLight(player.Center, 0.4f, 0.4f, 1f);
                    fullBright = true;
                }
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
                celesteTorusOffsetsForDrawing = new Vector3[MaxCelesteTorusOrbs];
                for (int i = 0; i < MaxCelesteTorusOrbs; i++)
                {
                    celesteTorusOffsetsForDrawing[i] = aQPlayer.GetCelesteTorusPositionOffset(i);
                }
            }
            if (!aQPlayer.chomper && aQPlayer.monoxiderBird)
                aQPlayer.headOverlay = (byte)PlayerHeadOverlayID.MonoxideHat;
        }

        public override void ModifyDrawLayers(List<PlayerLayer> layers)
        {
            int i = layers.FindIndex((p) => p.mod.Equals("Terraria") && p.Name.Equals("Head"));
            if (i != -1)
            {
                PlayerLayersCache.postDrawHead.visible = true;
                layers.Insert(i + 1, PlayerLayersCache.postDrawHead);
            }
            i = layers.FindIndex((p) => p.mod.Equals("Terraria") && p.Name.Equals("Body"));
            if (i != -1)
            {
                PlayerLayersCache.postDrawBody.visible = true;
                layers.Insert(i + 1, PlayerLayersCache.postDrawBody);
            }
            i = layers.FindIndex((p) => p.mod.Equals("Terraria") && p.Name.Equals("HeldItem"));
            if (i != -1)
            {
                PlayerLayersCache.postDrawHeldItem.visible = true;
                layers.Insert(i + 1, PlayerLayersCache.postDrawHeldItem);
            }
            i = layers.FindIndex((p) => p.mod.Equals("Terraria") && p.Name.Equals("Wings"));
            if (i != -1)
            {
                PlayerLayersCache.postDrawWings.visible = true;
                layers.Insert(i + 1, PlayerLayersCache.postDrawWings);
            }
            PlayerLayersCache.preDraw.visible = true;
            layers.Insert(0, PlayerLayersCache.preDraw);
            PlayerLayersCache.postDraw.visible = true;
            layers.Add(PlayerLayersCache.postDraw);
        }

        public override void ModifyDrawHeadLayers(List<PlayerHeadLayer> layers)
        {
            layers.Add(PlayerLayersCache.postDrawHeadHead);
        }

        public override void ModifyScreenPosition()
        {
            ScreenShakeManager.ModifyScreenPosition();
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

                celesteTorusScale = 1f + celesteTorusRadius * 0.006f + celesteTorusDamage * 0.009f + celesteTorusKnockback * 0.0015f;

                var type = ModContent.ProjectileType<CelesteTorusCollider>();
                if (Main.myPlayer == player.whoAmI && player.ownedProjectileCounts[type] <= 0)
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
            return 6.5f + player.velocity.Length() * 0.8f;
        }

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
                return false;
            return true;
        }


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
                x = (int)player.position.X + player.width / 2;
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
                y = (int)player.position.Y + player.height / 2;
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

        public static void HeadMinionSummonCheck(int player, int type)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].type != type && AQProjectile.Sets.HeadMinion[Main.projectile[i].type] && Main.projectile[i].owner == player)
                    Main.projectile[i].Kill();
            }
        }

        public static bool HasFoodBuff(int player)
        {
            for (int i = 0; i < Player.MaxBuffs; i++)
            {
                if (Main.player[player].buffTime[i] > 0 && AQBuff.Sets.FoodBuff[Main.player[player].buffType[i]])
                {
                    return true;
                }
            }
            return false;
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

        public static bool InVanitySlot(Player player, int type)
        {
            for (int i = DYE_WRAP; i < MAX_ARMOR; i++)
            {
                if (player.armor[i].type == type)
                    return true;
            }
            return false;
        }
    }
}