using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee.Nettlebane; 

public class NettlebaneOnHitProj : ModProjectile {
    public override string Texture => Aequus.BlankTexture;

    public override void SetDefaults() {
        Projectile.width = 160;
        Projectile.height = 160;
        Projectile.timeLeft = 6;
        Projectile.friendly = true;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = Projectile.timeLeft * 2;
        Projectile.penetrate = -1;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
    }

    public override void SendExtraAI(BinaryWriter writer) {
        writer.Write(Projectile.scale);
    }

    public override void ReceiveExtraAI(BinaryReader reader) {
        Projectile.scale = reader.ReadSingle();
    }
}