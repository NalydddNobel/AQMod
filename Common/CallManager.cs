using AQMod.Common.WorldEvents;

namespace AQMod.Common.DeveloperTools
{
    public class CallManager
    {
        public static object Call(params object[] args)
        {
            if (args.Length == 0 || !(args[0] is string callType))
                return null;
            switch (callType)
            {
                case "GlimmerEvent_GlimmerChance":
                if (args.Length > 1)
                {
                    try
                    {
                        GlimmerEvent.GlimmerChance = (int)args[1];
                    }
                    catch
                    {
                    }
                }
                return GlimmerEvent.GlimmerChance;

                case "GlimmerEvent_X":
                if (args.Length > 1)
                {
                    try
                    {
                        GlimmerEvent.X = (ushort)args[1];
                    }
                    catch
                    {
                    }
                }
                return GlimmerEvent.X;

                case "GlimmerEvent_Y":
                if (args.Length > 1)
                {
                    try
                    {
                        GlimmerEvent.Y = (ushort)args[1];
                    }
                    catch
                    {
                    }
                }
                return GlimmerEvent.Y;

                case "GlimmerEvent_FakeActive":
                return GlimmerEvent.FakeActive;

                case "GlimmerEvent_ActuallyActive":
                return GlimmerEvent.ActuallyActive;

                case "GlimmerEvent_Active":
                {
                    if (args.Length > 1 && args[1] is bool flag)
                    {
                        if (flag)
                        {
                            if (args.Length > 2 && args[2] is bool flag2)
                            {
                                GlimmerEvent.Activate(genuine: flag2);
                            }
                            else
                            {
                                GlimmerEvent.Activate(genuine: true);
                            }
                        }
                        else
                        {
                            GlimmerEvent.Deactivate();
                        }
                    }
                    return GlimmerEvent.IsActive;
                }
            }
            return null;
        }
    }
}