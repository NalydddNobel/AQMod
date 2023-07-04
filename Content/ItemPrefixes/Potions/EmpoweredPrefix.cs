using Aequus.Buffs.Misc.Empowered;
using Aequus.Items;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.ItemPrefixes.Potions {
    public class EmpoweredPrefix : PotionPrefixBase {
        public static Dictionary<int, int> ItemToEmpoweredBuff { get; private set; }

        public override bool Shimmerable => true;
        public override string GlintTexture => $"{this.NamespacePath()}/EmpoweredGlint";

        public override void Load() {
            base.Load();
            ItemToEmpoweredBuff = new Dictionary<int, int>();
        }

        public override void SetStaticDefaults() {
            // DisplayName.SetDefault("{$Mods.Aequus.PrefixName." + Name + "}");
        }

        public override void Unload() {
            ItemToEmpoweredBuff?.Clear();
            ItemToEmpoweredBuff = null;
            base.Unload();
        }

        public override void Apply(Item item) {
            if (ItemToEmpoweredBuff.TryGetValue(item.type, out int buff)) {
                item.buffType = buff;
            }
            item.buffTime /= 2;
        }

        public override void ModifyValue(ref float valueMult) {
            valueMult = 1.1f;
        }

        public override bool CanRoll(Item item) {
            return ItemToEmpoweredBuff.ContainsKey(item.type);
        }

        public override IEnumerable<TooltipLine> GetTooltipLines(Item item) {
            string key;
            if (item.ModItem == null) {
                key = "Mods.Aequus.Items.EmpoweredTooltips." + ItemID.Search.GetName(item.type);
            }
            else {
                key = "Mods." + item.ModItem.Mod.Name + ".Items." + item.ModItem.Name + ".EmpoweredTooltip";
            }
            List<TooltipLine> tooltips = new();
            if (TextHelper.TryGetValue(key, out string text)) {
                tooltips.Add(new(Mod, "EmpoweredTooltip", text) { OverrideColor = Colors.RarityGreen, });
            }

            float statIncrease = 1f;
            if (BuffLoader.GetBuff(item.buffType) is EmpoweredBuffBase empoweredBuff) {
                statIncrease = empoweredBuff.StatIncrease;
            }
            var tt = AequusItem.PercentageModifierLine(statIncrease, "EmpoweredPrefix.Ability");
            tooltips.Add(tt);
            return tooltips;
        }
    }
}