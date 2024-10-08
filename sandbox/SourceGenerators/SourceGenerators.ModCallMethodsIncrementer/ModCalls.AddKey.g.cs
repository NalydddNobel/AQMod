namespace Aequus;

public partial class ModCallHandler {
    [System.Runtime.CompilerServices.CompilerGenerated]
    static void AddKey(object[] args) {
        if (args[1] is not int itemId) {
            LogError($"Mod Call Parameter index 1 (\"itemId\") did not match Type \"int\".");
            return;
        }

        if (args[2] is not string texturePath) {
            LogError($"Mod Call Parameter index 2 (\"texturePath\") did not match Type \"string\".");
            return;
        }

        bool nonConsumable = default;
        if (args.Length > 3) {
            if (args[3] is bool optionalValue) {
                nonConsumable = optionalValue;
            }
            else {
                LogError($"Optional Mod Call Parameter index 3 (\"nonConsumable\") did not match Type \"bool\".");
            }
        }

        global::Aequus.Content.Systems.Keys.Keychains.KeychainDatabase.AddKeychainInfo(itemId, texturePath, nonConsumable);
    }
}