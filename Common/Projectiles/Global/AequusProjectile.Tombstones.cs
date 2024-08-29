using System.Collections.Generic;
using Terraria.DataStructures;

namespace Aequus.Projectiles;
public partial class AequusProjectile : GlobalProjectile {
    public bool _checkTombstone;
    public static List<int> HellTombstones { get; private set; }

    public void Load_Tombstones() {
        HellTombstones = new List<int>();
    }

    public void Unload_Tombstones() {
        HellTombstones?.Clear();
        HellTombstones = null;
    }

    public void OnSpawn_Tombstones(Projectile projectile, IEntitySource source) {
        //if (source is EntitySource_Misc misc && misc.Context == "PlayerDeath_TombStone")
        //{
        //    _checkTombstone = true;
        //    if (projectile.position.Y > Main.UnderworldLayer * 16f)
        //    {
        //        projectile.Aequus().transform = Main.rand.Next(HellTombstones);
        //    }
        //}
    }

    public void InitAI_Tombstones(Projectile projectile) {
        if (_checkTombstone) {
            _checkTombstone = false;
            if (!Main.player[projectile.owner].active) {
                return;
            }
            switch (Main.player[projectile.owner].name) {
                case "Leinfors": {
                        projectile.miscText += TextHelper.GetText("DeathMessage.Respect");
                    }
                    break;
            }
        }
    }
}