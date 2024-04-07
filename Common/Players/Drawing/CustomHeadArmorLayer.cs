using Aequus.Core.Assets;
using ReLogic.Content;
using System.ComponentModel;
using Terraria.DataStructures;

namespace Aequus.Common.Players.Drawing;

[Browsable(false)]
internal class CustomHeadArmorLayer : PlayerDrawLayer {
    public override Position GetDefaultPosition() {
        return new AfterParent(PlayerDrawLayers.Head);
    }

    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) {
        return EquipLoader.GetEquipTexture(EquipType.Head, drawInfo.drawPlayer.head) is IEquipTextureDraw;
    }

    protected override void Draw(ref PlayerDrawSet drawInfo) {
        if (EquipLoader.GetEquipTexture(EquipType.Head, drawInfo.drawPlayer.head) is not IEquipTextureDraw drawEquip) {
            return;
        }

        Player player = drawInfo.drawPlayer;

        Vector2 helmetOffset = drawInfo.helmetOffset;
        Rectangle bodyFrame2 = player.bodyFrame;
        Vector2 headVect = drawInfo.headVect;
        Color color2 = drawInfo.colorArmorHead;
        Vector2 drawCoordinates = helmetOffset + new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - player.bodyFrame.Width / 2 + player.width / 2), (int)(drawInfo.Position.Y - Main.screenPosition.Y + player.height - player.bodyFrame.Height + 4f)) + player.headPosition + drawInfo.headVect;
        drawEquip.Draw(ref drawInfo, drawCoordinates, bodyFrame2, color2, player.headRotation, headVect, drawInfo.playerEffect, drawInfo.cHead);
    }
}