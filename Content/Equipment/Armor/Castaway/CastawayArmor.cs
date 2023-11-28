using Aequus.Common.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Equipment.Armor.Castaway;

public class CastawayArmor : ModArmor {
    public static int DefenseRegenerationRate { get; set; } = 24;

    public override void Load() {
        var keyword = new Keyword(this.GetLocalization("DefenseDamageKeyword"), this.GetLocalization("DefenseDamageKeywordTip"), ItemID.AdhesiveBandage, new Color(187, 255, 170));
        AddArmor(new InstancedHelmet(this, new ArmorStats(Defense: 8, Rarity: ItemRarityID.White, Value: Item.sellPrice(silver: 22, copper: 50)), keyword), out var helmet);
        AddArmor(new InstancedBody(this, new ArmorStats(Defense: 10, Rarity: ItemRarityID.White, Value: Item.sellPrice(silver: 37, copper: 50)), keyword), out var body);
        AddArmor(new InstancedLegs(this, new ArmorStats(Defense: 6, Rarity: ItemRarityID.White, Value: Item.sellPrice(silver: 30)), keyword), out var legs);

        helmet.HookUpdateEquip((item, player) => {
            player.GetModPlayer<CastawayPlayer>().brokenDefenseMax += item.defense;
        });
        body.HookUpdateEquip((item, player) => {
            player.GetModPlayer<CastawayPlayer>().brokenDefenseMax += item.defense;
        });
        legs.HookUpdateEquip((item, player) => {
            player.GetModPlayer<CastawayPlayer>().brokenDefenseMax += item.defense;
        });
    }
}