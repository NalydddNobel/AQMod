using System;
using Terraria.DataStructures;

namespace Aequu2.Content.TownNPCs.SkyMerchant;

public class SkyMerchantSystem : ModSystem {
    public static int SkyMerchantX;
    public static int SpawnCheck;

    private static bool TrySpawningSkyMerchant() {
        float minX = SkyMerchantX * 16f;
        for (int i = 0; i < Main.maxPlayers; i++) {
            if (Main.player[i].active && !Main.player[i].DeadOrGhost && Math.Abs(Main.player[i].position.X - minX) < NPC.safeRangeX * 16f) {
                minX = Math.Min(Main.player[i].position.X - NPC.safeRangeX * 16f - 100f, minX);
                i = -1;
                continue;
            }
        }
        NPC.NewNPC(new EntitySource_Misc("Aequu2: Sky Merchant"), (int)minX, Main.rand.Next((int)Helper.ZoneSkyHeightY / 2, (int)Helper.ZoneSkyHeightY) * 16, ModContent.NPCType<SkyMerchant>());
        return true;
    }

    public override void PostUpdateNPCs() {
        if (!Main.dayTime) {
            SkyMerchantX = Main.maxTilesX;
            return;
        }
        SkyMerchantX = (int)(Main.maxTilesX * (Main.time / Main.dayLength));
        if (Main.netMode != NetmodeID.MultiplayerClient && SpawnCheck++ > 30 && WorldGen.InWorld(SkyMerchantX, 60, 50)) {
            SpawnCheck = 0;
            for (int i = 0; i < Main.maxPlayers; i++) {
                if (Main.player[i].active && !Main.player[i].DeadOrGhost && !Main.player[i].ZoneWaterCandle && Helper.ZoneSkyHeight(Main.player[i]) && Math.Abs((int)Main.player[i].Center.X - SkyMerchantX * 16) < NPC.safeRangeX * 24f) {
                    if (TrySpawningSkyMerchant()) {
                        if (Main.tenthAnniversaryWorld) {
                            TextBroadcast.NewText("Announcement.HasArrived", CommonColor.TextVillagerHasArrived, Main.npc[NPC.FindFirstNPC(ModContent.NPCType<SkyMerchant>())].GetFullNetName());
                        }
                    }
                    break;
                }
            }
        }
    }
}