using Aequus.Buffs.Pets;
using Aequus.Common.Recipes;
using Aequus.Items.Accessories.Utility;
using Aequus.Projectiles.Misc.Pets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Vanity.Pets.Light
{
    public class PersonalDronePack : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ModContent.ProjectileType<DronePet>(), ModContent.BuffType<DroneBuff>());
            Item.width = 20;
            Item.height = 20;
            Item.value = Item.sellPrice(gold: 2);
            Item.rare = ItemRarityID.Pink;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            return true;
        }

        public override void AddRecipes()
        {
            AequusRecipes.AddShimmerCraft(Type, ModContent.ItemType<HyperJet>());
        }
    }
}