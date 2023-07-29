using System;
using System.Collections.Generic;
using System.IO;
using Terraria.ModLoader.IO;

namespace Aequus.Common.Carpentry;

public readonly struct BuildChallengeSaveData {
    public readonly List<string> Challenges;
    public readonly string SaveKey;

    public BuildChallengeSaveData(string saveKey) {
        SaveKey = saveKey;
        Challenges = new();
    }

    public void Clear() {
        Challenges?.Clear();
    }

    public void SaveData(TagCompound tag) {
        if (Challenges.Count > 0) {
            tag[SaveKey] = Challenges;
        }
    }

    public void LoadData(TagCompound tag) {
        Clear();
        if (tag.TryGet<List<string>>(SaveKey, out var saveData)) {
            Challenges.AddRange(saveData);
        }
    }

    public void Clone(BuildChallengeSaveData clone) {
        Challenges?.Clear();
        if (Challenges == null || clone.Challenges == null) {
            return;
        }

        Challenges.AddRange(clone.Challenges);
    }

    public bool ContainsChallenge(int id) {
        return ContainsChallenge(BuildChallengeLoader.registeredBuildChallenges[id]);
    }

    public bool ContainsChallenge(BuildChallenge buildChallenge) {
        return ContainsChallenge(buildChallenge.FullName);
    }

    public bool ContainsChallenge(string internalName) {
        return Challenges.Contains(internalName);
    }

    public bool ContainsAny(Predicate<string> predicate) {
        return Challenges.ContainsAny(predicate);
    }

    public void Add(BuildChallenge challenge) {
        Challenges.Add(challenge.FullName);
    }

    public void TrimData() {
        for (int i = 0; i < Challenges.Count - 1; i++) {
            for (int j = i + 1; j < Challenges.Count; j++) {
                if (Challenges[i] == Challenges[j]) {
                    Challenges.RemoveAt(j);
                    j--;
                }
            }
        }
    }

    public void SendData(BinaryWriter writer) {
        TrimData();
        List<int> l = new();
        for (int i = 0; i < Challenges.Count; i++) {
            if (BuildChallengeLoader.TryFind(Challenges[i], out var _)) {
                l.Add(i);
            }
        }

        writer.Write(l.Count);
        for (int i = 0; i < l.Count; i++) {
            writer.Write(l[i]);
        }
    }

    public void RecieveData(BinaryReader reader) {
        Challenges.Clear();
        int amt = reader.ReadInt32();
        var bounties = new List<int>();
        for (int i = 0; i < amt; i++) {
            var b = BuildChallengeLoader.registeredBuildChallenges[reader.ReadInt32()];
            Challenges.Add(b.FullName);
        }
    }
}