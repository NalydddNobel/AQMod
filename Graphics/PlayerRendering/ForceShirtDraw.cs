using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Aequus.Graphics.PlayerRendering
{
    public class ForceShirtDraw : PlayerDrawLayer
    {
        public static HashSet<int> BodyForceShirt { get; private set; }

        public override void Load()
        {
            BodyForceShirt = new HashSet<int>();
        }

        public override Position GetDefaultPosition()
        {
            return new BeforeParent(PlayerDrawLayers.Torso);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (BodyForceShirt.Contains(drawInfo.drawPlayer.body))
            {
                int old = drawInfo.drawPlayer.body;
                drawInfo.drawPlayer.body = 0;
                PlayerDrawLayers.Torso.DrawWithTransformationAndChildren(ref drawInfo);
                drawInfo.drawPlayer.body = old;
            }
        }
    }
}
