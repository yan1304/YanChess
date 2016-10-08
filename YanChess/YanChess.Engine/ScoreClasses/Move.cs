using System;
using YanChess.GameLogic;

namespace YanChess.Engine
{
    /// <summary>
    /// Класс используемый для хранения ходов и их весов при расчетах
    /// </summary>
    public class Move : IComparable
    {
        /// <summary>
        /// Ход
        /// </summary>
        public MoveCoord MC { get; set; }

        /// <summary>
        /// Вес хода
        /// </summary>
        public int Score { get; set; }

        public Move()
        {
        }

        /// <summary>
        /// Класс используемый для хранения ходов и их весов при расчетах
        /// </summary>
        /// <param name="s">Вес хода</param>
        /// <param name="mc">Ход</param>
        public Move(int s, MoveCoord mc)
        {
            MC = mc;
            Score = s;
        }
        /// <summary>
        /// Для возможности сортировки ходов
        /// </summary>
        /// <param name="obj">Объект Move</param>
        /// <returns></returns>
        int IComparable.CompareTo(object obj)
        {
            if (Score == ((Move)obj).Score) return 0;
            if (Score > ((Move)obj).Score) return 1;
            else return -1;
        }
    }
}
