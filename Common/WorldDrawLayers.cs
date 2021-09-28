using AQMod.Assets;
using AQMod.Assets.SceneLayers;
using AQMod.NPCs.Crabson;
using AQMod.NPCs.SiegeEvent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Common
{
    public static class WorldDrawLayers
    {
        private static bool _initialized;
        public static GoreNestWorldOverlay GoreNest { get; private set; }
        public static UltimateSwordWorldOverlay UltimateSword { get; private set; }

        internal static void Setup(bool test = false)
        {
            On.Terraria.Main.DrawNPCs += Main_DrawNPCs;
            GoreNest = new GoreNestWorldOverlay(test);
            UltimateSword = new UltimateSwordWorldOverlay();
            _initialized = true;
        }

        internal static void Unload()
        {
            GoreNest = null;
            UltimateSword = null;
        }

        public static void Update()
        {
            if (!_initialized)
            {
                return;
            }
            GoreNest.Update();
            UltimateSword.Update();
        }

        public static void Reset()
        {
            GoreNest.Reset();
            UltimateSword.Reset();
        }

        private static void Main_DrawNPCs(On.Terraria.Main.orig_DrawNPCs orig, Main self, bool behindTiles)
        {
            if (behindTiles)
            {
                int trapImpType = ModContent.NPCType<Trapper>();
                int crabsonType = ModContent.NPCType<JerryCrabson>();
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active)
                    {
                        if (Main.npc[i].type == crabsonType)
                        {
                            var crabsonPosition = Main.npc[i].Center;
                            DrawMethods.DrawJerryChain(new Vector2(crabsonPosition.X - 24f, crabsonPosition.Y), Main.npc[(int)Main.npc[i].localAI[0]].Center);
                            DrawMethods.DrawJerryChain(new Vector2(crabsonPosition.X + 24f, crabsonPosition.Y), Main.npc[(int)Main.npc[i].localAI[1]].Center);
                        }
                        else if (Main.npc[i].type == trapImpType)
                        {
                            var chainTexture = TextureCache.TrapperChain.GetValue();
                            int npcOwner = (int)Main.npc[i].ai[1] - 1;
                            int height = chainTexture.Height - 2;
                            var npcCenter = Main.npc[i].Center;
                            var trapImpCenter = Main.npc[npcOwner].Center;
                            Vector2 velocity = npcCenter - trapImpCenter;
                            int length = (int)(velocity.Length() / height);
                            velocity.Normalize();
                            velocity *= height;
                            float rotation = velocity.ToRotation() + MathHelper.PiOver2;
                            Vector2 origin = new Vector2(chainTexture.Width / 2f, chainTexture.Height / 2f);
                            for (int j = 1; j < length; j++)
                            {
                                var position = trapImpCenter + velocity * j;
                                var color = Lighting.GetColor((int)(position.X / 16), (int)(position.Y / 16f));
                                if (j < 6)
                                {
                                    color *= (1f / 6f) * j;
                                }
                                Main.spriteBatch.Draw(chainTexture, position - Main.screenPosition, null, color, rotation, origin, 1f, SpriteEffects.None, 0f);
                            }
                        }
                    }
                }
            }
            else
            {
                GoreNest.Draw();
                UltimateSword.Draw();
            }
            orig(self, behindTiles);
        }
    }
}