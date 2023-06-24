using Aequus.Content.Elites;
using Aequus.Tiles.MossCaves.ElitePlants;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.NPCs {
    public partial class AequusNPC : GlobalNPC
    {
        public static int PrefixCount => Elites.Count;

        public static readonly HashSet<int> CannotBeElite = new();
        internal static readonly List<ElitePrefix> Elites = new();

        private bool[] prefixActiveLookup;
        private string _prefixNameCache;
        private bool _anyPrefixes;
        private List<ElitePrefix> eliteInstances;

        public static int EliteCount => Elites.Count;

        public static ElitePrefix FromID(int id)
        {
            return Elites[id];
        }
        public static ElitePrefix FromName(string name)
        {
            return Elites.Find(x => x.Name == name);
        }

        public bool HasPrefix(int type){
            return prefixActiveLookup[type];
        }
        public bool HasPrefix(ElitePrefix prefix){
            return HasPrefix(prefix.Type);
        }
        public bool HasPrefix<T>() where T : ElitePrefix {
            return HasPrefix(ModContent.GetInstance<T>());
        }

        private void Unload_Elites()
        {
            CannotBeElite.Clear();
            Elites.Clear();
        }

        public void ResetElitePrefixes()
        {
            prefixActiveLookup = new bool[PrefixCount];
            eliteInstances = new List<ElitePrefix>();
            _prefixNameCache = null;
            _anyPrefixes = false;
        }

        public void SetPrefix(NPC npc, int type, bool value, bool force = false)
        {
            if (!force && CannotBeElite.Contains(type))
                return;

            if (!value && prefixActiveLookup[type])
            {
                var prefixType = Elites[type].GetType();
                eliteInstances.RemoveAt(eliteInstances.FindIndex(p => p.GetType().Equals(prefixType)));
            }
            else if (value && !prefixActiveLookup[type])
            {
                eliteInstances.Add(Elites[type].ProperClone(npc));
            }
            prefixActiveLookup[type] = value;
            _anyPrefixes = eliteInstances.Count > 0;
            for (int i = 0; i < PrefixCount; i++)
            {
                if (prefixActiveLookup[i])
                {
                    _anyPrefixes = true;
                    break;
                }
            }
            SetupName();
        }

        private void ResetEliteInstances()
        {
            (eliteInstances ??= new List<ElitePrefix>()).Clear();
        }
        private void SetupName()
        {
            _prefixNameCache = null;
            if (!_anyPrefixes)
                return;

            for (int i = 0; i < PrefixCount; i++)
            {
                if (prefixActiveLookup[i])
                {
                    if (string.IsNullOrEmpty(_prefixNameCache))
                    {
                        _prefixNameCache = "";
                    }
                    else
                    {
                        _prefixNameCache += ", ";
                    }
                    _prefixNameCache += Elites[i].EliteName;
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
            foreach (var e in eliteInstances)
            {
                output |= e.PreAI(npc);
            }
            return output;
        }
        private void PostAI_Elites(NPC npc)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient && syncedTimer % 4 == 0 
                && !npc.boss && !npc.SpawnedFromStatue && !isChildNPC && npc.damage > 0 && npc.lifeMax > 5 && npc.chaseable) {
                EliteBuffPlantsHostile.CheckElitePlants(npc);
            }

            if (!_anyPrefixes)
                return;

            foreach (var e in eliteInstances)
            {
                e.PostAI(npc);
            }
        }

        private bool PreDraw_Elites(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!_anyPrefixes)
                return true;

            bool output = true;
            foreach (var e in eliteInstances)
            {
                output |= e.PreDraw(npc, spriteBatch, screenPos, drawColor);
            }
            return output;
        }
        private void PostDraw_Elites(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!_anyPrefixes)
                return;

            foreach (var e in eliteInstances)
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
                bitWriter.WriteBit(prefixActiveLookup[i]);
                eliteInstances.Find(p => p.Type == i)?.SendExtraAI(npc, bitWriter, binaryWriter);
            }
        }

        private void ReceiveExtraAI_Elites(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            if (!bitReader.ReadBit()) // Has no prefixes if this returns false
            {
                if (_anyPrefixes)
                {
                    ResetElitePrefixes();
                }
                return;
            }

            _anyPrefixes = true;
            ResetEliteInstances();
            for (int i = 0; i < PrefixCount; i++) {
                prefixActiveLookup[i] = bitReader.ReadBit();
                if (prefixActiveLookup[i]) {
                    var prefix = Elites[i].ProperClone(npc);
                    prefix.RecieveExtraAI(npc, bitReader, binaryReader);
                    eliteInstances.Add(prefix);
                }
            }
            SetupName();
        }
    }
}