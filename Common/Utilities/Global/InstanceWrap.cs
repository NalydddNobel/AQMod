global using static Aequus.Common.Utilities.Global.InstanceWrap;

namespace Aequus.Common.Utilities.Global;

public static class InstanceWrap {
    public static T Instance<T>() where T : class {
        return ModContent.GetInstance<T>();
    }
}
