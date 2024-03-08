using Terraria.Cinematics;

namespace Aequus.Content.Cutscenes;

/// <summary><see cref="DD2Film"/> <see cref="DSTFilm"/></summary>
internal abstract class AequusFilm : Film {
    protected Vector2 _startPoint;
    protected string Name => GetType().Name;

    protected abstract void Begin();
    protected abstract void End();
    protected abstract void BuildSequence();

    public void BuildFullSequence() {
        BuildSequence();
        AppendKeyFrames(RestoreHeldItem);
    }

    public sealed override void OnBegin() {
        base.OnBegin();
        Begin();
        Main.NewText($"{Name}: Begin");
    }
    public sealed override void OnEnd() {
        base.OnEnd();
        End();
        Main.NewText($"{Name}: End");
    }

    #region Player
    protected static Player Player => Main.LocalPlayer;
    private Item _heldItem;

    protected FrameEvent SetHeldItem(int itemType) {
        return SetHeldItem(new Item(itemType));
    }
    protected FrameEvent SetHeldItem(Item item) {
        return (FrameEventData evt) => {
            _heldItem ??= Player.HeldItem;
            Player.inventory[Player.selectedItem] = item;
        };
    }

    protected void RestoreHeldItem(FrameEventData evt) {
        RestoreHeldItem();
    }
    protected void RestoreHeldItem() {
        if (_heldItem != null) {
            Player.inventory[Player.selectedItem] = _heldItem;
            _heldItem = null;
        }
    }

    protected void StopPlayerUseItem(FrameEventData evt) {
        Player.controlUseItem = false;
    }

    protected void StopPlayerMovement(FrameEventData evt) {
        Player.controlRight = false;
        Player.controlLeft = false;
    }

    protected void WalkPlayerLeft(FrameEventData evt) {
        Player.controlLeft = true;
    }

    protected void WalkPlayerRight(FrameEventData evt) {
        Player.controlRight = true;
    }
    #endregion

    protected static void FindFloorAt(Vector2 position, out int x, out int y) {
        x = (int)position.X;
        y = (int)position.Y;
        int i = x / 16;
        int j;
        for (j = y / 16; !WorldGen.SolidTile(i, j); j++) {
        }

        y = j * 16;
    }
}
