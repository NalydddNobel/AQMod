using System.Collections.Generic;
using Terraria.UI;

namespace Aequus.UI
{
    public interface IChooseInterfaceLayer
    {
        int GetLayerIndex(List<GameInterfaceLayer> layers);
    }
}