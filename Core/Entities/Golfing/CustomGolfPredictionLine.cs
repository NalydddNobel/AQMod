using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using Terraria.GameContent;
using Terraria.GameContent.Golf;
using Terraria.Graphics;

namespace Aequu2.Core.Entities.Golfing;

public class CustomGolfPredictionLine {
    private static CustomGolfPredictionLine Instance { get; set; }

    private readonly List<Vector2> _positions;

    private readonly int _iterations;

    private readonly Color[] _colors = new Color[2]
    {
        Color.White,
        Color.Gray
    };

    private readonly BasicDebugDrawer _drawer = new BasicDebugDrawer(Main.instance.GraphicsDevice);

    private float _time;

    public CustomGolfPredictionLine(int iterations) {
        _positions = new List<Vector2>(iterations * 2 + 1);
        _iterations = iterations;
    }

    public void Update(Entity golfBall, Vector2 impactVelocity, float roughLandResistance) {
        bool bubbleSolid = Main.tileSolid[TileID.Bubble];
        Main.tileSolid[TileID.Bubble] = false;

        _positions.Clear();
        _time += 1f / 60f;
        var oldPosition = golfBall.position;
        var oldVelocity = golfBall.velocity;
        GolfHelper.HitGolfBall(golfBall, impactVelocity, roughLandResistance);
        _positions.Add(golfBall.position);
        float angularVelocity = 0f;
        for (int i = 0; i < _iterations; i++) {
            GolfHelper.StepGolfBall(golfBall, ref angularVelocity);
            _positions.Add(golfBall.position);
        }
        golfBall.velocity = oldVelocity;
        golfBall.position = oldPosition;

        Main.tileSolid[TileID.Bubble] = bubbleSolid;
    }

    public static void Draw(Entity golfBall, Vector2 impactVelocity, float chargeProgress, float roughLandResistance) {
        Instance ??= new(20);

        Instance.Update(golfBall, impactVelocity, roughLandResistance);
        Instance.Draw(Main.Camera, Main.spriteBatch, chargeProgress);
    }

    public void Draw(Camera camera, SpriteBatch spriteBatch, float chargeProgress) {
        _drawer.Begin(camera.GameViewMatrix.TransformationMatrix);

        var value = TextureAssets.Extra[33].Value;
        var origin = value.Size() / 2f;
        var positionOffset = new Vector2(3.5f, 3.5f) - camera.UnscaledPosition;
        float localTraveled = 0f;
        float totalTraveled = 0f;
        for (int i = 0; i < _positions.Count - 1; i++) {
            GetSectionLength(i, out var length, out var _);
            if (length != 0f) {
                for (; localTraveled < totalTraveled + length; localTraveled += 4f) {
                    float num3 = (localTraveled - totalTraveled) / length + i;
                    var position = GetPosition((localTraveled - totalTraveled) / length + i);
                    var color = GetColor(num3);
                    color *= MathHelper.Clamp(2f - 2f * num3 / (_positions.Count - 1), 0f, 1f);
                    spriteBatch.Draw(value, position + positionOffset, null, color, 0f, origin, GetScale(localTraveled), SpriteEffects.None, 0f);
                }

                totalTraveled += length;
            }
        }

        _drawer.End();
    }

    private Color GetColor(float index) {
        float num = index * 0.5f - _time * MathF.PI * 1.5f;
        int colorIndex = (int)Math.Floor(num) % _colors.Length;
        if (colorIndex < 0) {
            colorIndex += _colors.Length;
        }

        float amount = num - (float)Math.Floor(num);
        return Color.Lerp(_colors[colorIndex], _colors[(colorIndex + 1) % _colors.Length], amount) with { A = 64 } * 0.6f;
    }

    private float GetScale(float travelledLength) {
        return 0.2f + Utils.GetLerpValue(0.8f, 1f, (float)Math.Cos(travelledLength / 50f + _time * -MathF.PI) * 0.5f + 0.5f, clamped: true) * 0.15f;
    }

    private void GetSectionLength(int startIndex, out float length, out float rotation) {
        int num = startIndex + 1;
        if (num >= _positions.Count) {
            num = _positions.Count - 1;
        }

        length = Vector2.Distance(_positions[startIndex], _positions[num]);
        rotation = (_positions[num] - _positions[startIndex]).ToRotation();
    }

    private Vector2 GetPosition(float indexProgress) {
        int num = (int)Math.Floor(indexProgress);
        int num2 = num + 1;
        if (num2 >= _positions.Count) {
            num2 = _positions.Count - 1;
        }

        float amount = indexProgress - num;
        return Vector2.Lerp(_positions[num], _positions[num2], amount);
    }
}