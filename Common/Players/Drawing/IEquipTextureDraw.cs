using System.ComponentModel;
using Terraria.DataStructures;

namespace Aequus.Common.Players.Drawing;
internal interface IEquipTextureDraw {
    void Draw(ref PlayerDrawSet drawInfo, Vector2 position, Rectangle frame, Color color, float rotation, Vector2 origin, SpriteEffects effects, int shader);
}

[Browsable(false)]
internal class CustomFaceAccLayer : PlayerDrawLayer {
    public override Position GetDefaultPosition() {
        return new AfterParent(PlayerDrawLayers.FaceAcc);
    }

    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) {
        return EquipLoader.GetEquipTexture(EquipType.Face, drawInfo.drawPlayer.face) is IEquipTextureDraw;
    }

    protected override void Draw(ref PlayerDrawSet drawInfo) {
        if (EquipLoader.GetEquipTexture(EquipType.Face, drawInfo.drawPlayer.face) is not IEquipTextureDraw drawEquip) {
            return;
        }

        Player player = drawInfo.drawPlayer;
        Vector2 mountDrawOffset = Vector2.Zero;
        if (player.mount.Active && player.mount.Type == 52) {
            mountDrawOffset = new Vector2(28f, -2f);
        }

        Vector2 drawLocation = mountDrawOffset + new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - player.bodyFrame.Width / 2 + player.width / 2), (int)(drawInfo.Position.Y - Main.screenPosition.Y + player.height - player.bodyFrame.Height + 4f)) + player.headPosition + drawInfo.headVect;

        drawEquip.Draw(ref drawInfo, drawLocation, player.bodyFrame, drawInfo.colorArmorHead, player.headRotation, drawInfo.headVect, drawInfo.playerEffect, drawInfo.cFace);
    }
}