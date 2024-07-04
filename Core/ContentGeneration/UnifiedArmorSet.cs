using AequusRemake.Core.Entities.Items.Components;
using AequusRemake.Core.Entities.Items.Tooltips;
using System;
using Terraria.Localization;

namespace AequusRemake.Core.ContentGeneration;

public abstract class UnifiedArmorSet : ModTexturedType, ILocalizedModType {
    public record struct ArmorStats(int Defense, int Rarity = ItemRarityID.White, int Value = 0, bool Vanity = false);
    public record struct Keyword(LocalizedText Name, LocalizedText Tooltip, int ShowcaseItem = ItemID.None, Color? TextColor = null);

    public UnifiedArmorSet() {
        _name = base.Name.Replace("Armor", "");
    }

    private readonly string _name;
    public override string Name => _name;

    public string LocalizationCategory => "Items.Armor";

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

internal abstract class InstancedArmor(UnifiedArmorSet armor, UnifiedArmorSet.ArmorStats stats, UnifiedArmorSet.Keyword keyword = default, string suffix = "", params object[] tooltipArguments) : InstancedModItem(armor.Name + suffix, armor.Texture + suffix), IAddKeywords {
    protected readonly UnifiedArmorSet _armor = armor;
    protected readonly UnifiedArmorSet.ArmorStats _stats = stats;
    protected readonly UnifiedArmorSet.Keyword _keyword = keyword;
    protected readonly string _suffix = suffix;
    protected readonly object[] _tooltipArgs = tooltipArguments;

    public override LocalizedText DisplayName => _armor.GetLocalization(_suffix + ".DisplayName", PrettyPrintName);
    public override LocalizedText Tooltip {
        get {
            var tip = _armor.GetLocalization(_suffix + ".Tooltip", () => "");
            return _tooltipArgs != null ? tip.WithFormatArgs(_tooltipArgs) : tip;
        }
    }

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

    public delegate bool Hook_IsArmorSet(Item head, Item body, Item legs);
    private Hook_IsArmorSet _isArmorSet;
    public InstancedArmor HookIsArmorSet(Hook_IsArmorSet IsArmorSet) {
        _isArmorSet += IsArmorSet;
        return this;
    }
    public override bool IsArmorSet(Item head, Item body, Item legs) {
        return _isArmorSet?.Invoke(head, body, legs) == true;
    }

    private Action<Item, Player> _updateArmorSet;
    public InstancedArmor HookUpdateArmorSet(Action<Item, Player> UpdateArmorSet) {
        _updateArmorSet += UpdateArmorSet;
        return this;
    }
    public override void UpdateArmorSet(Player player) {
        _updateArmorSet?.Invoke(Item, player);
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
internal class InstancedHelmet(UnifiedArmorSet armor, UnifiedArmorSet.ArmorStats stats, UnifiedArmorSet.Keyword keyword = default, string suffix = "", params object[] tooltipArguments) : InstancedArmor(armor, stats, keyword, "Helmet" + suffix, tooltipArguments) { }

[Autoload(false)]
[AutoloadEquip(EquipType.Body)]
internal class InstancedBody(UnifiedArmorSet armor, UnifiedArmorSet.ArmorStats stats, UnifiedArmorSet.Keyword keyword = default, string suffix = "", params object[] tooltipArguments) : InstancedArmor(armor, stats, keyword, "Chestplate" + suffix, tooltipArguments) { }

[Autoload(false)]
[AutoloadEquip(EquipType.Legs)]
internal class InstancedLegs(UnifiedArmorSet armor, UnifiedArmorSet.ArmorStats stats, UnifiedArmorSet.Keyword keyword = default, string suffix = "", params object[] tooltipArguments) : InstancedArmor(armor, stats, keyword, "Leggings" + suffix, tooltipArguments) { }