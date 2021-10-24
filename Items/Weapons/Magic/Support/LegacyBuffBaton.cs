using System;
using Terraria.ID;

namespace AQMod.Items.Weapons.Magic.Support
{
    [Obsolete()]
    public abstract class LegacyBuffBaton : LegacyBuffStaff
    {
        protected override int BuffTime => 18000;

        protected override int Rarity => ItemRarityID.LightRed;

        protected override int ManaCost => 10;
    }
}