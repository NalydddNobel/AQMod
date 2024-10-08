using Aequus.Common.Drawing;
using Aequus.Common.Particles.New;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Aequus.Content.Items.Potions.SpawnpointPotion;

internal class SpawnpointBeaconParticles : NewParticleSystem {
    public override int ParticleCount => _particles.Count;

    readonly Dictionary<int, List<SpawnpointBeaconEffect>> _particles = [];

    public override void OnActivate() {
        Instance<DrawLayers>().PreDrawPlayer += DrawPortals;
    }

    public override void Deactivate() {
        Instance<DrawLayers>().PreDrawPlayer -= DrawPortals;
    }

    void DrawPortals(SpriteBatch sb, Player player) {
        if (!player.TryGetModPlayer(out SpawnpointPotionPlayer beaconPlayer) || !_particles.TryGetValue(player.whoAmI, out var list) || list.Count == 0) {
            return;
        }

        Texture2D texture = AequusTextures.SpawnpointPotionPortal;
        foreach (var b in list) {
            Vector2 drawCoordinates = b.Location.ToWorldCoordinates() - Main.screenPosition;
            Color color = Color.White with { A = 128 } * 0.5f * b.Opacity;
            float rotation = b.Timer * 0.1f;
            sb.DrawAlign(texture, drawCoordinates, null, color, rotation, b.Scale, SpriteEffects.None);
            sb.DrawAlign(texture, drawCoordinates, null, color, -rotation, b.Scale, SpriteEffects.FlipHorizontally);
        }
    }

    public override void Draw(SpriteBatch spriteBatch) { }

    public override void Update() {
        foreach (var list in _particles.Values) {
            list.RemoveAll(b => b.Scale <= 0f || b.Opacity <= 0f);
            foreach (SpawnpointBeaconEffect b in list) {
                Active = true;
                Player player = Main.player[b.Owner];
                if (!player.active || !player.TryGetModPlayer(out SpawnpointPotionPlayer beaconPlayer) || beaconPlayer.beaconPos != b.Location) {
                    b.Scale -= 0.01f;
                    b.Scale *= 0.99f;
                    b.Opacity *= 0.98f;
                    continue;
                }

                float maxScale = 0.6f;
                float maxOpacity = 0.5f;
                if (beaconPlayer.beaconPointCd <= 0) {
                    maxScale = 1.15f;
                    maxOpacity = 1f;
                }

                if (b.Scale < maxScale) {
                    b.Scale += 0.02f;
                    b.Scale *= 1.01f;

                    if (b.Scale > maxScale) {
                        b.Scale = maxScale;
                    }
                }

                if (b.Opacity < maxOpacity) {
                    b.Opacity += 0.05f;
                    if (b.Opacity > maxOpacity) {
                        b.Opacity = maxOpacity;
                    }
                }

                b.Timer++;

                float size = (AequusTextures.SpawnpointPotionPortal.Width() / 2f + Main.rand.NextFloat(-6f, 2f)) * b.Scale;
                if (Main.GameUpdateCount % 6 == 0 || Main.rand.NextBool(12)) {
                    Vector2 velocity = Main.rand.NextVector2Unit();
                    Dust d = Dust.NewDustPerfect(b.Location.ToWorldCoordinates() + velocity * size, DustID.MagicMirror, Velocity: velocity.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(0.2f, 0.66f), Alpha: 200, Scale: 0.5f);
                    d.fadeIn = d.scale + 0.33f;
                }
            }
        }
    }

    public SpawnpointBeaconEffect AddEffect(int player, Point where) {
        SpawnpointBeaconEffect nextEffect = new() {
            Location = where,
            Scale = 0.05f,
            Opacity = 0.1f,
            Timer = 0f,
            Owner = player
        };

        Activate();
        (CollectionsMarshal.GetValueRefOrAddDefault(_particles, player, out _) ??= new()).Add(nextEffect);

        return nextEffect;
    }

    public class SpawnpointBeaconEffect {
        public Point Location;
        public float Scale;
        public float Opacity;
        public float Timer;
        public int Owner;
    }
}
