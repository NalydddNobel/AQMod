using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Aequus.Buffs.Misc.Empowered
{
    public abstract class EmpoweredBuffBase : ModBuff
    {
        public abstract int OriginalBuffType { get; }
        public virtual float StatIncrease => 1f;

        public override void Update(Player player, ref int buffIndex)
        {
            for (int i = buffIndex + 1; i < Player.MaxBuffs; i++)
            {
                if (player.buffType[i] == OriginalBuffType && player.buffTime[i] > 1)
                {
                    player.buffType[buffIndex] = player.buffType[i];
                    player.buffTime[buffIndex] = Math.Max(player.buffTime[buffIndex], player.buffTime[i]);
                    player.DelBuff(i);
                    break;
                }
            }
            for (int i = buffIndex - 1; i >= 0; i--)
            {
                if (player.buffType[i] == OriginalBuffType && player.buffTime[i] > 1)
                {
                    player.buffType[i] = player.buffType[buffIndex];
                    player.buffTime[i] = Math.Max(player.buffTime[buffIndex], player.buffTime[i]);
                    player.DelBuff(buffIndex);
                    buffIndex--;
                    break;
                }
                if (player.buffType[i] >= Main.maxBuffTypes && BuffLoader.GetBuff(player.buffType[i]) is EmpoweredBuffBase empoweredBuff)
                {
                    player.buffType[i] = empoweredBuff.OriginalBuffType;
                    break;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, int buffIndex, ref BuffDrawParams drawParams)
        {
            byte a = drawParams.DrawColor.A;
            drawParams.DrawColor *= 1f + Main.cursorAlpha;
            drawParams.DrawColor.A = a;
            return true;
        }
    }
}