using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Monsters.Jungle.Might
{
    [AutoloadBossHead()]
    public class CoreOfMight : BaseCore
    {
        public static List<EnemySpawnData> SpawnData { get; private set; }

        public override bool IsLoadingEnabled(Mod mod)
        {
            return true;
        }

        public override void Load()
        {
            base.Load();
            SpawnData = new List<EnemySpawnData>();
        }

        public override EnemySpawnData ChooseSpawnType(int index, int count)
        {
            return SpawnData[index % SpawnData.Count];
        }

        public override Color OnTileColor(float distance)
        {
            return new Color(2, 35, 128) * (1f - distance / 3000f) * AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 2f, 0.75f, 1f);
        }

        public override void AI()
        {
            base.AI();
            if (Main.netMode == NetmodeID.Server)
                return;
            Lighting.AddLight(NPC.Center, new Vector3(0f, 0.33f, 1f) * 4f);
            foreach (var c in AequusHelpers.CircularVector(20, NPC.localAI[1]))
            {
                Lighting.AddLight(NPC.Center + c * NPC.Size / 2f, new Vector3(0f, 0.33f, 1f) * 2f);
            }
            foreach (var e in Enemies)
            {
                foreach (var t in e.ConnectedTendril)
                {
                    float mult = 1.5f;
                    if (Collision.SolidCollision(t.drawLoc, 16, 16))
                    {
                        mult = 0.5f;
                    }
                    Lighting.AddLight(t.drawLoc, new Vector3(0f, 0.33f, 1f) * mult);
                }
            }
        }
    }
}