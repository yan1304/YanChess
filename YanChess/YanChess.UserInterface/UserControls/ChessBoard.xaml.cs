using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using YanChess.GameLogic;
using System.Windows.Threading;
using System.Threading;

namespace YanChess.UserInterface
{
    public struct Coord
    {
        public int x;
        public int y;
    }
    /// <summary>
    /// Логика взаимодействия для ChessBoard.xaml
    /// </summary>
    public partial class ChessBoard : UserControl
    {
        /// <summary>
        /// Движок
        /// </summary>
        public Engine.Engine engine { get; set; }
        private MoveCoord move;
        private bool isSelected;
        public bool IsStartGame { get; set; }
        public bool IsPCMove { get; set; }

        public bool IsWithPC { get; set; }
        /// <summary>
        /// Находится ли в режиме редактирования(т.е. вместо ходов добавляются/убираются фигуры
        /// </summary>
        public bool IsInsert { get; set; }
        /// <summary>
        /// Устанавливаемая фигура (если IsInsert == true)
        /// </summary>
        public Square newFigure { get; set; }
        public bool IsWhite { get; set; }

        public ChessBoard()
        {
            IsWithPC = false;
            engine = new Engine.Engine();
            move = new MoveCoord();
            IsPCMove = false;
            isSelected = false;
            newFigure = new Square(new NotFigur());
            InitializeComponent();
            NewChessBoard(true);
            IsInsert = false;
            IsStartGame = false;
        }

        /// <summary>
        /// Нарисовать новую доску
        /// </summary>
        /// <param name="isWhite"></param>
        public void NewChessBoard(bool isWhite)
        {
            gridBoard.Children.Clear();
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Button g = new Button();
                    g.Name = "button" + (7 - x).ToString() + y; 
                    g.Click += ClickChBut;
                    g.Template = (ControlTemplate)FindResource("chbTemplate");
                    //g.Style = (Style)FindResource();
                    g.Background = new ImageBrush(new BitmapImage(new Uri(@"Resourses\NOT.png", UriKind.Relative)));
                    Grid b = new Grid();
                    b.Children.Add(g);
                    Grid.SetColumn(g, 0);
                    Grid.SetRow(g, 0);
                    if (x % 2 == 1)
                    {
                        if (y % 2 == 0)
                        {
                            var ib = new ImageBrush();
                            ib.ImageSource = new BitmapImage(new Uri(@"Resourses\B.png", UriKind.Relative));
                            b.Background = ib;
                        }
                        else
                        {
                            var ib = new ImageBrush();
                            ib.ImageSource = new BitmapImage(new Uri(@"Resourses\W.png", UriKind.Relative));
                            b.Background = ib;
                        }
                    }
                    else
                    {
                        if (y % 2 == 0)
                        {
                            var ib = new ImageBrush();
                            ib.ImageSource = new BitmapImage(new Uri(@"Resourses\W.png", UriKind.Relative));
                            b.Background = ib;
                        }
                        else
                        {
                            var ib = new ImageBrush();
                            ib.ImageSource = new BitmapImage(new Uri(@"Resourses\B.png", UriKind.Relative));
                            b.Background = ib;
                        }
                    }
                    IsWhite = isWhite;
                    b.Name = "button" + (7 - x).ToString() + y;
                    if (IsWhite)
                    {
                        gridBoard.Children.Add(b);
                        Grid.SetColumn(b, y);
                        Grid.SetRow(b, x);
                    }
                    else
                    {
                        gridBoard.Children.Add(b);
                        Grid.SetColumn(b, 7 - y);
                        Grid.SetRow(b, 7 - x);
                    }
                }
            }
        }

        /// <summary>
        /// Переворот доски
        /// </summary>
        public void Flip()
        {
            IsWhite = !IsWhite;
            NewChessBoard(IsWhite);
            UpdatePosition();
        }

        /// <summary>
        /// Получить координаты кнопки
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public Coord GetCoordFromButton(Button b)
        {
            Coord c = new Coord();
            string buf = "";
            buf = b.Name.Replace("button", "");
            c.x = Convert.ToInt32(buf[0].ToString());
            c.y = Convert.ToInt32(buf[1].ToString());
            return c;

        }

        /// <summary>
        /// Получить кнопку из координат
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public string GetButtonNameFromCoord(int x, int y)
        {
            StringBuilder buf = new StringBuilder("button");
            buf = buf.Append(x);
            buf = buf.Append(y);
            return buf.ToString();
        }

        /// <summary>
        /// Получить кнопку из координат
        /// </summary>
        public string GetButtonNameFromCoord(Coord c)
        {
            StringBuilder buf = new StringBuilder("button");
            buf = buf.Append(c.x);
            buf = buf.Append(c.y);
            return buf.ToString();
        }

        /// <summary>
        /// Изменить ихображение фигуры на клетке
        /// </summary>
        public void ChangeImage(Button b, int x, int y)
        {
            StringBuilder s = new StringBuilder(@"Resourses\");
            var imb = new Image();
            switch (GameLogic.GameLogic.GamePosition.Board[x, y].Figure.Color)
            {
                case ColorFigur.black: s = s.Append("B"); break;
                case ColorFigur.white: s = s.Append("W"); break;
            }
            switch (GameLogic.GameLogic.GamePosition.Board[x, y].Figure.Type)
            {
                case TypeFigur.bishop:
                    s = s.Append("B.png"); break;
                case TypeFigur.king: s = s.Append("K.png"); break;
                case TypeFigur.knight: s = s.Append("N.png"); break;
                case TypeFigur.rock: s = s.Append("R.png"); break;
                case TypeFigur.peen: s = s.Append("P.png"); break;
                case TypeFigur.queen: s = s.Append("Q.png"); break;
                case TypeFigur.none: s = new StringBuilder(@"Resourses\NOT.png");break;
            }
            b.Background = new ImageBrush(new BitmapImage(new Uri(s.ToString(), UriKind.Relative)));
        }

        /// <summary>
        /// Обновляет позицию
        /// </summary>
        public void UpdatePosition()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    foreach (UIElement g in gridBoard.Children)
                    {
                        if (g is Grid)
                        {
                            foreach (UIElement b in ((Grid)g).Children)
                            {
                                if (b is Button)
                                {
                                    if (GetButtonNameFromCoord(i, j) == ((Button)b).Name)
                                    {
                                        ChangeImage((Button)b, i, j);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Событие нажатия на клетку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickChBut(object sender, RoutedEventArgs e)
        {
            if (IsPCMove) return; //Если ход компьютера - никакой реакции

            //При редактировании фигур 
            if (IsInsert)
            {
                if (newFigure.Figure.Type == TypeFigur.king) //Проверяем, что в случае установке короля на доску он будет один
                {
                    foreach (Square s in GameLogic.GameLogic.GamePosition.Board)
                    {
                        if (s.Figure.Type == TypeFigur.king && newFigure.Figure.Color == s.Figure.Color) return;
                    }
                }
                Coord c = GetCoordFromButton((Button)e.OriginalSource);
                GameLogic.GameLogic.GamePosition.Board[c.x, c.y] = new Square(newFigure.Figure);
                UpdatePosition();
            }
            else
            {
                //Если фигура не выбрана
                if (!isSelected)
                {
                    foreach (Grid g in gridBoard.Children)
                    {
                        if (g.Name == ((Button)e.OriginalSource).Name)
                        {
                            #region//Если нажали на клетку с фигурой - выделение клетки и получение координат
                            if (g.Background is ImageBrush)
                            {
                                int x = Convert.ToInt32(g.Name[6].ToString());
                                int y = Convert.ToInt32(g.Name[7].ToString());
                                
                                if (GameLogic.GameLogic.GamePosition.Board[x, y] != new Square(new NotFigur()))
                                {
                                    g.Background = new SolidColorBrush(Colors.Aqua);
                                    isSelected = true;
                                    move.StartFigure = GameLogic.GameLogic.GamePosition.Board[x, y].Figure;
                                    move.xStart = x;
                                    move.yStart = y;
                                    break;
                                }
                            }
                            #endregion

                            #region//Если нажали на клетку с уже выделенной фигурой - отменить выделение
                            else
                            {
                                isSelected = false;
                                move = new MoveCoord();
                                int x = Convert.ToInt32(g.Name[6].ToString());
                                int y = Convert.ToInt32(g.Name[7].ToString());
                                if (x % 2 == 1)
                                {
                                    if (y % 2 == 0)
                                    {
                                        var ib = new ImageBrush();
                                        ib.ImageSource = new BitmapImage(new Uri(@"Resourses\B.png", UriKind.Relative));
                                        g.Background = ib;
                                    }
                                    else
                                    {
                                        var ib = new ImageBrush();
                                        ib.ImageSource = new BitmapImage(new Uri(@"Resourses\W.png", UriKind.Relative));
                                        g.Background = ib;
                                    }
                                }
                                else
                                {
                                    if (y % 2 == 0)
                                    {
                                        var ib = new ImageBrush();
                                        ib.ImageSource = new BitmapImage(new Uri(@"Resourses\W.png", UriKind.Relative));
                                        g.Background = ib;
                                    }
                                    else
                                    {
                                        var ib = new ImageBrush();
                                        ib.ImageSource = new BitmapImage(new Uri(@"Resourses\B.png", UriKind.Relative));
                                        g.Background = ib;
                                    }
                                }
                                break;
                            }
                            #endregion
                        }
                    }
                }
                else
                {
                    //Gлучаем конечные координаты и делаем ход
                    isSelected = false;
                    move.IsCastling = false;
                    foreach (Grid g in gridBoard.Children)
                    {
                        if (!(g.Background is ImageBrush))
                        {
                            int x = Convert.ToInt32(g.Name[6].ToString());
                            int y = Convert.ToInt32(g.Name[7].ToString());
                            if (x % 2 == 0)
                            {
                                if (y % 2 == 0)
                                {
                                    var ib = new ImageBrush();
                                    ib.ImageSource = new BitmapImage(new Uri(@"Resourses\B.png", UriKind.Relative));
                                    g.Background = ib;
                                }
                                else
                                {
                                    var ib = new ImageBrush();
                                    ib.ImageSource = new BitmapImage(new Uri(@"Resourses\W.png", UriKind.Relative));
                                    g.Background = ib;
                                }
                            }
                            else
                            {
                                if (y % 2 == 0)
                                {
                                    var ib = new ImageBrush();
                                    ib.ImageSource = new BitmapImage(new Uri(@"Resourses\W.png", UriKind.Relative));
                                    g.Background = ib;
                                }
                                else
                                {
                                    var ib = new ImageBrush();
                                    ib.ImageSource = new BitmapImage(new Uri(@"Resourses\B.png", UriKind.Relative));
                                    g.Background = ib;
                                }
                            }
                        }
                        if (g.Name == ((Button)e.OriginalSource).Name)
                        {
                            int x = Convert.ToInt32(g.Name[6].ToString());
                            int y = Convert.ToInt32(g.Name[7].ToString());
                            if (x == move.xStart && y == move.yStart)
                            {
                                move = new MoveCoord();
                                return;
                            }
                            //Получаем конечные координаты
                            move.xEnd = x;
                            move.yEnd = y;
                            move.NewFigure = move.StartFigure;

                            //Если игра началась, то ходим по правилам
                            if (IsStartGame)
                            {
                                //Делаем ход
                                if (GameLogic.GameLogic.TryMoved(move))
                                {
                                    //Переключаем очередь хода для ИИ, если играем с ПК
                                    if (IsWithPC) IsPCMove = !IsPCMove;

                                    // Если был ход пешкой на последнюю горизонталь - открыть окно выбора фигуры
                                    #region
                                    if (GameLogic.GameLogic.GamePosition.Board[x, y].Figure.Type == TypeFigur.peen)
                                    {
                                        if (GameLogic.GameLogic.GamePosition.Board[x, y].Figure.Color == ColorFigur.white && x == 7)
                                        {
                                            GameLogic.GameLogic.TimeBlack.Stop();
                                            GameLogic.GameLogic.TimeWhite.Start();
                                            WindowConvertPeen wp = new WindowConvertPeen();
                                            wp.ShowDialog();
                                            
                                            if ((bool)wp.DialogResult)
                                            {
                                                GameLogic.GameLogic.GamePosition.Board[x, y] = new Square(wp.Figure.Figure);
                                                GameLogic.GameLogic.GamePosition.Board[x, y].Figure.Color = ColorFigur.white;
                                            }
                                            else GameLogic.GameLogic.GamePosition.Board[x, y] = new Square(new Queen(ColorFigur.white));
                                            GameLogic.GameLogic.TimeBlack.Start();
                                            GameLogic.GameLogic.TimeWhite.Stop();
                                        }
                                        else if (GameLogic.GameLogic.GamePosition.Board[x, y].Figure.Color == ColorFigur.black && x == 0)
                                        {
                                            GameLogic.GameLogic.TimeBlack.Start();
                                            GameLogic.GameLogic.TimeWhite.Stop();
                                            WindowConvertPeen wp = new WindowConvertPeen();
                                            wp.ShowDialog();
                                            if ((bool)wp.DialogResult)
                                            {
                                                GameLogic.GameLogic.GamePosition.Board[x, y] = new Square(wp.Figure.Figure);
                                                GameLogic.GameLogic.GamePosition.Board[x, y].Figure.Color = ColorFigur.black;
                                            }
                                            else GameLogic.GameLogic.GamePosition.Board[x, y] = new Square(new Queen(ColorFigur.black));
                                            GameLogic.GameLogic.TimeBlack.Stop();
                                            GameLogic.GameLogic.TimeWhite.Start();
                                        }
                                    }
                                    #endregion
                                    // Проверить позицию на наличие пата или мата
                                    int? isWin = GameLogic.GameLogic.CheckPosition(GameLogic.GameLogic.GamePosition);
                                    if (GameLogic.GameLogic.BlackWin || isWin < 0 && isWin != null)
                                    {
                                        MessageBox.Show("BlackWin");
                                    }
                                    else if (GameLogic.GameLogic.WhiteWin || isWin > 0)
                                    {
                                        MessageBox.Show("WhiteWin");
                                    }
                                    else if (isWin == 0)
                                    {
                                        MessageBox.Show("Ничья");
                                    }
                                    // Звук, уведомляющий о перемещении фигуры
                                    Console.Beep(240, 300);
                                }
                            }
                            //Если мы вводим позицию (не играем), то просто переместить фигуру
                            else
                            {
                                #region
                                GameLogic.GameLogic.GamePosition.Move(move);
                                if (GameLogic.GameLogic.GamePosition.Board[x, y].Figure.Type == TypeFigur.peen)
                                {
                                    if (GameLogic.GameLogic.GamePosition.Board[x, y].Figure.Color == ColorFigur.white && x == 7)
                                    {
                                        WindowConvertPeen wp = new WindowConvertPeen();
                                        wp.ShowDialog();
                                        if ((bool)wp.DialogResult)
                                        {
                                            GameLogic.GameLogic.GamePosition.Board[x, y] = new Square(wp.Figure.Figure);
                                        }
                                        else GameLogic.GameLogic.GamePosition.Board[x, y] = new Square(new Queen(ColorFigur.white));
                                    }
                                    else if (GameLogic.GameLogic.GamePosition.Board[x, y].Figure.Color == ColorFigur.black && x == 0)
                                    {
                                        WindowConvertPeen wp = new WindowConvertPeen();
                                        wp.ShowDialog();
                                        if ((bool)wp.DialogResult)
                                        {
                                            GameLogic.GameLogic.GamePosition.Board[x, y] = new Square(wp.Figure.Figure);
                                        }
                                        else GameLogic.GameLogic.GamePosition.Board[x, y] = new Square(new Queen(ColorFigur.black));
                                    }
                                }
                                #endregion
                            }
                        }
                    }
                }
                UpdatePosition();

                PCMove(); // Передать ход ПК 
            }
        }

        private void PCMove()
        {
            // Проверка, что игра не окончена и она идет с ПК
            if (IsPCMove && IsStartGame && IsWithPC)
            {
                if (GameLogic.GameLogic.BlackWin || GameLogic.GameLogic.WhiteWin || GameLogic.GameLogic.GamePosition.IsMat
                    || GameLogic.GameLogic.GamePosition.IsPat) return;

                //Запуск поиска хода за ПК
                Task.Run(() =>
                {
                    if (GameLogic.GameLogic.Moves.Count == 0) engine = new Engine.Engine();
                    int? myK = engine.SearchInTime(GameLogic.GameLogic.GamePosition);
                    MoveCoord mc = new MoveCoord();
                    mc.xStart = -1;
                    if (engine.MovesWithScore.Count > 0)
                        mc = engine.MovesWithScore.Where(vpm => vpm != null).Where(vpm => vpm.Score == myK).FirstOrDefault().MC;
                    if (mc.xStart != -1)
                    {
                        bool b = GameLogic.GameLogic.TryMoved(mc);
                        if (!b) mc = engine.SearchAllLegalMoves(GameLogic.GameLogic.GamePosition).First(); // Если движок не присал ходов, сделать первый возможный
                        if (mc != null) GameLogic.GameLogic.TryMoved(mc);
                        if (GameLogic.GameLogic.GamePosition.Board[mc.xEnd, mc.yEnd].Figure.Type == TypeFigur.peen && (mc.xEnd == 0 || mc.xEnd == 7))
                        {
                            GameLogic.GameLogic.GamePosition.Board[mc.xEnd, mc.yEnd] = new Square(new Queen(GameLogic.GameLogic.GamePosition.Board[mc.xEnd, mc.yEnd].Figure.Color));
                        }
                        IsPCMove = !IsPCMove;
                    }

                    //Проверка на наличие мата или пата
                    Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (ThreadStart)delegate ()
                    {
                        UpdatePosition();
                        int? isWin = GameLogic.GameLogic.CheckPosition(GameLogic.GameLogic.GamePosition);

                        if (GameLogic.GameLogic.BlackWin || isWin < 0 && isWin != null)
                        {
                            MessageBox.Show("Чёрные победили!");
                            Engine.Engine.IsStopSearch = true;
                        }
                        else if (GameLogic.GameLogic.WhiteWin || isWin > 0)
                        {
                            MessageBox.Show("Белые победили!");
                            Engine.Engine.IsStopSearch = true;
                        }
                        else if (isWin == 0 || GameLogic.GameLogic.MovesWithoutEating >= 50)
                        {
                            if (!(GameLogic.GameLogic.MovesWithoutEating > 49)) MessageBox.Show("Ничья");
                            Engine.Engine.IsStopSearch = true;
                        }
                        // Звук о совершенном ходе
                        Console.Beep(240, 300);

                        //Запуск поиска ходов в режиме хода игрока
                        if (!Engine.Engine.IsStopSearch)
                        {
                            Task.Run(() =>
                            {
                                engine.SearchInTime(GameLogic.GameLogic.GamePosition, 1, true);
                            });
                        }
                        else Engine.Engine.IsStopSearch = false;
                    });

                });
                return;
            }
        }
    }
}
