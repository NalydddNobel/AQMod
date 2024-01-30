using System;
using System.Collections.Generic;

namespace Aequus;

public partial class AequusPlayer {
    public class TimerData {
        public Single TimePassed;
        public Int32 MaxTime;
        public String Name;

        public Boolean Active => TimePassed < MaxTime;
    }

    public Dictionary<String, TimerData> Timers;

    public Boolean TryGetTimer(String name, out TimerData timer) {
        return Timers.TryGetValue(name, out timer);
    }

    public Boolean TimerActive(String name) {
        return TryGetTimer(name, out var timer) && timer.Active;
    }

    public void SetTimer(String name, Int32 time) {
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

    public static Dictionary<String, TimerData> LocalTimers { get; internal set; } = new();
}