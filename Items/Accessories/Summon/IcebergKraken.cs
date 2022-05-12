using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Summon
{
    public sealed class IcebergKraken : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemDefaults.RarityGaleStreams;
            Item.value = ItemDefaults.GaleStreamsValue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().autoSentry = true;
            player.Aequus().frostburnSentry = true;
            player.maxTurrets++;
        }

        public override void AddRecipes()
        {
            AequusRecipes.SpaceSquidDrop(this, ModContent.ItemType<SentrySquid>());
        }
    }
}