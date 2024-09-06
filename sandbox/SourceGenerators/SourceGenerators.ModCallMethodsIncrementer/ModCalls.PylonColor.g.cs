namespace Aequus;

public partial class ModCallHandler {
    [System.Runtime.CompilerServices.CompilerGenerated]
    static System.Func<Microsoft.Xna.Framework.Color> PylonColor(object[] args) {
        if (args[1] is not int tileType) {
            LogError($"Mod Call Parameter index 1 (\"tileType\") did not match Type \"int\".");
            return default;
        }

        int tileStyle = default;
        if (args.Length > 2) {
            if (args[2] is int optionalValue) {
                tileStyle = optionalValue;
            }
            else {
                LogError($"Optional Mod Call Parameter index 2 (\"tileStyle\") did not match Type \"int\".");
            }
        }

        Microsoft.Xna.Framework.Color? pylonColor = default;
        if (args.Length > 3) {
            if (args[3] is Microsoft.Xna.Framework.Color optionalValue) {
                pylonColor = optionalValue;
            }
            else {
                LogError($"Optional Mod Call Parameter index 3 (\"pylonColor\") did not match Type \"Microsoft.Xna.Framework.Color\".");
            }
        }

        return global::Aequus.AequusTile.GetSetPylonColor(tileType, tileStyle, pylonColor);
    }
}