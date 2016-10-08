using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YanChess.GameLogic
{
    [Serializable]
    public class Peen : Figure
    {
        private Peen()
        {
        }

        /// <summary>
        /// Создать пешку
        /// </summary>
        /// <param name="color"></param>
        public Peen(ColorFigur color): base(TypeFigur.peen, color)
        {
            if (color == ColorFigur.none) Color = ColorFigur.white;
            IsEnPassant = false;
        }

        /// <summary>
        /// Может быть съедена на проходе
        /// </summary>
        public bool IsEnPassant { get; set; }

        /// <summary>
        /// проверка возможности хода для фигуры
        /// </summary>
        public override bool CheckMove(Position position, MoveCoord mc)
        {
            if (position.IsWhiteMove && mc.xEnd <= mc.xStart) return false;
            if (!position.IsWhiteMove && mc.xEnd >= mc.xStart) return false;
            bool isLegal = false;
            if (position.Board[mc.xEnd, mc.yEnd].Figure.Color == position.Board[mc.xStart, mc.yStart].Figure.Color) return false;
            if (position.Board[mc.xStart, mc.yStart].Figure.Color == ColorFigur.white && position.IsWhiteMove)
            {
                //ход пешкой вперед
                if (mc.yEnd == mc.yStart)
                {
                    if (mc.xStart == 1)
                    {
                        if (mc.xEnd == 3)
                        {
                            if (position.Board[mc.xEnd, mc.yEnd].Figure.Type == TypeFigur.none && position.Board[mc.xEnd - 1, mc.yEnd].Figure.Type == TypeFigur.none)
                            {
                                isLegal = true;
                            }
                            else return false;
                        }
                        else if (mc.xEnd == 2)
                        {
                            if (position.Board[mc.xEnd, mc.yEnd].Figure.Type == TypeFigur.none)
                            {
                                isLegal = true;
                            }
                            else return false;
                        }
                        else return false;
                    }
                    else
                    {
                        if (Math.Abs(mc.xEnd - mc.xStart) == 1)
                        {
                            if (position.Board[mc.xEnd, mc.yEnd].Figure.Type == TypeFigur.none)
                            {
                                isLegal = true;
                            }
                            else return false;
                        }
                        else return false;
                    }
                }
                else
                {
                    //Взятия
                    if (Math.Abs(mc.xEnd - mc.xStart) == 1)
                    {
                        if(Math.Abs(mc.yEnd-mc.yStart)==1)
                        {
                            if (position.Board[mc.xEnd, mc.yEnd].Figure.Color == ColorFigur.black)
                            {
                                isLegal = true;
                            }
                            else if (mc.xEnd - 1 >= 0)
                            {
                                if (position.Board[mc.xEnd - 1, mc.yEnd].Figure.Color == ColorFigur.black && position.Board[mc.xEnd - 1, mc.yEnd].Figure.Type == TypeFigur.peen)
                                {
                                    if (((Peen)position.Board[mc.xEnd - 1, mc.yEnd].Figure).IsEnPassant)
                                    {
                                        mc.IsEnPassant = true;
                                        isLegal = true;
                                    }
                                    else return false;
                                }
                                else return false;
                            }
                            else return false;
                        }
                    }
                    else return false;
                }
            }
            else if (position.Board[mc.xStart, mc.yStart].Figure.Color == ColorFigur.black && !(position.IsWhiteMove))
            {
                //ход пешкой вперед
                if (mc.yEnd == mc.yStart)
                {
                    if (mc.xStart == 6)
                    {
                        if (mc.xEnd == 4)
                        {
                            if (position.Board[mc.xEnd, mc.yEnd].Figure.Type == TypeFigur.none && position.Board[mc.xEnd + 1, mc.yEnd].Figure.Type == TypeFigur.none)
                            {
                                isLegal = true;
                            }
                            else return false;
                        }
                        else if (mc.xEnd == 5)
                        {
                            if (position.Board[mc.xEnd, mc.yEnd].Figure.Type == TypeFigur.none)
                            {
                                isLegal = true;
                            }
                            else return false;
                        }
                        else return false;
                    }
                    else
                    {
                        if (Math.Abs(mc.xEnd - mc.xStart) == 1)
                        {
                            if (position.Board[mc.xEnd, mc.yEnd].Figure.Type == TypeFigur.none)
                            {
                                isLegal = true;
                            }
                            else return false;
                        }
                        else return false;
                    }
                }
                else
                {
                    //Взятия
                    if (Math.Abs(mc.xEnd - mc.xStart) == 1)
                    {
                        if (Math.Abs(mc.yEnd - mc.yStart) == 1)
                        {
                            if (position.Board[mc.xEnd, mc.yEnd].Figure.Color == ColorFigur.white)
                            {
                                isLegal = true;
                            }
                            else if (mc.xEnd + 1 < 8)
                            {
                                if (position.Board[mc.xEnd + 1, mc.yEnd].Figure.Color == ColorFigur.white && position.Board[mc.xEnd + 1, mc.yEnd].Figure.Type == TypeFigur.peen)
                                {
                                    if (((Peen)position.Board[mc.xEnd + 1, mc.yEnd].Figure).IsEnPassant)
                                    {
                                        mc.IsEnPassant = true;
                                        isLegal = true;
                                    }
                                    else return false;
                                }
                                else return false;
                            }
                            else return false;
                        }
                    }
                    else return false;
                }
            }
            else return false;
            if(isLegal)
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