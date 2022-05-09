using Terraria.ModLoader;

namespace Aequus.Common
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
    public interface IModCallArgSettable
    {
        IModCallArgSettable HandleArg(string name, object value);

        public static IModCallArgSettable HandleArgs(IModCallArgSettable obj, int start, object[] args)
        {
            if (args.Length < start + 2)
            {
                return obj;
            }
            while (true)
            {
                if (args.Length < start + 1)
                {
                    Aequus.Instance.Logger.Error("Invalid arg lengths. Safely leaving handler, final arg: " + (args[^1] == null ? "NULL" : args[^1].ToString()));
                    return obj;
                }

                if (args[start] is string name)
                {
                    obj = obj.HandleArg(name, args[start + 1]);
                }
                else
                {
                    Aequus.Instance.Logger.Error("Invalid arg type '" + args[start].GetType().FullName + "', string expected. Safely leaving handler, arg value: " + (args[start] == null ? "NULL" : args[start].ToString()));
                    return obj;
                }
                start += 2;

                if (args.Length < start + 2)
                {
                    return obj;
                }
            }
        }

        protected static void DoesntExistReport(string name, IModCallArgSettable me)
        {
            Aequus.Instance.Logger.Error(name + " doesn't exist in " + me.GetType().Name);
        }
        protected static void SuccessReport(string name, object value, IModCallArgSettable me)
        {
            if (Aequus.LogMore)
                Aequus.Instance.Logger.Info("Setting " + name + " in " + me.GetType().Name + " to " + (value == null ? "NULL" : value.ToString()));
        }
    }
}