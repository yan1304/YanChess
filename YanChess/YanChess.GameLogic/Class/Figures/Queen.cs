using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YanChess.GameLogic
{
    [Serializable]
    public class Queen : Figure
    {
        private Queen()
        {
        }
        /// <summary>
        /// Создать ферзя
        /// </summary>
        /// <param name="color"></param>
        public Queen(ColorFigur color) : base(TypeFigur.queen, color)
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
                if (Math.Abs(mc.xStart - mc.xEnd) == Math.Abs(mc.yEnd - mc.yStart))
                {
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
                                            break;
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
                                            break;
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
                                            break;
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
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (mc.yEnd == mc.yStart)
                    {
                        for (int x = 1; x < 8; x++)
                        {
                            if (((mc.xStart + x) < mc.xEnd) && ((mc.xStart + x) < 8))
                            {
                                if (position.Board[mc.xStart + x, mc.yEnd].Figure.Type != TypeFigur.none)
                                {
                                    isLegal = false;
                                    break;
                                }
                            }
                            else if (((mc.xStart - x) > mc.xEnd) && ((mc.xStart - x) >= 0))
                            {
                                if (position.Board[mc.xStart - x, mc.yEnd].Figure.Type != TypeFigur.none)
                                {
                                    isLegal = false;
                                    break;
                                }
                            }
                        }
                    }
                    else if (mc.xEnd == mc.xStart)
                    {
                        for (int x = 1; x < 8; x++)
                        {
                            if (((mc.yStart + x) < mc.yEnd) && ((mc.yStart + x) < 8))
                            {
                                if (position.Board[mc.xStart, mc.yStart + x].Figure.Type != TypeFigur.none)
                                {
                                    isLegal = false;
                                    break;
                                }
                            }
                            else if (((mc.yStart - x) > mc.yEnd) && ((mc.yStart - x) >= 0))
                            {
                                if (position.Board[mc.xStart, mc.yStart - x].Figure.Type != TypeFigur.none)
                                {
                                    isLegal = false;
                                    break;
                                }
                            }
                        }
                    }
                    else return false;
                }
            }
            else return false;

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