using AQMod.Items.Materials;
using AQMod.Items.Materials.Energies;
using AQMod.Items.Recipes;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Tools
{
    public class Globebulb : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.Glowmask(() => Color.Lerp(new Color(95, 80, 20, 0), new Color(0, 0, 0, 0), ((float)Math.Sin(Main.GlobalTime * 15f) + 1f) * 0.25f + 0.25f));
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 28;
            item.rare = ItemRarityID.Pink;
            item.value = Item.sellPrice(gold: 1);
            item.useTime = 20;
            item.useAnimation = 20;
            item.mana = 20;
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
                AQWorld.dayrate += 24;
            }
            return false;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ModContent.ItemType<Lightbulb>(), 9);
            r.AddRecipeGroup(AQRecipeGroups.DemoniteBarOrCrimtaneBar, 5);
            r.AddIngredient(ModContent.ItemType<CosmicEnergy>());
            r.AddTile(TileID.Anvils);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}