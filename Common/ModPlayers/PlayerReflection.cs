using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.ModPlayers {
    public class PlayerReflection : ILoadable {
        public static MethodInfo Player_SpawnHallucination;

        public void Load(Mod mod) {
            Player_SpawnHallucination = typeof(Player).GetMethod("SpawnHallucination", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public void Unload() {
            Player_SpawnHallucination = null;
        }
    }
}
