using Aequus;
using Aequus.Common.Items.SlotDecals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.SentryChip {
    [LegacyName("MechsSentry")]
    public class Sentinel6510 : Sentry6502 {
        public override void SetDefaults() {
            base.SetDefaults();
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.sellPrice(gold: 6);
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            var aequus = player.Aequus();
            aequus.accSentrySlot = true;
            aequus.accSentryInheritence = Item;
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Sentry6502>())
                .AddIngredient(ItemID.SoulofMight, 5)
                .AddIngredient(ItemID.SoulofSight, 5)
                .AddIngredient(ItemID.SoulofFright, 5)
                .AddTile(TileID.MythrilAnvil)
                .TryRegisterAfter(ItemID.PapyrusScarab);
        }
    }
}