using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader.IO;

namespace Aequus.Content.ItemModules
{
    public class ItemModuleData : TagSerializable, IEnumerable<KeyValuePair<int, Item>>, IEnumerable
    {
        public readonly Dictionary<int, Item> Modules;

        public ItemModuleData()
        {
            Modules = new Dictionary<int, Item>();
        }

        public Item this[int key] { get => Modules[key]; set => Modules[key] = value; }
        public int Count => Modules.Count;

        public TagCompound SerializeData()
        {
            List<int> keys = new List<int>();
            List<Item> values = new List<Item>();
            foreach (var pair in Modules)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
            return new TagCompound()
            {
                ["keys"] = keys.ToList(),
                ["values"] = values.ToList(),
            };
        }

        public static ItemModuleData LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("keys") && tag.ContainsKey("values"))
            {
                var data = new ItemModuleData();
                List<int> keys = tag.Get<List<int>>("keys");
                List<Item> values = tag.Get<List<Item>>("values");
                for (int i = 0; i < keys.Count; i++)
                {
                    data.Modules.Add(keys[i], values[i]);
                }
                return data;
            }
            return null;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Modules.GetEnumerator();
        }

        IEnumerator<KeyValuePair<int, Item>> IEnumerable<KeyValuePair<int, Item>>.GetEnumerator()
        {
            return Modules.GetEnumerator();
        }
    }
}