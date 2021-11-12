using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace AQMod.Common.CrossMod
{
    internal class CrossModType
    {
        public string ModName { get; protected set; }

        public CrossModType(string name) 
        {
            ModName = name;
        }

        public bool Active { get; internal set; }
        public Mod Instance { get; private set; }
        public Mod GetInstance()
        {
            return Instance = ModLoader.GetMod(ModName);
        }

        public void Load()
        {
            Instance = null;
            try
            {
                GetInstance();
            }
            catch
            {
            }
            Active = Instance != null;
            setup();
        }

        protected virtual void setup()
        {
        }

        public virtual void Unload()
        {
        }


        public int ItemType(string name)
        {
            return Instance.ItemType(name);
        }

        public int NPCType(string name)
        {
            return Instance.NPCType(name);
        }

        public int TileType(string name)
        {
            return Instance.TileType(name);
        }

        public int BuffType(string name)
        {
            return Instance.BuffType(name);
        }

        public int DustType(string name)
        {
            return Instance.DustType(name);
        }

        public int MountType(string name)
        {
            return Instance.MountType(name);
        }

        public int PrefixType(string name)
        {
            return Instance.PrefixType(name);
        }

        public int ProjectileType(string name)
        {
            return Instance.ProjectileType(name);
        }

        public int TileEntityType(string name)
        {
            return Instance.TileEntityType(name);
        }

        public int WallType(string name)
        {
            return Instance.WallType(name);
        }

        public Texture2D GetTexture(string name)
        {
            return Instance.GetTexture(name);
        }

        public bool TryGetTexture(string name, out Texture2D texture)
        {
            if (Instance.TextureExists(name))
            {
                texture = Instance.GetTexture(name);
                return true;
            }
            texture = null;
            return false;
        }
    }
}