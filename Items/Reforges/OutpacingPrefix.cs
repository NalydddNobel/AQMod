using Terraria.Localization;

namespace AQMod.Items.Reforges
{
    public sealed class OutpacingPrefix : CooldownPrefix
    {
        protected override float CooldownMultiplier => 1.15f;
        protected override float ComboMultiplier => 1.15f;

        public override void SetDefaults()
        {
            DisplayName.SetDefault("Outpacing");
            DisplayName.AddTranslation(GameCulture.Chinese, "超越");
            // DisplayName.AddTranslation(GameCulture.Russian, "");
        }
    }
}
