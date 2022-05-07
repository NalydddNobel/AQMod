namespace Aequus.Common
{
    public interface IModCallable
    {
        public const string Success = "Success";
        public const string Failure = "Failure";

        object HandleModCall(Aequus aequus, object[] args);
    }
}