using Aequus.Buffs.Debuffs;
using Aequus.Graphics;
using Aequus.Graphics.RenderTargets;
using Aequus.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Common.GlobalNPCs
{
    public class BitCrushedGlobalNPC : GlobalNPC
    {
        public bool CheckUpdateNPC(NPC npc, int i)
        {
            if (AequusHelpers.iterations == 0 && npc.HasBuff<BitCrushedDebuff>())
            {
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    return false;
                }
                int rate = 7;
                if (Main.GameUpdateCount % rate == 0)
                {
                    for (int k = 0; k < rate - 1; k++)
                    {
                        AequusHelpers.iterations = k + 1;
                        npc.UpdateNPC(i);
                    }
                    AequusHelpers.iterations = 0;
                }
                return false;
            }
            return true;
        }

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!npc.IsABestiaryIconDummy)
            {
                if (npc.HasBuff<BitCrushedDebuff>())
                {
                    var r = EffectsSystem.EffectRand;
                    r.SetRand((int)(Main.GlobalTimeWrappedHourly * 32f) / 10 + npc.whoAmI * 10);
                    int amt = Math.Max((npc.width + npc.height) / 20, 1);
                    for (int k = 0; k < amt; k++)
                    {
                        var dd = new DrawData(ParticleTextures.gamestarParticle.Texture.Value, npc.Center - Main.screenPosition, null, Color.White, 0f, ParticleTextures.gamestarParticle.Origin, Vector2.Zero, SpriteEffects.None, 0);
                        if (k != 0)
                        {
                            dd.position.X += (int)r.Rand(-npc.width, npc.width);
                            dd.position.Y += (int)r.Rand(-npc.height, npc.height);
                            dd.scale = new Vector2((int)r.Rand(amt * 2, amt * 5));
                        }
                        else
                        {
                            dd.scale = new Vector2(npc.frame.Width, npc.frame.Height);
                        }
                        GamestarRenderer.DrawData.Add(dd);
                    }
                }
            }
            return true;
        }
    }
}
