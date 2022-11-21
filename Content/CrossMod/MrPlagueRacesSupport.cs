using Aequus.Common;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.CrossMod
{
    public class MrPlagueRacesSupport : IPostSetupContent
    {
        public static Mod MrPlagueRaces { get; private set; }

        public static ModPlayer MrPlagueRacesPlayer { get; private set; }
        public static List<FieldInfo> RacePlayerFieldInfo { get; private set; }

        public void Load(Mod mod)
        {
            MrPlagueRaces = null;
            if (ModLoader.TryGetMod("MrPlagueRaces", out var mrPlagueRaces))
            {
                MrPlagueRaces = mrPlagueRaces;
                RacePlayerFieldInfo = new List<FieldInfo>();
            }
        }

        public static void LoadPlayerReferences()
        {
            RacePlayerFieldInfo.Clear();
            if (MrPlagueRaces.TryFind<ModPlayer>("MrPlagueRacesPlayer", out var mrPlagueRacesPlayer))
            {
                MrPlagueRacesPlayer = mrPlagueRacesPlayer;
                foreach (var f in MrPlagueRacesPlayer.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (f.FieldType == typeof(Color))
                    {
                        RacePlayerFieldInfo.Add(f);
                    }
                    else if (f.Name == "race")
                    {
                        RacePlayerFieldInfo.Add(f);
                    }
                }
            }
        }

        public void PostSetupContent(Aequus aequus)
        {
            LoadPlayerReferences();
        }

        public static bool TryGetMrPlagueRacePlayer(Player player, out ModPlayer racePlayer)
        {
            return player.TryGetModPlayer(MrPlagueRacesPlayer, out racePlayer);
        }

        public void Unload()
        {
            MrPlagueRaces = null;
        }
    }
}