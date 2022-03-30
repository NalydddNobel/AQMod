using Aequus.Common.ID;
using Aequus.Items.Misc;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Aequus.Items
{
    public sealed class ItemSets : ILoadable
    {
        public static Dictionary<int, ItemDedication> Dedicated;

        void ILoadable.Load(Mod mod)
        {
            Dedicated = new Dictionary<int, ItemDedication>()
            {
                //[ModContent.ItemType<NoonPotion>()] = new ItemDedication(new Color(200, 80, 50, 255)),
                [ModContent.ItemType<FamiliarPickaxe>()] = new ItemDedication(new Color(200, 65, 70, 255)),
                //[ModContent.ItemType<MothmanMask>()] = new ItemDedication(new Color(50, 75, 250, 255)),
                //[ModContent.ItemType<RustyKnife>()] = new ItemDedication(new Color(30, 255, 60, 255)),
                //[ModContent.ItemType<Thunderbird>()] = new ItemDedication(new Color(200, 125, 255, 255)),
                //[ModContent.ItemType<Baguette>()] = new ItemDedication(new Color(187, 142, 42, 255)),
                //[ModContent.ItemType<StudiesoftheInkblot>()] = new ItemDedication(new Color(110, 110, 128, 255)),
            };
        }

        void ILoadable.Unload()
        {
        }
    }
}