using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Aequus.Common.PlayerLayers {
    internal class ForceDrawShirt : PlayerDrawLayer
    {
        public static HashSet<int> BodyShowShirt { get; private set; }

        public override void Load()
        {
            BodyShowShirt = new HashSet<int>();
        }

        public override Position GetDefaultPosition()
        {
            return new BeforeParent(PlayerDrawLayers.Torso);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (BodyShowShirt.Contains(drawInfo.drawPlayer.body))
            {
                int old = drawInfo.drawPlayer.body;
                drawInfo.drawPlayer.body = 0;
                PlayerDrawLayers.Torso.DrawWithTransformationAndChildren(ref drawInfo);
                drawInfo.drawPlayer.body = old;
            }
        }
    }
}
