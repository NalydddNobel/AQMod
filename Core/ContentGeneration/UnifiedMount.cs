using Terraria.Audio;
using Terraria.Localization;

namespace AequusRemake.Core.ContentGeneration;

/// <summary>A <see cref="ModMount"/> which automagically registers a related <see cref="ModItem"/> and <see cref="ModBuff"/>, suffixed with +"Item" and +"Buff" respectively.</summary>
public abstract class UnifiedMount : ModMount, ILocalizedModType {
    public ModItem MountItem { get; private set; }
    public ModBuff MountBuff { get; private set; }

    public string LocalizationCategory => "Mounts";

    internal virtual ModBuff CreateMountBuff() {
        return new InstancedMountBuff(this);
    }
    internal virtual ModItem CreateMountItem() {
        return new InstancedMountItem(this);
    }

    public override void Load() {
        MountItem = CreateMountItem();
        MountBuff = CreateMountBuff();
        Mod.AddContent(MountItem);
        Mod.AddContent(MountBuff);
    }

    public override void SetStaticDefaults() {
        MountData.buff = MountBuff.Type;
    }
}

internal class InstancedMountBuff : InstancedModBuff {
    protected readonly UnifiedMount _parent;

    public InstancedMountBuff(UnifiedMount parent)
        : base(parent.Name, parent.NamespaceFilePath() + $"/{parent.Name.Replace("Mount", "")}Buff") {
        _parent = parent;
    }

    public override string LocalizationCategory => _parent.LocalizationCategory;
    public override LocalizedText DisplayName => _parent.GetLocalization("BuffDisplayName");
    public override LocalizedText Description => _parent.GetLocalization("BuffDescription");

    public override void SetStaticDefaults() {
        Main.buffNoTimeDisplay[Type] = true;
        Main.buffNoSave[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex) {
        player.mount.SetMount(_parent.Type, player);
        player.buffTime[buffIndex] = 10;
    }
}

internal class InstancedMountItem(UnifiedMount parent, int itemRarity = ItemRarityID.Yellow, int value = Item.gold * 5, SoundStyle? SoundOverride = null)
    : InstancedModItem(parent.Name + "Item", parent.NamespaceFilePath() + $"/{parent.Name.Replace("Mount", "")}Item") {
    [CloneByReference]
    private readonly UnifiedMount _parent = parent;
    private readonly int _rarity = itemRarity;
    private readonly int _value = value;
    private readonly SoundStyle _soundStyle = SoundOverride ?? SoundID.Item34;

    public override string LocalizationCategory => _parent.LocalizationCategory;
    public override LocalizedText DisplayName => _parent.GetLocalization("ItemDisplayName");
    public override LocalizedText Tooltip => _parent.GetLocalization("ItemTooltip");

    public override void SetDefaults() {
        Item.DefaultToMount(_parent.Type);
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTime = 15;
        Item.useAnimation = 15;
        Item.UseSound = _soundStyle;
        Item.rare = _rarity;
        Item.value = _value;
    }
}