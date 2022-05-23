using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Ranged
{
    public class Slingshot : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            Item.damage = 25;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.width = 32;
            Item.height = 24;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemRarityID.Green;
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 7.5f;
            Item.autoReuse = true;
            Item.UseSound = SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Slingshot/stretch")?.WithVolume(0.2f);
            Item.value = Item.sellPrice(gold: 2);
            Item.knockBack = 1f;
            Item.useAmmo = AmmoID.Bullet;
        }
    }
}