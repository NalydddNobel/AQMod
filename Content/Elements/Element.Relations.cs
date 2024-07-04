using AequusRemake.Core.CodeGeneration;
using AequusRemake.Core.CrossMod;
using AequusRemake.Core.Entites.Bestiary;
using AequusRemake.DataSets;
using AequusRemake.DataSets.Structures;
using System.Collections.Generic;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace AequusRemake.Content.Elements;

public partial class Element {
    private readonly HashSet<int> _npcs = [];
    private readonly HashSet<int> _items = [];

    private readonly Dictionary<IDEntry<ItemID>, bool> _manualItems = [];

    public bool ContainsNPC(int i) {
        return _npcs.Contains(i);
    }

    public bool ContainsItem(int i) {
        return _items.Contains(i);
    }

    public bool AddNPC(int i) {
        return _npcs.Add(i);
    }

    public bool RemoveNPC(int i) {
        return _npcs.Remove(i);
    }

    public bool AddItem(int i) {
        _manualItems[i] = true;
        return AddItemInner(i);
    }

    public bool RemoveItem(int i) {
        _manualItems[i] = false;
        return RemoveItemInner(i);
    }

    /// <summary>Attempts to add a ModNPC's type using its full name.</summary>
    /// <param name="fullName">The full name of the mod npc. Example: ThoriumMod/Viscount</param>
    public bool AddNPC(string fullName) {
        if (!ExtendCrossMod.GetContentFromName(fullName, out ModNPC modNPC)) {
            return false;
        }

        return AddNPC(modNPC.Type);
    }

    /// <summary>Attempts to add a ModItem's type using its full name.</summary>
    /// <param name="fullName">The full name of the mod item. Example: ThoriumMod/IllumiteBar</param>
    public bool AddItem(string fullName) {
        if (!ExtendCrossMod.GetContentFromName(fullName, out ModItem modItem)) {
            return false;
        }

        return AddItem(modItem.Type);
    }

    /// <summary>Attempts to remove a ModItem's type using its full name.</summary>
    /// <param name="fullName">The full name of the mod item. Example: ThoriumMod/IllumiteBar</param>
    public bool RemoveItem(string fullName) {
        if (!ExtendCrossMod.GetContentFromName(fullName, out ModItem modItem)) {
            return false;
        }

        return RemoveItem(modItem.Type);
    }

    private bool AddItemInner(int i) {
        return _items.Add(i);
    }

    private bool RemoveItemInner(int i) {
        return _items.Remove(i);
    }

    public void AddBestiaryRelations(params BestiaryTags[] tags) {
        foreach (var tag in tags) {
            foreach (NPC npc in ContentSamples.NpcsByNetId.Values) {
                if (npc == null || npc.townNPC || ContainsNPC(npc.netID) || !tag.ContainsNPCIdInner(npc.netID) || NPCDataSet.NoBestiaryElementInheritence.Contains(npc.type)) {
                    continue;
                }

                AddNPC(npc.netID);
            }
        }
    }

    public void AddItemRelationsFromNPCDrops() {
        foreach (int i in _npcs) {
            if (NPCDataSet.NoDropElementInheritence.Contains(i)) {
                continue;
            }

            BestiaryEntry entry = Main.BestiaryDB.FindEntryByNPCID(i);
            if (entry == null) {
                continue;
            }

            foreach (ItemDropBestiaryInfoElement drops in entry.Info.SelectWhereOfType<ItemDropBestiaryInfoElement>()) {
                DropRateInfo info = Publicization<ItemDropBestiaryInfoElement, DropRateInfo>.GetField(drops, "_droprateInfo");
                if (!_manualItems.ContainsKey(info.itemId) && !ItemDataSet.NoNPCElementInheritence.Contains(info.itemId)) {
                    AddItemInner(info.itemId);
                }
            }
        }
    }

    public void AddItemRelationsFromRecipes() {
        bool any;
        do {
            any = false;
            ExtendRecipe.ForEachRecipe(r => {
                if (_items.Contains(r.createItem.type) || ItemDataSet.NoRecipeElementInheritence.Contains(r.createItem.type)) {
                    return true;
                }

                // Make result item inherit elements of parents.
                foreach (Item ingredient in r.requiredItem) {
                    if (_items.Contains(ingredient.type) && !_manualItems.ContainsKey(r.createItem.type)) {
                        any = true; // Keep looping so the inheritance trees can be fully complete.
                        AddItemInner(r.createItem.type);
                        return true;
                    }
                }

                return true;
            });
        }
        while (any);
    }
}
