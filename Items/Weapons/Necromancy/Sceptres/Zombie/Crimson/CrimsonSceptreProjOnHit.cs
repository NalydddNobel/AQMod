using Microsoft.Xna.Framework;

namespace Aequus.Items.Weapons.Necromancy.Sceptres.Zombie.Crimson {
    public class CrimsonSceptreProjOnHit : SceptreOnHitSparkleBase {
        public override Color SparkleColor => Color.Red with { A = 0 };
    }
}