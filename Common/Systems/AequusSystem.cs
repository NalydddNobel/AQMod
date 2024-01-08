using Aequus.Content.Bosses.Crabson;

namespace Aequus.Common.Systems;

public partial class AequusSystem : ModSystem {
    public override void PreUpdateEntities() {
    }

    public override void PostUpdatePlayers() {
        Crabson.CrabsonBoss = -1;
        if (Main.netMode != NetmodeID.Server) {
            AequusPlayer.LocalTimers = Main.LocalPlayer.GetModPlayer<AequusPlayer>().Timers;
        }
    }
}