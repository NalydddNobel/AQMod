using Aequus.Common.UI;
using Aequus.Core.Reflection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus.Core.UI;

/// <summary>
/// This type does not get loaded/registered when loaded on a Server.
/// </summary>
public class UILayerLoader : ModSystem {
    private static readonly List<UILayer> _ui = new();

    private static readonly MethodList<UILayer> Hook_ClearWorld = MethodList<UILayer>.Create<Action>(i => i.OnClearWorld);
    private static readonly MethodList<UILayer> Hook_OnGameUpdate = MethodList<UILayer>.Create<Action>(i => i.OnGameUpdate);
    private static readonly MethodList<UILayer> Hook_OnUIUpdate = MethodList<UILayer>.Create<Action<GameTime>>(i => i.OnUIUpdate);

    public static void Register(UILayer uiLayer) {
        _ui.Add(uiLayer);
    }

    public override bool IsLoadingEnabled(Mod mod) {
        return Main.netMode != NetmodeID.Server;
    }

    public override void ClearWorld() {
        foreach (var i in Hook_ClearWorld) {
            i.OnClearWorld();
        }
    }

    public override void PreUpdateWorld() {
        foreach (var i in Hook_OnGameUpdate) {
            i.OnGameUpdate();
        }
    }

    public override void UpdateUI(GameTime gameTime) {
        foreach (var i in Hook_OnUIUpdate) {
            i.OnUIUpdate(gameTime);
        }
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
        foreach (var i in _ui) {
            UISystem.Insert(layers, i.Layer, i.LayerName, () => i.Draw(Main.spriteBatch), i.ScaleType);
        }
    }
}