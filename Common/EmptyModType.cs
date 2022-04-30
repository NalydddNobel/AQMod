using Terraria.ModLoader;

namespace Aequus.Common
{
    public abstract class EmptyModType : ModType
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