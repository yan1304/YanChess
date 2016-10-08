using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YanChess.GameLogic
{
    /// <summary>
    /// Статический класс, содержащий методы проверки атак для клеток 
    /// </summary>
    public static class AttackChecker
    {
        /// <summary>
        /// Проверяет какие клетки атакованы
        /// </summary>
        public static void CheckAttack(ref Position p)
        {
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    CheckAttack(ref p, x, y, ColorFigur.none);
                }
            }
        }
        /// <summary>
        /// Атакована ли данная клетка
        /// </summary>
        public static void CheckAttack(ref Position pos, int x, int y, ColorFigur color)
        {
            if (color == ColorFigur.none)
            {
                CheckAttack(ref pos, x, y, ColorFigur.white);
                CheckAttack(ref pos, x, y, ColorFigur.black);
            }
            if (color == ColorFigur.white)
            {
                //атака пешкой
                if (x > 0)
                {
                    if (y > 0)
                    {
                        if (pos.Board[x - 1, y - 1].Figure.Type == TypeFigur.peen && pos.Board[x - 1, y - 1].Figure.Color == ColorFigur.white)
                        {
                            pos.Board[x, y].IsAttackWhite = true;
                            return;
                        }
                    }
                    if (y < 7)
                    {
                        if (pos.Board[x - 1, y + 1].Figure.Type == TypeFigur.peen && pos.Board[x - 1, y + 1].Figure.Color == ColorFigur.white)
                        {
                            pos.Board[x, y].IsAttackWhite = true;
                            return;
                        }
                    }
                }
                //конем
                #region
                if (x + 1 < 8)
                {
                    if (x + 2 < 8)
                    {
                        if (y + 1 < 8)
                        {
                            if (pos.Board[x + 2, y + 1].Figure.Type == TypeFigur.knight && pos.Board[x + 2, y + 1].Figure.Color == ColorFigur.white)
                            {
                                pos.Board[x, y].IsAttackWhite = true;
                                return;
                            }
                        }
                        if (y - 1 >= 0)
                        {
                            if (pos.Board[x + 2, y - 1].Figure.Type == TypeFigur.knight && pos.Board[x + 2, y - 1].Figure.Color == ColorFigur.white)
                            {
                                pos.Board[x, y].IsAttackWhite = true;
                                return;
                            }
                        }
                    }
                    if (y + 2 < 8)
                    {
                        if (pos.Board[x + 1, y + 2].Figure.Type == TypeFigur.knight && pos.Board[x + 1, y + 2].Figure.Color == ColorFigur.white)
                        {
                            pos.Board[x, y].IsAttackWhite = true;
                            return;
                        }
                    }
                    if (y - 2 >= 0)
                    {
                        if (pos.Board[x + 1, y - 2].Figure.Type == TypeFigur.knight && pos.Board[x + 1, y - 2].Figure.Color == ColorFigur.white)
                        {
                            pos.Board[x, y].IsAttackWhite = true;
                            return;
                        }
                    }
                }
                if (x - 1 >= 0)
                {
                    if (x - 2 >= 0)
                    {
                        if (y + 1 < 8)
                        {
                            if (pos.Board[x - 2, y + 1].Figure.Type == TypeFigur.knight && pos.Board[x - 2, y + 1].Figure.Color == ColorFigur.white)
                            {
                                pos.Board[x, y].IsAttackWhite = true;
                                return;
                            }
                        }
                        if (y - 1 >= 0)
                        {
                            if (pos.Board[x - 2, y - 1].Figure.Type == TypeFigur.knight && pos.Board[x - 2, y - 1].Figure.Color == ColorFigur.white)
                            {
                                pos.Board[x, y].IsAttackWhite = true;
                                return;
                            }
                        }
                    }
                    if (y + 2 < 8)
                    {
                        if (pos.Board[x - 1, y + 2].Figure.Type == TypeFigur.knight && pos.Board[x - 1, y + 2].Figure.Color == ColorFigur.white)
                        {
                            pos.Board[x, y].IsAttackWhite = true;
                            return;
                        }
                    }
                    if (y - 2 >= 0)
                    {
                        if (pos.Board[x - 1, y - 2].Figure.Type == TypeFigur.knight && pos.Board[x - 1, y - 2].Figure.Color == ColorFigur.white)
                        {
                            pos.Board[x, y].IsAttackWhite = true;
                            return;
                        }
                    }
                }
                #endregion
                //слон, ладья, ферзь, король
                #region
                for (int i = 1; x + i < 8; i++)
                {
                    if (pos.Board[x + i, y] != new Square(new NotFigur()))
                    {
                        if (i == 1)
                        {
                            if (pos.Board[x + i, y] == new Square(new King(ColorFigur.white)))
                            {
                                pos.Board[x, y].IsAttackWhite = true;
                                return;
                            }
                            if (pos.Board[x + i, y] == new Square(new Queen(ColorFigur.white)))
                            {
                                pos.Board[x, y].IsAttackWhite = true;
                                return;
                            }
                            if (pos.Board[x + i, y] == new Square(new Rock(ColorFigur.white)))
                            {
                                pos.Board[x, y].IsAttackWhite = true;
                                return;
                            }
                        }
                        if ((pos.Board[x + i, y] != new Square(new Queen(ColorFigur.white))) && (pos.Board[x + i, y] != new Square(new Rock(ColorFigur.white)))) break;
                        else
                        {
                            pos.Board[x, y].IsAttackWhite = true;
                            return;
                        }
                    }
                }
                for (int i = 1; x - i >= 0; i++)
                {
                    if (pos.Board[x - i, y] != new Square(new NotFigur()))
                    {
                        if (i == 1)
                        {
                            if (pos.Board[x - i, y] == new Square(new King(ColorFigur.white)))
                            {
                                pos.Board[x, y].IsAttackWhite = true;
                                return;
                            }
                            if (pos.Board[x - i, y] == new Square(new Queen(ColorFigur.white)))
                            {
                                pos.Board[x, y].IsAttackWhite = true;
                                return;
                            }
                            if (pos.Board[x - i, y] == new Square(new Rock(ColorFigur.white)))
                            {
                                pos.Board[x, y].IsAttackWhite = true;
                                return;
                            }
                        }
                        if ((pos.Board[x - i, y] != new Square(new Queen(ColorFigur.white))) && (pos.Board[x - i, y] != new Square(new Rock(ColorFigur.white)))) break;
                        else
                        {
                            pos.Board[x, y].IsAttackWhite = true;
                            return;
                        }
                    }
                }
                for (int i = 1; y + i < 8; i++)
                {
                    if (pos.Board[x, y + i] != new Square(new NotFigur()))
                    {
                        if (i == 1)
                        {
                            if (pos.Board[x, y + i] == new Square(new King(ColorFigur.white)))
                            {
                                pos.Board[x, y].IsAttackWhite = true;
                                return;
                            }
                            if (pos.Board[x, y + i] == new Square(new Queen(ColorFigur.white)))
                            {
                                pos.Board[x, y].IsAttackWhite = true;
                                return;
                            }
                            if (pos.Board[x, y + i] == new Square(new Rock(ColorFigur.white)))
                            {
                                pos.Board[x, y].IsAttackWhite = true;
                                return;
                            }
                        }
                        if ((pos.Board[x, y + i] != new Square(new Queen(ColorFigur.white))) && (pos.Board[x, y + i] != new Square(new Rock(ColorFigur.white)))) break;
                        else
                        {
                            pos.Board[x, y].IsAttackWhite = true;
                            return;
                        }
                    }
                }
                for (int i = 1; y - i >= 0; i++)
                {
                    if (pos.Board[x, y - i] != new Square(new NotFigur()))
                    {
                        if (i == 1)
                        {
                            if (pos.Board[x, y - i] == new Square(new King(ColorFigur.white)))
                            {
                                pos.Board[x, y].IsAttackWhite = true;
                                return;
                            }
                            if (pos.Board[x, y - i] == new Square(new Queen(ColorFigur.white)))
                            {
                                pos.Board[x, y].IsAttackWhite = true;
                                return;
                            }
                            if (pos.Board[x, y - i] == new Square(new Rock(ColorFigur.white)))
                            {
                                pos.Board[x, y].IsAttackWhite = true;
                                return;
                            }
                        }
                        if ((pos.Board[x, y - i] != new Square(new Queen(ColorFigur.white))) && (pos.Board[x, y - i] != new Square(new Rock(ColorFigur.white)))) break;
                        else
                        {
                            pos.Board[x, y].IsAttackWhite = true;
                            return;
                        }
                    }
                }
                #endregion
                bool b1 = false;
                bool b2 = false;
                bool b3 = false;
                bool b4 = false;
                for (int i = 1; ((x + i < 8) && (y + i < 8)) || ((x + i < 8) && (y - i >= 0)) || ((x - i >= 0) && (y + i < 8)) || ((x - i >= 0) && (y - i >= 0)); i++)
                {
                    if (!b1 && (x + i < 8) && (y + i < 8))
                    {
                        if (pos.Board[x + i, y + i] != new Square(new NotFigur()))
                        {
                            if (i == 1)
                            {
                                if (pos.Board[x + i, y + i] == new Square(new King(ColorFigur.white)))
                                {
                                    pos.Board[x, y].IsAttackWhite = true;
                                    return;
                                }
                                if (pos.Board[x + i, y + i] == new Square(new Queen(ColorFigur.white)))
                                {
                                    pos.Board[x, y].IsAttackWhite = true;
                                    return;
                                }
                                if (pos.Board[x + i, y + i] == new Square(new Bishop(ColorFigur.white)))
                                {
                                    pos.Board[x, y].IsAttackWhite = true;
                                    return;
                                }
                                b1 = true;
                            }
                            if (pos.Board[x + i, y + i] == new Square(new Queen(ColorFigur.white)))
                            {
                                pos.Board[x, y].IsAttackWhite = true;
                                return;
                            }
                            if (pos.Board[x + i, y + i] == new Square(new Bishop(ColorFigur.white)))
                            {
                                pos.Board[x, y].IsAttackWhite = true;
                                return;
                            }
                            b1 = true;
                        }
                    }
                    if (!b2 && (x + i < 8) && (y - i >= 0))
                    {
                        if (pos.Board[x + i, y - i] != new Square(new NotFigur()))
                        {
                            if (i == 1)
                            {
                                if (pos.Board[x + i, y - i] == new Square(new King(ColorFigur.white)))
                                {
                                    pos.Board[x, y].IsAttackWhite = true;
                                    return;
                                }
                                if (pos.Board[x + i, y - i] == new Square(new Queen(ColorFigur.white)))
                                {
                                    pos.Board[x, y].IsAttackWhite = true;
                                    return;
                                }
                                if (pos.Board[x + i, y - i] == new Square(new Bishop(ColorFigur.white)))
                                {
                                    pos.Board[x, y].IsAttackWhite = true;
                                    return;
                                }
                                b2 = true;
                            }
                            if (pos.Board[x + i, y - i] == new Square(new Queen(ColorFigur.white)))
                            {
                                pos.Board[x, y].IsAttackWhite = true;
                                return;
                            }
                            if (pos.Board[x + i, y - i] == new Square(new Bishop(ColorFigur.white)))
                            {
                                pos.Board[x, y].IsAttackWhite = true;
                                return;
                            }
                            b2 = true;
                        }
                    }
                    if (!b3 && (x - i >= 0) && (y + i < 8))
                    {
                        if (pos.Board[x - i, y + i] != new Square(new NotFigur()))
                        {
                            if (i == 1)
                            {
                                if (pos.Board[x - i, y + i] == new Square(new King(ColorFigur.white)))
                                {
                                    pos.Board[x, y].IsAttackWhite = true;
                                    return;
                                }
                                if (pos.Board[x - i, y + i] == new Square(new Queen(ColorFigur.white)))
                                {
                                    pos.Board[x, y].IsAttackWhite = true;
                                    return;
                                }
                                if (pos.Board[x - i, y + i] == new Square(new Bishop(ColorFigur.white)))
                                {
                                    pos.Board[x, y].IsAttackWhite = true;
                                    return;
                                }
                                b3 = true;
                            }
                            if (pos.Board[x - i, y + i] == new Square(new Queen(ColorFigur.white)))
                            {
                                pos.Board[x, y].IsAttackWhite = true;
                                return;
                            }
                            if (pos.Board[x - i, y + i] == new Square(new Bishop(ColorFigur.white)))
                            {
                                pos.Board[x, y].IsAttackWhite = true;
                                return;
                            }
                            b3 = true;
                        }
                    }
                    if (!b4 && (x - i >= 0) && (y - i >= 0))
                    {
                        if (pos.Board[x - i, y - i] != new Square(new NotFigur()))
                        {
                            if (i == 1)
                            {
                                if (pos.Board[x - i, y - i] == new Square(new King(ColorFigur.white)))
                                {
                                    pos.Board[x, y].IsAttackWhite = true;
                                    return;
                                }
                                if (pos.Board[x - i, y - i] == new Square(new Queen(ColorFigur.white)))
                                {
                                    pos.Board[x, y].IsAttackWhite = true;
                                    return;
                                }
                                if (pos.Board[x - i, y - i] == new Square(new Bishop(ColorFigur.white)))
                                {
                                    pos.Board[x, y].IsAttackWhite = true;
                                    return;
                                }
                                b4 = true;
                            }
                            if (pos.Board[x - i, y - i] == new Square(new Queen(ColorFigur.white)))
                            {
                                pos.Board[x, y].IsAttackWhite = true;
                                return;
                            }
                            if (pos.Board[x - i, y - i] == new Square(new Bishop(ColorFigur.white)))
                            {
                                pos.Board[x, y].IsAttackWhite = true;
                                return;
                            }
                            b4 = true;
                        }
                    }
                    if (b1 && b2 && b3 && b4)
                    {
                        break;
                    }
                }
            }
            else
            {
                //эквивалентно белым
                if (x < 7)
                {
                    if (y > 0)
                    {
                        if (pos.Board[x + 1, y - 1] == new Square(new Peen(ColorFigur.black)))
                        {
                            pos.Board[x, y].IsAttackBlack = true;
                            return;
                        }
                    }
                    if (y < 7)
                    {
                        if (pos.Board[x + 1, y + 1] == new Square(new Peen(ColorFigur.black)))
                        {
                            pos.Board[x, y].IsAttackBlack = true;
                            return;
                        }
                    }
                }
                //конем
                #region
                if (x + 1 < 8)
                {
                    if (x + 2 < 8)
                    {
                        if (y + 1 < 8)
                        {
                            if (pos.Board[x + 2, y + 1] == new Square(new Knight(ColorFigur.black)))
                            {
                                pos.Board[x, y].IsAttackBlack = true;
                                return;
                            }
                        }
                        if (y - 1 >= 0)
                        {
                            if (pos.Board[x + 2, y - 1] == new Square(new Knight(ColorFigur.black)))
                            {
                                pos.Board[x, y].IsAttackBlack = true;
                                return;
                            }
                        }
                    }
                    if (y + 2 < 8)
                    {
                        if (pos.Board[x + 1, y + 2] == new Square(new Knight(ColorFigur.black)))
                        {
                            pos.Board[x, y].IsAttackBlack = true;
                            return;
                        }
                    }
                    if (y - 2 >= 0)
                    {
                        if (pos.Board[x + 1, y - 2] == new Square(new Knight(ColorFigur.black)))
                        {
                            pos.Board[x, y].IsAttackBlack = true;
                            return;
                        }
                    }
                }
                if (x - 1 >= 0)
                {
                    if (x - 2 >= 0)
                    {
                        if (y + 1 < 8)
                        {
                            if (pos.Board[x - 2, y + 1] == new Square(new Knight(ColorFigur.black)))
                            {
                                pos.Board[x, y].IsAttackBlack = true;
                                return;
                            }
                        }
                        if (y - 1 >= 0)
                        {
                            if (pos.Board[x - 2, y - 1] == new Square(new Knight(ColorFigur.black)))
                            {
                                pos.Board[x, y].IsAttackBlack = true;
                                return;
                            }
                        }
                    }
                    if (y + 2 < 8)
                    {
                        if (pos.Board[x - 1, y + 2] == new Square(new Knight(ColorFigur.black)))
                        {
                            pos.Board[x, y].IsAttackBlack = true;
                            return;
                        }
                    }
                    if (y - 2 >= 0)
                    {
                        if (pos.Board[x - 1, y - 2] == new Square(new Knight(ColorFigur.black)))
                        {
                            pos.Board[x, y].IsAttackBlack = true;
                            return;
                        }
                    }
                }
                #endregion
                //слон, ладья, ферзь, король
                #region
                for (int i = 1; x + i < 8; i++)
                {
                    if (pos.Board[x + i, y] != new Square(new NotFigur()))
                    {
                        if (i == 1)
                        {
                            if (pos.Board[x + i, y] == new Square(new King(ColorFigur.black)))
                            {
                                pos.Board[x, y].IsAttackBlack = true;
                                return;
                            }
                            if (pos.Board[x + i, y] == new Square(new Queen(ColorFigur.black)))
                            {
                                pos.Board[x, y].IsAttackBlack = true;
                                return;
                            }
                            if (pos.Board[x + i, y] == new Square(new Rock(ColorFigur.black)))
                            {
                                pos.Board[x, y].IsAttackBlack = true;
                                return;
                            }
                        }
                        if ((pos.Board[x + i, y] != new Square(new Queen(ColorFigur.black))) && (pos.Board[x + i, y] != new Square(new Rock(ColorFigur.black)))) break;
                        else
                        {
                            pos.Board[x, y].IsAttackBlack = true;
                            return;
                        }
                    }
                }
                for (int i = 1; x - i >= 0; i++)
                {
                    if (pos.Board[x - i, y] != new Square(new NotFigur()))
                    {
                        if (i == 1)
                        {
                            if (pos.Board[x - i, y] == new Square(new King(ColorFigur.black)))
                            {
                                pos.Board[x, y].IsAttackBlack = true;
                                return;
                            }
                            if (pos.Board[x - i, y] == new Square(new Queen(ColorFigur.black)))
                            {
                                pos.Board[x, y].IsAttackBlack = true;
                                return;
                            }
                            if (pos.Board[x - i, y] == new Square(new Rock(ColorFigur.black)))
                            {
                                pos.Board[x, y].IsAttackBlack = true;
                                return;
                            }
                        }
                        if ((pos.Board[x - i, y] != new Square(new Queen(ColorFigur.black))) && (pos.Board[x - i, y] != new Square(new Rock(ColorFigur.black)))) break;
                        else
                        {
                            pos.Board[x, y].IsAttackBlack = true;
                            return;
                        }
                    }
                }
                for (int i = 1; y + i < 8; i++)
                {
                    if (pos.Board[x, y + i] != new Square(new NotFigur()))
                    {
                        if (i == 1)
                        {
                            if (pos.Board[x, y + i] == new Square(new King(ColorFigur.black)))
                            {
                                pos.Board[x, y].IsAttackBlack = true;
                                return;
                            }
                            if (pos.Board[x, y + i] == new Square(new Queen(ColorFigur.black)))
                            {
                                pos.Board[x, y].IsAttackBlack = true;
                                return;
                            }
                            if (pos.Board[x, y + i] == new Square(new Rock(ColorFigur.black)))
                            {
                                pos.Board[x, y].IsAttackBlack = true;
                                return;
                            }
                        }
                        if ((pos.Board[x, y + i] != new Square(new Queen(ColorFigur.black))) && (pos.Board[x, y + i] != new Square(new Rock(ColorFigur.black)))) break;
                        else
                        {
                            pos.Board[x, y].IsAttackBlack = true;
                            return;
                        }
                    }
                }
                for (int i = 1; y - i >= 0; i++)
                {
                    if (pos.Board[x, y - i] != new Square(new NotFigur()))
                    {
                        if (i == 1)
                        {
                            if (pos.Board[x, y - i] == new Square(new King(ColorFigur.black)))
                            {
                                pos.Board[x, y].IsAttackBlack = true;
                                return;
                            }
                            if (pos.Board[x, y - i] == new Square(new Queen(ColorFigur.black)))
                            {
                                pos.Board[x, y].IsAttackBlack = true;
                                return;
                            }
                            if (pos.Board[x, y - i] == new Square(new Rock(ColorFigur.black)))
                            {
                                pos.Board[x, y].IsAttackBlack = true;
                                return;
                            }
                        }
                        if ((pos.Board[x, y - i] != new Square(new Queen(ColorFigur.black))) && (pos.Board[x, y - i] != new Square(new Rock(ColorFigur.black)))) break;
                        else
                        {
                            pos.Board[x, y].IsAttackBlack = true;
                            return;
                        }
                    }
                }
                #endregion
                bool b1 = false;
                bool b2 = false;
                bool b3 = false;
                bool b4 = false;
                for (int i = 1; ((x + i < 8) && (y + i < 8)) || ((x + i < 8) && (y - i >= 0)) || ((x - i >= 0) && (y + i < 8)) || ((x - i >= 0) && (y - i >= 0)); i++)
                {
                    if (!b1 && (x + i < 8) && (y + i < 8))
                    {
                        if (pos.Board[x + i, y + i] != new Square(new NotFigur()))
                        {
                            if (i == 1)
                            {
                                if (pos.Board[x + i, y + i] == new Square(new King(ColorFigur.black)))
                                {
                                    pos.Board[x, y].IsAttackBlack = true;
                                    return;
                                }
                                if (pos.Board[x + i, y + i] == new Square(new Queen(ColorFigur.black)))
                                {
                                    pos.Board[x, y].IsAttackBlack = true;
                                    return;
                                }
                                if (pos.Board[x + i, y + i] == new Square(new Bishop(ColorFigur.black)))
                                {
                                    pos.Board[x, y].IsAttackBlack = true;
                                    return;
                                }
                                b1 = true;
                            }
                            if (pos.Board[x + i, y + i] == new Square(new Queen(ColorFigur.black)))
                            {
                                pos.Board[x, y].IsAttackBlack = true;
                                return;
                            }
                            if (pos.Board[x + i, y + i] == new Square(new Bishop(ColorFigur.black)))
                            {
                                pos.Board[x, y].IsAttackBlack = true;
                                return;
                            }
                            b1 = true;
                        }
                    }
                    if (!b2 && (x + i < 8) && (y - i >= 0))
                    {
                        if (pos.Board[x + i, y - i] != new Square(new NotFigur()))
                        {
                            if (i == 1)
                            {
                                if (pos.Board[x + i, y - i] == new Square(new King(ColorFigur.black)))
                                {
                                    pos.Board[x, y].IsAttackBlack = true;
                                    return;
                                }
                                if (pos.Board[x + i, y - i] == new Square(new Queen(ColorFigur.black)))
                                {
                                    pos.Board[x, y].IsAttackBlack = true;
                                    return;
                                }
                                if (pos.Board[x + i, y - i] == new Square(new Bishop(ColorFigur.black)))
                                {
                                    pos.Board[x, y].IsAttackBlack = true;
                                    return;
                                }
                                b2 = true;
                            }
                            if (pos.Board[x + i, y - i] == new Square(new Queen(ColorFigur.black)))
                            {
                                pos.Board[x, y].IsAttackBlack = true;
                                return;
                            }
                            if (pos.Board[x + i, y - i] == new Square(new Bishop(ColorFigur.black)))
                            {
                                pos.Board[x, y].IsAttackBlack = true;
                                return;
                            }
                            b2 = true;
                        }
                    }
                    if (!b3 && (x - i >= 0) && (y + i < 8))
                    {
                        if (pos.Board[x - i, y + i] != new Square(new NotFigur()))
                        {
                            if (i == 1)
                            {
                                if (pos.Board[x - i, y + i] == new Square(new King(ColorFigur.black)))
                                {
                                    pos.Board[x, y].IsAttackBlack = true;
                                    return;
                                }
                                if (pos.Board[x - i, y + i] == new Square(new Queen(ColorFigur.black)))
                                {
                                    pos.Board[x, y].IsAttackBlack = true;
                                    return;
                                }
                                if (pos.Board[x - i, y + i] == new Square(new Bishop(ColorFigur.black)))
                                {
                                    pos.Board[x, y].IsAttackBlack = true;
                                    return;
                                }
                                b3 = true;
                            }
                            if (pos.Board[x - i, y + i] == new Square(new Queen(ColorFigur.black)))
                            {
                                pos.Board[x, y].IsAttackBlack = true;
                                return;
                            }
                            if (pos.Board[x - i, y + i] == new Square(new Bishop(ColorFigur.black)))
                            {
                                pos.Board[x, y].IsAttackBlack = true;
                                return;
                            }
                            b3 = true;
                        }
                    }
                    if (!b4 && (x - i >= 0) && (y - i >= 0))
                    {
                        if (pos.Board[x - i, y - i] != new Square(new NotFigur()))
                        {
                            if (i == 1)
                            {
                                if (pos.Board[x - i, y - i] == new Square(new King(ColorFigur.black)))
                                {
                                    pos.Board[x, y].IsAttackBlack = true;
                                    return;
                                }
                                if (pos.Board[x - i, y - i] == new Square(new Queen(ColorFigur.black)))
                                {
                                    pos.Board[x, y].IsAttackBlack = true;
                                    return;
                                }
                                if (pos.Board[x - i, y - i] == new Square(new Bishop(ColorFigur.black)))
                                {
                                    pos.Board[x, y].IsAttackBlack = true;
                                    return;
                                }
                                b4 = true;
                            }
                            if (pos.Board[x - i, y - i] == new Square(new Queen(ColorFigur.black)))
                            {
                                pos.Board[x, y].IsAttackBlack = true;
                                return;
                            }
                            if (pos.Board[x - i, y - i] == new Square(new Bishop(ColorFigur.black)))
                            {
                                pos.Board[x, y].IsAttackBlack = true;
                                return;
                            }
                            b4 = true;
                        }
                    }
                    if (b1 && b2 && b3 && b4)
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Очищает список атак
        /// </summary>
        public static void CheckAttackClear(ref Position p)
        {
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (p.Board[x, y].IsAttackBlack) p.Board[x, y].IsAttackBlack = false;
                    if (p.Board[x, y].IsAttackWhite) p.Board[x, y].IsAttackWhite = false;
                }
            }
        }
    }
}
