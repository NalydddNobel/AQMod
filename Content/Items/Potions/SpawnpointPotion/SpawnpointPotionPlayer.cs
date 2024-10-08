namespace Aequus.Content.Items.Potions.SpawnpointPotion;

public class SpawnpointPotionPlayer : ModPlayer {
    public Point beaconPos;
    public int beaconPointCd;

    SpawnpointBeaconParticles.SpawnpointBeaconEffect? _trackedEffect;

    public bool HasBeaconSpawnpoint => beaconPos.X > 0 && beaconPos.Y > 0;

    public bool HasValidBeaconSpawnpoint => beaconPointCd <= 0 && HasBeaconSpawnpoint;

    public override void Load() {
        On_Player.Spawn_SetPosition += On_Player_Spawn_SetPosition;
    }

    public override void PreUpdate() {
        if (beaconPointCd > 0) {
            beaconPointCd--;
        }
        if (HasBeaconSpawnpoint && !Main.dedServ && (_trackedEffect == null || _trackedEffect.Location != beaconPos)) {
            _trackedEffect = Instance<SpawnpointBeaconParticles>().AddEffect(Player.whoAmI, beaconPos);
        }
    }

    public override void UpdateDead() {
        if (beaconPointCd > 0) {
            beaconPointCd = 0;
            beaconPos = Point.Zero;
        }
    }

    static void On_Player_Spawn_SetPosition(On_Player.orig_Spawn_SetPosition orig, Player self, int floorX, int floorY) {
        bool beaconEffects = false;

        if (self.TryGetModPlayer(out SpawnpointPotionPlayer beaconPlayer) && beaconPlayer.HasValidBeaconSpawnpoint) {
            floorX = beaconPlayer.beaconPos.X;
            floorY = beaconPlayer.beaconPos.Y;

            beaconPlayer.beaconPos = Point.Zero;
            beaconEffects = true;
        }

        orig(self, floorX, floorY);

        if (beaconEffects) {

        }
    }
}
