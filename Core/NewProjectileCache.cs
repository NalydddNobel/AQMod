using System.Collections.Generic;

namespace Aequus.Core;

public sealed class NewProjectileCache : ILoadable {
    public static readonly List<Projectile> Projectiles = new();
    public static System.Boolean QueueProjectiles { get; private set; }

    public void Load(Mod mod) {
        On_Projectile.NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float_float += On_Projectile_NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float_float;
        QueueProjectiles = false;
    }

    private static System.Int32 On_Projectile_NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float_float(On_Projectile.orig_NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float_float orig, Terraria.DataStructures.IEntitySource spawnSource, System.Single X, System.Single Y, System.Single SpeedX, System.Single SpeedY, System.Int32 Type, System.Int32 Damage, System.Single KnockBack, System.Int32 Owner, System.Single ai0, System.Single ai1, System.Single ai2) {
        if (QueueProjectiles) {
            var projectile = new Projectile();
            projectile.SetDefaults(Type);
            projectile.position.X = X - projectile.width / 2f;
            projectile.position.Y = Y - projectile.height / 2f;
            projectile.velocity.X = SpeedX;
            projectile.velocity.Y = SpeedY;
            projectile.damage = Damage;
            projectile.knockBack = KnockBack;
            projectile.owner = Owner;
            projectile.ai[0] = ai0;
            projectile.ai[1] = ai1;
            projectile.ai[2] = ai2;
            Projectiles.Add(projectile);
            return Main.maxProjectiles;
        }

        return orig(spawnSource, X, Y, SpeedX, SpeedY, Type, Damage, KnockBack, Owner, ai0, ai1, ai2);
    }

    public void Unload() {
    }

    public static void Begin() {
        Projectiles.Clear();
        QueueProjectiles = true;
    }

    public static void End() {
        QueueProjectiles = false;
    }
}