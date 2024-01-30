using System;
using Terraria.DataStructures;

namespace Aequus.Content.TownNPCs.SkyMerchant;

public class SkyMerchantSystem : ModSystem {
    public static Int32 SkyMerchantX;
    public static Int32 SpawnCheck;

    private static Boolean TrySpawningSkyMerchant() {
        Single minX = SkyMerchantX * 16f;
        for (Int32 i = 0; i < Main.maxPlayers; i++) {
            if (Main.player[i].active && !Main.player[i].DeadOrGhost && Math.Abs(Main.player[i].position.X - minX) < NPC.safeRangeX * 16f) {
                minX = Math.Min(Main.player[i].position.X - NPC.safeRangeX * 16f - 100f, minX);
                i = -1;
                continue;
            }
        }
        NPC.NewNPC(new EntitySource_Misc("Aequus: Sky Merchant"), (Int32)minX, Main.rand.Next((Int32)Helper.ZoneSkyHeightY / 2, (Int32)Helper.ZoneSkyHeightY) * 16, ModContent.NPCType<SkyMerchant>());
        return true;
    }

    public override void PostUpdateNPCs() {
        if (!Main.dayTime) {
            SkyMerchantX = Main.maxTilesX;
            return;
        }
        SkyMerchantX = (Int32)(Main.maxTilesX * (Main.time / Main.dayLength));
        if (Main.netMode != NetmodeID.MultiplayerClient && SpawnCheck++ > 30 && WorldGen.InWorld(SkyMerchantX, 60, 50)) {
            SpawnCheck = 0;
            for (Int32 i = 0; i < Main.maxPlayers; i++) {
                if (Main.player[i].active && !Main.player[i].DeadOrGhost && !Main.player[i].ZoneWaterCandle && Helper.ZoneSkyHeight(Main.player[i]) && Math.Abs((Int32)Main.player[i].Center.X - SkyMerchantX * 16) < NPC.safeRangeX * 24f) {
                    if (TrySpawningSkyMerchant()) {
                        if (Main.tenthAnniversaryWorld) {
                            TextBroadcast.NewText("Announcement.HasArrived", CommonColor.TEXT_TOWN_NPC_ARRIVED, Main.npc[NPC.FindFirstNPC(ModContent.NPCType<SkyMerchant>())].GetFullNetName());
                        }
                    }
                    break;
                }
            }
        }
    }
}