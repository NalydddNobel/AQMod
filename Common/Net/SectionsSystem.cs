using System.Collections.Generic;
using Terraria.Localization;

namespace Aequus.Common.Net;

[Autoload(Side = ModSide.Server)]
public class SectionsSystem : ModSystem {
    public OnSectionSent? SendSection { get; internal set; }

    readonly Queue<SectionQueueInfo> _sectionQueue = [];

    public override void PreUpdateWorld() {
        if (_sectionQueue.TryDequeue(out var info)) {
            SendSection?.Invoke(info);
        }
    }

    public override bool HijackSendData(int whoAmI, int msgType, int remoteClient, int ignoreClient, NetworkText text, int number, float number2, float number3, float number4, int number5, int number6, int number7) {
        if (msgType == MessageID.TileSection) {
            int x = number;
            int y = (int)number2;
            int w = (int)number3;
            int h = (int)number4;

            _sectionQueue.Enqueue(new() {
                ToClient = whoAmI,
                X = x,
                Y = y,
                Width = w,
                Height = h
            });
        }
        return false;
    }
}

public delegate void OnSectionSent(SectionQueueInfo info);

public readonly record struct SectionQueueInfo(int ToClient, int X, int Y, int Width, int Height) {
    public readonly Rectangle Bounds = new Rectangle(X, Y, Width, Height);
}