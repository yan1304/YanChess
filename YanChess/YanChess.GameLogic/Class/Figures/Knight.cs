using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YanChess.GameLogic
{
    [Serializable]
    public class Knight : Figure
    {
        private Knight()
        {
        }
        /// <summary>
        /// Создать коня
        /// </summary>
        /// <param name="color"></param>
        public Knight(ColorFigur color):base(TypeFigur.knight, color)
        {
            if (color == ColorFigur.none) Color = ColorFigur.white;
        }
        /// <summary>
        /// проверка возможности хода для фигуры
        /// </summary>
        public override bool CheckMove(Position position, MoveCoord mc)
        {
            bool isLegal = false;
            if (position.Board[mc.xEnd, mc.yEnd].Figure.Color == position.Board[mc.xStart, mc.yStart].Figure.Color) return false;
            if (Math.Abs(mc.xEnd - mc.xStart) == 2)
            {
                if (Math.Abs(mc.yEnd - mc.yStart) == 1)
                {
                    isLegal = true;
                }
                else return false;
            }
            else if (Math.Abs(mc.xEnd - mc.xStart) == 1)
            {
                if (Math.Abs(mc.yEnd - mc.yStart) == 2)
                {
                    isLegal = true;
                }
                else return false;
            }
            else return false;
            if (position.IsWhiteMove)
            {
                if (position.Board[mc.xStart, mc.yStart].Figure.Color == ColorFigur.black) return false;
            }
            else
            {
                if (position.Board[mc.xStart, mc.yStart].Figure.Color == ColorFigur.white) return false;
            }

            if (isLegal)
            {
                //проверка на отсутствие шаха королю после хода
                position.MoveChess(mc);
                //черным
                isLegal = IsHaventCheck(position);
            }

            position.MoveBack(mc);
            return isLegal;
        }
    }
}