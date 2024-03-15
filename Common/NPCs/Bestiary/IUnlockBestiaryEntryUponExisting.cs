using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace Aequus.Common.NPCs.Bestiary;

internal interface IUnlockBestiaryEntryUponExisting {
    /// <summary>Automatically unlocks a bestiary entry when the NPC spawns into the world.</summary>
    internal sealed class UnlockUponExistingUICollectionInfoProvider : IBestiaryUICollectionInfoProvider {
        private readonly string _persistentIdentifierToCheck;

        public UnlockUponExistingUICollectionInfoProvider(ModNPC modNPC) : this(modNPC.NPC) { }
        public UnlockUponExistingUICollectionInfoProvider(NPC npc) : this(npc.GetBestiaryCreditId()) { }
        public UnlockUponExistingUICollectionInfoProvider(string persistentId) {
            _persistentIdentifierToCheck = persistentId;
        }

        public BestiaryUICollectionInfo GetEntryUICollectionInfo() {
            BestiaryEntryUnlockState state = BestiaryEntryUnlockState.NotKnownAtAll_0;
            if (ContentSamples.NpcNetIdsByPersistentIds.TryGetValue(_persistentIdentifierToCheck, out var npcId) && ModContent.GetInstance<BestiarySystem>()._npcTypesSpawned.Contains(npcId)) {
                state = BestiaryEntryUnlockState.CanShowDropsWithDropRates_4;
            }
            return new() {
                UnlockState = state,
            };
        }

        public UIElement ProvideUIElement(BestiaryUICollectionInfo info) {
            return null;
        }

        internal static void Save(TagCompound tag, BestiarySystem bestiarySystem) {
            try {
                List<string> spawnedNPCs = new List<string>();
                foreach (int npc in bestiarySystem._npcTypesSpawned) {
                    if (NPCID.Search.TryGetName(npc, out string internalName)) {
                        spawnedNPCs.Add(internalName);
                    }
                }

                if (spawnedNPCs.Count > 0) {
                    tag[BestiarySystem.SAVE_KEY_BESTIARY_UNLOCK_UPON_EXISTING] = spawnedNPCs;
                }
            }
            catch (Exception ex) {
                bestiarySystem.Mod.Logger.Error($"Failure saving UnlockUponExistingUICollectionInfoProvider.\n{ex}");
            }
        }

        internal static void Load(TagCompound tag, BestiarySystem bestiarySystem) {
            try {
                bestiarySystem._npcTypesSpawned.Clear();
                if (!tag.TryGet(BestiarySystem.SAVE_KEY_BESTIARY_UNLOCK_UPON_EXISTING, out List<string> list)) {
                    return;
                }

                var seperator = Aequus.MOD_NAME_SEPERATOR;
                foreach (string s in list) {
                    if (!s.Contains(seperator)) {
                        continue;
                    }

                    string[] modAndName = s.Split(seperator);
                    if (modAndName.Length < 2) {
                        continue;
                    }
                    string modName = modAndName[0];
                    string npcName = modAndName[1];

                    if (!ModLoader.TryGetMod(modName, out Mod mod)) {
                        continue;
                    }
                    if (!mod.TryFind(npcName, out ModNPC modNPC)) {
                        continue;
                    }

                    bestiarySystem._npcTypesSpawned.Add(modNPC.Type);
                }
            }
            catch (Exception ex) {
                bestiarySystem.Mod.Logger.Error($"Failure loading UnlockUponExistingUICollectionInfoProvider.\n{ex}");
            }
        }
    }

    internal sealed class UnlockUponExistingGlobalNPC : GlobalNPC {
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation) {
            return entity.ModNPC is IUnlockBestiaryEntryUponExisting;
        }

        public override void SetBestiary(NPC npc, BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
            if (npc.ModNPC is IUnlockBestiaryEntryUponExisting) {
                bestiaryEntry.UIInfoProvider = new UnlockUponExistingUICollectionInfoProvider(npc.ModNPC);
            }
        }

        public override void OnSpawn(NPC npc, IEntitySource source) {
            if (npc.ModNPC is IUnlockBestiaryEntryUponExisting) {
                ModContent.GetInstance<BestiarySystem>()._npcTypesSpawned.Add(npc.type);
            }
        }
    }
}