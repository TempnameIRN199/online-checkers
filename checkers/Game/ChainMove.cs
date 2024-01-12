using System.Collections.Generic;

namespace Checkers.Game;

public class ChainMove : Move
{
    public List<(Position capture, Position free)> Steps = new();

    public int BecomesQueenOn;
    public Cell Piece;
    public ChainMove() 
    {
        IsCapture = true;
    }
    public ChainMove Fork()
    {
        return new ChainMove()
        {
            From = From,
            To = To,
            IsCapture = true,
            BecomesQueen = BecomesQueen,
            BecomesQueenOn = BecomesQueenOn,
            Steps = new(Steps),
        };
    }
}

