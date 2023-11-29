using System;
using Terraria.ModLoader;

namespace Aequus.Core.Autoloading;

internal abstract class AutoloadXAttribute : Attribute {
    internal abstract void Load(ModType modType);
}