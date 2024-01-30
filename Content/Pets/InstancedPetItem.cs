using Aequus.Common.Items;
using Terraria.DataStructures;
using Terraria.Localization;

namespace Aequus.Content.Pets;

[Autoload(false)]
internal class InstancedPetItem : InstancedModItem {
    private readonly ModPet _modPet;
    private readonly System.Int32 _rarity;
    private readonly System.Int32 _value;
    private readonly Color? _alphaOverride;

    public InstancedPetItem(ModPet modPet, System.Int32 itemRarity = ItemRarityID.Orange, System.Int32 value = Item.gold * 5, Color? alphaOverride = null) : base(modPet.Name + "Item", modPet.NamespaceFilePath() + $"/{modPet.Name}Item") {
        _modPet = modPet;
        _rarity = itemRarity;
        _value = value;
        _alphaOverride = alphaOverride;
    }

    public override System.String LocalizationCategory => "Pets";

    public override LocalizedText DisplayName => Language.GetOrRegister(_modPet.GetLocalizationKey("ItemDisplayName"));
    public override LocalizedText Tooltip => Language.GetOrRegister(_modPet.GetLocalizationKey("ItemTooltip"));

    public override void SetDefaults() {
        Item.DefaultToVanitypet(_modPet.Type, _modPet.PetBuff.Type);
        Item.width = 20;
        Item.height = 20;
        Item.value = _value;
        Item.rare = _rarity;
        Item.master = _rarity == ItemRarityID.Master;
    }

    public override Color? GetAlpha(Color lightColor) {
        return _alphaOverride;
    }

    public override System.Boolean Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, System.Int32 type, System.Int32 damage, System.Single knockback) {
        player.AddBuff(Item.buffType, 2);
        return true;
    }
}