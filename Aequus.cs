using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace Aequus
{
    public class Aequus : Mod
    {
        internal delegate void LegacyDrawMethod(Texture2D texture, Vector2 position, Rectangle? frame, Color color, float scale, Vector2 origin, float rotation, SpriteEffects effects, float layerDepth);

        public const string TextureNone = "Aequus/Assets/None";

        public static Color MysteriousGuideTooltip => new Color(225, 100, 255, 255);
        public static Color DemonSiegeTooltip => new Color(255, 170, 150, 255);
        public static bool GameWorldActive => Main.instance.IsActive && !Main.gamePaused && !Main.gameInactive;

        public static Aequus Instance { get; private set; }

        public override void Load()
        {
            Instance = this;
        }

        public override void Unload()
        {
            Instance = null;
        }

        public static Matrix GetWorldViewPoint()
        {
            GraphicsDevice graphics = Main.graphics.GraphicsDevice;
            Vector2 screenZoom = Main.GameViewMatrix.Zoom;
            int width = graphics.Viewport.Width;
            int height = graphics.Viewport.Height;

            var zoom = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up) *
                Matrix.CreateTranslation(width / 2f, height / -2f, 0) *
                Matrix.CreateRotationZ(MathHelper.Pi) * Matrix.CreateScale(screenZoom.X, screenZoom.Y, 1f);
            var projection = Matrix.CreateOrthographic(width, height, 0, 1000);
            return zoom * projection;
        }

        public static Texture2D Tex(string path)
        {
            return TexAsset(path).Value;
        }
        public static Texture2D MyTex(string path)
        {
            return TexAsset(Instance.Name + "/" + path).Value;
        }
        public static Asset<Texture2D> TexAsset(string path)
        {
            return ModContent.Request<Texture2D>(path, AssetRequestMode.ImmediateLoad);
        }
    }
}