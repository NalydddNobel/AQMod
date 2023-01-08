﻿using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Carpentery.Paint
{
    public class OmniPaintPlayer : ModPlayer
    {
        public byte selectedPaint;

        public override void Initialize()
        {
            selectedPaint = PaintID.RedPaint;
        }
    }
}