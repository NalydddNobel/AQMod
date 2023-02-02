using Aequus.Common.ModPlayers;
using Aequus.Projectiles.Misc.Bobbers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Tools.FishingRods
{
    public class Starcatcher : ModItem, ItemHooks.IModifyFishingPower
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.WoodFishingPole);
            Item.fishingPole = 45;
            Item.shootSpeed = 16f;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(silver: 72);
            Item.shoot = ModContent.ProjectileType<StarcatcherBobber>();
        }

        public void ModifyFishingPower(Player player, AequusPlayer fishing, Item fishingRod, ref float fishingLevel)
        {
            if (Main.ColorOfTheSkies.ToVector3().Length() < 1f)
            {
                fishingLevel += 0.2f;
            }
        }
    }
}