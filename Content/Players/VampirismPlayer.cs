using Terraria.ModLoader;

namespace AQMod.Content.Players
{
    public sealed class VampirismPlayer : ModPlayer
    {
        public ushort Vampirism;

        public bool HasVampirism => Vampirism != 0;
        public bool IsVampire => Vampirism == ushort.MaxValue;

        public override void ResetEffects()
        {
            if (!IsVampire && Vampirism > 0)
            {
                Vampirism--;
                if (Vampirism == 0)
                {
                    Vampirism = ushort.MaxValue;
                    player.ClearBuff(ModContent.BuffType<Buffs.Timers.Vampirism>());
                }
                else
                {
                    GiveVampirism(1);
                }
            }
        }

        public void GiveVampirism(int time)
        {
            if (IsVampire || HasVampirism)
            {
                return;
            }
            Vampirism = (ushort)time;
            player.AddBuff(ModContent.BuffType<Buffs.Timers.Vampirism>(), time);
        }
    }
}