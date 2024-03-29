﻿using Aequus;
using Aequus.Common.Items;
using Aequus.Common.Recipes;
using Aequus.Projectiles.Summon.Misc;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Armor.SetWizard {
    [AutoloadGlowMask]
    [AutoloadEquip(EquipType.Head)]
    public partial class MoonlunaHat : ModItem {

        public static readonly HashSet<int> IsRobe = new();

        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
            ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
            IsRobe.Add(ItemID.AmethystRobe);
            IsRobe.Add(ItemID.TopazRobe);
            IsRobe.Add(ItemID.SapphireRobe);
            IsRobe.Add(ItemID.EmeraldRobe);
            IsRobe.Add(ItemID.RubyRobe);
            IsRobe.Add(ItemID.DiamondRobe);
            IsRobe.Add(ItemID.AmberRobe);
            IsRobe.Add(ItemID.GypsyRobe);
        }

        public override void Unload() {
            IsRobe.Clear();
        }

        public override void SetDefaults() {
            int head = Item.headSlot;
            Item.CloneDefaults(ItemID.WizardHat);
            Item.headSlot = head;
            Item.damage = 20;
            Item.DamageType = DamageClass.Summon;
        }

        public override void UpdateEquip(Player player) {
            if (Main.myPlayer == player.whoAmI) {
                var aequus = player.Aequus();
                aequus.wearingPassiveSummonHelmet = true;
                if (aequus.summonHelmetTimer != 0) {
                    aequus.summonHelmetTimer -= (int)player.velocity.Length() / 2;
                }
                if (aequus.summonHelmetTimer <= 0) {
                    if (aequus.summonHelmetTimer != -1) {
                        int damage = player.GetWeaponDamage(Item);
                        int p = Projectile.NewProjectile(player.GetSource_Accessory(Item, "Helmet"), player.Center + new Vector2(0f, Main.rand.NextFloat(-player.height / 2f, player.height / 2f)), Vector2.Zero,
                            ModContent.ProjectileType<MoonlunaHatProj>(), damage, 0f, player.whoAmI);
                        Main.projectile[p].ArmorPenetration = Item.ArmorPenetration;
                    }
                    aequus.summonHelmetTimer = 30;
                }
            }
            player.maxMinions++;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) {
            return !body.IsAir && IsRobe.Contains(body.type);
        }

        public override void UpdateArmorSet(Player player) {
            player.setBonus = TextHelper.GetTextValue("ArmorSetBonus.MoonlunaHat");
            player.maxMinions++;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) {
            tooltips.RemoveKnockback();
            tooltips.RemoveCritChance();
        }

        public override void AddRecipes() {
            AequusRecipes.AddShimmerCraft(ItemID.WizardHat, ModContent.ItemType<MoonlunaHat>(), condition: null);
        }
    }
}