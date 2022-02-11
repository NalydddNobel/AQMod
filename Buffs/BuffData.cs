using Terraria;

namespace AQMod.Buffs
{
    public struct BuffData
    {
        public readonly int BuffType;
        public readonly int BuffDuration;

        public BuffData(int buffType, int buffDuration)
        {
            BuffType = buffType;
            BuffDuration = buffDuration;
        }

        public void ApplyDebuff(NPC npc)
        {
            npc.AddBuff(BuffType, BuffDuration, quiet: false);
        }
    }
}