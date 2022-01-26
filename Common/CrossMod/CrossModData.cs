using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.ModLoader;

namespace AQMod.Common.CrossMod
{
    internal struct CrossModData
    {
        public readonly string name;
        public readonly string codeName;
        public readonly bool active;
        public readonly Mod mod;

        public static CrossModData Unloaded => new CrossModData();

        public CrossModData(string name)
        {
            this.name = name;
            codeName = AQStringCodes.EncodeModName(name);
            mod = null;
            try
            {
                mod = ModLoader.GetMod(name);
            }
            catch (Exception ex)
            {
                AQMod.GetInstance().Logger.Error("Mod failed to be checked active: " + name, ex);
            }
            active = mod != null;
        }

        public int ItemType(string name)
        {
            return mod.ItemType(name);
        }

        public int NPCType(string name)
        {
            return mod.NPCType(name);
        }

        public int TileType(string name)
        {
            return mod.TileType(name);
        }

        public int BuffType(string name)
        {
            return mod.BuffType(name);
        }

        public int DustType(string name)
        {
            return mod.DustType(name);
        }

        public int MountType(string name)
        {
            return mod.MountType(name);
        }

        public int PrefixType(string name)
        {
            return mod.PrefixType(name);
        }

        public int ProjectileType(string name)
        {
            return mod.ProjectileType(name);
        }

        public int TileEntityType(string name)
        {
            return mod.TileEntityType(name);
        }

        public int WallType(string name)
        {
            return mod.WallType(name);
        }

        public Texture2D GetTexture(string name)
        {
            return mod.GetTexture(name);
        }

        public bool TryGetTexture(string name, out Texture2D texture)
        {
            if (mod.TextureExists(name))
            {
                texture = mod.GetTexture(name);
                return true;
            }
            texture = null;
            return false;
        }
    }
}