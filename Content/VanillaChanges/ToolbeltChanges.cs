using Aequus.Common.Backpacks;
using Aequus.Content.Configuration;
using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace Aequus.Content.VanillaChanges;

public class ToolbeltChanges : GlobalItem {
    public override System.Boolean IsLoadingEnabled(Mod mod) {
        return VanillaChangesConfig.Instance.MoveToolbelt;
    }

    private static LocalizedText _tooltip;

    public static System.Int32 Capacity { get; set; } = 5;
    public static System.Single SlotHue { get; set; } = 0.4f;
    public static System.Int32 SpawnRate { get; set; } = 7;

    public override System.Boolean InstancePerEntity => true;
    protected override System.Boolean CloneNewInstances => true;

    public override System.Boolean AppliesToEntity(Item entity, System.Boolean lateInstantiation) {
        return entity.type == ItemID.Toolbelt;
    }

    public override void Load() {
        _tooltip = Language.GetOrRegister("Mods.Aequus.Items.Toolbelt.Tooltip").WithFormatArgs(Capacity);
    }

    public override void UpdateAccessory(Item item, Player player, System.Boolean hideVisual) {
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

        private System.String _nameCache;

        public override System.Int32 Capacity => ToolbeltChanges.Capacity;

        public override System.Single SlotHue => 0.275f;

        public override LocalizedText DisplayName => Language.GetOrRegister("ItemName.Toolbelt");

        public override System.String GetDisplayName(Player player) {
            if (Toolbelt != null) {
                _nameCache = Toolbelt.Name;
            }
            if (!System.String.IsNullOrEmpty(_nameCache)) {
                return _nameCache;
            }
            return base.GetDisplayName(player);
        }

        public override void ResetEffects(Player player) {
            Toolbelt = null;
        }

        public override System.Boolean IsActive(Player player) {
            return Toolbelt != null;
        }

        public override System.Boolean IsVisible() {
            return ModContent.GetInstance<ToolbeltBuilderToggle>().CurrentState == 0;
        }

        public override System.Boolean IsLoadingEnabled(Mod mod) {
            return VanillaChangesConfig.Instance.MoveToolbelt;
        }

        public override System.Boolean CanAcceptItem(System.Int32 slot, Item incomingItem) {
            return incomingItem.createTile > -1 || incomingItem.createWall > -1 || incomingItem.tileWand > -1;
        }

        public override void PostDrawSlot(SpriteBatch spriteBatch, Vector2 slotCenter, Vector2 slotTopLeft, System.Int32 slot) {
            if (Inventory[slot] != null && !Inventory[slot].IsAir) {
                return;
            }

            Main.GetItemDrawFrame(ItemID.GrayBrickWall, out Texture2D itemTexture, out Rectangle frame);
            spriteBatch.Draw(itemTexture, slotCenter, frame, Color.White with { A = 150 } * 0.5f, 0f, frame.Size() / 2f, Main.inventoryScale, SpriteEffects.None, 0f);
        }
    }
    public class ToolbeltBuilderToggle : BuilderToggle {
        private LocalizedText _on;
        private LocalizedText _off;

        public override System.Boolean Active() {
            return BackpackLoader.Get<ToolbeltBackpack>(Main.LocalPlayer).IsActive(Main.LocalPlayer);
        }

        public override System.String DisplayValue() {
            return (CurrentState == 0 ? _on : _off).Value;
        }

        public override Color DisplayColorTexture() {
            return CurrentState == 0 ? Color.White : Color.Gray;
        }

        public override void Load() {
            _on = Language.GetOrRegister("Mods.Aequus.Items.Toolbelt.BackpackEnabled");
            _off = Language.GetOrRegister("Mods.Aequus.Items.Toolbelt.BackpackDisabled");
        }

        public override System.Boolean IsLoadingEnabled(Mod mod) {
            return VanillaChangesConfig.Instance.MoveToolbelt;
        }
    }
}