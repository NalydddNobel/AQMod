using System.Collections.Generic;
using Terraria.ModLoader;

namespace Aequus.Common.Building {
    public class BuildChallengeLoader : ILoadable {
        public static readonly List<BuildChallenge> registeredBuildChallenges = new();
        public static readonly Dictionary<string, BuildChallenge> NameToBuildChallenge = new();

        public static int BuildChallengeCount => registeredBuildChallenges.Count;

        public static int Register(BuildChallenge buildChallenge) {
            registeredBuildChallenges.Add(buildChallenge);
            NameToBuildChallenge.Add(buildChallenge.FullName.Replace("Aequus/", ""), buildChallenge);
            return BuildChallengeCount - 1;
        }

        public void Load(Mod mod) {
        }

        public void Unload() {
            NameToBuildChallenge.Clear();
        }
    }
}