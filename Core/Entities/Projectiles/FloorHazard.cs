using System;
using System.Collections.Generic;

namespace Aequu2.Core.Entities;

// Heavily inspired by instrex's floor hazards system!

/// <summary>An abstract projectile which represents a floor hazard.</summary>
public abstract class FloorHazard(FloorHazard.Info info) : ModProjectile {
    private List<Point> _hazardPoints;
    private bool[] _hazardLookup;

    public List<Point> HazardPoints => _hazardPoints;

    protected readonly Info _info = info;

    public int X => (int)Projectile.position.X / 16;
    public int Y => (int)Projectile.position.Y / 16;
    public int TileWidth => Projectile.width / 16;
    public int TileHeight => Projectile.width / 16;

    public readonly record struct Info(bool HurtsPlayers, bool HurtsNPCs, bool HurtsProjectiles, int StartWidth = 2, int StartHeight = 2);

    public override void SetDefaults() {
        //Projectile.friendly = _info.HurtsNPCs;
        //Projectile.hostile = _info.HurtsPlayers;

        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;

        _hazardPoints = [];
        SetSize(_info.StartWidth, _info.StartHeight);
    }

    public override void AI() {
        if (_info.HurtsProjectiles) {
            FloorHazardGlobalProjectile._nextHazards.Add(this);
        }

        // Stick to 16x16 grid.
        Projectile.position.X -= Projectile.position.X % 16f;
        Projectile.position.Y -= Projectile.position.Y % 16f;

        Rectangle hitbox = Projectile.getRect();

        if (_info.HurtsPlayers) {
            for (int i = 0; i < Main.maxPlayers; i++) {
                Player player = Main.player[i];
                Rectangle playerHitbox = player.getRect();

                if (player.active && player.hurtCooldowns[ImmunityCooldownID.TileContactDamage] <= 0 && Projectile.Colliding(hitbox, playerHitbox)) {
                    OnHazardCollideWithPlayer(player);
                }
            }
        }

        if (_info.HurtsNPCs) {
            for (int i = 0; i < Main.maxNPCs; i++) {
                NPC npc = Main.npc[i];
                Rectangle npcHitbox = npc.getRect();

                if (npc.active && !npc.dontTakeDamage && Projectile.Colliding(hitbox, npcHitbox)) {
                    OnHazardCollideWithNPC(npc);
                }
            }
        }

        base.AI();
    }

    public virtual void OnHazardCollideWithPlayer(Player target) { }

    public virtual void OnHazardCollideWithNPC(NPC target) { }

    public virtual void OnHazardCollideWithProjectile(Projectile other) { }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
        return IsHitboxTouchingHazard(targetHitbox);
    }

    public bool IsHitboxTouchingHazard(in Rectangle targetHitbox) {
        if (!Projectile.Hitbox.Contains(targetHitbox)) {
            return false;
        }

        int bottomY = (targetHitbox.Y + targetHitbox.Height + 4) / 16;
        int x = (targetHitbox.X + targetHitbox.Width / 2) / 16;
        int lookup = GetLookupIndex(x, bottomY);

        return LookupInBounds(lookup) ? _hazardLookup[lookup] : false;
    }

    public bool IsEntityTouchingHazard(Entity entity) {
        return IsHitboxTouchingHazard(entity.Hitbox);
    }

    public virtual bool IsValidSpotForHazard(int i, int j) {
        return Framing.GetTileSafely(i, j).IsSolid() && !Framing.GetTileSafely(i, j - 1).IsFullySolid();
    }

    public void SetSize(int width, int height) {
        Vector2 center = Projectile.Center;
        Projectile.width = width * 16;
        Projectile.height = height * 16;
        Projectile.Center = center;

        _hazardLookup = new bool[TileWidth * TileHeight];
        foreach (Point p in _hazardPoints) {
            int lookup = GetLookupIndex(p.X, p.Y);

            if (LookupInBounds(lookup)) {
                _hazardLookup[lookup] = true;
            }
        }
    }

    public void Resize(int expandWidth, int expandHeight) {
        SetSize(TileWidth + expandWidth, TileHeight + expandHeight);
    }

    private void AddPointInner(int i, int j, int lookup) {
        _hazardLookup[lookup] = true;
        _hazardPoints.Add(new Point(i, j));
    }

    public bool TryAddPoint(int i, int j) {
        int lookup = GetLookupIndex(i, j);
        if (!LookupInBounds(lookup)) {
            return false;
        }

        AddPointInner(i, j, lookup);

        return true;
    }

    public void AddPoint(int i, int j) {
        if (!TryAddPoint(i, j)) {
            throw new IndexOutOfRangeException("Attempted to add an X and Y coordinate out of bounds of a floor hazard.");
        }
    }

    public bool TryRemovePoint(int i, int j) {
        int lookup = GetLookupIndex(i, j);
        if (!LookupInBounds(lookup)) {
            return false;
        }

        _hazardLookup[lookup] = false;
        _hazardPoints.Remove(new Point(i, j));

        return true;
    }

    public void RemovePoint(int i, int j) {
        if (!TryRemovePoint(i, j)) {
            throw new IndexOutOfRangeException("Attempted to add an X and Y coordinate out of bounds of a floor hazard.");
        }
    }

    public int GetLookupIndex(int i, int j) {
        i -= X;
        j -= Y;

        return j * TileWidth + i;
    }

    public bool LookupInBounds(int index) {
        return _hazardLookup.IndexInRange(index);
    }

    public bool Contains(int i, int j) {
        int lookup = GetLookupIndex(i, j);
        if (!LookupInBounds(lookup)) {
            return false;
        }

        return _hazardLookup[lookup];
    }

    public override bool ShouldUpdatePosition() {
        return false;
    }

    public override void OnKill(int timeLeft) {
        FloorHazardGlobalProjectile._nextHazards.Remove(this);
        FloorHazardGlobalProjectile.ActiveHazards.Remove(this);
    }

    #region Common Behaviors
    protected void MarkAllAvailable() {
        for (int i = X; i < X + TileWidth; i++) {
            for (int j = Y; j < Y + TileHeight; j++) {
                int lookup = GetLookupIndex(i, j);
                if (!LookupInBounds(lookup)) {
                    continue;
                }

                if (IsValidSpotForHazard(i, j)) {
                    AddPointInner(i, j, lookup);
                }
            }
        }
    }

    protected void Spread(Action<int, int> OnSpreadCallback = null) {
        List<Point> newPoints = new(_hazardPoints.Count);

        int expandX = 0;
        int expandY = 0;

        foreach (Point p in _hazardPoints) {
            for (int x = p.X - 1; x <= p.X + 1; x++) {
                for (int y = p.Y - 1; y <= p.Y + 1; y++) {
                    Point next = new Point(x, y);
                    if (!_hazardPoints.Contains(next) && IsValidSpotForHazard(x, y)) {
                        newPoints.Add(next);

                        OnSpreadCallback?.Invoke(x, y);

                        if (x < X) {
                            expandX = Math.Max(expandX, X - x);
                        }
                        else if (X + TileWidth >= x) {
                            expandX = Math.Max(expandX, x - (X + TileWidth) + 1);
                        }

                        if (y < Y) {
                            expandY = Math.Max(expandY, Y - y);
                        }
                        else if (Y + TileHeight >= y) {
                            expandY = Math.Max(expandY, y - (Y + TileHeight) + 1);
                        }
                    }
                }
            }
        }

        Resize(expandX * 2, expandY * 2);

        foreach (Point p in newPoints) {
            TryAddPoint(p.X, p.Y);
        }
    }
    #endregion
}

public class FloorHazardGlobalProjectile : GlobalProjectile {
    internal static List<FloorHazard> _nextHazards = [];
    public static List<FloorHazard> ActiveHazards = [];

    internal static void SwapHazardList() {
        (ActiveHazards, _nextHazards) = (_nextHazards, ActiveHazards);
        _nextHazards.Clear();
    }

    public override void AI(Projectile projectile) {
        for (int i = 0; i < ActiveHazards.Count; i++) {
            FloorHazard hazard = ActiveHazards[i];
            Rectangle hitbox = hazard.Projectile.Hitbox;
            Rectangle otherHitbox = projectile.Hitbox;

            if (hazard.Projectile.Colliding(hitbox, otherHitbox) && projectile.Colliding(otherHitbox, hitbox)) {
                hazard.OnHazardCollideWithProjectile(projectile);
            }
        }
    }
}

public class FloorHazardSystem : ModSystem {
    public override void PreUpdateEntities() {
        FloorHazardGlobalProjectile.SwapHazardList();
    }
}