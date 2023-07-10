using Aequus.Common.Items.SentryChip;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Items.SentryChip {
    public class InnerTubeInteraction : SentryInteraction {
        public override void Load(Mod mod) {
            AddTo(ItemID.FloatingTube);
        }

        public override void OnSentryAI(SentryAccessoryInfo info) {
            if (Collision.WetCollision(info.Projectile.position, info.Projectile.width, info.Projectile.height)) {
                info.Projectile.velocity.Y -= 0.4f;
                if (!Collision.WetCollision(info.Projectile.position + info.Projectile.velocity, info.Projectile.width, info.Projectile.height)) {
                    info.Projectile.velocity.Y = 0f;
                }
            }
        }
    }
}