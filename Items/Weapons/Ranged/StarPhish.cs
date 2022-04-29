using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Ranged
{
    public class StarPhish : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 8;
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 18;
            Item.useTime = 31;
            Item.useAnimation = 31;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2f;
            Item.autoReuse = true;
            Item.value = Item.sellPrice(silver: 10);
            Item.useAmmo = AmmoID.Dart;
            Item.shoot = ProjectileID.Seed;
            Item.shootSpeed = 25f;
            Item.UseSound = SoundID.Item65;
            Item.noMelee = true;
            Item.rare = ItemDefaults.RarityCrabCrevice;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int amt = Main.rand.Next(35, 50);
            var n = Vector2.Normalize(velocity);
            Vector2 dustPos = position + n * 30f + new Vector2(0, -4f);
            for (int i = 0; i < amt; i++)
            {
                int d = Dust.NewDust(dustPos, 10, 10, 33);
                Vector2 v = n.RotatedBy(Main.rand.NextFloat(-0.314f, 0.314f));
                Main.dust[d].velocity.X = v.X * Main.rand.NextFloat(6f, 12f);
                Main.dust[d].velocity.Y = v.Y * Main.rand.NextFloat(6f, 12f);
            }
            amt = Main.rand.Next(8, 12);
            for (int i = 0; i < amt; i++)
            {
                int d = Dust.NewDust(dustPos, 10, 10, 15);
                Vector2 v = n.RotatedBy(Main.rand.NextFloat(-0.157f, 0.157f));
                Main.dust[d].velocity.X = v.X * Main.rand.NextFloat(3f, 6f);
                Main.dust[d].velocity.Y = v.Y * Main.rand.NextFloat(3f, 6f);
            }
            return true;
        }
    }
}