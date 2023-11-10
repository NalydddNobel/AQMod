using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Core;

public sealed class NewProjectileCache : ILoadable {
    public static readonly List<Projectile> Projectiles = new();
    public static bool QueueProjectiles;

    public void Load(Mod mod) {
        On_Projectile.NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float_float += On_Projectile_NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float_float;
        QueueProjectiles = false;
    }

    private static int On_Projectile_NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float_float(On_Projectile.orig_NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float_float orig, Terraria.DataStructures.IEntitySource spawnSource, float X, float Y, float SpeedX, float SpeedY, int Type, int Damage, float KnockBack, int Owner, float ai0, float ai1, float ai2) {
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