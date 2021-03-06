using Aequus.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Aequus.Items.Weapons.Melee
{
    public class RockMan : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.DamageType = DamageClass.Melee;
            Item.damage = 20;
            Item.knockBack = 3f;
            Item.useAnimation = 24;
            Item.useTime = 24;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.UseSound = SoundID.Item1;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(gold: 1);
            Item.shootSpeed = 8f;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<RockManProj>();
            Item.autoReuse = true;
        }

        //public override bool? UseItem(Player player)
        //{
        //    for (int i = 0; i < Main.maxChests; i++)
        //    {
        //        if (Main.chest[i] != null && Main.chest[i]?.item.ContainsAny((i) => i.type == Type) == true)
        //        {
        //            player.Center = new Vector2(Main.chest[i].x, Main.chest[i].y) * 16f;
        //        }
        //    }
        //    return base.UseItem(player);
        //}

        public override int ChoosePrefix(UnifiedRandom rand)
        {
            return AequusHelpers.RollSwordPrefix(Item, rand);
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = velocity.RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f));
            position += Vector2.Normalize(velocity) * 10f;
        }
    }
}