using Aequus.Projectiles.Ranged;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Ranged
{
    public class Slingshot : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 25;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 32;
            Item.useAnimation = 32;
            Item.width = 32;
            Item.height = 24;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemRarityID.Green;
            Item.shoot = ModContent.ProjectileType<SlingshotProj>();
            Item.noUseGraphic = true;
            Item.shootSpeed = 7.5f;
            Item.UseSound = new SoundStyle("Aequus/Sounds/Items/Slingshot/stretch") { Volume = 0.2f, };
            Item.value = Item.sellPrice(gold: 2);
            Item.knockBack = 1f;
            Item.useAmmo = SlingshotAmmos.BirdAmmo;
            Item.channel = true;
        }
    }

    public class SlingshotAmmos : GlobalItem
    {
        public static int BirdAmmo;

        public override void Load()
        {
            BirdAmmo = ItemID.Bird;
        }

        public override void SetDefaults(Item item)
        {
            if (item.type == ItemID.Bird || item.type == ItemID.Cardinal || item.type == ItemID.BlueJay ||
                item.type == ItemID.Duck || item.type == ItemID.MallardDuck || item.type == ItemID.Seagull ||
                item.type == ItemID.Grebe)
            {
                item.ammo = BirdAmmo;
            }
        }
    }
}