using Aequus.Common.DataSets;
using Aequus.Common.Items;
using Aequus.Common.Items.Tooltips;
using Aequus.CrossMod.ThoriumModSupport;
using Aequus.Items.Materials.Energies;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace Aequus.Items.Equipment.Armor.SetAetherial {
    public abstract class AetherialArmorPiece : ModItem, IAddSpecialTooltips, ItemHooks.IHoverSlot {
        protected const string ThoriumTipKey = "ThoriumModTooltip";
        protected static bool iterating;

        public int armorSetChecks;
        public int pretendToBeItem;
        protected Item _itemCache;

        protected int head;
        protected int body;
        protected int legs;

        public abstract List<Item> ArmorList { get; }

        protected virtual int LuminiteRecipeStack => Math.Max(FragmentRecipeStack - 2, 1);
        protected abstract int FragmentRecipeStack { get; }

        public abstract int EquipSlot { get; }

        public abstract ref int GetEquipSlotField(Item item);

        public abstract ref int GetEquipSlotField(Player player);

        protected override bool CloneNewInstances => true;

        protected virtual LocalizedText GetAetherialArmorFormatArg(string key) {
            return this.GetLocalization(key);
        }

        protected object[] GetFormatArgs(params object[] args) {
            int index = (args?.Length).GetValueOrDefault(0);
            object[] resultArgs = new object[index + 1];
            for (int i = 0; i < index; i++) {
                resultArgs[i] = args[i];
            }
            if (ThoriumMod.Instance != null) {
                resultArgs[index++] = GetAetherialArmorFormatArg(ThoriumTipKey);
            }
            return resultArgs;
        }

        private void ClearSlotCaches() {
            head = -1;
            body = -1;
            legs = -1;
        }

        public override void SetDefaults() {
            ClearSlotCaches();
            _itemCache = null;
        }

        #region Ability
        private bool CheckForOtherAetherialPieces(Player player) {
            int count = 0;
            for (int i = 0; i < 3; i++) {
                if (player.armor[i].ModItem is AetherialArmorPiece) {
                    count++;
                }
            }
            return count <= 1;
        }

        private void CheckSetbonus(Player player) {
            pretendToBeItem = 0;
            int foundSetbonuses = 0;
            int equipSlot = EquipSlot;
            foreach (var armor in ArmorList) {
                Item item = new(armor.type);
                item.Prefix(Item.prefix);
                item.defense = 0;

                player.armor[equipSlot] = item;
                GetEquipSlotField(player) = GetEquipSlotField(item);
                player.UpdateArmorSets(player.whoAmI);
                player.armor[equipSlot] = Item;

                if (!string.IsNullOrEmpty(player.setBonus)) {
                    _itemCache = item;
                    pretendToBeItem = armor.type;
                    foundSetbonuses++;
                }

                if (foundSetbonuses > armorSetChecks)
                    return;
            }

            if (foundSetbonuses != armorSetChecks + 1) {
                armorSetChecks = 0;
                ClearSlotCaches();
            }
        }

        private void InnerCheckForRefresh(Player player, int slot, ref int equipSlotCache, ref bool updateSet) {
            if (player.armor[slot] == null || player.armor[slot].IsAir) {
                equipSlotCache = -1;
            }
            else if (player.armor[slot].type != equipSlotCache) {
                updateSet = true;
                equipSlotCache = player.armor[slot].type;
            }
        }

        private void CheckForRefresh(Player player) {
            bool updateSet = false;
            InnerCheckForRefresh(player, 0, ref head, ref updateSet);
            InnerCheckForRefresh(player, 1, ref body, ref updateSet);
            InnerCheckForRefresh(player, 2, ref legs, ref updateSet);
            if (updateSet) {
                CheckSetbonus(player);
            }
        }

        public void SetArmorPiece(Player player) {
            if (_itemCache == null) {
                _itemCache = new(pretendToBeItem);
                _itemCache.Prefix(Item.prefix);
                _itemCache.defense = 0;
            }
            GetEquipSlotField(player) = GetEquipSlotField(_itemCache);
            player.armor[EquipSlot] = _itemCache;
        }

        public override void UpdateEquip(Player player) {
            if (pretendToBeItem > 0 && !iterating) {
                try {
                    SetArmorPiece(player);
                    iterating = true;
                    iterating = false;
                }
                catch {
                }
                player.armor[EquipSlot] = Item;
            }
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) {
            return true;
        }

        public override void UpdateArmorSet(Player player) {
            if (!CheckForOtherAetherialPieces(player)) {
                pretendToBeItem = 0;
                _itemCache = null;
                return;
            }

            if (iterating)
                return;

            iterating = true;
            try {
                CheckForRefresh(player);
                if (pretendToBeItem > 0 && _itemCache != null) {
                    player.armor[EquipSlot] = _itemCache;
                    try {
                        SetArmorPiece(player);
                        player.ApplyEquipFunctional(player.armor[EquipSlot], false);
                        player.UpdateArmorSets(player.whoAmI);
                    }
                    catch {
                    }
                    player.armor[EquipSlot] = Item;
                }
            }
            catch {
            }
            iterating = false;
        }
        #endregion

        public override void AddRecipes() {
            var r = CreateRecipe()
                .AddIngredient(ItemID.LunarBar, LuminiteRecipeStack)
                .AddIngredient<UltimateEnergy>(1);

            var fragments = ItemSets.OrderedPillarFragments_ByColor;
            int stack = FragmentRecipeStack;

            for (int i = 0; i < fragments.Count; i++) {
                r.AddIngredient(fragments[i], stack);
            }

            r.AddTile(TileID.LunarCraftingStation)
                .TryRegisterAfter(ItemID.CelestialSigil);
        }

        public override ModItem Clone(Item newEntity) {
            var clone = (AetherialArmorPiece)base.Clone(newEntity);
            if (_itemCache != null) {
                clone._itemCache = _itemCache.Clone();
            }
            return clone;
        }

        public override void NetSend(BinaryWriter writer) {
            writer.Write((byte)armorSetChecks);
        }

        public override void NetReceive(BinaryReader reader) {
            armorSetChecks = reader.ReadByte();
        }

        public override void SaveData(TagCompound tag) {
            if (armorSetChecks > 0) {
                tag["ArmorSetChecks"] = armorSetChecks;
            }
        }

        public override void LoadData(TagCompound tag) {
            if (tag.TryGet("ArmorSetChecks", out int armorSetChecks)) {
                this.armorSetChecks = armorSetChecks;
            }
        }

        public bool HoverSlot(Item[] inventory, int context, int slot) {
            if (context != ItemSlot.Context.EquipArmor) {
                ClearSlotCaches();
                _itemCache = null;
                pretendToBeItem = 0;
                return false;
            }
            if (Main.mouseRight && Main.mouseRightRelease) {
                SoundEngine.PlaySound(SoundID.Grab);
                armorSetChecks++;
                ClearSlotCaches();
                return true;
            }
            return false;
        }

        public virtual void AddSpecialTooltips(List<SpecialAbilityTooltipInfo> tooltips) {
            if (!CheckForOtherAetherialPieces(Main.LocalPlayer)) {
                return;
            }

            SpecialAbilityTooltipInfo tooltip = new(TextHelper.GetTextValue("Items.AetherialArmor.ArmorMatching"), Color.Lerp(Color.Orange, Color.White, 0.8f), Type);
            tooltip.AddLine(TextHelper.GetTextValue("Items.AetherialArmor.SpecialTooltip"));
            tooltips.Add(tooltip);

            if (pretendToBeItem > 0) {
                tooltip = new(TextHelper.GetTextValue("Items.AetherialArmor.Setbonus", Lang.GetItemName(pretendToBeItem).Value), Color.Lerp(Color.Lime, Color.White, 0.8f), pretendToBeItem);
                //tooltip.AddLine(TextHelper.GetTextValue("Items.AetherialArmor.EquippedAs", Lang.GetItemName(pretendToBeItem).Value));
                var itemTooltip = Lang.GetTooltip(pretendToBeItem);
                for (int i = 0; i < itemTooltip.Lines; i++) {
                    tooltip.AddLine(itemTooltip.GetLine(i));
                }
                tooltips.Add(tooltip);
            }
        }
    }
}
