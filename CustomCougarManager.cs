using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImprovedCougar
{
    [RegisterTypeInIl2Cpp]
    internal class CustomCougarManager : MonoBehaviour
    {

        public void Update()
        {

            if(GameManager.m_IsPaused || GameManager.s_IsGameplaySuspended) return;
            if (SaveGameSystem.IsRestoreInProgress()) return;

            PlayerStruggle struggle = GameManager.m_PlayerStruggle;
            if (struggle != null && struggle.InStruggle()) return;

            //eventually handle spawning and spawn point moving, wander paths, all that stuff

        }

    }
}
