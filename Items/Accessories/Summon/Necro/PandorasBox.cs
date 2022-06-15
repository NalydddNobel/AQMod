using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Summon.Necro
{
    public sealed class PandorasBox : ModItem
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
            Item.rare = ItemDefaults.RarityDungeon;
            Item.value = ItemDefaults.DungeonValue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().ghostProjExtraUpdates += 1;
        }
    }
}