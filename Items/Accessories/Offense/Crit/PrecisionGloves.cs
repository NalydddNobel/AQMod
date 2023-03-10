using Aequus.Content.Town.PhysicistNPC.Analysis;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Offense.Crit
{
    [AutoloadGlowMask()]
    public class PrecisionGloves : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            AnalysisSystem.IgnoreItem.Add(Type);
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 5);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().accPreciseCrits += 1f;
        }
    }
}

namespace Aequus.NPCs
{
    //public struct ScalingRectangle
    //{
    //    public float X;
    //    public float Y;
    //    public int Width;
    //    public int Height;

    //    public ScalingRectangle(float x, float y, int width, int height)
    //    {
    //        X = x; 
    //        Y = y; 
    //        Width = width; 
    //        Height = height;
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public Rectangle GetRectangle(int width, int height)
    //    {
    //        return new Rectangle((int)(X * width) - Width / 2, (int)(Y * height) - Height / 2, Width, Height);
    //    }
    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public Rectangle GetRectangle(Point size)
    //    {
    //        return GetRectangle(size.X, size.Y);
    //    }
    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public Rectangle GetRectangle(Vector2 size)
    //    {
    //        return GetRectangle((int)size.X, (int)size.Y);
    //    }
    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public Rectangle GetRectangle(Rectangle scaler)
    //    {
    //        return GetRectangle(scaler.Width, scaler.Height);
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public bool Contains(int width, int height, Point value)
    //    {
    //        return GetRectangle(width, height).Contains(value);
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public bool Intersects(int myWidth, int myHeight, Rectangle value)
    //    {
    //        return GetRectangle(myWidth, myHeight).Intersects(value);
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public bool Intersects(int myWidth, int myHeight, ScalingRectangle value, int width, int height)
    //    {
    //        return Intersects(myWidth, myHeight, value.GetRectangle(width, height));
    //    }
    //}

    //public struct EnemySweetSpot
    //{
    //    public const float AnimationTime = 12;
    //    public const int TargetSize = 32;

    //    public ScalingRectangle[] targets;
    //    public float timeLeft;
    //    public float animation;

    //    public bool NoTargets => targets == null || targets.Length == 0;

    //    public void Update(NPC npc)
    //    {
    //        if (NoTargets)
    //        {
    //            timeLeft = 0f;
    //            animation = 0f;
    //            return;
    //        }
    //        if (timeLeft > 0f)
    //        {
    //            timeLeft--;
    //            if (timeLeft <= AnimationTime)
    //            {
    //                animation -= 1f / AnimationTime;
    //            }
    //            if (timeLeft < 0f)
    //            {
    //                timeLeft = 0f;
    //            }
    //        }
    //    }

    //    public void Initialize(NPC npc, float rectangleSize = 1f)
    //    {
    //        targets ??= new ScalingRectangle[0];
    //        int targetSize = (int)(rectangleSize * TargetSize);
    //        int amt = Math.Max(targets.Length, 3);
    //        var hitbox = new Rectangle(0, 0, npc.width, npc.height);
    //        for (int i = 0; i < amt; i++)
    //        {
    //            int length = Math.Min(i + 1, targets.Length + 1);
    //            for (int j = 0; j < 10; j++)
    //            {
    //                var p = Helper.ClosestDistance(hitbox, Main.rand.NextVector2Unit() * 1000f);
    //                var r = new ScalingRectangle(p.X / (float)hitbox.Width, p.Y / (float)hitbox.Height, targetSize, targetSize);
    //                if (r.X > r.Y)
    //                {
    //                    r.X = r.X > 1f ? 1f : 0f;
    //                }
    //                else
    //                {
    //                    r.Y = r.Y > 1f ? 1f : 0f;
    //                }

    //                if (length > 2)
    //                {
    //                    for (int k = 0; k < targets.Length; k++)
    //                    {
    //                        if (targets[k].Intersects(npc.width, npc.height, r, npc.width, npc.height))
    //                        {
    //                            goto Continue;
    //                        }
    //                    }
    //                }

    //                if (targets.Length < length)
    //                {
    //                    Array.Resize(ref targets, length + 1);
    //                }
    //                targets[^1] = r;
    //                break;

    //            Continue:
    //                continue;
    //            }
    //        }
    //    }

    //    public bool CheckSweetSpotHit(NPC npc, Vector2 hitLocation)
    //    {
    //        if (NoTargets)
    //            return false;

    //        var edgePoint = Helper.ClosestDistance(npc.Hitbox, hitLocation).ToPoint();
    //        //Helper.DebugDust(hitLocation, DustID.CursedTorch);
    //        //Helper.DebugDust(edgePoint.ToVector2());
    //        edgePoint.X -= (int)npc.position.X;
    //        edgePoint.Y -= (int)npc.position.Y;
    //        foreach (var r in targets)
    //        {
    //            if (r.GetRectangle(npc.width, npc.height).Contains(edgePoint))
    //                return true;
    //        }
    //        return false;
    //    }

    //    public void DrawSweetSpots(NPC npc, SpriteBatch spriteBatch)
    //    {
    //        if (NoTargets)
    //        {
    //            return;
    //        }
    //        var bloom = AequusTextures.Bloom0.Value;
    //        var bloomOrigin = bloom.Size() / 2f;
    //        foreach (var r in targets)
    //        {
    //            var rectangle = r.GetRectangle(npc.Size.ToPoint());
    //            var drawPosition = rectangle.Center() - Main.screenPosition + npc.position;
    //            spriteBatch.Draw(
    //                AequusTextures.Bloom0, 
    //                drawPosition, 
    //                null, 
    //                Color.Red,
    //                0f,
    //                bloomOrigin,
    //                0.5f, SpriteEffects.None, 0f);
    //            Helper.DrawRectangle(rectangle, npc.position - Main.screenPosition, Color.Red * 0.2f);
    //        }
    //    }
    //}

    public partial class AequusNPC
    {
        // Sweet spots are handled by each client separately,
        // meaning each player has different sweet spots even if they're all using the accessory
        // This is possible since damage hits are calculated by the client, and I dont want to messy up a boss for every player because of one's acc choice
        //public EnemySweetSpot sweetSpot;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetDefaults_SweetSpots(NPC npc)
        {
        }
    }
}

namespace Aequus
{
    public partial class AequusPlayer
    {
        public float accPreciseCrits;
    }
}