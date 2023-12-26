global using Aequus.Core.Utilities;
global using Microsoft.Xna.Framework;
global using Microsoft.Xna.Framework.Graphics;
global using Terraria;
global using Terraria.ID;
global using Terraria.ModLoader;
using Aequus.Core.Networking;
using System.IO;
using System.Reflection;

namespace Aequus;

public partial class Aequus : Mod {
    public static Aequus Instance { get; private set; }

    public static Assembly TerrariaAssembly => typeof(Main).Assembly;

    public static bool highQualityEffects = true;

    /// <summary>
    /// Shorthand for a bunch of checks determining whether the game is unpaused.
    /// </summary>
    public static bool GameWorldActive => (Main.instance.IsActive && !Main.gamePaused && !Main.gameInactive) || Main.netMode != NetmodeID.SinglePlayer;

    public override void Load() {
        Instance = this;
        LoadModCalls();
    }

    public override void Unload() {
        Instance = null;
        UnloadModCalls();
    }

    public override void HandlePacket(BinaryReader reader, int whoAmI) {
        PacketSystem.HandlePacket(reader, whoAmI);
    }
}