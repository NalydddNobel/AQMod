using Aequus.Content.DataSets;

namespace Aequus.Content.DedicatedContent.IronLotus;

public class IronLotusDebuff : ModBuff {
    public static int Damage { get; set; } = 300;
    public static int DamageNumber { get; set; } = 15;

    public override string Texture => AequusTextures.TemporaryDebuffIcon;

    public override void SetStaticDefaults() {
        Main.debuff[Type] = true;
        BuffSets.PlayerDoTDebuff.Add(Type);
    }

    public override void Update(NPC npc, ref int buffIndex) {
        if (npc.TryGetGlobalNPC<IronLotusDebuffNPC>(out var debuff)) {
            debuff.hasDebuff = true;
        }
    }
}
