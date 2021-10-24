using AQMod.Common;
using AQMod.Content.Dusts;
using AQMod.Effects.WorldEffects;
using AQMod.Items.Weapons.Magic;
using AQMod.Items.Weapons.Melee;
using AQMod.Items.Weapons.Ranged;
using AQMod.Localization;
using AQMod.NPCs.Monsters.DemonicEvent;
using AQMod.Tiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace AQMod.Content.WorldEvents.DemonSiege
{
    public class DemonSiege
    {
        private static bool _active;
        public static bool IsActive => _active && X > 0 && Y > 0;
        public static Color TextColor => new Color(250, 95, 10, 255);

        internal static List<DemonSiegeUpgrade> _upgrades;
        internal static List<DemonSiegeEnemy> _enemies;

        public static ushort X;
        public static ushort Y;
        public static ushort UpgradeTime;
        public static byte PlayerActivator { get; private set; }
        public static Item BaseItem { get; private set; }
        public static DemonSiegeUpgrade Upgrade { get; private set; }

        public static DemonSiegeEnemy spawnEnemy;
        public static int spawnEnemyX = -1;
        public static int spawnEnemyY = -1;
        public static int spawnEnemyTimer;

        public const int SPAWN_ENEMY_DELAY = 150;

        private static List<int> HellBanners()
        {
            return new List<int>()
            {
                ItemID.HellboundBanner,
                ItemID.HellHammerBanner,
                ItemID.HelltowerBanner,
                ItemID.LostHopesofManBanner,
                ItemID.ObsidianWatcherBanner,
                ItemID.LavaEruptsBanner,
            };
        }

        public static int GetHellBannerDrop(UnifiedRandom rand)
        {
            var l = HellBanners();
            return l[rand.Next(l.Count)];
        }

        public static Point altarTopLeft()
        {
            var tile = Framing.GetTileSafely(X, Y);
            return new Point(X - tile.frameX / 18, Y - tile.frameY / 18);
        }

        public static Rectangle altarRectangle()
        {
            var topLeft = altarTopLeft();
            return new Rectangle(topLeft.X, topLeft.Y, 3, 3);
        }

        /// <summary>
        /// Tries to get a demon siege instance based on the item's type. Returns null if no upgrade was found
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static DemonSiegeUpgrade? GetUpgrade(Item item)
        {
            foreach (var d in _upgrades)
            {
                if (d.baseItem == item.type)
                    return d;
            }
            return null;
        }

        internal static void Setup()
        {
            Reset();
            _upgrades = new List<DemonSiegeUpgrade>();
            AddDemonSeigeUpgrade(new DemonSiegeUpgrade(ItemID.LightsBane, ModContent.ItemType<HellsBoon>(), DemonSiegeUpgradeProgression.PreHardmode, DemonSiegeUpgrade.UpgradeTime_PreHardmode));
            AddDemonSeigeUpgrade(new DemonSiegeUpgrade(ItemID.BloodButcherer, ModContent.ItemType<CrimsonHellSword>(), DemonSiegeUpgradeProgression.PreHardmode, DemonSiegeUpgrade.UpgradeTime_PreHardmode));
            AddDemonSeigeUpgrade(new DemonSiegeUpgrade(ItemID.DemonBow, ModContent.ItemType<HamaYumi>(), DemonSiegeUpgradeProgression.PreHardmode, DemonSiegeUpgrade.UpgradeTime_PreHardmode));
            AddDemonSeigeUpgrade(new DemonSiegeUpgrade(ItemID.TendonBow, ModContent.ItemType<Deltoid>(), DemonSiegeUpgradeProgression.PreHardmode, DemonSiegeUpgrade.UpgradeTime_PreHardmode));
            AddDemonSeigeUpgrade(new DemonSiegeUpgrade(ModContent.ItemType<SeltzerRain>(), ModContent.ItemType<FizzlingFire>(), DemonSiegeUpgradeProgression.PreHardmode, DemonSiegeUpgrade.UpgradeTime_PreHardmode));

            _enemies = new List<DemonSiegeEnemy>();
            AddDemonSeigeEnemy(DemonSiegeEnemy.FromT<Cindera>(DemonSiegeUpgradeProgression.PreHardmode, DemonSiegeEnemy.SPAWNTIME_CINDERA, 20));
            AddDemonSeigeEnemy(DemonSiegeEnemy.FromT<TrapImp>(DemonSiegeUpgradeProgression.PreHardmode, DemonSiegeEnemy.SPAWNTIME_PRE_HARDMODE_REGULAR, 32));
            AddDemonSeigeEnemy(DemonSiegeEnemy.FromT<Magmalbubble>(DemonSiegeUpgradeProgression.PreHardmode, DemonSiegeEnemy.SPAWNTIME_PRE_HARDMODE_REGULAR, 32));
        }

        internal static void Unload()
        {
            _upgrades = null;
            _enemies = null;
            BaseItem = null;
        }

        public static void AddDemonSeigeUpgrade(DemonSiegeUpgrade upgrade)
        {
            if (AQMod.Loading)
                _upgrades.Add(upgrade);
        }

        public static void AddDemonSeigeEnemy(DemonSiegeEnemy enemy)
        {
            if (AQMod.Loading)
                _enemies.Add(enemy);
        }

        public static bool CheckSpot(int x, int y)
        {
            var tile = Framing.GetTileSafely(x, y);
            if (tile.active() && tile.type == ModContent.TileType<GoreNest>())
                return true;
            return false;
        }

        public static bool Activate(int x, int y, int p, Item item)
        {
            if (CheckSpot(x, y))
            {
                BaseItem = item.Clone();
                Upgrade = GetUpgrade(BaseItem).GetValueOrDefault(default(DemonSiegeUpgrade));
                PlayerActivator = (byte)p;
                item.TurnToAir();
                item.type = ItemID.None;
                item.prefix = 0;
                if (Upgrade.upgradeTime > 0)
                {
                    UpgradeTime = Upgrade.upgradeTime;
                }
                else
                {
                    UpgradeTime = DemonSiegeUpgrade.UpgradeTime_PreHardmode;
                }
                X = (ushort)x;
                Y = (ushort)y;
                _active = true;
                return true;
            }
            return false;
        }

        public static void Deactivate()
        {
            if (BaseItem != null && BaseItem.type > ItemID.None)
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
            var rectangle = altarRectangle();
            rectangle = new Rectangle(rectangle.X * 16, rectangle.Y * 16, rectangle.Width * 16, rectangle.Height * 16);
            rectangle.Y -= 32;
            if (doEffects)
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
            Item.NewItem(rectangle, type, 1, false, prefix);
            BaseItem = null;
        }

        public static bool CloseEnoughToDemonSiege(Player player)
        {
            if (!IsActive)
                return false;
            return Vector2.Distance(player.Center, new Vector2(X * 16f, Y * 16f)) < 2000f;
        }

        public static void UpdateEvent()
        {
            if (!IsActive)
                return;
            if (PlayerActivator == byte.MaxValue)
                return;
            var player = Main.player[PlayerActivator];
            if (!CloseEnoughToDemonSiege(player))
            {
                if (PlayerActivator == Main.myPlayer)
                    Main.NewText(AQText.ModText("Common.DemonSiegeTooFarAway"), TextColor);
                Deactivate();
                return;
            }
            if (player.dead || !player.active)
            {
                if (PlayerActivator == Main.myPlayer)
                    Main.NewText(AQText.ModText("Common.DemonSiegeDeath"), TextColor);
                Deactivate();
                return;
            }
            if (PlayerActivator == Main.myPlayer)
            {
                if (UpgradeTime <= 300 && UpgradeTime >= 60)
                {
                    if (UpgradeTime % 60 == 0)
                        Main.PlaySound(SoundID.Item98, new Vector2(X * 16f, Y * 16f));
                }
            }
            if (UpgradeTime == 0)
            {
                UpgradeItem(doEffects: true);
                Deactivate();
                WorldDefeats.DownedDemonSiege = true;
            }
            else
            {
                UpgradeTime--;
                SpawnEnemies(player);
            }
        }

        private static int GetSpawnChance()
        {
            return Main.expertMode ? 200 : 300;
        }

        private static void SpawnEnemies(Player player)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            if (spawnEnemyX != -1 && spawnEnemyY != -1)
            {
                if (Main.netMode != NetmodeID.Server && spawnEnemyTimer == SPAWN_ENEMY_DELAY)
                    AQMod.WorldEffects.Add(new DemonSiegeSpawnEffect(spawnEnemyX * 16 + 8, spawnEnemyY * 16 + 16, spawnEnemy));
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
                var progression = Upgrade.progression;
                var enemies = new List<DemonSiegeEnemy>();
                for (int i = 0; i < _enemies.Count; i++)
                {
                    if (_enemies[i].progression <= progression)
                        enemies.Add(_enemies[i]);
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

        public static void Reset()
        {
            _active = false;
            X = 0;
            Y = 0;
            spawnEnemyX = -1;
            spawnEnemyY = -1;
            UpgradeTime = 1;
            BaseItem = null;
            PlayerActivator = byte.MaxValue;
        }
    }
}