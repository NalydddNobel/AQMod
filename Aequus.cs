using Aequus.Common.Configuration;
using Aequus.Common.Utilities;
using Aequus.Items;
using Aequus.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
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

        internal static Color GreenSlimeColor => ContentSamples.NpcsByNetId[NPCID.GreenSlime].color;
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

        public override void AddRecipes()
        {
            PlayerZombie.SetupBuffImmunities();
            ItemsCatalogue.LoadAutomaticEntries();
        }

        public override void Unload()
        {
            Instance = null;
            AequusHelpers.Main_dayTime = null;
            InventoryInterface = null;
            NPCTalkInterface = null;
        }
    }
}