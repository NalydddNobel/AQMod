using Terraria.ID;

namespace AQMod.Items.BuffItems.Staffs
{
    public abstract class BuffBaton : BuffStaff
    {
        protected override int BuffTime => 18000;

        protected override int Rarity => ItemRarityID.LightRed;

        protected override int ManaCost => 10;
    }
}