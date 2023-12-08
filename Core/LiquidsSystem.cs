using Aequus.Content.Fishing.CrabPots;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Core;

public class LiquidsSystem : ModSystem {
    public static int WaterStyle { get; set; }

    public override void PreUpdateEntities() {
        if (Main.netMode == NetmodeID.Server) {
            return;
        }

        bool bloodMoon = Main.bloodMoon;
        Main.bloodMoon = false;
        try {
            WaterStyle = Main.CalculateWaterStyle(ignoreFountains: true);
        }
        catch {
        }
        Main.bloodMoon = bloodMoon;
    }

    public static void SendWaterStyle(BinaryWriter writer, int waterStyleId) {
        writer.Write(waterStyleId);
        if (waterStyleId >= Main.maxLiquidTypes) {
            writer.Write(LoaderManager.Get<WaterFallStylesLoader>().Get(waterStyleId).FullName);
        }
    }

    public static int ReceiveWaterStyle(BinaryReader reader) {
        int waterStyleId = reader.ReadInt32();
        return waterStyleId >= Main.maxLiquidTypes ? CrabPotBiomeData.GetWaterStyle(reader.ReadString()) : waterStyleId;
    }
}
