using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.TownNPCs.SkyMerchant;

public class SkyMerchantSystem : ModSystem {
    public static int SkyMerchantX;
    public static int SpawnCheck;

    public override void PostUpdateNPCs() {
        if (Main.dayTime) {
            SkyMerchantX = Main.maxTilesX;
            return;
        }
        SkyMerchantX = (int)(Main.maxTilesX * (Main.time / Main.dayLength));
        if (Main.netMode != NetmodeID.MultiplayerClient && SpawnCheck++ > 30) {
            SpawnCheck = 0;
            for (int i = 0; i < Main.maxPlayers; i++) {
                if (Main.player[i].active && !Main.player[i].ZoneWaterCandle && Helper.ZoneSkyHeight(Main.player[i]) && Math.Abs((int)Main.player[i].Center.X - SkyMerchantX) < NPC.safeRangeX * 1.5f) {
                    NPC.NewNPC(new EntitySource_Misc("Aequus: Sky Merchant"), SkyMerchantX * 16, Main.rand.Next(50 * 16, ((int)Helper.ZoneSkyHeightY - 20) * 16), ModContent.NPCType<SkyMerchant>(), Target: i);
                    break;
                }
            }
        }
    }
}