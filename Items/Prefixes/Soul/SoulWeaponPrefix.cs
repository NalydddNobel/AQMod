using Aequus.Items.Weapons.Summon.Necro;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Aequus.Items.Prefixes.Soul
{
    public abstract class SoulWeaponPrefix : ModPrefix
    {
        private static List<SoulWeaponPrefix> Prefixes;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("{$Mods.Aequus.PrefixName." + GetType().Name + "}");
        }

        public override void Load()
        {
            if (Prefixes == null)
            {
                Prefixes = new List<SoulWeaponPrefix>();
            }
            Prefixes.Add(this);
            OnLoad();
        }
        public virtual void OnLoad()
        {
        }

        public override bool CanRoll(Item item)
        {
            return item.ModItem is SoulCandleBase;
        }

        public static ModPrefix ChoosePrefix(Item item, UnifiedRandom rand)
        {
            for (int i = 0; i < 1000; i++)
            {
                var p = Prefixes[rand.Next(Prefixes.Count)];
                if (p.CanChoose(item, rand))
                {
                    return p;
                }
            }
            return ModContent.GetInstance<DiabolicPrefix>();
        }

        public virtual bool CanChoose(Item item, UnifiedRandom rand)
        {
            return true;
        }
    }
}