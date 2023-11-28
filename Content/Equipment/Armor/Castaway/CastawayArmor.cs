using Aequus.Common.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Equipment.Armor.Castaway;

public class CastawayArmor : ModArmor {
    public static int DefenseRegenerationRate { get; set; } = 24;

    /// <summary>
    /// Determines how many spiky balls can be in the world at once before they're forced to despawn.
    /// </summary>
    public static int MaxBallsOut { get; set; } = 10;
    /// <summary>
    /// Determines how many spiky balls can be released at once when damaged.
    /// </summary>
    public static int MaxBallsOnHit { get; set; } = 3;

    public static float KnockbackResistanceBody { get; set; } = 0.25f;
    public static float KnockbackResistanceLegs { get; set; } = 0.25f;

    public override void Load() {
        var keyword = new Keyword(this.GetLocalization("DefenseDamageKeyword"), this.GetLocalization("DefenseDamageKeywordTip"), ItemID.AdhesiveBandage, new Color(187, 255, 170));
        AddArmor(new InstancedHelmet(
                this,
                new ArmorStats(Defense: 8, Rarity: ItemRarityID.White, Value: Item.sellPrice(silver: 22, copper: 50)),
                keyword
            ), out var helmet);
        AddArmor(new InstancedBody(
                this,
                new ArmorStats(Defense: 10, Rarity: ItemRarityID.White, Value: Item.sellPrice(silver: 37, copper: 50)),
                keyword,
                tooltipArguments: TextHelper.Percent(KnockbackResistanceBody)
            ), out var body);
        AddArmor(new InstancedLegs(
                this,
                new ArmorStats(Defense: 6, Rarity: ItemRarityID.White, Value: Item.sellPrice(silver: 30)),
                keyword,
                tooltipArguments: TextHelper.Percent(KnockbackResistanceLegs)
            ), out var legs);

        helmet
            .HookUpdateEquip((item, player) => {
                player.GetModPlayer<CastawayPlayer>().brokenDefenseMax += item.defense;
            })
            .HookIsArmorSet((head2, body2, legs2) => body2.type == body.Type && legs.Type == legs.Type)
            .HookUpdateArmorSet((item, player) => {
                player.setBonus = this.GetLocalizedValue("Setbonus");
                player.GetModPlayer<CastawayPlayer>().setbonus = true;
            });

        body.HookUpdateEquip((item, player) => {
            player.GetModPlayer<CastawayPlayer>().brokenDefenseMax += item.defense;
            player.GetModPlayer<CastawayPlayer>().kbResist -= KnockbackResistanceBody;
        });

        legs.HookUpdateEquip((item, player) => {
            player.GetModPlayer<CastawayPlayer>().brokenDefenseMax += item.defense;
            player.GetModPlayer<CastawayPlayer>().kbResist -= KnockbackResistanceLegs;
        });
    }
}