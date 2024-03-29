﻿using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;

namespace Aequus.Common.Utilities {
    public struct SelectableChatHelper
    {
        private readonly string AddKey;
        private readonly List<(string, Func<object>)> Text;

        public SelectableChatHelper(string add = "Mods.Aequus.")
        {
            AddKey = add;
            Text = new List<(string, Func<object>)>();
        }

        public SelectableChatHelper Add(string key, Func<object> getObj)
        {
            Text.Add((key, getObj));
            return this;
        }

        public SelectableChatHelper Add(string key, object obj)
        {
            return Add(key, () => obj);
        }

        public SelectableChatHelper Add(string key)
        {
            return Add(key, null);
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
                Language.GetTextValueWith(key, tuple.Item2.Invoke()) : Language.GetTextValue(key);
        }
    }
}