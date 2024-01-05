using Aequus.Common.Backpacks;
using Aequus.Content.Configuration;
using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace Aequus.Content.VanillaChanges;

internal class ToolbeltChanges : GlobalItem {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaChangesConfig.Instance.MoveToolbelt;
    }

    private static LocalizedText _tooltip;

    public static int Capacity { get; set; } = 5;
    public static Color SlotColor { get; set; } = new Color(255, 200, 230);
    public static int SpawnRate { get; set; } = 7;

    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
        return entity.type == ItemID.Toolbelt;
    }

    public override void Load() {
        _tooltip = Language.GetOrRegister("Mods.Aequus.Items.Toolbelt.Tooltip").WithFormatArgs(Capacity);
    }

    public override void UpdateAccessory(Item item, Player player, bool hideVisual) {
        if (!player.TryGetModPlayer(out BackpackPlayer backpackPlayer)) {
            return;
        }
        BackpackLoader.Get<ToolbeltBackpack>(backpackPlayer).Toolbelt = item;
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
        tooltips.AddTooltip(new TooltipLine(Mod, "AequusTooltip", _tooltip.Value));
    }

    public override void ModifyItemLoot(Item item, ItemLoot itemLoot) {
        if (item.type == ItemID.IronCrate) {
            itemLoot.Add(ItemDropRule.Common(ItemID.Toolbelt, chanceDenominator: SpawnRate * 3));
        }
    }

    public class ToolbeltBackpack : BackpackData {
        public Item Toolbelt { get; internal set; }

        private string _nameCache;

        public override int Capacity => ToolbeltChanges.Capacity;

        public override Color SlotColor => new Color(255, 180, 250);

        public override LocalizedText DisplayName => Language.GetOrRegister("ItemName.Toolbelt");

        public override string GetDisplayName(Player player) {
            if (Toolbelt != null) {
                _nameCache = Toolbelt.Name;
            }
            if (!string.IsNullOrEmpty(_nameCache)) {
                return _nameCache;
            }
            return base.GetDisplayName(player);
        }

        public override void ResetEffects(Player player) {
            Toolbelt = null;
        }

        public override bool IsActive(Player player) {
            return Toolbelt != null;
        }

        public override bool IsVisible() {
            return ModContent.GetInstance<ToolbeltBuilderToggle>().CurrentState == 0;
        }

        public override bool IsLoadingEnabled(Mod mod) {
            return VanillaChangesConfig.Instance.MoveToolbelt;
        }
    }
    public class ToolbeltBuilderToggle : BuilderToggle {
        private LocalizedText _on;
        private LocalizedText _off;

        public override bool Active() {
            return BackpackLoader.Get<ToolbeltBackpack>(Main.LocalPlayer).IsActive(Main.LocalPlayer);
        }

        public override string DisplayValue() {
            return (CurrentState == 0 ? _on : _off).Value;
        }

        public override Color DisplayColorTexture() {
            return CurrentState == 0 ? Color.White : Color.Gray;
        }

        public override void Load() {
            _on = Language.GetOrRegister("Mods.Aequus.Items.Toolbelt.BackpackEnabled");
            _off = Language.GetOrRegister("Mods.Aequus.Items.Toolbelt.BackpackDisabled");
        }

        public override bool IsLoadingEnabled(Mod mod) {
            return VanillaChangesConfig.Instance.MoveToolbelt;
        }
    }
}