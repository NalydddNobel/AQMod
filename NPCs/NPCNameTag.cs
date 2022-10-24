using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.NPCs
{
    public class NPCNameTag : GlobalNPC
    {
        public string NameTag;

        public bool HasNameTag => NameTag != null;

        public override bool InstancePerEntity => true;

        public override bool PreAI(NPC npc)
        {
            // Isn't very sync supportive, but blah, it's a name tag.
            // Not too concerning if worm names don't sync for half a second... right.
            if (npc.realLife != -1 && Main.GameUpdateCount % 30 == 0 && Main.npc[npc.realLife].TryGetGlobalNPC<NPCNameTag>(out var nameTag))
            {
                NameTag = nameTag.NameTag;
            }
            if (HasNameTag)
            {
                npc.GivenName = NameTag;
            }
            return true;
        }

        public override bool NeedSaving(NPC npc)
        {
            return NameTag != null;
        }

        public override void SaveData(NPC npc, TagCompound tag)
        {
            if (HasNameTag && npc.realLife == -1)
            {
                if (npc.netID < Main.maxNPCTypes)
                    tag["ID"] = npc.netID; // Vanilla entities don't load properly for some reason! So I am doing this to save their ID for reloading properly.
                tag["NameTag"] = NameTag;
            }
        }

        public override void LoadData(NPC npc, TagCompound tag)
        {
            var position = npc.position;
            if (npc.netID == 0 && tag.TryGet("ID", out int netID))
            {
                npc.netID = netID;
                npc.type = netID;
                npc.CloneDefaults(netID); // ??????????????? 
            }
            npc.position = position;
            npc.timeLeft = (int)(NPC.activeTime * 1.25f);
            npc.wet = Collision.WetCollision(npc.position, npc.width, npc.height);
            if (tag.TryGet("NameTag", out string savedNameTag))
                NameTag = savedNameTag;

            if (HasNameTag && Aequus.LogMore)
            {
                Mod.Logger.Debug($"netID: {npc.netID}, {npc}");
                Mod.Logger.Debug(NameTag == null ? "Null" : NameTag);
            }
        }
    }
}