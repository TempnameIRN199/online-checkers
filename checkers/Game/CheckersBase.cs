using System.Collections.Generic;

namespace Checkers.Game;

public abstract class CheckersBase
{
    public abstract Cell?[,] Board { get; protected set; }
    public abstract Player PlayerToMove { get; protected set; }
    public abstract IEnumerable<Move> GetAvailableMoves(Position piecePostition);
    public abstract IReadOnlyList<Move> GetAllAvailableMoves();
    public abstract CheckersBase Fork(Move? move = null);

    protected bool CanPerformAttack(Position capture, Position free, ChainMove? ignoreChain = null)
    {
        return
                TryGetCell(capture, out var capturecell)
                && capturecell != null
                && capturecell?.Player != PlayerToMove
                && TryGetCell(free, out var emptycell)
                && (emptycell.IsEmpty(free, ignoreChain));
    }

    public bool TryGetCell(Position piecePostition, out Cell? piece)
    {
        piece = default;
        if (piecePostition.X < 0 || piecePostition.X >= 8 || piecePostition.Y < 0 || piecePostition.Y >= 8)
            return false;

        piece = Board[piecePostition.Y, piecePostition.X];
        return true;
    }
    protected abstract void CalculateAllAvailableMoves();
    protected (int black, int white) CountPieces()
    {
        int black = 0;
        int white = 0;
        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++)
                if (Board[i, j]?.Player == Player.White)
                {
                    white++;
                }
                else if (Board[i, j]?.Player == Player.Black)
                {
                    black++;
                }
        return (black, white);
    }

    public virtual MoveResult MakeMove(Move move)
    {
        switch (move)
        {
            case ChainMove chain:
                {
                    //var cell = Board[move.From.Y, move.From.X]!.Value;
                    foreach (var step in chain.Steps)
                    {
                        Board[step.capture.Y, step.capture.X] = null;
                    }
                    Board[move.From.Y, move.From.X] = null;
                    if (chain.BecomesQueen) chain.Piece.IsQueen = true;
                    Board[move.To.Y, move.To.X] = chain.Piece;
                }
                break;
            case Move:
                {
                    var cell = Board[move.From.Y, move.From.X]!.Value;
                    Board[move.From.Y, move.From.X] = null;
                    if (move.BecomesQueen)
                        cell.IsQueen = true;
                    Board[move.To.Y, move.To.X] = cell;
                    if (move.IsCapture)
                    {
                        Board[move.Capture.Y, move.Capture.X] = null;
                    }
                }
                break;
        }
        PlayerToMove = PlayerToMove == Player.White ? Player.Black : Player.White;
        CalculateAllAvailableMoves();
        if (GetAllAvailableMoves().Count == 0)
        {
            return PlayerToMove == Player.White ? MoveResult.BlackWin : MoveResult.WhiteWin;
        }
        var (black, white) = CountPieces();
        if (black == 0) return MoveResult.WhiteWin;
        if (white == 0) return MoveResult.BlackWin;
        return MoveResult.InProgress;
    }
    protected virtual void CheckQueenAttack(List<(Position capture, Position free)> positions, Position piecePosition, Position diagonalDirection, ChainMove? ignoreChain)
    {
        var pos = piecePosition;
        var dir = diagonalDirection;
        Cell? cell = default;
        while (TryGetCell(pos + dir, out cell) && cell.IsEmpty(pos + dir, ignoreChain))
        {
            pos += dir;
        }
        if (cell is not null)
        {
            pos += dir;
            var free = pos + dir;
            if (CanPerformAttack(pos, free, ignoreChain))
            {
                positions.Add((pos, free));
            }
        }
    }
    protected virtual void CheckQueenMove(List<Move> moves, Position piecePosition, Position dir) 
    {
        var pos = piecePosition;
        while (TryGetCell(pos + dir, out Cell? cell) && cell is null)
        {
            pos += dir;
            moves.Add(new Move() { From = piecePosition, To = pos });
        }
    } 
}

