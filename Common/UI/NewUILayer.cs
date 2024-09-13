using Aequus.Common.Utilities;
using System;
using System.Collections.Generic;
using Terraria.UI;

namespace Aequus.Common.UI;

/// <summary>
/// Used to simplify UI implementation. Only loads in singleplayer or on multiplayer clients.
/// </summary>
[Autoload(Side = ModSide.Client)]
public abstract class NewUILayer : GameInterfaceLayer, ILoadable, IPostSetupContent {
    public readonly string InsertLayer;
    public readonly int InsertOffset;

    /// <param name="Name">Name of this layer.</param>
    /// <param name="InsertLayer">Name of the layer to search the index of.</param>
    /// <param name="ScaleType">Scale Type of this layer.</param>
    /// <param name="InsertOffset">Index offset for inserting the layer. Defaults to 1 (Inserts after layer)</param>
    protected NewUILayer(string Name, string InsertLayer, InterfaceScaleType ScaleType, int InsertOffset = 1) : base("Aequus: " + Name, ScaleType) {
        this.InsertLayer = InsertLayer;
        this.InsertOffset = InsertOffset;
    }

    public virtual void OnLoad() { }
    public virtual void PostSetupContent() { }
    public virtual void OnUnload() { }

    public virtual void OnClearWorld() { }
    public virtual void OnPreUpdatePlayers() { }
    public virtual bool OnUIUpdate(GameTime gameTime) {
        return true;
    }

    protected virtual void OnActivate() { }
    protected virtual void OnDeactivate() { }

    /// <summary>
    /// Activates the UI Layer.
    /// </summary>
    public void Activate() {
        if (Active) {
            return;
        }

        OnActivate();
        ModContent.GetInstance<NewUILayersSystem>()._activeInterfaces.AddLast(this);
        Active = true;
    }

    /// <summary>
    /// Deactivates the UI Layer. This does not instantly remove the layer, instead it will be removed on the next ui update.
    /// </summary>
    public void Deactivate() {
        if (!Active) {
            return;
        }

        OnDeactivate();
        Active = false;
    }

    public void Load(Mod mod) {
        OnLoad();
        Active = false;
        NewUILayersSystem.Register(this);
    }

    void IPostSetupContent.PostSetupContent(Aequus aequus) {
        PostSetupContent();
    }

    public void Unload() {
        OnUnload();
    }
}

/// <summary>
/// Only loads in singleplayer or on multiplayer clients.
/// </summary>
[Autoload(Side = ModSide.Client)]
public class NewUILayersSystem : ModSystem {
    public override void ClearWorld() {
        foreach (var i in _userInterfaces) {
            i.OnClearWorld();
        }
    }

    public override void PreUpdatePlayers() {
        foreach (var i in _onPreUpdatePlayers) {
            i.OnPreUpdatePlayers();
        }
    }

    public override void UpdateUI(GameTime gameTime) {
        //DiagnosticsMenu.TrackNumber(DiagnosticsMenu.TrackerType.UILayers, _activeInterfaces.Count);

        if (_activeInterfaces.Count <= 0) {
            return;
        }

        LinkedListNode<NewUILayer> node = _activeInterfaces.First;

        do {
            if (!node.Value.Active || !node.Value.OnUIUpdate(gameTime)) {
                node.Value.Active = false;
                _activeInterfaces.Remove(node);
            }
        }
        while ((node = node.Next) != null);
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
        foreach (var i in _activeInterfaces) {
            int index = layers.FindIndex((l) => l.Name.Equals(i.InsertLayer));
            if (index != -1) {
                layers.Insert(index + i.InsertOffset, i);
            }
        }
    }

    internal static readonly List<NewUILayer> _userInterfaces = new();
    internal readonly LinkedList<NewUILayer> _activeInterfaces = new();

    private readonly List<NewUILayer> _onPreUpdatePlayers = new();

    public static void Register(NewUILayer layer) {
        _userInterfaces.Add(layer);
    }

    public override void SetStaticDefaults() {
        foreach (NewUILayer i in _userInterfaces) {
            Type t = i.GetType();
            if (t.HasMethodOverride(nameof(NewUILayer.OnPreUpdatePlayers))) {
                _onPreUpdatePlayers.Add(i);
            }
        }
    }
}