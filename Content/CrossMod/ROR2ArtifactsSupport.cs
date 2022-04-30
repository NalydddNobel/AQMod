using Aequus.Common;
using Terraria;

namespace Aequus.Content.CrossMod
{
    internal sealed class ROR2ArtifactsSupport : EmptyModType
    {
        public static ModData ROR2Artifacts;

        public override void SetStaticDefaults()
        {
            ROR2Artifacts = new ModData("ROR2Artifacts");
        }

        public static NPC GetParent(NPC me)
        {
            return (NPC)ROR2Artifacts.Call("GetParent", me);
        }

        public static NPC SetParent(NPC me, NPC parent)
        {
            return (NPC)ROR2Artifacts.Call("SetParent", me, parent);
        }
    }
}