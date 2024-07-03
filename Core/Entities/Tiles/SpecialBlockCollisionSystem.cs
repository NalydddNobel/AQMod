using Aequu2.Core.Entities.Tiles.Components;
using System.Collections.Generic;
using System.Linq;

namespace Aequu2.Core.Entities.Tiles;

internal class SpecialBlockCollisionSystem : ModSystem {
    private List<ModTile> _solidToProjsAndItems;
    private List<ModTile> _solidToNPCs;

    public override void PreUpdateProjectiles() {
        foreach (ModTile modTile in _solidToProjsAndItems) {
            Main.tileSolid[modTile.Type] = true;
        }
    }

    public override void PostUpdateItems() {
        foreach (ModTile modTile in _solidToProjsAndItems) {
            Main.tileSolid[modTile.Type] = false;
        }
    }

    public override void PreUpdateNPCs() {
        foreach (ModTile modTile in _solidToNPCs) {
            Main.tileSolid[modTile.Type] = true;
        }
    }

    public override void PostUpdateNPCs() {
        foreach (ModTile modTile in _solidToNPCs) {
            Main.tileSolid[modTile.Type] = false;
        }
    }

    public override void OnModLoad() {
        _solidToProjsAndItems = Mod.GetContent<ModTile>().Where(m => m is ISolidToProjectilesAndItems).ToList();
        _solidToNPCs = Mod.GetContent<ModTile>().Where(m => m is ISolidToNPCs).ToList();
    }
}
