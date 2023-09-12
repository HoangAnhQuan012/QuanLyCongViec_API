using quanLyCongViec.Debugging;

namespace quanLyCongViec
{
    public class quanLyCongViecConsts
    {
        public const string LocalizationSourceName = "quanLyCongViec";

        public const string ConnectionStringName = "Default";

        public const bool MultiTenancyEnabled = true;


        /// <summary>
        /// Default pass phrase for SimpleStringCipher decrypt/encrypt operations
        /// </summary>
        public static readonly string DefaultPassPhrase =
            DebugHelper.IsDebug ? "gsKxGZ012HLL3MI5" : "86d07a2c051b4eaf9c8fda8a032034b3";
    }
}
