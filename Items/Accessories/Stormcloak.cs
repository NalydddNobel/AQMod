using Aequus.Content;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories
{
    [AutoloadEquip(EquipType.Back, EquipType.Front)]
    public class Stormcloak : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.damage = 50;
            Item.accessory = true;
            Item.rare = ItemDefaults.RarityDustDevil;
            Item.value = ItemDefaults.DustDevilValue;
            Item.expert = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().accDustDevilExpert = Item;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.RemoveKnockback();
            tooltips.RemoveCritChance();
        }

        public static List<Projectile> GetBlowableProjectiles(Player player, Item item, bool onlyMine = false)
        {
            var projectiles = new List<Projectile>();
            var rect = Utils.CenteredRectangle(player.Center, new Vector2(onlyMine ? 640f : 320f));
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].Colliding(Main.projectile[i].getRect(), rect) && PushableEntities.ProjectileIDs.Contains(Main.projectile[i].type))
                {
                    if (((Main.projectile[i].hostile || (Main.player[Main.projectile[i].owner].hostile && Main.player[Main.projectile[i].owner].team != player.team)) && !onlyMine) ||
                        Main.projectile[i].Aequus().sourceItemUsed == item.type)
                    {
                        projectiles.Add(Main.projectile[i]);
                        if (projectiles.Count >= 3 * item.Aequus().accStacks)
                            return projectiles;
                    }
                }
            }
            return projectiles;
        }

        public override void AddRecipes()
        {
            ModContent.GetInstance<TheReconstructionGlobalItem>().addEntry(Type);
        }
    }
}