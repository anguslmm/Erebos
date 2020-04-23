using System;
using System.Collections.Generic;
using System.Linq;
using Erebos.Engine.Enums;
using Erebos.Engine.Pieces;
using UnityEngine;

namespace Erebos.Engine.GameManagement
{
    public class GameBoard : MonoBehaviour, IInteractable
    {
        public int tileSize;

        public Vector3 firstTileStart;

        public GameObject rookPrefab;
        public GameObject pawnPrefab;
        public GameObject bishopPrefab;
        public GameObject queenPrefab;
        public GameObject kingPrefab;
        public GameObject knightPrefab;

        public GameManager gameManager;

        private readonly Dictionary<Type, GameObject> _pieceToPrefabDictionary = new Dictionary<Type, GameObject>();

        private BoardCell[][] _boardCells;

        public Dictionary<Sides, Dictionary<Type, List<Piece>>> PiecesBySideInPlay { get; } = new Dictionary<Sides, Dictionary<Type, List<Piece>>>
        {
            {Sides.Black, new Dictionary<Type, List<Piece>>()},
            {Sides.White, new Dictionary<Type, List<Piece>>()}
        };

        public Dictionary<Sides, HashSet<BoardCell>> CellsUnderAttackBySide { get; } = new Dictionary<Sides, HashSet<BoardCell>>
        {
            {Sides.Black, new HashSet<BoardCell>()},
            {Sides.White, new HashSet<BoardCell>()}
        };

        public Piece SelectedPiece { get; private set; } = null;

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

        private void InitializeCells()
        {
            var boardCells = new BoardCell[8][];
            for (var x = 0; x < 8; x++)
            {
                var row = new BoardCell[8];
                for (var y = 0; y < 8; y++)
                {
                    row[y] = new BoardCell(x, y, this);
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
            piece.Side = y <= 1 ? Sides.White : Sides.Black;
            piece.gameObject.name = $"{typeof(T).Name}-{piece.Side}";

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

        private BoardCell GetCellFromPosition(Vector3 position)
        {
            Debug.Log(position);
            var x = (int) (position.x / tileSize * -1);
            var y = (int) (position.z / tileSize * -1);
            Debug.Log($"{x},{y}");

            return GetCellFromPosition(x, y);
        }

        public BoardCell GetCellFromPosition(int x, int y)
        {
            return _boardCells[x][y];
        }
        
        public bool TryGetCellFromPosition(int x, int y, out BoardCell boardCell)
        {
            boardCell = null;

            if (x < 0 || x > 7 || y < 0 || y > 7)
                return false;
            
            boardCell = _boardCells[x][y];
            return true;
        }

        public void RecalculateCellsUnderAttack()
        {
            RecalculateCellsUnderAttack(Sides.Black);
            RecalculateCellsUnderAttack(Sides.White);
        }

        public void RecalculateCellsUnderAttack(Sides side)
        {
            var boardCells = new HashSet<BoardCell>(PiecesBySideInPlay[side].SelectMany(pair =>
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
                // Is the cell empty?
                if (boardCell.Piece == null || boardCell.Piece.Side != GameManager.Instance.CurrentTurn)
                    return;

                // It must be our piece!
                SelectedPiece = boardCell.Piece;
                SelectedPiece.Select();
            }
            else
            {
                // Here we have a piece selected so the player is trying to do something with that piece or change the selection.
                if (SelectedPiece.Equals(boardCell.Piece))
                {
                    SelectedPiece.Deselect();
                    SelectedPiece = null;
                    return;
                }

                if (!SelectedPiece.FindPossibleMovementPaths().Contains(boardCell))
                    return;

                // Here, the piece is allowed to move to this cell.  The piece will know how to move to that cell and tell the occupying piece, if any, what to do.
                SelectedPiece.MoveToCell(boardCell);
                GameManager.Instance.EndTurn();
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