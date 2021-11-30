namespace AQMod.Common.Graphics.DrawTypes
{
    public interface IDrawObject : IDrawType
    {
        /// <summary>
        /// Return false to kill this draw object instance
        /// </summary>
        /// <returns></returns>
        bool Update();
    }
}