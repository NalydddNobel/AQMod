using Aequus.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Ranged
{
    public class CrusadersCrossbow : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            Item.damage = 22;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 32;
            Item.useAnimation = 32;
            Item.width = 30;
            Item.height = 30;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 3f;
            Item.useAmmo = AmmoID.Arrow;
            Item.UseSound = SoundID.Item5;
            Item.value = Item.sellPrice(gold: 2);
            Item.noMelee = true;
            Item.knockBack = 0.1f;
            Item.autoReuse = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-4f, 0f);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<CrusadersCrossbowBolt>();
        }
    }
}