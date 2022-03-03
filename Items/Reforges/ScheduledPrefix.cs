using Terraria;

namespace AQMod.Items.Reforges
{
    public sealed class ScheduledPrefix : CooldownPrefix
    {
        protected override float CooldownMultiplier => 0.75f;

        public override void SetDefaults()
        {
            DisplayName.SetDefault("Scheduled");
            // DisplayName.AddTranslation(GameCulture.Chinese, "");
            // DisplayName.AddTranslation(GameCulture.Russian, "");
        }

        public override void Apply(Item item)
        {
            base.Apply(item);
            item.mana = (int)(item.mana * 1.15f);
            item.useTime = (int)(item.useTime * 1.3f);
            item.useAnimation = (int)(item.useAnimation * 1.3f);
            item.knockBack *= 1.15f;
        }
    }
}