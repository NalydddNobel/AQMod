using System.Collections.Generic;
using Terraria.ModLoader;

namespace Aequus.Common.Carpentry {
    public class BuildChallengeLoader : ILoadable {
        public static readonly List<BuildChallenge> registeredBuildChallenges = new();

        public static int BuildChallengeCount => registeredBuildChallenges.Count;

        public static int Register(BuildChallenge buildChallenge) {
            registeredBuildChallenges.Add(buildChallenge);
            return BuildChallengeCount - 1;
        }

        public void Load(Mod mod) {
        }

        public void Unload() {
        }

        public static BuildChallenge Find(string fullName) {
            return ModContent.Find<BuildChallenge>(fullName);
        }

        public static bool TryFind(string fullName, out BuildChallenge challenge) {
            return ModContent.TryFind(fullName, out challenge);
        }
    }
}