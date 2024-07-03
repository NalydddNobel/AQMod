using Aequu2.Core.Debug;
using System;
using System.Collections.Generic;
using Terraria.UI;

namespace Aequu2.Core.UI;

/// <summary>Only loads in singleplayer or on multiplayer clients.</summary>
[Autoload(Side = ModSide.Client)]
public class UILayersSystem : ModSystem {
    public override void ClearWorld() {
        foreach (var i in _registered) {
            i.OnClearWorld();
        }
    }

    public override void PreUpdatePlayers() {
        foreach (var i in _onPreUpdatePlayers) {
            i.OnPreUpdatePlayers();
        }
    }

    public override void UpdateUI(GameTime gameTime) {
        DiagnosticsMenu.TrackNumber(DiagnosticsMenu.TrackerType.UILayers, _active.Count);

        if (_active.Count <= 0) {
            return;
        }

        LinkedListNode<IUserInterfaceLayer> node = _active.First;

        do {
            if (!node.Value.IsActive || !node.Value.OnUIUpdate(gameTime)) {
                node.Value.IsActive = false;
                node.Value.OnRemove();
                _active.Remove(node);
            }
        }
        while ((node = node.Next) != null);
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
        foreach (var i in _active) {
            int index = layers.FindIndex((l) => l.Name.Equals(i.InsertLayer));
            if (index != -1) {
                layers.Insert(index + i.InsertOffset, i.GetGameInterfaceLayer());
            }
        }
    }

    internal static readonly List<IUserInterfaceLayer> _registered = new();
    internal readonly LinkedList<IUserInterfaceLayer> _active = new();
    private readonly List<IUserInterfaceLayer> _onPreUpdatePlayers = new();

    public static void Register(IUserInterfaceLayer layer) {
        _registered.Add(layer);
    }

    public override void SetStaticDefaults() {
        foreach (IUserInterfaceLayer i in _registered) {
            Type t = i.GetType();
            if (t.HasMethodOverride(nameof(IUserInterfaceLayer.OnPreUpdatePlayers))) {
                _onPreUpdatePlayers.Add(i);
            }
        }
    }
}