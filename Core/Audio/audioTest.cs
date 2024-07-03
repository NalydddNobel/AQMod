namespace Aequu2.Core.Audio;
internal class audioTest {
    public static void echotest() {
        int milliseconds = Main.rand.Next(350, 650);
        float decayFactor = Main.rand.NextFloat(0.3f, 0.7f);
        Main.NewText($"{milliseconds}ms delay, {decayFactor} delay factor.");
        new DynamicSoundStyle($"{AequusSounds.PhilBugHurt.ModPath()}.ogg", new EchoEffect(milliseconds, decayFactor)).Play();
    }
}
