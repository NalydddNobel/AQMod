using Aequus.Common.DataSets;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Armor.SetAetherial {
    [AutoloadEquip(EquipType.Body)]
    public class AetherialBreastplate : AetherialArmorPiece {
        public static float IncreasedDamageAndCrit = 0.2f;
        public static float MeleeSpeed = 0.12f;
        public static float ProjectileSpeed = 0.3f;
        public static int ArmorPenetration = 5;
        public static int MaxMinionAndSentrySlots = 2;
        public static int Regeneration = 4;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(GetFormatArgs(
            TextHelper.Create.Percent(IncreasedDamageAndCrit),
            TextHelper.Create.Percent(MeleeSpeed),
            TextHelper.Create.Percent(ProjectileSpeed),
            ArmorPenetration,
            MaxMinionAndSentrySlots,
            Regeneration / 2
        ));

        public override List<Item> ArmorList => ItemSets.Chestplates;

        protected override int FragmentRecipeStack => 10;

        public override int EquipSlot => 1;

        public override ref int GetEquipSlotField(Item item) => ref item.bodySlot;
        public override ref int GetEquipSlotField(Player player) => ref player.body;


        public override void SetDefaults() {
            base.SetDefaults();
            Item.width = 20;
            Item.height = 20;
            Item.defense = 0;
            Item.rare = ItemRarityID.Red;
        }

        public override void UpdateEquip(Player player) {
            player.GetDamage(DamageClass.Generic) += IncreasedDamageAndCrit;
            player.GetCritChance(DamageClass.Generic) += IncreasedDamageAndCrit;
            player.GetAttackSpeed(DamageClass.Melee) += MeleeSpeed;
            player.Aequus().statProjectileSpeed.Get(DamageClass.Generic) += ProjectileSpeed;
            player.GetArmorPenetration(DamageClass.Generic) += ArmorPenetration;
            player.maxMinions += MaxMinionAndSentrySlots;
            player.maxTurrets += MaxMinionAndSentrySlots;
            player.lifeRegen += Regeneration;
            player.aggro += 400;
            base.UpdateEquip(player);
        }
    }
}