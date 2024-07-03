using System.Collections.Generic;
using Terraria.UI;

namespace Aequu2.Core.UI;

[Autoload(Side = ModSide.Client)]
public class NPCChat : ModSystem {
    public const string INTERFACE_LAYER_KEY = "Aequu2: NPC Talk Interface";

    public readonly UserInterface Interface = new();
    private GameInterfaceLayer _layer;

    public override void UpdateUI(GameTime gameTime) {
        Interface.IsVisible = true;
        Interface.Update(gameTime);
    }

    public override void ClearWorld() {
        Interface.SetState(null);
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
        if (!Interface.IsVisible || Interface.CurrentState == null) {
            return;
        }

        int index = layers.FindIndex(i => i.Name.Equals(InterfaceLayerNames.NPCSignDialog_24));

        if (index == -1) {
            return;
        }

        layers.Insert(index, _layer ??= new LegacyGameInterfaceLayer(INTERFACE_LAYER_KEY, () => {
            Interface.Draw(Main.spriteBatch, Main._drawInterfaceGameTime);
            return true;
        }, InterfaceScaleType.UI));
    }
}
