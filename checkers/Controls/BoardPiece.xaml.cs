using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using Checkers.Game;

namespace Checkers.Controls;
public partial class BoardPiece : UserControl, INotifyPropertyChanged
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
    private bool _isQueen;
    public bool IsQueen 
    {
        get => _isQueen;
        set 
        {
            _isQueen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsQueen)));
        }
    }
    public Position Position 
    {
        get => new Position((int)((Canvas.GetLeft(this) - 1.0) / 10.0), (int)((Canvas.GetTop(this) - 1.0) / 10));
        set { Canvas.SetLeft(this, value.X * 10 + 1); Canvas.SetTop(this, value.Y * 10 + 1); }
    }
    public BoardPiece(Brush color)
    {
        _currentColor = color;
        IsQueen = false;
        InitializeComponent();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    
}
