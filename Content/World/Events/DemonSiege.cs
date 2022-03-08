using AQMod.Assets;
using AQMod.Common;
using AQMod.Common.Utilities;
using AQMod.Dusts;
using AQMod.Items.Weapons.Magic;
using AQMod.Items.Weapons.Melee;
using AQMod.Items.Weapons.Melee.Yoyo;
using AQMod.Items.Weapons.Ranged;
using AQMod.Items.Weapons.Summon;
using AQMod.NPCs.Monsters.DemonSiegeMonsters;
using AQMod.Projectiles.Monster;
using AQMod.Tiles.Nature;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AQMod.Content.World.Events
{
    public sealed class DemonSiege : WorldEvent
    {
        public enum UpgradeProgression : byte
        {
            PreHardmode = 0,
            Hardmode = 1,
            PostPlantera = 2,
        }
        public struct SiegeEnemy
        {
            public readonly int type;
            public readonly int spawnWidth;
            public readonly int spawnTime;
            public readonly UpgradeProgression progression;

            public const int SPAWNTIME_CINDERA = 120;
            public const int SPAWNTIME_PRE_HARDMODE_REGULAR = 150;

            public static SiegeEnemy FromT<T>(UpgradeProgression progression, int time = SPAWNTIME_PRE_HARDMODE_REGULAR, int width = 20) where T : ModNPC
            {
                return new SiegeEnemy(ModContent.NPCType<T>(), progression, time, width);
            }

            public SiegeEnemy(int type, UpgradeProgression progression, int time = SPAWNTIME_PRE_HARDMODE_REGULAR, int width = 32)
            {
                this.type = type;
                this.progression = progression;
                spawnTime = time;
                spawnWidth = width;
            }
        }
        public struct SiegeUpgrade
        {
            public readonly int baseItem;
            public readonly int rewardItem;
            public readonly UpgradeProgression progression;
            public readonly ushort upgradeTime;

            public const ushort UpgradeTime_PreHardmode = 5400;

            public SiegeUpgrade(int baseItem, int rewardItem, UpgradeProgression progression, ushort time = UpgradeTime_PreHardmode)
            {
                this.baseItem = baseItem;
                this.rewardItem = rewardItem;
                this.progression = progression;
                upgradeTime = time;
            }
        }
        public sealed class CustomProgressBar : EventProgressBar
        {
            public override Texture2D IconTexture => ModContent.GetTexture(TexturePaths.EventIcons + "demonsiege");
            public override string EventName => Language.GetTextValue("Mods.AQMod.EventName.DemonSiege");
            public override Color NameBGColor => new Color(120, 90 + (int)(Math.Sin(Main.GlobalTime * 5f) * 10), 20, 128);
            public override float EventProgress => 1f - DemonSiege.UpgradeTime / (float)DemonSiege.Upgrade.upgradeTime;

            public override bool IsActive() => Main.LocalPlayer.Biomes().zoneDemonSiege;
            public override string ModifyProgressText(string text) => Language.GetTextValue("Mods.AQMod.Common.TimeLeft", AQUtils.TimeText3(DemonSiege.UpgradeTime));
        }

        internal override EventProgressBar ProgressBar => new CustomProgressBar();

        public static Color TextColor => new Color(250, 95, 10, 255);
        internal static List<SiegeUpgrade> Upgrades { get; private set; }
        internal static List<SiegeEnemy> Enemies { get; private set; }
        public static List<int> HellBanners { get; private set; }

        public static bool active;
        public static ushort X;
        public static ushort Y;
        public static bool IsActive => active && X > 0 && Y > 0;

        public static ushort UpgradeTime;
        public static byte PlayerActivator { get; private set; }
        public static Item BaseItem { get; private set; }
        public static SiegeUpgrade Upgrade { get; private set; }

        public static SiegeEnemy spawnEnemy;
        public static int spawnEnemyX = -1;
        public static int spawnEnemyY = -1;
        public static int spawnEnemyTimer;

        public const int SPAWN_ENEMY_DELAY = 150;

        internal static void Load()
        {
            Reset();

            Upgrades = new List<SiegeUpgrade>
            {
                new SiegeUpgrade(ItemID.LightsBane, ModContent.ItemType<HellsBoon>(), UpgradeProgression.PreHardmode, SiegeUpgrade.UpgradeTime_PreHardmode),
                new SiegeUpgrade(ItemID.BloodButcherer, ModContent.ItemType<CrimsonHellSword>(), UpgradeProgression.PreHardmode, SiegeUpgrade.UpgradeTime_PreHardmode),
                new SiegeUpgrade(ItemID.CorruptYoyo, ModContent.ItemType<Dysesthesia>(), UpgradeProgression.PreHardmode, SiegeUpgrade.UpgradeTime_PreHardmode),
                new SiegeUpgrade(ItemID.DemonBow, ModContent.ItemType<HamaYumi>(), UpgradeProgression.PreHardmode, SiegeUpgrade.UpgradeTime_PreHardmode),
                new SiegeUpgrade(ItemID.TendonBow, ModContent.ItemType<Deltoid>(), UpgradeProgression.PreHardmode, SiegeUpgrade.UpgradeTime_PreHardmode),
                new SiegeUpgrade(ModContent.ItemType<SeltzerRain>(), ModContent.ItemType<FizzlingFire>(), UpgradeProgression.PreHardmode, SiegeUpgrade.UpgradeTime_PreHardmode),
                new SiegeUpgrade(ModContent.ItemType<ScarletSea>(), ModContent.ItemType<Skrawler>(), UpgradeProgression.PreHardmode, SiegeUpgrade.UpgradeTime_PreHardmode),
                new SiegeUpgrade(ModContent.ItemType<CorruptPot>(), ModContent.ItemType<PiranhaPot>(), UpgradeProgression.PreHardmode, SiegeUpgrade.UpgradeTime_PreHardmode),
            };

            Enemies = new List<SiegeEnemy>
            {
                SiegeEnemy.FromT<Cindera>(UpgradeProgression.PreHardmode, SiegeEnemy.SPAWNTIME_CINDERA, 20),
                SiegeEnemy.FromT<TrapImp>(UpgradeProgression.PreHardmode, SiegeEnemy.SPAWNTIME_PRE_HARDMODE_REGULAR, 32),
                SiegeEnemy.FromT<Magmalbubble>(UpgradeProgression.PreHardmode, SiegeEnemy.SPAWNTIME_PRE_HARDMODE_REGULAR, 32),
            };

            HellBanners = new List<int>()
            {
                ItemID.HellboundBanner,
                ItemID.HellHammerBanner,
                ItemID.HelltowerBanner,
                ItemID.LostHopesofManBanner,
                ItemID.ObsidianWatcherBanner,
                ItemID.LavaEruptsBanner,
            };
        }

        internal static void Unload()
        {
            Upgrades?.Clear();
            Upgrades = null;
            Enemies?.Clear();
            Enemies = null;
            HellBanners?.Clear();
            HellBanners = null;
            BaseItem = null;
        }

        public override void Initialize()
        {
            Reset();
        }

        public static void AddDemonSeigeUpgrade(SiegeUpgrade upgrade)
        {
            if (AQMod.Loading)
                Upgrades.Add(upgrade);
        }

        public static void AddDemonSeigeEnemy(SiegeEnemy enemy)
        {
            if (AQMod.Loading)
                Enemies.Add(enemy);
        }

        public static bool CheckSpot(int x, int y)
        {
            var tile = Framing.GetTileSafely(x, y);
            if (tile.active() && tile.type == ModContent.TileType<GoreNest>())
                return true;
            return false;
        }

        public static bool Activate(int x, int y, int plr, Item item, bool fromServer = false)
        {
            if (CheckSpot(x, y))
            {
                if (fromServer)
                {
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendData(MessageID.WorldData);
                    }
                }
                else if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    NetHelper.RequestDemonSiege(x, y, plr, item);
                }
                BaseItem = item.Clone();
                Upgrade = GetUpgrade(BaseItem).GetValueOrDefault(default(SiegeUpgrade));
                if (!fromServer)
                {
                    item.TurnToAir();
                    item.type = ItemID.None;
                    item.prefix = 0;
                }
                PlayerActivator = (byte)plr;
                if (Upgrade.upgradeTime > 0)
                {
                    UpgradeTime = Upgrade.upgradeTime;
                }
                else
                {
                    UpgradeTime = SiegeUpgrade.UpgradeTime_PreHardmode;
                }
                X = (ushort)x;
                Y = (ushort)y;
                active = true;
                return true;
            }
            return false;
        }

        public static void Deactivate()
        {
            if (BaseItem != null && BaseItem.type > ItemID.None && Main.netMode != NetmodeID.MultiplayerClient)
            {
                int x = X * 16 + 8;
                int y = Y * 16 + 8;
                y -= 24;
                Item.NewItem(new Rectangle(x, y, 16, 16), BaseItem.type, 1, false, BaseItem.prefix);
            }
            Reset();
        }

        public static void UpgradeItem(bool doEffects = true)
        {
            if (BaseItem == null)
                return;
            Item item = BaseItem;
            int type = BaseItem.type;
            byte prefix = item.prefix;
            if (Upgrade.rewardItem > 0)
                type = Upgrade.rewardItem;
            var rectangle = AltarRectangle();
            rectangle = new Rectangle(rectangle.X * 16, rectangle.Y * 16, rectangle.Width * 16, rectangle.Height * 16);
            rectangle.Y -= 32;
            if (doEffects && Main.netMode != NetmodeID.Server)
            {
                var center = new Vector2(rectangle.X + rectangle.Width / 2f, rectangle.Y + rectangle.Height / 2f);
                Main.PlaySound(SoundID.DD2_DarkMageSummonSkeleton, new Vector2(center.X, center.Y));
                Main.PlaySound(SoundID.DD2_KoboldExplosion, new Vector2(center.X, center.Y));
                for (int i = 0; i < 50; i++)
                {
                    int d = Dust.NewDust(center, 2, 2, DustID.Fire);
                    var normal = new Vector2(0f, 1f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi));
                    Main.dust[d].position = center + normal * 16f;
                    Main.dust[d].velocity = normal * 4f;
                    Main.dust[d].noGravity = true;
                }
                var direction = new Vector2(1f, 0f);
                for (int i = 0; i < 10; i++)
                {
                    int d = Dust.NewDust(center, 2, 2, DustID.Fire);
                    Main.dust[d].position = center + direction * 16f;
                    Main.dust[d].velocity = direction * (4f + i * 0.1f);
                    Main.dust[d].noGravity = true;
                }
                direction = new Vector2(-1f, 0f);
                for (int i = 0; i < 10; i++)
                {
                    int d = Dust.NewDust(center, 2, 2, DustID.Fire);
                    Main.dust[d].position = center + direction * 16f;
                    Main.dust[d].velocity = direction * (4f + i * 0.1f);
                    Main.dust[d].noGravity = true;
                }
                direction = new Vector2(0f, 1f);
                for (int i = 0; i < 10; i++)
                {
                    int d = Dust.NewDust(center, 2, 2, DustID.Fire);
                    Main.dust[d].position = center + direction * 16f;
                    Main.dust[d].velocity = direction * (4f + i * 0.1f);
                    Main.dust[d].noGravity = true;
                }
                direction = new Vector2(0f, -1f);
                for (int i = 0; i < 10; i++)
                {
                    int d = Dust.NewDust(center, 2, 2, DustID.Fire);
                    Main.dust[d].position = center + direction * 16f;
                    Main.dust[d].velocity = direction * (4f + i * 0.1f);
                    Main.dust[d].noGravity = true;
                }
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
                Item.NewItem(rectangle, type, 1, false, prefix);
            BaseItem = null;
        }

        private static void WriteMessage(string text)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText(text, TextColor);
            }
            else
            {
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    // Dead players count towards the broadcast if they are close enough to the Demon Siege event
                    if (Main.player[i].active && Main.player[i].Distance(new Vector2(X * 16f, Y * 16f)) < 2000f)
                    {
                        var clr = TextColor;
                        MessageBroadcast.Broadcast(text, clr.R, clr.G, clr.B);
                        return;
                    }
                }
                Main.NewText(text, TextColor);
            }
        }
        private static bool UpdateEvent_CheckPlayer()
        {
            var player = Main.player[PlayerActivator];
            if (player.dead || !player.active)
            {
                WriteMessage(Language.GetTextValue("Mods.AQMod.Common.DemonSiegeDeath"));
                Deactivate();
                return false;
            }
            if (player.Distance(new Vector2(X * 16f, Y * 16f)) > 2000f)
            {
                WriteMessage(Language.GetTextValue("Mods.AQMod.Common.DemonSiegeTooFarAway"));
                Deactivate();
                return false;
            }
            return true;
        }
        private static void UpdateEvent_NPCSpawning()
        {
            if (spawnEnemyX != -1 && spawnEnemyY != -1)
            {
                if (spawnEnemyTimer == spawnEnemy.spawnTime)
                {
                    Projectile.NewProjectile(new Vector2(spawnEnemyX * 16f + 8f, spawnEnemyY * 16f + 16f), Vector2.Zero,
                        ModContent.ProjectileType<DemonSiegeSpawnEffect>(), 0, 0, Main.myPlayer, spawnEnemy.spawnTime, spawnEnemy.spawnWidth);
                }
                spawnEnemyTimer--;
                if (spawnEnemyTimer <= 0)
                {
                    int n = NPC.NewNPC(spawnEnemyX * 16 + 8, spawnEnemyY * 16 + 16, spawnEnemy.type);
                    Main.npc[n].position.X = spawnEnemyX * 16f + 8f - Main.npc[n].width / 2f;
                    Main.npc[n].position.Y = spawnEnemyY * 16 + 16 - Main.npc[n].height;
                    spawnEnemyTimer = 0;
                    spawnEnemyX = -1;
                    spawnEnemyY = -1;
                    if (Main.netMode != NetmodeID.Server)
                    {
                        for (int i = 0; i < 50; i++)
                        {
                            int d = Dust.NewDust(Main.npc[n].position, Main.npc[n].width, Main.npc[n].height, ModContent.DustType<DemonSpawnDust>());
                            Main.dust[d].velocity.X *= 0.666f;
                            Main.dust[d].velocity.Y -= 3.666f;
                        }
                    }
                }
            }
            else
            {
                if (Main.rand.Next(GetSpawnChance()) != 0)
                    return;
                var player = Main.player[PlayerActivator];
                var progression = Upgrade.progression;
                var enemies = new List<SiegeEnemy>();
                for (int i = 0; i < Enemies.Count; i++)
                {
                    if (Enemies[i].progression <= progression)
                        enemies.Add(Enemies[i]);
                }
                var spawn = enemies[Main.rand.Next(enemies.Count)];
                if (spawn.type == ModContent.NPCType<TrapImp>())
                {
                    int trapperCount = NPC.CountNPCS(ModContent.NPCType<Trapper>());
                    if (trapperCount > 1 && Main.rand.Next(trapperCount) == 0)
                        return;
                }
                int npcCount = NPC.CountNPCS(spawn.type);
                if (npcCount > 1 && Main.rand.Next(npcCount) == 0)
                    return;
                int cX = 0;
                int cY = 0;
                int playerX = (int)((player.position.X + player.width / 2f) / 16f);
                int playerY = (int)((player.position.Y + player.height / 2f) / 16f);
                for (int i = 0; i < 1000; i++)
                {
                    int x = playerX + Main.rand.Next(-40, 40);
                    int y = playerY + Main.rand.Next(-20, 20);
                    if (i >= 999)
                    {
                        cX = x;
                        cY = y;
                        break;
                    }
                    if (x < 10)
                    {
                        x = 10;
                    }
                    else if (x > Main.maxTilesX - 10)
                    {
                        x = Main.maxTilesX - 10;
                    }
                    if (y < 10)
                    {
                        y = 10;
                    }
                    else if (y > Main.maxTilesY - 10)
                    {
                        y = Main.maxTilesY - 10;
                    }
                    if (Main.tile[x, y] == null)
                        Main.tile[x, y] = new Tile();
                    if (Main.tile[x, y].active() && Main.tileSolid[Main.tile[x, y].type])
                        continue;
                    if (!Main.tile[x, y + 1].active() || !Main.tileSolid[Main.tile[x, y + 1].type])
                        continue;
                    int xOff = x - playerX;
                    int yOff = y - playerY;
                    if (Math.Sqrt(xOff * xOff + yOff * yOff) < 12.0)
                        continue;
                    if (Math.Sqrt(xOff * xOff + yOff * yOff) > 24.0 && !Collision.CanHit(new Vector2(x * 16f, y * 16f), 16, 16, player.position, player.width, player.height))
                        continue;
                    cX = x;
                    cY = y;
                    break;
                }
                spawnEnemyX = cX;
                spawnEnemyY = cY;
                spawnEnemy = spawn;
                spawnEnemyTimer = spawn.spawnTime;
            }
        }
        public static void UpdateEvent()
        {
            if (!IsActive || PlayerActivator == byte.MaxValue || PlayerActivator == Main.myPlayer && !UpdateEvent_CheckPlayer())
                return;
            if (Main.tile[X, Y] == null)
            {
                Main.tile[X, Y] = new Tile();
                Deactivate();
                return;
            }
            if (!Main.tile[X, Y].active() || Main.tile[X, Y].type != ModContent.TileType<GoreNest>())
            {
                Deactivate();
                return;
            }
            if (UpgradeTime == 0)
            {
                UpgradeItem(doEffects: true);
                Deactivate();
                WorldDefeats.DownedDemonSiege = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            else
            {
                if (UpgradeTime <= 300 && UpgradeTime >= 60)
                {
                    if (UpgradeTime % 60 == 0)
                        Main.PlaySound(SoundID.Item98, new Vector2(X * 16f, Y * 16f));
                }
                UpgradeTime--;
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    return;
                UpdateEvent_NPCSpawning();
            }
        }

        private static int GetSpawnChance()
        {
            return Main.expertMode ? 150 : 300;
        }

        public static void Reset()
        {
            active = false;
            X = 0;
            Y = 0;
            spawnEnemyX = -1;
            spawnEnemyY = -1;
            UpgradeTime = 1;
            BaseItem = null;
            PlayerActivator = byte.MaxValue;
        }

        public override void NetSend(BinaryWriter writer)
        {
            if (BaseItem == null)
            {
                Deactivate();
            }
            writer.Write(active);
            if (active)
            {
                writer.Write(spawnEnemyX);
                writer.Write(spawnEnemyY);
                writer.Write(spawnEnemyTimer);
                writer.Write(UpgradeTime);
                writer.Write(X);
                writer.Write(Y);

                AQUtils.NetWriteItem(writer, BaseItem);
            }
        }

        public override void NetReceive(BinaryReader reader)
        {
            active = reader.ReadBoolean();
            if (active)
            {
                spawnEnemyX = reader.ReadInt32();
                spawnEnemyY = reader.ReadInt32();
                spawnEnemyTimer = reader.ReadInt32();
                UpgradeTime = reader.ReadUInt16();
                X = reader.ReadUInt16();
                Y = reader.ReadUInt16();

                BaseItem = AQUtils.NetRecieveItem(reader);
            }
        }

        /// <summary>
        /// Tries to get a demon siege instance based on the item's type. Returns null if no upgrade was found
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static SiegeUpgrade? GetUpgrade(Item item)
        {
            foreach (var d in Upgrades)
            {
                if (d.baseItem == item.type)
                    return d;
            }
            return null;
        }

        public static Point AltarCorner()
        {
            var tile = Framing.GetTileSafely(X, Y);
            return new Point(X - tile.frameX / 18, Y - tile.frameY / 18);
        }

        public static Rectangle AltarRectangle()
        {
            var corner = AltarCorner();
            return new Rectangle(corner.X, corner.Y, 3, 3);
        }

        public static (SiegeUpgrade? upgrade, Item item) FindUpgradeableItem(Player player)
        {
            for (int i = 0; i < Main.maxInventory; i++)
            {
                var upgrade = GetUpgrade(player.inventory[i]);
                if (upgrade != null)
                {
                    return (upgrade, player.inventory[i]);
                }
            }
            return (null, null);
        }
    }
}