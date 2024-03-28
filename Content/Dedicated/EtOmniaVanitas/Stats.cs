namespace Aequus.Content.Dedicated.EtOmniaVanitas;

public class Stats {
    public readonly record struct Scale(int Damage, int UseTime, float ShootSpeed, float AmmoConsumptionReduction, IRarityProvider Rarity, int CooldownTime, float ChargeShotDamageIncrease, int ChargeShotDefenseReduction, int ChargeShotDefenseReductionDuration, int FrostburnDebuff = BuffID.Frostburn);

    public interface IRarityProvider {
        int GetRarity();
    }
    public readonly record struct IdRarity(int Rarity) : IRarityProvider {
        public int GetRarity() {
            return Rarity;
        }
    }
    public readonly record struct ModdedRarity(Mod Mod, string Name) : IRarityProvider {
        public int GetRarity() {
            if (Mod != null && Mod.TryFind<ModRarity>(Name, out ModRarity modRarity)) {
                return modRarity.Type;
            }

            return ItemRarityID.Purple;
        }
    }
}
