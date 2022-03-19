using AQMod.Content.Players;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories.Summon
{
    public class CelesteTorus : ModItem, IUpdateVanity
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.damage = 30;
            item.knockBack = 2f;
            item.summon = true;
            item.accessory = true;
            item.rare = ItemRarityID.Pink;
            item.value = Item.sellPrice(gold: 4);
            item.expert = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            //try
            //{
            //    int line = AQItem.LegacyGetLineIndex(tooltips, "Damage");
            //    var player = Main.LocalPlayer;
            //    var aQPlayer = player.GetModPlayer<AQPlayer>();
            //    tooltips.Insert(line, new TooltipLine(mod, "Knockback", AQItem.KBTooltip(6.5f)));
            //    tooltips.Insert(line, new TooltipLine(mod, "Damage", aQPlayer.GetCelesteTorusDamage() + (string)AQItem.DamageTip()));
            //}
            //catch
            //{
            //}
        }

        public override void ModifyWeaponDamage(Player player, ref float add, ref float mult, ref float flat)
        {
            mult += (int)(player.statDefense / 1.5f + player.endurance * 80f) / 30f;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<AQPlayer>().accOmegaStarite = item;
        }

        void IUpdateVanity.UpdateVanitySlot(Player player, AQPlayer aQPlayer, PlayerDrawEffects drawEffects, int i)
        {
            drawEffects.cCelesteTorus = player.dye[i % AQPlayer.MaxDye].dye;
        }
    }
}