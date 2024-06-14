using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace Aequus.Core.Debugging.CheatCodes;

public interface ICheatCode {
    int Type { get; internal set; }

    string Name { get; }
    string DisplayName { get; internal set; }

    Params Params { get; }
    internal IStateProvider States { get; }

    internal bool IsPressed { get; set; }

    protected Keys[] Inputs { get; }

    public bool CheckKeys(in KeyboardState keyboard) {
        for (int i = 0; i < Inputs.Length; i++) {
            if (!keyboard.IsKeyDown(Inputs[i])) {
                return false;
            }
        }

        return true;
    }

    public bool MatchingKeys(ICheatCode other) {
        return Inputs.All(c => other.Inputs.Any(c2 => c == c2));
    }
}

internal abstract partial class CheatCode<T>(Params parameters, T stateProvider, params Keys[] inputs) : ModType, ICheatCode where T : IStateProvider {
    int ICheatCode.Type { get; set; }
    string ICheatCode.Name => Name;

    public string DisplayName { get; set; }

    public Params Params { get; init; } = parameters;

    IStateProvider ICheatCode.States => State;
    public T State => stateProvider;

    public Keys[] Inputs { get; init; } = inputs;

    bool ICheatCode.IsPressed { get; set; }

    protected sealed override void Register() {
        CheatCodeManager.Register(this);
    }

    public sealed override void SetupContent() {
        DisplayName = PrettyPrintName();
        SetStaticDefaults();
    }

    public override bool IsLoadingEnabled(Mod mod) {
#if DEBUG
        return true;
#else
        return !Params.HasFlag(Params.DebugOnly);
#endif
    }
}
