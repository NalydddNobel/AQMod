using Terraria.ModLoader;

namespace Aequus.Common.DamageClasses {
    public abstract class DamageClassStat<TStat> {
        protected TStat[] arr;

        public DamageClassStat() {
            arr = new TStat[DamageClassLoader.DamageClassCount];
        }

        public void Clear() {
            for (int i = 0; i < arr.Length; i++) {
                arr[i] = default;
            }
        }

        public ref TStat Get(DamageClass damageClass) {
            return ref arr[damageClass.Type];
        }
        public ref TStat Get<T>() where T : DamageClass {
            return ref Get(ModContent.GetInstance<T>());
        }

        public abstract TStat GetTotalStats(DamageClass damageClass);
        public TStat GetTotalStats<T>() where T : DamageClass {
            return GetTotalStats(ModContent.GetInstance<T>());
        }
    }

    public class DamageClassStatFloat : DamageClassStat<float> {
        public override float GetTotalStats(DamageClass damageClass) {
            float result = Get(damageClass);
            for (int i = 0; i < arr.Length; i++) {
                if (i == damageClass.Type) {
                    continue;
                }
                var otherClass = AequusDamageClasses.DamageClasses[i];
                result += Get(otherClass) * damageClass.GetModifierInheritance(otherClass).damageInheritance;
            }
            return result;
        }
    }
}