using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.GameContent.Bestiary;

namespace Aequus.Core.Entites.Bestiary;

public partial class BestiaryTags(IEnumerable<IFilterInfoProvider> ValidTags) {
    public BestiaryTags(params IFilterInfoProvider[] ValidTags) : this(ValidTags as IEnumerable<IFilterInfoProvider>) { }
    public BestiaryTags(params BestiaryTags[] validTags) : this(validTags.SelectMany(t => t._tags)) { }

    private IFilterInfoProvider[] _tags = ValidTags.ToArray();
    private string[] _keys = ValidTags.Select(t => t.GetDisplayNameKey()).ToArray();

    public bool Contains(IFilterInfoProvider compareTag) {
        return _keys.Any(s => compareTag.GetDisplayNameKey() == s);
    }

    public bool Contains(IEnumerable<IBestiaryInfoElement> bestiaryElements) {
        return bestiaryElements?.SelectWhereOfType<IFilterInfoProvider>()?.Any(Contains) ?? false;
    }

    private readonly Dictionary<int, bool> _npcIdCache = [];

    public bool ContainsNPCId(int i) {
        // Return lookup if one exists.
        if (_npcIdCache.TryGetValue(i, out bool existingResult)) {
            return existingResult;
        }

        bool result = ContainsNPCIdInner(i);
        // Cache result for any repeat lookups.
        _npcIdCache[i] = result;
        return result;
    }

    internal bool ContainsNPCIdInner(int i) {
        // Get NPC bestiary entry.
        BestiaryEntry bestiaryEntry = Main.BestiaryDB.FindEntryByNPCID(i);
        if (bestiaryEntry == null || bestiaryEntry.Info == null) {
            return false;
        }

        return Contains(bestiaryEntry.Info);
    }

    /// <summary>Call in PostSetupContent.</summary>
    public void Add(ModBiome biome) {
        Add(biome.ModBiomeBestiaryInfoElement);
    }

    public void Add(IFilterInfoProvider filterInfoProvider) {
        Array.Resize(ref _tags, _tags.Length + 1);
        Array.Resize(ref _keys, _keys.Length + 1);
        _tags[^1] = filterInfoProvider;
        _keys[^1] = filterInfoProvider.GetDisplayNameKey();
        _npcIdCache.Clear();
    }
}
