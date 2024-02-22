using Aequus.Common.Items;
using Aequus.Content.Bosses.BossMasks;
using Aequus.Content.Bosses.Trophies;

namespace Aequus.Content.Bosses;

// Temporary, until all bosses are ported and can handle item loading per-type.
public class BossItemInstantiator : ModSystem {
    public override void Load() {
#pragma warning disable CS0618 // Type or member is obsolete
        //AddBossContent("Crabson", ItemCommons.Rarity.CrabsonLoot, preHardmode: true, new BasicRelicRenderer(AequusTextures.CrabsonRelic), LegacyBossRelicsTile.Crabson);
        //AddBossMask("Crabson");

        AddBossContent("DustDevil", ItemCommons.Rarity.DustDevilLoot, preHardmode: true, new BasicRelicRenderer(AequusTextures.DustDevilRelic), LegacyBossRelicsTile.DustDevil);
        Mod.AddContent(new DustDevilMask("DustDevil"));

        AddLegacyTrophy("RedSprite", new BasicRelicRenderer(AequusTextures.RedSpriteRelic), LegacyBossRelicsTile.RedSprite);
        AddBossMask("RedSprite");

        AddLegacyTrophy("SpaceSquid", new BasicRelicRenderer(AequusTextures.SpaceSquidRelic), LegacyBossRelicsTile.SpaceSquid);
        AddBossMask("SpaceSquid");

        void AddBossContent(string name, int internalRarity, bool preHardmode, IRelicRenderer relicRenderer, int legacyTrophyId = -1) {
            AddLegacyTrophy(name, relicRenderer, legacyTrophyId);
        }

        void AddBossMask(string name) => Mod.AddContent(new InstancedBossMask(name));
#pragma warning restore CS0618 // Type or member is obsolete
    }

    private void AddTrophiesDirect(InstancedRelicTile relic, InstancedTrophyTile trophy, int legacyTrophyId) {
        Mod.AddContent(trophy);
        Mod.AddContent(relic);
        Mod.AddContent(new InstancedTrophyItem(trophy));
        Mod.AddContent(new InstancedRelicItem(relic));

        if (legacyTrophyId > -1) {
            LegacyBossTrophiesTile.LegacyConverter[legacyTrophyId] = trophy;
            LegacyBossRelicsTile.LegacyConverter[legacyTrophyId] = relic;
        }
    }

    private void AddLegacyTrophy(string name, IRelicRenderer renderer, int legacyTrophyId = -1) {
        var modRelic = new InstancedRelicTile(name, renderer);
        var modTrophy = new InstancedTrophyTile(name);
        AddTrophiesDirect(modRelic, modTrophy, legacyTrophyId);
    }

    public static void AddTrophies(ModNPC modNPC, IRelicRenderer renderer, int legacyId) {
        var modRelic = new InstancedRelicTile(modNPC, renderer);
        var modTrophy = new InstancedTrophyTile(modNPC);

        ModContent.GetInstance<BossItemInstantiator>().AddTrophiesDirect(modRelic, modTrophy, legacyId);
    }
}