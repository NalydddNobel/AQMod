using Aequus.Projectiles.Summon.Misc;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Armor.PassiveSummon
{
    [AutoloadEquip(EquipType.Head)]
    public class DartTrapHat : ModItem
    {
        public virtual int TimeBetweenShots => 320;
        public virtual int ProjectileShot => ModContent.ProjectileType<DartTrapHatProj>();
        public virtual float Speed => 10f;

        public override void SetStaticDefaults()
        {
            this.SetResearch(1);
            ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.defense = 1;
            Item.DamageType = DamageClass.Summon;
            Item.damage = 28;
            Item.ArmorPenetration = 10;
            Item.knockBack = 2f;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 30);
        }

        public override void UpdateEquip(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                var aequus = player.Aequus();
                aequus.wearingSummonHelmet = true;
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
                player.GetDamage(DamageClass.Summon) += 0.10f;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.DartTrap)
                .AddIngredient(ItemID.CopperBar, 8)
                .AddRecipeGroup("PresurePlate")
                .AddTile(TileID.Anvils)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.DartTrap)
                .AddIngredient(ItemID.TinBar, 8)
                .AddRecipeGroup("PresurePlate")
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}