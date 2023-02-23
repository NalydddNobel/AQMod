using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Aequus.Graphics.PlayerLayers
{
    internal class DrawDataTrackers
    {
        public abstract class Tracker : PlayerDrawLayer
        {
            public int DDIndex;

            protected override void Draw(ref PlayerDrawSet drawInfo)
            {
                DDIndex = drawInfo.DrawDataCache.Count;
            }
        }

        public class DrawHeldItem_27_Tracker : Tracker
        {
            public override Position GetDefaultPosition()
            {
                return new BeforeParent(PlayerDrawLayers.HeldItem);
            }
        }

        public class ArmOverItem_28_Tracker : Tracker
        {
            public override Position GetDefaultPosition()
            {
                return new BeforeParent(PlayerDrawLayers.ArmOverItem);
            }
        }
    }
}
