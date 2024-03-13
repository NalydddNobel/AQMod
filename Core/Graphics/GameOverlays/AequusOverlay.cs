using Microsoft.Xna.Framework;
using Terraria.Graphics.Effects;

namespace Aequus.Core.Graphics.GameOverlays;
public abstract class AequusOverlay : Overlay, ILoad {
    public Mod Mod { get; private set; }

    public string Name => GetType().Name;

    public string EffectKey { get; private set; }
    protected bool active;

    protected AequusOverlay(EffectPriority priority, RenderLayers layer) : base(priority, layer) {
    }

    public override void Activate(Vector2 position, params object[] args) {
        active = true;
    }

    public override void Deactivate(params object[] args) {
        active = false;
    }

    public override bool IsVisible() {
        return active;
    }

    public override void Update(GameTime gameTime) {
    }

    public virtual void OnModLoad() {
    }

    public virtual bool SpecialVisuals(Player player) {
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