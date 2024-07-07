using Terraria.UI;

namespace AequusRemake.Core.GUI;

/// <summary>Used to simplify UI implementation. Only loads in singleplayer or on multiplayer clients.</summary>
[Autoload(Side = ModSide.Client)]
public abstract class UILayer : GameInterfaceLayer, IUserInterfaceLayer, ILoad {
    bool IUserInterfaceLayer.IsActive { get => Active; set => Active = value; }
    public string InsertLayer { get; init; }
    public int InsertOffset { get; init; }

    public Mod Mod { get; private set; }

    /// <param name="Name">Name of this layer.</param>
    /// <param name="InsertLayer">Name of the layer to search the index of.</param>
    /// <param name="ScaleType">Scale Type of this layer.</param>
    /// <param name="InsertOffset">Index offset for inserting the layer. Defaults to 1 (Inserts after layer)</param>
    protected UILayer(string Name, string InsertLayer, InterfaceScaleType ScaleType, int InsertOffset = 1) : base("AequusRemake: " + Name, ScaleType) {
        this.InsertLayer = InsertLayer;
        this.InsertOffset = InsertOffset;
    }

    public virtual void OnLoad() { }
    public virtual void OnUnload() { }

    public void Load(Mod mod) {
        Mod = mod;
        OnLoad();
        Active = false;
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

    public virtual void OnActivate() { }
    public virtual void OnDeactivate() { }
    public virtual void OnRemove() { }

    public GameInterfaceLayer GetGameInterfaceLayer() {
        return this;
    }
}
