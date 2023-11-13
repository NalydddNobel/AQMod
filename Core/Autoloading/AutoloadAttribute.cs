using System;
using Terraria.ModLoader;

namespace Aequus.Core.Autoloading;

internal abstract class AutoloadAttribute : Attribute {
    internal abstract void Load(ModType modType);
}