using Aequus.Core.Graphics;
using Aequus.Core.Particles;
using System;
using System.Collections.Generic;

namespace Aequus.Content.Items.PermaPowerups.NoHit;

public class NoHitRewardParticles : ParticleSystem {
    private static int _nextSlot;
    private readonly List<ParticleSlot> _items = new();

    public override int ParticleCount => _items.Count;

    public void New(NoHitReward item) {
        Activate();

        if (item._slot == null || item._slot.SlotId == 0) {
            InitializeNewSlotForItem(item);
            return;
        }

        for (int i = 0; i < _items.Count; i++) {
            if (_items[i].SlotId == item._slot.SlotId) {
                return;
            }
        }

        InitializeNewSlotForItem(item);
    }

    private void InitializeNewSlotForItem(NoHitReward item) {
        item._slot = new ParticleSlot(++_nextSlot);
        item._slot.EnsureItem();
        _items.Add(item._slot);
    }

    public override void Update() {
        Active = false;

        lock (_items) {
            for (int i = 0; i < _items.Count; i++) {
                Active = true;

                ParticleSlot slot = _items[i];
                if (!slot.EnsureItem()) {
                    slot.Opacity -= 0.04f;
                    if (slot.Opacity <= 0f) {
                        _items.RemoveAt(i);
                        i--;
                    }
                    continue;
                }

                Item item = Main.item[slot.Item];
                bool floorFound = ExtendCollision.GetFloor(item.Center, 192, out Vector2 floorPoint);

                if (item.beingGrabbed) {
                    if (slot.Opacity > 0f) {
                        slot.Opacity -= 0.1f;
                    }
                }
                else if (floorFound) {
                    if (slot.Opacity < 1f) {
                        slot.Opacity += 0.02f;
                    }
                }
                else {
                    if (slot.Opacity > 0f) {
                        slot.Opacity -= 0.025f;
                    }
                }

                floorPoint.Y += 2f;

                slot.FloorLocation = (floorPoint / 2f).Floor() * 2f;
            }
        }
    }

    public override void Draw(SpriteBatch spriteBatch) {
        lock (_items) {
            for (int i = 0; i < _items.Count; i++) {
                ParticleSlot slot = _items[i];

                ulong seed = (ulong)slot.SlotId;
                Item item = Main.item[slot.Item];
                Vector2 floorPoint = slot.FloorLocation;

                Texture2D bloom = AequusTextures.Bloom;
                Rectangle bloomFrame = bloom.Frame(verticalFrames: 2, frameY: 0);
                Vector2 bloomOrigin = bloom.Size() / 2f;
                Color bloomColor = Color.White with { A = 0 } * 0.15f * slot.Opacity;

                Vector2 innerBeamScale = new Vector2(0.25f, 5f);
                Vector2 outerBeamScale = new Vector2(0.5f, 5f);

                spriteBatch.Draw(bloom, floorPoint - Main.screenPosition, null, bloomColor, 0f, bloomOrigin, 0.7f, SpriteEffects.None, 0f);
                spriteBatch.Draw(bloom, floorPoint - Main.screenPosition, bloomFrame, bloomColor * 1.5f, 0f, bloomOrigin, innerBeamScale, SpriteEffects.None, 0f);
                spriteBatch.Draw(bloom, floorPoint - Main.screenPosition, bloomFrame, bloomColor, 0f, bloomOrigin, outerBeamScale, SpriteEffects.None, 0f);
                spriteBatch.Draw(bloom, floorPoint - Main.screenPosition, bloomFrame, bloomColor * 1.5f, MathHelper.Pi, bloomOrigin, innerBeamScale.X, SpriteEffects.None, 0f);
                spriteBatch.Draw(bloom, floorPoint - Main.screenPosition, bloomFrame, bloomColor, MathHelper.Pi, bloomOrigin, outerBeamScale.X, SpriteEffects.None, 0f);

                Texture2D dustTexture = AequusTextures.BaseParticleTexture;
                int dustCount = 150;
                Color dustColor = Color.White * 0.5f * slot.Opacity;
                for (int k = 0; k < dustCount; k++) {
                    float flareScale = (0.33f + Utils.RandomFloat(ref seed));
                    float timer = k / (float)dustCount * 2f + Main.GlobalTimeWrappedHourly * 0.1f + Utils.RandomFloat(ref seed) * 0.05f;
                    float timerWrapped = timer % 2f;
                    Vector2 sparklePosition = floorPoint - Main.screenPosition + new Vector2(Utils.RandomInt(ref seed, -24, 24), Utils.RandomInt(ref seed, -300, -40) + 4f);
                    Rectangle dustFrame = dustTexture.Frame(verticalFrames: 3, frameY: Utils.RandomInt(ref seed, 3));

                    if (timerWrapped > 1f) {
                        continue;
                    }

                    timerWrapped = MathF.Pow(timerWrapped, 5f);

                    float wave = MathF.Sin(timerWrapped * MathHelper.Pi);
                    sparklePosition.Y += timerWrapped * 48f * flareScale;
                    Main.spriteBatch.Draw(dustTexture, sparklePosition, dustFrame, dustColor * MathF.Pow(wave, 30f), 0f, dustFrame.Size() / 2f, wave * flareScale, SpriteEffects.None, 0f);
                }
            }
        }
    }

    public override void OnActivate() {
        DrawLayers.Instance.WorldBehindTiles += Draw;
    }

    public override void Deactivate() {
        DrawLayers.Instance.WorldBehindTiles -= Draw;
    }

    public record ParticleSlot(int SlotId) {
        public int Item { get; private set; }
        public float Opacity;
        public Vector2 FloorLocation;

        public bool EnsureItem() {
            if (ValidItem(Item)) {
                return true;
            }

            for (int i = 0; i < Main.maxItems; i++) {
                if (ValidItem(i)) {
                    Item = i;
                    return true;
                }
            }

            return false;
        }

        private bool ValidItem(int i) {
            return Main.item[i].active && Main.item[i].ModItem is NoHitReward currentReward && currentReward._slot.SlotId == SlotId;
        }
    }
}