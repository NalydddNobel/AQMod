using Aequus.Common.Items.Dedications;

namespace Aequus.Items.Equipment.PetsVanity.Familiar;
public class FamiliarPickaxe : PetItemBase {
    public override int ProjId => ModContent.ProjectileType<FamiliarPet>();
    public override int BuffId => ModContent.BuffType<FamiliarBuff>();

    public override void Load() {
        DedicationRegistry.Register(this, new AnonymousDedication(new Color(200, 65, 70, 255)));
    }

    public override void SetDefaults() {
        base.SetDefaults();
        Item.value = Item.buyPrice(gold: 20);
    }
}