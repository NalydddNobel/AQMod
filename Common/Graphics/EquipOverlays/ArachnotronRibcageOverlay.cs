using AQMod.Content.Players;
using AQMod.Items.Armor.Arachnotron;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace AQMod.Common.Graphics.PlayerEquips
{
    public class ArachnotronRibcageOverlay : EquipBodyOverlay
    {
        public ArachnotronRibcageOverlay() : base(ModContent.GetTexture(AQUtils.GetPath2<ABArachnotronRibcage>("_BodyGlow")))
        {
        }

        public override void Draw(PlayerDrawInfo info)
        {
            if (info.shadow == 0)
            {
                if (Main.myPlayer == info.drawPlayer.whoAmI)
                {
                    if (PlayerDrawEffects.MyOldPositionsLengthCache < PlayerDrawEffects.Draw_ArachnotronArmorOldPositionLength)
                        PlayerDrawEffects.MyOldPositionsLengthCache = PlayerDrawEffects.Draw_ArachnotronArmorOldPositionLength;
                    PlayerDrawEffects.MyArachnotronBodyTrail = true;
                }
                GetBasicPlayerDrawInfo(info, out Vector2 bodyPosition, out float opacity);
                var clr = new Color(250, 250, 250, 0);
                var texture = Asset.Value;
                Main.playerDrawData.Add(new DrawData(texture, bodyPosition, info.drawPlayer.bodyFrame, clr, info.drawPlayer.headRotation, info.headOrigin, 1f, info.spriteEffects, 0) { shader = info.headArmorShader });
            }
        }
    }
}