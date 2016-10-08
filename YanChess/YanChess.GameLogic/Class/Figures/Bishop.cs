using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YanChess.GameLogic
{
    [Serializable]
    public class Bishop : Figure
    {
        private Bishop()
        {
        }
        /// <summary>
        /// Создать слона
        /// </summary>
        /// <param name="color"></param>
        public Bishop(ColorFigur color): base(TypeFigur.bishop, color)
        {
            if (color == ColorFigur.none) Color = ColorFigur.white;
        }

        /// <summary>
        /// проверка возможности хода для фигуры
        /// </summary>
        public override bool CheckMove(Position position, MoveCoord mc)
        {
            bool isLegal = true;
            if (position.Board[mc.xEnd, mc.yEnd].Figure.Color == position.Board[mc.xStart, mc.yStart].Figure.Color) return false;
            if ((position.Board[mc.xStart, mc.yStart].Figure.Color == ColorFigur.white && position.IsWhiteMove)
                || (position.Board[mc.xStart, mc.yStart].Figure.Color == ColorFigur.black && !(position.IsWhiteMove)))
            {
                if (Math.Abs(mc.xStart - mc.xEnd) != Math.Abs(mc.yEnd - mc.yStart)) return false;
                for (int x = 1; x < 8; x++)
                {
                    if (mc.xEnd > mc.xStart)
                    {
                        if ((mc.xStart + x <= 7) && (mc.xStart + x < mc.xEnd))
                        {
                            if (mc.yEnd > mc.yStart)
                            {
                                if ((mc.yStart + x <= 7) && (mc.yStart + x < mc.yEnd))
                                {
                                    if (position.Board[mc.xStart + x, mc.yStart + x].Figure.Type != TypeFigur.none)
                                    {
                                        isLegal = false;
                                        goto l1;
                                    }
                                }
                            }
                            else
                            {
                                if ((mc.yStart - x >= 0) && (mc.yStart - x > mc.yEnd))
                                {
                                    if (position.Board[mc.xStart + x, mc.yStart - x].Figure.Type != TypeFigur.none)
                                    {
                                        isLegal = false;
                                        goto l1;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if ((mc.xStart - x >= 0) && (mc.xStart - x > mc.xEnd))
                        {
                            if (mc.yEnd > mc.yStart)
                            {
                                if ((mc.yStart + x <= 7) && (mc.yStart + x < mc.yEnd))
                                {
                                    if (position.Board[mc.xStart - x, mc.yStart + x].Figure.Type != TypeFigur.none)
                                    {
                                        isLegal = false;
                                        goto l1;
                                    }
                                }
                            }
                            else
                            {
                                if ((mc.yStart - x >= 0) && (mc.yStart - x > mc.yEnd))
                                {
                                    if (position.Board[mc.xStart - x, mc.yStart - x].Figure.Type != TypeFigur.none)
                                    {
                                        isLegal = false;
                                        goto l1;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else return false;

            l1: if (isLegal)
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