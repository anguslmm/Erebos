using System;
using System.Collections.Generic;
using System.Linq;
using Erebos.Engine.Enums;
using Erebos.Engine.Pieces;
using UnityEngine;

namespace Erebos.Engine.GameManagement
{
    public class ChessBoard : MonoBehaviour, IInteractable
    {
        // Configurable fields to be set in the Unity Editor
        public int tileSize;

        public Vector3 firstTileStart;

        public GameObject rookPrefab;
        public GameObject pawnPrefab;
        public GameObject bishopPrefab;
        public GameObject queenPrefab;
        public GameObject kingPrefab;
        public GameObject knightPrefab;

        public Material whitePiecesMaterial;
        public Material blackPiecesMaterial;
        
        // Properties that hold the game's state
        public Sides CurrentTurn { get; private set; } = Sides.Black;

        public int TurnNumber { get; private set; } = 1;

        private readonly Dictionary<Type, GameObject> _pieceToPrefabDictionary = new Dictionary<Type, GameObject>();

        private ChessBoardCell[][] _boardCells;

        public Dictionary<Sides, Dictionary<Type, List<Piece>>> PiecesBySideInPlay { get; } = new Dictionary<Sides, Dictionary<Type, List<Piece>>>
        {
            {Sides.Black, new Dictionary<Type, List<Piece>>()},
            {Sides.White, new Dictionary<Type, List<Piece>>()}
        };

        public Dictionary<Sides, HashSet<ChessBoardCell>> CellsUnderAttackBySide { get; } = new Dictionary<Sides, HashSet<ChessBoardCell>>
        {
            {Sides.Black, new HashSet<ChessBoardCell>()},
            {Sides.White, new HashSet<ChessBoardCell>()}
        };

        public Piece SelectedPiece { get; private set; } = null;
        
        // Events related to game state changes
        public event EventHandler<TurnEndedEventArgs> TurnEnded; 
        public event EventHandler<TurnEndedEventArgs> TurnEnding;

        public event EventHandler<PieceDestroyedEventArgs> PieceDestroyed; 

        void Start()
        {
            _pieceToPrefabDictionary[typeof(Pawn)] = pawnPrefab;
            _pieceToPrefabDictionary[typeof(Rook)] = rookPrefab;
            _pieceToPrefabDictionary[typeof(Bishop)] = bishopPrefab;
            _pieceToPrefabDictionary[typeof(Queen)] = queenPrefab;
            _pieceToPrefabDictionary[typeof(King)] = kingPrefab;
            _pieceToPrefabDictionary[typeof(Knight)] = knightPrefab;

            InitializeCells();
            InitializePieces();
        }
        
        public void EndTurn()
        {
            TurnEnding?.Invoke(this, new TurnEndedEventArgs());
            CurrentTurn = CurrentTurn.Opposite();
            TurnNumber++;
            SelectedPiece.OnDeselected();
            SelectedPiece = null;
            TurnEnded?.Invoke(this, new TurnEndedEventArgs());
        }

        private void InitializeCells()
        {
            var boardCells = new ChessBoardCell[8][];
            for (var x = 0; x < 8; x++)
            {
                var row = new ChessBoardCell[8];
                for (var y = 0; y < 8; y++)
                {
                    row[y] = new ChessBoardCell(x, y, this);
                }

                boardCells[x] = row;
            }

            _boardCells = boardCells;
        }

        private void InitializePieces()
        {
            // Pawns
            foreach (var y in new[] {1, 6})
                for (var x = 0; x <= 7; x++)
                    InstantiatePieceAt<Pawn>(x, y);

            // Rooks
            foreach (var x in new[] {0, 7})
            foreach (var y in new[] {0, 7})
                InstantiatePieceAt<Rook>(x, y);

            // Knights
            foreach (var x in new[] {1, 6})
            foreach (var y in new[] {0, 7})
                InstantiatePieceAt<Knight>(x, y);

            // Bishops
            foreach (var x in new[] {2, 5})
            foreach (var y in new[] {0, 7})
                InstantiatePieceAt<Bishop>(x, y);

            // Queens
            InstantiatePieceAt<Queen>(4, 0);
            InstantiatePieceAt<Queen>(3, 7);

            // Kings
            InstantiatePieceAt<King>(3, 0);
            InstantiatePieceAt<King>(4, 7);
        }

        private void InstantiatePieceAt<T>(int x, int y) where T : Piece, new()
        {
            if (!_pieceToPrefabDictionary.TryGetValue(typeof(T), out var prefab))
                throw new ArgumentOutOfRangeException(nameof(T), typeof(T).Name, "Unknown piece type!");

            var piece = Instantiate(prefab).AddComponent<T>();
            piece.transform.parent = gameObject.transform;

            var boardCell = _boardCells[x][y];
            boardCell.Piece = piece;
            piece.InitializeToCell(boardCell);

            if (PiecesBySideInPlay[piece.Side].TryGetValue(typeof(T), out var pieces))
                pieces.Add(piece);
            else
            {
                PiecesBySideInPlay[piece.Side][typeof(T)] = new List<Piece> {piece};
            }
        }

        private ChessBoardCell GetCellFromPosition(Vector3 position)
        {
            Debug.Log(position);
            var x = (int) (position.x / tileSize * -1);
            var y = (int) (position.z / tileSize * -1);
            Debug.Log($"{x},{y}");

            return GetCellFromPosition(x, y);
        }

        public ChessBoardCell GetCellFromPosition(int x, int y)
        {
            return _boardCells[x][y];
        }
        
        public bool TryGetCellFromPosition(int x, int y, out ChessBoardCell chessBoardCell)
        {
            chessBoardCell = null;

            if (x < 0 || x > 7 || y < 0 || y > 7)
                return false;
            
            chessBoardCell = _boardCells[x][y];
            return true;
        }

        public void RecalculateCellsUnderAttack()
        {
            RecalculateCellsUnderAttack(Sides.Black);
            RecalculateCellsUnderAttack(Sides.White);
        }

        public void RecalculateCellsUnderAttack(Sides side)
        {
            var boardCells = new HashSet<ChessBoardCell>(PiecesBySideInPlay[side].SelectMany(pair =>
            {
                return pair.Value.SelectMany(x => x.FindPossibleMovementPaths());
            }));

            CellsUnderAttackBySide[side] = boardCells;
        }
        
        public void OnPrimaryMouseUp(MouseEventArgs mouseEventArgs)
        {
            var boardCell = GetCellFromPosition(mouseEventArgs.Point);

            if (SelectedPiece == null)
            {
                // We know we're going to try to make a selection
                // Is the cell empty or an enemy piece?
                if (!boardCell.IsOccupied || boardCell.Piece.Side != CurrentTurn)
                    return;

                // It must be our piece!
                SelectedPiece = boardCell.Piece;
                SelectedPiece.OnSelected();
            }
            else
            {
                // Here we have a piece selected so the player is trying to do something with that piece or change the selection.
                if (SelectedPiece.Equals(boardCell.Piece))
                {
                    SelectedPiece.OnDeselected();
                    SelectedPiece = null;
                    return;
                }

                if (!SelectedPiece.FindPossibleMovementPaths().Contains(boardCell))
                    return;
                
                // Ok so the piece is allowed to move here.  Do we need to destroy a piece to move there? 
                if (boardCell.IsOccupied)
                {
                    boardCell.Piece.DestroyPiece();
                    boardCell.Piece = null;
                    PieceDestroyed?.Invoke(this, new PieceDestroyedEventArgs());
                }

                boardCell.Piece = SelectedPiece;
                SelectedPiece.MoveToCell(boardCell);
                EndTurn();
            }
        }

        public void OnPrimaryMouseDown(MouseEventArgs mouseEventArgs)
        {
        }

        public void OnSecondaryMouseUp(MouseEventArgs mouseEventArgs)
        {
        }

        public void OnSecondaryMouseDown(MouseEventArgs mouseEventArgs)
        {
        }

        public void OnMouseHover(MouseEventArgs mouseEventArgs)
        {
        }
    }
}