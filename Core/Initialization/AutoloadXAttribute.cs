using System;

namespace Aequus.Core.Initialization;

internal abstract class AutoloadXAttribute : Attribute {
    internal abstract void Load(ModType modType);
}