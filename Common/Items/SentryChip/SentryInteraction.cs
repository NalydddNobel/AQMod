using Aequus.Projectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Aequus.Common.Items.SentryChip {
    public abstract class SentryInteraction : ILoadable {
        protected virtual void AddTo(int itemId) {
            SentryAccessoriesDatabase.Register(itemId, CreateRegisterInstance(itemId));
        }

        public virtual SentryInteraction CreateRegisterInstance(int itemId) {
            return this;
        }

        public virtual void Load(Mod mod) {
        }

        public virtual void Unload() {
        }

        public virtual void OnSentryAI(SentryAccessoryInfo info) {
        }

        public virtual void OnSentryCreateProjectile(IEntitySource source, Projectile newProjectile, AequusProjectile newAequusProjectile, SentryAccessoryInfo info) {
        }
    }
}