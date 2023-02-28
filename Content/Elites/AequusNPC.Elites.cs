using Aequus.Content.Elites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.NPCs
{
    public partial class AequusNPC : GlobalNPC
    {
        public static int PrefixCount => registeredPrefixes.Count;

        public static HashSet<int> CannotBeElite { get; private set; }
        internal static List<ElitePrefix> registeredPrefixes;

        private bool[] prefix;
        private string _prefixNameCache;
        private bool _anyPrefixes;
        private List<ElitePrefix> _prefixCache;

        public static ElitePrefix FromID(int id)
        {
            return registeredPrefixes[id];
        }
        public static ElitePrefix FromName(string name)
        {
            return registeredPrefixes.Find(x => x.Name == name);
        }
        public bool HasPrefix(int type)
        {
            return prefix[type];
        }

        private void Load_Elites()
        {
            registeredPrefixes = new List<ElitePrefix>();
            CannotBeElite = new HashSet<int>();
        }

        private void Unload_Elites()
        {
            CannotBeElite?.Clear();
            CannotBeElite = null;
            registeredPrefixes?.Clear();
            registeredPrefixes = null;
        }

        public void ResetElitePrefixes()
        {
            prefix = new bool[PrefixCount];
            _prefixCache = new List<ElitePrefix>();
            _prefixNameCache = null;
            _anyPrefixes = false;
        }

        public void SetPrefix(NPC npc, int type, bool value, bool force = false)
        {
            if (!force && CannotBeElite.Contains(type))
                return;

            if (!value && prefix[type])
            {
                _prefixCache.Remove(registeredPrefixes[type]);
            }
            else if (value && !prefix[type])
            {
                _prefixCache.Add(registeredPrefixes[type]);
            }
            prefix[type] = value;
            _anyPrefixes = _prefixCache.Count > 0;
            for (int i = 0; i < PrefixCount; i++)
            {
                if (prefix[i])
                {
                    _anyPrefixes = true;
                    break;
                }
            }
            SetupName();
        }

        private void SetupQuickCache()
        {
            if (_prefixCache == null)
            {
                _prefixCache = new List<ElitePrefix>();
            }
            _prefixCache.Clear();
            for (int i = 0; i < PrefixCount; i++)
            {
                if (prefix[i])
                {
                    _prefixCache.Add(registeredPrefixes[i]);
                }
            }
        }
        private void SetupName()
        {
            _prefixNameCache = null;
            if (!_anyPrefixes)
                return;

            for (int i = 0; i < PrefixCount; i++)
            {
                if (prefix[i])
                {
                    if (string.IsNullOrEmpty(_prefixNameCache))
                    {
                        _prefixNameCache = "";
                    }
                    else
                    {
                        _prefixNameCache += ", ";
                    }
                    _prefixNameCache += registeredPrefixes[i].EliteName;
                }
            }
        }

        public override void ModifyTypeName(NPC npc, ref string typeName)
        {
            if (_anyPrefixes)
            {
                if (string.IsNullOrEmpty(_prefixNameCache))
                {
                    SetupName();
                }
                typeName = _prefixNameCache + " " + typeName;
            }
        }

        private bool PreAI_Elites(NPC npc)
        {
            if (!_anyPrefixes)
                return true;

            bool output = true;
            foreach (var e in _prefixCache)
            {
                output |= e.PreAI(npc);
            }
            return output;
        }
        private void PostAI_Elites(NPC npc)
        {
            if (!_anyPrefixes)
                return;

            foreach (var e in _prefixCache)
            {
                e.PostAI(npc);
            }
        }

        private bool PreDraw_Elites(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!_anyPrefixes)
                return true;

            bool output = true;
            foreach (var e in _prefixCache)
            {
                output |= e.PreDraw(npc, spriteBatch, screenPos, drawColor);
            }
            return output;
        }
        private void PostDraw_Elites(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!_anyPrefixes)
                return;

            foreach (var e in _prefixCache)
            {
                e.PostDraw(npc, spriteBatch, screenPos, drawColor);
            }
        }

        private void SendExtraAI_Elites(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            bitWriter.WriteBit(_anyPrefixes);

            if (!_anyPrefixes)
                return;

            for (int i = 0; i < PrefixCount; i++)
            {
                bitWriter.WriteBit(prefix[i]);
            }
        }

        private void ReceiveExtraAI_Elites(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            if (!bitReader.ReadBit()) // Has no prefixes if this is false
            {
                if (_anyPrefixes)
                {
                    ResetElitePrefixes();
                }
                return;
            }

            for (int i = 0; i < PrefixCount; i++)
            {
                prefix[i] = bitReader.ReadBit();
            }
            SetupQuickCache();
            SetupName();
        }
    }
}