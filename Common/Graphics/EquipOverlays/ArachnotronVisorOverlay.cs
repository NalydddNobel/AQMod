using AQMod.Items.Armor.Arachnotron;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace AQMod.Common.Graphics.PlayerEquips
{
    public class ArachnotronVisorOverlay : EquipHeadOverlay
    {
        public ArachnotronVisorOverlay() : base(ModContent.GetTexture(AQUtils.GetPath<ArachnotronVisor>("_HeadGlow")))
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
                var texture = Asset.Value;
                Main.playerDrawData.Add(new DrawData(texture, headPosition, info.drawPlayer.bodyFrame, clr, info.drawPlayer.headRotation, info.headOrigin, 1f, info.spriteEffects, 0) { shader = info.headArmorShader });
            }
        }
    }
}