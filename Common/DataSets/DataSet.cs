using Aequus.Core.IO;
using Aequus.Core.Utilities;
using System.Collections.Generic;
using System.Reflection;
using Terraria.ModLoader;

namespace Aequus.Common.DataSets {
    public abstract class DataSet : ILoadable, IPostSetupContent, IAddRecipes, IPostAddRecipes {
        private readonly BindingFlags _memberFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

        protected virtual ContentFileInfo ContentFileInfo => default;
        protected JsonContentFile<string> ContentFile { get; private set; }

        private void LoadValue(JsonContentFile<string> file, string name, object value) {
            if (value is ICollection<int> iCollectionInt) {
                file.AddContentIDsToCollection(name, iCollectionInt);
            }
        }

        private void LoadContentFile(string loadOrderPrefix) {
            var t = GetType();
            foreach (var f in t.GetFields(_memberFlags)) {
                LoadValue(ContentFile, loadOrderPrefix + f.Name, f.GetValue(this));
            }
            foreach (var p in t.GetProperties(_memberFlags)) {
                LoadValue(ContentFile, loadOrderPrefix + p.Name, p.GetValue(this));
            }
        }

        public void Load(Mod mod) {
            if (ContentFileInfo.HasContentFile) {
                ContentFile = new(mod, $"DataSets/{GetType().Name}", ContentFileInfo.idDictionary);
                LoadContentFile("$");
            }
            OnLoad(mod);
        }

        private void Clear(object obj) {
            if (obj == null) {
                return;
            }

            var m = obj.GetType().GetMethod("Clear", BindingFlags.Public | BindingFlags.Instance);
            if (m == null) {
                return;
            }

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
            foreach (var f in t.GetFields(_memberFlags)) {
                Clear(f.GetValue(this));
            }
            foreach (var p in t.GetProperties(_memberFlags)) {
                Clear(p.GetValue(this));
            }
        }

        public virtual void OnLoad(Mod mod) {
        }

        public virtual void PostSetupContent() {
        }

        public void PostSetupContent(Aequus aequus) {
            if (ContentFile != null) {
                LoadContentFile("");
            }
            PostSetupContent();
        }

        public virtual void AddRecipes() {
        }

        public void AddRecipes(Aequus aequus) {
            if (ContentFile != null) {
                LoadContentFile("%");
            }
            AddRecipes();
        }

        public virtual void PostAddRecipes() {
        }

        public void PostAddRecipes(Aequus aequus) {
            PostAddRecipes();
        }

        public virtual void OnUnload() {
        }
    }
}
