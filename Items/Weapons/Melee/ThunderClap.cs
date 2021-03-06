using Aequus.Items.Recipes;
using Aequus.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Aequus.Items.Weapons.Melee
{
    public class ThunderClap : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            Item.SetWeaponValues(55, 32f, 15);
            Item.width = 30;
            Item.height = 30;
            Item.useTime = 100;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item1;
            Item.rare = ItemDefaults.RarityGaleStreams;
            Item.value = ItemDefaults.GaleStreamsValue;
            Item.DamageType = DamageClass.Melee;
            Item.shoot = ModContent.ProjectileType<ThunderClapProj>();
            Item.shootSpeed = 1f;
            Item.channel = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
        }

        public override int ChoosePrefix(UnifiedRandom rand)
        {
            return AequusHelpers.RollSwordPrefix(Item, rand);
        }

        public override void AddRecipes()
        {
            AequusRecipes.RedSpriteRecipe(this, ItemID.SlapHand);
        }
    }
}