using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

public class GameManager : MonoBehaviour
{
    private static bool firstFrame = false;

    private static PlayerLoopSystem originalPlayerLoop;

    private void Awake()
    {
        MakePlayerLoop();
    }

    private void MakePlayerLoop()
    {
        originalPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
        var loop = PlayerLoop.GetDefaultPlayerLoop();

        for (int i = 0; i < loop.subSystemList.Length; i++)
        {
            if (loop.subSystemList[i].type == typeof(Update))
            {
                var updateList = new List<PlayerLoopSystem>(loop.subSystemList[i].subSystemList);

                var customSystem = new PlayerLoopSystem
                {
                    type = typeof(CustomGameLogic),
                    updateDelegate = CustomUpdate
                };

                updateList.Insert(0, customSystem);
                loop.subSystemList[i].subSystemList = updateList.ToArray();
                break;
            }
        }

        PlayerLoop.SetPlayerLoop(loop);
    }

    private static void CustomUpdate()
    {
        //Skips the first frame because it freezes
        if (!firstFrame)
        {
            firstFrame = true;
            return;
        }
        
        PaddlePhysics.Frame();
        
        var balls = new List<BallController>(BallManager.GetBalls());
        foreach (var ball in balls)
        {
            ball.Frame();
        }
    }

    private struct CustomGameLogic {}

#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoadMethod]
    private static void ResetPlayerLoopInEditor()
    {
        UnityEditor.EditorApplication.playModeStateChanged += state =>
        {
            if (state == UnityEditor.PlayModeStateChange.ExitingPlayMode)
            {
                PlayerLoop.SetPlayerLoop(originalPlayerLoop);
            }
        };
    }
#endif
}