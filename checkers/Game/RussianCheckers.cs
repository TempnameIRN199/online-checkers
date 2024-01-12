using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;

namespace Checkers.Game;
public class RussianCheckers : CheckersBase
{

    public RussianCheckers()
    {
        Board = new Cell?[8, 8];
        InitializeBoard();

        CalculateAllAvailableMoves();
    }
    private List<Move> _allAvailableMoves = new();

    public override Cell?[,] Board { get; protected set; }
    public override Player PlayerToMove { get; protected set; }
    public bool TryGetCell(Position piecePostition, out Cell? piece)
    {
        piece = default;
        if (piecePostition.X < 0 || piecePostition.X >= 8 || piecePostition.Y < 0 || piecePostition.Y >= 8)
            return false;

        piece = Board[piecePostition.Y, piecePostition.X];
        return true;
    }
    protected override void CalculateAllAvailableMoves()
    {
        _allAvailableMoves.Clear();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                _allAvailableMoves.AddRange(CalculateMovesAt(new(i, j)));
            }
        }
        if (_allAvailableMoves.Any(x => x.IsCapture))
            _allAvailableMoves.RemoveAll(x => !x.IsCapture);
    }
    protected void CalculateChainMove(List<Move> moves, ChainMove chain, Position nextCapture, Position after, bool isQueen = false)
    {
        int queenRow = PlayerToMove == Player.White ? 7 : 0;
        var prev = chain.Steps.Select(x => x.free).LastOrDefault(chain.From);
        if (after.Y == queenRow && !chain.BecomesQueen && !Board[chain.From.Y, chain.From.X].Value.IsQueen)
        {
            chain.BecomesQueen = true;
            chain.BecomesQueenOn = chain.Steps.Count;
        }
        chain.Steps.Add((nextCapture, after));
        var attackable = GetAllAttackablePieces(after, isQueen, chain);
        var posibleChain = attackable.ExceptBy(chain.Steps.Select(x => x.capture), x => x.capture).ToList();
        if (posibleChain.Count() == 0)
        {
            chain.To = after;
            moves.Add(chain);
        }
        else
        {
            var fork = chain.Fork();
            CalculateChainMove(moves, chain, posibleChain.First().capture, posibleChain.First().freeplace, isQueen);
            foreach (var posible in posibleChain.Skip(1))
            {
                var forked = fork.Fork();
                CalculateChainMove(moves, forked, posible.capture, posible.freeplace, isQueen);
            }
        }

    }
    protected virtual void CalculateAttackMove(List<Move> moves, Position piecePostition, Position pieceToAttack, Position freePos)
    {
        var piece = Board[piecePostition.Y, piecePostition.X].Value;
        int queenRow = PlayerToMove == Player.White ? 7 : 0;
        bool becomesQueen = queenRow == freePos.Y && !piece.IsQueen;
        var posibleChain = GetAllAttackablePieces(freePos, piece.IsQueen || becomesQueen).Where(x => x.capture != pieceToAttack);
        if (posibleChain.Count() != 0)
        {
            foreach (var posible in posibleChain)
            {
                var chain = new ChainMove();
                chain.Piece = piece;
                chain.BecomesQueen = becomesQueen;
                chain.BecomesQueenOn = 0;
                chain.From = piecePostition;
                CalculateChainMove(moves, chain, pieceToAttack, freePos, piece.IsQueen || becomesQueen);

            }
        }
        else
        {
            moves.Add(new Move()
            {
                IsCapture = true,
                From = piecePostition,
                To = freePos,
                Capture = pieceToAttack,
                BecomesQueen = becomesQueen,
            });
        }
    }
    protected virtual List<(Position capture, Position freeplace)> GetAllAttackablePieces(Position piecePosition, bool isQueen = false, ChainMove ignoreChain = null)
    {
        if (isQueen)
        {
            return GetAllAttackablePiecesForQueen(piecePosition, ignoreChain);
        }
        var positions = new List<(Position, Position)>();
        Position leftBottomPos = new(piecePosition.X - 1, piecePosition.Y - 1);
        Position leftBottomPosEmpty = new(piecePosition.X - 2, piecePosition.Y - 2);
        Position rightBottomPos = new(piecePosition.X + 1, piecePosition.Y - 1);
        Position rightBottomPosEmpty = new(piecePosition.X + 2, piecePosition.Y - 2);

        Position leftTopPos = new(piecePosition.X - 1, piecePosition.Y + 1);
        Position leftTopPosEmpty = new(piecePosition.X - 2, piecePosition.Y + 2);
        Position rightTopPos = new(piecePosition.X + 1, piecePosition.Y + 1);
        Position rightTopPosEmpty = new(piecePosition.X + 2, piecePosition.Y + 2);
        if (CanPerformAttack(leftBottomPos, leftBottomPosEmpty, ignoreChain))
        {
            positions.Add((leftBottomPos, leftBottomPosEmpty));
        }
        if (CanPerformAttack(rightBottomPos, rightBottomPosEmpty, ignoreChain))
        {
            positions.Add((rightBottomPos, rightBottomPosEmpty));
        }
        if (CanPerformAttack(leftTopPos, leftTopPosEmpty, ignoreChain))
        {
            positions.Add((leftTopPos, leftTopPosEmpty));
        }
        if (CanPerformAttack(rightTopPos, rightTopPosEmpty, ignoreChain))
        {
            positions.Add((rightTopPos, rightTopPosEmpty));
        }

        return positions;
    }
    protected virtual List<(Position capture, Position freeplace)> GetAllAttackablePiecesForQueen(Position piecePosition, ChainMove? ignoreChain)
    {
        var positions = new List<(Position capture, Position freeplace)>();
        CheckQueenAttack(positions, piecePosition, new(1, 1), ignoreChain);
        CheckQueenAttack(positions, piecePosition, new(-1, 1), ignoreChain);
        CheckQueenAttack(positions, piecePosition, new(1, -1), ignoreChain);
        CheckQueenAttack(positions, piecePosition, new(-1, -1), ignoreChain);
        var count = positions.Count;
        for (int i = 0; i < count; i++)
        {
            var (capture, free) = positions[i];
            var capt = capture;
            var nextpos = Position.GetAfterCapture(capture, free);
            while (TryGetCell(nextpos, out var nextfree) && (nextfree.IsEmpty(nextpos, ignoreChain)))
            {
                positions.Add((capt, nextpos));

                capture = free;
                free = nextpos;
                nextpos = Position.GetAfterCapture(capture, free);

            }
        }
        return positions;

    }

    private List<Move> CalculateMovesAt(Position piecePosition)
    {
        var piececell = Board[piecePosition.Y, piecePosition.X];
        if (piececell is null) return new();
        if (piececell?.Player != PlayerToMove) return new();


        var moves = new List<Move>();

        var piece = piececell.Value;
        int forwardDirection = piece.Player == Player.White ? 1 : -1;
        int queenRow = piece.Player == Player.White ? 7 : 0;


        var attackables = GetAllAttackablePieces(piecePosition, piece.IsQueen);
        if (attackables.Count == 0)
        {
            if (piece.IsQueen)
            {
                CheckQueenMove(moves, piecePosition, new(1, 1));
                CheckQueenMove(moves, piecePosition, new(-1, 1));
                CheckQueenMove(moves, piecePosition, new(1, -1));
                CheckQueenMove(moves, piecePosition, new(-1, -1));
            }
            var leftPosMove = new Position(piecePosition.X - 1, piecePosition.Y + forwardDirection);
            if (TryGetCell(leftPosMove, out Cell? leftNeighbour) && leftNeighbour is null)
            {
                moves.Add(new Move() { From = piecePosition, To = leftPosMove, IsCapture = false, BecomesQueen = leftPosMove.Y == queenRow });
            }
            var rightPosMove = new Position(piecePosition.X + 1, piecePosition.Y + forwardDirection);
            if (TryGetCell(rightPosMove, out Cell? rightNeighbour) && rightNeighbour is null)
            {
                moves.Add(new Move() { From = piecePosition, To = rightPosMove, IsCapture = false, BecomesQueen = rightPosMove.Y == queenRow });
            }
        }
        else
        {
            foreach (var attackable in attackables)
            {
                CalculateAttackMove(moves, piecePosition, attackable.capture, attackable.freeplace);
            }

        }

        return moves;
    }
    public override IEnumerable<Move> GetAvailableMoves(Position piecePostition)
    {
        return _allAvailableMoves.Where(x => x.From == piecePostition);
    }
    private void InitializeBoard()
    {
        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++)
                Board[i, j] = null;
        for (int i = 0; i < 4; i++)
        {
            Board[0, i * 2] = new Cell { Player = Player.White };
            Board[1, (i) * 2 + 1] = new Cell { Player = Player.White };
            Board[2, i * 2] = new Cell { Player = Player.White };
        }
        for (int i = 0; i < 4; i++)
        {
            Board[7, (i) * 2 + 1] = new Cell { Player = Player.Black };
            Board[6, i * 2] = new Cell { Player = Player.Black };
            Board[5, (i) * 2 + 1] = new Cell { Player = Player.Black };
        }

    }

    public override IReadOnlyList<Move> GetAllAvailableMoves()
    {
        return _allAvailableMoves;
    }

    public override CheckersBase Fork(Move? move = null)
    {
        var checkers = new RussianCheckers();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                checkers.Board[i, j] = Board[i, j];
            }
        }
        checkers.PlayerToMove = PlayerToMove;
        if (move != null)
            checkers.MakeMove(move);
        return checkers;
    }
}
