using Aequus.Common.Recipes;
using Aequus.Projectiles.Summon.Necro;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Summon.Scepters
{
    [AutoloadGlowMask]
    public class FriendshipMagick : ScepterBase
    {
        public override void SetDefaults()
        {
            Item.DefaultToNecromancy(60);
            //Item.SetWeaponValues(50, 0f);
            Item.shoot = ModContent.ProjectileType<FriendshipMagickProj>();
            Item.shootSpeed = 30f;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(gold: 5);
            Item.mana = 50;
            Item.UseSound = SoundID.Item8;
            Item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            // Dummy condition, remove in proper 1.4.4 port
            AequusRecipes.CreateShimmerTransmutation(ModContent.ItemType<Revenant>(), ModContent.ItemType<FriendshipMagick>(), condition: AequusRecipes.ConditionOmegaStarite);
        }
    }
}