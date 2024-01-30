using Aequus.Content.Fishing.CrabPots;
using System.IO;

namespace Aequus.Core;

public class LiquidsSystem : ModSystem {
    public static System.Int32 WaterStyle { get; set; }

    public override void PreUpdateEntities() {
        if (Main.netMode == NetmodeID.Server) {
            return;
        }

        System.Boolean bloodMoon = Main.bloodMoon;
        Main.bloodMoon = false;
        try {
            WaterStyle = Main.CalculateWaterStyle(ignoreFountains: true);
        }
        catch {
        }
        Main.bloodMoon = bloodMoon;
    }

    public static void SendWaterStyle(BinaryWriter writer, System.Int32 waterStyleId) {
        writer.Write(waterStyleId);
        if (waterStyleId >= Main.maxLiquidTypes) {
            writer.Write(LoaderManager.Get<WaterFallStylesLoader>().Get(waterStyleId).FullName);
        }
    }

    public static System.Int32 ReceiveWaterStyle(BinaryReader reader) {
        System.Int32 waterStyleId = reader.ReadInt32();
        return waterStyleId >= Main.maxLiquidTypes ? CrabPotBiomeData.GetWaterStyle(reader.ReadString()) : waterStyleId;
    }
}
