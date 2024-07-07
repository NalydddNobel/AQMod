using Terraria.UI;

namespace AequusRemake.Core.GUI;

[Autoload(Side = ModSide.Client)]
public class UIStateLayer : UIState, IUserInterfaceLayer, ILoad {
    public bool IsActive { get; set; }
    public string InsertLayer { get; init; }
    public int InsertOffset { get; init; }

    private readonly string _name;
    private readonly InterfaceScaleType _scaleType;
    private GameInterfaceLayer _layer;

    /// <param name="Name">Name of this layer.</param>
    /// <param name="InsertLayer">Name of the layer to search the index of.</param>
    /// <param name="ScaleType">Scale Type of this layer.</param>
    /// <param name="InsertOffset">Index offset for inserting the layer. Defaults to 1 (Inserts after layer)</param>
    protected UIStateLayer(string Name, string InsertLayer, InterfaceScaleType ScaleType, int InsertOffset = 1) {
        this.InsertLayer = InsertLayer;
        this.InsertOffset = InsertOffset;
        _name = $"AequusRemake: {Name}";
        _scaleType = ScaleType;
    }

    public virtual void OnLoad() { }
    public virtual void OnUnload() { }

    public void Load(Mod mod) {
        OnLoad();
        IsActive = false;
        UILayersSystem.Register(this);
    }
    public void Unload() {
        OnUnload();
    }

    public virtual void OnClearWorld() { }
    public virtual void OnPreUpdatePlayers() { }
    public virtual bool OnUIUpdate(GameTime gameTime) {
        return true;
    }

    void IUserInterfaceLayer.OnActivate() {
        OnActivate();
    }
    void IUserInterfaceLayer.OnDeactivate() {
        OnDeactivate();
    }
    public virtual void OnRemove() { }

    public GameInterfaceLayer GetGameInterfaceLayer() {
        return _layer ??= new LegacyGameInterfaceLayer(_name, () => {
            Draw(Main.spriteBatch);
            return true;
        }, _scaleType);
    }
}
