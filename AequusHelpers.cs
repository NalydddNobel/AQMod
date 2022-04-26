using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Aequus
{
    public static class AequusHelpers
    {
        public static int Iterations;

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

        public static bool HasMouseItem()
        {
            return Main.mouseItem != null && !Main.mouseItem.IsAir;
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

        public static bool ContainsAny<T>(this IEnumerable<T> en, int en2)
        {
            foreach (var t in en)
            {
                if (t.Equals(en2))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool ContainsAny<T>(this IEnumerable<T> en, IEnumerable<T> en2)
        {
            foreach (var t in en)
            {
                foreach (var t2 in en2)
                {
                    if (t.Equals(t2))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static void PlaySound<T>() where T : ModSound
        {
            SoundEngine.PlaySound(From<T>());
        }
        public static void PlaySound<T>(Vector2 location) where T : ModSound
        {
            SoundEngine.PlaySound(From<T>(), location);
        }
        public static LegacySoundStyle From<T>() where T : ModSound
        {
            return SoundLoader.GetLegacySoundSlot(typeof(T).Namespace.Replace('.', '/') + "/" + ModContent.GetInstance<T>().Name);
        }
        public static void PlaySound(SoundType type, string name)
        {
            if (type != SoundType.Sound)
            {
                name = type.ToString() + "/" + name;
            }
            var slot = SoundLoader.GetLegacySoundSlot("Aequus/Sounds/" + name);
            SoundEngine.PlaySound(slot);
        }
        public static void PlaySound(SoundType type, string name, Vector2 position)
        {
            if (Main.dedServ)
            {
                return;
            }
            if (type != SoundType.Sound)
            {
                name = type.ToString() + "/" + name;
            }
            var slot = SoundLoader.GetLegacySoundSlot("Aequus/Sounds/" + name);
            SoundEngine.PlaySound(slot, position);
        }
        public static void PlaySound(SoundType type, string name, Vector2 position, float volume = 1f, float pitch = 0f)
        {
            if (Main.dedServ)
            {
                return;
            }
            if (type != SoundType.Sound)
            {
                name = type.ToString() + "/" + name;
            }
            var slot = SoundLoader.GetLegacySoundSlot("Aequus/Sounds/" + name);
            SoundEngine.PlaySound(slot.SoundId, (int)position.X, (int)position.Y, slot.Style, volume, pitch);
        }
        public static void PlaySound(this LegacySoundStyle value, Vector2 position, float volume, float pitch)
        {
            SoundEngine.PlaySound(value.SoundId, (int)position.X, (int)position.Y, value.Style, volume, pitch);
        }
        public static void PlaySound(this LegacySoundStyle value, Vector2 position, float volume)
        {
            SoundEngine.PlaySound(value.SoundId, (int)position.X, (int)position.Y, value.Style, volume);
        }
        public static void PlaySound(this LegacySoundStyle value, Vector2 position)
        {
            SoundEngine.PlaySound(value, position);
        }
        public static void PlaySound(this LegacySoundStyle value)
        {
            SoundEngine.PlaySound(value);
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

        public static void GetItemDrawData(this Item item, out Rectangle frame)
        {
            frame = Main.itemAnimations[item.type] == null ? TextureAssets.Item[item.type].Value.Frame() : Main.itemAnimations[item.type].GetFrame(TextureAssets.Item[item.type].Value);
        }

        public static Vector2 ClosestDistance(this Rectangle rect, Vector2 other)
        {
            var center = rect.Center.ToVector2();
            var n = Vector2.Normalize(other - center);
            float x = Math.Min((other.X - center.X).Abs(), rect.Width / 2f);
            float y = Math.Min((other.Y - center.Y).Abs(), rect.Height / 2f);
            return center + n * new Vector2(x, y);
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

        public static int RollVanillaSwordPrefix(Item item, UnifiedRandom rand)
        {
            int choice = rand.Next(40);
            switch (choice)
            {
                default:
                    return PrefixID.Large;
                case 1:
                    return PrefixID.Massive;
                case 2:
                    return PrefixID.Dangerous;
                case 3:
                    return 4;
                case 4:
                    return 5;
                case 5:
                    return 6;
                case 6:
                    return 7;
                case 7:
                    return 8;
                case 8:
                    return 9;
                case 9:
                    return 10;
                case 10:
                    return 11;
                case 11:
                    return 12;
                case 12:
                    return 13;
                case 13:
                    return 14;
                case 14:
                    return 15;
                case 15:
                    return 36;
                case 16:
                    return 37;
                case 17:
                    return 38;
                case 18:
                    return 53;
                case 19:
                    return 54;
                case 20:
                    return 55;
                case 21:
                    return 39;
                case 22:
                    return 40;
                case 23:
                    return 56;
                case 24:
                    return 41;
                case 25:
                    return 57;
                case 26:
                    return 42;
                case 27:
                    return 43;
                case 28:
                    return 44;
                case 29:
                    return 45;
                case 30:
                    return 46;
                case 31:
                    return 47;
                case 32:
                    return 48;
                case 33:
                    return 49;
                case 34:
                    return 50;
                case 35:
                    return 51;
                case 36:
                    return 59;
                case 37:
                    return PrefixID.Demonic;
                case 38:
                    return PrefixID.Zealous;
                case 39:
                    return PrefixID.Legendary;
            }
        }
        public static int RollSwordPrefix(Item item, UnifiedRandom rand)
        {
            int num = RollVanillaSwordPrefix(item, rand);
            PrefixLoader.Roll(item, ref num, 40, rand, PrefixCategory.AnyWeapon, PrefixCategory.Melee);
            return num;
        }

        public static void MeleeScale(Projectile proj)
        {
            float scale = Main.player[proj.owner].GetAdjustedItemScale(Main.player[proj.owner].HeldItem);
            if (scale != 1f)
            {
                proj.scale *= scale;
                proj.width = (int)(proj.width * proj.scale);
                proj.height = (int)(proj.height * proj.scale);
            }
        }

        public static Color MaxRGBA(this Color color, byte amt)
        {
            return MaxRGBA(color, amt, amt);
        }
        public static Color MaxRGBA(this Color color, byte amt, byte a)
        {
            return MaxRGBA(color, amt, amt, amt, a);
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
            return;
            // TODO: Check if these fields still exist
            typeof(NPC).GetField("waterMovementSpeed", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(npc, water);
            typeof(NPC).GetField("lavaMovementSpeed", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(npc, lava);
            typeof(NPC).GetField("honeyMovementSpeed", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(npc, honey);
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
            //try
            //{
            //}
            //catch
            //{
            //    Main.NewText(index);
            //    Main.NewText(index % colors.Length, Color.Blue);
            //    Main.NewText((index + 1) % colors.Length, Color.Red);
            //    Main.NewText(amount, Color.BlanchedAlmond);
            //    Main.NewText(amount % colors.Length, Color.AliceBlue);
            //    Main.NewText(colors.Length, Main.DiscoColor);
            //}
            //return Color.Black;
        }

        public static int TimedBasedOn(int timer, int ticksPer, int loop)
        {
            timer %= ticksPer * loop;
            return timer / ticksPer;
        }

        public static void DrawUIBack(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Rectangle itemFrame, float itemScale, Color color, float progress = 1f)
        {
            int frameY = (int)(texture.Height * progress);
            var uiFrame = new Rectangle(0, texture.Height - frameY, texture.Width, frameY);
            position.Y += uiFrame.Y * Main.inventoryScale;
            var center = position + itemFrame.Size() / 2f * itemScale;
            spriteBatch.Draw(texture, center, uiFrame, color, 0f, texture.Size() / 2f, Main.inventoryScale, SpriteEffects.None, 0f);
        }

        public static Item DefaultItem(int type)
        {
            var item = new Item();
            item.SetDefaults(type);
            return item;
        }

        public static bool Insert(this Chest chest, int itemType, int index)
        {
            return Insert(chest, itemType, 1, index);
        }
        public static bool Insert(this Chest chest, int itemType, int itemStack, int index)
        {
            var item = DefaultItem(itemType);
            item.stack = itemStack;
            return InsertIntoUnresizableArray(chest.item, item, index);
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

        public static bool UpdateProjActive(Projectile projectile, ref bool active)
        {
            if (Main.player[projectile.owner].dead)
                active = false;
            if (active)
                projectile.timeLeft = 2;
            return active;
        }

        public static void SetResearch(this ModItem modItem, int amt)
        {
            SetResearch(modItem.Type, amt);
        }
        public static void SetResearch(int type, int amt)
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[type] = amt;
        }

        public static int FindTargetWithLineOfSight(Vector2 position, int width = 2, int height = 2, float maxRange = 800f, object me = null, Func<int, bool> validCheck = null)
        {
            float num = maxRange;
            int result = -1;
            for (int i = 0; i < 200; i++)
            {
                NPC nPC = Main.npc[i];
                if (nPC.CanBeChasedBy(me) && (validCheck == null || validCheck.Invoke(i)))
                {
                    float num2 = Vector2.Distance(position, Main.npc[i].Center);
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

        public static float Wave(float time, float minimum, float maximum)
        {
            return minimum + ((float)Math.Sin(time) + 1f) / 2f * (maximum - minimum);
        }

        public static bool SolidTop(this Tile tile)
        {
            return Main.tileSolidTop[tile.TileType];
        }

        public static bool Solid(this Tile tile)
        {
            return Main.tileSolid[tile.TileType];
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
    }
}