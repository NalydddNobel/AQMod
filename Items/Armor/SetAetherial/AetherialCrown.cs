using Aequus.Common.DataSets;
using Aequus.CrossMod;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Items.Armor.SetAetherial {
    [AutoloadEquip(EquipType.Head)]
    public class AetherialCrown : AetherialArmorPiece {
        public static float MeleeWeaponSize = 0.1f;
        public static float NoConsumeChance = 0.33f;
        public static float ManaCostMultiplier = 0.9f;
        public static int MaxMana = 20;
        public static int MaxMinionAndSentrySlots = 1;
        public static int Regeneration = 2;
        public static int ThoriumMod_BonusBardInspirationMax = 1;
        public static int ThoriumMod_BonusHealerHealBonus = 1;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(GetFormatArgs(
            TextHelper.Create.Percent(MeleeWeaponSize), 
            TextHelper.Create.Percent(NoConsumeChance),
            MaxMana,
            TextHelper.Create.MultiplierPercentDifference(ManaCostMultiplier),
            MaxMinionAndSentrySlots,
            Regeneration / 2
        ));

        public override List<Item> ArmorList => ItemSets.Helmets;

        protected override int FragmentRecipeStack => 5;

        public override int EquipSlot => 0;

        public override ref int GetEquipSlotField(Item item) => ref item.headSlot;
        public override ref int GetEquipSlotField(Player player) => ref player.head;

        public override void SetStaticDefaults() {
            ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = false;
            ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
        }

        public override void SetDefaults() {
            base.SetDefaults();
            Item.DefaultToHeadgear(20, 20, Item.headSlot);
            Item.defense = 0;
            Item.rare = ItemRarityID.Red;
            _itemCache = null;
        }

        public override void UpdateEquip(Player player) {
            player.lifeRegen += 2;
            player.Aequus().statMeleeScale += MeleeWeaponSize;
            player.Aequus().armorAetherialAmmoCost = true;
            player.manaCost *= ManaCostMultiplier;
            player.statManaMax2 += MaxMana;
            player.maxMinions += MaxMinionAndSentrySlots;
            player.maxTurrets += MaxMinionAndSentrySlots;
            if (ThoriumMod.Instance != null) {
                ThoriumMod.Call("BonusBardInspirationMax", player, ThoriumMod_BonusBardInspirationMax);
                ThoriumMod.Call("BonusHealerHealBonus", player, ThoriumMod_BonusHealerHealBonus);
            }

            base.UpdateEquip(player);
        }
    }
}