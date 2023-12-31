using Aequus.Common.Particles;
using Aequus.Common.UI;
using Aequus.Content.Graphics;
using Aequus.Content.Tiles.CraftingStations.TrashCompactor;
using Aequus.Core.Graphics.Tiles;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Aequus.Core.Graphics;

public class DrawLayers : ModSystem {
    public sealed class AboveWater : ActionDrawQueue<AboveWater> { }
    public sealed class WorldUI : ActionDrawQueue<WorldUI> { }

    private class WorldUIDrawLayer : UILayer {
        public override string Layer => InterfaceLayers.EntityHealthBars_16;
        public override InterfaceScaleType ScaleType => InterfaceScaleType.Game;

        public override bool Draw(SpriteBatch spriteBatch) {
            WorldUI.DrawAll();
            return true;
        }
    }

    #region Game Clear Ticks 
    internal static readonly List<IActionDrawQueue> _actionDrawQueues = new();

    public override void PreUpdatePlayers() {
        Clear();
    }

    public override void ClearWorld() {
        Clear();
    }

    private static void Clear() {
        foreach (var q in _actionDrawQueues) {
            q.Actions = null;
        }
    }
    #endregion

    #region Loading
    public override void Load() {
        On_Main.DoDraw_WallsTilesNPCs += DrawAboveTiles;
        On_Main.DrawInfernoRings += DrawAboveWater;
    }

    private static void DrawAboveTiles(On_Main.orig_DoDraw_WallsTilesNPCs orig, Main self) {
        orig(self);
        TrashCompactor.ItemSpewingSystem.DrawAll();
    }

    private static void DrawAboveWater(On_Main.orig_DrawInfernoRings orig, Main self) {
        orig(self);
        LegacySpecialTileRenderer.Render(TileRenderLayerID.PostDrawLiquids);
        LegacyParticleSystem.GetLayer(ParticleLayer.AboveLiquid).Draw(Main.spriteBatch);
        if (RadonMossFogRenderer.Instance.IsReady) {
            Main.spriteBatch.End();
            RadonMossFogRenderer.Instance.DrawOntoScreen(Main.spriteBatch);
            Main.spriteBatch.BeginWorld(shader: false);
        }
        AboveWater.DrawAll();
    }
    #endregion
}

internal interface IActionDrawQueue {
    Action Actions { get; set; }
}

public abstract class ActionDrawQueue<TSelfType> : ILoadable, IActionDrawQueue where TSelfType : ActionDrawQueue<TSelfType> {
    private static ActionDrawQueue<TSelfType> _this;
    private static Action _actions;

    public Action Actions { get => _actions; set => _actions = value; }

    public void Load(Mod mod) {
        _this = this;
        DrawLayers._actionDrawQueues.Add(this);
    }

    public void Unload() {
        _this = null;
    }

    public static void Draw(Action action) {
        _actions += action;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRect, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effect, float inactiveLayerDepth = 0f) {
        Draw(() => Main.spriteBatch.Draw(texture, position, sourceRect, color, rotation, origin, scale, effect, inactiveLayerDepth));
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRect, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effect, float inactiveLayerDepth = 0f) {
        Draw(() => Main.spriteBatch.Draw(texture, position, sourceRect, color, rotation, origin, scale, effect, inactiveLayerDepth));
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DrawColorCodedString(DynamicSpriteFont font, string text, Vector2 position, Color baseColor, float rotation, Vector2 origin, Vector2 baseScale, float maxWidth = -1f, bool ignoreColors = false) {
        Draw(() => ChatManager.DrawColorCodedString(Main.spriteBatch, font, text, position, baseColor, rotation, origin, baseScale, maxWidth, ignoreColors));
    }

    internal static void DrawAll() {
        _actions?.Invoke();
        _actions = null;
    }
}