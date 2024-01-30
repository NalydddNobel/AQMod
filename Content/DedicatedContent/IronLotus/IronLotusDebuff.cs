using Aequus.Content.DataSets;
using Aequus.Core.DataSets;

namespace Aequus.Content.DedicatedContent.IronLotus;

public class IronLotusDebuff : ModBuff {
    public static System.Int32 Damage { get; set; } = 300;
    public static System.Int32 DamageNumber { get; set; } = 15;

    public override System.String Texture => AequusTextures.TemporaryDebuffIcon;

    public override void SetStaticDefaults() {
        Main.debuff[Type] = true;
        BuffSets.PlayerDoTDebuff.Add((BuffEntry)Type);
    }

    public override void Update(NPC npc, ref System.Int32 buffIndex) {
        if (npc.TryGetGlobalNPC<IronLotusDebuffNPC>(out var debuff)) {
            debuff.hasDebuff = true;
        }
    }
}
