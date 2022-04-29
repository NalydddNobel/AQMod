using Aequus.Projectiles.Summon;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Armor.PassiveSummon
{
    [AutoloadEquip(EquipType.Head)]
    public class FlowerCrown : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(1);
            ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.DamageType = DamageClass.Summon;
            Item.damage = 10;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 10);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            for (int i = 0; i < tooltips.Count; i++)
            {
                if (tooltips[i].Mod == "Terraria")
                {
                    if (tooltips[i].Name == "Knockback")
                    {
                        tooltips.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public override void UpdateEquip(Player player)
        {
            var aequus = player.Aequus();
            aequus.summonHelmetTimer--;
            bool spawn;
            if (aequus.summonHelmetTimer < 0)
            {
                aequus.summonHelmetTimer = 120;
                spawn = true;
            }
            else
            {
                int chance = Math.Max(aequus.summonHelmetTimer - (int)(player.velocity.Length() * 4f), 10);
                spawn = Main.rand.NextBool(chance);
            }
            if (spawn)
            {
                int damage = player.GetWeaponDamage(Item);
                var spawnPosition = player.gravDir == -1
                       ? player.position + new Vector2(player.width / 2f + 8f * player.direction, player.height - 10)
                       : player.position + new Vector2(player.width / 2f + 8f * player.direction, 10f);
                int w = Math.Max(player.width / 2 - 8, 4);
                spawnPosition.X += Main.rand.Next(-w, w);
                int p = Projectile.NewProjectile(player.GetSource_Accessory(Item, "Helmet"), spawnPosition, new Vector2(Main.windSpeedCurrent * 2f + player.velocity.X, Main.rand.NextFloat(-0.75f, 0.25f) + player.velocity.Y), ModContent.ProjectileType<FlowerCrownProj>(), damage, player.armor[0].knockBack * player.GetKnockback(DamageClass.Summon).Additive, player.whoAmI);
                Main.projectile[p].ai[1] += Main.rand.Next(-120, 10);
                Main.projectile[p].timeLeft += Main.rand.Next(-60, 60);
                Main.projectile[p].scale += Main.rand.NextFloat(-0.1f, 0.05f);
                Main.projectile[p].frame = Main.rand.Next(3);
                Main.projectile[p].rotation = Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Daybloom, 3)
                .AddIngredient(ItemID.Mushroom)
                .AddIngredient(ItemID.Wood, 8)
                .Register();
        }
    }
}