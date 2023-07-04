using Aequus.Common.Items;
using Aequus.Common.Items.EquipmentBooster;
using Aequus.Common.PlayerLayers.Equipment;
using Aequus.Common.UI;
using Aequus.Content;
using Aequus.Items.Accessories.CrownOfBlood;
using Aequus.Items.Accessories.CrownOfBlood.Buffs;
using Microsoft.Xna.Framework;
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
            Load_ExpertEffects();
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
            modifier.Boost |= EquipBoostType.Abilities | EquipBoostType.Defense;
            player.AddBuff(aequus.CrownOfBloodHearts > 0 ? ModContent.BuffType<CrownOfBloodDebuff>() : ModContent.BuffType<CrownOfBloodBuff>(), 8);
        }

        public void UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem) {
            if (!isSetToHidden || !isNotInVanitySlot) {
                var crown = player.Aequus().GetEquipDrawer<HoverCrownEquip>();
                crown.SetEquip(this, dyeItem);
                crown.CrownColor = Color.Red;
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

        private void ResetEffects_CrownOfBlood() {
            accCrownOfBlood = null;
            accCrownOfBloodItemClone = null;
            crownOfBloodBees = 0;
            crownOfBloodDeerclops = 0;
            crownOfBloodFriendlySlimes = 0;
        }

        private void ClearCrownOfBlood() {
            ResetEffects_CrownOfBlood();
            crownOfBloodHearts = 0;
            crownOfBloodRegenTime = 0;
            crownOfBloodCD = 0;
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

        private void PostUpdateEquips_CrownOfBlood() {
            if (crownOfBloodCD > 0) {
                crownOfBloodCD--;
                Player.AddBuff(ModContent.BuffType<CrownOfBloodCooldown>(), crownOfBloodCD);
            }

            PostUpdateEquips_WormScarfEmpowerment();
            PostUpdateEquips_BoneHelmEmpowerment();
            PostUpdateEquips_RoyalGels();
        }

        private void InflictCrownOfBloodDownside(Player.HurtInfo hit) {
            if (accCrownOfBlood != null) {
                CrownOfBloodHearts += hit.Damage / Player.HealthPerHeart();
            }
        }
    }
}