using System.IO;
using Terraria.ModLoader.IO;

namespace Aequu2.Core.Components.NPCs;

public interface ITrackTimeBetweenHits {
}

public class TrackTimeBetweenHits : GlobalNPC {
    public override bool InstancePerEntity => true;

    public int timeSinceLastHit;

    public override bool AppliesToEntity(NPC entity, bool lateInstantiation) {
        return entity.ModNPC is ITrackTimeBetweenHits;
    }

    public override void ResetEffects(NPC npc) {
        timeSinceLastHit++;
    }

    public override void HitEffect(NPC npc, NPC.HitInfo hit) {
        if (hit.Damage > 0) {
            timeSinceLastHit = 0;
        }
    }

    public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter) {
        binaryWriter.Write(timeSinceLastHit);
    }

    public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader) {
        timeSinceLastHit = binaryReader.ReadInt32();
    }
}

public static class TrackTimeBetweenHitsExtensions {
    public static int TimeSinceLastHit<T>(this T trackTimeBetweenHits) where T : ModNPC, ITrackTimeBetweenHits {
        return trackTimeBetweenHits.NPC.GetGlobalNPC<TrackTimeBetweenHits>().timeSinceLastHit;
    }
}
