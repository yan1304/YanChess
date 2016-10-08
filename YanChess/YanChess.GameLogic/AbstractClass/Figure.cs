using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YanChess.GameLogic
{
    [Serializable]
    public abstract class Figure
    {

        public ColorFigur Color { get; set; }

        public TypeFigur Type { get; set; }
        /// <summary>
        /// Конструктор для пустой клетки (нет фигуры)
        /// </summary>
        public Figure():this(TypeFigur.none,ColorFigur.none)
        {
        }
        /// <summary>
        /// Конструктор фигуры с указанием ее типа и цвета
        /// </summary>
        /// <param name="type"></param>
        /// <param name="color"></param>
        public Figure(TypeFigur type, ColorFigur color)
        {
            Color = color;
            Type = type;
        }

        /// <summary>
        /// проверка возможности хода для фигуры
        /// </summary>
        abstract public bool CheckMove(Position position, MoveCoord mc);

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return ((((Figure)obj).Type==Type)&& (((Figure)obj).Color==Color));
        }
        public static bool operator ==(Figure a, Figure b)
        {
            return (a.Type == b.Type && a.Color == b.Color);
        }
        public static bool operator !=(Figure a, Figure b)
        {
            return !(a == b);
        }
        // override object.GetHashCode
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Проверка стоит ли шах королю после хода
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        protected bool IsHaventCheck(Position position)
        {
            //белым
            if (!position.IsWhiteMove)
            {
                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        if (position.Board[x, y] == new Square(new King(ColorFigur.white)))
                        {
                            AttackChecker.CheckAttack(ref position, x, y, ColorFigur.black);
                            if (position.Board[x, y].IsAttackBlack)
                            {
                                position.Board[x, y].IsAttackBlack = false;
                                return false;
                            }
                        }
                    }
                }
            }
            //черным
            else
            {
                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        if (position.Board[x, y] == new Square(new King(ColorFigur.black)))
                        {
                            AttackChecker.CheckAttack(ref position, x, y, ColorFigur.white);
                            if (position.Board[x, y].IsAttackWhite)
                            {
                                position.Board[x, y].IsAttackWhite = false;
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }
    }
}