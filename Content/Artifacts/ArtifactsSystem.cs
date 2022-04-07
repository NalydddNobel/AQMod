using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Artifacts
{
    public sealed class ArtifactsSystem : ModSystem
    {
        public static bool CommandGameMode;
        public static bool SwarmsGameMode;

        public static HashSet<int> SwarmsNPCBlacklist { get; private set; }

        public override void Load()
        {
            SwarmsNPCBlacklist = new HashSet<int>()
            {
                NPCID.BrainofCthulhu,
                NPCID.LunarTowerSolar,
                NPCID.LunarTowerVortex,
                NPCID.LunarTowerNebula,
                NPCID.LunarTowerStardust,
                NPCID.GolemHead,
                NPCID.Golem,
                NPCID.GolemFistLeft,
                NPCID.GolemFistRight,
                NPCID.CultistBossClone,
                NPCID.AncientDoom,
                NPCID.MartianSaucerCannon,
                NPCID.MartianSaucerTurret,
                NPCID.MoonLordHand,
                NPCID.MoonLordHead,
                NPCID.MoonLordLeechBlob,
                NPCID.SkeletronHand,
                NPCID.PrimeCannon,
                NPCID.PrimeLaser,
                NPCID.PrimeSaw,
                NPCID.PrimeVice,
                NPCID.TheHungryII,
                NPCID.Slimer2,
                NPCID.PumpkingBlade,
                NPCID.PirateShipCannon,
                NPCID.MothronSpawn,
                NPCID.WindyBalloon,
                NPCID.VortexLarva,
                NPCID.StardustCellSmall,
                NPCID.Slimeling,
            };
        }

        public override void PreUpdatePlayers()
        {
            CommandGameMode = false;
            SwarmsGameMode = false;
        }
    }
}