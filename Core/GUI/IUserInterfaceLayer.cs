using Terraria.UI;

namespace AequusRemake.Core.GUI;

public interface IUserInterfaceLayer {
    bool IsActive { get; set; }
    string InsertLayer { get; }
    int InsertOffset { get; }

    void OnClearWorld();
    void OnPreUpdatePlayers();
    /// <param name="gameTime"></param>
    /// <returns>Whether or not to deactivate this UI.</returns>
    bool OnUIUpdate(GameTime gameTime);

    void OnActivate();
    void OnDeactivate();
    /// <summary>Called right before this UI Layer node is removed.</summary>
    void OnRemove();

    GameInterfaceLayer GetGameInterfaceLayer();
}

public static class UILayerExtensions {
}
