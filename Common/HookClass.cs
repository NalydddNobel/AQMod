using Terraria.ModLoader;

namespace Aequus.Common {
    public abstract class HookClass : ILoadable {
        public void Load(Mod mod) {
        }
        public abstract void LoadHooks(Mod mod);

        public void Unload() {
        }
        public virtual void UnloadHooks() {
        }
    }
}