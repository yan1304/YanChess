using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YanChess.GameLogic
{
    /// <summary>
    /// Класс единичного хода
    /// </summary>
    public class MoveCoord : IComparable
    {
        public MoveCoord()
        {
            StartFigure = new NotFigur();
            EndFigure = new NotFigur();
            NewFigure = new NotFigur();
        }
        public int xStart { get; set; }

        public int xEnd { get; set; }

        public int yStart { get; set; }

        public int yEnd { get; set; }
        public bool IsKingParamChange { get; set; }
        public bool IsRockParamChange { get; set; }
        /// <summary>
        /// Фигура, стоявшая на xStart, yStart до хода
        /// </summary>
        public Figure StartFigure { get; set; }

        /// <summary>
        /// Фигура, стоящая на клетке с координатами xEnd, yEnd до хода
        /// </summary>
        public Figure EndFigure { get; set; }

        /// <summary>
        /// Фигура стоящая на xEnd, yEnd после хода
        /// </summary>
        public Figure NewFigure { get; set; }

        /// <summary>
        /// Была ли рокировка
        /// </summary>
        public bool IsCastling { get; set; }
        /// <summary>
        /// Было ли взятие на проходе
        /// </summary>
        public bool IsEnPassant { get; set; }
        public bool HavePassant { get; internal set; }
        public int xPassant { get; internal set; }
        public int yPassant { get; internal set; }


        /// <summary>
        /// Для сортировки ходов (в приоритете взятия, ходы фигурами, пешки на 8 горизонталь и потом близость к центру)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            //if (Score == ((Move)obj).Score) return 0;
            //if (Score > ((Move)obj).Score) return 1;
            //else return -1;

            int leftAttack = ScoreInFigur(this);
            int rightAttack = ScoreInFigur((MoveCoord)obj);
            if (leftAttack > rightAttack) return -1;
            if (rightAttack > leftAttack) return 1;
            else return 0;
        }

        private int ScoreInFigur(MoveCoord mc)
        {
            int d = 0;
            switch (mc.EndFigure.Type)
            {
                case TypeFigur.king: d += 60;break;
                case TypeFigur.bishop: d += 30;break;
                case TypeFigur.knight: d += 31; break;
                case TypeFigur.peen: d += 10; break;
                case TypeFigur.queen: d += 90; break;
                case TypeFigur.rock: d += 50; break;
            }
            switch (mc.StartFigure.Type)
            {
                case TypeFigur.king: d += 6; break;
                case TypeFigur.bishop: d += 3; break;
                case TypeFigur.knight: d += 3; break;
                case TypeFigur.peen: d += 1; break;
                case TypeFigur.queen: d += 9; break;
                case TypeFigur.rock: d += 5; break;
            }
            if (mc.StartFigure.Type == TypeFigur.peen && mc.NewFigure.Type == TypeFigur.queen) d += 100;
            d += Math.Min(7 - mc.xEnd, xEnd); //первыми лучше проверять фигуры в центре
            d += Math.Min(7 - mc.yEnd, yEnd); 
            return d;
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if(xStart == ((MoveCoord)obj).xStart && yStart == ((MoveCoord)obj).yStart 
                && yEnd == ((MoveCoord)obj).yEnd && xEnd == ((MoveCoord)obj).xEnd)
            {
                return true;
            }
            return false;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            int b = IsEnPassant ? 1 : 0;
            return xEnd*10000+yEnd*1000+xStart*100+yStart*10+b;
        }
    }
}