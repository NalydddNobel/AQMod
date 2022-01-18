using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace AQMod.Gores
{
    public static class AQGore
    {
        public static string Rand(string path, int max = 0)
        {
            return Rand(path, Main.rand, max);
        }
        public static string Rand(string path, UnifiedRandom rand, int max = 0)
        {
            return Rand(path, 0, max);
        }
        public static string Rand(string path, int min = 0, int max = 0)
        {
            return Rand(path, Main.rand, min, max);
        }
        public static string Rand(string path, UnifiedRandom rand, int min = 0, int max = 0)
        {
            return path + "_" + min + rand.Next(max - min);
        }

        public static int GetID(string name)
        {
            return ModGore.GetGoreSlot("AQMod/Gores/" + name);
        }

        public static int GetID<T>() where T : ModGore
        {
            return ModGore.GetGoreSlot((typeof(T).Namespace + '/' + typeof(T).Name).Replace('.', '/'));
        }

        public static void NewGore(Rectangle rect, string name, float scale = 1f)
        {
            NewGore(rect, ModGore.GetGoreSlot("AQMod/Gores/" + name), scale);
        }
        public static void NewGore(Rectangle rect, int type, float scale = 1f)
        {
            NewGore(rect, 4f, type, scale);
        }
        public static void NewGore(Rectangle rect, float speed, string name, float scale = 1f)
        {
            NewGore(rect, speed, ModGore.GetGoreSlot("AQMod/Gores/" + name), scale);
        }
        public static void NewGore(Rectangle rect, float speed, int type, float scale = 1f)
        {
            NewGore(rect, new Vector2(speed, 0f).RotatedBy(-Main.rand.NextFloat(MathHelper.Pi, MathHelper.Pi)), type, scale);
        }
        public static void NewGore(Rectangle rect, Vector2 velocity, string name, float scale = 1f)
        {
            NewGore(rect, velocity, ModGore.GetGoreSlot("AQMod/Gores/" + name), scale);
        }
        public static void NewGore(Rectangle rect, Vector2 velocity, int type, float scale = 1f)
        {
            NewGore(rect.RandomPosition(), velocity, type, scale);
        }
        public static void NewGore(Vector2 position, string name, float scale = 1f)
        {
            NewGore(position, ModGore.GetGoreSlot("AQMod/Gores/" + name), scale);
        }
        public static void NewGore(Vector2 position, int type, float scale = 1f)
        {
            NewGore(position, 4f, type, scale);
        }
        public static void NewGore(Vector2 position, float speed, string name, float scale = 1f)
        {
            NewGore(position, speed, ModGore.GetGoreSlot("AQMod/Gores/" + name), scale);
        }
        public static void NewGore(Vector2 position, float speed, int type, float scale = 1f)
        {
            NewGore(position, new Vector2(speed, 0f).RotatedBy(-Main.rand.NextFloat(MathHelper.Pi, MathHelper.Pi)), type, scale);
        }
        public static void NewGore(Vector2 position, Vector2 velocity, string name, float scale = 1f)
        {
            NewGore(position, velocity, ModGore.GetGoreSlot("AQMod/Gores/" + name), scale);
        }
        public static void NewGore(Vector2 position, Vector2 velocity, int type, float scale = 1f)
        {
            Gore.NewGore(position, velocity, type, scale);
        }
    }
}
