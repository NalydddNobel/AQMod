using Aequus.Common.ModPlayers;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Misc {
    public class CrownOfBlood : ModItem, ItemHooks.IUpdateItemDye {

        public override void SetStaticDefaults() {
            SacrificeTotal = 1;
        }

        public override void SetDefaults() {
            Item.DefaultToAccessory(14, 20);
            Item.rare = ItemDefaults.RarityDemonSiege;
            Item.value = Item.buyPrice(gold: 7, silver: 50);
            Item.canBePlacedInVanityRegardlessOfConditions = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {

            var aequus = player.Aequus();
            var equipModifiers = aequus.equipModifiers;
            var parameters = EquipEmpowermentParameters.Defense | EquipEmpowermentParameters.Abilities;
            var equip = equipModifiers.FirstUnempoweredAccessory(parameters);
            equip.type |= parameters;
            equip.bonusColor = EquipEmpowermentManager.CrownOfBloodEmpowermentColor;
            equip.slotColor = new(150, 60, 60, 255);
            aequus.crownOfBloodDisableLifeRegen = true;
        }

        public void UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem) {
            if (!isSetToHidden || !isNotInVanitySlot) {
                player.Aequus().crown = Type;
                player.Aequus().cCrown = dyeItem.dye;
            }
        }
    }
}

namespace Aequus {
    public partial class AequusPlayer : ModPlayer {

        public bool crownOfBloodDisableLifeRegen;

        public void CheckDisableLifeRegen() {
            if (crownOfBloodDisableLifeRegen) {
                Player.lifeRegen = Math.Min(Player.lifeRegen, 0);
                Player.lifeRegenTime = Math.Min(Player.lifeRegenTime, 0);
            }
        }
    }
}