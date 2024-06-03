using Aequus.Core.CrossMod;
using System;

namespace Aequus.Content.CrossMod.SplitSupport.Photography;

internal record struct AlbumQuestInfo(int Frame, IContentIdProvider[] NPCIds, IContentIdProvider Envelope, Predicate<NPC> SpecialCondition = null) {
    public AlbumQuestInfo(int Frame, IContentIdProvider NPCId, IContentIdProvider Envelope, Predicate<NPC> SpecialCondition = null)
        : this(Frame, new IContentIdProvider[] { NPCId, }, Envelope, SpecialCondition) { }

    public IContentIdProvider NPC => NPCIds[0];
    public ModItem Poster { get; internal set; }
}
