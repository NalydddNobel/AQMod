using AQMod.Assets;
using AQMod.Common.Utilities;
using AQMod.Items.Armor.Arachnotron;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace AQMod.Common.PlayerLayers.ArmorOverlays
{
    public class ArachnotronVisorOverlay : ArmorHeadOverlay
    {
        public ArachnotronVisorOverlay() : base(new TextureAsset(CommonUtils.GetPath<ArachnotronVisor>() + "_HeadGlow"))
        {
        }

        public override void Draw(PlayerDrawInfo info)
        {
            if (info.shadow == 0)
            {
                if (Main.myPlayer == info.drawPlayer.whoAmI)
                {
                    if (AQPlayer.oldPosLength < AQPlayer.ARACHNOTRON_OLD_POS_LENGTH)
                        AQPlayer.oldPosLength = AQPlayer.ARACHNOTRON_OLD_POS_LENGTH;
                    AQPlayer.arachnotronHeadTrail = true;
                }
                GetBasicPlayerDrawInfo(info, out Vector2 headPosition, out float opacity);
                var clr = new Color(250, 250, 250, 0);
                var texture = Texture.GetValue();
                Main.playerDrawData.Add(new DrawData(texture, headPosition, info.drawPlayer.bodyFrame, clr, info.drawPlayer.headRotation, info.headOrigin, 1f, info.spriteEffects, 0) { shader = info.headArmorShader });
            }
        }
    }
}