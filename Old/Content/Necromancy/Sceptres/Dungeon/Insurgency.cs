using Aequus.Common.Items;
using Aequus.Core.Initialization;

namespace Aequus.Old.Content.Necromancy.Sceptres.Dungeon;

[AutoloadGlowMask]
public class Insurgency : ScepterBase {
    public override void SetDefaults() {
        Item.DefaultToNecromancy(50);
        Item.SetWeaponValues(125, 0.8f, 0);
        Item.shoot = ModContent.ProjectileType<InsurgencyProj>();
        Item.shootSpeed = 30f;
        Item.rare = ItemCommons.Rarity.HardDungeonLoot;
        Item.value = ItemCommons.Price.HardDungeonLoot;
        Item.mana = 20;
        Item.UseSound = SoundID.Item8;
        Item.autoReuse = true;
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient<Revenant>()
            .AddIngredient(ItemID.SpectreBar, 12)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }
}