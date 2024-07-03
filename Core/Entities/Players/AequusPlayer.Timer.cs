using System;
using System.Collections.Generic;

namespace Aequu2;

public partial class AequusPlayer {
    public class TimerData {
        public float TimePassed;
        public int MaxTime;
        public string Name;

        public bool Active => TimePassed < MaxTime;
    }

    public Dictionary<string, TimerData> Timers;

    public bool TryGetTimer(string name, out TimerData timer) {
        return Timers.TryGetValue(name, out timer);
    }

    public bool TimerActive(string name) {
        return TryGetTimer(name, out var timer) && timer.Active;
    }

    public void SetTimer(string name, int time) {
        if (TryGetTimer(name, out var timer)) {
            if (!timer.Active) {
                timer.MaxTime = 0;
            }
            timer.TimePassed = 0f;
            timer.MaxTime = timer.TimePassed > timer.MaxTime ? time : Math.Max(timer.MaxTime, time);
            return;
        }

        Timers[name] = new() { MaxTime = time, Name = name, };
    }

    private void UpdateTimers() {
        foreach (var t in Timers) {
            t.Value.TimePassed++;
        }
    }

    public static Dictionary<string, TimerData> LocalTimers { get; internal set; } = new();
}