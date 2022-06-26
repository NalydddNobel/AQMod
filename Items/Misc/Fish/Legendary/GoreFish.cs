using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.Fish.Legendary
{
    public class GoreFish : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(2);
            AequusItem.LegendaryFish.Add(Type);
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.Batfish);
            Item.maxStack = 999;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            try
            {
                if (NPC.AnyNPCs(NPCID.Angler))
                    tooltips.Insert(tooltips.GetIndex("Tooltip#"), new TooltipLine(Mod, "AnglerHint", AequusText.GetText("AnglerHint")) { OverrideColor = AequusTooltips.HintColor, });
            }
            catch
            {
            }
        }
    }
}