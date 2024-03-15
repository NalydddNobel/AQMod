using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Common.NPCs.Bestiary;

public class BestiarySystem : ModSystem {
    public readonly HashSet<int> _npcTypesSpawned = new();
    public const string SAVE_KEY_BESTIARY_UNLOCK_UPON_EXISTING = "BUUO";

    public override void ClearWorld() {
        _npcTypesSpawned.Clear();
    }

    public override void SaveWorldData(TagCompound tag) {
        IUnlockBestiaryEntryUponExisting.UnlockUponExistingUICollectionInfoProvider.Save(tag, this);
    }

    public override void LoadWorldData(TagCompound tag) {
        IUnlockBestiaryEntryUponExisting.UnlockUponExistingUICollectionInfoProvider.Load(tag, this);
    }
}
