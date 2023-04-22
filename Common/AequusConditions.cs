using Terraria;

namespace Aequus.Common {
    public class AequusConditions {
        public static Condition DownedCrabson => new(TextHelper.GetOrRegister("Condition.DownedCrabson"), () => AequusWorld.downedCrabson);
        public static Condition DownedOmegaStarite => new(TextHelper.GetOrRegister("Condition.DownedOmegaStarite"), () => AequusWorld.downedOmegaStarite);
        public static Condition DownedDustDevil => new(TextHelper.GetOrRegister("Condition.DownedDustDevil"), () => AequusWorld.downedDustDevil);
        private static Condition DownedUpriser => new(TextHelper.GetOrRegister("Condition.DownedUpriser"), () => AequusWorld.downedUpriser);

        public static Condition DownedHyperStarite => new(TextHelper.GetOrRegister("Condition.DownedHyperStarite"), () => AequusWorld.downedHyperStarite);
        public static Condition DownedUltraStarite => new(TextHelper.GetOrRegister("Condition.DownedUltraStarite"), () => AequusWorld.downedUltraStarite);
        public static Condition DownedRedSprite => new(TextHelper.GetOrRegister("Condition.DownedRedSprite"), () => AequusWorld.downedRedSprite);
        public static Condition DownedSpaceSquid => new(TextHelper.GetOrRegister("Condition.DownedSpaceSquid"), () => AequusWorld.downedSpaceSquid);

        public static Condition DownedGlimmer => new(TextHelper.GetOrRegister("Condition.DownedGlimmer"), () => AequusWorld.downedEventCosmic);
        public static Condition DownedDemonSiege => new(TextHelper.GetOrRegister("Condition.DownedDemonSiege"), () => AequusWorld.downedEventDemon);
        public static Condition DownedGaleStreams => new(TextHelper.GetOrRegister("Condition.DownedGaleStreams"), () => AequusWorld.downedEventAtmosphere);

        public static Condition InCrabCrevice => new(TextHelper.GetOrRegister("Condition.InCrabCrevice"), () => Main.LocalPlayer.Aequus().ZoneCrabCrevice);
        public static Condition InGlimmer => new(TextHelper.GetOrRegister("Condition.InGlimmer"), () => Main.LocalPlayer.Aequus().ZoneGlimmer);
        public static Condition InDemonSiege => new(TextHelper.GetOrRegister("Condition.InDemonSiege"), () => Main.LocalPlayer.Aequus().ZoneDemonSiege);
        public static Condition InGoreNest => new(TextHelper.GetOrRegister("Condition.InGoreNest"), () => Main.LocalPlayer.Aequus().ZoneGoreNest);
        public static Condition InPeacefulGlimmer => new(TextHelper.GetOrRegister("Condition.InPeacefulGlimmer"), () => Main.LocalPlayer.Aequus().ZonePeacefulGlimmer);
    }
}