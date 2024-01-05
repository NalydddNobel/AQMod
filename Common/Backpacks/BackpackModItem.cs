using Terraria.Localization;

namespace Aequus.Common.Backpacks;

public abstract class BackpackModItem : ModItem {
    public abstract int Capacity { get; set; }
    public abstract Color SlotColor { get; set; }

    [CloneByReference]
    private ItemBackpackInstance _backpack;

    protected override bool CloneNewInstances => true;

    public override void Load() {
        _backpack = new ItemBackpackInstance(this);
        Mod.AddContent(_backpack);
    }

    public override void SetDefaults() {
        Item.DefaultToAccessory();
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        if (!player.TryGetModPlayer(out BackpackPlayer backpackPlayer)) {
            return;
        }
        ((ItemBackpackInstance)backpackPlayer.backpacks[_backpack.Type]).BackpackItem = this;
    }

    [Autoload(false)]
    private class ItemBackpackInstance : BackpackData {
        public readonly BackpackModItem _parentItem;
        public BuilderToggle _toggle;
        public BackpackModItem BackpackItem;

        private string _nameCache;

        public override LocalizedText DisplayName => _parentItem.DisplayName;

        public override int Capacity => BackpackItem?.Capacity ?? 0;

        public override bool SupportsInfoAccessories => true;

        public override Color SlotColor => BackpackItem?.SlotColor ?? _parentItem.SlotColor;

        public override string Name => _parentItem.Name;

        public ItemBackpackInstance(BackpackModItem parentItem) {
            _parentItem = parentItem;
        }

        public override string GetDisplayName(Player player) {
            if (BackpackItem != null) {
                _nameCache = BackpackItem.Item.Name;
            }
            if (!string.IsNullOrEmpty(_nameCache)) {
                return _nameCache;
            }
            return base.GetDisplayName(player);
        }

        public override void ResetEffects(Player player) {
            BackpackItem = null;
        }

        public override bool IsActive(Player player) {
            return BackpackItem != null;
        }

        public override bool IsVisible() {
            return _toggle.CurrentState == 0;
        }

        public override void Load() {
            _toggle = new BackpackBuilderToggle(_parentItem, this);
            Mod.AddContent(_toggle);
        }

        public override BackpackData CreateInstance() {
            return base.CreateInstance();
        }
    }

    [Autoload(false)]
    private class BackpackBuilderToggle : BuilderToggle {
        private readonly BackpackData _backpack;
        private readonly BackpackModItem _backpackItem;

        public override string Name => _backpack.Name;

        public override string Texture => _backpackItem.Texture + "BuilderToggle";

        public BackpackBuilderToggle(BackpackModItem backpackItem, BackpackData backpack) {
            _backpack = backpack;
            _backpackItem = backpackItem;
        }

        protected LocalizedText BuilderSlotTextOnCache { get; private set; }
        protected LocalizedText BuilderSlotTextOffCache { get; private set; }

        public override void Load() {
            BuilderSlotTextOnCache = _backpackItem.GetLocalization("BackpackEnabled", () => "Backpack On");
            BuilderSlotTextOffCache = _backpackItem.GetLocalization("BackpackDisabled", () => "Backpack Off");
        }

        public override bool Active() {
            return BackpackLoader.Get(Main.LocalPlayer, _backpack).IsActive(Main.LocalPlayer);
        }

        public override string DisplayValue() {
            return (CurrentState == 0 ? BuilderSlotTextOnCache : BuilderSlotTextOffCache).Value;
        }

        public override Color DisplayColorTexture() {
            return CurrentState == 0 ? Color.White : Color.Gray;
        }
    }
}
