using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;

namespace Aequus.Content.DedicatedContent.EtOmniaVanitas;
public class EtOmniaVanitasDropRule : CommonDrop {
    public EtOmniaVanitasDropRule(int chanceDenominator, int amountDroppedMinimum = 1, int amountDroppedMaximum = 1, int chanceNumerator = 1) : base(EtOmniaVanitasLoader.Tier1.Type, chanceDenominator, amountDroppedMinimum, amountDroppedMaximum, chanceNumerator) {
    }

    public override ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info) {
        if (info.player.RollLuck(chanceDenominator) < chanceNumerator) {
            int stack = info.rng.Next(amountDroppedMinimum, amountDroppedMaximum + 1);

            int item;
            if (info.npc != null) {
                item = Item.NewItem(new EntitySource_Loot(info.npc), info.rng.NextVector2FromRectangle(info.npc.getRect()), itemId, stack, prefixGiven: -1);
            }
            else {
                item = Item.NewItem(new EntitySource_ItemOpen(info.player, info.item), info.rng.NextVector2FromRectangle(info.player.getRect()), itemId, stack, prefixGiven: -1);
            }

            // Automatically upgrade this item into its strongest form based on progression
            if (item > -1 && item != Main.maxItems && Main.item[item].ModItem is EtOmniaVanitas modItem) {
                modItem.UpgradeIntoStrongest(playSound: false);
            }

            if (Main.netMode == NetmodeID.MultiplayerClient) {
                NetMessage.SendData(MessageID.SyncItem, number: item, number2: 1f);
            }

            return new() { State = ItemDropAttemptResultState.Success };
        }

        return new() { State = ItemDropAttemptResultState.FailedRandomRoll };
    }
}