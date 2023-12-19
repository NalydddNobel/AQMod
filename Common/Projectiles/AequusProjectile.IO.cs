using System.IO;
using Terraria.ModLoader.IO;

namespace Aequus.Common.Projectiles;

public partial class AequusProjectile {
    public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter) {
        bitWriter.WriteBit(isProjectileChild);
        bitWriter.WriteBit(noSpecialEffects);
        bitWriter.WriteBit(parentNPCIndex > -1);
        if (parentNPCIndex > -1) {
            binaryWriter.Write(parentNPCIndex);
        }
        bitWriter.WriteBit(parentItemType > 0);
        if (parentItemType > 0) {
            binaryWriter.Write(parentItemType);

            bitWriter.WriteBit(itemData != 0);
            if (itemData != 0) {
                binaryWriter.Write(itemData);
            }
        }
    }

    public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader) {
        itemData = 0;
        isProjectileChild = bitReader.ReadBit();
        noSpecialEffects = bitReader.ReadBit();
        if (bitReader.ReadBit()) {
            parentNPCIndex = binaryReader.ReadInt16();
        }
        if (bitReader.ReadBit()) {
            parentItemType = binaryReader.ReadInt32();

            if (bitReader.ReadBit()) {
                itemData = binaryReader.ReadInt32();
            }
        }
    }
}