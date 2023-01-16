using Terraria.ModLoader;

namespace Aequus.Common.Utilities.TypeUnboxing
{
    public abstract class TypeUnboxer<T> : ILoadable
    {
        public static TypeUnboxer<T> Instance;

        public void Load(Mod mod)
        {
            Instance = this;
        }
        public void Unload()
        {
            Instance = null;
        }

        public abstract bool TryUnbox(object obj, out T value);
        public virtual T Unbox(object obj)
        {
            TryUnbox(obj, out T value);
            return value;
        }
    }
}