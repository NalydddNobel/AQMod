using Aequus.Common;
using System;
using Terraria.ModLoader;

namespace Aequus.Content.CrossMod
{
    public class EmoteBubbleLibSupport : IPostSetupContent
    {
        private ModData EmoteBubbleLib;

        void ILoadable.Load(Mod mod)
        {
        }

        void IPostSetupContent.PostSetupContent(Aequus aequus)
        {
            EmoteBubbleLib = new ModData("EmoteBubbleLib");
            if (EmoteBubbleLib.Enabled)
            {
                Call("Crabson", "crabson", "Crabson", 7, () => AequusWorld.downedCrabson);
                Call("OmegaStarite", "omegastarite", "OmegaStarite", 7, () => AequusWorld.downedOmegaStarite);
                Call("RedSprite", "redsprite", "RedSprite", 7, () => AequusWorld.downedRedSprite);
                Call("SpaceSquid", "spacesquid", "SpaceSquid", 7, () => AequusWorld.downedSpaceSquid);
            }
        }

        void ILoadable.Unload()
        {
        }

        private void Call(string internalName, string command, string texture)
        {
            EmoteBubbleLib.Call("AddEmote", ModContent.GetInstance<Aequus>(), internalName, command, "Aequus/Assets/UI/Emotes/" + texture);
        }
        private void Call(string internalName, string command, string texture, byte listing)
        {
            EmoteBubbleLib.Call("AddEmote", ModContent.GetInstance<Aequus>(), internalName, command, "Aequus/Assets/UI/Emotes/" + texture, listing);
        }
        private void Call(string internalName, string command, string texture, byte listing, Func<bool> canNPCChat)
        {
            EmoteBubbleLib.Call("AddEmote", ModContent.GetInstance<Aequus>(), internalName, command, "Aequus/Assets/UI/Emotes/" + texture, listing, canNPCChat);
        }
    }
}