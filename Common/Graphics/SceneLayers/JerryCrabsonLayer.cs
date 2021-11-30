using AQMod.Common.Utilities;
using AQMod.NPCs.Boss.Crabson;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Common.Graphics.SceneLayers
{
    public class JerryCrabsonLayer : SceneLayer
    {
        public static LayerKey Key { get; private set; }

        public override string Name => "CrabsonChains";
        public override SceneLayering Layering => SceneLayering.BehindTiles_BehindNPCs;

        protected override void OnRegister(LayerKey key)
        {
            Key = key;
        }

        internal override void Unload()
        {
            Key = LayerKey.Null;
        }

        protected override void Draw()
        {
            int crabsonType = ModContent.NPCType<JerryCrabson>();
            var chain = ModContent.GetTexture(AQUtils.GetPath<JerryClaw>("_Chain"));
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active)
                {
                    if (Main.npc[i].type == crabsonType)
                    {
                        var crabsonPosition = Main.npc[i].Center;
                        Drawing.DrawJerryChain(chain, new Vector2(crabsonPosition.X - 24f, crabsonPosition.Y), Main.npc[(int)Main.npc[i].localAI[0]].Center);
                        Drawing.DrawJerryChain(chain, new Vector2(crabsonPosition.X + 24f, crabsonPosition.Y), Main.npc[(int)Main.npc[i].localAI[1]].Center);
                    }
                }
            }
        }
    }
}