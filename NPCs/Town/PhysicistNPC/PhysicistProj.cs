using Aequus.Projectiles.Magic;

namespace Aequus.NPCs.Town.PhysicistNPC {
    public class PhysicistProj : UmystickBullet {
        public override string Texture => Helper.GetPath<UmystickBullet>();

        public override void SetDefaults() {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.npcProj = true;
        }
    }
}