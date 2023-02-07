using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common
{
    public struct EntityCommons
    {
        public readonly Entity ent;

        public readonly int life;
        public readonly int maxLife;
        public readonly int[] buffType;
        public readonly int[] buffTime;
        public readonly int maxBuffs;

        public EntityCommons(Entity ent)
        {
            this.ent = ent;
            life = 0;
            maxLife = 0;
            buffTime = buffType = null;
            maxBuffs = 0;
            if (ent is NPC npc)
            {
                life = npc.life;
                maxLife = npc.lifeMax;
                buffType = npc.buffType;
                buffTime = npc.buffTime;
                maxBuffs = NPC.maxBuffs;
            }
            else if (ent is Player player)
            {
                life = player.statLife;
                maxLife = player.statLifeMax2;
                buffType = player.buffType;
                buffTime = player.buffTime;
                maxBuffs = Player.MaxBuffs;
            }
        }

        public bool HasBuff<T>() where T : ModBuff
        {
            return HasBuff(ModContent.BuffType<T>());
        }
        public bool HasBuff(int type)
        {
            if (ent is NPC npc)
            {
                return npc.HasBuff(type);
            }
            if (ent is Player player)
            {
                return player.HasBuff(type);
            }
            return false;
        }

        public void AddBuff<T>(int duration, bool quiet = false) where T : ModBuff
        {
            AddBuff(ModContent.BuffType<T>(), duration, quiet);
        }
        public void AddBuff(int type, int duration, bool quiet = false)
        {
            if (ent is NPC npc)
            {
                npc.AddBuff(type, duration, quiet);
            }
            else if (ent is Player player)
            {
                player.AddBuff(type, duration, quiet);
            }
        }

        public bool ImmuneToBuff(int type)
        {
            if (ent is NPC npc)
            {
                return npc.buffImmune[type];
            }
            if (ent is Player player)
            {
                return player.buffImmune[type];
            }
            return true;
        }
    }
}