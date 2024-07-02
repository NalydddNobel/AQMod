using Aequus.Core.Structures;

namespace Aequus.Core;

public partial class Commons {
    public class Refs {
        public static readonly ReferenceCache<bool> BloodMoon = new ReferenceCache<bool>(() => ref Main.bloodMoon);
        public static readonly ReferenceCache<bool> Eclipse = new ReferenceCache<bool>(() => ref Main.eclipse);
        public static readonly ReferenceCache<bool> FrostMoon = new ReferenceCache<bool>(() => ref Main.snowMoon);
        public static readonly ReferenceCache<bool> PumpkinMoon = new ReferenceCache<bool>(() => ref Main.pumpkinMoon);
        public static readonly ReferenceCache<bool> DayTime = new ReferenceCache<bool>(() => ref Main.dayTime);
        public static readonly ReferenceCache<double> Time = new ReferenceCache<double>(() => ref Main.time);
    }
}
