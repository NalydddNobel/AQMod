using Aequus.Biomes.DemonSiege;
using Aequus.Buffs;
using Aequus.Items.Accessories.Summon.Necro;
using Aequus.Items.GlobalItems;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.BuffPotions
{
    public class DeathsEmbrace : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.DrinkParticleColors[Type] = new Color[] { Color.Black * 0.66f, Color.DarkRed.UseA(180) * 0.66f, };
            DemonSiegeSystem.RegisterSacrifice(new SacrificeData(ModContent.ItemType<SpiritBottle>(), Type, UpgradeProgressionType.PreHardmode) { Hide = true, });
            TooltipsGlobal.Dedicated[Type] = new TooltipsGlobal.ItemDedication(new Color(20, 70, 30, 255));
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.HeartreachPotion);
            Item.consumable = false;
            Item.buffTime = 300;
            Item.rare = ItemRarityID.LightPurple;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(gold: 1);
            Item.buffType = ModContent.BuffType<DeathsEmbraceBuff>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            foreach (var t in tooltips)
            {
                if (t.Mod == "Terraria" && t.Name.StartsWith("Tooltip"))
                {
                    t.Text = t.Text.Replace("{{", $"[c/{Colors.AlphaDarken(Color.Lerp(Color.Red, Color.White, 0.5f)).Hex3()}:").Replace("}}", $"]");
                }
            }
        }
    }
}