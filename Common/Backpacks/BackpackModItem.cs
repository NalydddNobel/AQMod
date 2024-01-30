using Terraria.Localization;

namespace Aequus.Common.Backpacks;

public abstract class BackpackModItem : ModItem {
    public abstract System.Int32 Capacity { get; set; }
    public abstract System.Single SlotHue { get; set; }

    [CloneByReference]
    private ItemBackpackInstance _backpack;

    protected override System.Boolean CloneNewInstances => true;

    public override void Load() {
        _backpack = new ItemBackpackInstance(this);
        Mod.AddContent(_backpack);
    }

    public override void SetDefaults() {
        Item.DefaultToAccessory();
    }

    public override void UpdateAccessory(Player player, System.Boolean hideVisual) {
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

        private System.String _nameCache;

        public override LocalizedText DisplayName => _parentItem.DisplayName;

        public override System.Int32 Capacity => BackpackItem?.Capacity ?? 0;

        public override System.Boolean SupportsInfoAccessories => true;

        public override System.Single SlotHue => BackpackItem?.SlotHue ?? _parentItem.SlotHue;

        public override System.String Name => _parentItem.Name;

        public ItemBackpackInstance(BackpackModItem parentItem) {
            _parentItem = parentItem;
        }

        public override System.String GetDisplayName(Player player) {
            if (BackpackItem != null) {
                _nameCache = BackpackItem.Item.Name;
            }
            if (!System.String.IsNullOrEmpty(_nameCache)) {
                return _nameCache;
            }
            return base.GetDisplayName(player);
        }

        public override void ResetEffects(Player player) {
            BackpackItem = null;
        }

        public override System.Boolean IsActive(Player player) {
            return BackpackItem != null;
        }

        public override System.Boolean IsVisible() {
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

        public override System.String Name => _backpack.Name;

        public override System.String Texture => _backpackItem.Texture + "BuilderToggle";

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

        public override System.Boolean Active() {
            return BackpackLoader.Get(Main.LocalPlayer, _backpack).IsActive(Main.LocalPlayer);
        }

        public override System.String DisplayValue() {
            return (CurrentState == 0 ? BuilderSlotTextOnCache : BuilderSlotTextOffCache).Value;
        }

        public override Color DisplayColorTexture() {
            return CurrentState == 0 ? Color.White : Color.Gray;
        }
    }
}
