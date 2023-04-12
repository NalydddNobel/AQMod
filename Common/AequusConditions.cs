using Aequus.Content.Events.GlimmerEvent;
using Terraria;

namespace Aequus.Common {
    public class AequusConditions {
        public static Condition DownedCrabson => new(TextHelper.GetOrRegister("Condition.Crabson"), () => AequusWorld.downedCrabson);
        public static Condition DownedOmegaStarite => new(TextHelper.GetOrRegister("Condition.OmegaStarite"), () => AequusWorld.downedOmegaStarite);
        public static Condition DownedDustDevil => new(TextHelper.GetOrRegister("Condition.DustDevil"), () => AequusWorld.downedDustDevil);
        private static Condition DownedUpriser => new(TextHelper.GetOrRegister("Condition.Upriser"), () => AequusWorld.downedUpriser);

        public static Condition DownedHyperStarite => new(TextHelper.GetOrRegister("Condition.HyperStarite"), () => AequusWorld.downedHyperStarite);
        public static Condition DownedUltraStarite => new(TextHelper.GetOrRegister("Condition.UltraStarite"), () => AequusWorld.downedUltraStarite);
        public static Condition DownedRedSprite => new(TextHelper.GetOrRegister("Condition.RedSprite"), () => AequusWorld.downedRedSprite);
        public static Condition DownedSpaceSquid => new(TextHelper.GetOrRegister("Condition.SpaceSquid"), () => AequusWorld.downedSpaceSquid);

        public static Condition DownedGlimmer => new(TextHelper.GetOrRegister("Condition.Glimmer"), () => AequusWorld.downedEventCosmic);
        public static Condition DownedDemonSiege => new(TextHelper.GetOrRegister("Condition.DemonSiege"), () => AequusWorld.downedEventDemon);
        public static Condition DownedGaleStreams => new(TextHelper.GetOrRegister("Condition.GaleStreams"), () => AequusWorld.downedEventAtmosphere);

        public static Condition InGlimmer => new(TextHelper.GetOrRegister("Condition.InZoneGlimmer"), () => Main.LocalPlayer.Aequus().ZoneGlimmer);
    }
}