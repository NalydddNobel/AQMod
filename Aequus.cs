using Aequus.Common.Configuration;
using Aequus.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus
{
    public class Aequus : Mod
    {
        internal delegate void LegacyDrawMethod(Texture2D texture, Vector2 position, Rectangle? frame, Color color, float scale, Vector2 origin, float rotation, SpriteEffects effects, float layerDepth);

        public const string TextureNone = "Aequus/Assets/None";

        public static Aequus Instance { get; private set; }
        public static UserInterface InventoryInterface { get; private set; }
        public static UserInterface NPCTalkInterface { get; private set; }

        public static bool GameWorldActive => Main.instance.IsActive && !Main.gamePaused && !Main.gameInactive;
        public static bool HQ => ClientConfig.Instance.HighQuality;

        internal static Color GreenSlimeColor => new Color(0, 220, 40, 100);
        internal static Color BlueSlimeColor => new Color(0, 80, 255, 100);

        public override void Load()
        {
            Instance = this;
            AequusHelpers.Main_dayTime = new StaticManipulator<bool>(() => ref Main.dayTime);
            AequusText.OnModLoad(this);
            ClientConfig.OnModLoad(this);
            if (Main.netMode != NetmodeID.Server)
            {
                InventoryInterface = new UserInterface();
                NPCTalkInterface = new UserInterface();
            }
        }

        public override void Unload()
        {
            Instance = null;
            AequusHelpers.Main_dayTime = null;
            InventoryInterface = null;
            NPCTalkInterface = null;
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

        public static Asset<Texture2D> TexAsset(string path)
        {
            return ModContent.Request<Texture2D>(path, AssetRequestMode.ImmediateLoad);
        }
        public static Texture2D Tex(string path)
        {
            return TexAsset(path).Value;
        }
        public static Texture2D MyTex(string path)
        {
            return TexAsset(Instance.Name + "/" + path).Value;
        }

        public static string GetText(string key)
        {
            return AequusText.Text["Mods.Aequus." + key].GetTranslation(Language.ActiveCulture);
        }
    }
}