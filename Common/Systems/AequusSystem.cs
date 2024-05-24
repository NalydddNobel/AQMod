using Aequus.Core.Assets;

namespace Aequus.Common.Systems;

public partial class AequusSystem : ModSystem {
    public override void PostUpdatePlayers() {
        if (Main.netMode != NetmodeID.Server) {
            AequusPlayer.LocalTimers = Main.LocalPlayer.GetModPlayer<AequusPlayer>().Timers;
        }

        AequusShaders.LoadAllShadersForDragonLensShaderDebuggingSinceItWillThrowAnErrorIfTheyAllAreNotLoaded();
    }
}