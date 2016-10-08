using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YanChess.GameLogic
{
    [Serializable]
    public class Square:ICloneable
    {
        /// <summary>
        /// Фигура на данной клетке
        /// </summary>
        public Figure Figure { get; set; }

        /// <summary>
        /// Атакована ли клетка черными
        /// </summary>
        public bool IsAttackBlack { get; set; }

        /// <summary>
        /// Атакована ли клетка белыми
        /// </summary>
        public bool IsAttackWhite { get; set; }

        /// <summary>
        /// Пустая клетка
        /// </summary>
        public Square()
        {
            Figure = new NotFigur();
            IsAttackBlack = IsAttackWhite = false;
        }
        /// <summary>
        /// Клетка с фигурой
        /// </summary>
        /// <param name="figur"></param>
        public Square(Figure figur)
        {
            Figure = figur;
            IsAttackBlack = IsAttackWhite = false;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return (((Square)obj).Figure.Equals(Figure));
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public object Clone()
        {
            Square s = new Square();
            switch(Figure.Type)
            {
                case TypeFigur.king:
                    s.Figure = new King(Figure.Color);
                    break;
                case TypeFigur.queen:
                    s.Figure = new Queen(Figure.Color);
                    break;
                case TypeFigur.rock:
                    s.Figure = new Rock(Figure.Color);
                    break;
                case TypeFigur.bishop:
                    s.Figure = new Bishop(Figure.Color);
                    break;
                case TypeFigur.knight:
                    s.Figure = new Knight(Figure.Color);
                    break;
                case TypeFigur.peen:
                    s.Figure = new Peen(Figure.Color);
                    ((Peen)s.Figure).IsEnPassant = ((Peen)Figure).IsEnPassant;
                    break;
                case TypeFigur.none:
                    s.Figure = new NotFigur();
                    break;
            }
            s.IsAttackBlack = IsAttackBlack;
            s.IsAttackWhite = IsAttackWhite;
            return s;
        }

        public static bool operator ==(Square a, Square b)
        {
            return (a.Figure==b.Figure);
        }
        public static bool operator !=(Square a, Square b)
        {
            return !(a == b);
        }
    }
}