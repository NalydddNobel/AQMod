using Aequus.Common.ItemPrefixes;
using Terraria.ModLoader;

namespace Aequus.Content.ItemPrefixes;
public sealed class CooldownPrefixInitializer : ILoadable {
    public void Load(Mod mod) {
        // Good Prefixes
        mod.AddContent(new CooldownPrefix("Constant", priceMultiplier: 1.66f, cooldownMultiplier: 0.80f, new() {
            UseTimeMultiplier = 0.75f,
            ManaMultiplier = 0.75f,
        }));

        mod.AddContent(new CooldownPrefix("Punctual", priceMultiplier: 1.5f, cooldownMultiplier: 0.90f, new() {
            DamageMultiplier = 1.05f,
            ManaMultiplier = 0.9f,
            KnockbackMultiplier = 1.25f,
        }));

        mod.AddContent(new CooldownPrefix("Steady", priceMultiplier: 1.5f, cooldownMultiplier: 0.90f, new() {
            DamageMultiplier = 1.1f,
            UseTimeMultiplier = 0.9f,
        }));

        mod.AddContent(new CooldownPrefix("Organized", priceMultiplier: 1.25f, cooldownMultiplier: 0.95f, new() {
            DamageMultiplier = 1.05f,
            KnockbackMultiplier = 0.9f,
            UseTimeMultiplier = 0.9f,
        }));

        mod.AddContent(new CooldownPrefix("Dependant", priceMultiplier: 1.15f, cooldownMultiplier: 0.95f, new() {
            UseTimeMultiplier = 0.9f,
        }));


        // Bad Prefixes
        mod.AddContent(new CooldownPrefix("Tardy", priceMultiplier: 0.7f, cooldownMultiplier: 1.2f, new() {
            UseTimeMultiplier = 1.2f,
        }));

        mod.AddContent(new CooldownPrefix("Delayed", priceMultiplier: 0.7f, cooldownMultiplier: 1.12f, new() {
            DamageMultiplier = 0.95f,
            UseTimeMultiplier = 1.4f,
        }));

        mod.AddContent(new CooldownPrefix("Late", priceMultiplier: 0.7f, cooldownMultiplier: 1.10f, new() {
            DamageMultiplier = 0.95f,
            UseTimeMultiplier = 1.2f,
        }));

        mod.AddContent(new CooldownPrefix("Overscheduled", priceMultiplier: 0.88f, cooldownMultiplier: 1.10f, new() {
            UseTimeMultiplier = 1.08f,
        }));
    }

    public void Unload() {
    }
}