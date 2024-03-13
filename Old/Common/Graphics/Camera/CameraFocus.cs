using System;

namespace Aequus.Old.Common.Graphics.Camera;

/// <summary>
/// Manages screen panning
/// </summary>
public class CameraFocus : ModSystem {
    /// <summary>
    /// A context string for the current focus
    /// </summary>
    public string Context { get; private set; }

    /// <summary>
    /// The target position for the camera in the world
    /// </summary>
    public Vector2? target;
    /// <summary>
    /// The camera's position 
    /// </summary>
    public Vector2? cameraPosition;
    /// <summary>
    /// A priority for determining whether or not a new camera focus can be added
    /// </summary>
    public CameraPriority priority;
    /// <summary>
    /// The speed of the camera
    /// </summary>
    public float speed;
    /// <summary>
    /// The progress of the camera from its original position to its destination
    /// </summary>
    public float cameraProgress;
    /// <summary>
    /// How long the camera holds in its target position after the caller stops running <see cref="SetTarget(string, Vector2, CameraPriority, float, int)"/>
    /// </summary>
    public int hold;
    /// <summary>
    /// Whether or not the camera is returning
    /// </summary>
    public bool returning;
    public bool makePlayerInvincible;

    private Vector2 ScreenTarget => target.Value - new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f);

    public override void Load() {
        Initialize();
    }

    public override void OnWorldLoad() {
        if (Main.netMode != NetmodeID.Server)
            Initialize();
    }

    internal void Initialize() {
        target = null;
        cameraPosition = null;
        priority = CameraPriority.None;
        speed = 10f;
        hold = 0;
        returning = false;
    }

    /// <summary>
    /// Sets the camera target to a position, may return false if your priority is below a current target's priority
    /// </summary>
    /// <param name="key">Context key, only really useful for 3rd party mods</param>
    /// <param name="target">A world coordinate to point the camera at</param>
    /// <param name="priority">The priority of this camera target. If there is a current camera target, and its priority is above, it will prevent you from setting a target and this method will return false</param>
    /// <param name="speed">The speed of the camera, the exact time it will get there is: <code>speed / 50</code>So 6 speed means it will get into position in 8 frames. Clamped between 6 and 25</param>
    /// <param name="hold">How long the camera holds in its target position after the caller stops running <see cref="SetTarget(string, Vector2, CameraPriority, float, int)"/></param>
    /// <returns>Whether or not the camera target has been set to a new position. Spamming SetTarget will only return true on the first SetTarget call, and the rest will return false</returns>
    public bool SetTarget(string key, Vector2 target, CameraPriority priority, float speed = 6f, int hold = 25) {
        if (this.target == null || this.priority < priority || key == Context) {
            this.target = target;
            this.priority = priority;
            if (priority >= CameraPriority.MinibossDefeat) {
                makePlayerInvincible = true;
            }
            this.speed = speed;
            returning = false;
            this.hold = hold;
            Context = key;
            return true;
        }
        return false;
    }

    public void UpdateScreen(Player player) {
        if (makePlayerInvincible && player.TryGetModPlayer(out CameraFocusPlayer cameraFocusPlayer)) {
            cameraFocusPlayer.sceneInvulnerability = Math.Max(cameraFocusPlayer.sceneInvulnerability, 30);
        }
        if (target != null && !returning) {
            if (cameraPosition == null) {
                cameraPosition = Main.screenPosition;
            }
            ClampSpeed();
            var difference = ScreenTarget - Main.screenPosition;
            cameraProgress += speed / 50f;
            cameraPosition = Main.screenPosition + difference * cameraProgress;
            if (cameraProgress >= 1f) {
                cameraProgress = 1f;
                cameraPosition = ScreenTarget;
                returning = true;
            }
            Main.screenPosition = cameraPosition.Value;
        }
        else if (cameraPosition != null) {
            if (hold > 0) {
                Main.screenPosition = cameraPosition.Value;
                hold--;
                return;
            }
            ClampSpeed();
            var difference = ScreenTarget - Main.screenPosition;
            cameraProgress -= speed / 200f + cameraProgress * 0.1f;
            if (cameraProgress <= 0f) {
                returning = false;
                target = null;
                cameraProgress = 0f;
                cameraPosition = null;
                Context = null;
                makePlayerInvincible = false;
                return;
            }
            cameraPosition = Main.screenPosition + difference * cameraProgress;
            Main.screenPosition = cameraPosition.Value;
        }
    }
    private void ClampSpeed() {
        speed = MathHelper.Clamp(speed, 0.5f, 25f);
    }

    internal static Vector2 GetY_Check(Vector2 position) {
        return Main.player[Main.myPlayer].gravDir == -1 ? GetY(position) : position;
    }
    internal static Vector2[] GetY_Check(Vector2[] arr) {
        if (Main.player[Main.myPlayer].gravDir == -1) {
            for (int i = 0; i < arr.Length; i++) {
                arr[i] = GetY(arr[i]);
            }
        }
        return arr;
    }
    internal static Vector2[] GetY_Check_WorldPos(Vector2[] arr) {
        if (Main.player[Main.myPlayer].gravDir == -1) {
            for (int i = 0; i < arr.Length; i++) {
                arr[i] = GetY(arr[i] - Main.screenPosition) + Main.screenPosition;
            }
        }
        return arr;
    }
    internal static Vector2 GetY(Vector2 position) {
        return new Vector2(position.X, Main.screenHeight - position.Y);
    }
}

public class CameraFocusPlayer : ModPlayer {
    public int sceneInvulnerability;

    public override void ResetEffects() {
        if (sceneInvulnerability > 0) {
            sceneInvulnerability--;
        }
    }
}

public enum CameraPriority {
    None,
    Weak,
    MinibossDefeat,
    BossDefeat,
    VeryImportant,
}