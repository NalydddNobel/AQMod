using Aequus.Graphics.PlayerLayers;
using Aequus.Projectiles.Summon.Misc;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Armor.Passive
{
    [AutoloadEquip(EquipType.Head)]
    public class DartTrapHat : ModItem, ItemHooks.IUpdateItemDye
    {
        public virtual int TimeBetweenShots => 320;
        public virtual int ProjectileShot => ModContent.ProjectileType<DartTrapHatProj>();
        public virtual float Speed => 10f;
        public virtual int Damage => 28;

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            StackingHatEffect.Blacklist.Add(Item.headSlot);
            ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.damage = Damage;
            Item.defense = 1;
            Item.DamageType = DamageClass.Summon;
            Item.ArmorPenetration = 10;
            Item.knockBack = 2f;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 30);
        }

        public override void UpdateEquip(Player player)
        {
            var aequus = player.Aequus();
            aequus.wearingSummonHelmet = true;
            player.GetDamage(DamageClass.Summon) += 0.1f;
            if (Main.myPlayer == player.whoAmI)
            {
                aequus.summonHelmetTimer--;
                if (aequus.summonHelmetTimer <= 0)
                {
                    if (aequus.summonHelmetTimer != -1)
                    {
                        int damage = player.GetWeaponDamage(Item);
                        var spawnPosition = player.gravDir == -1
                            ? player.position + new Vector2(player.width / 2f + 8f * player.direction, player.height)
                            : player.position + new Vector2(player.width / 2f + 8f * player.direction, 0f);
                        int p = Projectile.NewProjectile(player.GetSource_Accessory(Item, "Helmet"), spawnPosition, new Vector2(Speed, 0f) * player.direction, ProjectileShot, damage, player.armor[0].knockBack * player.GetKnockback(DamageClass.Summon).Additive, player.whoAmI);
                        Main.projectile[p].ArmorPenetration = Item.ArmorPenetration;
                    }
                    aequus.summonHelmetTimer = TimeBetweenShots;
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.DartTrap)
                .AddIngredient(ItemID.CopperBar, 8)
                .AddRecipeGroup("PresurePlate")
                .AddTile(TileID.Anvils)
                .TryRegisterBefore(ItemID.CopperBar);
            CreateRecipe()
                .AddIngredient(ItemID.DartTrap)
                .AddIngredient(ItemID.TinBar, 8)
                .AddRecipeGroup("PresurePlate")
                .AddTile(TileID.Anvils)
                .TryRegisterBefore(ItemID.CopperBar);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.RemoveKnockback();
            tooltips.RemoveCritChance();
        }

        public void UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem)
        {
            if (player.Aequus().stackingHat == 0)
                player.Aequus().stackingHat = Item.headSlot;
        }
    }
}