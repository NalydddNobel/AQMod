using ReLogic.Graphics;
using System.Diagnostics;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace Aequu2.Core.Debug;

internal class DiagnosticsMenu : ModSystem {
    public enum TimerType : byte {
        Particles,
        PostDrawLiquids,
        SeaFireflies,
        Count
    }
    public enum TrackerType : byte {
        ParticleScenes,
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
    public static void ClearOrEndStopwatch(TimerType diag, bool end) {
        if (end) {
            ClearStopwatch(diag);
        }
        else {
            EndStopwatch(diag);
        }
    }
    [Conditional("DEBUG")]
    public static void EndStopwatch(TimerType diag) {
        _stopwatch.Stop();
        byte index = (byte)diag;
        _durations[index] = _stopwatch.Elapsed.TotalMilliseconds;
    }
    [Conditional("DEBUG")]
    public static void ClearStopwatch(TimerType diag) {
        _stopwatch.Stop();
        byte index = (byte)diag;
        _durations[index] = 0.0;
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
        ChatManager.DrawColorCodedString(spriteBatch, font, "Aequu2", new Vector2(wordX, y), Color.White, 0f, Vector2.Zero, Vector2.One);

        for (int i = 0; i < _durations.Length; i++) {
            if (_durations[i] < 0.01) {
                continue;
            }
            string name = ((TimerType)i).ToString();
            y += yOffset;
            spriteBatch.DrawString(font, name + ":", new Vector2(wordX, y), Color.White);
            spriteBatch.DrawString(font, $"{_durations[i]:F2}ms", new Vector2(numberX, y), Color.White);
        }

        for (int i = 0; i < _trackedNumbers.Length; i++) {
            if (_trackedNumbers[i] == 0) {
                continue;
            }

            string name = ((TrackerType)i).ToString();
            y += yOffset;
            spriteBatch.DrawString(font, name + ":", new Vector2(wordX, y), Color.White);
            spriteBatch.DrawString(font, _trackedNumbers[i].ToString(), new Vector2(numberX, y), Color.White);
        }
    }

    public override bool IsLoadingEnabled(Mod mod) {
        return Aequu2.DEBUG_MODE;
    }

    public override void Load() {
        _stopwatch = new();
    }
}