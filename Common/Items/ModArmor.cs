using Aequus.Common.Items.Components;
using Aequus.Common.Items.Tooltips;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Common.Items;

public abstract class ModArmor : ModTexturedType, ILocalizedModType {
    public record struct ArmorStats(int Defense, int Rarity = ItemRarityID.White, int Value = 0, bool Vanity = false);
    public record struct Keyword(LocalizedText Name, LocalizedText Tooltip, int ShowcaseItem = ItemID.None, Color? TextColor = null);

    public ModArmor() {
        _name = base.Name.Replace("Armor", "");
    }

    private readonly string _name;
    public override string Name => _name;

    public string LocalizationCategory => "Armor";

    protected sealed override void Register() {
    }

    public sealed override void SetupContent() {
        SetStaticDefaults();
    }

    internal void AddArmor(InstancedArmor inArmor, out InstancedArmor outArmor) {
        outArmor = inArmor;
        Mod.AddContent(inArmor);
    }
}

internal abstract class InstancedArmor : InstancedModItem, IAddKeywords {
    protected readonly ModArmor _armor;
    protected readonly ModArmor.ArmorStats _stats;
    protected readonly ModArmor.Keyword _keyword;
    protected readonly string _suffix;

    public InstancedArmor(ModArmor armor, ModArmor.ArmorStats stats, ModArmor.Keyword keyword = default, string suffix = "") : base(armor.Name + suffix, armor.Texture + suffix) {
        _armor = armor;
        _stats = stats;
        _keyword = keyword;
        _suffix = suffix;
    }

    public override LocalizedText DisplayName => _armor.GetLocalization(_suffix + ".DisplayName", PrettyPrintName);
    public override LocalizedText Tooltip => _armor.GetLocalization(_suffix + ".Tooltip", () => "");

    private Action<Item> _setStaticDefaults;
    public InstancedArmor HookSetStaticDefaults(Action<Item> SetStaticDefaults) {
        _setStaticDefaults += SetStaticDefaults;
        return this;
    }
    public override void SetStaticDefaults() {
        _setStaticDefaults?.Invoke(Item);
    }

    private Action<Item> _setDefaults;
    public InstancedArmor HookSetDefaults(Action<Item> SetDefaults) {
        _setDefaults += SetDefaults;
        return this;
    }
    public override void SetDefaults() {
        Item.width = 18;
        Item.height = 18;
        Item.defense = _stats.Defense;
        Item.rare = _stats.Rarity;
        Item.value = _stats.Value;
        Item.vanity = _stats.Vanity;
        _setDefaults?.Invoke(Item);
    }

    private Action<Item, Player> _updateEquip;
    public InstancedArmor HookUpdateEquip(Action<Item, Player> UpdateEquip) {
        _updateEquip += UpdateEquip;
        return this;
    }
    public override void UpdateEquip(Player player) {
        _updateEquip?.Invoke(Item, player);
    }

    private Action<Item, Player> _updateVanity;
    public InstancedArmor HookUpdateVanity(Action<Item, Player> UpdateVanity) {
        _updateVanity += UpdateVanity;
        return this;
    }
    public override void UpdateVanity(Player player) {
        _updateVanity?.Invoke(Item, player);
    }

    private Action<Item, Player> _armorSetShadows;
    public InstancedArmor HookArmorSetShadows(Action<Item, Player> ArmorSetShadows) {
        _armorSetShadows += ArmorSetShadows;
        return this;
    }
    public override void ArmorSetShadows(Player player) {
        _armorSetShadows?.Invoke(Item, player);
    }

    public delegate bool Hook_IsVanitySet(Item item, int head, int body, int legs);
    private Hook_IsVanitySet _isVanitySet;
    public InstancedArmor HookIsVanitySet(Hook_IsVanitySet IsVanitySet) {
        _isVanitySet += IsVanitySet;
        return this;
    }
    public override bool IsVanitySet(int head, int body, int legs) {
        return _isVanitySet?.Invoke(Item, head, body, legs) == true;
    }

    public void AddSpecialTooltips() {
        if (_keyword.Tooltip == null) {
            return;
        }

        var keyword = new Keyword(_keyword.Name?.Value, _keyword.TextColor ?? Color.White, _keyword.ShowcaseItem);
        keyword.AddLine(_keyword.Tooltip.Value);
        KeywordSystem.Tooltips.Add(keyword);
    }
}

[Autoload(false)]
[AutoloadEquip(EquipType.Head)]
internal class InstancedHelmet : InstancedArmor {
    public InstancedHelmet(ModArmor armor, ModArmor.ArmorStats stats, ModArmor.Keyword keyword = default, string suffix = "") : base(armor, stats, keyword, "Helmet" + suffix) {
    }
}

[Autoload(false)]
[AutoloadEquip(EquipType.Body)]
internal class InstancedBody : InstancedArmor {
    public InstancedBody(ModArmor armor, ModArmor.ArmorStats stats, ModArmor.Keyword keyword = default, string suffix = "") : base(armor, stats, keyword, "Chestplate" + suffix) {
    }
}

[Autoload(false)]
[AutoloadEquip(EquipType.Legs)]
internal class InstancedLegs : InstancedArmor {
    public InstancedLegs(ModArmor armor, ModArmor.ArmorStats stats, ModArmor.Keyword keyword = default, string suffix = "") : base(armor, stats, keyword, "Leggings" + suffix) {
    }
}