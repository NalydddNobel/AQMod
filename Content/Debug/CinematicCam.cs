using Aequus.Common.Effects;
using Aequus.Common.Items;

namespace Aequus.Content.Debug;
internal class CinematicCam : ModItem {
    public override string Texture => AequusTextures.Gamestar.FullPath;

    public override bool IsLoadingEnabled(Mod mod) {
        return Aequus.DevelopmentFeatures;
    }

    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 0;
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