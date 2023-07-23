using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Tools.Cameras.MapCamera.Clip;
public class PixelCameraClipAmmo : ModItem {
    public static int AmmoID => ModContent.ItemType<PixelCameraClipAmmo>();

    public override string Texture => AequusTextures.PixelCameraClip.Path;

    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 5;
    }

    public override void SetDefaults() {
        Item.width = 16;
        Item.height = 16;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.buyPrice(gold: 1);
        Item.maxStack = Item.CommonMaxStack;
        Item.ammo = AmmoID;
        Item.consumable = true;
    }
}