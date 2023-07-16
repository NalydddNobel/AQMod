using Microsoft.Xna.Framework;

namespace Aequus.Items.Weapons.Necromancy.Sceptres.Zombie.Corruption {
    public class ZombieSceptreProjOnHit : SceptreOnHitSparkleBase {
        public override Color SparkleColor => Color.Blue with { A = 0 };
    }
}