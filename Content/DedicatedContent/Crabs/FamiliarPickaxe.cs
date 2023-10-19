using Aequus.Common.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.DedicatedContent.Crabs;

public class FamiliarPickaxe : PetItemBase, IDedicatedItem {
    public override int ProjId => ModContent.ProjectileType<FamiliarPet>();
    public override int BuffId => ModContent.BuffType<FamiliarBuff>();

    public string DisplayedDedicateeName => null;
    public Color TextColor => new Color(200, 65, 70);

    public override void SetDefaults() {
        base.SetDefaults();
        Item.value = Item.buyPrice(gold: 20);
    }
}