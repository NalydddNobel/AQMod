using Microsoft.Xna.Framework.Input;
using System.Linq;
using Terraria.Localization;

namespace Aequus.Core.Debugging.CheatCodes;

internal abstract partial class CheatCode(Params parameters, params Keys[] inputs) : ModType, ILocalizedModType {
    public Params Params = parameters;
    public int Type { get; internal set; }
    public bool Active { get; protected set; } = false;

    public string LocalizationCategory => "Cheats";

    public LocalizedText DisplayName { get; private set; }
    public LocalizedText ToggleOnText { get; private set; }
    public LocalizedText ToggleOffText { get; private set; }

    protected readonly Keys[] _inputs = inputs;

    protected sealed override void Register() {
        CheatCodeManager.Register(this);
    }

    public sealed override void SetupContent() {
        DisplayName = this.GetLocalization("DisplayName", PrettyPrintName);
        ToggleOnText = Language.GetOrRegister("Mods.Aequus.Cheats.ToggleOn").WithFormatArgs(DisplayName);
        ToggleOffText = Language.GetOrRegister("Mods.Aequus.Cheats.ToggleOff").WithFormatArgs(DisplayName);
        SetStaticDefaults();
    }

    public bool CheckKeys(in KeyboardState keyboard) {
        for (int i = 0; i < _inputs.Length; i++) {
            if (!keyboard.IsKeyDown(_inputs[i])) {
                return false;
            }
        }

        return true;
    }

    public void Toggle() {
        Active = !Active;

        if (Active) {
            Main.NewText(ToggleOnText);
        }
        else {
            Main.NewText(ToggleOffText);
        }
    }

    internal void SetActive() {
        Active = true;
    }

    public static bool IsActive<T>() where T : CheatCode {
        return ModContent.GetInstance<T>()?.Active ?? false;
    }

    public bool MatchingKeys(CheatCode other) {
        return _inputs.All(c => other._inputs.Any(c2 => c == c2));
    }

    public override bool IsLoadingEnabled(Mod mod) {
#if DEBUG
        return true;
#else
        return !parameters.HasFlag(Params.DebugOnly);
#endif
    }
}
