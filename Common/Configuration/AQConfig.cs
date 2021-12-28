using System;
using System.Reflection;
using Terraria.ModLoader.Config;

namespace AQMod.Common.Configuration
{
    [BackgroundColor(10, 10, 40, 220)]
    public abstract class AQConfig : ModConfig
    {
        private const BindingFlags _validBinding = BindingFlags.Instance | BindingFlags.Public;
        private const AttributeTargets _validAttributeUseage = AttributeTargets.Property | AttributeTargets.Field;

        [AttributeUsage(_validAttributeUseage)]
        protected class StaticBoundPropertyAttribute : Attribute
        {
            private readonly PropertyInfo _setProperty;

            public StaticBoundPropertyAttribute(Type type, string propertyName)
            {
                _setProperty = type.GetProperty(propertyName);
            }
            public StaticBoundPropertyAttribute(string typeName, string propertyName)
            {
                _setProperty = AQMod.Instance.Code.GetType(typeName).GetProperty(propertyName);
            }

            internal static void OnChangedConfig(AQConfig config)
            {
                foreach (var f in config.GetType().GetFields(_validBinding))
                {
                    var attribute = f.GetCustomAttribute<StaticBoundPropertyAttribute>();
                    if (attribute != null)
                    {
                        attribute._setProperty.SetValue(null, f.GetValue(config));
                    }
                }
                foreach (var p in config.GetType().GetProperties(_validBinding))
                {
                    var attribute = p.GetCustomAttribute<StaticBoundPropertyAttribute>();
                    if (attribute != null)
                    {
                        attribute._setProperty.SetValue(null, p.GetValue(config));
                    }
                }
            }
        }

        public override void OnChanged()
        {
            StaticBoundPropertyAttribute.OnChangedConfig(this);
        }
    }
}