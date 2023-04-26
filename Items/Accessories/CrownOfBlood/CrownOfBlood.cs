using Aequus.Common.ModPlayers;
using Aequus.Content;
using Aequus.Items.Accessories.CrownOfBlood;
using Aequus.UI;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus.Items.Accessories.CrownOfBlood {
    [LegacyName("CrownOfBlood")]
    public partial class CrownOfBloodItem : ModItem, ItemHooks.IUpdateItemDye {
        public const int AccessorySlot = 0;
        public const int ArmorSlot = Player.SupportedSlotsArmor + AccessorySlot;

        public override void Load() {
            LoadDataSets();
            LoadExpertEffects();
        }

        public override void Unload() {
            NoBoost.Clear();
        }

        public override void SetStaticDefaults() {
            SentryAccessoriesDatabase.OnAI[Type] = SentryAccessoriesDatabase.ApplyEquipFunctional_AI;
        }

        public override void SetDefaults() {
            Item.DefaultToAccessory(14, 20);
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.buyPrice(gold: 15);
            Item.hasVanityEffects = true;
        }

        public override bool CanEquipAccessory(Player player, int slot, bool modded) {
            return slot > AccessorySlot;
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            var aequus = player.Aequus();
            aequus.accCrownOfBlood = Item;
            aequus.accCrownOfBloodItemClone = player.armor[ArmorSlot];
            var modifier = aequus.equipModifiers.GetVanilla(ArmorSlot);
            modifier.addedStacks++;
            modifier.type |= EquipEmpowermentParameters.Abilities | EquipEmpowermentParameters.Defense;
            player.AddBuff(aequus.CrownOfBloodHearts > 0 ? ModContent.BuffType<CrownOfBloodDebuff>() : ModContent.BuffType<CrownOfBloodBuff>(), 8);
        }

        public void UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem) {
            if (!isSetToHidden || !isNotInVanitySlot) {
                player.Aequus().crown = Type;
                player.Aequus().cCrown = dyeItem.dye;
            }
        }

        private static bool CheckItemSlot(AequusUI.ItemSlotContext context) {
            return _equipAnimation <= 0f || context.Context != ItemSlot.Context.EquipAccessory || context.Slot != ArmorSlot;
        }
    }
}

namespace Aequus {
    public partial class AequusPlayer : ModPlayer {

        public Item accCrownOfBlood;
        public Item accCrownOfBloodItemClone;

        private int crownOfBloodHearts;
        /// <summary>
        /// The amount of hearts consumed by the <see cref="CrownOfBloodItem"/>.
        /// </summary>
        public int CrownOfBloodHearts { get => crownOfBloodHearts; set => crownOfBloodHearts = Math.Clamp(value, 0, Player.TotalHearts() - 1); }
        public int crownOfBloodRegenTime;

        private void ResetCrownOfBlood() {
            accCrownOfBlood = null;
            accCrownOfBloodItemClone = null;
        }

        private void ClearCrownOfBlood() {
            accCrownOfBlood = null;
            accCrownOfBloodItemClone = null;
            crownOfBloodHearts = 0;
            crownOfBloodRegenTime = 0;
        }

        private void UpdateCrownOfBlood() {

            if (Main.myPlayer == Player.whoAmI) {
                CrownOfBloodItem.UpdateEquipEffect(accCrownOfBlood != null, accCrownOfBloodItemClone);
            }

            if (crownOfBloodHearts <= 0) {
                crownOfBloodRegenTime = 0;
                return;
            }

            if (Main.myPlayer == Player.whoAmI) {
                CrownOfBloodHeartsOverlay.AnimationTimer += 2f + crownOfBloodHearts / 5f;
            }

            if (timeSinceLastHit > 300) {
                crownOfBloodRegenTime++;
                if (crownOfBloodRegenTime > 50) {
                    crownOfBloodRegenTime = 0;

                    crownOfBloodHearts--;
                    if (crownOfBloodHearts <= 0) {
                        return;
                    }
                }
            }
            else {
                crownOfBloodRegenTime = 0;
            }

            int hearts = Player.TotalHearts();
            int heartsLeft = Math.Max(hearts - crownOfBloodHearts, 1);
            Player.statLife = Math.Min(Player.statLife, Player.statLifeMax2 / hearts * heartsLeft);
        }

        private void InflictCrownOfBloodDownside(Player.HurtInfo hit) {
            if (accCrownOfBlood != null) {
                CrownOfBloodHearts += hit.Damage / Player.HealthPerHeart();
            }
        }
    }
}