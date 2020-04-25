using System;
using System.Collections.Generic;
using Erebos.Engine.Enums;
using Erebos.Engine.GameManagement;
using UnityEngine;

namespace Erebos.Engine.Pieces
{
    public abstract class Piece : MonoBehaviour, IEquatable<Piece>
    {
        public Sides Side { get; set; }

        public Sides OpposingSide => Side == Sides.Black ? Sides.White : Sides.Black;

        public bool HasMoved { get; set; }

        public ChessBoardCell ChessBoardCell { get; private set; }


        public abstract HashSet<ChessBoardCell> FindPossibleMovementPaths();

        public virtual void MoveToCell(ChessBoardCell desiredChessBoardCell)
        {
            ChessBoardCell = desiredChessBoardCell;
            gameObject.transform.position = desiredChessBoardCell.ToRelativeCenterPosition();
        }

        public virtual void InitializeToCell(ChessBoardCell desiredChessBoardCell)
        {
            ChessBoardCell = desiredChessBoardCell;
            gameObject.transform.position = desiredChessBoardCell.ToRelativeCenterPosition();
            
            Side = ChessBoardCell.Y <= 1 ? Sides.White : Sides.Black;
            gameObject.name = $"{GetType().Name}-{Side}";
        }

        public void Select()
        {
            Debug.Log($"{this} showing selected!");
        }
        
        public void Deselect()
        {
            Debug.Log($"{this} deselected!");
        }

        public bool Equals(Piece other)
        {
            return other != null && gameObject.GetInstanceID() == other.gameObject.GetInstanceID();
        }

        public override string ToString()
        {
            return gameObject.name;
        }
    }
}