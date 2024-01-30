using Terraria.Graphics.Effects;

namespace Aequus.Core.Graphics.GameOverlays;
public abstract class AequusOverlay : Overlay, ILoadable {
    public Mod Mod { get; private set; }

    public System.String Name => GetType().Name;

    public System.String EffectKey { get; private set; }
    protected System.Boolean active;

    protected AequusOverlay(EffectPriority priority, RenderLayers layer) : base(priority, layer) {
    }

    public override void Activate(Vector2 position, params System.Object[] args) {
        active = true;
    }

    public override void Deactivate(params System.Object[] args) {
        active = false;
    }

    public override System.Boolean IsVisible() {
        return active;
    }

    public override void Update(GameTime gameTime) {
    }

    public virtual void OnModLoad() {
    }

    public virtual System.Boolean SpecialVisuals(Player player) {
        return true;
    }

    public void Load(Mod mod) {
        Mod = mod;
        EffectKey = Name;
        OverlaysSceneEffect.RegisteredOverlays.Add(this);
        Overlays.Scene.Bind(EffectKey, this);
        OnModLoad();
    }

    public virtual void OnModUnload() {
    }

    public void Unload() {
        Mod = null;
        OnModUnload();
    }
}