using System.Linq;

namespace Checkers.Game;
public static class Extensions 
{
    public static bool IsEmpty(this Cell? cell, Position pos, ChainMove? ignoreChain = null) 
    {
        return cell == null || (ignoreChain is not null && NotInChainAnymore(ignoreChain, pos));
    }

    private static bool NotInChainAnymore(ChainMove ignoreChain, Position cell) 
    {
        if (cell == ignoreChain.From) return true;
        return false;
    }
}
