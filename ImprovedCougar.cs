using ComplexLogger;
using Il2Cpp;

namespace ImprovedCougar
{
    public class Main : MelonMod
    {
        internal static ComplexLogger<Main> Logger = new();

        public override void OnInitializeMelon()
        {
            Logger.Log("Improved Cougar is online", FlaggedLoggingLevel.Always);
            Settings.CustomSettings.OnLoad();
        }

        protected bool Initialize()
        {
            //return EAFManager.Instance.RegisterSpawnableAi(typeof(TrackingWolf), TrackingWolf.Settings);
            return false;
        }

        public override void OnUpdate()
        {
            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.UpArrow))
            {
                GameManager.GetCougarManager().enabled = true;
            }
        }

    }
}
