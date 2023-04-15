using Aequus.Common.Effects;
using Aequus.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Unused.Items.DebugItems {
    internal class CinematicCam : ModItem {
        public override string Texture => AequusTextures.Gamestar.Path;

        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 0;

            /* Tooltip.SetDefault(
                """
                Use to set camera to your cursor
                Use again to reset camera
                Testing Item
                """); */
        }

        public override void SetDefaults() {
            Item.DefaultToHoldUpItem();
            Item.useTime = 2;
            Item.useAnimation = 2;
            Item.rare = ItemRarityID.Red;
            Item.width = 20;
            Item.height = 20;
            Item.color = Main.OurFavoriteColor;
        }

        public override bool? UseItem(Player player) {
            int x = Helper.MouseTileX;
            int y = Helper.MouseTileY;

            var camera = ModContent.GetInstance<CameraFocus>();
            if (camera.hold <= 10) {
                foreach (var g in Main.gore) {
                    g.active = false;
                }
                camera.SetTarget("Test", Main.MouseWorld, CameraPriority.VeryImportant, hold: int.MaxValue - 100);
            }
            else {
                camera.hold = 2;
            }

            return true;
        }
    }
}