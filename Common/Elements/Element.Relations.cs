using Aequus.Common.Bestiary;
using Aequus.Core.CodeGeneration;
using System.Collections.Generic;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Common.Elements;

public partial class Element {
    private readonly HashSet<int> _npcs = [];
    private readonly HashSet<int> _items = [];

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
        return _items.Add(i);
    }

    public bool RemoveItem(int i) {
        return _items.Remove(i);
    }

    public void AddBestiaryRelations(params BestiaryTags[] tags) {
        foreach (var tag in tags) {
            foreach (NPC npc in ContentSamples.NpcsByNetId.Values) {
                if (npc != null && !npc.townNPC && !ContainsNPC(npc.netID) && tag.ContainsNPCIdInner(npc.netID)) {
                    AddNPC(npc.netID);
                }
            }
        }
    }

    public void AddItemRelationsFromNPCs() {
        foreach (int i in _npcs) {
            BestiaryEntry entry = Main.BestiaryDB.FindEntryByNPCID(i);
            if (entry == null) {
                continue;
            }

            foreach (ItemDropBestiaryInfoElement drops in entry.Info.SelectWhereOfType<ItemDropBestiaryInfoElement>()) {
                DropRateInfo info = Publicization<ItemDropBestiaryInfoElement, DropRateInfo>.GetField(drops, "_droprateInfo");
                AddItem(info.itemId);
            }
        }

        // Remove common items.
        RemoveItem(ItemID.Gel);
        RemoveItem(ItemID.WoodenArrow);
        RemoveItem(ItemID.Shackle);
        RemoveItem(ItemID.ZombieArm);
        RemoveItem(ItemID.SpiffoPlush);
        RemoveItem(ItemID.Hook);
        RemoveItem(ItemID.SharkFin);
        RemoveItem(ItemID.BoneSword);
        RemoveItem(ItemID.SlimeStaff);
    }

    public void AddItemRelationsFromRecipes() {
        bool any;
        do {
            any = false;
            ExtendRecipe.ForEachRecipe(r => {
                if (_items.Contains(r.createItem.type)) {
                    return true;
                }

                // Make result item inherit elements of parents.
                foreach (Item ingredient in r.requiredItem) {
                    if (_items.Contains(ingredient.type)) {
                        any = true; // Keep looping so the inheritance trees can be fully complete.
                        AddItem(r.createItem.type);
                        return true;
                    }
                }

                return true;
            });
        }
        while (any);
    }
}
