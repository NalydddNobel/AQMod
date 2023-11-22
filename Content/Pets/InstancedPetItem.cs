using Aequus.Common.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Content.Pets;

[Autoload(false)]
internal class InstancedPetItem : InstancedModItem {
    private readonly ModPet _modPet;
    private readonly int _rarity;
    private readonly int _value;
    private readonly Color? _alphaOverride;

    public InstancedPetItem(ModPet modPet, int itemRarity = ItemRarityID.Orange, int value = Item.gold * 5, Color? alphaOverride = null) : base(modPet.Name + "Item", modPet.NamespaceFilePath() + $"/{modPet.Name}Item") {
        _modPet = modPet;
        _rarity = itemRarity;
        _value = value;
        _alphaOverride = alphaOverride;
    }

    public override string LocalizationCategory => "Pets";

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

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
        player.AddBuff(Item.buffType, 2);
        return true;
    }
}