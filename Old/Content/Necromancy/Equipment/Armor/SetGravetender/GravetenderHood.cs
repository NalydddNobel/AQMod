using Aequus.Content.DataSets;
using System.Linq;

namespace Aequus.Old.Content.Necromancy.Equipment.Armor.SetGravetender;

[LegacyName("NecromancerHood", "SeraphimHood")]
[AutoloadEquip(EquipType.Head)]
public class GravetenderHood : ModItem {
    public override void SetDefaults() {
        Item.defense = 2;
        Item.width = 20;
        Item.height = 20;
        Item.rare = ItemRarityID.Blue;
        Item.shoot = ModContent.ProjectileType<GravetenderWisp>();
        Item.buffType = ModContent.BuffType<GravetenderMinionBuff>();
        Item.value = Item.sellPrice(silver: 20);
    }

    public override bool IsArmorSet(Item head, Item body, Item legs) {
        return body.type == ModContent.ItemType<GravetenderRobes>();
    }

    public override void UpdateArmorSet(Player player) {
        player.setBonus = this.GetLocalizedValue("Setbonus");
        var aequus = player.GetModPlayer<AequusPlayer>();

        int closestNPC = -1;
        float closestDistance = float.MaxValue;
        for (int i = 0; i < Main.maxNPCs; i++) {
            if (Main.npc[i].active && Main.npc[i].TryGetGlobalNPC(out NecromancyNPC necromancyNPC) && necromancyNPC.isZombie && necromancyNPC.zombieOwner == player.whoAmI) {
                float distance = player.Distance(Main.npc[i].Center);
                if (distance < closestDistance) {
                    closestNPC = i;
                    closestDistance = distance;
                }
            }
        }

        aequus.gravetenderGhost = closestNPC;
    }

    public override void UpdateEquip(Player player) {
        player.GetDamage<SummonDamageClass>() += 0.1f;
    }

    public override void AddRecipes() {
        foreach (int rottenChunk in ItemTypeVariants.RottenChunk.Where(i => i.ValidEntry)) {
            CreateRecipe()
                .AddIngredient(ItemID.Cobweb, 50)
                .AddIngredient(rottenChunk, 5)
                .AddTile(TileID.Loom)
                .AddCondition(Condition.InGraveyard)
                .Register()
                .SortBeforeFirstRecipesOf(ItemID.GravediggerShovel);
        }
    }
}