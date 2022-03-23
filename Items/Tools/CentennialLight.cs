using AQMod.Items.Materials.Energies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Tools
{
    public class CentennialLight : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.Glowmask();
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 28;
            item.rare = ItemRarityID.Cyan;
            item.value = Item.sellPrice(gold: 1);
            item.useTime = 20;
            item.useAnimation = 20;
            item.mana = 15;
            item.autoReuse = true;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.UseSound = SoundID.Item8;
        }

        public override void HoldItem(Player player)
        {
            player.accWatch += 3;
        }

        public override bool UseItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI || Main.netMode == NetmodeID.Server)
            {
                if (Main.netMode == NetmodeID.Server && player.itemAnimation == 1)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
                AQWorld.dayrate += 75;
            }
            return false;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ModContent.ItemType<Globebulb>());
            r.AddIngredient(ItemID.CobaltBar, 15);
            r.AddIngredient(ModContent.ItemType<AtmosphericEnergy>(), 5);
            r.AddIngredient(ItemID.SoulofLight, 5);
            r.AddIngredient(ItemID.SoulofNight, 5);
            r.AddTile(TileID.Anvils);
            r.SetResult(this);
            r.AddRecipe();

            r = new ModRecipe(mod);
            r.AddIngredient(ModContent.ItemType<Globebulb>());
            r.AddIngredient(ItemID.PalladiumBar, 15);
            r.AddIngredient(ModContent.ItemType<AtmosphericEnergy>(), 5);
            r.AddIngredient(ItemID.SoulofLight, 5);
            r.AddIngredient(ItemID.SoulofNight, 5);
            r.AddTile(TileID.Anvils);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}