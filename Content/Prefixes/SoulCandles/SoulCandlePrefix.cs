using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Aequus.Content.Prefixes.SoulCandles
{
    public abstract class SoulCandlePrefix : ModPrefix
    {
        private static List<SoulCandlePrefix> Prefixes;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("{$Mods.Aequus.PrefixName." + GetType().Name + "}");
        }

        public sealed override void Load()
        {
            if (Prefixes == null)
            {
                Prefixes = new List<SoulCandlePrefix>();
            }
            Prefixes.Add(this);
            OnLoad();
        }
        public virtual void OnLoad()
        {
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