using Aequus.Core.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus.Common.UI; 

public abstract class UILayer : ModType {
    public abstract string Layer { get; }
    
    public virtual InterfaceScaleType ScaleType => InterfaceScaleType.UI;

    public string LayerName { get; private set; }

    protected sealed override void Register() {
        LayerName = $"{Mod.Name}: {Name}";
        UILayerLoader.Register(this);
    }

    public sealed override void SetupContent() {
        SetStaticDefaults();
    }

    public virtual void OnClearWorld() {
    }
    public virtual void OnGameUpdate() {
    }
    public virtual void OnUIUpdate(GameTime gameTime) {
    }
    public abstract bool Draw(SpriteBatch spriteBatch);
}
