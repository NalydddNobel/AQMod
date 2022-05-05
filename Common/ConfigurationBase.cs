using Microsoft.Xna.Framework;
using System;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;

namespace Aequus.Common
{
    [BackgroundColor(10, 10, 40, 220)]
    public abstract class ConfigurationBase : ModConfig
    {
        protected const string Key = "$Mods.Aequus.Configuration.";
    }
}