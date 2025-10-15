using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    public event Action<LevelState> OnStateChanged;

    public enum LevelState
    {
        PlayerTurn,
        AITurn
    }

    private LevelState _currentState = LevelState.PlayerTurn;
    public LevelState CurrentState => _currentState;

    public void ChangeState(LevelState newState)
    {
        if (_currentState == newState)
            return;

        _currentState = newState;

        OnStateChanged?.Invoke(_currentState);

        //HandleStateEnter(_currentState);
    }

    private void HandleStateEnter(LevelState state)
    {
        throw new NotImplementedException();
    }
}
