using System;

namespace YanChess.Engine
{
    /// <summary>
    /// Класс применяется для записи позиций в словарь
    /// </summary>
    [Serializable]
    public class ScoreDepth
    {
        /// <summary>
        /// Вес позиции
        /// </summary>
        public int Score { get; set; }
        /// <summary>
        /// Глубина рассчета, при которой получен вес хода
        /// </summary>
        public int Depth { get; set; }


        public ScoreDepth()
        {
        }

        /// <summary>
        /// Класс применяется для записи позиций в словарь
        /// </summary>
        /// <param name="score">Вес позиции</param>
        /// <param name="depth">Глубина рассчета, при которой получен вес хода</param>
        public ScoreDepth(int score, int depth)
        {
            Score = score;
            Depth = depth;
        }
    }


}
