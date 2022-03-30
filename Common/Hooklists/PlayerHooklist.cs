using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Aequus.Common.Hooklists
{
    public partial class PlayerHooklist : ILoadable
    {
        void ILoadable.Load(Mod mod)
        {
            On.Terraria.DataStructures.PlayerDrawLayers.DrawPlayer_RenderAllLayers += OnRenderPlayer;
        }

        void ILoadable.Unload()
        {
        }
    }
}