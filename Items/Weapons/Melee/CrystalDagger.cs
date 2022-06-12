using Aequus.Common;
using Aequus.Projectiles.Melee;
using Aequus.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Aequus.Items.Weapons.Melee
{
    public class CrystalDagger : ModItem
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
            Item.damage = 12;
            Item.knockBack = 2f;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.UseSound = SoundID.Item1;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 75);
            Item.shootSpeed = 8f;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<CrystalDaggerProj>();
            Item.autoReuse = true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return lightColor.MaxRGBA(120);
        }

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
            velocity = velocity.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f));
        }
    }
}