namespace AequusRemake.Core.Graphics.GameOverlays;

public interface IOverlayDrawer {
    bool Update();
    void Draw(SpriteBatch spriteBatch);
}