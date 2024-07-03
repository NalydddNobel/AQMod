using System;

namespace Aequu2.Core.Debug.CheatCodes;

internal interface IStateProvider {
    void OnPress(ICheatCode code);
    void OnDepress(ICheatCode code) { }
    char WriteData();
    void ReadData(char data);
}

public class HoldStateProvider : IStateProvider {
    public bool Active { get; private set; }

    void IStateProvider.OnPress(ICheatCode code) {
        Active = true;
    }

    void IStateProvider.OnDepress(ICheatCode code) {
        Active = false;
    }

    void IStateProvider.ReadData(char data) {
        Active = data == '1';
    }

    char IStateProvider.WriteData() {
        return Active ? '1' : '0';
    }
}

public class ToggleStateProvider : IStateProvider {
    public bool Active { get; private set; }

    void IStateProvider.OnPress(ICheatCode code) {
        Active = !Active;

        if (Active) {
            Main.NewText($"{code.DisplayName}: on");
        }
        else {
            Main.NewText($"{code.DisplayName}: off");
        }
    }

    void IStateProvider.ReadData(char data) {
        Active = data == '1';
    }

    char IStateProvider.WriteData() {
        return Active ? '1' : '0';
    }
}

public class MultiStateProvider(params string[] states) : IStateProvider {
    public int CurrentState { get; private set; }

    private readonly string[] States = states;

    private static int DataOffset => '0';

    void IStateProvider.OnPress(ICheatCode code) {
        CurrentState = Math.Clamp((CurrentState + 1) % States.Length, 0, States.Length - 1);

        Main.NewText($"{code.DisplayName}: {States[CurrentState]}");
    }

    void IStateProvider.ReadData(char data) {
        CurrentState = Math.Clamp(data - DataOffset, 0, States.Length - 1);
    }

    char IStateProvider.WriteData() {
        return (char)(CurrentState + DataOffset);
    }
}