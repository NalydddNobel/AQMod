using Aequus.Core.Assets;
using Aequus.Core.CodeGeneration;
using System.IO;
using Terraria.ModLoader.IO;

namespace Aequus;

[Gen.AequusSystem_WorldField<bool>("downedSalamancer")]
[Gen.AequusSystem_WorldField<bool>("downedDustDevil")]
[Gen.AequusSystem_WorldField<bool>("downedRedSprite")]
[Gen.AequusSystem_WorldField<bool>("downedSpaceSquid")]
public partial class AequusSystem : ModSystem, ISGUtil {
    /// <summary>Returns a point with the beginning (left) (X) and end (right) (Y) of the 'safe' underworld in remix worlds. This does not represent a point in 2D space, but rather two X coordinates.</summary>
    public static Point RemixWorldSafeUnderworldRange => new Point((int)(Main.maxTilesX * 0.38) + 50, (int)(Main.maxTilesX * 0.62));

    public static bool HardmodeTier => Main.hardMode || downedOmegaStarite;

    public override void PostUpdatePlayers() {
        if (Main.netMode != NetmodeID.Server) {
            AequusPlayer.LocalTimers = Main.LocalPlayer.GetModPlayer<AequusPlayer>().Timers;
        }

        AequusShaders.LoadAllShadersForDragonLensShaderDebuggingSinceItWillThrowAnErrorIfTheyAllAreNotLoaded();
    }

    public override void SaveWorldData(TagCompound tag) {
        SaveInner(tag);
    }

    public override void LoadWorldData(TagCompound tag) {
        LoadInner(tag);
    }

    public override void NetSend(BinaryWriter writer) {
        SendDataInner(writer);
    }

    public override void NetReceive(BinaryReader reader) {
        ReceiveDataInner(reader);
    }
}