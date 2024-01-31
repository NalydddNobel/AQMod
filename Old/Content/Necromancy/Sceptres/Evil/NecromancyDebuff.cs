using Aequus.Old.Content.Necromancy.Networking;
using Aequus.Old.Content.Necromancy.Rendering;

namespace Aequus.Old.Content.Necromancy.Sceptres.Evil;

public class NecromancyDebuff : ModBuff {
    public override string Texture => AequusTextures.TemporaryDebuffIcon;

    public virtual float Tier => 1f;
    public virtual int DamageSet => 20;
    public virtual float BaseSpeed => 0.25f;

    public override void SetStaticDefaults() {
        Main.debuff[Type] = true;
        Main.buffNoSave[Type] = true;
    }

    public override void Update(NPC npc, ref int buffIndex) {
        var zombie = npc.GetGlobalNPC<NecromancyNPC>();
        zombie.ghostDebuffDOT = 16;
        zombie.ghostDamage = DamageSet;
        zombie.ghostSpeed = BaseSpeed;
        zombie.DebuffTier(Tier);
        zombie.RenderLayer(ColorTargetID.ZombieScepter);
    }

    public static void Apply<T>(NPC npc, int time, int player) where T : NecromancyDebuff {
        npc = npc.realLife == -1 ? npc : Main.npc[npc.realLife];

        float tier = ModContent.GetInstance<T>().Tier;
        bool cheat = tier >= 100;
        if (cheat) {
            npc.buffImmune[ModContent.BuffType<T>()] = false;
        }
        npc.AddBuff(ModContent.BuffType<T>(), time);
        npc.GetGlobalNPC<NecromancyNPC>().zombieOwner = player;
        if (Main.netMode == NetmodeID.MultiplayerClient) {
            Aequus.GetPacket<SyncNecromancyOwnerPacket>().Send(npc.whoAmI, player);
        }
    }
}