﻿namespace Aequus.Common.Necromancy {
    public struct ZombieInfo {
        public bool IsZombie;
        public int PlayerOwner;
        public int SetDamage;

        public bool HasSetDamage => SetDamage > 0;

        public ZombieInfo() {
            IsZombie = false;
            PlayerOwner = 0;
            SetDamage = 0;
        }

        public void Inherit(ZombieInfo info) {
            IsZombie = info.IsZombie;
            PlayerOwner = info.PlayerOwner;
            SetDamage = info.SetDamage;
        }
    }
}