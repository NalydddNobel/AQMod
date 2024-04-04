using Aequus.Common.Items.Components;
using Aequus.Items.Tools.Cameras.MapCamera;
using Aequus.Old.Content.Carpentry.Challenges.ActuatorDoor.Reward.Clip;

namespace Aequus.Old.Content.Carpentry.Challenges.ActuatorDoor.Reward;

public class PixelCamera : ModItem, ICooldownItem {
    public int CooldownTime => 600;

    public override void SetStaticDefaults() {
        ItemSets.GamepadExtraRange[Type] = 400;
    }

    public override void SetDefaults() {
        Item.width = 20;
        Item.height = 20;
        Item.rare = ItemRarityID.Green;
        Item.value = Item.buyPrice(gold: 5);
        Item.useAmmo = PixelCameraClipAmmo.AmmoID;
        Item.shoot = ModContent.ProjectileType<PixelCameraHeldProj>();
        Item.shootSpeed = 1f;
        Item.useTime = 28;
        Item.useAnimation = 28;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.channel = true;
        Item.noUseGraphic = true;
    }

    public override bool CanUseItem(Player player) {
        return !this.HasCooldown(player);
    }

    public override bool CanConsumeAmmo(Item ammo, Player player) {
        return false;
    }
}