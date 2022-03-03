namespace AQMod.Items.Reforges
{
    public sealed class TimedPrefix : CooldownPrefix
    {
        protected override float CooldownMultiplier => 0.9f;

        public override void SetDefaults()
        {
            DisplayName.SetDefault("Timed");
            // DisplayName.AddTranslation(GameCulture.Chinese, "");
            // DisplayName.AddTranslation(GameCulture.Russian, "");
        }
    }
}