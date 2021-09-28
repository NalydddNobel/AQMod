using AQMod.Assets;

namespace AQMod.Items.GrapplingHooks
{
    public static class ChainTextures
    {
        public static TEA<ChainTextureID> Chains { get; private set; }

        internal static void Setup()
        {
            Chains = new TEA<ChainTextureID>(ChainTextureID.Count, "AQMod/Items/GrapplingHooks/Chains", "Chain");
        }
    }
}