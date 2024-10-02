using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.Localization;

namespace Aequus.Common.Utilities;

internal class LocalizationErrorHandler : LoadedType {
    private static HashSet<string> _errors;

    protected override void Load() {
        On_LocalizedText.Format += On_LocalizedText_Format;
    }

    static string On_LocalizedText_Format(On_LocalizedText.orig_Format orig, LocalizedText self, object[] args) {
        try {
            return orig(self, args);
        }
        catch (Exception ex) {
            if (!self.Key.StartsWith("Mods.Aequus")) {
                throw;
            }

            LogLocalizationError(ex);
        }
        return self.Value;

        void LogLocalizationError(Exception ex) {
            string identifier = ex.StackTrace ?? self.Key;
            if (!string.IsNullOrEmpty(identifier)) {
                if (_errors.Contains(identifier)) {
                    return;
                }
                _errors.Add(identifier);
            }
            if (Aequus.Instance?.Logger != null) {
                Aequus.Instance.Logger.Error($"Error formatting \"{self.Value}\" with args {string.Join(", ", args.Select(o => o?.ToString() ?? "nullref"))}");
            }
        }
    }
}
