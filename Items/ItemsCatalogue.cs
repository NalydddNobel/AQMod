using Aequus.Common;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items
{
    public sealed class ItemsCatalogue : LoadableType, IAddRecipes
    {
        public static HashSet<int> SummonStaff { get; private set; }

        public override void Load()
        {
            SummonStaff = new HashSet<int>();
        }

        void IAddRecipes.AddRecipes(Aequus aequus)
        {
            InnerLoadAutomaticEntries();
        }

        public static void InnerLoadAutomaticEntries()
        {
            for (int i = 0; i < ItemLoader.ItemCount; i++)
            {
                var item = ContentSamples.ItemsByType[i];
                if (IsSummonStaff(item))
                {
                    SummonStaff.Add(i);
                }
            }
        }
        public static bool IsSummonStaff(Item item)
        {
            return item.damage > 0 && item.DamageType == DamageClass.Summon && item.shoot > ProjectileID.None && item.useStyle > 0 && (ContentSamples.ProjectilesByType[item.shoot].minionSlots > 0f || ContentSamples.ProjectilesByType[item.shoot].sentry);
        }
    }
}