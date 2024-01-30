namespace Aequus.Core.Graphics.GameOverlays;

public interface IOverlayDrawer {
    System.Boolean Update();
    void Draw(SpriteBatch spriteBatch);
}