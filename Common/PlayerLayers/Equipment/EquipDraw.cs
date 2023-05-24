using Aequus.Common.PlayerLayers.Equipment;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Aequus.Common.PlayerLayers.Equipment {
    public abstract class EquipDraw : ILoadable {
        private Dictionary<int, Asset<Texture2D>> _equipDictionary;

        public readonly string equipTextureSuffix;
        protected abstract string EquipTextureSuffix { get; }

        public EquipDraw() {
            equipTextureSuffix = EquipTextureSuffix;
            _equipDictionary = new();
            Type = -1;
        }

        public ModItem EquipItem { get; private set; }
        public int Dye { get; private set; }
        public int Type { get; internal set; }

        public virtual void Clear() {
            EquipItem = null;
            Dye = 0;
        }

        public void Set(ModItem modItem, int dye) {
            EquipItem = modItem;
            Dye = dye;
        }
        public void SetEquip(ModItem modItem, Item dye) {
            Set(modItem, dye.dye);
        }

        /// <returns>Whether or not to draw an equip.</returns>
        public bool CanDraw() {
            return EquipItem != null && EquipItem.Item != null;
        }

        protected virtual Asset<Texture2D> LoadEquipTexture(ModItem modItem) {
            return ModContent.Request<Texture2D>(modItem.Texture + equipTextureSuffix);
        }

        /// <returns>The Equip Texture to use.</returns>
        public Asset<Texture2D> GetEquipTexture() {
            int type = EquipItem.Item.type;
            if (_equipDictionary.TryGetValue(type, out var asset)) {
                return asset;
            }

            var loadedAsset = LoadEquipTexture(EquipItem);
            _equipDictionary.Add(type, loadedAsset);
            return loadedAsset;
        }

        public virtual void Load(Mod mod) {
            EquipDrawLoader.Register(this);
        }

        public virtual void Unload() {
        }

        public abstract void Draw(ref PlayerDrawSet drawInfo, AequusPlayer aequus);
    }
}

namespace Aequus {
    public partial class AequusPlayer {
        private EquipDraw[] _equipDraws;

        private void LoadEquips() {
            _equipDraws = new EquipDraw[EquipDrawLoader.Count];
            for (int i = 0; i < _equipDraws.Length; i++) {
                _equipDraws[i] = (EquipDraw)Activator.CreateInstance(EquipDrawLoader.Get(i).GetType());
            }
        }

        public bool TryGetEquipDrawer<T>(out T value) where T : EquipDraw {
            int type = ModContent.GetInstance<T>().Type;
            if (_equipDraws == null) {
                LoadEquips();
            }
            value = (T)_equipDraws[type];
            return true;
        }

        public T GetEquipDrawer<T>() where T : EquipDraw {
            int type = ModContent.GetInstance<T>().Type;
            if (_equipDraws == null) {
                LoadEquips();
            }

            return (T)_equipDraws[type];
        }

        private void ResetEffects_EquipDraws() {
            if (_equipDraws == null) {
                return;
            }
            for (int i = 0; i < _equipDraws.Length; i++) {
                _equipDraws[i].Clear();
            }
        }

        private void DrawEquip<T>(ref PlayerDrawSet drawInfo) where T : EquipDraw {
            if (TryGetEquipDrawer<T>(out var equipDraw) && equipDraw.CanDraw()) {
                equipDraw.Draw(ref drawInfo, this);
            }
        }
    }
}