using System.Reflection;
using Terraria.ModLoader;

namespace Aequus.Common.DataSets {
    public abstract class DataSet : ILoadable, IPostSetupContent, IAddRecipes {
        public void Load(Mod mod) {
            OnLoad(mod);
        }

        private void Clear(object obj) {
            if (obj == null) {
                return;
            }

            var m = obj.GetType().GetMethod("Clear", BindingFlags.Public | BindingFlags.Instance);
            var p = m.GetParameters();
            if (p.Length > 0) {
                return;
            }

            m.Invoke(obj, null);
        }

        public void Unload() {
            OnUnload();

            // Automatically clear data sets
            var t = GetType();
            var flags = BindingFlags.Public | BindingFlags.Static;
            foreach (var f in t.GetFields(flags)) {
                Clear(f.GetValue(this));
            }
            foreach (var p in t.GetProperties(flags)) {
                Clear(p.GetValue(this));
            }
        }

        public virtual void OnLoad(Mod mod) {
        }
        public virtual void OnUnload() {
        }
        public virtual void PostSetupContent(Aequus aequus) {
        }
        public virtual void AddRecipes(Aequus aequus) {

        }
    }
}
