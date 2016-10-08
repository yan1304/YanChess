using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YanChess.GameLogic
{
    [Serializable]
    public class King : Figure
    {
        private King()
        {
        }
        /// <summary>
        /// Создать короля
        /// </summary>
        /// <param name="color"></param>
        public King(ColorFigur color) : base(TypeFigur.king, color)
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
            //проверка рокировки
            if (position.Board[mc.xStart, mc.yStart].Figure.Color == ColorFigur.white && position.IsWhiteMove)
            {
                if (mc.xStart == 0 && mc.xEnd == 0 && mc.yStart == 4 && (mc.yEnd == 6 || mc.yEnd == 2))
                {
                    if (position.IsKingMovedWhite) return false;
                    if (mc.yEnd == 6)
                    {
                        if (position.Board[mc.xEnd, 7].Figure.Type != TypeFigur.rock && position.Board[mc.xEnd, 7].Figure.Color != ColorFigur.white) return false;
                        if (position.IsRightRockMovedWhite) return false;
                        AttackChecker.CheckAttack(ref position, 0, 4, ColorFigur.black);
                        AttackChecker.CheckAttack(ref position, 0, 5, ColorFigur.black);
                        AttackChecker.CheckAttack(ref position, 0, 6, ColorFigur.black);
                        if (position.Board[0, 4].IsAttackBlack || position.Board[0, 5].IsAttackBlack || position.Board[0, 6].IsAttackBlack 
                            || position.Board[0, 5].Figure.Type != TypeFigur.none || position.Board[0, 6].Figure.Type != TypeFigur.none)
                        {
                            position.Board[0, 4].IsAttackBlack = false;
                            position.Board[0, 5].IsAttackBlack = false;
                            position.Board[0, 6].IsAttackBlack = false;
                            return false;
                        }
                        else
                        {
                            position.Board[0, 4].IsAttackBlack = false;
                            position.Board[0, 5].IsAttackBlack = false;
                            position.Board[0, 6].IsAttackBlack = false;
                            mc.IsCastling = true;
                            isLegal = true;
                        }
                    }
                    else
                    {
                        if (position.Board[mc.xEnd, 0].Figure.Type != TypeFigur.rock && position.Board[mc.xEnd, 0].Figure.Color != ColorFigur.white) return false;
                        if (position.IsLeftRockMovedWhite) return false;
                        AttackChecker.CheckAttack(ref position, 0, 4, ColorFigur.black);
                        AttackChecker.CheckAttack(ref position, 0, 3, ColorFigur.black);
                        AttackChecker.CheckAttack(ref position, 0, 2, ColorFigur.black);
                        if (position.Board[0, 4].IsAttackBlack || position.Board[0, 3].IsAttackBlack || position.Board[0, 2].IsAttackBlack 
                            || position.Board[0, 2].Figure.Type != TypeFigur.none || position.Board[0, 3].Figure.Type != TypeFigur.none)
                        {
                            position.Board[0, 4].IsAttackBlack = false;
                            position.Board[0, 3].IsAttackBlack = false;
                            position.Board[0, 2].IsAttackBlack = false;
                            return false;
                        }
                        else
                        {
                            position.Board[0, 4].IsAttackBlack = false;
                            position.Board[0, 3].IsAttackBlack = false;
                            position.Board[0, 2].IsAttackBlack = false;
                            mc.IsCastling = true;
                            isLegal = true;
                        }
                    }
                }
                else
                {
                    if (Math.Abs(mc.xEnd - mc.xStart) <= 1)
                    {
                        if (Math.Abs(mc.yEnd - mc.yStart) <= 1)
                        {
                            AttackChecker.CheckAttack(ref position, mc.xEnd, mc.yEnd, ColorFigur.black);
                            if (position.Board[mc.xEnd, mc.yEnd].IsAttackBlack)
                            {
                                position.Board[mc.xEnd, mc.yEnd].IsAttackBlack = false;
                                return false;
                            }
                            else isLegal = true;
                        }
                        else return false;
                    }
                    else return false;
                }
            }
            else if ((!position.IsWhiteMove) && position.Board[mc.xStart, mc.yStart].Figure.Color == ColorFigur.black)
            {
                if (mc.xStart == 7 && mc.xEnd == 7 && mc.yStart == 4 && (mc.yEnd == 6 || mc.yEnd == 2))
                {
                    if (position.IsKingMovedBlack) return false;
                    if (mc.yEnd == 6)
                    {
                        if (position.Board[mc.xEnd, 7].Figure.Type != TypeFigur.rock && position.Board[mc.xEnd, 7].Figure.Color != ColorFigur.black) return false;
                        if (position.IsRightRockMovedBlack) return false;
                        AttackChecker.CheckAttack(ref position, 7, 4, ColorFigur.white);
                        AttackChecker.CheckAttack(ref position, 7, 5, ColorFigur.white);
                        AttackChecker.CheckAttack(ref position, 7, 6, ColorFigur.white);
                        if (position.Board[7, 4].IsAttackWhite || position.Board[7, 5].IsAttackWhite || position.Board[7, 6].IsAttackWhite
                            || position.Board[7, 5].Figure.Type != TypeFigur.none || position.Board[7, 6].Figure.Type != TypeFigur.none)
                        {
                            position.Board[7, 4].IsAttackWhite = false;
                            position.Board[7, 5].IsAttackWhite = false;
                            position.Board[7, 6].IsAttackWhite = false;
                            return false;
                        }
                        else
                        {
                            position.Board[7, 4].IsAttackWhite = false;
                            position.Board[7, 5].IsAttackWhite = false;
                            position.Board[7, 6].IsAttackWhite = false;
                            mc.IsCastling = true;
                            isLegal = true;
                        }
                    }
                    else
                    {
                        if (position.Board[mc.xEnd, 0].Figure.Type != TypeFigur.rock && position.Board[mc.xEnd, 0].Figure.Color != ColorFigur.black) return false;
                        if (position.IsLeftRockMovedBlack) return false;
                        AttackChecker.CheckAttack(ref position, 7, 4, ColorFigur.white);
                        AttackChecker.CheckAttack(ref position, 7, 3, ColorFigur.white);
                        AttackChecker.CheckAttack(ref position, 7, 2, ColorFigur.white);
                        if (position.Board[7, 4].IsAttackWhite || position.Board[7, 3].IsAttackWhite || position.Board[7, 2].IsAttackWhite
                            || position.Board[7, 2].Figure.Type != TypeFigur.none || position.Board[7, 3].Figure.Type != TypeFigur.none)
                        {
                            position.Board[7, 4].IsAttackWhite = false;
                            position.Board[7, 3].IsAttackWhite = false;
                            position.Board[7, 2].IsAttackWhite = false;
                            return false;
                        }
                        else
                        {
                            position.Board[7, 4].IsAttackWhite = false;
                            position.Board[7, 3].IsAttackWhite = false;
                            position.Board[7, 2].IsAttackWhite = false;
                            mc.IsCastling = true;
                            isLegal = true;
                        }
                    }
                }
                else
                {
                    if (Math.Abs(mc.xEnd - mc.xStart) <= 1)
                    {
                        if (Math.Abs(mc.yEnd - mc.yStart) <= 1)
                        {
                            AttackChecker.CheckAttack(ref position, mc.xEnd, mc.yEnd, ColorFigur.white);
                            if (position.Board[mc.xEnd, mc.yEnd].IsAttackWhite)
                            {
                                position.Board[mc.xEnd, mc.yEnd].IsAttackWhite = false;
                                return false;
                            }
                            else isLegal = true;
                        }
                        else return false;
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