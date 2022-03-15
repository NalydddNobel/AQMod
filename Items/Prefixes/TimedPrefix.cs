using Terraria.Localization;

namespace AQMod.Items.Prefixes
{
    public sealed class TimedPrefix : CooldownPrefix
    {
        protected override float CooldownMultiplier => 0.9f;

        public override void SetDefaults()
        {
            DisplayName.SetDefault("Timed");
            DisplayName.AddTranslation(GameCulture.Chinese, "时限");
            // DisplayName.AddTranslation(GameCulture.Russian, "");
        }
    }
}