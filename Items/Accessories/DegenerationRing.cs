using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories
{
    [AutoloadEquip(EquipType.HandsOn)]
    public class DegenerationRing : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 28;
            item.rare = AQItem.Rarities.GoreNestRare;
            item.value = Item.sellPrice(silver: 80);
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.fireWalk = true;
            player.buffImmune[BuffID.OnFire] = true;
            player.buffImmune[BuffID.Burning] = true;
            player.buffImmune[BuffID.CursedInferno] = true;
            player.buffImmune[BuffID.ShadowFlame] = true;
            player.buffImmune[ModContent.BuffType<Buffs.Debuffs.BlueFire>()] = true;
            player.buffImmune[ModContent.BuffType<Buffs.Debuffs.Sparkling>()] = true;
            player.buffImmune[ModContent.BuffType<Buffs.Debuffs.CorruptionHellfire>()] = true;
            player.buffImmune[ModContent.BuffType<Buffs.Debuffs.CrimsonHellfire>()] = true;
        }
    }
}