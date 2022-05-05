using System.Collections.Generic;
using Terraria.ID;

namespace Aequus.Common.Catalogues
{
    public class HeatDamageTypes : LoadableType
    {
        public static HashSet<int> HeatNPC { get; private set; }
        public static HashSet<int> HeatProjectile { get; private set; }

        public override void Load()
        {
            HeatNPC = new HashSet<int>()
            {
                NPCID.Lavabat,
                NPCID.LavaSlime,
                NPCID.FireImp,
                NPCID.MeteorHead,
                NPCID.HellArmoredBones,
                NPCID.HellArmoredBonesMace,
                NPCID.HellArmoredBonesSpikeShield,
                NPCID.HellArmoredBonesSword,
                NPCID.BlazingWheel,
            };

            HeatProjectile = new HashSet<int>()
            {
                ProjectileID.CultistBossFireBall,
                ProjectileID.CultistBossFireBallClone,
                ProjectileID.EyeFire,
                ProjectileID.GreekFire1,
                ProjectileID.GreekFire2,
                ProjectileID.GreekFire3,
            };
        }
    }
}
