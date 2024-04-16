namespace Aequus.Content.Tools.Keychain;

public class KeychainPlayer : ModPlayer {
    public Keychain keyChain;

    public override void ResetEffects() {
        keyChain = null;
    }
}
