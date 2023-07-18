using Aequus.Common.Items;
using Aequus.Content.Events.DemonSiege;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Accessories.Misc.Luck {
    public class RabbitsFoot : ModItem {
        /// <summary>
        /// Default Value: 1
        /// </summary>
        public static float LuckRerolls = 1f;

        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
            DemonSiegeSystem.RegisterSacrifice(new SacrificeData(ItemID.Bunny, Type, EventTier.PreHardmode) { Hide = true, });
        }

        public override void SetDefaults() {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemDefaults.RarityDemonSiege;
            Item.value = ItemDefaults.ValueDemonSiege;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            player.Aequus().luckRerolls += LuckRerolls;
        }
    }
}