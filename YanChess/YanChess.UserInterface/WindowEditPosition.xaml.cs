using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using YanChess.GameLogic;
using YanChess.UserInterface;

namespace YanChess.UserInterface.Windows
{
    /// <summary>
    /// Логика взаимодействия для WindowEditPosition.xaml
    /// </summary>
    public partial class WindowEditPosition : Window
    {
        public WindowEditPosition()
        {
            GameLogic.GameLogic.GamePosition = new Position();
            InitializeComponent();
            initButtons();
            chessBoard.IsStartGame = false;
        }

        private void initButtons()
        {
            for (int x = 0; x < 12; x++)
            {
                Button g = new Button();
                switch (x)
                {
                    case 0: g.Name = "buttonWK"; break;
                    case 1: g.Name = "buttonWQ"; break;
                    case 2: g.Name = "buttonWR"; break;
                    case 3: g.Name = "buttonWB"; break;
                    case 4: g.Name = "buttonWN"; break;
                    case 5: g.Name = "buttonWP"; break;
                    case 6: g.Name = "buttonBK"; break;
                    case 7: g.Name = "buttonBQ"; break;
                    case 8: g.Name = "buttonBR"; break;
                    case 9: g.Name = "buttonBB"; break;
                    case 10: g.Name = "buttonBN"; break;
                    case 11: g.Name = "buttonBP"; break;
                }
                g.Click += ClickSelBut;
                g.Template = (ControlTemplate)FindResource("chbTemplate");
                //g.Style = (Style)FindResource();
                StringBuilder sb = new StringBuilder();
                sb=sb.Append(g.Name.Replace("button", ""));
                sb = sb.Append(".png");
                g.Background = new ImageBrush(new BitmapImage(new Uri(sb.ToString(), UriKind.Relative)));
                Grid b = new Grid();
                b.Name = g.Name.Replace("button", "grid");
                b.Background = new ImageBrush(new BitmapImage(new Uri(@"Resourses\B.png", UriKind.Relative)));
                b.Children.Add(g);
                Grid.SetColumn(g, 0);
                Grid.SetRow(g, 0);
                
                gridButtons.Children.Add(b);
                Grid.SetColumn(b, (x%6));
                Grid.SetRow(b, (x-(x%6))/6);
            }

        }
        private void ClickSelBut(object sender, RoutedEventArgs e)
        {
            chessBoard.IsInsert = true;
            statusBarText.Text = "Вставка фигуры";
            foreach(Grid g in gridButtons.Children)
            {
                if(g.Name.Replace("grid","")==((Button)e.OriginalSource).Name.Replace("button",""))
                {
                    if (g.Background is ImageBrush)
                    {
                        g.Background = new SolidColorBrush(Colors.Aqua);
                        SelectFigurFromButton((Button)e.OriginalSource);
                    }
                    else
                    {
                        g.Background = new ImageBrush(new BitmapImage(new Uri(@"Resourses\B.png", UriKind.Relative)));
                        chessBoard.IsInsert = false;
                    }
                }
                else
                {
                    g.Background = new ImageBrush(new BitmapImage(new Uri(@"Resourses\B.png", UriKind.Relative)));
                }
            }
        }

        private void SelectFigurFromButton(Button b)
        {
            switch (b.Name)
            {
                case "buttonWK": chessBoard.newFigure = new Square(new King(ColorFigur.white));break;
                case "buttonWQ": chessBoard.newFigure = new Square(new Queen(ColorFigur.white)); break;
                case "buttonWR": chessBoard.newFigure = new Square(new Rock(ColorFigur.white)); break;
                case "buttonWB": chessBoard.newFigure = new Square(new Bishop(ColorFigur.white)); break;
                case "buttonWN": chessBoard.newFigure = new Square(new Knight(ColorFigur.white)); break;
                case "buttonWP": chessBoard.newFigure = new Square(new Peen(ColorFigur.white)); break;
                case "buttonBK": chessBoard.newFigure = new Square(new King(ColorFigur.black)); break;
                case "buttonBQ": chessBoard.newFigure = new Square(new Queen(ColorFigur.black)); break;
                case "buttonBR": chessBoard.newFigure = new Square(new Rock(ColorFigur.black)); break;
                case "buttonBB": chessBoard.newFigure = new Square(new Bishop(ColorFigur.black)); break;
                case "buttonBN": chessBoard.newFigure = new Square(new Knight(ColorFigur.black)); break;
                case "buttonBP": chessBoard.newFigure = new Square(new Peen(ColorFigur.black)); break;
            }
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            switch (((MenuItem)sender).Header.ToString())
            {
                case "Exit":
                    this.Close();
                    break;
                case "Back to main window":
                    backButton_Click(sender, e);
                    break;
                case "Flip board":
                    chessBoard.Flip();
                    break;
                case "Option":
                    WindowGameOption w = new WindowGameOption(true);
                    w.Show();
                    break;

            }
        }

        private void startPosButton_Click(object sender, RoutedEventArgs e)
        {
            CreateStartPos();
        }

        public void CreateStartPos()
        {
            GameLogic.GameLogic.GamePosition = new Position("start");
            chessBoard.UpdatePosition();
        }
        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Height > Width) Width = Height*1.4;
            else if (Width > Height) Height = Width/1.4;
            if (Height == 1000)
            {
                clrButton.FontSize = 30;
                startPosButton.FontSize = 30;
                backButton.FontSize = 30;
                startButton.FontSize = 30;
            }
            else if(Height >900)
            {
                clrButton.FontSize = 27;
                startPosButton.FontSize = 27;
                backButton.FontSize = 27;
                startButton.FontSize = 27;
            }
            else if (Height > 800)
            {
                clrButton.FontSize = 25;
                startPosButton.FontSize = 25;
                backButton.FontSize = 25;
                startButton.FontSize = 25;
            }
            else if (Height > 700)
            {
                clrButton.FontSize = 23;
                startPosButton.FontSize = 23;
                backButton.FontSize = 23;
                startButton.FontSize = 23;
            }
            else if (Height > 600)
            {
                clrButton.FontSize = 21;
                startPosButton.FontSize = 21;
                backButton.FontSize = 21;
                startButton.FontSize = 21;
            }
            else
            {
                clrButton.FontSize = 20;
                startPosButton.FontSize = 20;
                backButton.FontSize = 20;
                startButton.FontSize = 20;
            }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            chessBoard.IsInsert = false;
            MainWindow m = new MainWindow();
            m.Show();
            this.Close();
        }

        private void ButtonMove_Click(object sender, RoutedEventArgs e)
        {
            chessBoard.IsInsert = false;
            statusBarText.Text = "Перемещение фигуры";
            foreach (Grid g in gridButtons.Children)
            {
                if (!(g.Background is ImageBrush)) g.Background = new ImageBrush(new BitmapImage(new Uri(@"Resourses\B.png", UriKind.Relative)));
            }
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            if (GameLogic.GameLogic.CheckPosition(GameLogic.GameLogic.GamePosition) != null)
            {
                statusBarText.Text = "Позиция содержит мат-пат";
                return;
            }
            GameLogic.GameLogic.GamePosition.IsWhiteMove = true;
            if (GameLogic.GameLogic.CheckPosition(GameLogic.GameLogic.GamePosition) != null)
            {
                statusBarText.Text = "Позиция содержит мат-пат";
                return;
            }
            bool haveKingW = false;
            bool haveKingB = false;
            foreach (Square s in GameLogic.GameLogic.GamePosition.Board)
            {
                if(s.Figure.Type==TypeFigur.king)
                {
                    if(s.Figure.Color == ColorFigur.white)
                    {
                        haveKingW = true;
                    }
                    else
                    {
                        haveKingB = true;
                    }
                }
            }
            if (!(haveKingW && haveKingB))
            {
                statusBarText.Text = "Отсутствует король, у одной или обоих сторон";
                return;
            }
            chessBoard.IsInsert = false;
            if (GameLogic.GameLogic.GamePosition.Board[0, 4] == new Square(new King(ColorFigur.white)))
            {
                if ((bool)isLeftW.IsChecked && GameLogic.GameLogic.GamePosition.Board[0, 0] == new Square(new Rock(ColorFigur.white)))
                {
                    GameLogic.GameLogic.GamePosition.IsKingMovedWhite = false;
                    GameLogic.GameLogic.GamePosition.IsLeftRockMovedWhite = false;
                }
                else
                {
                    GameLogic.GameLogic.GamePosition.IsLeftRockMovedWhite = true;
                }
                if ((bool)isRightW.IsChecked && GameLogic.GameLogic.GamePosition.Board[0, 7] == new Square(new Rock(ColorFigur.white)))
                {
                    GameLogic.GameLogic.GamePosition.IsKingMovedWhite = false;
                    GameLogic.GameLogic.GamePosition.IsRightRockMovedWhite = false;
                }
                else
                {
                    GameLogic.GameLogic.GamePosition.IsRightRockMovedWhite = true;
                }
            }
            else
            {
                GameLogic.GameLogic.GamePosition.IsKingMovedWhite = true;
            }
                
            if (GameLogic.GameLogic.GamePosition.Board[7, 4] == new Square(new King(ColorFigur.black)))
            {
                if ((bool)isLeftB.IsChecked && GameLogic.GameLogic.GamePosition.Board[7, 0] == new Square(new Rock(ColorFigur.black)))
                {
                    GameLogic.GameLogic.GamePosition.IsKingMovedBlack = false;
                    GameLogic.GameLogic.GamePosition.IsLeftRockMovedBlack = false;
                }
                else
                {
                    GameLogic.GameLogic.GamePosition.IsLeftRockMovedBlack = true;
                }
                if ((bool)isRightB.IsChecked && GameLogic.GameLogic.GamePosition.Board[7, 7] == new Square(new Rock(ColorFigur.black)))
                {
                    GameLogic.GameLogic.GamePosition.IsKingMovedBlack = false;
                    GameLogic.GameLogic.GamePosition.IsRightRockMovedBlack = false;
                }
                else
                {
                    GameLogic.GameLogic.GamePosition.IsLeftRockMovedBlack = true;
                }
            }
            else
            {
                GameLogic.GameLogic.GamePosition.IsKingMovedBlack = true;
            }
            GameLogic.GameLogic.GamePosition.IsWhiteMove = (bool)isWhiteMove.IsChecked;
            Option o = new Option(GameLogic.GameLogic.GamePosition);
            o.Show();
            this.Close();
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            chessBoard.IsInsert = true;
            statusBarText.Text = "Удаление фигуры";
            chessBoard.newFigure = new Square(new NotFigur());
            foreach (Grid g in gridButtons.Children)
            {
                if (!(g.Background is ImageBrush)) g.Background = new ImageBrush(new BitmapImage(new Uri(@"Resourses\B.png", UriKind.Relative)));
            }
        }

        private void clrButton_Click(object sender, RoutedEventArgs e)
        {
            GameLogic.GameLogic.GamePosition = new Position();
            chessBoard.UpdatePosition();
        }
    }
}
