using Aequus.Old.Content.Items.Weapons.Sentries.PhysicistSentry;
using System.Collections.Generic;

namespace Aequus.Old.Content.TownNPCs.PhysicistNPC;

public class PhysicistTownSentryProj : PhysicistSentryProj {
    public override string Texture => AequusTextures.PhysicistSentryProj.Path;

    public override void SetDefaults() {
        base.SetDefaults();
        Projectile.sentry = false;
        Projectile.DamageType = DamageClass.Generic;
        Projectile.timeLeft = 1800;
    }

    public override void OnSetup() {
        var list = new List<Projectile>();
        for (int i = 0; i < Main.maxProjectiles; i++) {
            var proj = Main.projectile[i];
            if (!proj.active || proj.type != Type) {
                continue;
            }
            list.Add(proj);
        }

        if (list.Count < 2) {
            return;
        }

        list.Sort((p, p2) => p.timeLeft.CompareTo(p2.timeLeft));
        list.Reverse();
        while (list.Count > 2) {
            list[2].Kill();
            list.RemoveAt(2);
        }
    }

    public override int GetTarget(Vector2 projectilePosition) {
        return Projectile.FindTargetWithLineOfSight(500f);
    }
}

public class PhysicistTownSentryLightning : PhysicistSentryLightning {
    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        ProjectileID.Sets.SentryShot[Type] = false;
    }

    public override void SetDefaults() {
        base.SetDefaults();
        Projectile.DamageType = DamageClass.Generic;
    }
}
