using System;
using Erebos.Engine.Enums;
using UnityEngine;

namespace Erebos.Engine.GameManagement
{
    public class GameManager
    {
        public static GameManager Instance = new GameManager();
        
        public Sides CurrentTurn { get; private set; } = Sides.Black;

        public int TurnNumber { get; private set; } = 1;

        public GameBoard GameBoard { get; }

        public event EventHandler<TurnEndedEventArgs> OnTurnEnded; 
        public event EventHandler<TurnEndedEventArgs> OnTurnEnding; 

        private GameManager()
        {
            GameBoard = GameObject.Find("GameBoard").GetComponent<GameBoard>();
        }

        public void EndTurn()
        {
            OnTurnEnding?.Invoke(this, new TurnEndedEventArgs());
            Instance.CurrentTurn = Instance.CurrentTurn == Sides.Black ? Sides.White : Sides.Black;
            Instance.TurnNumber++;
            OnTurnEnded?.Invoke(this, new TurnEndedEventArgs());
        }

    }
}
