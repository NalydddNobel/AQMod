using Aequus.Items.Misc.Energies;
using Aequus.Projectiles.Summon.Necro;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Summon.Necro.Scepters
{
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
            CreateRecipe()
                .AddIngredient<Revenant>()
                .AddIngredient<CosmicEnergy>()
                .AddTile(TileID.Anvils)
                .AddCondition(Recipe.Condition.InGraveyardBiome)
                .TryRegisterAfter(ItemID.RainbowRod);
        }
    }
}