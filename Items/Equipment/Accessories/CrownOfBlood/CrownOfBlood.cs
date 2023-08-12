using Aequus.Common.Items;
using Aequus.Common.Items.EquipmentBooster;
using Aequus.Common.Items.SentryChip;
using Aequus.Common.PlayerLayers.Equipment;
using Aequus.Common.UI;
using Aequus.Content.Items.SentryChip;
using Aequus.Items.Equipment.Accessories.CrownOfBlood.Buffs;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus.Items.Equipment.Accessories.CrownOfBlood;

[LegacyName("CrownOfBlood")]
public partial class CrownOfBloodItem : ModItem, ItemHooks.IUpdateItemDye {
    public const int AccessorySlot = 0;
    public const int ArmorSlot = Player.SupportedSlotsArmor + AccessorySlot;

    private EquipBoostInfo _info;

    public override void Load() {
        Load_ExpertEffects();
    }

    public override void Unload() {
    }

    public override void SetStaticDefaults() {
        SentryAccessoriesDatabase.Register<ApplyEquipFunctionalInteraction>(Type);
    }

    public override void SetDefaults() {
        Item.DefaultToAccessory(14, 20);
        Item.rare = ItemRarityID.LightPurple;
        Item.value = Item.buyPrice(gold: 10);
        Item.hasVanityEffects = true;
    }

    public override bool CanEquipAccessory(Player player, int slot, bool modded) {
        return slot > AccessorySlot;
    }

    private void ApplyAntiCheat(Player player, AequusPlayer aequusPlayer) {
        if (!aequusPlayer.accCrownOfBloodAntiCheat) {
            return;
        }
        aequusPlayer.CrownOfBloodHearts = Math.Max((player.statLifeMax - player.statLife) / player.HealthPerHeart(), aequusPlayer.CrownOfBloodHearts);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        _info ??= new(ArmorSlot);
        _info.Boost = EquipBoostType.Defense | EquipBoostType.Abilities;
        var aequusPlayer = player.Aequus();
        aequusPlayer.accCrownOfBlood = Item;

        var cloneItem = player.armor[ArmorSlot];
        aequusPlayer.accCrownOfBloodItemClone = cloneItem;
        if (cloneItem.accessory && cloneItem.TryGetGlobalItem<EquipBoostGlobalItem>(out var equipBoostGlobalItem)) {
            equipBoostGlobalItem.equipEmpowerment = _info;
        }
        player.AddBuff(aequusPlayer.CrownOfBloodHearts > 0 ? ModContent.BuffType<CrownOfBloodDebuff>() : ModContent.BuffType<CrownOfBloodBuff>(), 8);
        ApplyAntiCheat(player, aequusPlayer);
    }

    public void UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem) {
        if (!isSetToHidden || !isNotInVanitySlot) {
            var crown = player.Aequus().GetEquipDrawer<HoverCrownEquip>();
            crown.SetEquip(this, dyeItem);
            crown.CrownColor = Color.Red;
        }
    }

    private static bool CheckItemSlot(AequusUI.ItemSlotContext context) {
        return _equipAnimation <= 0f || context.Context != ItemSlot.Context.EquipAccessory || context.Slot != ArmorSlot;
    }
}