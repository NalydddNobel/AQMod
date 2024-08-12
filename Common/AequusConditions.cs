using Aequus.Common.Carpentry;
using Aequus.Content.Biomes.CrabCrevice;
using Aequus.Content.Biomes.Oblivion;
using Aequus.Content.Events.DemonSiege;
using Aequus.Content.Events.GlimmerEvent;
using Aequus.Content.Events.GlimmerEvent.Peaceful;
using System;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace Aequus.Common {
    public class AequusConditions {
        public static Condition DownedCrabson => new(TextHelper.GetOrRegister("Condition.DownedCrabson"), () => AequusWorld.downedCrabson);
        public static Condition DownedOmegaStarite => new(TextHelper.GetOrRegister("Condition.DownedOmegaStarite"), () => AequusWorld.downedOmegaStarite);
        public static Condition DownedDustDevil => new(TextHelper.GetOrRegister("Condition.DownedDustDevil"), () => AequusWorld.downedDustDevil);
        private static Condition DownedUpriser => new(TextHelper.GetOrRegister("Condition.DownedUpriser"), () => AequusWorld.downedUpriser);
        private static Condition DownedYinYang => new(TextHelper.GetOrRegister("Condition.DownedYinYang"), () => AequusWorld.downedYinYang);

        public static Condition DownedHyperStarite => new(TextHelper.GetOrRegister("Condition.DownedHyperStarite"), () => AequusWorld.downedHyperStarite);
        public static Condition DownedUltraStarite => new(TextHelper.GetOrRegister("Condition.DownedUltraStarite"), () => AequusWorld.downedUltraStarite);
        public static Condition DownedRedSprite => new(TextHelper.GetOrRegister("Condition.DownedRedSprite"), () => AequusWorld.downedRedSprite);
        public static Condition DownedSpaceSquid => new(TextHelper.GetOrRegister("Condition.DownedSpaceSquid"), () => AequusWorld.downedSpaceSquid);

        public static Condition DownedGlimmer => new(TextHelper.GetOrRegister("Condition.DownedGlimmer"), () => AequusWorld.downedEventCosmic);
        public static Condition DownedDemonSiege => new(TextHelper.GetOrRegister("Condition.DownedDemonSiege"), () => AequusWorld.downedEventDemon);
        public static Condition DownedGaleStreams => new(TextHelper.GetOrRegister("Condition.DownedGaleStreams"), () => AequusWorld.downedEventAtmosphere);

        public static Condition InCrabCrevice => new(TextHelper.GetOrRegister("Condition.InCrabCrevice"), Main.LocalPlayer.InModBiome<CrabCreviceBiome>);
        public static Condition InGlimmer => new(TextHelper.GetOrRegister("Condition.InGlimmer"), Main.LocalPlayer.InModBiome<GlimmerZone>);
        public static Condition InDemonSiege => new(TextHelper.GetOrRegister("Condition.InDemonSiege"), Main.LocalPlayer.InModBiome<DemonSiegeZone>);
        public static Condition NearGoreNest => new(TextHelper.GetOrRegister("Condition.InGoreNest"), Main.LocalPlayer.InModBiome<OblivionAltarBiome>);
        public static Condition InPeacefulGlimmer => new(TextHelper.GetOrRegister("Condition.InPeacefulGlimmer"), Main.LocalPlayer.InModBiome<PeacefulGlimmerZone>);

        public static Condition HasCompletedBuildChallenge(BuildChallenge challenge) {
            return new(TextHelper.GetOrRegister("Condition.HasCompletedBuildChallenge").WithFormatArgs(challenge.GetDisplayName()), () => CarpentrySystem.CompletedBounties.ContainsChallenge(challenge) && Main.LocalPlayer.GetModPlayer<CarpentryPlayer>().CollectedBounties.ContainsChallenge(challenge));
        }

        public static Condition HasNotCompletedBuildChallenge(BuildChallenge challenge) {
            return new(TextHelper.GetOrRegister("Condition.HasCompletedBuildChallenge").WithFormatArgs(challenge.GetDisplayName()), () => !CarpentrySystem.CompletedBounties.ContainsChallenge(challenge));
        }
    }
}