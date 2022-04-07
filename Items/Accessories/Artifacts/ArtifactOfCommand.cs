using Aequus.Content.Artifacts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Artifacts
{
    public sealed class ArtifactOfCommand : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.CanGetPrefixes[Type] = false;
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory(16, 16);
            Item.rare = ItemRarityID.Red;
            Item.value = Item.sellPrice(gold: 50);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            ArtifactsSystem.CommandGameMode = true;
        }
    }
}