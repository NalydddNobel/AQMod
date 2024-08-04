using System;
using System.Collections.Generic;

namespace Aequus;

public sealed class TimerData(string Name, TimerPlayer Player) {
    public string Name { get; init; } = Name;

    public int Start { get; init; } = Player.timeElapsed;
    public int End { get; internal set; }

    public int Duration => End - Start;

    public int TimeElapsed => End - Player.timeElapsed;

    public bool Active() {
        return End > Player.timeElapsed;
    }
}

public sealed class TimerPlayer : ModPlayer {
    internal int timeElapsed;
    public Dictionary<string, TimerData> Timers = [];

    public bool TryGetTimer(string name, out TimerData? timer) {
        return Timers.TryGetValue(name, out timer);
    }

    public bool IsTimerActive(string name) {
        return TryGetTimer(name, out TimerData? timer) && timer!.Active();
    }

    public void SetTimer(string name, int duration) {
        int startTime = timeElapsed;
        int endTime = startTime + duration;

        if (TryGetTimer(name, out TimerData? timer)) {
            endTime = Math.Max(timer!.End, endTime);
        }

        Timers[name] = new TimerData(name, this) { End = endTime, };
    }

    public override void PreUpdate() {
        timeElapsed++;
    }

    public static Dictionary<string, TimerData> LocalTimers => Main.LocalPlayer.GetModPlayer<TimerPlayer>().Timers;
}
