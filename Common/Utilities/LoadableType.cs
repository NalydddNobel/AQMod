using Terraria.ModLoader;

namespace Aequus.Common.Utilities
{
    public abstract class LoadableType : ModType
    {
        protected sealed override void Register()
        {
        }
        public sealed override void SetupContent()
        {
            SetStaticDefaults();
        }
    }
}