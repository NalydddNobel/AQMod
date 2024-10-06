using Aequus.Common.Drawing;
using Aequus.Common.Net;
using Aequus.Common.Utilities.Helpers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Terraria.ModLoader.IO;

namespace Aequus.Common.Entities.TileActors;

/// <summary>The system which manages the custom Tile-Entity-like "Grid Actors".</summary>
public class GridActorSystem : ModSystem {
    public const int SectionX = 200;
    public const int SectionY = 200;

    public int SectionsWidth { get; private set; }
    public int SectionsHeight { get; private set; }

    public static List<GridActor> Registered = [];

    uint NextId;
    readonly Dictionary<uint, GridActor> _actors = [];

    List<GridActor>[,]? _actorSections;

    public static byte Register(GridActor next) {
        Registered.Add(next);

        if (Registered.Count > byte.MaxValue) {
            throw new System.Exception("Registered too many Grid Actor types. Upgrade ids from byte to ushort.");
        }

        return (byte)(Registered.Count - 1);
    }

    /// <returns>All actors found within the section that contains <paramref name="tileX"/> and <paramref name="tileY"/>. Returns null if out of bounds.</returns>
    public bool TryGetActorsInSection(int tileX, int tileY, [NotNullWhen(true)] out List<GridActor> actors) {
        int secX = tileX / SectionX;
        int secY = tileY / SectionY;

        if (secX < 0 || secX >= SectionsWidth || secY < 0 || secY >= SectionsHeight) {
            actors = null!;
            return false;
        }

        actors = _actorSections![secX, secY]!;
        return true;
    }

    /// <returns>All actors found within the section that contains <paramref name="tileX"/> and <paramref name="tileY"/>. Returns null if out of bounds.</returns>
    public List<GridActor>? GetActorsInSection(int tileX, int tileY) {
        int secX = tileX / SectionX;
        int secY = tileY / SectionY;

        return _actorSections![secX, secY]!;
    }

    public override void SetStaticDefaults() {
        if (Main.netMode == NetmodeID.Server) {
            Instance<SectionsSystem>().SendSection += SendSection;
        }
    }

    void SendSection(SectionQueueInfo info) {
        for (int x = info.X / SectionX * SectionX; x < info.X + info.Width; x += SectionsWidth) {
            for (int y = info.Y / SectionY * SectionY; y < info.Y + info.Height; y += SectionsHeight) {
                if (!TryGetActorsInSection(x, y, out var actors)) {
                    continue;
                }

                foreach (GridActor actor in actors) {
                    if (info.Bounds.Contains(actor.Location)) {
                        Instance<GridActorPacket>().Send(actor);
                    }
                }
            }
        }
    }

    public GridActor Place(int x, int y, byte actorId) {
        return Place(new Point(x, y), actorId);
    }

    public GridActor Place(Point location, byte type) {
        GridActor next = PlaceManual(location, type, NextId++);
        next.OnPlace();
        return next;
    }

    internal GridActor PlaceManual(Point location, byte type, uint id) {
        GridActor next = Registered[type].NextInstance();
        next.Location = location;
        next.Id = id;

        _actors[id] = next;

        int secX = location.X / SectionX;
        int secY = location.Y / SectionY;
        if (secX >= 0 && secX < SectionsWidth && secY >= 0 && secY < SectionsHeight) {
            _actorSections![secX, secY]!.Add(next);
        }

        return next;
    }

    public bool Remove(GridActor instance) {
        if (!_actors.Remove(instance.Id)) {
            return false;
        }

        int secX = instance.Location.X / SectionX;
        int secY = instance.Location.Y / SectionY;
        if (secX >= 0 && secX < SectionsWidth && secY >= 0 && secY < SectionsHeight) {
            _actorSections![secX, secY]!.Remove(instance);
        }

        if (instance is IGridDrawSystem draw) {
            Instance<TileDrawSystem>().RemoveGridActor(instance);
        }

        instance.OnRemove();

        return true;
    }

    public override void ClearWorld() {
        SectionsWidth = MathTools.DivCeiling(Main.maxTilesX, SectionX);
        SectionsHeight = MathTools.DivCeiling(Main.maxTilesY, SectionY);
        NextId = 0;

        _actorSections = ArrayTools.New2D(SectionsWidth, SectionsHeight, (x, y) => new List<GridActor>());
    }

    const string Tag_ActorData = "Actors";
    const string Tag_Actor_Type = "Id";
    const string Tag_Actor_Location = "XY";

    public override void SaveWorldData(TagCompound tag) {
        TagCompound actorData = [];

        foreach (var pair in _actors) {
            TagCompound perActorData = [];

            perActorData[Tag_Actor_Type] = pair.Value.Type;
            perActorData[Tag_Actor_Location] = pair.Value.Location;

            pair.Value.SaveData(perActorData);

            actorData[pair.Key.ToString(CultureInfo.InvariantCulture)] = perActorData;
        }

        tag[Tag_ActorData] = actorData;
    }

    public override void LoadWorldData(TagCompound tag) {
        _actors.Clear();
        NextId = 0;

        if (!tag.TryGet(Tag_ActorData, out TagCompound actorData)) {
            return;
        }

        foreach (var perActorData in actorData.Where(t => t.Value is TagCompound).Select(t => t.Value as TagCompound)) {
            if (!perActorData!.TryGet(Tag_Actor_Type, out byte type)) {
                continue;
            }
            if (!perActorData.TryGet(Tag_Actor_Location, out Point xy)) {
                continue;
            }

            GridActor a = PlaceManual(xy, type, NextId++);

            a.LoadData(perActorData);
        }
    }
}
