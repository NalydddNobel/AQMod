using Aequus.Common;
using Aequus.NPCs;
using Microsoft.Xna.Framework;
using System;
using Terraria.ModLoader;

namespace Aequus.Content.CrossMod
{
    public class ModSupport : IPostSetupContent, IAddRecipes
    {
        public static Mod CalamityMod => CalamityModSupport.CalamityMod;
        public static Mod Polarities => PolaritiesSupport.Polarities;
        [Obsolete("Emote Bubble Lib is discontinued.")]
        public static Mod EmoteBubbleLib => EmoteBubbleLibSupport.EmoteBubbleLib.Mod;
        public static Mod TrueTooltips { get; private set; }
        public static Mod Fargowiltas { get; private set; }
        public static Mod ColoredDamageTypes { get; private set; }

        void ILoadable.Load(Mod mod)
        {
        }

        void IPostSetupContent.PostSetupContent(Aequus aequus)
        {
            TrueTooltips = FindMod(nameof(TrueTooltips));

            Fargowiltas = FindMod(nameof(Fargowiltas));

            ColoredDamageTypes = FindMod(nameof(ColoredDamageTypes));
            if (ColoredDamageTypes != null)
            {
                ColoredDamageTypesSupport();
            }
        }
        private static Mod FindMod(string name)
        {
            return ModLoader.TryGetMod(name, out var value) ? value : null;
        }
        private static void ColoredDamageTypesSupport()
        {
            //Mod.Call("AddDamageType", DamageClass DamageClassToBeAdded, Color TooltipColor, Color DamageColor, Color CritDamageColor)
            ColoredDamageTypes.Call("AddDamageType", NecromancyDamageClass.Instance,
                NecromancyDamageClass.NecromancyDamageColor, NecromancyDamageClass.NecromancyDamageColor, NecromancyDamageClass.NecromancyDamageColor * 1.25f);
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
            TrueTooltips = null;
            Fargowiltas = null;
            ColoredDamageTypes = null;
        }
    }
}