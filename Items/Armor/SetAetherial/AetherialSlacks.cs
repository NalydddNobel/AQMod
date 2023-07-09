using Aequus.Common.DataSets;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Items.Armor.SetAetherial {
    [AutoloadEquip(EquipType.Legs)]
    public class AetherialSlacks : AetherialArmorPiece {
        public static float MovementSpeed = 0.33f;
        public static float RunAcceleration = 0.03f;
        public static float FlyDuration = 0.5f;
        public static int Regeneration = 4;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(GetFormatArgs(
            TextHelper.Create.Percent(MovementSpeed),
            TextHelper.Create.Percent(FlyDuration),
            Regeneration / 2
        ));

        public override List<Item> ArmorList => ItemSets.Leggings;

        protected override int FragmentRecipeStack => 7;

        public override int EquipSlot => 2;

        public override ref int GetEquipSlotField(Item item) => ref item.legSlot;
        public override ref int GetEquipSlotField(Player player) => ref player.legs;

        public override void SetDefaults() {
            base.SetDefaults();
            Item.width = 20;
            Item.height = 20;
            Item.defense = 0;
            Item.rare = ItemRarityID.Red;
        }

        public override void UpdateEquip(Player player) {
            player.moveSpeed *= 1f + MovementSpeed;
            player.runAcceleration += RunAcceleration;
            player.Aequus().flightStats.wingTime *= 1f + FlyDuration;
            player.lifeRegen += Regeneration;

            base.UpdateEquip(player);
        }
    }
}