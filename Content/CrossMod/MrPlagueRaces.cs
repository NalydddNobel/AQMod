using Aequus.Core.CrossMod;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Reflection;

namespace Aequus.Content.CrossMod;

internal class MrPlagueRaces : SupportedMod<MrPlagueRaces> {
    public static ModPlayer MrPlagueRacesPlayer { get; private set; }
    public static List<FieldInfo> RacePlayerFieldInfo { get; private set; }

    public override void OnLoad(Mod mod) {
        base.OnLoad(mod);
        RacePlayerFieldInfo = new List<FieldInfo>();
    }

    public override void PostSetupContent() {
        RacePlayerFieldInfo.Clear();
        if (Instance.TryFind<ModPlayer>("MrPlagueRacesPlayer", out var mrPlagueRacesPlayer)) {
            MrPlagueRacesPlayer = mrPlagueRacesPlayer;
            foreach (var f in MrPlagueRacesPlayer.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance)) {
                if (f.FieldType == typeof(Color)) {
                    RacePlayerFieldInfo.Add(f);
                }
                else if (f.Name == "race") {
                    RacePlayerFieldInfo.Add(f);
                }
            }
        }
    }

    public static bool TryGetMrPlagueRacePlayer(Player player, out ModPlayer racePlayer) {
        return player.TryGetModPlayer(MrPlagueRacesPlayer, out racePlayer);
    }

    public override void SafeUnload() {
        RacePlayerFieldInfo?.Clear();
        MrPlagueRacesPlayer = null;
    }
}