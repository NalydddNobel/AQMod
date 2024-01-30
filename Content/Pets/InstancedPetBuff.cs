using Aequus.Common.Buffs;
using Terraria.Localization;

namespace Aequus.Content.Pets;

[Autoload(false)]
internal class InstancedPetBuff : InstancedBuff {
    public delegate ref System.Boolean GetPlayerRef(Player player);

    private readonly ModPet _modPet;
    public readonly GetPlayerRef _petFlag;
    private readonly System.Boolean _lightPet;

    public InstancedPetBuff(ModPet modPet, GetPlayerRef petFlag, System.Boolean lightPet) : base(modPet.Name + "Buff", modPet.NamespaceFilePath() + $"/{modPet.Name}Buff") {
        _modPet = modPet;
        _petFlag = petFlag;
        _lightPet = lightPet;
    }

    public override System.String LocalizationCategory => "Pets";

    public override LocalizedText DisplayName => Language.GetOrRegister(_modPet.GetLocalizationKey("BuffDisplayName"));
    public override LocalizedText Description => Language.GetOrRegister(_modPet.GetLocalizationKey("BuffDescription"));

    public override void SetStaticDefaults() {
        Main.buffNoTimeDisplay[Type] = true;
        if (_lightPet) {
            Main.lightPet[Type] = true;
        }
        else {
            Main.vanityPet[Type] = true;
        }
    }

    public override void Update(Player player, ref System.Int32 buffIndex) {
        player.buffTime[buffIndex] = 18000;
        _petFlag(player) = true;
        if (player.ownedProjectileCounts[_modPet.Type] <= 0 && player.whoAmI == Main.myPlayer) {
            Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.position.X + player.width / 2f, player.position.Y + player.height / 2f, 0f, 0f, _modPet.Type, 0, 0f, player.whoAmI, 0f, 0f);
        }
    }
}