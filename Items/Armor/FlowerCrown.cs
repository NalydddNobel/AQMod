using Aequus.Projectiles.Summon.Misc;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Armor {
    [AutoloadEquip(EquipType.Head)]
    public class FlowerCrown : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
            ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
        }

        public override void SetDefaults() {
            Item.width = 16;
            Item.height = 16;
            Item.damage = 5;
            Item.DamageType = DamageClass.Summon;
            Item.ArmorPenetration = 10;
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(silver: 1);
        }

        public override void UpdateEquip(Player player) {
            if (Main.myPlayer == player.whoAmI) {
                var aequus = player.Aequus();
                aequus.wearingPassiveSummonHelmet = true;
                aequus.summonHelmetTimer--;
                if (NewPetal(player, aequus)) {
                    aequus.summonHelmetTimer = 150;
                    int damage = player.GetWeaponDamage(Item);
                    var spawnPosition = player.gravDir == -1
                           ? player.position + new Vector2(player.width / 2f + 8f * player.direction, player.height - 10)
                           : player.position + new Vector2(player.width / 2f + 8f * player.direction, 10f);
                    int w = Math.Max(player.width / 2 - 8, 4);
                    spawnPosition.X += Main.rand.Next(-w, w);
                    int p = Projectile.NewProjectile(player.GetSource_Accessory(Item, "Helmet"), spawnPosition,
                        new Vector2(Main.windSpeedCurrent * 2f + MathHelper.Clamp(player.velocity.X, -10f, 10f), Main.rand.NextFloat(-0.75f, 0.25f) + MathHelper.Clamp(player.velocity.Y, -10f, 10f)), ModContent.ProjectileType<FlowerCrownProj>(), damage, player.armor[0].knockBack * player.GetKnockback(DamageClass.Summon).Additive, player.whoAmI);
                    Main.projectile[p].ArmorPenetration = Item.ArmorPenetration;
                    Main.projectile[p].ai[1] += Main.rand.Next(-120, 10);
                    Main.projectile[p].timeLeft += Main.rand.Next(-60, 60);
                    Main.projectile[p].scale += Main.rand.NextFloat(-0.1f, 0.05f);
                    Main.projectile[p].frame = Main.rand.Next(3);
                    Main.projectile[p].rotation = Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi);
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