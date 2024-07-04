using AequusRemake.Core.Structures.ID;
using System.Collections.Generic;

namespace AequusRemake.Content.CrossMod.SplitSupport.Photography;

internal sealed partial class PhotographyLoader : ModSystem {
    public static InstancedEnvelope EnvelopePollutedOcean { get; private set; }
    public static InstancedEnvelope EnvelopeGlimmer { get; private set; }

    private readonly List<Album> _albumsToRegister = new();

    /// <param name="order">Sorting value for this album.</param>
    /// <param name="specialReward">Special reward for this album</param>
    /// <param name="values">Must have a length of 6.</param>
    private void AddPhotographyPage(float order, int specialReward, AlbumQuestInfo[] values) {
        _albumsToRegister.Add(new Album(order, specialReward, values));
    }

    private void LoadEnvelopes() {
        EnvelopePollutedOcean = new InstancedEnvelope("PollutedOcean", preHardmode: true);
        EnvelopeGlimmer = new InstancedEnvelope("Glimmer", preHardmode: true);

        Mod.AddContent(EnvelopePollutedOcean);
        Mod.AddContent(EnvelopeGlimmer);
        ModTypeLookup<ModItem>.RegisterLegacyNames(EnvelopePollutedOcean, "UndergroundOceanEnvelope");
    }

    private void LoadAlbumsAfterEnvelopes() {
        IProvideId spaceEnvelope = Split.GetContentProvider<ModItem>("BlueSkyEnvelope", ItemID.FloatingIslandFishingCrate);
        IProvideId hellEnvelope = Split.GetContentProvider<ModItem>("FieryEnvelope", ItemID.ObsidianLockbox);
        //IContentIdProvider dungeonEnvelope = Split.GetContentProvider<ModItem>("DungeonEnvelope", ItemID.DungeonFishingCrateHard);
        IProvideId bloodMoonEnvelope = Split.GetContentProvider<ModItem>("HorrificEnvelope", ItemID.DungeonFishingCrateHard);

        /*
        AddPhotographyPage(
            order: 10f,
            specialReward: ModContent.GetInstance<FamiliarPet>().PetItem.Type,
            [
                new(0, GetNPC<Enemies.PollutedOcean.BreadOfCthulhu.BreadOfCthulhu>(), Envelope(EnvelopePollutedOcean)),
                new(1, GetNPC<Old.Content.Enemies.BloodMoon.BloodMimic>(), bloodMoonEnvelope),
                new(2, GetNPC<Old.Content.Bosses.UltraStarite.UltraStarite>(), Envelope(EnvelopeGlimmer)),
                new(3, GetNPC<Old.Content.Enemies.DemonSiege.CinderBat.CinderBat>(), hellEnvelope),

                // Oblivision
                new(4, new Id(0), hellEnvelope),

                new(5, GetNPC<TownNPCs.SkyMerchant.SkyMerchant>(), spaceEnvelope),
            ]);
        */

        IProvideId Envelope(ModItem modItem) => new TypeId<ModItem>(modItem);
        IProvideId GetNPC<T>() where T : ModNPC => new TypeId<T>();
    }

    private void LoadPrintsAfterAlbums() {
        foreach (Album album in _albumsToRegister) {
            for (int i = 0; i < album.Quests.Length; i++) {
                if (album.Quests[i].NPC.GetId() > 0) {
                    InstancedPosterItem poster = new InstancedPosterItem(album.Quests[i]);
                    Mod.AddContent(poster);
                    album.Quests[i].Poster = poster;
                }
            }
        }
    }

    private void RegisterAlbums() {
        if (!Split.Enabled) {
            return;
        }

        foreach (Album album in _albumsToRegister) {
            var args = new object[35];
            args[0] = "Photography.AddPage";
            args[1] = AequusRemake.Instance;
            args[2] = album.Order;
            args[3] = album.SpecialReward;
            args[4] = Main.dedServ ? null : AequusTextures.SplitAlbum.Value;

            for (int i = 0; i < 6; i++) {
                int index = 5 + i * 5;
                AlbumQuestInfo quest = album.Quests[i];
                args[index] = quest.Frame;
                args[index + 1] = quest.NPCIds;
                args[index + 2] = quest.Envelope;
                args[index + 3] = quest.Poster.Type;
                args[index + 4] = quest.SpecialCondition;
            }

            Split.Instance.Call(args);
        }
    }

    public override void Load() {
        LoadEnvelopes();
    }

    public override void OnModLoad() {
        LoadAlbumsAfterEnvelopes();
        LoadPrintsAfterAlbums();
    }

    public override void PostSetupContent() {
        RegisterAlbums();
    }
}
