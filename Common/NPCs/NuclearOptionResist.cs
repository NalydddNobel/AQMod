using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.NPCs;

internal class NuclearOptionResist : GlobalNPC {
    public override bool IsLoadingEnabled(Mod mod) {
        return ModLoader.HasMod("HeavensMechanic");
    }

    public override bool AppliesToEntity(NPC npc, bool lateInstantiation) {
        return npc.ModNPC?.Equals(Mod) ?? false;
    }

    public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers) {
        if (projectile.TryGetGlobalProjectile<NuclearOptionResistProjectile>(out _)) {
            npc.life = npc.lifeMax;
            modifiers.SetMaxDamage(1);
        }
    }

    public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers) {
        if (NuclearOptionResistProjectile.Active) {
            npc.life = npc.lifeMax;
            modifiers.SetMaxDamage(1);
        }
    }

    public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone) {
        if (projectile.TryGetGlobalProjectile<NuclearOptionResistProjectile>(out _)) {
            npc.life = npc.lifeMax;
        }
    }

    public override void HitEffect(NPC npc, NPC.HitInfo hit) {
        if (NuclearOptionResistProjectile.Active) {
            npc.life = npc.lifeMax;
        }
    }
}

internal class NuclearOptionResistProjectile : GlobalProjectile {
    public static bool Active;

    public override bool IsLoadingEnabled(Mod mod) {
        return ModLoader.HasMod("HeavensMechanic");
    }

    public override bool AppliesToEntity(Projectile projectile, bool lateInstantiation) {
        return projectile.ModProjectile?.Name?.Equals("NuclearOptionProj") ?? false;
    }

    public override bool PreAI(Projectile projectile) {
        Active = true;
        return true;
    }

    public override void PostAI(Projectile projectile) {
        Active = false;
    }
}
