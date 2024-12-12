using ComplexLogger;

namespace ImprovedCougar
{
    public class Main : MelonMod
    {
        internal static ComplexLogger<Main> Logger = new();

        public override void OnInitializeMelon()
        {
            Logger.Log("Improved Cougar is online", FlaggedLoggingLevel.Always);
        }
    }
}
