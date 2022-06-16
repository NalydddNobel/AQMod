using Aequus.Common;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Aequus.Content.CrossMod
{
    internal class ColoredDamageTypesSupport : IPostSetupContent
    {
        private ModData ColoredDamageTypes;

        public static Color NecromancyColor => new Color(200, 120, 230);

        void ILoadable.Load(Mod mod)
        {
        }

        void IPostSetupContent.PostSetupContent(Aequus aequus)
        {
            ColoredDamageTypes = new ModData("ColoredDamageTypes");
            //Mod.Call("AddDamageType", DamageClass DamageClassToBeAdded, Color TooltipColor, Color DamageColor, Color CritDamageColor)

            if (ColoredDamageTypes.Enabled)
            {
                ColoredDamageTypes.Call("AddDamageType", Aequus.NecromancyDamage, NecromancyColor, NecromancyColor, NecromancyColor * 1.25f);
            }
        }

        void ILoadable.Unload()
        {
            ColoredDamageTypes.Clear();
        }
    }
}