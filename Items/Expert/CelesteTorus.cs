using AQMod.Content.Players;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Expert
{
    public class CelesteTorus : ModItem, IUpdateVanity
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.rare = ItemRarityID.Pink;
            item.value = Item.sellPrice(gold: 8);
            item.expert = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            try
            {
                int line = AQItem.FindTTLineSpot(tooltips, "Damage");
                var player = Main.LocalPlayer;
                var aQPlayer = player.GetModPlayer<AQPlayer>();
                tooltips.Insert(line, new TooltipLine(mod, "Knockback", AQItem.KBTooltip(6.5f)));
                tooltips.Insert(line, new TooltipLine(mod, "Damage", aQPlayer.GetCelesteTorusDamage() + (string)AQItem.DamageTip()));
            }
            catch
            {
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            aQPlayer.blueSpheres = true;
        }

        void IUpdateVanity.UpdateVanitySlot(Player player, AQPlayer aQPlayer, PlayerDrawEffects drawEffects, int i)
        {
            drawEffects.cCelesteTorus = player.dye[i % AQPlayer.MaxDye].dye;
        }
    }
}