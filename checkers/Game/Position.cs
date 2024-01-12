namespace Checkers.Game;

public struct Position
{
    public int X, Y;
    public Position(int x, int y) => (X, Y) = (x, y);
    //public static Position GetCapture(Position from, Position to)
    //{
    //    var x = (to.X - from.X) / 2 + from.X;
    //    var y = (to.Y - from.Y) / 2 + from.Y;
    //    return new(x, y);
    //}
    public static Position GetAfterCapture(Position from, Position capture)
    {
        var x = (capture.X - from.X) * 2 + from.X;
        var y = (capture.Y - from.Y) * 2 + from.Y;
        return new(x, y);
    }
       public override string ToString() => $"{X}, {Y}";
    public static bool operator ==(Position p1, Position p2) => p1.X == p2.X && p1.Y == p2.Y;
    public static bool operator !=(Position p1, Position p2) => p1.X != p2.X || p1.Y != p2.Y;
    public static Position operator +(Position p1, Position p2) => new(p1.X + p2.X, p1.Y + p2.Y);
    public static Position operator -(Position p1, Position p2) => new(p1.X - p2.X, p1.Y - p2.Y);

}

