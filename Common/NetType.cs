using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AQMod.Common
{
    public enum NetType : byte
    {
        /// <summary>
        /// Tells the server that someone wants to summon Omega Starite with the Ultimate Sword.
        /// </summary>
        SummonOmegaStarite = 0,
        /// <summary>
        /// Tells the server that someone wants to summon Omega Starite with the Ultimate Sword.
        /// </summary>
        UpdateGlimmerEvent = 1,
        Count
    }
}
