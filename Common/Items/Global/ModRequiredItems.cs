using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.Items.Global {
    public class ModRequiredItems : GlobalItem {
        private string _modNeeded;

        public override bool InstancePerEntity => true;

        public override bool AppliesToEntity(Item item, bool lateInstantiation) {
            return item.ModItem != null && item.ModItem.GetType().GetAttribute<ModRequiredAttribute>() != null;
        }

        public override void SetDefaults(Item item) {
            var attr = item?.ModItem?.GetType()?.GetAttribute<ModRequiredAttribute>();
            if (attr != null) {
                _modNeeded = attr.ModNeeded;
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
            if (!ModLoader.HasMod(_modNeeded)) {
                tooltips.AddTooltip(new TooltipLine(Mod, "NeedsMod", TextHelper.GetTextValue("NeedsMod", _modNeeded)) { OverrideColor = Color.Lerp(Color.White, Color.Turquoise * 1.5f, 0.5f), });
            }
        }
    }
}