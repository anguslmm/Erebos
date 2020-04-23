using System;
using System.Collections.Generic;
using Erebos.Engine.Enums;
using Erebos.Engine.GameManagement;
using UnityEngine;

namespace Erebos.Engine.Pieces
{
    public abstract class Piece : MonoBehaviour, IInteractable, IEquatable<Piece>
    {
        public Sides Side { get; set; }

        public Sides OpposingSide => Side == Sides.Black ? Sides.White : Sides.Black;

        public bool HasMoved { get; set; } = false;

        public BoardCell BoardCell { get; private set; } = null;

        public BoardCell OriginCell { get; private set; } = null;

        public abstract HashSet<BoardCell> FindPossibleMovementPaths();

        public virtual void MoveToCell(BoardCell desiredBoardCell)
        {
            BoardCell = desiredBoardCell;
            gameObject.transform.position = desiredBoardCell.BoardPosition.ToRelativeCenterPosition();
        }

        public virtual void InitializeToCell(BoardCell desiredBoardCell)
        {
            OriginCell = desiredBoardCell;
            BoardCell = desiredBoardCell;
            gameObject.transform.position = desiredBoardCell.BoardPosition.ToRelativeCenterPosition();
        }

        public void Select()
        {
            Debug.Log($"{this} showing selected!");
        }
        
        public void Deselect()
        {
            Debug.Log($"{this} deselected!");
        }
        
        public void OnPrimaryMouseUp(MouseEventArgs mouseEventArgs)
        {
            throw new NotImplementedException();
        }

        public void OnPrimaryMouseDown(MouseEventArgs mouseEventArgs)
        {
            throw new NotImplementedException();
        }

        public void OnSecondaryMouseUp(MouseEventArgs mouseEventArgs)
        {
            throw new NotImplementedException();
        }

        public void OnSecondaryMouseDown(MouseEventArgs mouseEventArgs)
        {
            throw new NotImplementedException();
        }

        public void OnMouseHover(MouseEventArgs mouseEventArgs)
        {
            throw new NotImplementedException();
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