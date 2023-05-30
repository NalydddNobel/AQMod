using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace Aequus.Common.Preferences {
    [BackgroundColor(10, 10, 40, 220)]
    public abstract class ConfigurationBase : ModConfig, IPostSetupContent
    {
        protected const string Key = "$Mods.Aequus.Configuration.";
        protected abstract string ConfigKey { get; }

        protected class NameAttribute : LabelAttribute
        {
            public NameAttribute(string name) : base(Key + name)
            {
            }
        }
        protected class DescAttribute : TooltipAttribute
        {
            public DescAttribute(string tooltip) : base(Key + tooltip + "Tooltip")
            {
            }
        }
        protected class MemberBGColorAttribute : BackgroundColorAttribute
        {
            public MemberBGColorAttribute() : base(47, 29, 140, 180)
            {
            }
        }
        protected class MemberBGColor_SecondaryAttribute : BackgroundColorAttribute
        {
            public MemberBGColor_SecondaryAttribute() : base(80, 80, 130, 180)
            {
            }
        }
        protected void FixItemIcon(string key)
        {
            TextHelper.ModifyText($"Configuration.{ConfigKey}.{key}", TextHelper.Modifications.UpdateItemCommands);
        }
        protected void FormatText(string key, object obj)
        {
            TextHelper.ModifyText($"Configuration.{ConfigKey}.{key}", t => t.SetValue(t.Value.FormatWith(obj)));
        }

        public void PostSetupContent(Aequus aequus)
        {
            AddCustomTranslations();
        }
        public abstract void AddCustomTranslations();

        public virtual void Load(Mod mod)
        {

        }
        public virtual void Unload()
        {

        }
    }
}