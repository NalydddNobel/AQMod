using Aequus.Common.Items.Components;
using Aequus.Common.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Equipment.Accessories.Inventory.ScavengerBag;

public class ScavengerBagBackpackData : BackpackData {
    public override int Slots => ScavengerBag.SlotAmount;
    public override bool SupportsInfoAccessories => true;

    public override Color SlotColor => new Color(255, 200, 255, 255);

    public override bool IsActive(Player player, AequusPlayer aequusPlayer) {
        return aequusPlayer.accScavengerBag != null;
    }

    public override bool IsVisible() {
        return ModContent.GetInstance<ScavengerBagBuilderToggle>().CurrentState == 0;
    }

    public override string GetDisplayName(Player player) {
        if (player.TryGetModPlayer<AequusPlayer>(out var aequusPlayer) && aequusPlayer.accScavengerBag != null && aequusPlayer.accScavengerBag.TryGetGlobalItem<AequusItem>(out var aequusItem) && !string.IsNullOrEmpty(aequusItem.NameTag)) {
            return aequusItem.NameTag;
        }
        return base.GetDisplayName(player);
    }

    public override void Deactivate(Player player, AequusPlayer aequusPlayer) {
        for (int i = 0; i < Inventory.Length; i++) {
            Inventory[i].TurnToAir();
        }
    }

    public override void Activate(Player player, AequusPlayer aequusPlayer) {
        if (aequusPlayer.accScavengerBag == null || aequusPlayer.accScavengerBag.ModItem is not IStorageItem inventoryBackpack || inventoryBackpack.Inventory == null) {
            return;
        }

        inventoryBackpack.Deposit(Inventory);
    }

    protected override void OnResetEffects(Player player, AequusPlayer aequusPlayer) {
        if (aequusPlayer.accScavengerBag != null && Inventory != null && Active && activeOld) {
            if (aequusPlayer.accScavengerBag.ModItem is IStorageItem inventoryBackpack) {
                inventoryBackpack.Inherit(aequusPlayer.accScavengerBag, Inventory);
            }
            return;
        }
    }
}