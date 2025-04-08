using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

public class GameManager : MonoBehaviour
{
    public Transform ball;
    public Transform paddle;

    private void Awake()
    {
        BallPhysics.Inititate(ball);
        PaddlePhysics.Initiate(paddle);
        MakePlayerLoop();
    }

    private void MakePlayerLoop()
    {
        var loop = PlayerLoop.GetDefaultPlayerLoop();

        for (int i = 0; i < loop.subSystemList.Length; i++)
        {
            if (loop.subSystemList[i].type == typeof(Update))
            {
                var updateList = new System.Collections.Generic.List<PlayerLoopSystem>(loop.subSystemList[i].subSystemList);

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
    
    static void CustomUpdate()
    {
        PaddlePhysics.Frame();
        BallPhysics.Frame();
    }

    struct CustomGameLogic {}

}
