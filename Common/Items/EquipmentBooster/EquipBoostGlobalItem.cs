using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.IO;
using Terraria.ModLoader;

namespace Aequus.Common.Items.EquipmentBooster;

public class EquipBoostGlobalItem : GlobalItem {
    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
        return entity.accessory || entity.headSlot > -1 || entity.bodySlot > -1 || entity.legSlot > -1;
    }

    public override bool InstancePerEntity => true;

    public EquipBoostInfo equipEmpowerment = null;

    public override void SetDefaults(Item entity) {
        equipEmpowerment = null;
    }

    public override void UpdateInventory(Item item, Player player) {
        if (!AequusPlayer.EquipmentModifierUpdate) {
            equipEmpowerment = null;
        }
    }

    public override void Update(Item item, ref float gravity, ref float maxFallSpeed) {
        equipEmpowerment = null;
    }

    public override void UpdateEquip(Item item, Player player) {
        if (!AequusPlayer.EquipmentModifierUpdate) {
            equipEmpowerment = null;
        }
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
        if (equipEmpowerment == null) {
            return;
        }

        var color = equipEmpowerment.textColor ?? Color.White;

        if (equipEmpowerment.HasFlag(EquipBoostType.Defense) && item.defense > 0) {
            for (int i = 0; i < tooltips.Count; i++) {
                if (tooltips[i].Mod == "Terraria" && tooltips[i].Name == "Defense") {
                    var text = tooltips[i].Text.Split(' ');
                    string number = text[0];
                    if (int.TryParse(number, out int numberValue)) {
                        text[0] = TextHelper.ColorCommand(numberValue.ToString(), color, alphaPulse: true);
                        tooltips[i].Text = string.Join(' ', text);
                    }
                    break;
                }
            }
        }
    }
}