namespace Aequus.Core.Structures;

public interface IEntityAnchor {
    int Index { get; set; }
    bool IsActive { get; }
}

public struct NPCAnchor : IEntityAnchor {
    public int Index { get; set; }

    public readonly bool IsActive => Main.npc.IndexInRange(Index) && Main.npc[Index].active;

    public NPCAnchor() {
        Index = -1;
    }

    public static implicit operator NPCAnchor(int index) {
        return new NPCAnchor() { Index = index };
    }

    public static implicit operator int(NPCAnchor npcAnchor) {
        return npcAnchor.Index;
    }

    public static bool operator ==(NPCAnchor me, NPCAnchor other) {
        return me.Index == other.Index;
    }

    public static bool operator !=(NPCAnchor me, NPCAnchor other) {
        return me.Index != other.Index;
    }

    public static bool operator <(NPCAnchor me, NPCAnchor other) {
        return me.Index < other.Index;
    }

    public static bool operator  >(NPCAnchor me, NPCAnchor other) {
        return me.Index > other.Index;
    }
}
