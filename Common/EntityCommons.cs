using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common {
    public readonly struct EntityCommons {
        public readonly Entity Entity;

        public readonly int Life;
        public readonly int MaxLife;
        public readonly int[] BuffType;
        public readonly int[] BuffTime;
        public readonly int MaxBuffs;

        public EntityCommons(Entity entity) {
            Entity = entity;
            Life = 0;
            MaxLife = 0;
            BuffTime = BuffType = null;
            MaxBuffs = 0;
            if (entity is NPC npc) {
                Life = npc.life;
                MaxLife = npc.lifeMax;
                BuffType = npc.buffType;
                BuffTime = npc.buffTime;
                MaxBuffs = NPC.maxBuffs;
            }
            else if (entity is Player player) {
                Life = player.statLife;
                MaxLife = player.statLifeMax2;
                BuffType = player.buffType;
                BuffTime = player.buffTime;
                MaxBuffs = Player.MaxBuffs;
            }
        }

        public bool HasBuff<T>() where T : ModBuff {
            return HasBuff(ModContent.BuffType<T>());
        }
        public bool HasBuff(int type) {
            if (Entity is NPC npc) {
                return npc.HasBuff(type);
            }
            if (Entity is Player player) {
                return player.HasBuff(type);
            }
            return false;
        }

        public void AddBuff<T>(int duration, bool quiet = false) where T : ModBuff {
            AddBuff(ModContent.BuffType<T>(), duration, quiet);
        }
        public void AddBuff(int type, int duration, bool quiet = false) {
            if (Entity is NPC npc) {
                npc.AddBuff(type, duration, quiet);
            }
            else if (Entity is Player player) {
                player.AddBuff(type, duration, quiet);
            }
        }

        public bool ImmuneToBuff(int type) {
            if (Entity is NPC npc) {
                return npc.buffImmune[type];
            }
            if (Entity is Player player) {
                return player.buffImmune[type];
            }
            return true;
        }
    }
}