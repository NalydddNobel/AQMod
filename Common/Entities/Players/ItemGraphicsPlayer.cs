using Terraria.DataStructures;

namespace Aequus.Common.Entities.Players;

public class HeldItemLayer : PlayerDrawLayer {
    public override Position GetDefaultPosition() {
        return new AfterParent(PlayerDrawLayers.HeldItem);
    }

    protected override void Draw(ref PlayerDrawSet drawInfo) {
        int heldItem = drawInfo.drawPlayer.HeldItem.type;
        if (ItemLoader.GetItem(heldItem) is not ICustomHeldItemGraphics custom) {
            return;
        }

        custom.Draw(ref drawInfo);
    }
}

public interface ICustomHeldItemGraphics {
    void Draw(ref PlayerDrawSet info);
}
