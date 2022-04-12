using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Common
{
    public sealed class GameCamera : ModSystem
    {
        public string FocusKey { get; private set; }

        private Vector2? target;
        private Vector2? cameraPosition;
        private CameraPriority priority;
        private float speed;
        private int hold;
        private bool returning;

        private Vector2 ScreenTarget => target.Value - new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f);

        public override void Load()
        {
            Initialize();
        }

        public override void OnWorldLoad()
        {
            if (Main.netMode != NetmodeID.Server)
                Initialize();
        }

        internal void Initialize()
        {
            target = null;
            cameraPosition = null;
            priority = CameraPriority.None;
            speed = 10f;
            hold = 0;
            returning = false;
        }

        public bool SetTarget(string key, Vector2 target, CameraPriority priority, float speed = 128f, int hold = 25)
        {
            if (key == FocusKey)
            {
                this.hold = hold;
            }
            if (this.target == null || this.priority < priority)
            {
                this.target = target;
                this.priority = priority;
                this.speed = speed;
                returning = false;
                this.hold = hold;
                FocusKey = key;
                return true;
            }
            return false;
        }

        private void CheckSpeed()
        {
            speed = Math.Max(speed, 128f);
        }
        internal void UpdateScreen()
        {
            if (target != null && !returning)
            {
                if (cameraPosition == null)
                {
                    cameraPosition = Main.screenPosition;
                }
                CheckSpeed();
                var difference = ScreenTarget - cameraPosition.Value;
                float l = difference.Length();
                if (l <= speed)
                {
                    cameraPosition = ScreenTarget;
                    returning = true;
                }
                else
                {
                    cameraPosition += Vector2.Normalize(difference) * speed;
                }
                Main.screenPosition = cameraPosition.Value;
            }
            else if (cameraPosition != null)
            {
                if (hold > 0)
                {
                    Main.screenPosition = cameraPosition.Value;
                    hold--;
                    return;
                }
                target = null;
                returning = false;
                CheckSpeed();
                var difference = Main.screenPosition - cameraPosition.Value;
                float l = difference.Length();
                if (l <= speed)
                {
                    cameraPosition = null;
                    FocusKey = null;
                    return;
                }
                else
                {
                    cameraPosition += Vector2.Normalize(difference) * speed;
                }
                Main.screenPosition = cameraPosition.Value;
            }
        }

        internal static Vector2 GetY_Check(Vector2 position)
        {
            return Main.player[Main.myPlayer].gravDir == -1 ? GetY(position) : position;
        }
        internal static Vector2[] GetY_Check(Vector2[] arr)
        {
            if (Main.player[Main.myPlayer].gravDir == -1)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    arr[i] = GetY(arr[i]);
                }
            }
            return arr;
        }
        internal static Vector2[] GetY_Check_WorldPos(Vector2[] arr)
        {
            if (Main.player[Main.myPlayer].gravDir == -1)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    arr[i] = GetY(arr[i] - Main.screenPosition) + Main.screenPosition;
                }
            }
            return arr;
        }
        internal static Vector2 GetY(Vector2 position)
        {
            return new Vector2(position.X, Main.screenHeight - position.Y);
        }
    }
}