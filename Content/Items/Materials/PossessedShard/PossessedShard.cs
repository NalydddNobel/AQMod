using Aequus.Common.Items;
using System.IO;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Content.Items.Materials.PossessedShard;

public class PossessedShard : ModItem {
    internal bool livingAndBreathingInvidiual;

    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 10;
    }

    public override void SetDefaults() {
        Item.width = 14;
        Item.height = 14;
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ItemDefaults.RarityEarlyHardmode - 1;
        Item.value = Item.sellPrice(silver: 7);
    }

    public override Color? GetAlpha(Color lightColor) {
        return Color.Lerp(Color.White, lightColor, Helper.Wave(Main.GlobalTimeWrappedHourly * 2.5f, 0.66f, 1f));
    }

    public override bool CanPickup(Player player) {
        return !livingAndBreathingInvidiual;
    }

    public override void OnSpawn(IEntitySource source) {
        if (source is EntitySource_Parent parentSource && parentSource.Entity is NPC npc && npc.type != ModContent.NPCType<PossessedShardNPC>()) {
            livingAndBreathingInvidiual = true;
        }
    }

    public override void Update(ref float gravity, ref float maxFallSpeed) {
        if (livingAndBreathingInvidiual) {
            if (Main.netMode != NetmodeID.MultiplayerClient) {
                for (int i = 0; i < Item.stack; i++) {
                    NPC.NewNPC(Item.GetSource_FromThis(), (int)Item.Center.X, (int)Item.Center.Y, ModContent.NPCType<PossessedShardNPC>());
                }
            }

            Item.active = false;
        }
    }

    public override void NetSend(BinaryWriter writer) {
        writer.Write(livingAndBreathingInvidiual);
    }

    public override void NetReceive(BinaryReader reader) {
        livingAndBreathingInvidiual = reader.ReadBoolean();
    }

    [Gen.AequusNPC_ModifyNPCLoot]
    internal static void ModifyNPCLoot(NPC npc, NPCLoot loot) {
        switch (npc.type) {
            case NPCID.CursedHammer:
            case NPCID.CrimsonAxe:
                loot.Add(ItemDropRule.Common(ModContent.ItemType<PossessedShard>(), minimumDropped: 2, maximumDropped: 3));
                break;

            case NPCID.PossessedArmor:
                loot.Add(ItemDropRule.Common(ModContent.ItemType<PossessedShard>(), chanceDenominator: 2));
                break;

        }
    }

}