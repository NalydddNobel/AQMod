using Aequus.Old.Content.Necromancy.Networking;
using Aequus.Old.Content.Necromancy.Rendering;

namespace Aequus.Old.Content.Necromancy.Sceptres.Evil;

public class NecromancyDebuff : ModBuff {
    public override string Texture => AequusTextures.TemporaryDebuffIcon;

    public virtual float Tier => 1f;
    public virtual int DamageSet => 20;
    public virtual float GhostSpeedBoost => 0.25f;

    public override void SetStaticDefaults() {
        Main.debuff[Type] = true;
        Main.buffNoSave[Type] = true;
        BuffSets.IsATagBuff[Type] = true;
    }

    public override void Update(NPC npc, ref int buffIndex) {
        var zombie = npc.GetGlobalNPC<NecromancyNPC>();
        zombie.ghostDebuffDOT = 16;
        zombie.ghostDamage = DamageSet;
        zombie.ghostSpeed = GhostSpeedBoost;
        zombie.DebuffTier(Tier);
        zombie.RenderLayer(ColorTargetID.ZombieScepter);
    }

    public static void Apply<T>(NPC npc, int time, int player) where T : NecromancyDebuff {
        npc = npc.realLife == -1 ? npc : Main.npc[npc.realLife];

        int buffId = ModContent.BuffType<T>();
        NecromancyDebuff buff = ModContent.GetInstance<T>();

        float tier = buff.Tier;
        bool cheat = tier >= 100;
        if (cheat) {
            npc.buffImmune[buffId] = false;
        }
        npc.AddBuff(buffId, time);
        npc.GetGlobalNPC<NecromancyNPC>().zombieOwner = player;

        // Do a single tick update when applying the debuff
        int index = npc.FindBuffIndex(buffId);
        if (index != -1) {
            buff.Update(npc, ref index);
        }

        if (Main.netMode == NetmodeID.MultiplayerClient) {
            Aequus.GetPacket<SyncNecromancyOwnerPacket>().Send(npc.whoAmI, player);
        }
    }
}