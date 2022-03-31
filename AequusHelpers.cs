using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus
{
    public static partial class AequusHelpers
    {
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

        public static bool UglyCodeForCheckingIfYouAreInAnAlternateMaterialWorld(int tier)
        {
            if (Main.drunkWorld)
            {
                return WorldGen.genRand.NextBool();
            }
            if (tier == 1)
            {
                return WorldGen.SavedOreTiers.Iron == TileID.Iron;
            }
            else if (tier == 2)
            {
                return WorldGen.SavedOreTiers.Silver == TileID.Silver;
            }
            else if (tier == 3)
            {
                return WorldGen.SavedOreTiers.Gold == TileID.Gold;
            }
            return WorldGen.SavedOreTiers.Copper == TileID.Copper;
        }
    }
}