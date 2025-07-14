using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SystemObject<GameManager> {
    public enum GameState {
        Title,
        InGame,
        Ending
    }

    public GameState CurrentState { get; private set; }

    public override void Initialize() {
        CurrentState = GameState.Title;
        Debug.Log("GameManager initialized. Current state: Title");
    }

    public void ChangeState(GameState newState) {
        CurrentState = newState;
        Debug.Log($"Game state changed to: {newState}");
    }
}
