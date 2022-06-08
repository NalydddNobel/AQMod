using Aequus.Biomes;
using Aequus.NPCs.Boss;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories
{
    public class RabbitsFoot : ModItem
    {
        public override void SetStaticDefaults()
        {
            DemonSiegeInvasion.Register(DemonSiegeInvasion.PHM(ItemID.Bunny, Type));

            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemDefaults.RarityDemonSiege;
            Item.value = ItemDefaults.DemonSiegeValue;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().luckRerolls += 1f;
        }
    }
}