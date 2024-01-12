using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using Checkers.Game;

namespace Checkers.Controls;
public partial class BoardCell : UserControl, INotifyPropertyChanged
{
    private Brush _currentColor;

    public Brush CurrentColor 
    {
        get => _currentColor;
        set 
        {
            _currentColor = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentColor)));
        } 
    }
    private bool _isActive;
    public bool IsActive 
    {
        get => _isActive;
        set 
        {
            _isActive = value;
            CurrentColor = _isActive ? ActiveColor : DefaultColor;
        }
    }
    private bool _isLastMove;
    public bool IsLastMove
    {
        get => _isLastMove;
        set
        {
            _isLastMove = value;
            CurrentColor = _isLastMove ? LastMoveColor : IsActive ? ActiveColor : DefaultColor;
        }
    }
    public Brush ActiveColor { get; private set; }
    public Brush DefaultColor { get; private set; }
    public Brush LastMoveColor { get; private set; }
    public Position Position => new Position((int)(Canvas.GetLeft(this) / 10.0), (int)(Canvas.GetTop(this) / 10));
    public BoardCell(Brush defaultColor, Brush activeColor, Brush lastMoveColor)
    {
        _currentColor = defaultColor;
        DefaultColor = defaultColor;
        ActiveColor = activeColor;
        LastMoveColor = lastMoveColor;
        IsActive = false;
        InitializeComponent();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    
}
