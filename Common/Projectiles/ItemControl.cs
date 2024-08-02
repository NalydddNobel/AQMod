using Aequus.Projectiles;
using System.IO;
using Terraria.ModLoader.IO;

namespace Aequus.Common.Projectiles;

public partial class ItemControl : GlobalProjectile {
    protected override bool CloneNewInstances => true;
    public override bool InstancePerEntity => true;

    /// <summary>Custom data to be used by items which utilize the <see cref="IManageProjectile"/> interface or by channeled items. This data is synced in multiplayer.</summary>
    public int ItemAI { get; internal set; }

    /// <summary>If this is true, special effects should be disabled on the projectile. This is only set to true by Javelin-like projectiles.</summary>
    public bool NoSpecialEffects { get; set; }

    private IManageProjectile? _item;

    public override void SetDefaults(Projectile projectile) {
        NoSpecialEffects = false;
        ItemAI = 0;
    }

    public override bool PreAI(Projectile projectile) {
        if (projectile.TryGetGlobalProjectile(out AequusProjectile aequus) && aequus.FromItem) {
            ModItem item = ItemLoader.GetItem(aequus.sourceItemUsed);
            if (item is IManageProjectile manager) {
                _item = manager;
            }
        }
        return _item?.PreAIProjectile(projectile) ?? true;
    }

    public override void AI(Projectile projectile) {
        _item?.AIProjectile(projectile);
    }

    public override void PostAI(Projectile projectile) {
        _item?.PostAIProjectile(projectile);
    }

    public override bool PreDraw(Projectile projectile, ref Color lightColor) {
        return _item?.PreDrawProjectile(projectile, ref lightColor) ?? true;
    }

    public override void PostDraw(Projectile projectile, Color lightColor) {
        _item?.PostDrawProjectile(projectile, in lightColor);
    }

    public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone) {
        _item?.OnHitNPCProjectile(projectile, target, hit, damageDone);
    }

    public override bool OnTileCollide(Projectile projectile, Vector2 oldVelocity) {
        return _item?.OnTileCollideProjectile(projectile, oldVelocity) ?? true;
    }

    public override bool PreKill(Projectile projectile, int timeLeft) {
        return _item?.PreKillProjectile(projectile, timeLeft) ?? true;
    }

    public override void OnKill(Projectile projectile, int timeLeft) {
        _item?.OnKillProjectile(projectile, timeLeft);
    }

    public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter) {
        bitWriter.WriteBit(NoSpecialEffects);

        bitWriter.WriteBit(ItemAI != 0);
        if (ItemAI != 0) {
            binaryWriter.Write(ItemAI);
        }
    }

    public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader) {
        NoSpecialEffects = bitReader.ReadBit();

        if (bitReader.ReadBit()) {
            ItemAI = binaryReader.ReadInt32();
        }
        else {
            ItemAI = 0;
        }
    }
}