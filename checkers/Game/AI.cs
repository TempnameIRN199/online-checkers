using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Checkers.Game;

public class AI
{
    private CheckersBase _checkers;

    public AI(CheckersBase checkers)
    {
        _checkers = checkers;
    }
    public Move GetNextMove()
    {
        var moves = _checkers.GetAllAvailableMoves().AsParallel().Select(move => (CalculateMoveScore(move, _checkers, 3), move)).ToList();
        var move = moves.MaxBy(x => x.Item1).move;
        return move;
    }
    
    private int CalculateMoveScore(Move move, CheckersBase checkers, int depth = 1)
    {
        if (depth == 0) return 0;
        int score = 0;
        if (move is not ChainMove && move.IsCapture)
        {
            if (checkers.Board[move.Capture.Y, move.Capture.X].Value.IsQueen)
            {
                score += 5;
            }
            else
            {
                score += 1;
            }
        }
        else if (move is ChainMove chain)
        {
            foreach (var step in chain.Steps)
            {
                if (checkers.Board[step.capture.Y, step.capture.X].Value.IsQueen)
                {
                    score += 5;
                }
                else
                {
                    score += 1;
                }
            }
        }
        if (move.BecomesQueen) score += 5;
        var newcheckers = checkers.Fork();
        var player = newcheckers.PlayerToMove;
        var result = newcheckers.MakeMove(move);
        if (player == Player.White && result == MoveResult.WhiteWin)
        {
            return 1000;
        }
        if (player == Player.Black && result == MoveResult.BlackWin)
        {
            return 1000;
        }
        if (player == Player.White && result == MoveResult.BlackWin)
        {
            return -1000;
        }
        if (player == Player.Black && result == MoveResult.WhiteWin)
        {
            return -1000;
        }
        var nextScore = (int)double.Ceiling(newcheckers.GetAllAvailableMoves().Select(x => CalculateMoveScore(x, newcheckers.Fork(), depth - 1)).Average());

        return score - nextScore;
    }
}
