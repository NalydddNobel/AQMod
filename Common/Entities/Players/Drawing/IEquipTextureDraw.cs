using Terraria.DataStructures;

namespace Aequus.Common.Entities.Players.Drawing;

internal interface IEquipTextureDraw {
    void Draw(ref PlayerDrawSet drawInfo, Vector2 position, Rectangle frame, Color color, float rotation, Vector2 origin, SpriteEffects effects, int shader);
}
