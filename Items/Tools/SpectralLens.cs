using AQMod.Assets.ItemOverlays;
using AQMod.Common;
using AQMod.Items.Energies;
using AQMod.Localization;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Tools
{
    public class SpectralLens : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemOverlayLoader.Register(new SpectralLensOverlay(), item.type);
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 24;
            item.rare = ItemRarityID.Pink;
            item.value = Item.sellPrice(gold: 2);
            item.useTime = 120;
            item.useAnimation = 120;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.UseSound = SoundID.NPCDeath6;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(250, 250, 250, 222);

        public override bool UseItem(Player player)
        {
            AQNPC.NoEnergyDrops = !AQNPC.NoEnergyDrops;
            if (AQNPC.NoEnergyDrops)
            {
                AQMod.BroadcastMessage(AQText.Key + "Common.EnergyDontDrop", new Color(80, 200, 255, 255));
            }
            else
            {
                AQMod.BroadcastMessage(AQText.Key + "Common.EnergyDoDrop", new Color(80, 200, 255, 255));
            }
            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(mod, "Status", string.Format("Energy drops are currently {0}", AQNPC.NoEnergyDrops ? "disabled" : "enabled")));
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.Lens, 2);
            r.AddIngredient(ModContent.ItemType<UltimateEnergy>());
            r.AddTile(TileID.DemonAltar);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}