using Aequus.Content;
using Aequus.Items;
using Aequus.Items.Accessories.CrownOfBlood.Projectiles;
using Aequus.Projectiles.Misc;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Unused.Items {
    public class TheReconstruction : ModItem, ItemHooks.IUpdateItemDye {

        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults() {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(gold: 2);
            Item.hasVanityEffects = true;
            Item.expert = true;
        }

        public override void AddRecipes() {
        }

        void ItemHooks.IUpdateItemDye.UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem) {
            if (!isSetToHidden || !isNotInVanitySlot) {
                player.Aequus().equippedEyes = Type;
                player.Aequus().cEyes = dyeItem.dye;
            }
        }
    }

    //public class TheReconstructionGlobalItem : GlobalItem {
    //    public static Dictionary<int, string> TooltipItems { get; private set; }

    //    public override void Load() {
    //        TooltipItems = new Dictionary<int, string>();
    //        addEntry(ItemID.EoCShield);
    //        addEntry(ItemID.WormScarf);
    //        addEntry(ItemID.BrainOfConfusion);
    //        addEntry(ItemID.BoneGlove);
    //        //AddEntry(ItemID.HiveBackpack);
    //        //AddEntry(ItemID.BoneHelm);
    //        addEntry(ItemID.VolatileGelatin);
    //        addEntry(ItemID.SporeSac);
    //    }

    //    internal string entryKey(int itemID) {
    //        return $"Mods.Aequus.ItemTooltip.{nameof(TheReconstruction)}.{TextHelper.ItemKeyName(itemID, Aequus.Instance)}";
    //    }
    //    internal void addEntry(int itemID) {
    //        TooltipItems.Add(itemID, entryKey(itemID));
    //    }

    //    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
    //        try {
    //            if (!Main.LocalPlayer.Aequus().accExpertBoost || !TooltipItems.TryGetValue(item.type, out var text)) {
    //                return;
    //            }
    //            int add = 0;
    //            if (item.type == ItemID.BoneHelm) {
    //                add = 0;
    //            }
    //            tooltips.Insert(tooltips.GetIndex("Tooltip#") + add, new TooltipLine(Mod, "MechsTooltip", Language.GetTextValue(text)) { OverrideColor = Color.Lerp(Color.Red, Color.White, 0.35f), });
    //        }
    //        catch {
    //        }
    //    }

    //    public override void UpdateAccessory(Item item, Player player, bool hideVisual) {
    //        if (player.Aequus().ExpertBoost) {
    //            TheReconstruction.ExpertEffect_UpdateAccessory(item, player);
    //        }
    //    }
    //}
}