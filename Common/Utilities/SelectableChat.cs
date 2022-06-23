using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;

namespace Aequus.Common.Utilities
{
    internal struct SelectableChat
    {
        private readonly string AddKey;
        private readonly List<(string, Func<object>)> Text;

        public SelectableChat(string add = "Mods.Aequus.")
        {
            AddKey = add;
            Text = new List<(string, Func<object>)>();
        }

        public SelectableChat Add(string key, Func<object> getObj)
        {
            Text.Add((key, getObj));
            return this;
        }

        public SelectableChat Add(string key)
        {
            return Add(key, null);
        }

        public SelectableChat Add(string key, object obj)
        {
            return Add(key, () => obj);
        }

        public string Get()
        {
            if (Text.Count == 0)
            {
                return "No Text";
            }
            var tuple = Text[Main.rand.Next(Text.Count)];
            string key = AddKey + tuple.Item1;
            return tuple.Item2 != null ?
                Language.GetTextValueWith(key, tuple.Item2) : Language.GetTextValue(key);
        }
    }
}