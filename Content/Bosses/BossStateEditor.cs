using Aequus.Common.NPCs;
using Terraria.Localization;

namespace Aequus.Content.Bosses;

#if DEBUG
public class BossStateEditor : ModItem {
    protected override bool CloneNewInstances => true;

    private int _npcSelected = -1;

    public override LocalizedText DisplayName => Language.GetText(PrettyPrintName());
    public override LocalizedText Tooltip => LocalizedText.Empty;

    public override void HoldItem(Player player) {
        if (_npcSelected == -1) {
            return;
        }

        NPC npc = Main.npc[_npcSelected];
        if (!npc.active || npc.ModNPC is not AequusBoss boss) {
            _npcSelected = -1;
            return;
        }

        if (player.chatOverhead.timeLeft > 0) {
            string value = player.chatOverhead.chatText.Trim().ToLower();
            if (int.TryParse(value, out int parseResult)) {
                boss.State = parseResult;
            }
            else if (boss.StateNameToId.TryGetValue(value, out int lookupResult)) {
                boss.State = lookupResult;
            }
        }
        player.chatOverhead.NewMessage($"CURRENT STATE IS:\n{(boss.StateIdToName.TryGetValue(boss.State, out string stateName) ? stateName : "Unknown")}", 120);
    }

    public override bool? UseItem(Player player) {
        if (Main.netMode != NetmodeID.SinglePlayer) {
            return false;
        }

        int closestBoss = -1;
        float closestBossDistance = float.MaxValue;
        for (int i = 0; i < Main.maxNPCs; i++) {
            NPC npc = Main.npc[i];
            if (npc.ModNPC is AequusBoss boss) {
                float distance = (Main.MouseWorld - npc.Center).Length();
                if (distance < closestBossDistance) {
                    closestBoss = i;
                    closestBossDistance = distance;
                }
            }
        }
        _npcSelected = closestBoss;

        if (closestBoss != -1) {
            Main.NewText($"Can now edit state of {Main.npc[closestBoss]}.");
        }
        else {
            Main.NewText("No bosses detected.");
        }
        return true;
    }

    public override string Texture => AequusTextures.TemporaryBuffIcon;

    public override void SetDefaults() {
        Item.width = 16;
        Item.height = 16;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.useTime = 16;
        Item.useAnimation = 16;
        _npcSelected = -1;

        if (Main.netMode != NetmodeID.SinglePlayer) {
            Item.SetDefaults(ItemID.None);
        }
    }
}
#endif
