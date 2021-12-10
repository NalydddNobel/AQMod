using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Assets
{
    public static class TextureGrabber
    {
        public static Texture2D GetTexture(this ModItem ModItem)
        {
            return GetItem(ModItem.item.type);
        }

        public static Texture2D GetTexture(this Item Item)
        {
            return GetItem(Item.type);
        }

        public static Texture2D GetItem(int type)
        {
            //Main.instance.LoadItem(type);
            return Main.itemTexture[type];
        }

        public static Texture2D GetTexture(this ModProjectile ModProjectile)
        {
            return GetProjectile(ModProjectile.projectile.type);
        }

        public static Texture2D GetTexture(this Projectile Projectile)
        {
            return GetProjectile(Projectile.type);
        }

        public static Texture2D GetProjectile(int type)
        {
            Main.instance.LoadProjectile(type);
            return Main.projectileTexture[type];
        }

        public static Texture2D GetNPC(int type)
        {
            Main.instance.LoadNPC(type);
            return Main.npcTexture[type];
        }

        public static Texture2D GetBuff(int type)
        {
            //Main.instance.LoadBuff(type);
            return Main.buffTexture[type];
        }

        public static Texture2D GetWall(int type)
        {
            Main.instance.LoadWall(type);
            return Main.wallTexture[type];
        }
    }
}