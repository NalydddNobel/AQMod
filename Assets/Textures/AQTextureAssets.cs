using AQMod.Assets.Enumerators;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace AQMod.Assets.Textures
{
    public class AQTextureAssets
    {
        public const string None = "Assets/Textures/Empty";

        public const int MapIconFrames = 4;
        public const int MapIconWidth = 34;
        public const int MapIconWidthPadding = 2;
        public const int TrueMapIconWidth = MapIconWidth - MapIconWidthPadding;
        public const int MapIconHeight = 32;

        public TEA<ExtraID> Extras { get; private set; }
        public TEA<GlowID> Glows { get; private set; }
        public TEA<PlayerMaskID> PlayerMasks { get; private set; }
        public TEA<LightID> Lights { get; private set; }
        public TEA<TrailID> Trails { get; private set; }
        public TEA<PlayerHeadOverlayID> PlayerHeadOverlays { get; private set; }

        public Texture2D Pixel { get; private set; }
        public Texture2D UI { get; private set; }

        public static Texture2D GetItem(int type)
        {
            //Main.instance.LoadItem(type);
            return Main.itemTexture[type];
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

        public AQTextureAssets()
        {
        }
    }
}