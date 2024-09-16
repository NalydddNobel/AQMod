using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Aequus.Common.DamageClasses {
    public class AequusDamageClasses : IPostSetupContent {
        public static List<DamageClass> DamageClasses = new();

        public void Load(Mod mod) {
        }

        public void PostSetupContent() {
            DamageClasses.AddRange(new[] {
                DamageClass.Default,
                DamageClass.Generic,
                DamageClass.Melee,
                DamageClass.MeleeNoSpeed,
                DamageClass.Ranged,
                DamageClass.Magic,
                DamageClass.Summon,
                DamageClass.SummonMeleeSpeed,
                DamageClass.MagicSummonHybrid,
                DamageClass.Throwing
            });
            foreach (var damageClass in ModContent.GetContent<DamageClass>()) {
                DamageClasses.Add(damageClass);
            }
        }

        public void Unload() {
            DamageClasses.Clear();
        }
    }
}