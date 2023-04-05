using Aequus;
using Aequus.Common.GlobalProjs;
using Aequus.Common.Recipes;
using Aequus.Common.Utilities.TypeUnboxing;
using Aequus.Content.Town.CarpenterNPC.Quest;
using Aequus.Items;
using Aequus.Projectiles;
using Aequus.Tiles;
using log4net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Creative;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.UI.Elements;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using Terraria.UI;
using Terraria.Utilities;

namespace Aequus
{
    public static partial class Helper
    {
        public const char AirCharacter = '⠀';
        public const string AirString = "⠀";

        public static int iterations;
        public static bool AnyIterations => iterations > 0;

        /// <summary>
        /// Determines whether or not the mouse has an item
        /// </summary>
        public static bool HasMouseItem => Main.mouseItem != null && !Main.mouseItem.IsAir;
        public static Vector2 ScaledMouseScreen => Vector2.Transform(Main.ReverseGravitySupport(Main.MouseScreen, 0f), Matrix.Invert(Main.GameViewMatrix.ZoomMatrix));
        public static Vector2 ScaledMouseworld => ScaledMouseScreen + Main.screenPosition;
        public const BindingFlags LetMeIn = BindingFlags.NonPublic | BindingFlags.Instance;

        public static Point MouseTile => Main.MouseWorld.ToTileCoordinates();
        public static int MouseTileX => (int)Main.MouseWorld.X / 16;
        public static int MouseTileY => (int)Main.MouseWorld.Y / 16;

        public static Regex SubstitutionRegex { get; private set; }
        public static TypeUnboxer<int> UnboxInt { get; private set; }
        public static TypeUnboxer<float> UnboxFloat { get; private set; }
        public static TypeUnboxer<bool> UnboxBoolean { get; private set; }

        public static Color ColorGreenSlime => ContentSamples.NpcsByNetId[NPCID.GreenSlime].color;
        public static Color ColorBlueSlime => ContentSamples.NpcsByNetId[NPCID.BlueSlime].color;
        public static Color ColorFurniture => new Color(191, 142, 111, 255);
        public static Color ColorLightedFurniture => new Color(253, 221, 3, 255);

        private static Mod Mod => ModContent.GetInstance<Aequus>();

        #region RNG
        public static ulong TileSeed(int i, int j)
        {
            ulong x = (ulong)i;
            ulong y = (ulong)j;
            return x * x + y * y * x + x;
        }
        public static ulong TileCoordinatesSeed(Point point)
        {
            return TileSeed(point.X, point.Y);
        }
        #endregion

        #region Collision
        public static Vector2 ClosestDistance(this Rectangle rect, Vector2 other)
        {
            var center = rect.Center.ToVector2();
            var n = Vector2.Normalize(other - center);
            float x = Math.Min((other.X - center.X).Abs(), rect.Width / 2f);
            float y = Math.Min((other.Y - center.Y).Abs(), rect.Height / 2f);
            return center + n * new Vector2(x, y);
        }

        public static bool InSolidCollision(this Rectangle worldRectangle)
        {
            return Collision.SolidCollision(new(worldRectangle.X, worldRectangle.Y), worldRectangle.Width, worldRectangle.Height);
        }
        #endregion

        #region Cross Mod
        public static bool TryAddItem<TValue>(this Mod mod, Dictionary<int, TValue> list, string itemName, TValue value)
        {
            if (mod.TryFind<ModItem>(itemName, out var modItem))
            {
                list.Add(modItem.Type, value);
            }
            return false;
        }
        public static bool TryAddItem(this Mod mod, HashSet<int> list, string itemName)
        {
            if (mod.TryFind<ModItem>(itemName, out var modItem))
            {
                list.Add(modItem.Type);
            }
            return false;
        }
        public static bool TryAddItem(this Mod mod, List<int> list, string itemName)
        {
            if (mod.TryFind<ModItem>(itemName, out var modItem))
            {
                list.Add(modItem.Type);
            }
            return false;
        }
        #endregion

        public static void SetValue(this LocalizedText text, string value) {
            TextHelper.LocalizedText_SetValue.Invoke(text, new object[] { value, });
        }

        public static float ScaleDown(float value, float minThreshold, float scaleMultiplier)
        {
            return (value - minThreshold) * scaleMultiplier + minThreshold;
        }

        public static Recipe ResultPrefix<T>(this Recipe recipe) where T : ModPrefix
        {
            AequusRecipes.PrefixedRecipeResultOverride.Add(recipe.createItem.type);
            recipe.createItem.Prefix(ModContent.PrefixType<T>());
            return recipe;
        }

        public static bool ShadedSpot(int x, int y)
        {
            if (!WorldGen.InWorld(x, y) || y > (int)Main.worldSurface || Main.tile[x, y].IsFullySolid())
                return true;

            for (int j = 1; j < 10; j++)
            {
                if (!WorldGen.InWorld(x, y - 1, 10))
                {
                    break;
                }

                if (Main.tile[x, y - j].HasTile
                    && !Main.tileSolidTop[Main.tile[x, y - j].TileType]
                    && Main.tileSolid[Main.tile[x, y - j].TileType])
                {
                    return true;
                }
            }

            return Main.tile[x, y].WallType == WallID.None && !WallID.Sets.Transparent[Main.tile[x, y].WallType] && !WallID.Sets.AllowsWind[Main.tile[x, y].WallType];
        }
        public static bool ShadedSpot(Point tileCoordinates)
        {
            return ShadedSpot(tileCoordinates.X, tileCoordinates.Y);
        }
        public static bool ShadedSpot(Vector2 worldCoordinates)
        {
            return ShadedSpot(worldCoordinates.ToTileCoordinates());
        }

        public static bool InOuterThirds(this Vector2 where)
        {
            return Math.Abs(where.X - Main.maxTilesX * 8f) > Main.maxTilesX * 5.33f;
        }

        public static bool SetQuestFish(int itemType)
        {
            for (int i = 0; i < Main.anglerQuestItemNetIDs.Length; i++)
            {
                if (Main.anglerQuestItemNetIDs[i] == itemType)
                {
                    Main.anglerQuest = i;
                    return true;
                }
            }
            return false;
        }


        #region FastRandom Extensions
        public static int Int(this ref FastRandom rand, int min, int max)
        {
            rand.NextSeed();
            return rand.Next(max);
        }
        public static int Int(this ref FastRandom rand, int max)
        {
            rand.NextSeed();
            return rand.Next(max);
        }

        public static bool Bool(this ref FastRandom rand)
        {
            return Bool(ref rand, 2);
        }
        public static bool Bool(this ref FastRandom rand, int consequent)
        {
            rand.NextSeed();
            return rand.Next(consequent) == 0;
        }

        public static float Float(this ref FastRandom rand, float max)
        {
            return (float)(Float(ref rand) * max);
        }
        public static float Float(this ref FastRandom rand, float min, float max)
        {
            return (float)(Float(ref rand) * (max - min) + min);
        }
        public static float Float(this ref FastRandom rand)
        {
            rand.NextSeed();
            return (float)rand.NextFloat();
        }
        #endregion

        public static int QualityFromFPS(int highQ, int lowQ)
        {
            return (int)MathHelper.Lerp(highQ, lowQ, Math.Clamp(1f - Main.frameRate / 60f, 0f, 1f));
        }

        public static int FindFloor(int x, int y, int distance = 60)
        {
            if (WorldGen.InWorld(x, y) && Main.tile[x, y].IsFullySolid())
            {
                for (int j = 0; j > -distance; j--)
                {
                    if (!WorldGen.InWorld(x, y + j, 30))
                        continue;
                    if (!Main.tile[x, y + j].IsFullySolid() || Main.tile[x, y + j].SolidTopType())
                    {
                        return y + j;
                    }
                }
            }
            for (int j = 0; j < distance; j++)
            {
                if (!WorldGen.InWorld(x, y + j, 30))
                    continue;
                if (Main.tile[x, y + j].IsFullySolid() || Main.tile[x, y + j].SolidTopType())
                {
                    return y + j;
                }
            }
            return -1;
        }

        /// <summary>
        /// Attempts to find an <see cref="IItemDropRule"/> which falls under the condition, then replaces it with a <see cref="LeadingConditionRule"/> which activates the rule on success. Returns false if no rule is found.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="loot"></param>
        /// <param name="condition"></param>
        /// <param name="ruleCondition"></param>
        /// <returns></returns>
        public static bool AddConditionToExistingRule<T>(this NPCLoot loot, Func<T, bool> condition, IItemDropRuleCondition ruleCondition) where T : class, IItemDropRule
        {
            if (loot.Find(condition, out var rule))
            {
                loot.RemoveWhere((rule) => rule is T wantedRule && condition(wantedRule));
                loot.Add(new LeadingConditionRule(ruleCondition)).OnSuccess(rule);
            }
            return true;
        }
        /// <summary>
        /// Attempts to find an <see cref="IItemDropRule"/> which falls under the condition. Returns false if no rule is found.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="loot"></param>
        /// <param name="condition"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        public static bool Find<T>(this ItemLoot loot, Func<T, bool> condition, out T rule) where T : class, IItemDropRule
        {
            return Find(loot.Get(includeGlobalDrops: false), condition, out rule);
        }
        /// <summary>
        /// Attempts to find an <see cref="IItemDropRule"/> which falls under the condition. Returns false if no rule is found.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="loot"></param>
        /// <param name="condition"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        public static bool Find<T>(this NPCLoot loot, Func<T, bool> condition, out T rule) where T : class, IItemDropRule
        {
            return Find(loot.Get(includeGlobalDrops: false), condition, out rule);
        }
        /// <summary>
        /// Attempts to find an <see cref="IItemDropRule"/> which falls under the condition. Returns false if no rule is found.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="condition"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        public static bool Find<T>(this List<IItemDropRule> list, Func<T, bool> condition, out T rule) where T : class, IItemDropRule
        {
            rule = default(T);
            var rule2 = list.Find((rule) => rule is T wantedRule && condition(wantedRule));
            if (rule2 is T)
            {
                rule = (T)rule2;
                return true;
            }
            return false;
        }
        /// <summary>
        /// Attempts to find the first result of <see cref="IItemDropRule"/> of the specified type parameter <typeparamref name="T"/>. Recommended to use <see cref="Find{T}(List{IItemDropRule}, Func{T, bool}, out T)"/> instead for more consistent results. Returns false if no rule is found.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        public static bool FindFirst<T>(this List<IItemDropRule> list, out IItemDropRule rule) where T : class, IItemDropRule
        {
            rule = list.Find((rule) => rule is T);
            return rule != default(IItemDropRule);
        }

        public static void ResetVanillaNPCTexture(int npcID)
        {
            TextureAssets.Npc[npcID] = GetVanillaNPCTexture(npcID);
        }
        public static Asset<Texture2D> GetVanillaNPCTexture(int npcID)
        {
            return ModContent.Request<Texture2D>($"Terraria/Images/NPC_{npcID}");
        }

        public static void FixUIText(this UIText text)
        {
            text.MinWidth.Set(0f, 1f);
            text.MinHeight.Set(0f, 1f);
            text.Recalculate();
        }

        public static T GetOrDefault<T>(this TagCompound tag, string key, T defaultValue)
        {
            if (tag.TryGet<T>(key, out var val))
            {
                return val;
            }
            return defaultValue;
        }

        public static Vector2 RotateTowards(Vector2 currentPosition, Vector2 currentVelocity, Vector2 targetPosition, float maxChange)
        {
            float scaleFactor = currentVelocity.Length();
            float targetAngle = currentPosition.AngleTo(targetPosition);
            return currentVelocity.ToRotation().AngleTowards(targetAngle, maxChange).ToRotationVector2() * scaleFactor;
        }

        public static Color GetRarityColor(int rare)
        {
            switch (rare)
            {
                default:
                    if (rare > ItemRarityID.Purple)
                    {
                        return RarityLoader.GetRarity(rare).RarityColor;
                    }
                    return Color.White;

                case ItemRarityID.Master:
                    return new Color(255, (byte)(Main.masterColor * 200f), 0, 255);
                case ItemRarityID.Expert:
                    return Main.DiscoColor;
                case ItemRarityID.Quest:
                    return Colors.RarityAmber;
                case ItemRarityID.Gray:
                    return Color.Gray;
                case ItemRarityID.White:
                    return Color.White;
                case ItemRarityID.Blue:
                    return Colors.RarityBlue;
                case ItemRarityID.Green:
                    return Colors.RarityGreen;
                case ItemRarityID.Orange:
                    return Colors.RarityOrange;
                case ItemRarityID.LightRed:
                    return Colors.RarityRed;
                case ItemRarityID.Pink:
                    return Colors.RarityPink;
                case ItemRarityID.LightPurple:
                    return Colors.RarityPurple;
                case ItemRarityID.Lime:
                    return Colors.RarityLime;
                case ItemRarityID.Yellow:
                    return Colors.RarityYellow;
                case ItemRarityID.Cyan:
                    return Colors.RarityCyan;
                case ItemRarityID.Red:
                    return Colors.RarityDarkRed;
                case ItemRarityID.Purple:
                    return Colors.RarityDarkPurple;
            }
        }

        public static void SaveRarity(TagCompound tag, string modKey, string vanillaKey, int rare)
        {
            if (rare > ItemRarityID.Purple)
            {
                tag[modKey] = RarityLoader.GetRarity(rare).FullName;
            }
            else
            {
                tag[vanillaKey] = rare;
            }
        }

        public static bool LoadRarity(TagCompound tag, string modKey, string vanillaKey, out int value)
        {
            value = default(int);
            if (tag.TryGet(modKey, out string rarityName))
            {
                var split = rarityName.Split('/');
                if (!ModLoader.TryGetMod(split[0], out var mod))
                {
                    return false;
                }
                if (!mod.TryFind<ModRarity>(split[1], out var rare))
                {
                    return false;
                }
                value = rare.Type;
                return true;
            }
            else if (tag.TryGet(vanillaKey, out int rare))
            {
                value = rare;
                return true;
            }
            return false;
        }

        public static void DebugTagCompound(TagCompound tag, ILog logger)
        {
            foreach (var val in tag)
            {
                logger.Debug(val.Key + ": " + val.Value.ToString());
            }
        }

        public static void AddToTime(double time, double add, bool dayTime, out double result, out bool resultDayTime)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            while (add > 0)
            {
                double max = dayTime ? Main.dayLength : Main.nightLength;
                if (time + add > max)
                {
                    add -= (max - time);
                    dayTime = !dayTime;
                }
                else
                {
                    time += add;
                    add = 0;
                }
                if (stopWatch.ElapsedMilliseconds > 10)
                    break;
            }
            stopWatch.Stop();
            result = time;
            resultDayTime = dayTime;
        }

        public static Color ValueColor(int value)
        {
            return value >= Item.platinum ? Colors.CoinPlatinum : (value >= Item.gold ? Colors.CoinGold : (value >= Item.silver ? Colors.CoinSilver : (value >= Item.copper ? Colors.CoinCopper : Color.Gray)));
        }

        public static Color ReadColor(string text)
        {
            var val = text.Split(',');
            return new Color(int.Parse(val[0].Trim()), int.Parse(val[1].Trim()), int.Parse(val[2].Trim()), val.Length > 3 ? int.Parse(val[3].Trim()) : 255);
        }

        public static bool ConsumeItem(Player player, Item item)
        {
            if (ItemLoader.ConsumeItem(item, player))
            {
                item.stack--;
            }
            if (item.stack <= 0)
                item.TurnToAir();
            return true;
        }

        public static int EquipmentStacks(this Item item, int baseAmount = 1) {
            var empowerment = item.Aequus().equipEmpowerment;
            if (empowerment == null) {
                return baseAmount;
            }
            return empowerment.addedStacks + baseAmount;
        }

        public static bool IsArmor(this Item item) {
            return !item.accessory && item.defense > 0 && (item.headSlot > 0 || item.bodySlot > 0 || item.legSlot > 0);
        }

        public static bool IsPet(this Item item)
        {
            return item.buffType > 0 && (Main.vanityPet[item.buffType] || Main.lightPet[item.buffType]);
        }

        public static bool IsLightPet(this Item item)
        {
            return item.buffType > 0 && Main.lightPet[item.buffType];
        }

        public static bool IsRegularPet(this Item item)
        {
            return item.buffType > 0 && Main.vanityPet[item.buffType];
        }

        public static bool IsATool(this Item item)
        {
            return item.pick > 0 || item.axe > 0 || item.hammer > 0;
        }

        public static void Transform(this Item item, int newType)
        {
            int prefix = item.prefix;
            int stack = item.stack;
            item.SetDefaults(newType);
            item.Prefix(prefix);
            item.stack = stack;
        }

        public static void Transform<T>(this Item item) where T : ModItem
        {
            Transform(item, ModContent.ItemType<T>());
        }

        public static Item SetDefaults<T>(this Item item) where T : ModItem
        {
            item.SetDefaults(ModContent.ItemType<T>());
            return item;
        }

        public static Rectangle TileRectangle(Vector2 worldPosition, int widthDiv2, int heightDiv2)
        {
            return new Rectangle((int)worldPosition.X / 16 - widthDiv2, (int)worldPosition.Y / 16 - heightDiv2, widthDiv2 * 2, heightDiv2 * 2);
        }

        public static Rectangle Frame(this Rectangle rectangle, int frameX, int frameY, int sizeOffsetX = 0, int sizeOffsetY = 0)
        {
            return new Rectangle(rectangle.X + (rectangle.Width - sizeOffsetX) * frameX, rectangle.Y + (rectangle.Width - sizeOffsetY) * frameY, rectangle.Width, rectangle.Height);
        }

        public static Rectangle MultiplyWH(this Rectangle rectangle, float multiplier)
        {
            return new Rectangle(rectangle.X, rectangle.Y, (int)(rectangle.Width * multiplier), (int)(rectangle.Height * multiplier));
        }

        public static Rectangle Multiply(this Rectangle rectangle, float multiplier)
        {
            return new Rectangle((int)(rectangle.X * multiplier), (int)(rectangle.Y * multiplier), (int)(rectangle.Width * multiplier), (int)(rectangle.Height * multiplier));
        }

        public static Vector2 NumFloor(this Vector2 myVector, int amt = 2)
        {
            return (myVector / amt).Floor() * amt;
        }

        public static bool IsMinionProj(this Projectile projectile) {
            return projectile.minion || projectile.sentry || ProjectileID.Sets.MinionShot[projectile.type] || ProjectileID.Sets.SentryShot[projectile.type];
        }

        public static bool IsHostile(this Projectile projectile, Player player)
        {
            return projectile.hostile || (projectile.friendly && Main.player[projectile.owner].team != player.team);
        }

        public static float GetMinionReturnSpeed(this Projectile projectile, float minSpeed = 10f, float playerSpeedThreshold = 1.25f)
        {
            return Math.Max(Math.Max(Main.player[projectile.owner].velocity.Length() * playerSpeedThreshold, minSpeed), (Main.player[projectile.owner].Center - projectile.Center).Length() / 32f * playerSpeedThreshold);
        }

        public static int GetTileStyle(int tileID, int frameX, int frameY)
        {
            var tileObjectData = TileObjectData.GetTileData(tileID, 0, 0);
            if (tileObjectData == null)
            {
                return -1;
            }

            int num = frameX / tileObjectData.CoordinateFullWidth;
            int num2 = frameY / tileObjectData.CoordinateFullHeight;
            int num3 = tileObjectData.StyleWrapLimit;
            if (num3 == 0)
            {
                num3 = 1;
            }

            int num4 = (!tileObjectData.StyleHorizontal) ? (num * num3 + num2) : (num2 * num3 + num);
            int result = num4 / tileObjectData.StyleMultiplier;
            _ = num4 % tileObjectData.StyleMultiplier;
            return result;
        }

        public static void OpenShop(int npcType)
        {
            Main.playerInventory = true;
            Main.npcChatText = "";
            Main.SetNPCShopIndex(99);
            Main.instance.shop[Main.npcShop].SetupShop(npcType);
        }

        public static bool CheckForSolidRoofAbove(Point p, int limit, out Point roof)
        {
            int endY = Math.Max(p.Y - limit, 36);
            roof = p;
            for (int j = p.Y; j > endY; j--)
            {
                if (Main.tile[p.X, j].IsFullySolid())
                {
                    roof.Y = j;
                    return true;
                }
            }
            return false;
        }
        public static bool CheckForSolidGroundOrLiquidBelow(Point p, int limit, out Point solidGround)
        {
            int endY = Math.Min(p.Y + limit, Main.maxTilesY - 36);
            solidGround = p;
            for (int j = p.Y; j < endY; j++)
            {
                if (Main.tile[p.X, j].IsFullySolid())
                {
                    solidGround.Y = j;
                    return true;
                }
                else if (Main.tile[p.X, j].LiquidAmount > 0)
                {
                    solidGround.Y = j;
                    return true;
                }
            }
            return false;
        }

        public static bool CheckForSolidGroundBelow(Point p, int limit, out Point solidGround)
        {
            int endY = Math.Min(p.Y + limit, Main.maxTilesY - 36);
            solidGround = p;
            for (int j = p.Y; j < endY; j++)
            {
                if (Main.tile[p.X, j].IsFullySolid())
                {
                    solidGround.Y = j;
                    return true;
                }
            }
            return false;
        }

        public static bool InSceneRenderedMap(this TileMapCache map, Point p)
        {
            return InSceneRenderedMap(map, p.X, p.Y);
        }

        public static bool InSceneRenderedMap(this TileMapCache map, int x, int y)
        {
            return x >= (PhotoRenderer.TilePaddingForChecking / 2) && x <= (map.Width - PhotoRenderer.TilePaddingForChecking / 2)
                && y >= (PhotoRenderer.TilePaddingForChecking / 2) && y <= (map.Width - PhotoRenderer.TilePaddingForChecking / 2);
        }

        public static int GetAnimationFrame(this Player player)
        {
            return Math.Clamp(player.bodyFrame.Y / player.bodyFrame.Height, 0, Main.OffsetsPlayerHeadgear.Length);
        }

        public static bool IsFriendly(this Player targetPlayer, Player me)
        {
            return !targetPlayer.hostile || targetPlayer.team <= 0 || targetPlayer.team == me.team;
        }

        public static List<string> GetStringListOfBiomes(this Player player)
        {
            var biomes = new List<string>();

            if (player.ZoneDungeon)
            {
                biomes.Add("Bestiary_Biomes.TheDungeon");
            }
            if (player.ZoneLihzhardTemple)
            {
                biomes.Add("Bestiary_Biomes.TheTemple");
            }
            if (player.ZoneCorrupt)
            {
                biomes.Add("Bestiary_Biomes.TheCorruption");
            }
            if (player.ZoneCrimson)
            {
                biomes.Add("Bestiary_Biomes.Crimson");
            }
            if (player.ZoneHallow)
            {
                biomes.Add("Bestiary_Biomes.TheHallow");
            }
            if (player.ZoneForest)
            {
                biomes.Add("Mods.Aequus.BiomeName.Forest");
            }
            if (player.ZoneJungle)
            {
                biomes.Add("Bestiary_Biomes.Jungle");
            }
            if (player.ZoneSnow)
            {
                biomes.Add("Mods.Aequus.BiomeName.Snow_Tundra");
            }
            if (player.ZoneBeach)
            {
                biomes.Add("Bestiary_Biomes.Ocean");
            }
            else if (player.ZoneDesert || player.ZoneUndergroundDesert)
            {
                biomes.Add("Bestiary_Biomes.Desert");
            }
            if (player.ZoneGemCave)
            {
                biomes.Add("Mods.Aequus.BiomeName.GemCave");
            }
            if (player.ZoneGranite)
            {
                biomes.Add("Bestiary_Biomes.Granite");
            }
            if (player.ZoneMarble)
            {
                biomes.Add("Bestiary_Biomes.Marble");
            }
            if (player.ZoneGlowshroom)
            {
                biomes.Add("ItemName.GlowingMushroom");
            }
            if (player.ZoneMeteor)
            {
                biomes.Add("Bestiary_Biomes.Meteor");
            }

            foreach (var m in ModLoader.Mods)
            {
                foreach (var modBiome in m.GetContent<ModBiome>())
                {
                    if (player.InModBiome(modBiome))
                    {
                        biomes.Add($"Mods.{modBiome.Mod.Name}.BiomeName.{modBiome.Name}");
                    }
                }
            }
            return biomes;
        }

        public static Item HeldItemFixed(this Player player)
        {
            if (Main.myPlayer == player.whoAmI && player.selectedItem == 58 && Main.mouseItem != null && !Main.mouseItem.IsAir)
            {
                return Main.mouseItem;
            }
            return player.HeldItem;
        }

        public static Point GetSpawn(this Player player) => new Point(GetSpawnX(player), GetSpawnY(player));
        public static int GetSpawnY(this Player player) => player.SpawnY > 0 ? player.SpawnY : Main.spawnTileY;
        public static int GetSpawnX(this Player player) => player.SpawnX > 0 ? player.SpawnX : Main.spawnTileX;

        /// <summary>
        /// Lazy version of <see cref="Player.IsInTileInteractionRange(int, int, TileReachCheckSettings)"/> which doesn't require you putting <see cref="TileReachCheckSettings"/> as a parameter
        /// </summary>
        /// <param name="player"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInTileInteractionRange(this Player player, int x, int y)
        {
            return player.IsInTileInteractionRange(x, y);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInTileInteractionRange(this Player player, int x, int y, int extraX, int extraY)
        {
            Player.tileRangeX += extraX;
            Player.tileRangeY += extraY;
            bool value = player.IsInTileInteractionRange(x, y);
            Player.tileRangeX -= extraX;
            Player.tileRangeY -= extraY;
            return value;
            //return player.IsInTileInteractionRange(x, y, new TileReachCheckSettings() { OverrideXReach = Player.tileRangeX + extraX, OverrideYReach = Player.tileRangeY + extraY });
        }

        public static bool Zen(this Player player, bool? active = null)
        {
            var zen = player.GetModPlayer<AequusPlayer>();
            if (active.HasValue)
                zen.forceZen = active.Value;
            return zen.forceZen;
        }

        public static bool HasItemCheckAllBanks<T>(this Player player) where T : ModItem
        {
            return HasItemCheckAllBanks(player, ModContent.ItemType<T>());
        }
        public static bool HasItemCheckAllBanks(this Player player, int item)
        {
            return player.HasItem(item) ||
                player.bank.HasItem(item) ||
                player.bank2.HasItem(item) ||
                player.bank3.HasItem(item) ||
                player.bank4.HasItem(item);
        }

        public static Item FindItemInInvOrVoidBag(this Player player, Predicate<Item> search, out bool inVoidBag)
        {
            var i = player.FindItem(search);
            inVoidBag = false;
            if (i != null)
                return i;
            inVoidBag = true;
            if (player.HasItem(ItemID.VoidLens))
                return player.bank4.FindItem(search);
            return null;
        }
        public static bool HasItemInInvOrVoidBag(this Player player, int item)
        {
            return player.HasItem(item) || (player.HasItem(ItemID.VoidLens) && player.bank4.HasItem(item));
        }

        public static void SquishAndStackItem(this Chest chest, int i)
        {
            for (int j = Chest.maxItems - 1; j > i; j--)
            {
                if (chest.item[j] != null && chest.item[i].type == chest.item[j].type && ItemLoader.CanStack(chest.item[i], chest.item[j]))
                {
                    int diff = Math.Min(chest.item[i].stack + chest.item[j].stack, chest.item[i].maxStack) - chest.item[i].stack;
                    chest.item[i].stack += diff;
                    chest.item[j].stack -= diff;
                    if (chest.item[j].stack <= 0)
                    {
                        chest.item[j].TurnToAir();
                    }
                    if (chest.item[i].stack >= chest.item[i].maxStack)
                    {
                        return;
                    }
                }
            }
        }
        public static void SquishAndStackContents(this Chest chest)
        {
            for (int i = 0; i < Chest.maxItems; i++)
            {
                if (chest.item[i] != null && chest.item[i].stack < chest.item[i].maxStack)
                {
                    SquishAndStackItem(chest, i);
                }
            }
            var l = new List<Item>();
            for (int i = 0; i < Chest.maxItems; i++)
            {
                if (chest.item[i] != null && !chest.item[i].IsAir)
                {
                    l.Add(chest.item[i]);
                }
            }
            for (int i = 0; i < l.Count; i++)
            {
                chest.item[i] = l[i].Clone();
            }
            for (int i = l.Count; i < Chest.maxItems; i++)
            {
                if (chest.item[i] == null)
                {
                    chest.item[i] = new Item();
                }
                else
                {
                    chest.item[i].TurnToAir();
                }
            }
        }
        public static Item AddItem(this Chest chest, int item, int stack = 1, int prefix = 0)
        {
            var emptySlot = chest.FindEmptySlot();
            if (emptySlot != null)
            {
                emptySlot.SetDefaults(item);
                emptySlot.stack = stack;
                if (prefix > 0)
                    emptySlot.Prefix(prefix);
            }
            return emptySlot;
        }

        public static bool IsSynced(this Chest chest)
        {
            if (chest.item == null)
                return false;
            for (int i = 0; i < Chest.maxItems; i++)
            {
                if (chest.item[i] == null)
                    return false;
            }
            return true;
        }

        public static Item FindItem(this Player player, Predicate<Item> search)
        {
            for (int i = 0; i < Main.InventorySlotsTotal; i++)
            {
                if (!player.inventory[i].IsAir && search(player.inventory[i]))
                    return player.inventory[i];
            }
            return null;
        }

        public static bool TryStackingInto(this Item[] inv, int maxSlots, Item item, out int i)
        {
            i = -1;
            while (item.stack > 0)
            {
                i = FindSuitableSlot(inv, maxSlots, item);
                if (i == -1)
                    return false;
                if (inv[i].IsAir)
                {
                    inv[i] = item.Clone();
                    return true;
                }
                int stack = inv[i].stack + item.stack;
                if (stack > inv[i].maxStack)
                {
                    item.stack = stack - inv[i].maxStack;
                    inv[i].stack = inv[i].maxStack;
                    continue;
                }
                inv[i].stack = stack;
                return true;
            }
            return false;
        }

        public static int FindSuitableSlot(this Item[] inv, int maxSlots, Item item)
        {
            if (item.stack != item.maxStack)
            {
                for (int i = 0; i < Chest.maxItems; i++)
                {
                    if (inv[i].type == item.type && inv[i].stack < inv[i].maxStack && inv[i].prefix == item.prefix && ItemLoader.CanStack(item, inv[i]))
                        return i;
                }
            }
            for (int i = 0; i < Chest.maxItems; i++)
            {
                if (inv[i].IsAir)
                    return i;
            }
            return -1;
        }

        public static Item FindEmptySlot(this Chest chest)
        {
            for (int i = 0; i < Chest.maxItems; i++)
            {
                if (chest.item[i].IsAir)
                    return chest.item[i];
            }
            return null;
        }
        public static Item FindItem(this Chest chest, Predicate<Item> search)
        {
            for (int i = 0; i < Chest.maxItems; i++)
            {
                if (!chest.item[i].IsAir && search(chest.item[i]))
                    return chest.item[i];
            }
            return null;
        }

        public static bool HasItem(this Chest chest, int item)
        {
            for (int i = 0; i < Chest.maxItems; i++)
            {
                if (!chest.item[i].IsAir && chest.item[i].type == item)
                    return true;
            }
            return false;
        }

        public static Vector2 MouseWorld(this Player player)
        {
            var mouseWorld = Main.MouseWorld;
            player.LimitPointToPlayerReachableArea(ref mouseWorld);
            return mouseWorld;
        }

        public static float FlipRotation(float rotation)
        {
            var v = (rotation).ToRotationVector2();
            v.Y = -v.Y;
            return v.ToRotation();
        }

        public static void ShootRotation(Projectile projectile, float rotation)
        {
            if (Main.player[projectile.owner].gravDir == -1)
            {
                rotation = FlipRotation(rotation - MathHelper.PiOver2) + MathHelper.PiOver2;
            }
            float angle = Math.Abs(rotation);
            int dir = Math.Sign(rotation);
            if (dir != Main.player[projectile.owner].direction)
            {
                Main.player[projectile.owner].direction = dir;
            }
            int frame = (angle <= 0.6f) ? 2 : ((angle >= (MathHelper.PiOver2 - 0.1f) && angle <= MathHelper.PiOver4 * 3f) ? 3 : ((!(angle > MathHelper.Pi * 3f / 4f)) ? 3 : 4));
            Main.player[projectile.owner].bodyFrame.Y = Main.player[projectile.owner].bodyFrame.Height * frame;
        }

        public static Rectangle WorldRectangle(this Rectangle rectangle)
        {
            rectangle.X *= 16;
            rectangle.Y *= 16;
            rectangle.Width *= 16;
            rectangle.Height *= 16;
            return rectangle;
        }

        public static Color HueMultiply(this Color color, float multiplier)
        {
            var hsl = Main.rgbToHsl(color);
            hsl.X *= multiplier;
            return Main.hslToRgb(hsl);
        }

        public static Color HueAdd(this Color color, float hueAdd)
        {
            var hsl = Main.rgbToHsl(color);
            hsl.X += hueAdd;
            hsl.X %= 1f;
            return Main.hslToRgb(hsl);
        }

        public static Color HueSet(this Color color, float hue)
        {
            var hsl = Main.rgbToHsl(color);
            hsl.X = hue;
            return Main.hslToRgb(hsl);
        }

        public static Color SaturationMultiply(this Color color, float saturation)
        {
            var hsl = Main.rgbToHsl(color);
            hsl.Y *= saturation;
            return Main.hslToRgb(hsl);
        }

        public static Color SaturationSet(this Color color, float saturation)
        {
            var hsl = Main.rgbToHsl(color);
            hsl.Y = saturation;
            return Main.hslToRgb(hsl);
        }

        public static Color[,] Get2DColorArr(this Texture2D texture, Rectangle frame)
        {
            var arr = Get1DColorArr(texture);
            var clrs = new Color[frame.Width, frame.Height];
            for (int i = 0; i < arr.Length; i++)
            {
                clrs[i % frame.Width, i / frame.Width] = arr[i];
            }
            return clrs;
        }

        public static Color[,] Get2DColorArr(this Texture2D texture)
        {
            return Get2DColorArr(texture, texture.Frame());
        }

        public static Color[] Get1DColorArr(this Texture2D texture, Rectangle frame)
        {
            int start = frame.X + frame.Y * frame.Width;
            var clrs = new Color[frame.Width * frame.Height - start];
            texture.GetData(clrs, start, clrs.Length);
            return clrs;
        }

        public static Color[] Get1DColorArr(this Texture2D texture)
        {
            return Get1DColorArr(texture, texture.Frame());
        }

        public static Color HueShift(Color color, float hueShift)
        {
            var hsl = Main.rgbToHsl(color);
            hsl.X += hueShift;
            return Main.hslToRgb(hsl);
        }

        public static int NPCType(Mod mod, string name)
        {
            if (NPCID.Search.TryGetId(mod.Name + "/" + name, out var value))
            {
                return value;
            }
            return 0;
        }

        public static bool PointCollision(Vector2 where, int width = 2, int height = 2)
        {
            return Collision.SolidCollision(where - new Vector2(width / 2f, height / 2f), width, height);
        }

        public static bool WearingSet(this Player player, int head, int body, int legs)
        {
            return player.head == head && player.body == body && player.legs == legs;
        }

        public static string CapSpaces(string text)
        {
            return Regex.Replace(text, "([A-Z])", " $1").Trim();
        }
        public static string ToStringNull(object value)
        {
            if (value == null)
                return "Null";
            return value.ToString();
        }

        public static bool IsSectionLoaded(int tileX, int tileY)
        {
            if (Main.netMode == NetmodeID.SinglePlayer || Main.sectionManager == null)
            {
                return true;
            }
            return Main.sectionManager.SectionLoaded(Netplay.GetSectionX(tileX), Netplay.GetSectionY(tileY));
        }

        public static bool IsSectionLoaded(Point p)
        {
            return IsSectionLoaded(p.X, p.Y);
        }

        public static T[] CloneArray<T>(this T[] array)
        {
            var array2 = new T[array.Length];
            array.CopyTo(array2, 0);
            return array2;
        }

        public static T2[] GetSpecific<T, T2>(this List<T> arr, Func<T, T2> get)
        {
            var arr2 = new T2[arr.Count];
            for (int i = 0; i < arr.Count; i++)
            {
                arr2[i] = get(arr[i]);
            }
            return arr2;
        }
        public static T2[] GetSpecific<T, T2>(this T[] arr, Func<T, T2> get)
        {
            var arr2 = new T2[arr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                arr2[i] = get(arr[i]);
            }
            return arr2;
        }

        public static int Mean(this List<int> arr)
        {
            int num = 0;
            for (int i = 0; i < arr.Count; i++)
            {
                num += arr[i];
            }
            return num / arr.Count;
        }
        public static int Mean(this byte[] arr)
        {
            int num = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                num += arr[i];
            }
            return num / arr.Length;
        }
        public static int Mean(this int[] arr)
        {
            int num = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                num += arr[i];
            }
            return num / arr.Length;
        }

        public static bool HasOwner(this Projectile projectile)
        {
            return projectile.owner > -1 && projectile.owner < Main.maxPlayers;
        }

        public static void UpdateCacheList<T>(T[] arr)
        {
            for (int i = arr.Length - 1; i > 0; i--)
            {
                arr[i] = arr[i - 1];
            }
        }

        public static Color GetRainbowColor(Projectile projectile, float index)
        {
            float laserLuminance = 0.5f;
            float laserAlphaMultiplier = 0f;
            float lastPrismHue = projectile.GetLastPrismHue(index % 6f, ref laserLuminance, ref laserAlphaMultiplier);
            float lastPrismHue2 = projectile.GetLastPrismHue((index + 1f) % 6f, ref laserLuminance, ref laserAlphaMultiplier);
            return Main.hslToRgb(MathHelper.Lerp(lastPrismHue, lastPrismHue2, index.Abs() % 1f), 1f, laserLuminance);
        }
        public static Color GetRainbowColor(int player, float position)
        {
            return GetRainbowColor(new Projectile() { owner = player }, position);
        }
        public static Color GetRainbowColor(Player player, float position)
        {
            return GetRainbowColor(player.whoAmI, position);
        }

        public static T TryFindChildElement<T>(UIElement element) where T : UIElement
        {
            foreach (var e in element.Children)
            {
                if (e is T value)
                {
                    return value;
                }
                var v = TryFindChildElement<T>(e);
                if (v != null)
                {
                    return v;
                }
            }
            return null;
        }

        public static int TryGetNPCNetID(this BestiaryEntry entry)
        {
            return (entry.Info[0] as NPCNetIdBestiaryInfoElement).NetId;
        }

        public static Color GetBrightestLight(Point tilePosition, int tilesSize)
        {
            var lighting = Color.Black;
            int realSize = tilesSize / 2;
            tilePosition.fluffize(10 + realSize);
            for (int i = tilePosition.X - realSize; i <= tilePosition.X + realSize; i++)
            {
                for (int j = tilePosition.Y - realSize; j <= tilePosition.Y + realSize; j++)
                {
                    var v = Lighting.GetColor(i, j);
                    lighting.R = Math.Max(v.R, lighting.R);
                    lighting.G = Math.Max(v.G, lighting.G);
                    lighting.B = Math.Max(v.B, lighting.B);
                }
            }
            return lighting;
        }
        /// <summary>
        /// Gets the mean of light surrounding a point
        /// </summary>
        /// <param name="x">The tile's X coordinate</param>
        /// <param name="y">The tile's Y coordinate</param>
        /// <param name="width">The width in tiles</param>
        /// <param name="height">The width in tiles</param>
        /// <returns></returns>
        public static Color GetLightingSection(int x, int y, int width, int height)
        {
            Vector3 lighting = Vector3.Zero;
            float amount = 0f;
            int largestSide = Math.Max(width, height);
            x = Math.Clamp(x, largestSide, Main.maxTilesX - largestSide);
            y = Math.Clamp(y, largestSide, Main.maxTilesY - largestSide);
            for (int i = x; i < x + width; i++)
            {
                for (int j = y; j < y + height; j++)
                {
                    lighting += Lighting.GetColor(i, j).ToVector3();
                    amount++;
                }
            }
            if (amount == 0f)
                return Color.White;
            return new Color(lighting / amount);
        }
        /// <summary>
        /// Gets the mean of light surrounding a point
        /// </summary>
        /// <param name="tilePosition">The tile center</param>
        /// <param name="width">The width in tiles</param>
        /// <param name="height">The width in tiles</param>
        /// <returns></returns>
        public static Color GetLightingSection(Point tilePosition, int width, int height)
        {
            return GetLightingSection(tilePosition.X - width, tilePosition.Y, width, height);
        }
        /// <summary>
        /// Gets the mean of light surrounding a point
        /// </summary>
        /// <param name="tilePosition">The tile center</param>
        /// <param name="tilesSize">The size in tile coordinates</param>
        /// <returns></returns>
        public static Color GetLightingSection(Point tilePosition, int tilesSize = 10)
        {
            Vector3 lighting = Vector3.Zero;
            float amount = 0f;
            int realSize = tilesSize / 2;
            tilePosition.fluffize(10 + realSize);
            for (int i = tilePosition.X - realSize; i <= tilePosition.X + realSize; i++)
            {
                for (int j = tilePosition.Y - realSize; j <= tilePosition.Y + realSize; j++)
                {
                    lighting += Lighting.GetColor(i, j).ToVector3();
                    amount++;
                }
            }
            if (amount == 0f)
                return Color.White;
            return new Color(lighting / amount);
        }
        /// <summary>
        /// Gets the mean of light surrounding a point
        /// </summary>
        /// <param name="x">The center tile X</param>
        /// <param name="y">The center tile Y</param>
        /// <param name="tilesSize">The size in tile coordinates</param>
        /// <returns></returns>
        public static Color GetLightingSection(int x, int y, int tilesSize = 10)
        {
            return GetLightingSection(new Point(x, y), tilesSize);
        }
        /// <summary>
        /// Gets the mean of light surrounding a point
        /// </summary>
        /// <param name="worldPosition">The center</param>
        /// <param name="tilesSize">The size in tile coordinates</param>
        /// <returns></returns>
        public static Color GetLightingSection(Vector2 worldPosition, int tilesSize = 10)
        {
            return GetLightingSection(worldPosition.ToTileCoordinates(), tilesSize);
        }

        public static Color GetColor(Vector2 v, Color color)
        {
            return Lighting.GetColor((int)(v.X / 16), (int)(v.Y / 16f), color);
        }

        public static Color GetColor(Vector2 v)
        {
            return Lighting.GetColor((int)(v.X / 16), (int)(v.Y / 16f));
        }

        public static bool HereditarySource(IEntitySource source, out Entity entity)
        {
            entity = null;
            if (source == null)
            {
                return false;
            }
            if (source is EntitySource_DropAsItem dropAsItem)
            {
                entity = dropAsItem.Entity;
                return true;
            }
            if (source is EntitySource_Buff buff)
            {
                entity = buff.Entity;
                return true;
            }
            if (source is EntitySource_OnHit onHit)
            {
                entity = onHit.EntityStruck;
                return true;
            }
            if (source is EntitySource_Parent parent)
            {
                entity = parent.Entity;
                return true;
            }
            if (source is EntitySource_HitEffect hitEffect)
            {
                entity = hitEffect.Entity;
                return true;
            }
            if (source is EntitySource_Death death)
            {
                entity = death.Entity;
                return true;
            }
            if (source is EntitySource_Loot loot)
            {
                entity = loot.Entity;
                return true;
            }
            return false;
        }

        public static string ReplaceTextWithStringArgs(ref string text, string needsToStartWith, string key, Func<string[], object> turnStringArgsIntoObject)
        {
            int i = text.IndexOf(needsToStartWith);
            if (i > -1)
            {
                string val = text[i..].Split('+')[0];
                ReplaceText(ref text, $"{val}+",
                    Language.GetTextValueWith(key, turnStringArgsIntoObject(val.Replace('+', ' ').Split('|'))));
            }
            return text;
        }
        public static string ReplaceText(ref string text, string oldText, string newText)
        {
            text = text.Replace(oldText, newText);
            return text;
        }

        public static string FormatWith(this string text, object obj)
        {
            string input = text;
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(obj);
            return SubstitutionRegex.Replace(input, delegate (Match match)
            {
                if (match.Groups[1].Length != 0)
                {
                    return "";
                }

                string name = match.Groups[2].ToString();
                PropertyDescriptor propertyDescriptor = properties.Find(name, ignoreCase: false);
                return (propertyDescriptor != null) ? (propertyDescriptor.GetValue(obj) ?? "")!.ToString() : "";
            });
        }

        public static void GetMinionLeadership(this Projectile projectile, out int leader, out int minionPos, out int count)
        {
            leader = -1;
            minionPos = 0;
            count = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == projectile.owner && Main.projectile[i].type == projectile.type)
                {
                    if (leader == -1)
                    {
                        leader = i;
                    }
                    if (i == projectile.whoAmI)
                    {
                        minionPos = count;
                    }
                    count++;
                }
            }
        }

        public static int GetMinionTarget(this Projectile projectile, Vector2 position, out float distance, float maxDistance = 2000f, float? ignoreTilesDistance = 0f)
        {
            if (Main.player[projectile.owner].HasMinionAttackTargetNPC)
            {
                distance = Vector2.Distance(Main.npc[Main.player[projectile.owner].MinionAttackTargetNPC].Center, projectile.Center);
                if (distance < maxDistance)
                {
                    return Main.player[projectile.owner].MinionAttackTargetNPC;
                }
            }
            int target = -1;
            distance = maxDistance;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].CanBeChasedBy(projectile))
                {
                    float d = Vector2.Distance(position, Main.npc[i].Center).UnNaN();
                    if (d < distance)
                    {
                        if (!ignoreTilesDistance.HasValue || d < ignoreTilesDistance || Collision.CanHit(position - projectile.Size / 2f, projectile.width, projectile.height, Main.npc[i].position, Main.npc[i].width, Main.npc[i].height))
                        {
                            distance = d;
                            target = i;
                        }
                    }
                }
            }
            return target;
        }
        public static int GetMinionTarget(this Projectile projectile, out float distance, float maxDistance = 2000f, float? ignoreTilesDistance = 0f)
        {
            return GetMinionTarget(projectile, projectile.Center, out distance, maxDistance, ignoreTilesDistance);
        }

        public static void DefaultToExplosion(this Projectile projectile, int size, DamageClass damageClass, int timeLeft = 2)
        {
            projectile.width = size;
            projectile.height = size;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.DamageType = damageClass;
            projectile.aiStyle = -1;
            projectile.timeLeft = timeLeft;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = projectile.timeLeft + 1;
            projectile.penetrate = -1;
        }

        public static void CollideWithOthers(this Projectile projectile, float speed = 0.05f)
        {
            var rect = projectile.getRect();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && i != projectile.whoAmI && projectile.type == Main.projectile[i].type && projectile.owner == Main.projectile[i].owner
                    && projectile.Colliding(rect, Main.projectile[i].getRect()))
                {
                    projectile.velocity += Main.projectile[i].DirectionTo(projectile.Center).UnNaN() * speed;
                }
            }
        }

        public static Player GetHereditaryOwnerPlayer(Entity entity)
        {
            if (entity is Player player)
            {
                return player;
            }
            else if (entity is Projectile projectile && projectile.TryGetGlobalProjectile<SentryAccessoriesManager>(out var Sentry6502))
            {
                return Sentry6502.dummyPlayer;
            }
            return null;
        }

        public static Entity GetHereditaryOwner(this Projectile projectile)
        {
            int projIdentity = (int)projectile.ai[0] - 1;

            if (projIdentity > -1)
            {
                projIdentity = FindProjectileIdentity(projectile.owner, projIdentity);
                if (projIdentity == -1 || !Main.projectile[projIdentity].active || !Main.projectile[projIdentity].TryGetGlobalProjectile<SentryAccessoriesManager>(out var value))
                {
                    if (Main.myPlayer == projectile.owner)
                    {
                        projectile.Kill();
                        projectile.netUpdate = true;
                    }
                }
                else
                {
                    return Main.projectile[projIdentity];
                }
            }
            return Main.player[projectile.owner];
        }

        public static void Transform(this Projectile proj, int newType)
        {
            int damage = proj.damage;
            float kb = proj.knockBack;
            int owner = proj.owner;
            var center = proj.Center;
            var velo = proj.velocity;

            proj.SetDefaults(newType);

            proj.damage = damage;
            proj.knockBack = kb;
            proj.owner = owner;
            proj.Center = center;
            proj.velocity = velo;
        }

        public static SpriteEffects GetSpriteEffect(this Projectile projectile)
        {
            return (-projectile.spriteDirection).ToSpriteEffect();
        }

        public static SpriteEffects ToSpriteEffect(this int value)
        {
            return value == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        }

        public static float UnNaN(this float value)
        {
            return float.IsNaN(value) ? 0f : value;
        }
        public static Vector2 UnNaN(this Vector2 value)
        {
            return new Vector2(UnNaN(value.X), UnNaN(value.Y));
        }

        public static Recipe TryRegisterAfter(this Recipe rec, int itemID)
        {
            if (!HasRecipe(itemID))
            {
                rec.Register();
                return rec;
            }
            try
            {
                rec.Register();
                rec.SortAfterFirstRecipesOf(itemID);
            }
            catch
            {
            }
            return rec;
        }
        public static Recipe TryRegisterBefore(this Recipe rec, int itemID)
        {
            if (!HasRecipe(itemID))
            {
                rec.Register();
                return rec;
            }
            try
            {
                rec.Register();
                rec.SortBeforeFirstRecipesOf(itemID);
            }
            catch
            {
            }
            return rec;
        }
        public static Recipe UnsafeSortRegister(this Recipe rec, Action<Recipe> postRegisterCauseStableDoesntHaveRegisterReturnRecipeGuh)
        {
            rec.Register();
            postRegisterCauseStableDoesntHaveRegisterReturnRecipeGuh(rec);
            return rec;
        }

        public static bool HasRecipe(int item)
        {
            foreach (var r in Main.recipe)
            {
                if (r?.createItem?.type == item)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool CheckHeredity(this Projectile projectile, AequusProjectile sources, Projectile projectile2)
        {
            return projectile2.active && projectile2.owner == projectile.owner && projectile.type == projectile2.type && sources.sourceProjIdentity == projectile2.GetGlobalProjectile<AequusProjectile>().sourceProjIdentity;
        }
        public static bool CheckHeredity(this Projectile projectile, Projectile projectile2)
        {
            return CheckHeredity(projectile, projectile.GetGlobalProjectile<AequusProjectile>(), projectile2);
        }

        public static float Opacity(this Dust dust)
        {
            return 1f - dust.alpha / 255f;
        }

        public static float Angle(Vector2 me, Vector2 to)
        {
            return (to - me).ToRotation();
        }

        public static Vector2 To(Vector2 me, Vector2 to, float speed)
        {
            return Vector2.Normalize(me - to) * speed;
        }

        public static Rectangle Fluffize(this Rectangle rect, int padding = 10)
        {
            if (rect.X < 10)
            {
                rect.X = 10;
            }
            else if (rect.X + rect.Width > Main.maxTilesX - padding)
            {
                rect.X = Main.maxTilesX - rect.Width - padding;
            }

            if (rect.Y < 10)
            {
                rect.Y = 10;
            }
            else if (rect.Y + rect.Height > Main.maxTilesX - padding)
            {
                rect.Y = Main.maxTilesX - rect.Height - padding;
            }
            return rect;
        }

        public static void AddList<TKey, TValue>(this Dictionary<TKey, List<TValue>> dictionary, TKey key, TValue value)
        {
            if (dictionary.TryGetValue(key, out var list))
            {
                if (list == null)
                {
                    dictionary[key] = new List<TValue>() { value };
                    return;
                }
                list.Add(value);
                return;
            }
            dictionary[key] = new List<TValue>() { value };
        }
        public static List<TValue> ToList<TKey, TValue>(this Dictionary<TKey, TValue>.ValueCollection keys)
        {
            var l = new List<TValue>();
            foreach (var k in keys)
            {
                l.Add(k);
            }
            return l;
        }

        public static List<TKey> ToList<TKey, TValue>(this Dictionary<TKey, TValue>.KeyCollection keys)
        {
            var l = new List<TKey>();
            foreach (var k in keys)
            {
                l.Add(k);
            }
            return l;
        }

        /// <summary>
        /// Drops a specified amount of money at the location provided
        /// </summary>
        /// <param name="source">Entity source for item spawn.</param>
        /// <param name="rect">Area the money can drop.</param>
        /// <param name="amt">Amount of money to drop</param>
        /// <param name="quiet">Whether or not the coin items should be synced in Multiplayer.</param>
        public static void DropMoney(IEntitySource source, Rectangle rect, long amt, bool quiet = true)
        {
            int[] coins = Utils.CoinsSplit(amt);
            for (int i = 0; i < coins.Length; i++)
            {
                if (coins[i] <= 0)
                {
                    continue;
                }

                int itemType = ItemID.CopperCoin + i;
                var referenceItem = new Item(itemType);
                do
                {
                    int amount = Math.Min(coins[i], referenceItem.maxStack);
                    int item = Item.NewItem(source, rect, itemType, amount);
                    if (!quiet && Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        NetMessage.SendData(MessageID.SyncItem, -1, -1, null, item, 1f);
                    }
                    coins[i] -= referenceItem.maxStack * referenceItem.value;
                } 
                while (coins[i] > 0);
            }
        }

        public static bool TryGetFieldOf<T>(this Type type, string name, BindingFlags flags, object obj, out T value)
        {
            value = default(T);
            var f = type.GetField(name, flags);
            if (f == null || f.FieldType != typeof(T))
                return false;
            value = (T)f.GetValue(obj);
            return true;
        }

        public static MiscShaderData UseImage1(this MiscShaderData misc, Asset<Texture2D> texture)
        {
            typeof(MiscShaderData).GetField("_uImage1", LetMeIn).SetValue(misc, texture);
            return misc;
        }

        public static IEnumerable<(T attr, MemberInfo info)> GetFieldsPropertiesOfAttribute<T>(Type t) where T : Attribute
        {
            var l = new List<(T, MemberInfo)>();
            foreach (var f in t.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
            {
                var attr = f.GetCustomAttribute<T>();
                if (attr != null)
                {
                    l.Add((attr, f));
                }
            }
            foreach (var p in t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
            {
                var attr = p.GetCustomAttribute<T>();
                if (attr != null)
                {
                    l.Add((attr, p));
                }
            }
            return l;
        }

        public static IEntitySource GetSource_TileBreak(this ModTile modTile, int i, int j, string context = null)
        {
            return new EntitySource_TileBreak(i, j, context);
        }

        public static Vector2 NextFromRect(this UnifiedRandom rand, Rectangle rectangle)
        {
            return rectangle.TopLeft() + new Vector2(rand.NextFloat(rectangle.Width), rand.NextFloat(rectangle.Height));
        }

        public static Vector2 NextCircularFromRect(this UnifiedRandom rand, Rectangle rectangle)
        {
            return rectangle.Center.ToVector2() + rand.NextVector2Unit() * new Vector2(rand.NextFloat(rectangle.Width / 2f), rand.NextFloat(rectangle.Height / 2f));
        }

        public static Point Fluffize(this Point point, int fluff = 10)
        {
            point.fluffize(fluff);
            return point;
        }

        public static void fluffize(this ref Point point, int fluff = 10)
        {
            if (point.X < fluff)
            {
                point.X = fluff;
            }
            else if (point.X > Main.maxTilesX - fluff)
            {
                point.X = Main.maxTilesX - fluff;
            }
            if (point.Y < fluff)
            {
                point.Y = fluff;
            }
            else if (point.Y > Main.maxTilesY - fluff)
            {
                point.Y = Main.maxTilesY - fluff;
            }
        }

        public static void Set<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (dict.ContainsKey(key))
            {
                dict[key] = value;
                return;
            }
            dict.Add(key, value);
        }

        /// <summary>
        /// Attempts to find a projectile index using the identity and owner provided. Returns -1 otherwise.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static int FindProjectileIdentity(int owner, int identity)
        {
            for (int i = 0; i < 1000; i++)
            {
                if (Main.projectile[i].owner == owner && Main.projectile[i].identity == identity && Main.projectile[i].active)
                {
                    return i;
                }
            }
            return -1;
        }
        public static int FindProjectileIdentity_OtherwiseFindPotentialSlot(int owner, int identity)
        {
            int projectile = FindProjectileIdentity(owner, identity);
            if (projectile == -1)
            {
                for (int i = 0; i < 1000; i++)
                {
                    if (!Main.projectile[i].active)
                    {
                        projectile = i;
                        break;
                    }
                }
            }
            if (projectile == 1000)
            {
                projectile = Projectile.FindOldestProjectile();
            }
            return projectile;
        }

        public static T GetOrDefault<T>(this Dictionary<int, T> dict, int index, T Default = default(T))
        {
            if (dict.TryGetValue(index, out T value))
            {
                return value;
            }
            return Default;
        }

        public static List<int> AllWhichShareBanner(int type, bool vanillaOnly = false)
        {
            var list = new List<int>();
            int banner = ContentSamples.NpcsByNetId[type].ToBanner();
            if (banner == 0)
            {
                return list;
            }
            foreach (var n in ContentSamples.NpcsByNetId)
            {
                if (vanillaOnly && n.Key > NPCID.Count)
                {
                    continue;
                }
                if (banner == n.Value.ToBanner())
                {
                    list.Add(n.Key);
                }
            }
            return list;
        }

        public static void Max(this (int, int) tuple, int value)
        {
            tuple.Item1 = Math.Max(tuple.Item1, value);
            tuple.Item2 = Math.Max(tuple.Item2, value);
        }

        public static byte TickDown(ref byte value, byte tickAmt = 1)
        {
            if (value > 0)
            {
                if (value - tickAmt < 0)
                {
                    value = 0;
                    return 0;
                }
                value -= tickAmt;
            }
            return value;
        }
        public static ushort TickDown(ref ushort value, ushort tickAmt = 1)
        {
            if (value > 0)
            {
                if (value - tickAmt < 0)
                {
                    value = 0;
                    return 0;
                }
                value -= tickAmt;
            }
            return value;
        }
        public static int TickDown(ref int value, uint tickAmt = 1)
        {
            if (value > 0)
            {
                value -= (int)tickAmt;
                if (value < 0)
                {
                    value = 0;
                }
            }
            return value;
        }

        public static Rectangle Frame(this Projectile projectile)
        {
            return TextureAssets.Projectile[projectile.type].Value.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
        }

        public static int CalcHealing(int life, int maxLife, int heal)
        {
            return life + heal > maxLife ? maxLife - life : heal;
        }

        public static float CalcProgress(int length, int i)
        {
            return 1f - 1f / length * i;
        }

        public static int TrailLength(this Projectile projectile)
        {
            return ProjectileID.Sets.TrailCacheLength[projectile.type];
        }

        public static void LoopingFrame(this Projectile projectile, int ticksPerFrame)
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > ticksPerFrame)
            {
                projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
                projectile.frameCounter = 0;
            }
        }

        public static void WriteText(this Stream stream, string text, Encoding encoding = null, int tabs = 0)
        {
            encoding ??= Encoding.ASCII;
            if (tabs > 0)
            {
                string tabVal = "";
                for (int i = 0; i < tabs; i++)
                    tabVal += "	";

                if (text.EndsWith('\n'))
                {
                    text = $"{text[0..^1].Replace("\n", $"\n{tabVal}")}\n";
                }
                else
                {
                    text = text.Replace("\n", $"\n{tabVal}");
                }
                text = $"{tabVal}{text}";
            }
            var val = encoding.GetBytes(text);
            stream.Write(val, 0, val.Length);
        }

        public static T GetValue<T>(this PropertyInfo property, object obj)
        {
            return (T)property.GetValue(obj);
        }
        public static T GetValue<T>(this FieldInfo field, object obj)
        {
            return (T)field.GetValue(obj);
        }
        public static T ReflectiveCloneTo<T>(this T obj, T obj2)
        {
            return ReflectiveCloneTo(obj, obj2, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }
        public static T ReflectiveCloneTo<T>(this T obj, T obj2, BindingFlags flags)
        {
            var t = typeof(T);
            foreach (var f in t.GetFields(flags))
            {
                if (!f.IsInitOnly)
                {
                    f.SetValue(obj2, f.GetValue(obj));
                }
            }
            foreach (var p in t.GetProperties(flags))
            {
                if (p.CanWrite)
                {
                    p.SetValue(obj2, p.GetValue(obj));
                }
            }
            return obj2;
        }

        public static void DropHearts(IEntitySource source, Rectangle hitbox, int guaranteedAmount, int randomAmount) {
            for (int i = 0; i < guaranteedAmount; i++) {
                Item.NewItem(source, hitbox, ItemID.Heart);
            }
            randomAmount = Main.rand.Next(randomAmount);
            for (int i = 0; i < randomAmount; i++) {
                Item.NewItem(source, hitbox, ItemID.Heart);
            }
        }
        public static bool IsManaPickup(this Item item) {
            return item.type switch {
                ItemID.Star => true,
                ItemID.SoulCake => true,
                ItemID.SugarPlum => true,
                _ => false
            };
        }
        public static bool IsHeartPickup(this Item item) {
            return item.type switch {
                ItemID.Heart => true,
                ItemID.CandyApple => true,
                ItemID.CandyCane => true,
                _ => false
            };
        }

        public static AequusItem AequusTooltips(this Item item)
        {
            return item.GetGlobalItem<AequusItem>();
        }
        public static AequusItem Aequus(this Item item)
        {
            return item.GetGlobalItem<AequusItem>();
        }
        public static AequusProjectile Aequus(this Projectile projectile)
        {
            return projectile.GetGlobalProjectile<AequusProjectile>();
        }
        public static AequusPlayer Aequus(this Player player)
        {
            return player.GetModPlayer<AequusPlayer>();
        }

        public static string GenderString(this Player player)
        {
            return player.Male ? "Male" : "Female";
        }
        public static void AddLifeRegen(this Player player, int regen)
        {
            if (regen < 0)
            {
                if (player.lifeRegen > 0)
                {
                    player.lifeRegen = 0;
                    player.lifeRegenTime = 0;
                }
                player.lifeRegen += regen;
                return;
            }
            bool badRegen = player.lifeRegen < 0;
            player.lifeRegen += regen;
            if (badRegen && player.lifeRegen > 0)
            {
                player.lifeRegen = 0;
            }
        }

        public static bool IsRectangleCollidingWithCircle(Vector2 circle, float circleRadius, Rectangle rectangle)
        {
            return Vector2.Distance(circle, rectangle.Center.ToVector2() + Vector2.Normalize(circle - rectangle.Center.ToVector2()) * rectangle.Size() / 2f) < circleRadius;
        }

        public static bool DeathrayHitbox(Vector2 center, Rectangle targetHitbox, float rotation, float length, float size, float startLength = 0f)
        {
            return DeathrayHitbox(center, targetHitbox, rotation.ToRotationVector2(), length, size, startLength);
        }
        public static bool DeathrayHitbox(Vector2 center, Rectangle targetHitbox, Vector2 normal, float length, float size, float startLength = 0f)
        {
            return DeathrayHitbox(center + normal * startLength, center + normal * startLength + normal * length, targetHitbox, size);
        }
        public static bool DeathrayHitbox(Vector2 from, Vector2 to, Rectangle targetHitbox, float size)
        {
            float _ = float.NaN;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), from, to, size, ref _);
        }

        public static bool IsThisTileOrIsACraftingStationOfThisTile(int craftingStationTile, int comparisonTile)
        {
            if (craftingStationTile == comparisonTile)
            {
                return true;
            }
            if (comparisonTile > TileID.Count)
            {
                var adjTiles = TileLoader.GetTile(comparisonTile).AdjTiles;
                if (adjTiles != null && adjTiles.ContainsAny(craftingStationTile))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsTheSameAs<T>(this List<T> list1, List<T> list2)
        {
            if (list1.Count != list2.Count)
                return false;
            for (int i = 0; i < list1.Count; i++)
            {
                if (!list1[i].Equals(list2[i]))
                    return false;
            }
            return true;
        }

        public static bool ContainsAll<T>(this IEnumerable<T> en, IEnumerable<T> en2)
        {
            foreach (var item in en2)
            {
                if (!en.ContainsAny(item))
                    return false;
            }
            return true;
        }

        public static bool ContainsAny<T>(this IEnumerable<T> en, Func<T, bool> search)
        {
            foreach (var t in en)
            {
                if (search(t))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool ContainsAny<T>(this IEnumerable<T> en, T en2)
        {
            return ContainsAny(en, (t) => t.Equals(en2));
        }
        public static bool ContainsAny<T>(this IEnumerable<T> en, params T[] en2)
        {
            return ContainsAny(en, (t) =>
            {
                foreach (var t2 in en2)
                {
                    if (t.Equals(t2))
                    {
                        return true;
                    }
                }
                return false;
            });
        }
        public static bool ContainsAny<T>(this IEnumerable<T> en, IEnumerable<T> en2)
        {
            return ContainsAny(en, (t) =>
            {
                foreach (var t2 in en2)
                {
                    if (t.Equals(t2))
                    {
                        return true;
                    }
                }
                return false;
            });
        }

        public static SoundStyle WithVolume(this SoundStyle soundStyle, float volume)
        {
            var value = soundStyle;
            value.Volume = volume;
            return value;
        }

        public static SoundStyle WithPitch(this SoundStyle soundStyle, float pitch)
        {
            var value = soundStyle;
            value.PitchRange = (pitch, pitch);
            return value;
        }

        public static void SetTrail(this ModProjectile modProjectile, int length = -1)
        {
            if (length > 0)
            {
                ProjectileID.Sets.TrailCacheLength[modProjectile.Type] = length;
            }
            ProjectileID.Sets.TrailingMode[modProjectile.Type] = 2;
        }

        public static bool NoDungeonOrTempleWall(this Tile tile)
        {
            return Main.wallDungeon[tile.WallType] && tile.WallType != WallID.LihzahrdBrickUnsafe;
        }
        public static bool IsConvertibleProbably(this Tile tile)
        {
            return TileID.Sets.Conversion.Grass[tile.TileType]
                    || TileID.Sets.Conversion.Stone[tile.TileType] || TileID.Sets.Conversion.Sand[tile.TileType]
                    || TileID.Sets.Conversion.Ice[tile.TileType];
        }
        public static void Slope(this Tile tile, byte value)
        {
            tile.Slope = (SlopeType)value;
        }
        public static void HalfBrick(this Tile tile, bool value)
        {
            tile.IsHalfBlock = value;
        }
        public static void Actuated(this Tile tile, bool value)
        {
            tile.IsActuated = value;
        }
        public static void Active(this Tile tile, bool value)
        {
            tile.HasTile = value;
        }

        public static int FindFirstPlayerWithin(Rectangle rectangle)
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Main.player[i].active && !Main.player[i].dead && rectangle.Intersects(Main.player[i].getRect()))
                {
                    return i;
                }
            }
            return -1;
        }

        public static bool ShouldDoEffects(Vector2 location)
        {
            return Main.netMode != NetmodeID.Server && (new Vector2(Main.screenPosition.X + Main.screenWidth / 2f, Main.screenPosition.Y + Main.screenHeight / 2f) - location).Length() < 1000f;
        }

        public static float CappedMeleeScale(this Player player)
        {
            var item = player.HeldItem;
            return Math.Clamp(player.GetAdjustedItemScale(item), 0.5f * item.scale, 2f * item.scale);
        }

        public static void CappedMeleeScale(Projectile proj)
        {
            float scale = Main.player[proj.owner].CappedMeleeScale();
            if (scale != 1f)
            {
                proj.scale *= scale;
                proj.width = (int)(proj.width * proj.scale);
                proj.height = (int)(proj.height * proj.scale);
            }
        }

        public static Color MaxRGBA(this Color color, byte amt)
        {
            return color.MaxRGBA(amt, amt);
        }
        public static Color MaxRGBA(this Color color, byte amt, byte a)
        {
            return color.MaxRGBA(amt, amt, amt, a);
        }
        public static Color MaxRGBA(this Color color, byte r, byte g, byte b, byte a)
        {
            color.R = Math.Max(color.R, r);
            color.G = Math.Max(color.G, g);
            color.B = Math.Max(color.B, b);
            color.A = Math.Max(color.A, a);
            return color;
        }

        public static Vector2[] LinearInterpolationBetween(Vector2 start, Vector2 end, int length)
        {
            var diff = (end - start) / length;
            var arr = new Vector2[length];
            arr[0] = start;
            arr[^1] = end;
            for (int i = 1; i < arr.Length - 1; i++)
            {
                arr[i] = start + diff * i;
            }
            return arr;
        }
        public static Vector2[] CircularVector(int amt, float angleAddition = 0f)
        {
            return Array.ConvertAll(Circular(amt, angleAddition), (f) => f.ToRotationVector2());
        }
        public static float[] Circular(int amt, float angleAddition = 0f)
        {
            var v = new float[amt];
            float f = MathHelper.TwoPi / amt;
            for (int i = 0; i < amt; i++)
            {
                v[i] = (f * i + angleAddition) % MathHelper.TwoPi;
            }
            return v;
        }

        public static T ModItem<T>(this Item item) where T : ModItem
        {
            return (T)item.ModItem;
        }
        public static T ModProjectile<T>(this Projectile projectile) where T : ModProjectile
        {
            return (T)projectile.ModProjectile;
        }

        public static void ScreenFlip(Vector2[] value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                value[i] = ScreenFlip(value[i]);
            }
        }
        public static Vector2 ScreenFlip(Vector2 value)
        {
            return new Vector2(value.X, ScreenFlip(value.Y));
        }
        public static float ScreenFlip(float value)
        {
            return -value + Main.screenHeight;
        }

        public static Color LerpBetween(Color[] colors, float amount)
        {
            if (amount < 0f)
            {
                amount %= colors.Length;
                amount = colors.Length - amount;
            }
            int index = (int)amount;
            return Color.Lerp(colors[index % colors.Length], colors[(index + 1) % colors.Length], amount % 1f);
        }

        public static int TimedBasedOn(int timer, int ticksPer, int loop)
        {
            timer %= ticksPer * loop;
            return timer / ticksPer;
        }

        public static Item DefaultItem(int type)
        {
            var item = new Item();
            item.SetDefaults(type);
            return item;
        }

        public static bool Insert(this Chest chest, int itemType, int itemStack, int index)
        {
            var item = DefaultItem(itemType);
            item.stack = itemStack;
            return InsertIntoUnresizableArray(chest.item, item, index);
        }
        public static bool Insert(this Chest chest, int itemType, int index)
        {
            return chest.Insert(itemType, 1, index);
        }
        public static bool InsertIntoUnresizableArray<T>(T[] arr, T value, int index)
        {
            if (index >= arr.Length)
            {
                return false;
            }
            for (int j = arr.Length - 1; j > index; j--)
            {
                arr[j] = arr[j - 1];
            }
            arr[index] = value;
            return true;
        }

        public static bool UpdateProjActive(Projectile projectile, int buff)
        {
            if (!Main.player[projectile.owner].active || Main.player[projectile.owner].dead)
            {
                Main.player[projectile.owner].ClearBuff(buff);
                return false;
            }
            if (Main.player[projectile.owner].HasBuff(buff))
            {
                projectile.timeLeft = 2;
            }
            return true;
        }
        public static bool UpdateProjActive<T>(Projectile projectile) where T : ModBuff
        {
            return UpdateProjActive(projectile, ModContent.BuffType<T>());
        }

        public static bool UpdateProjActive(Projectile projectile, ref bool active)
        {
            if (Main.player[projectile.owner].dead)
                active = false;
            if (active)
                projectile.timeLeft = 2;
            return active;
        }

        [Obsolete("Use ModItem.SacrificeTotal instead.")]
        public static void SetResearch(this ModItem modItem, int amt)
        {
            SetResearch(modItem.Type, amt);
        }
        [Obsolete("Use ModItem.SacrificeTotal instead.")]
        public static void SetResearch(int type, int amt)
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[type] = amt;
        }

        public static int FindTarget(Vector2 position, int width = 2, int height = 2, float maxRange = 800f, object me = null, Func<int, bool> validCheck = null)
        {
            float num = maxRange;
            int result = -1;
            var center = position + new Vector2(width / 2f, height / 2f);
            for (int i = 0; i < 200; i++)
            {
                NPC nPC = Main.npc[i];
                if (nPC.CanBeChasedBy(me) && (validCheck == null || validCheck.Invoke(i)))
                {
                    float num2 = Vector2.Distance(center, Main.npc[i].Center);
                    if (num2 < num)
                    {
                        num = num2;
                        result = i;
                    }
                }
            }
            return result;
        }

        public static int FindTargetWithLineOfSight(Vector2 position, int width = 2, int height = 2, float maxRange = 800f, object me = null, Func<int, bool> validCheck = null)
        {
            float num = maxRange;
            int result = -1;
            var center = position + new Vector2(width / 2f, height / 2f);
            for (int i = 0; i < 200; i++)
            {
                NPC nPC = Main.npc[i];
                if (nPC.CanBeChasedBy(me) && (validCheck == null || validCheck.Invoke(i)))
                {
                    float num2 = Vector2.Distance(center, Main.npc[i].Center);
                    if (num2 < num && Collision.CanHit(position, width, height, nPC.position, nPC.width, nPC.height))
                    {
                        num = num2;
                        result = i;
                    }
                }
            }
            return result;
        }

        public static int RollHigherFromLuck(this Player player, int amt)
        {
            return amt - player.RollLuck(amt);
        }

        public static Color UseR(this Color color, int R) => new Color(R, color.G, color.B, color.A);
        public static Color UseR(this Color color, float R) => new Color((int)(R * 255), color.G, color.B, color.A);

        public static Color UseG(this Color color, int G) => new Color(color.R, G, color.B, color.A);
        public static Color UseG(this Color color, float G) => new Color(color.R, (int)(G * 255), color.B, color.A);

        public static Color UseB(this Color color, int B) => new Color(color.R, color.G, B, color.A);
        public static Color UseB(this Color color, float B) => new Color(color.R, color.G, (int)(B * 255), color.A);

        public static Color UseA(this Color color, int alpha) => new Color(color.R, color.G, color.B, alpha);
        public static Color UseA(this Color color, float alpha) => new Color(color.R, color.G, color.B, (int)(alpha * 255));

        public static float FromByte(byte value, float maximum)
        {
            return value * maximum / 255f;
        }
        public static float FromByte(byte value, float minimum, float maximum)
        {
            return minimum + value * (maximum - minimum) / 255f;
        }

        public static bool CloseEnough(this float comparison, float intendedValue, float closeEnoughMargin = 1f)
        {
            return (comparison - intendedValue).Abs() <= closeEnoughMargin;
        }

        public static float WaveCos(float time, float minimum, float maximum)
        {
            return minimum + ((float)Math.Cos(time) + 1f) / 2f * (maximum - minimum);
        }

        public static float Wave(float time, float minimum, float maximum)
        {
            return minimum + ((float)Math.Sin(time) + 1f) / 2f * (maximum - minimum);
        }

        public static bool SolidTopType(this Tile tile)
        {
            return Main.tileSolidTop[tile.TileType];
        }

        public static bool IsIncludedIn(this TileDataCache tile, int[] arr)
        {
            return arr.ContainsAny(tile.TileType);
        }

        public static bool IsIncludedIn(this Tile tile, int[] arr)
        {
            return arr.ContainsAny(tile.TileType);
        }

        public static bool SolidType(this Tile tile)
        {
            return Main.tileSolid[tile.TileType];
        }

        public static bool IsSolid(this Tile tile)
        {
            return tile.HasTile && SolidType(tile) && !tile.IsActuated;
        }

        public static bool IsFullySolid(this Tile tile)
        {
            return IsSolid(tile) && !SolidTopType(tile);
        }

        public static int Abs(this int value)
        {
            return value < 0 ? -value : value;
        }
        public static float Abs(this float value)
        {
            return value < 0f ? -value : value;
        }

        public static string NamespacePath(this object obj)
        {
            return NamespacePath(obj.GetType());
        }
        public static string NamespacePath<T>()
        {
            return NamespacePath(typeof(T));
        }
        public static string NamespacePath(Type t)
        {
            return t.Namespace.Replace('.', '/');
        }
        public static string GetPath(this object obj)
        {
            return GetPath(obj.GetType());
        }
        public static string GetPath<T>()
        {
            return GetPath(typeof(T));
        }
        public static string GetPath(Type t)
        {
            return $"{NamespacePath(t)}/{t.Name}";
        }

        public class Loader : IOnModLoad
        {
            void ILoadable.Load(Mod mod)
            {
                iterations = 0;
            }

            void IOnModLoad.OnModLoad(Aequus aequus)
            {
                SubstitutionRegex = new Regex("{(\\?(?:!)?)?([a-zA-Z][\\w\\.]*)}", RegexOptions.Compiled);
                UnboxInt = new UnboxInt();
                UnboxFloat = new UnboxFloat();
                UnboxBoolean = new UnboxBoolean();
            }

            void ILoadable.Unload()
            {
                SubstitutionRegex = null;
                UnboxInt = null;
                UnboxFloat = null;
                UnboxBoolean = null;
            }
        }
    }
}