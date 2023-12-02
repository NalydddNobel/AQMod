using Aequus.Core.Networking;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus;

public class Aequus : Mod {
    public static Aequus Instance { get; private set; }

    public static bool Lite { get; set; } = true;

    public static bool highQualityEffects = true;

    /// <summary>
    /// Shorthand for a bunch of checks determining whether the game is unpaused.
    /// </summary>
    public static bool GameWorldActive => (Main.instance.IsActive && !Main.gamePaused && !Main.gameInactive) || Main.netMode != NetmodeID.SinglePlayer;

    public override void Load() {
        Instance = this;
    }

    public override void Unload() {
        Instance = null;
    }

    public override void HandlePacket(BinaryReader reader, int whoAmI) {
        PacketSystem.HandlePacket(reader, whoAmI);
    }
}