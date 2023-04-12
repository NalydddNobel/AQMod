using Terraria;

namespace Aequus.Common {
    public class AequusConditions {
        public static Condition DownedCrabson => new(TextHelper.GetText("Condition.Crabson"), () => AequusWorld.downedCrabson);
        public static Condition DownedOmegaStarite => new(TextHelper.GetText("Condition.OmegaStarite"), () => AequusWorld.downedOmegaStarite);
        public static Condition DownedDustDevil => new(TextHelper.GetText("Condition.DustDevil"), () => AequusWorld.downedDustDevil);
        private static Condition DownedUpriser => new(TextHelper.GetText("Condition.Upriser"), () => AequusWorld.downedUpriser);

        public static Condition DownedHyperStarite => new(TextHelper.GetText("Condition.HyperStarite"), () => AequusWorld.downedHyperStarite);
        public static Condition DownedUltraStarite => new(TextHelper.GetText("Condition.UltraStarite"), () => AequusWorld.downedUltraStarite);
        public static Condition DownedRedSprite => new(TextHelper.GetText("Condition.RedSprite"), () => AequusWorld.downedRedSprite);
        public static Condition DownedSpaceSquid => new(TextHelper.GetText("Condition.SpaceSquid"), () => AequusWorld.downedSpaceSquid);

        public static Condition DownedGlimmer => new(TextHelper.GetText("Condition.Glimmer"), () => AequusWorld.downedEventCosmic);
        public static Condition DownedDemonSiege => new(TextHelper.GetText("Condition.DemonSiege"), () => AequusWorld.downedEventDemon);
        public static Condition DownedGaleStreams => new(TextHelper.GetText("Condition.GaleStreams"), () => AequusWorld.downedEventAtmosphere);
    }
}