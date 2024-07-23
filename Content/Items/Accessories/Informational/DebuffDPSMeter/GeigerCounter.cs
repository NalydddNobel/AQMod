using Aequus.Common.Items;
using Aequus.Common.Items.EquipmentBooster;
using Aequus.Items.Materials;
using Aequus.Tiles.MossCaves.Radon;
using Terraria.Localization;

namespace Aequus.Content.Items.Accessories.Informational.DebuffDPSMeter;

[Gen.AequusPlayer_InfoField("accInfoDebuffDPS")]
public class GeigerCounter : ModItem {
    public override void SetStaticDefaults() {
        ItemID.Sets.WorksInVoidBag[Type] = true;
        EquipBoostDatabase.Instance.SetNoEffect(Type);
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.LifeformAnalyzer);
    }

    public override void UpdateInfoAccessory(Player player) {
        player.GetModPlayer<AequusPlayer>().accInfoDebuffDPS = true;
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<RadonMoss>(), 25)
            .AddIngredient(ItemID.SilverBar, 10)
            .AddIngredient(ModContent.ItemType<FrozenTechnology>())
            .AddTile(TileID.Tables)
            .AddTile(TileID.Chairs)
            .Register()

            .Clone()
            .ReplaceItem(ItemID.SilverBar, ItemID.TungstenBar)
            .Register();
    }
}

public class GeigerCounterInfoDisplay : InfoDisplay {
    public override string DisplayValue(ref Color displayColor, ref Color displayShadowColor) {
        int dps = GetDPS(Main.myPlayer);

        if (dps > 0) {
            return Language.GetTextValue("GameUI.DPS", dps);
        }

        displayColor = InactiveInfoTextColor;
        return Language.GetTextValue("GameUI.NoDPS");
    }

    private static int GetDPS(int plr) {
        double dps = 0.0;
        Vector2 playerPosition = Main.player[plr].Center;

        for (int i = 0; i < Main.maxNPCs; i++) {
            NPC npc = Main.npc[i];
            if (npc.active && npc.lifeRegen < 0 && npc.playerInteraction[plr]) {
                dps -= npc.lifeRegen / 2.0;
            }
        }

        return (int)dps;
    }

    public override bool Active() {
        return Main.LocalPlayer.GetModPlayer<AequusPlayer>().accInfoDebuffDPS;
    }
}
