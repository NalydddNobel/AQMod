using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Necromancy.Sceptres.Zombie.Corruption;

public class ZombieScepterMinion : ModProjectile {
    public override string Texture => Aequus.NPCTexture(NPCID.Ghost);

    public override void SetStaticDefaults() {
        Main.projFrames[Type] = Main.npcFrameCount[NPCID.Ghost];
    }

    public override void SetDefaults() {
        Projectile.DisableWorldInteractions();
        Projectile.DamageType = Aequus.NecromancyClass;
    }

    public override Color? GetAlpha(Color lightColor) {
        return lightColor * 1.5f * 0.8f;
    }

    public override void AI() {
        if (Main.player[Projectile.owner].HeldItemFixed().shoot == Type) {
            Projectile.timeLeft = 2;
        }

        if (!Main.player[Projectile.owner].ItemTimeIsZero) {
            Projectile.friendly = true;
            Projectile.GenericMovementTo(Main.MouseWorld, 16f, 0.15f, 20f);
        }
        else {
            Projectile.friendly = false;
            if (!Projectile.GenericMovementTo(Main.player[Projectile.owner].Center + new Vector2(Main.player[Projectile.owner].direction * -40, -36f), 12f, 0.1f, 32f)) {
                Projectile.velocity *= 0.9f;
            }
        }
        Projectile.rotation = Projectile.velocity.X * 0.02f;
        Projectile.spriteDirection = -Projectile.direction;
        Projectile.scale = 1f + Helper.Wave(Main.GlobalTimeWrappedHourly * 5f, -0.075f, 0.075f);
    }

    public override bool PreDraw(ref Color lightColor) {
        return true;
    }
}