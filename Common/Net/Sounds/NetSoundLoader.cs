using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.Net.Sounds {
    public class NetSoundLoader : ILoadable {
        private static List<NetSound> registeredSounds = new();
        public static int Count => registeredSounds.Count;

        public static void Register(NetSound netSound) {
            if (Count > byte.MaxValue) {
                throw new System.Exception("Net sound limit reached. Increase cap from byte to ushort!");
            }
            string name = netSound.FullName;
            if (registeredSounds.Find(s => s.FullName.Equals(name)) != null) {
                throw new System.Exception($"Two net sounds with the same name have been registered by {netSound.Mod.Name}. ({netSound.Name})");
            }

            netSound.Type = (byte)Count;
            registeredSounds.Add(netSound);
        }

        public static NetSound ByID(int id) {
            return registeredSounds.IndexInRange(id) ? registeredSounds[id] : null;
        }

        public static int NetSoundType<T>() where T : NetSound {
            return ModContent.GetInstance<T>().Type;
        }

        void ILoadable.Load(Mod mod) {
        }

        void ILoadable.Unload() {
            registeredSounds?.Clear();
        }
    }
}