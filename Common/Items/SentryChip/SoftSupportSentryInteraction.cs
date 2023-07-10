using Aequus.Projectiles;
using System;
using Terraria;
using Terraria.DataStructures;

namespace Aequus.Common.Items.SentryChip {
    public class SoftSupportSentryInteraction : SentryInteraction {
        public Action<SentryAccessoryInfo> OnAI;
        public Action<IEntitySource, Projectile, AequusProjectile, SentryAccessoryInfo> OnShoot;

        public override SentryInteraction CreateRegisterInstance(int itemId) {
            return new SoftSupportSentryInteraction();
        }

        public override void OnSentryAI(SentryAccessoryInfo info) {
            OnAI?.Invoke(info);
        }

        public override void OnSentryCreateProjectile(IEntitySource source, Projectile newProjectile, AequusProjectile newAequusProjectile, SentryAccessoryInfo info) {
            OnShoot?.Invoke(source, newProjectile, newAequusProjectile, info);
        }
    }
}