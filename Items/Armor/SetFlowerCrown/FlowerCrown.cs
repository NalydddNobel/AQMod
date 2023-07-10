using Aequus;
using Aequus.Common.Items;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Items.Armor.SetFlowerCrown {
    [AutoloadEquip(EquipType.Head)]
    public class FlowerCrown : ModItem {
        public static int PetalDamage = 5;
        public static int TagDamage = 2;
        public static int ArmorPenetration = 10;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(PetalDamage, TagDamage, ArmorPenetration);

        public override void SetStaticDefaults() {
            ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
        }

        public override void SetDefaults() {
            Item.width = 16;
            Item.height = 16;
            Item.DamageType = DamageClass.Summon;
            Item.ArmorPenetration = ArmorPenetration;
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(copper: 30);
        }

        public override void UpdateEquip(Player player) {
            if (Main.myPlayer == player.whoAmI) {
                var aequus = player.Aequus();
                aequus.wearingPassiveSummonHelmet = true;
                aequus.summonHelmetTimer--;
                if (NewPetal(player, aequus)) {
                    var spawnPosition = player.gravDir == -1
                           ? player.position + new Vector2(player.width / 2f + 8f * player.direction, player.height - 10)
                           : player.position + new Vector2(player.width / 2f + 8f * player.direction, 10f);
                    int w = Math.Max(player.width / 2 - 8, 4);
                    spawnPosition.X += Main.rand.Next(-w, w);
                    var p = Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item, "Helmet"), spawnPosition,
                        new Vector2(Main.windSpeedCurrent * 2f + MathHelper.Clamp(player.velocity.X, -10f, 10f), Main.rand.NextFloat(-0.75f, 0.25f) + MathHelper.Clamp(player.velocity.Y, -10f, 10f)), ModContent.ProjectileType<FlowerCrownProj>(), PetalDamage, 0f, player.whoAmI, 
                        ai1: Main.rand.Next(-120, 10));
                    if (p == null) {
                        return;
                    }
                    aequus.summonHelmetTimer = 150;
                    p.ArmorPenetration = Item.ArmorPenetration;
                }
            }
        }

        public bool NewPetal(Player player, AequusPlayer aequus) {
            return aequus.summonHelmetTimer < 0 ||
                Main.rand.NextBool((int)MathHelper.Clamp(aequus.summonHelmetTimer - (int)player.velocity.Length(), 30, 120));
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) {
            tooltips.RemoveKnockback();
            tooltips.RemoveCritChance();
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.Daybloom, 3)
                .AddIngredient(ItemID.Sunflower)
                .TryRegisterBefore(ItemID.CopperBar);
        }
    }
}