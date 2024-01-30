using ReLogic.Graphics;
using System.Diagnostics;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace Aequus.Core.UI;

internal class DiagnosticsMenu : ModSystem {
    public enum TimerType : System.Byte {
        Particles,
        PostDrawLiquids,
        Count
    }
    public enum TrackerType : System.Byte {
        ParticleScenes,
        UILayers,
        Count
    }

    private static Stopwatch _stopwatch;
    private static readonly System.Double[] _durations = new System.Double[(System.Int32)TimerType.Count];
    private static readonly System.Int32[] _trackedNumbers = new System.Int32[(System.Int32)TrackerType.Count];

    [Conditional("DEBUG")]
    public static void StartStopwatch() {
        _stopwatch.Restart();
    }
    [Conditional("DEBUG")]
    public static void EndStopwatch(TimerType diag) {
        _stopwatch.Stop();
        System.Byte index = (System.Byte)diag;
        _durations[index] = _stopwatch.Elapsed.TotalMilliseconds;
    }

    [Conditional("DEBUG")]
    public static void TrackNumber(TrackerType diag, System.Int32 value) {
        System.Byte index = (System.Byte)diag;
        _trackedNumbers[index] = value;
    }

    public override void PostDrawInterface(SpriteBatch spriteBatch) {
        if (!Main.drawDiag) {
            return;
        }

        DynamicSpriteFont font = FontAssets.MouseText.Value;

        System.Single wordX = 20f;
        System.Single numberX = 180f;
        System.Single y = 660f;
        System.Single yOffset = 20f;
        ChatManager.DrawColorCodedString(spriteBatch, font, "Aequus", new Vector2(wordX, y), Color.White, 0f, Vector2.Zero, Vector2.One);

        for (System.Int32 i = 0; i < _durations.Length; i++) {
            System.String name = ((TimerType)i).ToString();
            y += yOffset;
            spriteBatch.DrawString(font, name + ":", new Vector2(wordX, y), Color.White);
            spriteBatch.DrawString(font, $"{_durations[i]:F2}ms", new Vector2(numberX, y), Color.White);
        }

        for (System.Int32 i = 0; i < _trackedNumbers.Length; i++) {
            System.String name = ((TrackerType)i).ToString();
            y += yOffset;
            spriteBatch.DrawString(font, name + ":", new Vector2(wordX, y), Color.White);
            spriteBatch.DrawString(font, _trackedNumbers[i].ToString(), new Vector2(numberX, y), Color.White);
        }
    }

#if !DEBUG
    public override System.Boolean IsLoadingEnabled(Mod mod) => false;
#endif

    public override void Load() {
        _stopwatch = new();
    }
}