using Aequus.Common.CrossMod;
using Aequus.Content.DamageClasses;
using Microsoft.Xna.Framework;
using System;
using Terraria.ModLoader;

namespace Aequus.CrossMod {
    internal class ColoredDamageTypes : ModSupport<ColoredDamageTypes> {
        private void AddDamageType(DamageClass damageClass, Color tooltipColor, Color damageColor, Color criticalHitColor) {
            try {
                Instance.Call("AddDamageType", damageClass,
                    tooltipColor, damageColor, criticalHitColor);
            }
            catch (Exception ex) {
                Mod.Logger.Error($"{ex.Message}\n{ex.StackTrace}");
            }
        }

        public override void PostSetupContent() {
            if (Instance == null) {
                return;
            }
            AddDamageType(Aequus.NecromancyClass, NecromancyClass.NecromancyDamageColor, NecromancyClass.NecromancyDamageColor, NecromancyClass.NecromancyDamageColor);
        }
    }
}