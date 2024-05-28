using Aequus.Common;
using Terraria.DataStructures;
using tModLoaderExtended.GlowMasks;

namespace Aequus.Old.Content.Items.Weapons.Summon.Minions.ScribbleMinion;

[AutoloadGlowMask]
public class ScribbleNotebook : ModItem {
    public override void SetStaticDefaults() {
        ItemSets.GamepadWholeScreenUseRange[Item.type] = true;
    }

    public override void SetDefaults() {
        Item.damage = 20;
        Item.knockBack = 5f;
        Item.mana = 10;
        Item.width = 32;
        Item.height = 32;
        Item.useTime = 36;
        Item.useAnimation = 36;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.value = Commons.Cost.BossOmegaStarite;
        Item.rare = Commons.Rare.BossOmegaStarite;
        Item.UseSound = SoundID.Item44;
        Item.noMelee = true;
        Item.DamageType = DamageClass.Summon;
        ModContent.GetInstance<ScribbleNotebookMinion>().SetItemStats(Item);
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
        player.AddBuff(Item.buffType, 2);
        player.SpawnMinionOnCursor(source, player.whoAmI, type, Item.damage, knockback);
        return false;
    }
}