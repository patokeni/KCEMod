using Il2CppInterop.Runtime.Injection;
using KCEMod.Game;
using MelonLoader;
using System.Collections;
using UnityEngine;

[assembly: MelonInfo(typeof(KCEMod.Core), "KCEMod", "1.0.0", "xianxian", null)]
[assembly: MelonGame("KAMITSUBAKI STUDIO", "KAMITSUBAKI CITY ENSEMBLE")]

namespace KCEMod
{
    public class Core : MelonMod
    {
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized.");
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (sceneName.Equals("NewIdea2"))
            {
                MelonCoroutines.Start(WaitForGame());
            }
        }

        IEnumerator WaitForGame()
        {
            GameObject player;
            while (true)
            {
                player = GameObject.Find("Player");

                if (player == null)
                {
                    yield return new WaitForSeconds(1F);
                } else
                {
                    player.TryGetComponent<InputHandler>(out InputHandler handler);
                    if (handler == null)
                    {
                        player.AddComponent<InputHandler>();
                        LoggerInstance.Msg("We are playing");
                    }

                    break;
                }

            }
        }
    }
}