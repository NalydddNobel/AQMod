using Microsoft.Xna.Framework;
using System;

namespace Aequus.Common.GlobalItems
{
    public struct ItemDedication
    {
        public readonly Func<Color> color;

        public ItemDedication(Color color)
        {
            this.color = () => color;
        }

        public ItemDedication(Func<Color> getColor)
        {
            color = getColor;
        }
    }
}