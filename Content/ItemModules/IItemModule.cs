using System.Collections.Generic;

namespace Aequus.Content.ItemModules
{
    public interface IItemModule
    {
        List<int> ModuleTypes { get; set; }
    }
}