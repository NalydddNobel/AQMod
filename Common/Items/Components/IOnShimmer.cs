using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aequus.Common.Items.Components;

public interface IOnShimmer {
    /// <returns>true to run normal shimmer behavior.</returns>
    bool OnShimmer();
}
