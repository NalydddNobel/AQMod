using Terraria;
using Terraria.Localization;

namespace AQMod.Items.Reforges
{
    public sealed class SpeedrunningPrefix : CooldownPrefix
    {
        protected override float CooldownMultiplier => 1.3f;
        protected override float ComboMultiplier => 1.15f;

        public override void SetDefaults()
        {
            DisplayName.SetDefault("Speedrunning");
            DisplayName.AddTranslation(GameCulture.Chinese, "极速");
            // DisplayName.AddTranslation(GameCulture.Russian, "");
        }

        public override void Apply(Item item)
        {
            base.Apply(item);
            item.mana = (int)(item.mana * 0.95f);
            item.useTime = (int)(item.useTime * 0.9f);
            item.useAnimation = (int)(item.useAnimation * 0.9f);
            item.crit += 2;
            item.damage = (int)(item.damage * 1.05f);
        }
    }
}