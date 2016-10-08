using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace YanChess.GameLogic
{
    [Serializable]
    public class Position : IPosition
    {
        /// <summary>
        /// Создать пустую позицию
        /// </summary>
        public Position() : this("default")
        {
        }
        /// <summary>
        /// Создать позицию по опции ( default - пустую, start - стартовую)
        /// </summary>
        /// <param name="opt"></param>
        public Position(string opt)
        {
            opt = opt.ToLower();
            switch (opt)
            {
                case "start":
                    IsWhiteMove = true;
                    Board = new Square[8, 8];
                    for (int i = 0; i < 8; i++)
                    {
                        if (i > 1 && i < 6)
                        {
                            for (int j = 0; j < 8; j++)
                            {
                                Board[i, j] = new Square(new NotFigur());
                            }
                        }
                        else if (i == 1)
                        {
                            for (int j = 0; j < 8; j++)
                            {
                                Board[i, j] = new Square(new Peen(ColorFigur.white));
                            }
                        }
                        else if (i == 6)
                        {
                            for (int j = 0; j < 8; j++)
                            {
                                Board[i, j] = new Square(new Peen(ColorFigur.black));
                            }
                        }
                        else if (i == 0)
                        {
                            for (int j = 0; j < 8; j++)
                            {
                                if (j == 0 || j == 7)
                                {
                                    Board[i, j] = new Square(new Rock(ColorFigur.white));
                                }
                                else if (j == 1 || j == 6)
                                {
                                    Board[i, j] = new Square(new Knight(ColorFigur.white));
                                }
                                else if (j == 2 || j == 5)
                                {
                                    Board[i, j] = new Square(new Bishop(ColorFigur.white));
                                }
                                else if (j == 3)
                                {
                                    Board[i, j] = new Square(new Queen(ColorFigur.white));
                                }
                                else
                                {
                                    Board[i, j] = new Square(new King(ColorFigur.white));
                                }
                            }
                        }
                        else if (i == 7)
                        {
                            for (int j = 0; j < 8; j++)
                            {
                                if (j == 0 || j == 7)
                                {
                                    Board[i, j] = new Square(new Rock(ColorFigur.black));
                                }
                                else if (j == 1 || j == 6)
                                {
                                    Board[i, j] = new Square(new Knight(ColorFigur.black));
                                }
                                else if (j == 2 || j == 5)
                                {
                                    Board[i, j] = new Square(new Bishop(ColorFigur.black));
                                }
                                else if (j == 3)
                                {
                                    Board[i, j] = new Square(new Queen(ColorFigur.black));
                                }
                                else
                                {
                                    Board[i, j] = new Square(new King(ColorFigur.black));
                                }
                            }
                        }
                    }
                    break;
                case "default":
                default:
                    Board = new Square[8, 8];
                    for(int i = 0;i<8;i++)
                    { 
                        for(int j=0;j<8;j++)
                        { 
                            Board[i, j] = new Square(new NotFigur());
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Стоит ли пат
        /// </summary>
        public bool IsPat { get; set; }

        /// <summary>
        /// Стоит ли мат
        /// </summary>
        public bool IsMat { get; set; }

        /// <summary>
        /// Ход белых?
        /// </summary>
        public bool IsWhiteMove { get; set; }

        /// <summary>
        /// Расположение фигур на доске
        /// </summary>
        public Square[,] Board { get; set; }

        /// <summary>
        /// Ходила ли белая левая ладья
        /// </summary>
        public bool IsLeftRockMovedWhite { get; set; }

        /// <summary>
        /// Ходила ли белая правая ладья
        /// </summary>
        public bool IsRightRockMovedWhite { get; set; }

        /// <summary>
        /// Ходил ли белый король
        /// </summary>
        public bool IsKingMovedWhite { get; set; }

        /// <summary>
        /// Ходила ли черная левая ладья
        /// </summary>
        public bool IsLeftRockMovedBlack { get; set; }

        /// <summary>
        /// Ходила ли черная правая ладья
        /// </summary>
        public bool IsRightRockMovedBlack { get; set; }

        /// <summary>
        /// Ходил ли черный король
        /// </summary>
        public bool IsKingMovedBlack { get; set; }

        /// <summary>
        /// Глубокая копия позиции
        /// </summary>
        public IPosition DeepCopy()
        {
            Position p = new Position();
            p.IsKingMovedBlack = IsKingMovedBlack;
            p.IsKingMovedWhite = IsKingMovedWhite;
            p.IsLeftRockMovedBlack = IsLeftRockMovedBlack;
            p.IsLeftRockMovedWhite = IsLeftRockMovedWhite;
            p.IsMat = IsMat;
            p.IsPat = IsPat;
            p.IsRightRockMovedBlack = IsRightRockMovedBlack;
            p.IsRightRockMovedWhite = IsRightRockMovedWhite;
            p.IsWhiteMove = IsWhiteMove;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    p.Board[i, j] = (Square)Board[i, j].Clone();
                }
            }
            return p;
        }

        /// <summary>
        /// Сделать ход (независимо от правил)
        /// </summary>
        public void Move(MoveCoord move)
        {
            move.StartFigure = Board[move.xStart, move.yStart].Figure;
            move.EndFigure = Board[move.xEnd, move.yEnd].Figure;
            if (move.NewFigure.Type == TypeFigur.none) move.NewFigure = move.StartFigure;
            Board[move.xStart, move.yStart] = new Square(new NotFigur());
            Board[move.xEnd, move.yEnd] = new Square(move.NewFigure);
        }

        /// <summary>
        /// Откатить ход
        /// </summary>
        public void MoveBack(MoveCoord move)
        {
            Board[move.xEnd, move.yEnd] = new Square(move.EndFigure);
            Board[move.xStart, move.yStart] = new Square(move.StartFigure);
            if (move.StartFigure.Color == ColorFigur.white) IsWhiteMove = true;
            else IsWhiteMove = false;
            if (move.IsEnPassant)
            {
                if (move.NewFigure.Color == ColorFigur.white)
                {
                    Board[move.xStart, move.yEnd] = new Square(new Peen(ColorFigur.black));
                }
                else
                {
                    Board[move.xStart, move.yEnd] = new Square(new Peen(ColorFigur.white));
                }
                ((Peen)Board[move.xStart, move.yEnd].Figure).IsEnPassant = true;
            }
            if (move.HavePassant && Board[move.xPassant, move.yPassant].Figure.Type == TypeFigur.peen)
            {

                ((Peen)Board[move.xPassant, move.yPassant].Figure).IsEnPassant = true;
            }
            if (move.IsCastling)
            {
                if (move.NewFigure.Color == ColorFigur.white)
                {
                    if (move.yEnd == 6 && move.xEnd == 0)
                    {
                        Board[move.xEnd, 7] = new Square(new Rock(ColorFigur.white));
                        Board[move.xEnd, 5] = new Square(new NotFigur());
                        IsRightRockMovedWhite = false;
                    }
                    else if (move.yEnd == 2 && move.xEnd == 0)
                    {
                        Board[move.xEnd, 0] = new Square(new Rock(ColorFigur.white));
                        Board[move.xEnd, 3] = new Square(new NotFigur());
                        IsLeftRockMovedWhite = false;
                    }
                    IsKingMovedWhite = false;
                }
                else
                {
                    if (move.yEnd == 6 && move.xEnd == 7)
                    {
                        Board[move.xEnd, 7] = new Square(new Rock(ColorFigur.black));
                        Board[move.xEnd, 5] = new Square(new NotFigur());
                        IsRightRockMovedBlack = false;
                    }
                    else if (move.yEnd == 2 && move.xEnd == 7)
                    {
                        Board[move.xEnd, 0] = new Square(new Rock(ColorFigur.black));
                        Board[move.xEnd, 3] = new Square(new NotFigur());
                        IsLeftRockMovedBlack = false;
                    }
                    IsKingMovedBlack = false;
                }
            }
            if (move.IsKingParamChange)
            {
                if (move.StartFigure.Color == ColorFigur.white) IsKingMovedWhite = false;
                else IsKingMovedBlack = false;
            }
            if (move.IsRockParamChange)
            {
                if (move.StartFigure.Color == ColorFigur.white)
                {
                    if (move.yStart == 7)
                    {
                        IsRightRockMovedWhite = false;
                    }
                    else
                    {
                        IsLeftRockMovedWhite = false;
                    }
                }
                else
                {
                    if (move.yStart == 7)
                    {
                        IsRightRockMovedBlack = false;
                    }
                    else
                    {
                        IsLeftRockMovedBlack = false;
                    }
                }
            }
        }

        /// <summary>
        /// Ход с правилами т.е. с переключением хода, рокировкой и т.п.
        /// </summary>
        /// <param name="move"></param>
        public void MoveChess(MoveCoord move)
        {
            move.EndFigure = Board[move.xEnd, move.yEnd].Figure;
            if (Board[move.xStart, move.yStart].Figure.Type == TypeFigur.peen)
            {
                if (Board[move.xStart, move.yStart].Figure.Color == ColorFigur.white)
                {
                    if (Math.Abs(move.yStart - move.yEnd) == 1 && Math.Abs(move.xStart - move.yEnd) == 1)
                    {
                        if (Board[move.xStart, move.yEnd].Figure.Color == ColorFigur.black && Board[move.xStart, move.yEnd].Figure.Type == TypeFigur.peen)
                        {
                            if (((Peen)Board[move.xStart, move.yEnd].Figure).IsEnPassant)
                            {
                                move.IsEnPassant = true;
                            }
                        }
                    }
                }
                else
                {
                    if (Math.Abs(move.yStart - move.yEnd) == 1 && Math.Abs(move.xStart - move.yEnd) == 1)
                    {
                        if (Board[move.xStart, move.yEnd].Figure.Color == ColorFigur.white && Board[move.xStart, move.yEnd].Figure.Type == TypeFigur.peen)
                        {
                            if (((Peen)Board[move.xStart, move.yEnd].Figure).IsEnPassant)
                            {
                                move.IsEnPassant = true;
                            }
                        }
                    }
                }
            }
            //перемещаем фигуру
            Move(move);
            
            //убираем возможность взятий на проходе если такие были
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (Board[i, j].Figure.Type == TypeFigur.peen && Board[i, j].Figure.Color != move.StartFigure.Color)
                    {
                        if (((Peen)Board[i, j].Figure).IsEnPassant)
                        {
                            ((Peen)Board[i, j].Figure).IsEnPassant = false;
                            move.HavePassant = true;
                            move.xPassant = i;
                            move.yPassant = j;
                            break;
                        }
                    }
                }
            }
            //для короля
            if (move.StartFigure.Type == TypeFigur.king)
            {
                move.IsKingParamChange = true;
                if (move.StartFigure.Color == ColorFigur.white)
                {
                    IsKingMovedWhite = true;//король ходил
                    //при рокировке перемещаем ладью
                    if (move.IsCastling)
                    {
                        if (move.yEnd == 6)
                        {
                            Board[0, 7] = new Square(new NotFigur());
                            Board[0, 5] = new Square(new Rock(ColorFigur.white));
                            move.IsRockParamChange = true;
                        }
                        else
                        {
                            Board[0, 0] = new Square(new NotFigur());
                            Board[0, 3] = new Square(new Rock(ColorFigur.white));
                            move.IsRockParamChange = true;
                        }
                    }
                }
                else
                {
                    //Эквивалентные действия для черного короля
                    IsKingMovedBlack = true;
                    if (move.IsCastling)
                    {
                        if (move.yEnd == 6)
                        {
                            Board[7, 7] = new Square(new NotFigur());
                            Board[7, 5] = new Square(new Rock(ColorFigur.black));
                            move.IsRockParamChange = true;
                        }
                        else
                        {
                            Board[7, 0] = new Square(new NotFigur());
                            Board[7, 3] = new Square(new Rock(ColorFigur.black));
                            move.IsRockParamChange = true;
                        }
                    }
                }
            }
            else if (move.StartFigure.Type == TypeFigur.rock)
            {
                move.IsRockParamChange = true;
                //Для ладьи указать, что она двигалась (при попытке рокировки это будет учтено)
                if (move.xStart == 0 && move.StartFigure.Color == ColorFigur.white)
                {
                    if (move.yStart == 0)
                    {
                        IsLeftRockMovedWhite = true;
                    }
                    else if (move.yStart == 7)
                    {
                        IsRightRockMovedWhite = true;
                    }
                }
                else if (move.xStart == 7 && move.StartFigure.Color == ColorFigur.black)
                {
                    if (move.yStart == 0)
                    {
                        IsLeftRockMovedBlack = true;
                    }
                    else if (move.yStart == 7)
                    {
                        IsRightRockMovedBlack = true;
                    }
                }
            }
            else if (move.StartFigure.Type == TypeFigur.peen)
            {
                //для пешки нужно реализовать взятие на проходе и возможность взятия данной пешки на проходе
                if (move.IsEnPassant)
                {
                    //взятие на проходе
                    if (Board[move.xStart, move.yEnd].Figure.Type == TypeFigur.peen &&
                        Board[move.xStart, move.yEnd].Figure.Color != move.StartFigure.Color)
                    {
                        Board[move.xStart, move.yEnd] = new Square(new NotFigur());
                    }
                    else move.IsEnPassant = false;
                }
                else if (Math.Abs(move.xStart - move.xEnd) == 2)
                {
                    if (move.yStart == move.yEnd && ((move.StartFigure.Color == ColorFigur.white && move.xStart == 1) || (move.StartFigure.Color == ColorFigur.black && move.xStart == 6)))
                    {
                        ((Peen)Board[move.xEnd, move.yEnd].Figure).IsEnPassant = true;//пешка сделала ход через две клетки, т.е. ее можно взять на проходе (если есть чем)
                    }
                }
            }
            IsWhiteMove = !IsWhiteMove;//смена очереди хода
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            if (IsWhiteMove != ((Position)obj).IsWhiteMove) return false;
            if (IsKingMovedBlack != ((Position)obj).IsKingMovedBlack) return false;
            if (IsKingMovedWhite != ((Position)obj).IsKingMovedWhite) return false;
            if (!IsKingMovedBlack)
            {
                if (IsLeftRockMovedBlack != ((Position)obj).IsLeftRockMovedBlack) return false;
                if (IsRightRockMovedBlack != ((Position)obj).IsRightRockMovedBlack) return false;
            }
            if (!IsKingMovedWhite)
            {
                if (IsLeftRockMovedWhite != ((Position)obj).IsLeftRockMovedWhite) return false;
                if (IsRightRockMovedWhite != ((Position)obj).IsRightRockMovedWhite) return false;
            }
            bool isStop = false;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (!(Board[i, j].Equals(((Position)obj).Board[i, j])))
                    {
                        isStop = true;
                        break;
                    }
                }
            }
            return !isStop;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            int Coll = 0;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    switch (Board[i, j].Figure.Type)
                    {
                        case TypeFigur.none:
                            if (Board[i,j].Figure.Color == ColorFigur.white)
                            {
                                Coll += i * j * j;
                            }
                            else
                            {
                                Coll += i * j * j;
                            }
                            break;
                        case TypeFigur.king:
                            if (Board[i,j].Figure.Color == ColorFigur.white)
                            {
                                Coll += 21 * i * j * j;
                            }
                            else
                            {
                                Coll += 221 * i * j * j;
                            }
                            break;
                        case TypeFigur.queen:
                            if (Board[i,j].Figure.Color == ColorFigur.white)
                            {
                                Coll += 31 * i * j * j;
                            }
                            else
                            {
                                Coll += 331 * i * j * j;
                            }
                            break;
                        case TypeFigur.rock:
                            if (Board[i,j].Figure.Color == ColorFigur.white)
                            {
                                Coll += 41 * i * j * j;
                            }
                            else
                            {
                                Coll += 441 * i * j * j;
                            }
                            break;
                        case TypeFigur.bishop:
                            if (Board[i,j].Figure.Color == ColorFigur.white)
                            {
                                Coll += 51 * i * j * j;
                            }
                            else
                            {
                                Coll += 551 * i * j * j;
                            }
                            break;
                        case TypeFigur.knight:
                            if (Board[i,j].Figure.Color == ColorFigur.white)
                            {
                                Coll += 61 * i * j * j;
                            }
                            else
                            {
                                Coll += 661 * i * j * j;
                            }
                            break;
                        case TypeFigur.peen:
                            if (Board[i,j].Figure.Color == ColorFigur.white)
                            {
                                if (((Peen)Board[i, j].Figure).IsEnPassant) Coll *= 10;
                                Coll += 71 * i * j * j;
                            }
                            else
                            {
                                Coll += 771 * i * j * j;
                            }
                            break;
                    }
                }
            }
            if (IsWhiteMove) Coll = Coll + 98765;
            if (IsRightRockMovedWhite) Coll += Coll / 10;
            if (IsRightRockMovedBlack) Coll += Coll % 23000+43354;
            if (IsLeftRockMovedWhite) Coll += Coll % 10000 + 19486;
            if (IsLeftRockMovedBlack) Coll += Coll % 20000 + 54545;
            if (IsKingMovedWhite) Coll += Coll % 5000 + 99987;
            if (IsKingMovedBlack) Coll += Coll % 9898 + 48574;
            return Coll;
        }
    }
}