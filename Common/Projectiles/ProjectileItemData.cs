using Aequus.Common.Items.Components;
using System.IO;
using Terraria.ModLoader.IO;

namespace Aequus.Common.Projectiles;

public partial class ProjectileItemData : GlobalProjectile {
    protected override bool CloneNewInstances => true;
    public override bool InstancePerEntity => true;

    /// <summary>
    /// Custom data to be used by items which utilize the <see cref="IManageProjectile"/> interface or by channeled items. This data is synced in multiplayer.
    /// </summary>
    public int ItemData { get; internal set; }

    /// <summary>
    /// If this is true, special effects should be disabled on the projectile. This is only set to true by Javelin-like projectiles.
    /// </summary>
    public bool NoSpecialEffects { get; set; }

    public override void SetDefaults(Projectile projectile) {
        NoSpecialEffects = false;
        ItemData = 0;
    }

    public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter) {
        bitWriter.WriteBit(NoSpecialEffects);

        bitWriter.WriteBit(ItemData != 0);
        if (ItemData != 0) {
            binaryWriter.Write(ItemData);
        }
    }

    public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader) {
        NoSpecialEffects = bitReader.ReadBit();

        if (bitReader.ReadBit()) {
            ItemData = binaryReader.ReadInt32();
        }
        else {
            ItemData = 0;
        }
    }
}