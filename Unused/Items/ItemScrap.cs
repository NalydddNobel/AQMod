using Aequus.Common;
using Aequus.Common.Items;
using Aequus.Items;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Unused.Items {
    [UnusedContent]
    public class ItemScrap : ModItem, IPostAddRecipes {
        public static HashSet<int> ScrappableRarities { get; private set; }
        public static object Hook_RecipeLoader_ConsumeItem { get; private set; }

        public int Rarity;

        public override void Load() {
            ScrappableRarities = new HashSet<int>();
        }

        public override void Unload() {
            Hook_RecipeLoader_ConsumeItem = null;
            ScrappableRarities?.Clear();
            ScrappableRarities = null;
        }

        public override void SetDefaults() {
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = Item.CommonMaxStack;
            UpdateRarity();
        }

        public void UpdateRarity() {
            Item.rare = Rarity;
        }

        public override bool CanStack(Item item2) {
            return Rarity == item2.ModItem<ItemScrap>().Rarity;
        }

        public override bool CanStackInWorld(Item item2) {
            return Rarity == item2.ModItem<ItemScrap>().Rarity;
        }

        public override void SaveData(TagCompound tag) {
            if (Rarity < ItemRarityID.Count) {
                tag["RarityVanilla"] = Rarity;
                return;
            }
            tag["RarityMod"] = RarityLoader.GetRarity(Rarity).FullName;
        }

        public override void LoadData(TagCompound tag) {
            Rarity = ItemRarityID.White;
            UpdateRarity();
            if (tag.TryGet("RarityVanilla", out Rarity)) {
                return;
            }
            if (!tag.TryGet("RarityMod", out string value)) {
                return;
            }
            var split = value.Split('/');
            if (!ModLoader.TryGetMod(split[0], out var mod)
                || !mod.TryFind<ModRarity>(split[1], out var modRarity)) {
                return;
            }
            Rarity = modRarity.Type;
            UpdateRarity();
        }

        public override void UpdateInventory(Player player) {
            UpdateRarity();
        }

        public override void Update(ref float gravity, ref float maxFallSpeed) {
            UpdateRarity();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) {
            foreach (var t in tooltips) {
                if (t.Mod == "Terraria" && t.Name.StartsWith("Tooltip")) {
                    t.Text = t.Text.FormatWith(new { ItemRarity = TextHelper.GetRarityNameValue(Rarity), });
                }
            }
        }

        public override void AddRecipes() {
            var rarityCounts = new Dictionary<int, int>();
            for (int i = 0; i < ItemLoader.ItemCount; i++) {
                var item = AequusItem.SetDefaults(i, checkMaterial: false);

                if (rarityCounts.ContainsKey(item.rare)) {
                    rarityCounts[item.rare] = rarityCounts[item.rare] + 1;
                    continue;
                }
                rarityCounts.Add(item.rare, 1);
            }

            foreach (var r in rarityCounts) {
                if (r.Value > 5) {
                    ScrappableRarities.Add(r.Key);
                }
            }
        }

        public void ScrapCheck(Recipe recipe, int type, ref int amount) {
            if (amount > 0) {
                var inv = Main.LocalPlayer.inventory;
                int? myRare = null;
                for (int i = 0; i < Main.InventorySlotsTotal; i++) {
                    if (!inv[i].IsAir && inv[i].type == ModContent.ItemType<ItemScrap>()) {
                        if (myRare == null) {
                            myRare = AequusItem.SetDefaults(type).rare;
                        }
                        ScrapCheck_Inner(inv[i], recipe, type, myRare.Value, ref amount);
                    }
                }
                inv = Main.LocalPlayer.bank4.item;
                for (int i = 0; i < Chest.maxItems; i++) {
                    if (!inv[i].IsAir && inv[i].type == ModContent.ItemType<ItemScrap>()) {
                        if (myRare == null) {
                            myRare = AequusItem.SetDefaults(type).rare;
                        }
                        ScrapCheck_Inner(inv[i], recipe, type, myRare.Value, ref amount);
                    }
                }
            }
        }

        public void ScrapCheck_Inner(Item item, Recipe recipe, int type, int myRare, ref int amount) {
            if (ItemHelper.CanBeCraftedInto(recipe.createItem.type, type)) {
                return;
            }
            int rare = item.ModItem<ItemScrap>().Rarity;
            if (rare == myRare) {
                int rolls = Math.Clamp(amount, 1, Math.Clamp(myRare.Abs(), 1, 10) * 20);
                for (int k = 0; k < rolls; k++) {
                    if (Main.LocalPlayer.RollLuck(4 + rare / 2) == 0) {
                        amount--;
                        item.stack--;
                        if (item.stack <= 0) {
                            item.TurnToAir();
                            break;
                        }
                        if (amount <= 0) {
                            return;
                        }
                    }
                }
            }
        }

        void IPostAddRecipes.PostAddRecipes(Aequus aequus) {
            foreach (var r in Main.recipe) {
                if (r == null) {
                    continue;
                }
                foreach (var i in r.requiredItem) {
                    if (ScrappableRarities.Contains(i.rare)) {
                        r.AddConsumeItemCallback(ScrapCheck);
                        break;
                    }
                }
            }
        }
    }
}