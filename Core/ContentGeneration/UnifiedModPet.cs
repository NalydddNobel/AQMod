using Terraria.Localization;

namespace AequusRemake.Core.ContentGeneration;

/// <summary>A <see cref="ModProjectile"/> which automagically registers a related <see cref="ModItem"/> and <see cref="ModBuff"/>, suffixed with +"Item" and +"Buff" respectively.</summary>
public abstract class UnifiedModPet : ModProjectile {
    internal InstancedPetBuff PetBuff { get; set; }
    internal InstancedPetItem PetItem { get; set; }

    protected override bool CloneNewInstances => true;

    internal abstract InstancedPetBuff CreatePetBuff();
    internal virtual InstancedPetItem CreatePetItem() {
        return new(this);
    }

    public override string LocalizationCategory => "Pets";

    public override LocalizedText DisplayName => Language.GetText(this.GetLocalizationKey("DisplayName"));

    public sealed override void Load() {
        PetBuff = CreatePetBuff();
        PetItem = CreatePetItem();
        OnLoad();
        Mod.AddContent(PetBuff);
        Mod.AddContent(PetItem);
    }

    protected virtual void OnLoad() {

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