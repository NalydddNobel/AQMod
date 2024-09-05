namespace Aequus;

partial class AequusSystem {
    public override void PostUpdateWorld() {
        PostUpdateWorldInner();
    }

    /*
    public override void ClearWorld() {
        ClearWorldInner();
    }

    public override void SaveWorldData(TagCompound tag) {
        SaveInner(tag);
    }

    public override void LoadWorldData(TagCompound tag) {
        LoadInner(tag);
    }

    public override void NetSend(BinaryWriter writer) {
        SendDataInner(writer);
    }

    public override void NetReceive(BinaryReader reader) {
        ReceiveDataInner(reader);
    }
    */
}
