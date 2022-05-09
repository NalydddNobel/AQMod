using Terraria.ModLoader;

namespace Aequus.Content.CrossMod
{
    public interface IModCallable : ILoadable
    {
        public const string Success = "Success";
        public const string Failure = "Failure";

        object HandleModCall(Aequus aequus, object[] args);

        /// <summary>
        /// Attempts to unbox an <see cref="object"/> into an <see cref="int"/>
        /// <para>Accepts <see cref="int"/>, <see cref="uint"/>, <see cref="byte"/>, <see cref="sbyte"/>,
        /// <see cref="ushort"/>, <see cref="short"/>, <see cref="float"/>, and <see cref="double"/>.</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>-1 by default. An <see cref="int"/> representation of the object otherwise</returns>
        public static int UnboxIntoInt(object obj)
        {
            if (obj is int)
            {
                return (int)obj;
            }
            if (obj is uint)
            {
                return (int)(uint)obj;
            }
            if (obj is byte)
            {
                return (byte)obj;
            }
            if (obj is sbyte)
            {
                return (sbyte)obj;
            }
            if (obj is ushort)
            {
                return (ushort)obj;
            }
            if (obj is short)
            {
                return (short)obj;
            }
            if (obj is float)
            {
                return (int)(float)obj;
            }
            if (obj is double)
            {
                return (int)(double)obj;
            }
            return -1;
        }

        /// <summary>
        /// Attempts to unbox an <see cref="object"/> into an <see cref="int"/>
        /// <para>Accepts <see cref="float"/>, <see cref="double"/>, <see cref="int"/>, <see cref="uint"/>, <see cref="byte"/>, <see cref="sbyte"/>,
        /// <see cref="ushort"/>, and <see cref="short"/></para>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>-1 by default. An <see cref="int"/> representation of the object otherwise</returns>
        public static float UnboxIntoFloat(object obj)
        {
            if (obj is float)
            {
                return (int)(float)obj;
            }
            if (obj is double)
            {
                return (int)(double)obj;
            }
            if (obj is int)
            {
                return (int)obj;
            }
            if (obj is uint)
            {
                return (uint)obj;
            }
            if (obj is byte)
            {
                return (byte)obj;
            }
            if (obj is sbyte)
            {
                return (sbyte)obj;
            }
            if (obj is ushort)
            {
                return (ushort)obj;
            }
            if (obj is short)
            {
                return (short)obj;
            }
            return -1;
        }
    }
}