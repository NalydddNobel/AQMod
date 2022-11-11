using Aequus;
using Aequus.Common;
using Aequus.Common.GlobalItems;
using Aequus.Common.GlobalNPCs;
using Aequus.Common.ModPlayers;
using Aequus.Common.Utilities;
using Aequus.Graphics.RenderTargets;
using Aequus.Items;
using Aequus.NPCs;
using Aequus.Particles.Dusts;
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
using System.Text;
using System.Text.RegularExpressions;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Creative;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using Terraria.UI;
using Terraria.UI.Chat;
using Terraria.Utilities;

namespace Aequus
{
    /// <summary>
    /// A helper class which contains many useful methods
    /// </summary>
    public static class AequusHelpers
    {
        public const int NPCREGEN = 8;
        public const char AirCharacter = '⠀';
        public const string AirString = "⠀";

        /// <summary>
        /// A static integer used for counting how many iterations for an iterative process has occured. Use this to prevent infinite loops, and always be sure to reset to 0 afterwards.
        /// </summary>
        public static int iterations;

        /// <summary>
        /// Determines whether or not the mouse has an item
        /// </summary>
        public static bool HasMouseItem => Main.mouseItem != null && !Main.mouseItem.IsAir;
        public static Vector2 ScaledMouseScreen => Vector2.Transform(Main.ReverseGravitySupport(Main.MouseScreen, 0f), Matrix.Invert(Main.GameViewMatrix.ZoomMatrix));
        public static Vector2 ScaledMouseworld => ScaledMouseScreen + Main.screenPosition;
        public static Vector2 TileDrawOffset => Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
        public const BindingFlags LetMeIn = BindingFlags.NonPublic | BindingFlags.Instance;

        public static Point tile => Main.MouseWorld.ToTileCoordinates();
        public static int tileX => Main.MouseWorld.ToTileCoordinates().X;
        public static int tileY => Main.MouseWorld.ToTileCoordinates().Y;

        public static int ColorOnlyShaderIndex => ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex;
        public static ArmorShaderData ColorOnlyShader => GameShaders.Armor.GetSecondaryShader(ColorOnlyShaderIndex, Main.LocalPlayer);

        public static bool debugKey => Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift);

        public static Regex SubstitutionRegex { get; private set; }
        public static ITypeUnboxer<int> UnboxInt { get; private set; }
        public static ITypeUnboxer<float> UnboxFloat { get; private set; }
        public static ITypeUnboxer<bool> UnboxBoolean { get; private set; }

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

        public static void AddBuffToHeadOrSelf(this NPC npc, int buffID, int buffDuration)
        {
            if (npc.realLife != -1)
            {
                Main.npc[npc.realLife].AddBuff(buffID, buffDuration);
                return;
            }

            npc.AddBuff(buffID, buffDuration);
        }

        public static bool InSceneRenderedMap(this TileMapCache map, Point p)
        {
            return InSceneRenderedMap(map, p.X, p.Y);
        }

        public static bool InSceneRenderedMap(this TileMapCache map, int x, int y)
        {
            return x > (ShutterstockerSceneRenderer.TilePaddingForChecking / 2) && x < (map.Width - ShutterstockerSceneRenderer.TilePaddingForChecking / 2)
                && y > (ShutterstockerSceneRenderer.TilePaddingForChecking / 2) && y < (map.Width - ShutterstockerSceneRenderer.TilePaddingForChecking / 2);
        }

        public static Point GetSpawn(this Player player) => new Point(GetSpawnX(player), GetSpawnY(player));
        public static int GetSpawnY(this Player player) => player.SpawnY > 0 ? player.SpawnY : Main.spawnTileY;
        public static int GetSpawnX(this Player player) => player.SpawnX > 0 ? player.SpawnX : Main.spawnTileX;

        public static bool IsInCustomTileInteractionRange(this Player player, int x, int y, int extraX, int extraY)
        {
            int tileRangeX = Player.tileRangeX;
            int tileRangeY = Player.tileRangeY;
            Player.tileRangeX += extraX;
            Player.tileRangeY += extraY;
            bool value = player.IsInTileInteractionRange(x, y);
            Player.tileRangeX = tileRangeX;
            Player.tileRangeY = tileRangeY;
            return value;
        }
        public static bool Zen(this Player player, bool? active = null)
        {
            var zen = player.GetModPlayer<ZenPlayer>();
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

        public static bool HasItemInInvOrVoidBag(this Player player, int item)
        {
            return player.HasItem(item) || (player.HasItem(ItemID.VoidLens) && player.bank4.HasItem(item));
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

        public static Point Home(this NPC npc)
        {
            return new Point(npc.homeTileX, npc.homeTileY);
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

        public static int FixedDamage(this NPC npc)
        {
            return Main.masterMode ? npc.damage / 3 : Main.expertMode ? npc.damage / 2 : npc.damage;
        }

        public static void DrawLine(Vector2 start, float rotation, float length, float width, Color color)
        {
            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, start, new Rectangle(0, 0, 1, 1), color, rotation, new Vector2(1f, 0.5f), new Vector2(length, width), SpriteEffects.None, 0f);
        }
        public static void DrawLine(Vector2 start, Vector2 end, float width, Color color)
        {
            DrawLine(start, (start - end).ToRotation(), (end - start).Length(), width, color);
        }

        public static Color GetRainbowHue(Projectile projectile, float index)
        {
            float laserLuminance = 0.5f;
            float laserAlphaMultiplier = 0f;
            float lastPrismHue = projectile.GetLastPrismHue(index % 6f, ref laserLuminance, ref laserAlphaMultiplier);
            return Main.hslToRgb(lastPrismHue, 1f, laserLuminance);
        }
        public static Color GetRainbowHue(int player, float position)
        {
            return GetRainbowHue(new Projectile() { owner = player }, position);
        }
        public static Color GetRainbowHue(Player player, float position)
        {
            return GetRainbowHue(player.whoAmI, position);
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
            tilePosition.Fluffize(10 + realSize);
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

        public static void DrawTile(Tile tile, Texture2D texture, Vector2 drawCoordinates, Color drawColor, int frameX, int frameY, int width, int height)
        {
            if (tile.Slope == 0 && !tile.IsHalfBlock)
            {
                Main.spriteBatch.Draw(texture, drawCoordinates, new Rectangle(frameX, frameY, width, height), drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
            else if (tile.IsHalfBlock)
            {
                Main.spriteBatch.Draw(texture, new Vector2(drawCoordinates.X, drawCoordinates.Y + 10), new Rectangle(frameX, frameY, width, 6), drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
            else
            {
                byte b = (byte)tile.Slope;
                for (int i = 0; i < 8; i++)
                {
                    int num10 = i << 1;
                    Rectangle frame = new Rectangle(frameX, frameY + i * 2, num10, 2);
                    int xOffset = 0;
                    switch (b)
                    {
                        case 2:
                            frame.X = 16 - num10;
                            xOffset = 16 - num10;
                            break;
                        case 3:
                            frame.Width = 16 - num10;
                            break;
                        case 4:
                            frame.Width = 14 - num10;
                            frame.X = num10 + 2;
                            xOffset = num10 + 2;
                            break;
                    }
                    Main.spriteBatch.Draw(texture, new Vector2(drawCoordinates.X + (float)xOffset, drawCoordinates.Y + i * 2), frame, drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
            }
        }
        public static void DrawWall(int i, int j, Texture2D texture, Color color)
        {
            var tile = Main.tile[i, j];
            Main.spriteBatch.Draw(texture, new Vector2(i * 16 - (int)Main.screenPosition.X - 8, j * 16 - (int)Main.screenPosition.Y - 8) + TileDrawOffset, new Rectangle(tile.WallFrameX, tile.WallFrameX + Main.wallFrame[tile.WallType] * 180, 32, 32), color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        public static Color GetColor(Vector2 v, Color color)
        {
            return Lighting.GetColor((int)(v.X / 16), (int)(v.Y / 16f), color);
        }

        public static Color GetColor(Vector2 v)
        {
            return Lighting.GetColor((int)(v.X / 16), (int)(v.Y / 16f));
        }

        public static void DrawRectangle(Rectangle rect, Vector2 offset, Color color)
        {
            rect.X += (int)offset.X;
            rect.Y += (int)offset.Y;
            DrawRectangle(rect, color);
        }

        public static void DrawRectangle(Rectangle rect, Color color)
        {
            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, rect, color);
        }

        public static void DrawFishingLine(Player player, Vector2 bobberPosition, int bobberWidth, int bobberHeight, Vector2 bobberVelocity, float velocitySum, Vector2 lineOrigin, Color? customColor = null, Func<Vector2, Color, Color> getLighting = null)
        {
            var color = customColor.GetValueOrDefault(Color.White);
            if (getLighting == null)
            {
                getLighting = GetColor;
            }
            var bobberCenter = new Vector2(bobberPosition.X + bobberWidth / 2f, bobberPosition.Y + bobberHeight / 2f);
            int type = player.inventory[player.selectedItem].type;

            Vector2 playerToProjectile = bobberCenter - lineOrigin;
            bool canDraw = true;
            if (playerToProjectile.X == 0f && playerToProjectile.Y == 0f)
                return;
            float playerToProjectileMagnitude = playerToProjectile.Length();
            playerToProjectileMagnitude = 12f / playerToProjectileMagnitude;
            playerToProjectile *= playerToProjectileMagnitude;
            lineOrigin -= playerToProjectile;
            playerToProjectile = bobberCenter - lineOrigin;
            float widthAdd = bobberWidth * 0.5f;
            float heightAdd = bobberHeight * 0.1f;
            var texture = TextureAssets.FishingLine.Value;
            while (canDraw)
            {
                float height = 12f;
                float positionMagnitude = playerToProjectile.Length();
                if (float.IsNaN(positionMagnitude) || float.IsNaN(positionMagnitude))
                    break;

                if (positionMagnitude < 20f)
                {
                    height = positionMagnitude - 8f;
                    canDraw = false;
                }
                playerToProjectile *= 12f / positionMagnitude;
                lineOrigin += playerToProjectile;
                playerToProjectile.X = bobberPosition.X + widthAdd - lineOrigin.X;
                playerToProjectile.Y = bobberPosition.Y + heightAdd - lineOrigin.Y;
                if (positionMagnitude > 12f)
                {
                    float positionInverseMultiplier = 0.3f;
                    float absVelocitySum = Math.Abs(bobberVelocity.X) + Math.Abs(bobberVelocity.Y);
                    if (absVelocitySum > 16f)
                        absVelocitySum = 16f;
                    absVelocitySum = 1f - absVelocitySum / 16f;
                    positionInverseMultiplier *= absVelocitySum;
                    absVelocitySum = positionMagnitude / 80f;
                    if (absVelocitySum > 1f)
                        absVelocitySum = 1f;
                    positionInverseMultiplier *= absVelocitySum;
                    if (positionInverseMultiplier < 0f)
                        positionInverseMultiplier = 0f;
                    absVelocitySum = 1f - velocitySum / 100f;
                    positionInverseMultiplier *= absVelocitySum;
                    if (playerToProjectile.Y > 0f)
                    {
                        playerToProjectile.Y *= 1f + positionInverseMultiplier;
                        playerToProjectile.X *= 1f - positionInverseMultiplier;
                    }
                    else
                    {
                        absVelocitySum = Math.Abs(bobberVelocity.X) / 3f;
                        if (absVelocitySum > 1f)
                            absVelocitySum = 1f;
                        absVelocitySum -= 0.5f;
                        positionInverseMultiplier *= absVelocitySum;
                        if (positionInverseMultiplier > 0f)
                            positionInverseMultiplier *= 2f;
                        playerToProjectile.Y *= 1f + positionInverseMultiplier;
                        playerToProjectile.X *= 1f - positionInverseMultiplier;
                    }
                }
                float rotation = playerToProjectile.ToRotation() - MathHelper.PiOver2;
                Main.EntitySpriteDraw(texture, new Vector2(lineOrigin.X - Main.screenPosition.X + texture.Width * 0.5f, lineOrigin.Y - Main.screenPosition.Y + texture.Height * 0.5f),
                    new Rectangle(0, 0, texture.Width, (int)height), getLighting(lineOrigin, color), rotation, new Vector2(texture.Width * 0.5f, 0f), 1f, SpriteEffects.None, 0);
            }
        }

        public static void DrawFramedChain(Texture2D chain, Rectangle frame, Vector2 currentPosition, Vector2 endPosition, Vector2 screenPos, Func<Vector2, Color> getLighting = null)
        {
            if (getLighting == null)
            {
                getLighting = GetColor;
            }
            int height = frame.Height - 2;
            Vector2 velocity = endPosition - currentPosition;
            int length = (int)(velocity.Length() / height);
            velocity.Normalize();
            velocity *= height;
            float rotation = velocity.ToRotation() + MathHelper.PiOver2;
            var origin = new Vector2(frame.Width / 2f, frame.Height / 2f);
            for (int i = 0; i < length; i++)
            {
                var position = currentPosition + velocity * i;
                Main.EntitySpriteDraw(chain, position - screenPos, frame, getLighting(position), rotation, origin, 1f, SpriteEffects.None, 0);
            }
        }

        public static void DrawChain(Texture2D chain, Vector2 currentPosition, Vector2 endPosition, Vector2 screenPos, Func<Vector2, Color> getLighting = null)
        {
            DrawFramedChain(chain, chain.Bounds, currentPosition, endPosition, screenPos, getLighting);
        }

        public static bool HereditarySource(IEntitySource source, out Entity entity)
        {
            entity = null;
            if (source == null)
            {
                return false;
            }
            if (source is EntitySource_OnHit onHit)
            {
                entity = onHit.EntityStruck;
                return true;
            }
            else if (source is EntitySource_Parent parent)
            {
                entity = parent.Entity;
                return true;
            }
            else if (source is EntitySource_HitEffect hitEffect)
            {
                entity = hitEffect.Entity;
                return true;
            }
            else if (source is EntitySource_Death death)
            {
                entity = death.Entity;
                return true;
            }
            return false;
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
            else if (entity is Projectile projectile && projectile.TryGetGlobalProjectile<SentryAccessoriesGlobalProj>(out var santankSentry))
            {
                return santankSentry.dummyPlayer;
            }
            return null;
        }

        public static Entity GetHereditaryOwner(this Projectile projectile)
        {
            int projIdentity = (int)projectile.ai[0] - 1;

            if (projIdentity > -1)
            {
                projIdentity = FindProjectileIdentity(projectile.owner, projIdentity);
                if (projIdentity == -1 || !Main.projectile[projIdentity].active || !Main.projectile[projIdentity].TryGetGlobalProjectile<SentryAccessoriesGlobalProj>(out var value))
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

        public static void DropMoney(IEntitySource source, Rectangle rect, int amt, bool quiet = true)
        {
            int[] coins = Utils.CoinsSplit(amt);
            for (int i = 0; i < coins.Length; i++)
            {
                if (coins[i] > 0)
                {
                    int item = Item.NewItem(source, rect, ItemID.CopperCoin + i, coins[i]);
                    if (!quiet && Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        NetMessage.SendData(MessageID.SyncItem, -1, -1, null, item, 1f);
                    }
                }
            }
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

        public static void HideBestiaryEntry(this ModNPC npc)
        {
            NPCID.Sets.NPCBestiaryDrawOffset.Add(npc.Type, new NPCID.Sets.NPCBestiaryDrawModifiers(0) { Hide = true, });
        }

        public static void CollideWithOthers(this NPC npc, float speed = 0.05f)
        {
            var rect = npc.getRect();
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && i != npc.whoAmI && npc.type == Main.npc[i].type
                    && rect.Intersects(Main.npc[i].getRect()))
                {
                    npc.velocity += Main.npc[i].DirectionTo(npc.Center).UnNaN() * speed;
                }
            }
        }

        public static void GetDrawInfo(this NPC npc, out Texture2D texture, out Vector2 offset, out Rectangle frame, out Vector2 origin, out int trailLength)
        {
            texture = TextureAssets.Npc[npc.type].Value;
            offset = npc.Size / 2f;
            frame = npc.frame;
            origin = frame.Size() / 2f;
            trailLength = NPCID.Sets.TrailCacheLength[npc.type];
        }

        public static void GetDrawInfo(this Projectile projectile, out Texture2D texture, out Vector2 offset, out Rectangle frame, out Vector2 origin, out int trailLength)
        {
            texture = TextureAssets.Projectile[projectile.type].Value;
            offset = projectile.Size / 2f;
            frame = projectile.Frame();
            origin = frame.Size() / 2f;
            trailLength = ProjectileID.Sets.TrailCacheLength[projectile.type];
        }

        public static Vector2 NextFromRect(this UnifiedRandom rand, Rectangle rectangle)
        {
            return rectangle.Center.ToVector2() + new Vector2(rand.NextFloat(rectangle.Width), rand.NextFloat(rectangle.Height));
        }

        public static Vector2 NextCircularFromRect(this UnifiedRandom rand, Rectangle rectangle)
        {
            return rectangle.Center.ToVector2() + rand.NextVector2Unit() * new Vector2(rand.NextFloat(rectangle.Width / 2f), rand.NextFloat(rectangle.Height / 2f));
        }

        public static Point FluffizePoint(Point point, int fluff = 10)
        {
            point.Fluffize(fluff);
            return point;
        }

        public static void Fluffize(this ref Point point, int fluff = 10)
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
                if (vanillaOnly && n.Key > Main.maxNPCTypes)
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

        public static int ToBannerItem(this NPC npc)
        {
            return Item.BannerToItem(npc.ToBanner());
        }
        public static int ToBanner(this NPC npc)
        {
            return Item.NPCtoBanner(npc.BannerID());
        }

        public static void AddRegen(this NPC npc, int regen)
        {
            if (regen < 0 && npc.lifeRegen > 0)
            {
                npc.lifeRegen = 0;
            }
            npc.lifeRegen += regen;
        }
        public static void AddRegenOld(this NPC npc, int regen)
        {
            if (regen < 0 && npc.lifeRegen > 0)
            {
                npc.lifeRegen = 0;
            }
            npc.lifeRegen += regen * NPCREGEN;
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

        public static bool IsProbablyACritter(this NPC npc)
        {
            return NPCID.Sets.CountsAsCritter[npc.type] || (npc.lifeMax < 5 && npc.lifeMax != 1);
        }

        public static bool IsTheDestroyer(this NPC npc)
        {
            return npc.type == NPCID.TheDestroyer || npc.type == NPCID.TheDestroyerBody || npc.type == NPCID.TheDestroyerTail;
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

        public static void WriteText(this Stream stream, string text, Encoding encoding = null)
        {
            encoding ??= Encoding.ASCII;
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

        public static NPC CreateSudo(NPC npc)
        {
            var npc2 = new NPC();
            npc2.SetDefaults(npc.type);
            for (int i = 0; i < npc.ai.Length; i++)
            {
                npc2.ai[i] = npc.ai[i];
            }
            for (int i = 0; i < npc.localAI.Length; i++)
            {
                npc2.localAI[i] = npc.localAI[i];
            }
            npc2.width = npc.width;
            npc2.height = npc.height;
            npc2.scale = npc.scale;
            npc2.frame = npc.frame;
            npc2.direction = npc.direction;
            npc2.spriteDirection = npc.spriteDirection;
            npc2.velocity = npc.velocity;
            npc2.rotation = npc.rotation;
            npc2.gfxOffY = npc.gfxOffY;

            var oldSlot = Main.npc[npc.whoAmI];
            try
            {
                npc2.position = npc.position;
                Main.npc[npc.whoAmI] = npc2;
                npc2.AI();
                Main.npc[npc.whoAmI] = oldSlot;
                npc2.position = npc.position;
            }
            catch
            {
                Main.npc[npc.whoAmI] = oldSlot;
            }
            return npc2;
        }

        public static TooltipsGlobalItem AequusTooltips(this Item item)
        {
            return item.GetGlobalItem<TooltipsGlobalItem>();
        }
        public static AequusItem Aequus(this Item item)
        {
            return item.GetGlobalItem<AequusItem>();
        }
        public static ref float StatSpeed(this NPC npc)
        {
            return ref npc.GetGlobalNPC<StatSpeedGlobalNPC>().statSpeed;
        }
        public static AequusNPC Aequus(this NPC npc)
        {
            return npc.GetGlobalNPC<AequusNPC>();
        }
        public static AequusProjectile Aequus(this Projectile projectile)
        {
            return projectile.GetGlobalProjectile<AequusProjectile>();
        }
        public static AequusPlayer Aequus(this Player player)
        {
            return player.GetModPlayer<AequusPlayer>();
        }

        internal static void spawnNPC<T>(Vector2 where) where T : ModNPC
        {
            NPC.NewNPC(null, (int)where.X, (int)where.Y, ModContent.NPCType<T>());
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
            if (comparisonTile > Main.maxTileSets)
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

        public static void DrawTrail(this ModProjectile modProjectile, Action<Vector2, float> draw)
        {
            int trailLength = ProjectileID.Sets.TrailCacheLength[modProjectile.Type];
            var offset = new Vector2(modProjectile.Projectile.width / 2f, modProjectile.Projectile.height / 2f);
            for (int i = 0; i < trailLength; i++)
            {
                draw(modProjectile.Projectile.oldPos[i] + offset, 1f - 1f / trailLength * i);
            }
        }

        public static void SetTrail(this ModProjectile modProjectile, int length = -1)
        {
            if (length > 0)
            {
                ProjectileID.Sets.TrailCacheLength[modProjectile.Type] = length;
            }
            ProjectileID.Sets.TrailingMode[modProjectile.Type] = 2;
        }

        public static void GetItemDrawData(int item, out Rectangle frame)
        {
            frame = Main.itemAnimations[item] == null ? TextureAssets.Item[item].Value.Frame() : Main.itemAnimations[item].GetFrame(TextureAssets.Item[item].Value);
        }
        public static void GetItemDrawData(this Item item, out Rectangle frame)
        {
            GetItemDrawData(item.type, out frame);
        }

        public static Vector2 ClosestDistance(this Rectangle rect, Vector2 other)
        {
            var center = rect.Center.ToVector2();
            var n = Vector2.Normalize(other - center);
            float x = Math.Min((other.X - center.X).Abs(), rect.Width / 2f);
            float y = Math.Min((other.Y - center.Y).Abs(), rect.Height / 2f);
            return center + n * new Vector2(x, y);
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

        public static void SyncNPC(NPC npc)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendData(MessageID.SyncNPC, Main.myPlayer, -1, null, npc.whoAmI);
        }

        public static int CheckForPlayers(Rectangle rectangle)
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
            return Main.netMode != NetmodeID.Server && (new Vector2(Main.screenPosition.X + Main.screenWidth / 2f, Main.screenPosition.Y + Main.screenHeight / 2f) - location).Length() < 2000f;
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

        public static void SetLiquidSpeeds(this NPC npc, float water = 0.5f, float lava = 0.5f, float honey = 0.25f)
        {
            AequusNPC.NPC_waterMovementSpeed.SetValue(npc, water);
            AequusNPC.NPC_lavaMovementSpeed.SetValue(npc, lava);
            AequusNPC.NPC_honeyMovementSpeed.SetValue(npc, honey);
        }

        public static T ModItem<T>(this Item item) where T : ModItem
        {
            return (T)item.ModItem;
        }
        public static T ModProjectile<T>(this Projectile projectile) where T : ModProjectile
        {
            return (T)projectile.ModProjectile;
        }
        public static T ModNPC<T>(this NPC npc) where T : ModNPC
        {
            return (T)npc.ModNPC;
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
            return t.Namespace.Replace('.', '/') + "/" + t.Name;
        }

        public static void debugTextDraw(string text, Vector2 where)
        {
            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, text, where, Color.White, 0f, Vector2.Zero, Vector2.One);
        }
        public static Dust dustDebugDirect(Vector2 where, int dustType = DustID.Torch)
        {
            var d = Dust.NewDustPerfect(where, dustType);
            d.noGravity = true;
            d.fadeIn = d.scale * 2f;
            d.velocity = Vector2.Zero;
            return d;
        }
        public static void dustDebugColor(Vector2 where, Color color)
        {
            var d = dustDebugDirect(where, ModContent.DustType<MonoSparkleDust>());
            d.color = color;
        }
        public static void dustDebug(Vector2 where, int dustType = DustID.Torch)
        {
            dustDebugDirect(where, dustType);
        }
        public static void dustDebug(Point where, int dustType = DustID.Torch)
        {
            dustDebug(where.X, where.Y, dustType);
        }
        public static void dustDebug(int x, int y, int dustType = DustID.Torch)
        {
            var rect = new Rectangle(x * 16, y * 16, 16, 16);
            for (int i = 0; i < 4; i++)
            {
                i *= 4;
                var d = Dust.NewDustPerfect(new Vector2(rect.X + i, rect.Y), dustType);
                d.noGravity = true;
                d.fadeIn = d.scale * 2f;
                d.velocity = Vector2.Zero;
                d = Dust.NewDustPerfect(new Vector2(rect.X + i, rect.Y + rect.Height), dustType);
                d.noGravity = true;
                d.fadeIn = d.scale * 2f;
                d.velocity = Vector2.Zero;
                d = Dust.NewDustPerfect(new Vector2(rect.X, rect.Y + i), dustType);
                d.noGravity = true;
                d.fadeIn = d.scale * 2f;
                d.velocity = Vector2.Zero;
                d = Dust.NewDustPerfect(new Vector2(rect.X + rect.Width, rect.Y + i), dustType);
                d.noGravity = true;
                d.fadeIn = d.scale * 2f;
                d.velocity = Vector2.Zero;
                i /= 4;
            }
        }
        public static void dustDebug(Rectangle rect, int dustType = DustID.Torch)
        {
            int amt = rect.Width / 2;
            for (int i = 0; i < amt; i++)
            {
                i *= 2;
                var d = Dust.NewDustPerfect(new Vector2(rect.X + i, rect.Y), dustType);
                d.noGravity = true;
                d.fadeIn = d.scale * 2f;
                d.velocity = Vector2.Zero;
                d = Dust.NewDustPerfect(new Vector2(rect.X + i, rect.Y + rect.Height), dustType);
                d.noGravity = true;
                d.fadeIn = d.scale * 2f;
                d.velocity = Vector2.Zero;
                i /= 2;
            }
            amt = rect.Height / 2;
            for (int i = 0; i < amt; i++)
            {
                i *= 2;
                var d = Dust.NewDustPerfect(new Vector2(rect.X, rect.Y + i), dustType);
                d.noGravity = true;
                d.fadeIn = d.scale * 2f;
                d.velocity = Vector2.Zero;
                d = Dust.NewDustPerfect(new Vector2(rect.X + rect.Width, rect.Y + i), dustType);
                d.noGravity = true;
                d.fadeIn = d.scale * 2f;
                d.velocity = Vector2.Zero;
                i /= 2;
            }
        }

        public class Loader : IOnModLoad
        {
            void ILoadable.Load(Mod mod)
            {
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