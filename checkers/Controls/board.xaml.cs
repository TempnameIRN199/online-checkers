using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Checkers.Game;

namespace Checkers.Controls;
public partial class Board : UserControl
{
    const string ip = "127.0.0.1";
    const int port = 12345;

    IPEndPoint tcpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port); // получаем адреса для запуска сокета
    Socket tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // создаем сокет


    private CheckersBase _game;
    private BoardCell[,] _cells;
    private AI? _ai;
    private bool _humanTurn;
    private BoardCell _lastMove;
    private List<BoardCell> _activeCells = new();
    private List<Move> _activeMoves = new();
    private List<BoardPiece> _pieces;
    private int _currentStep;
    private bool _gameEnded;
    private MainWindow _parent;

    public Board(MainWindow parent, CheckersBase game, bool vsComputer, bool humanStarts)
    {
        InitializeComponent();

        tcpSocket.Connect(tcpEndPoint);

        _parent = parent; 
        if (vsComputer)
            _ai = new(game);
        _humanTurn = humanStarts;
        _cells = new BoardCell[8, 8];
        _game = game;

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                _cells[i, j] = new BoardCell((i + j) % 2 == 0 ? Brushes.DarkBlue : Brushes.DarkGray, Brushes.Red, Brushes.LightYellow)
                {
                    Width = 10,
                    Height = 10,
                };
                _cells[i, j].MouseDown += CellClicked;
                Canvas.SetLeft(_cells[i, j], j * 10);
                Canvas.SetTop(_cells[i, j], i * 10);
                gameCanvas.Children.Add(_cells[i, j]);
            }
        }
        var pieces = new List<BoardPiece>();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (_game.Board[i, j] is not null)
                {
                    pieces.Add(new BoardPiece(_game.Board[i, j]?.Player == Player.White ? Brushes.White : Brushes.Black));
                    gameCanvas.Children.Add(pieces.Last());
                    pieces.Last().IsQueen = _game.Board[i, j].Value.IsQueen;
                    pieces.Last().Position = new(j, i);
                    pieces.Last().MouseDown += PieceClicked;
                }
            }
        }
        _pieces = pieces;
        returnBtn.Click += delegate { _parent.Content = new MainMenu(); };
        if (!humanStarts)
        {
            DoAIMove();
        }
    }

    private void CheckGameResult(MoveResult result)
    {
        _humanTurn = !_humanTurn;
        switch (result)
        {
            case MoveResult.WhiteWin:
                MessageBox.Show("White wins");
                _gameEnded = true;
                break;
            case MoveResult.BlackWin:
                MessageBox.Show("Black wins");
                _gameEnded = true;
                break;
            case MoveResult.Draw:
                MessageBox.Show("Draw");
                _gameEnded = true;
                break;

        }

    }

    private async void DoAIMove()
    {
        if (_ai is not null && !_humanTurn)
        {
            var move = _ai.GetNextMove();
            await Task.Delay(500);
            CheckGameResult(_game.MakeMove(move));
            switch (move)
            {
                case ChainMove chain:
                    foreach (var step in chain.Steps)
                    {
                        var captured = _pieces.First(x => x.Position == step.capture);
                        _pieces.Remove(captured);
                        gameCanvas.Children.Remove(captured);
                    }
                    break;
                case Move:
                    if (move.IsCapture)
                    {
                        var captured = _pieces.First(x => x.Position == move.Capture);
                        _pieces.Remove(captured);
                        gameCanvas.Children.Remove(captured);

                    }
                    break;
            }
            var piece = _pieces.First(x => x.Position == move.From);
            if (move.BecomesQueen)
                piece.IsQueen = true;
            piece.Position = move.To;
            SetLastMove(move);

        }
        if (_gameEnded)
        {
            _parent.Content = new MainMenu();
        }
    }

    private void SetLastMove(Move move)
    {
        if (_lastMove != null)
            _lastMove.IsLastMove = false;
        _lastMove = _cells[move.From.Y, move.From.X];
        _lastMove.IsLastMove = true;
    }

    private void CellClicked(object sender, MouseButtonEventArgs e)
        {
        // isServerConnected = true;
        if (tcpSocket.Poll(1000, SelectMode.SelectRead) && (tcpSocket.Available == 0))
        {
            MessageBox.Show("Сервер не подключен");
            return;
        }
        else
        {
            if (IPAddress.Any != null)
            {
                var cell = (sender as BoardCell)!;

                if (_activeCells.Contains(cell))
                {
                    foreach (var available in _activeCells)
                        available.IsActive = false;
                    _activeCells.Clear();
                    var chains = _activeMoves.Where(x => x is ChainMove chain && _currentStep < chain.Steps.Count && chain.Steps[_currentStep].free == cell.Position).Cast<ChainMove>().ToList();

                    if (chains.Count != 0)
                    {
                        var chain = chains.First();
                        var captured = _pieces.First(x => x.Position == chain.Steps[_currentStep].capture);
                        gameCanvas.Children.Remove(captured);
                        _pieces.Remove(captured);
                        var previous = chain.From;
                        if (_currentStep - 1 >= 0)
                        {
                            previous = chain.Steps[_currentStep - 1].free;
                        }
                        var piece = _pieces.First(x => x.Position == previous);
                        piece.Position = chain.Steps[_currentStep].free;
                        if (chain.BecomesQueen && chain.BecomesQueenOn == _currentStep)
                            piece.IsQueen = true;
                        if (_currentStep == chain.Steps.Count - 1)
                        {
                            CheckGameResult(_game.MakeMove(chain));
                            SetLastMove(chain);

                            _currentStep = 0;

                        }
                        else
                        {
                            _currentStep++;
                            var availables = chains.Where(x => _currentStep < x.Steps.Count).Select(x => x.Steps[_currentStep].free);
                            _activeMoves = chains.Where(x => _currentStep < x.Steps.Count).Cast<Move>().ToList();

                            foreach (var available in availables)
                            {
                                _cells[available.Y, available.X].IsActive = true;
                                _activeCells.Add(_cells[available.Y, available.X]);
                            }
                        }
                    }
                    else
                    {
                        var move = _activeMoves.Find(x => x.To.X == cell.Position.X && x.To.Y == cell.Position.Y);
                        CheckGameResult(_game.MakeMove(move!));
                        SetLastMove(move);
                        var piece = _pieces.First(x => x.Position == move.From);
                        piece.Position = move.To;
                        if (move.BecomesQueen)
                            piece.IsQueen = true;
                        if (move.IsCapture)
                        {
                            var captured = _pieces.First(x => x.Position == move.Capture);
                            gameCanvas.Children.Remove(captured);
                            _pieces.Remove(captured);
                        }
                    }
                    if (_gameEnded)
                    {
                        _parent.Content = new MainMenu();
                    }
                    else
                    {
                        DoAIMove();
                    }
                }
            }
        }

        //if (isServerConnected == false)
        //{
        //    MessageBox.Show("Сервер не подключен");
        //    return;
        //}
        //else
        //{
        //    while (isServerConnected)
        //    {
        //        try
        //        {

        //            // NetworkStream stream = tcpClient.GetStream();


                    
        //        }
        //        catch (SocketException ex)
        //        {
        //            MessageBox.Show($"Ошибка клиента: {ex.Message}");
        //            tcpClient.Close();
        //            isServerConnected = false;
        //                // close
        //            throw;
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show($"Ошибка клиента: {ex.Message}");
        //            isServerConnected = false;
        //            tcpClient.Close();
        //            throw;
        //        }
        //        finally
        //        {
        //            isServerConnected = false;
        //        }
        //    }
        //}
    }

    private void PieceClicked(object o, EventArgs e)
    {
        foreach (var available in _activeCells)
            available.IsActive = false;
        _activeCells.Clear();
        var piece = (o as BoardPiece)!;
        var moves = _game.GetAvailableMoves(piece.Position);
        _activeMoves = moves.ToList();
        foreach (var move in moves)
        {
            switch (move)
            {
                case ChainMove chain:
                    var pos = chain.Steps[0].free;
                    _cells[pos.Y, pos.X].IsActive = true;
                    _activeCells.Add(_cells[pos.Y, pos.X]);
                    break;
                case Move:
                    _cells[move.To.Y, move.To.X].IsActive = true;
                    _activeCells.Add(_cells[move.To.Y, move.To.X]);
                    break;
            }

        }
    }



}
