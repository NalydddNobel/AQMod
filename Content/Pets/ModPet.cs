using Terraria.Localization;

namespace Aequus.Content.Pets;

public abstract class ModPet : ModProjectile {
    internal InstancedPetBuff PetBuff { get; set; }
    internal InstancedPetItem PetItem { get; set; }

    protected override bool CloneNewInstances => true;

    internal abstract InstancedPetBuff CreatePetBuff();
    internal virtual InstancedPetItem CreatePetItem() {
        return new(this);
    }

    public override string LocalizationCategory => "Pets";

    public override LocalizedText DisplayName => Language.GetText(this.GetLocalizationKey("DisplayName"));

    public override void Load() {
        PetBuff = CreatePetBuff();
        PetItem = CreatePetItem();
        Mod.AddContent(PetBuff);
        Mod.AddContent(PetItem);
    }

    public override bool PreAI() {
        ref bool active = ref PetBuff._petFlag(Main.player[Projectile.owner]);
        if (Main.player[Projectile.owner].dead) {
            active = false;
        }

        if (active) {
            Projectile.timeLeft = 2;
        }

        return true;
    }
}