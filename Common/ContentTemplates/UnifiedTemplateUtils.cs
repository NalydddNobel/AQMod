namespace Aequus.Common.ContentTemplates;

internal static class UnifiedTemplateUtils {
    public static T AddContent<T>(this IUnifiedTemplate unified, T modType) where T : ModType {
        unified.ToLoad.Add(modType);
        return modType;
    }

    public static void AddAllContent(this IUnifiedTemplate unified) {
        foreach (ModType m in unified.ToLoad) {
            unified.Mod.AddContent(m);
        }
    }
}
