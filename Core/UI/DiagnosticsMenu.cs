using ReLogic.Graphics;
using System.Diagnostics;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace Aequus.Core.UI;

internal class DiagnosticsMenu : ModSystem {
    public enum TimerType : byte {
        Particles,
        PostDrawLiquids,
        Count
    }
    public enum TrackerType : byte {
        UILayers,
        Count
    }

    private static Stopwatch _stopwatch;
    private static readonly double[] _durations = new double[(int)TimerType.Count];
    private static readonly int[] _trackedNumbers = new int[(int)TrackerType.Count];

    [Conditional("DEBUG")]
    public static void StartStopwatch() {
        _stopwatch.Restart();
    }
    [Conditional("DEBUG")]
    public static void EndStopwatch(TimerType diag) {
        _stopwatch.Stop();
        byte index = (byte)diag;
        _durations[index] = _stopwatch.Elapsed.TotalMilliseconds;
    }

    [Conditional("DEBUG")]
    public static void TrackNumber(TrackerType diag, int value) {
        byte index = (byte)diag;
        _trackedNumbers[index] = value;
    }

    public override void PostDrawInterface(SpriteBatch spriteBatch) {
        if (!Main.drawDiag) {
            return;
        }

        DynamicSpriteFont font = FontAssets.MouseText.Value;

        float wordX = 20f;
        float numberX = 180f;
        float y = 660f;
        float yOffset = 20f;
        ChatManager.DrawColorCodedString(spriteBatch, font, "Aequus", new Vector2(wordX, y), Color.White, 0f, Vector2.Zero, Vector2.One);

        for (int i = 0; i < _durations.Length; i++) {
            string name = ((TimerType)i).ToString();
            y += yOffset;
            spriteBatch.DrawString(font, name + ":", new Vector2(wordX, y), Color.White);
            spriteBatch.DrawString(font, $"{_durations[i]:F2}ms", new Vector2(numberX, y), Color.White);
        }

        for (int i = 0; i < _trackedNumbers.Length; i++) {
            string name = ((TrackerType)i).ToString();
            y += yOffset;
            spriteBatch.DrawString(font, name + ":", new Vector2(wordX, y), Color.White);
            spriteBatch.DrawString(font, _trackedNumbers[i].ToString(), new Vector2(numberX, y), Color.White);
        }
    }

    public override bool IsLoadingEnabled(Mod mod) =>
#if DEBUG
    true;
#else
        false;
#endif

    public override void Load() {
        _stopwatch = new();
    }
}