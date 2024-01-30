using Aequus.Content.DataSets;
using System.Diagnostics;
using System.IO;
using Terraria.ModLoader.IO;

namespace Aequus.Common.Renaming;

[LegacyName("NPCNameTag")]
[LegacyName("NameTagGlobalNPC")]
public sealed class RenameNPC : GlobalNPC {
    public System.Boolean HasCustomName => !System.String.IsNullOrEmpty(CustomName);

    public System.String CustomName { get; set; } = System.String.Empty;

    public override System.Boolean InstancePerEntity => true;

    public override System.Boolean AppliesToEntity(NPC entity, System.Boolean lateInstantiation) {
        return CanRename(entity);
    }

    public override System.Boolean PreAI(NPC npc) {
        if (npc.realLife != -1 && Main.npc[npc.realLife].TryGetGlobalNPC<RenameNPC>(out var nameTag)) {
            CustomName = nameTag.CustomName;
        }
        if (HasCustomName) {
            npc.GivenName = CustomName;
        }
        return true;
    }

    public override void SaveData(NPC npc, TagCompound tag) {
        if (HasCustomName && npc.realLife == -1) {
            if (npc.netID < NPCID.Count) {
                tag["ID"] = npc.netID; // Vanilla entities don't load properly for some reason! So I am doing this to save their ID for reloading properly.
            }

            tag["NameTag"] = CustomName;
        }
    }

    public override void LoadData(NPC npc, TagCompound tag) {
        var position = npc.position;

        // Workaround for vanilla entities not saving and loading properly
        if (npc.netID == 0 && tag.TryGet("ID", out System.Int32 netID)) {
            npc.netID = netID;
            npc.type = netID;
            npc.CloneDefaults(netID);
        }

        npc.position = position;
        npc.timeLeft = (System.Int32)(NPC.activeTime * 1.25f);
        npc.wet = Collision.WetCollision(npc.position, npc.width, npc.height);
        if (tag.TryGet("NameTag", out System.String savedNameTag)) {
            CustomName = savedNameTag;
        }

        LogCustomName(npc);
    }

    [Conditional("DEBUG")]
    private void LogCustomName(NPC npc) {
        if (HasCustomName) {
            Mod.Logger.Debug($"netID: {npc.netID}, {npc}");
            Mod.Logger.Debug(CustomName ?? "Null");
        }
    }

    public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter) {
        binaryWriter.Write(CustomName);
    }

    public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader) {
        CustomName = binaryReader.ReadString();
    }

    public static System.Boolean CanRename(NPC npc) {
        if (NPCSets.NameTagOverride.TryGetValue(npc.netID, out System.Boolean canBeRenamedOverride)) {
            return canBeRenamedOverride;
        }

        if (!npc.townNPC && !NPCID.Sets.SpawnsWithCustomName[npc.type] && (npc.boss || NPCID.Sets.ShouldBeCountedAsBoss[npc.type] || npc.immortal || npc.dontTakeDamage || npc.SpawnedFromStatue || (npc.realLife != -1 && npc.realLife != npc.whoAmI))) {
            return false;
        }

        // Respawn Id used by the Coin Loss Revenge system. If the id is 0, this enemy cannot utilize that system, nor can it utilize nametag saving/loading.
        if (NPCID.Sets.RespawnEnemyID.TryGetValue(npc.netID, out System.Int32 respawnId) && respawnId == 0) {
            return false;
        }

        return true;
    }
}