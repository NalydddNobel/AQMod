using Aequus.Common;
using Aequus.NPCs;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Aequus.Content.CrossMod
{
    // Warning: Fargowiltas.
    internal class FargowiltasSupport : IPostSetupContent, IAddRecipes
    {
        private static Mod fargowiltas;
        public static Mod Fargowiltas => fargowiltas;

        void ILoadable.Load(Mod mod)
        {
        }

        void IPostSetupContent.PostSetupContent(Aequus aequus)
        {
            if (ModLoader.TryGetMod("Fargowiltas", out fargowiltas))
            {
                return;
            }
            fargowiltas = null;
        }

        void IAddRecipes.AddRecipes(Aequus aequus)
        {
            if (Fargowiltas != null)
            {
                ShopQuotes.Database.GetNPC(AequusHelpers.NPCType(Fargowiltas, "Squirrel")).WithColor(Color.Gray * 1.66f);
            }
        }

        void ILoadable.Unload()
        {
            fargowiltas = null;
        }
    }
}