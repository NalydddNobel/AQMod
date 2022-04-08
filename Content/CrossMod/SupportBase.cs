using Terraria.ModLoader;

namespace Aequus.Content.CrossMod
{
    internal abstract class SupportBase : ModType
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