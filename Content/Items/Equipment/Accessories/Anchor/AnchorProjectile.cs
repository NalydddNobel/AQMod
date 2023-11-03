using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Equipment.Accessories.Anchor;

public class AnchorProjectile : ModProjectile {
    public override string Texture => AequusTextures.Projectile(ProjectileID.Anchor);

    //private Vector2[][] _frontCoordinates;
    //private float[][] _frontRotations;

    //private Vector2[][] _backCoordinates;
    //private float[][] _backRotations;

    public override void SetDefaults() {
        Projectile.width = 20;
        Projectile.height = 20;
        Projectile.aiStyle = -1;
        Projectile.penetrate = -1;
        Projectile.hide = true;
    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) {
        behindNPCsAndTiles.Add(index);
    }

    //private T[][] InitArr<T>(int amount, int segments) {
    //    var value = new T[amount][];
    //    for (int k = 0; k < amount; k++) {
    //        value[k] = new T[segments];
    //    }
    //    return value;
    //}

    public override bool PreDraw(ref Color lightColor) {
        //var npc = Main.npc[0];
        //if (_frontCoordinates == null) {
        //    const int RopeSegments = 24;
        //    const int RopesAmount = 2;
        //    _frontCoordinates = InitArr<Vector2>(RopesAmount, RopeSegments);
        //    _frontRotations = InitArr<float>(RopesAmount, RopeSegments);
        //    _backCoordinates = InitArr<Vector2>(RopesAmount, RopeSegments);
        //    _backRotations = InitArr<float>(RopesAmount, RopeSegments);
        //}

        //var ropeCoordinates = npc.Center;
        //var multiplier = new Vector2(1f, Helper.Oscillate(Main.GameUpdateCount / 30f, 0.2f, 0.3f));
        //float rotationMultiplier = Math.Sign(multiplier.Y);
        //for (int rope = 0; rope < _frontCoordinates.Length; rope++) {
        //    float ropeRotation = rope + Helper.Oscillate(Main.GameUpdateCount / (60f - 14f * rope), -0.1f, 0.1f);
        //    float ropeDistance = 1f + rope * 0.15f;
        //    for (int segment = 0; segment < _frontCoordinates[rope].Length; segment++) {
        //        float rotation = segment * MathHelper.Pi / (_frontCoordinates[rope].Length - 1);
        //        var offset = (new Vector2(ropeDistance, 0f).RotatedBy(rotation) * multiplier).RotatedBy(ropeRotation);
        //        _frontCoordinates[rope][segment] = ropeCoordinates + offset * npc.Size - Main.screenPosition;
        //        _frontRotations[rope][segment] = rotation * rotationMultiplier - MathHelper.PiOver2 + ropeRotation;

        //        _backCoordinates[rope][segment] = ropeCoordinates + offset.RotatedBy(MathHelper.Pi) * npc.Size - Main.screenPosition;
        //        _backRotations[rope][segment] = rotation * rotationMultiplier + MathHelper.PiOver2 + ropeRotation;
        //    }
        //}

        ////var ropeColor = DrawHelper.GetLightColor(ropeCoordinates);
        //SkyHunterCrossbow.DrawChain(npc.Center, Projectile.Center, 1f, 0, Projectile.whoAmI);
        //var ropeColor = Color.White;
        //var ropeTexture = AequusTextures.AnchorProjectileVertexRope;
        //for (int i = _backCoordinates.Length - 1; i >= 0; i--) {
        //    AequusDrawing.DrawBasicVertexLine(ropeTexture, _backCoordinates[i], _backRotations[i],
        //        p => Utils.MultiplyRGBA(ropeColor, Color.Lerp(Color.Gray, Color.DarkGray * 0.33f, MathF.Sin(p * MathHelper.Pi))) * Projectile.Opacity,
        //        p => 4f * Projectile.scale
        //    );
        //}
        //for (int i = 0; i < _frontCoordinates.Length; i++) {
        //    MiscWorldInterfaceElements.DrawBasicVertexLine(ropeTexture, _frontCoordinates[i], _frontRotations[i],
        //        p => Utils.MultiplyRGB(ropeColor, Color.Lerp(Color.Gray, Color.White, MathF.Sin(p * MathHelper.Pi))) * Projectile.Opacity,
        //        p => 4f * Projectile.scale
        //    );
        //}
        return true;
    }
}