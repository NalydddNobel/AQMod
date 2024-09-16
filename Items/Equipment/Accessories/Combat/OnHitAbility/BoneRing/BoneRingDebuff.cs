using Aequus.Common.Buffs;
using Aequus.Common.DataSets;
using Aequus.Common.Net.Sounds;
using Aequus.Common.NPCs.Global;

namespace Aequus.Items.Equipment.Accessories.Combat.OnHitAbility.BoneRing;
public class BoneRingDebuff : ModBuff, IAddRecipeGroups, BuffHooks.IOnAddBuff {
    public override string Texture => Aequus.VanillaTexture + "Buff_" + BuffID.Weak;
    public static float MovementSpeedMultiplier = 0.4f;

    public override void SetStaticDefaults() {
        Main.debuff[Type] = true;
        Main.buffNoSave[Type] = true;

        AequusBuff.SetImmune(NPCID.WallofFleshEye, false, Type);
        BuffSets.PlayerStatusDebuff.Add(Type);
    }

    public void AddRecipeGroups() {
        foreach (var n in NPCSets.StatSpeedBlacklist) {
            AequusBuff.SetImmune(n, false, Type);
        }
    }

    public override void Update(NPC npc, ref int buffIndex) {
        if (npc.TryGetGlobalNPC<StatSpeedGlobalNPC>(out var statSpeedGlobalNPC)) {
            statSpeedGlobalNPC.statSpeed *= MovementSpeedMultiplier;
        }
        npc.Aequus().debuffBoneRing = true;
    }

    public void PostAddBuff(NPC npc, int duration, bool quiet) {
        if (npc.HasBuff<BoneRingDebuff>()) {
            ModContent.GetInstance<WeaknessDebuffSound>().Play(npc.Center);
        }
    }
}