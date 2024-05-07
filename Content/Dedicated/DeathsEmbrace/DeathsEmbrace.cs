using Aequus.Common.Items.Dedications;
using System.Collections.Generic;

namespace Aequus.Content.Dedicated.DeathsEmbrace;

public class DeathsEmbrace : ModItem {
    public override void Load() {
        DedicationRegistry.Register(this, new DefaultDedication("bubbyboytoo", new Color(20, 70, 30)));
    }

    public override void SetStaticDefaults() {
        ItemSets.DrinkParticleColors[Type] = new Color[] { Color.Black * 0.66f, Color.DarkRed with { A = 180 } * 0.66f, };
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.HeartreachPotion);
        Item.consumable = false;
        Item.buffTime = 300;
        Item.rare = ItemRarityID.LightPurple;
        Item.maxStack = 1;
        Item.value = Item.sellPrice(gold: 1);
        Item.buffType = ModContent.BuffType<DeathsEmbraceBuff>();
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips) {
        foreach (var t in tooltips) {
            if (t.Mod == "Terraria" && t.Name.StartsWith("Tooltip")) {
                t.Text = t.Text.Replace("{{", $"[c/{Colors.AlphaDarken(Color.Lerp(Color.Red, Color.White, 0.5f)).Hex3()}:").Replace("}}", $"]");
            }
        }
    }
}
