using ReLogic.Graphics;
using System;
using System.Diagnostics;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace Aequus.Core;

internal class TimeLog : ModSystem {
    public enum Type : byte {
        NewParticleUpdates,
        OldParticleUpdates,
        Count
    }

    private static Stopwatch _stopWatch;
    private static long[] _durations;

    [Conditional("DEBUG")]
    public static void Start() {
        _stopWatch.Restart();
    }

    [Conditional("DEBUG")]
    public static void Log(Type log) {
        _stopWatch.Stop();
        byte index = (byte)log;
        _durations[index] = _stopWatch.ElapsedTicks;
    }

    public override void PostDrawInterface(SpriteBatch spriteBatch) {
        if (Main.playerInventory) {
            return;
        }

        DynamicSpriteFont font = FontAssets.MouseText.Value;
        for (int i = 0; i < _durations.Length; i++) {
            string name = ((Type)i).ToString();
            ChatManager.DrawColorCodedString(spriteBatch, font, name + " = " + _durations[i], new Vector2(20f, 80f + i * 20f), Color.White, 0f, Vector2.Zero, Vector2.One);
        }
    }

    public override bool IsLoadingEnabled(Mod mod) =>
#if DEBUG
    true;
#else
        false;
#endif

    public override void Load() {
        _stopWatch = new();
        _durations = new long[(byte)Type.Count];
    }
}