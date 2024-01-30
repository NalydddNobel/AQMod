using Aequus.Common.Items;
using Aequus.Content.Bosses.BossMasks;
using Aequus.Content.Bosses.Trophies;

namespace Aequus.Content.Bosses;

// Temporary, until all bosses are ported and can handle item loading per-type.
public class BossItemInstantiator : ModSystem {
    public override void Load() {
#pragma warning disable CS0618 // Type or member is obsolete
        AddBossContent("Crabson", ItemCommons.Rarity.CrabsonLoot, preHardmode: true, new BasicRelicRenderer(AequusTextures.CrabsonRelic), LegacyBossRelicsTile.Crabson);
        AddBossMask("Crabson");

        AddBossContent("OmegaStarite", ItemCommons.Rarity.OmegaStariteLoot, preHardmode: true, new OmegaStariteRelicRenderer(AequusTextures.OmegaStariteRelic, 5), LegacyBossRelicsTile.OmegaStarite);
        AddBossMask("OmegaStarite");

        AddBossContent("DustDevil", ItemCommons.Rarity.DustDevilLoot, preHardmode: true, new BasicRelicRenderer(AequusTextures.DustDevilRelic), LegacyBossRelicsTile.DustDevil);
        Mod.AddContent(new DustDevilMask("DustDevil"));

        AddBossTrophy("UltraStarite", new BasicRelicRenderer(AequusTextures.UltraStariteRelic), LegacyBossRelicsTile.UltraStarite);

        AddBossTrophy("RedSprite", new BasicRelicRenderer(AequusTextures.RedSpriteRelic), LegacyBossRelicsTile.RedSprite);
        AddBossMask("RedSprite");

        AddBossTrophy("SpaceSquid", new BasicRelicRenderer(AequusTextures.SpaceSquidRelic), LegacyBossRelicsTile.SpaceSquid);
        AddBossMask("SpaceSquid");

        void AddBossContent(string name, int internalRarity, bool preHardmode, IRelicRenderer relicRenderer, int legacyTrophyId = -1) {
            Mod.AddContent(new InstancedBossBag(name, internalRarity, preHardmode));
            AddBossTrophy(name, relicRenderer);
        }

        void AddBossTrophy(string name, IRelicRenderer renderer, int legacyTrophyId = -1) {
            var modRelic = new InstancedRelicTile(name, renderer);
            var modTrophy = new InstancedTrophyTile(name);

            Mod.AddContent(modTrophy);
            Mod.AddContent(modRelic);
            Mod.AddContent(new InstancedTrophyItem(modTrophy));
            Mod.AddContent(new InstancedRelicItem(modRelic));

            if (legacyTrophyId > -1) {
                LegacyBossTrophiesTile.LegacyConverter[legacyTrophyId] = modTrophy;
                LegacyBossRelicsTile.LegacyConverter[legacyTrophyId] = modRelic;
            }
        }

        void AddBossMask(string name) {
            Mod.AddContent(new InstancedBossMask(name));
        }
#pragma warning restore CS0618 // Type or member is obsolete
    }
}