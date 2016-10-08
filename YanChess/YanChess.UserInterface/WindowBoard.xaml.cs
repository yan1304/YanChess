using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using YanChess.GameLogic;
using YanChess.UserInterface.Windows;

namespace YanChess.UserInterface
{
    /// <summary>
    /// Логика взаимодействия для WindowBoard.xaml
    /// </summary>
    public partial class WindowBoard : Window
    {
        private double TecSizeX;
        private double TecSizeY;
        private bool isPC;
        private bool isWhite;
        private bool isFliped;
        private TimeSpan tw, tb;
        private static System.Timers.Timer timer;
        public WindowBoard(Position pos, TimeSpan tW,TimeSpan tB,bool IsWhite,bool IsPC,bool fliped)
        {
            //Установка параметров игры
            isFliped = fliped;
            timer = new System.Timers.Timer(300);
            InitializeComponent();
            chessBoard.IsStartGame = true;
            TecSizeX = Height;
            TecSizeY = Width;
            tw = tW;
            tb = tB;
            isWhite = IsWhite;
            isPC = IsPC;
            timeWhite.UpdateTime(tW);
            timeBlack.Color = ColorFigur.black;
            timeWhite.Color = ColorFigur.white;
            timeBlack.UpdateTime(tB);
            GameLogic.GameLogic.GamePosition = (Position)pos.DeepCopy();
            GameLogic.GameLogic.GamePosition.IsWhiteMove = IsWhite;
            if (!IsWhite) chessBoard.Flip();
            GameLogic.GameLogic.NewGame(tw,tb,null,null, (Position)pos.DeepCopy());
            timer.Enabled = true;
            timer.Elapsed += UpdateTime;
            timer.Elapsed += changeListMoves;
            chessBoard.IsWithPC = IsPC;
            Moves.Text = "";
            if(isFliped)
            {
                Grid.SetRow(timeBlack, 2);
                Grid.SetRow(timeWhite, 0);
            }

            //Проверяем в каком режиме включать движок
            if (IsWhite)
            {
                if (GameLogic.GameLogic.GamePosition.IsWhiteMove && chessBoard.IsWithPC)
                {
                    chessBoard.IsPCMove = false;
                    Task.Run(() =>
                    {
                        chessBoard.engine.SearchInTime(GameLogic.GameLogic.GamePosition, 1, true);
                    });
                }
                else if (chessBoard.IsWithPC)
                {
                    chessBoard.IsPCMove = true;
                    Task.Run(() =>
                    {
                        int? myK = chessBoard.engine.SearchInTime(GameLogic.GameLogic.GamePosition);
                        MoveCoord mc = chessBoard.engine.MovesWithScore.Where(vpm => vpm.Score == myK).FirstOrDefault().MC;
                        if (mc != null)
                        {
                            GameLogic.GameLogic.TryMoved(mc);
                        }
                        chessBoard.IsPCMove = !chessBoard.IsPCMove;
                        Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                        (ThreadStart)delegate ()
                        {
                            chessBoard.UpdatePosition();
                        });
                        Task.Run(() =>
                        {
                            chessBoard.engine.SearchInTime(GameLogic.GameLogic.GamePosition, 1, true);
                        });
                    });
                }
            }
            else if (chessBoard.IsWithPC)
            {
                if (GameLogic.GameLogic.GamePosition.IsWhiteMove)
                {
                    chessBoard.IsPCMove = true;
                    Task.Run(() =>
                    {
                        double? myK = chessBoard.engine.SearchInTime(GameLogic.GameLogic.GamePosition);
                        MoveCoord mc = chessBoard.engine.MovesWithScore.Where(vpm => vpm.Score == myK).FirstOrDefault().MC;
                        if (mc != null)
                        {
                            GameLogic.GameLogic.TryMoved(mc);
                        }
                        chessBoard.IsPCMove = !chessBoard.IsPCMove;
                        Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                        (ThreadStart)delegate ()
                        {
                            chessBoard.UpdatePosition();
                        });
                        Task.Run(() =>
                        {
                            chessBoard.engine.SearchInTime(GameLogic.GameLogic.GamePosition, 1, true);
                        });
                    });
                }
                else
                {
                    chessBoard.IsPCMove = false;
                    Task.Run(() =>
                    {
                        chessBoard.engine.SearchInTime(GameLogic.GameLogic.GamePosition, 1, true);
                    });
                }
            }
            chessBoard.UpdatePosition();
        }

        public void UpdateTime(object cender, EventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
            (ThreadStart)delegate ()
            {
                if ((GameLogic.GameLogic.MaxTimeBlack - GameLogic.GameLogic.TimeBlack.Elapsed).TotalMilliseconds <= 0) EndTime();
                else if ((GameLogic.GameLogic.MaxTimeWhite - GameLogic.GameLogic.TimeWhite.Elapsed).TotalMilliseconds <= 0) EndTime();
                else
                {
                    timeBlack.UpdateTime(GameLogic.GameLogic.MaxTimeBlack - GameLogic.GameLogic.TimeBlack.Elapsed);
                    timeWhite.UpdateTime(GameLogic.GameLogic.MaxTimeWhite - GameLogic.GameLogic.TimeWhite.Elapsed);
                };
            });
        }
        private void EndTime()
        {
            timer.Stop();
            if (GameLogic.GameLogic.GamePosition.IsWhiteMove) MessageBox.Show("Время белых вышло");
            else MessageBox.Show("Время черных вышло");
            GameLogic.GameLogic.StopGame();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            switch (((MenuItem)sender).Header.ToString())
            {
                case "Exit":
                    Engine.Engine.IsStopSearch = true;
                    this.Close();
                    break;
                case "Back to main window":
                    backButton_Click(sender, e);
                    break;
                case "Flip board":
                    chessBoard.Flip();
                    isFliped = !isFliped;
                    if (isFliped)
                    {
                        Grid.SetRow(timeBlack, 2);
                        Grid.SetRow(timeWhite, 0);
                    }
                    else
                    {
                        Grid.SetRow(timeBlack, 0);
                        Grid.SetRow(timeWhite, 2);
                    }
                    break;
                case "Option":
                    WindowGameOption w = new WindowGameOption(true);
                    w.Show();
                    break;
            }
        }
        
        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Height*1.5 > Width) Width = Height * 1.5;
            else if (Width > Height*1.5) Height = Width/1.5;
            TecSizeY = ActualHeight;
            TecSizeX = ActualWidth;
            if (Height == 1000)
            {
                Moves.FontSize = 20;
                backButton.FontSize = 28;
            }
            else if (Height > 900)
            {
                Moves.FontSize = 19;
                backButton.FontSize = 26;
            }
            else if (Height > 800)
            {
                Moves.FontSize = 18;
                backButton.FontSize = 24;
            }
            else if (Height > 700)
            {
                Moves.FontSize = 17;
                backButton.FontSize = 22;
            }
            else if (Height > 600)
            {
                Moves.FontSize = 16;
                backButton.FontSize = 21;
            }
            else
            {
                Moves.FontSize = 15;
                backButton.FontSize = 20;
            }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow m = new MainWindow();
            Engine.Engine.IsStopSearch = true;
            m.Show();
            timer.Stop();
            this.Close();
        }

        private void changeListMoves(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
            (ThreadStart)delegate ()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append($"1. ");
                int i = 1;
                foreach (MoveCoord mc in GameLogic.GameLogic.Moves)
                {
                    if (i != 1 && mc.StartFigure.Color == ColorFigur.white) sb.Append($"{i}. ");
                    if (i == 1 && mc.StartFigure.Color == ColorFigur.black && sb.ToString().Equals("1. ")) sb.Append("  ...          ");
                    sb.Append(ConvertIntCoordToChar(mc.yStart));
                    sb.Append($"{mc.xStart+1}");
                    sb.Append($" - ");
                    sb.Append(ConvertIntCoordToChar(mc.yEnd));
                    sb.Append($"{mc.xEnd + 1}");
                    if (mc.StartFigure.Color == ColorFigur.white)
                    {
                        sb.Append("          ");
                    }
                    else
                    {
                        sb.Append("\n");
                        i++;
                    }
                }
                Moves.Text = sb.ToString();
            });
        }

        private string ConvertIntCoordToChar(int i)
        {
            switch(i)
            {
                case 0: return "a";
                case 1: return "b";
                case 2: return "c";
                case 3: return "d";
                case 4: return "e";
                case 5: return "f";
                case 6: return "g";
                case 7: return "h";
                default: return "Error";
            }
        }
    }
}
