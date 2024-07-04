using AequusRemake.Core.Structures.ID;
using System;

namespace AequusRemake.Content.CrossMod.SplitSupport.Photography;

internal record struct AlbumQuestInfo(int Frame, IProvideId[] NPCIds, IProvideId Envelope, Predicate<NPC> SpecialCondition = null) {
    public AlbumQuestInfo(int Frame, IProvideId NPCId, IProvideId Envelope, Predicate<NPC> SpecialCondition = null)
        : this(Frame, new IProvideId[] { NPCId, }, Envelope, SpecialCondition) { }

    public IProvideId NPC => NPCIds[0];
    public ModItem Poster { get; internal set; }
}
