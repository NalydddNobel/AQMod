using Aequus.Common.Items;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Common.Bosses;

[Autoload(false)]
[AutoloadEquip(EquipType.Head)]
public class BossMask : ManualLoadItem {
    private readonly ModNPC _bossInstance;

    public BossMask(ModNPC boss) {
        _bossInstance = boss;
        _internalName = $"{boss.Name}Mask";
        _texturePath = $"Aequus/Content/Bosses/{boss.Name}/Items/{_internalName}";
    }

    public override LocalizedText DisplayName => this.GetLocalization("DisplayName", () => $"{_bossInstance.DisplayName.Value} Mask");
    public override LocalizedText Tooltip => LocalizedText.Empty;

    public override void SetDefaults() {
        Item.DefaultToHeadgear(16, 16, Item.headSlot);
        Item.rare = ItemCommons.Rarity.bossMasks;
        Item.vanity = true;
    }
}