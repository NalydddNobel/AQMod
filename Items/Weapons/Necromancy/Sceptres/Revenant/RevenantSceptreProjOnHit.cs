using Microsoft.Xna.Framework;

namespace Aequus.Items.Weapons.Necromancy.Sceptres.Revenant {
    public class RevenantSceptreProjOnHit : SceptreOnHitSparkleBase {
        public override Color SparkleColor => Color.Blue with { A = 0 };
    }
}