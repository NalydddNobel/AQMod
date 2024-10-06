using System.IO;
using Terraria.ModLoader.IO;

namespace Aequus.Common.Entities.TileActors;

public abstract class GridActor : ModType {
    public byte Type { get; internal set; }
    public Point Location { get; internal set; }
    public uint Id;

    protected sealed override void Register() {
        Type = GridActorSystem.Register(this);
    }

    public sealed override void SetupContent() {
        SetStaticDefaults();
    }

    public virtual GridActor NextInstance() {
        return (GridActor)MemberwiseClone();
    }

    public virtual void OnPlace() { }
    public virtual void OnRemove() { }
    public virtual void SaveData(TagCompound tag) { }
    public virtual void LoadData(TagCompound tag) { }
    public virtual void SendData(BinaryWriter writer) { }
    public virtual void ReceiveData(BinaryReader reader) { }
}
