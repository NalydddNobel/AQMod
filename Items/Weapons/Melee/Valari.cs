using Aequus.Common;
using Aequus.Projectiles.Melee;
using Aequus.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee
{
    public class Valari : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(1);

            LootPools.Bags.Lockbox_Secondary.Add(Type);
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.damage = 28;
            Item.useTime = 32;
            Item.useAnimation = 32;
            Item.rare = ItemDefaults.RarityDungeon;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.value = ItemDefaults.DungeonValue;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 5f;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<ValariProj>();
            Item.shootSpeed = 12f;
            Item.autoReuse = true;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 2;
        }
    }
}