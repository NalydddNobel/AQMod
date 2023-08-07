using Aequus.Common.CrossMod;
using Aequus.Content.Events.DemonSiege;
using Aequus.NPCs.BossMonsters.Crabson;
using Aequus.NPCs.BossMonsters.Crabson.CrabsonOld;
using Aequus.NPCs.BossMonsters.DustDevil;
using Aequus.NPCs.BossMonsters.OmegaStarite;
using Aequus.NPCs.Monsters.DemonSiege;
using Aequus.NPCs.Monsters.GaleStreams;
using Aequus.NPCs.Monsters.Glimmer;
using Aequus.NPCs.Monsters.Glimmer.UltraStarite;
using Aequus.NPCs.RedSprite;
using Aequus.NPCs.SpaceSquid;
using Terraria.ModLoader;

namespace Aequus.CrossMod.BossChecklistSupport {
    internal class BossChecklist : ModSupport<BossChecklist> {
        public const string Key = "BossChecklistIntegration";
        public const float GaleStreams_Progression = 8.1f;

        private string CreateDemonSiegeItemList(EventTier tier) {
            string demonSiegeItemList = "";
            foreach (var sacrifice in DemonSiegeSystem.RegisteredSacrifices) {
                if (sacrifice.Value.Hide || sacrifice.Value.Progression != tier)
                    continue;

                if (demonSiegeItemList.Length != 0)
                    demonSiegeItemList += ", ";

                demonSiegeItemList += TextHelper.ItemCommand(sacrifice.Key);
            }
            return demonSiegeItemList;
        }

        public override void PostSetupContent() {
            if (Instance == null)
                return;

            new BossChecklistEntry("Crabson", LogEntryType.Boss, new() { ModContent.NPCType<CrabsonOld>() }, Crabson.BossProgression, () => AequusWorld.downedCrabson)
                .UseCustomPortrait(AequusTextures.CrabsonBossChecklist)
                .Register();
            new BossChecklistEntry("OmegaStarite", LogEntryType.Boss, new() { ModContent.NPCType<OmegaStarite>() }, OmegaStarite.BossProgression, () => AequusWorld.downedOmegaStarite)
                .UseCustomPortrait(AequusTextures.OmegaStariteBossChecklist)
                .Register();
            new BossChecklistEntry("DustDevil", LogEntryType.Boss, new() { ModContent.NPCType<DustDevil>() }, DustDevil.BossProgression, () => AequusWorld.downedDustDevil)
                .UseCustomPortrait(AequusTextures.DustDevilBossChecklist)
                .Register();

            new BossChecklistEntry("UltraStarite", LogEntryType.MiniBoss, new() { ModContent.NPCType<UltraStarite>() }, UltraStarite.BossProgression, () => AequusWorld.downedUltraStarite)
                .UseCustomPortrait(AequusTextures.UltraStariteBossChecklist)
                .Register();
            new BossChecklistEntry("RedSprite", LogEntryType.MiniBoss, new() { ModContent.NPCType<RedSprite>() }, GaleStreams_Progression + 0.01f, () => AequusWorld.downedRedSprite)
                .UseCustomPortrait(AequusTextures.RedSpriteBossChecklist)
                .Register();
            new BossChecklistEntry("SpaceSquid", LogEntryType.MiniBoss, new() { ModContent.NPCType<SpaceSquid>() }, GaleStreams_Progression + 0.02f, () => AequusWorld.downedSpaceSquid)
                .UseCustomPortrait(AequusTextures.SpaceSquidBossChecklist)
                .Register();

            new BossChecklistEntry("Glimmer", LogEntryType.Event, new() { ModContent.NPCType<Starite>(), ModContent.NPCType<SuperStarite>(), ModContent.NPCType<HyperStarite>(), ModContent.NPCType<UltraStarite>() }, 4.6f, () => AequusWorld.downedEventCosmic)
                .UseCustomPortrait(AequusTextures.GlimmerBossChecklist)
                .Register();
            new BossChecklistEntry("DemonSiege", LogEntryType.Event, new() { ModContent.NPCType<Cindera>(), ModContent.NPCType<Magmabubble>(), ModContent.NPCType<TrapperImp>(), ModContent.NPCType<Trapper>() }, 6.1f, () => AequusWorld.downedEventDemon)
                .UseCustomPortrait(AequusTextures.DemonSiegeBossChecklist)
                .UseCustomSpawnInfo(TextHelper.GetOrRegister($"{Key}.DemonSiege.SpawnInfo").WithFormatArgs(CreateDemonSiegeItemList(EventTier.PreHardmode)))
                .Register();
            new BossChecklistEntry("GaleStreams", LogEntryType.Event, new() { ModContent.NPCType<StreamingBalloon>(), ModContent.NPCType<Vraine>(), ModContent.NPCType<WhiteSlime>(), ModContent.NPCType<RedSprite>(), ModContent.NPCType<SpaceSquid>(), }, GaleStreams_Progression, () => AequusWorld.downedEventAtmosphere)
                .UseCustomPortrait(AequusTextures.GaleStreamsBossChecklist)
                .Register();
        }
    }
}