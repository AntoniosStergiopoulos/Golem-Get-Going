using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public enum gameStates {Idle, Damaged, ReadyingJump, Jumping, Attacking, Dashing }
    private gameStates currentState;

    private void Awake()
    {
        instance = this;
        currentState = gameStates.Idle;
    }

    public gameStates GetGameState()
    {
        return currentState;
    }

    public void SetGameState(gameStates newState)
    {
        currentState = newState;
    }
}
