using AequusRemake.Content.Configuration;
using AequusRemake.Systems.Backpacks;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace AequusRemake.Systems.VanillaChanges;

public class ToolbeltChanges : GlobalItem {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaChangesConfig.Instance.MoveToolbelt;
    }

    private static LocalizedText _tooltip;

    public static int Capacity { get; set; } = 5;
    public static float SlotHue { get; set; } = 0.4f;

    public override bool InstancePerEntity => true;
    protected override bool CloneNewInstances => true;

    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
        return entity.type == ItemID.Toolbelt;
    }

    public override void Load() {
        _tooltip = Language.GetOrRegister("Mods.AequusRemake.Items.Toolbelt.Tooltip").WithFormatArgs(Capacity);
    }

    public override void SetDefaults(Item entity) {
        entity.StatsModifiedBy.Add(Mod);
    }

    public override void UpdateAccessory(Item item, Player player, bool hideVisual) {
        if (!player.TryGetModPlayer(out BackpackPlayer backpackPlayer)) {
            return;
        }
        BackpackLoader.GetPlayerInstance<ToolbeltBackpack>(backpackPlayer).Toolbelt = item;
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
        tooltips.AddTooltip(new TooltipLine(Mod, "AequusRemakeTooltip", _tooltip.Value));
    }

    public override void ModifyItemLoot(Item item, ItemLoot itemLoot) {
        if (item.type == ItemID.IronCrate) {
            itemLoot.Add(ItemDropRule.Common(ItemID.Toolbelt, chanceDenominator: 12));
        }
    }

    public class ToolbeltBackpack : BackpackData {
        public Item Toolbelt { get; internal set; }

        private string _nameCache;

        public override int Capacity => ToolbeltChanges.Capacity;

        public override float SlotHue => 0.275f;

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

        public override bool CanAcceptItem(int slot, Item incomingItem) {
            return incomingItem.createTile > -1 || incomingItem.createWall > -1 || incomingItem.tileWand > -1;
        }

        public override void PostDrawSlot(SpriteBatch spriteBatch, Vector2 slotCenter, Vector2 slotTopLeft, int slot) {
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

        public override bool Active() {
            return BackpackLoader.GetPlayerInstance<ToolbeltBackpack>(Main.LocalPlayer).IsActive(Main.LocalPlayer);
        }

        public override string DisplayValue() {
            return (CurrentState == 0 ? _on : _off).Value;
        }

        public override bool Draw(SpriteBatch spriteBatch, ref BuilderToggleDrawParams drawParams) {
            drawParams.Color = CurrentState == 0 ? Color.White : Color.Gray;
            return true;
        }

        public override void Load() {
            _on = Language.GetOrRegister("Mods.AequusRemake.Items.Toolbelt.BackpackEnabled");
            _off = Language.GetOrRegister("Mods.AequusRemake.Items.Toolbelt.BackpackDisabled");
        }

        public override bool IsLoadingEnabled(Mod mod) {
            return VanillaChangesConfig.Instance.MoveToolbelt;
        }
    }
}