using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YanChess.GameLogic;

namespace YanChess.Engine
{
    /// <summary>
    /// Класс, позволяющий движку работать с таймером
    /// </summary>
    internal static class EngineTime
    {
        private static Position gamePosition; //Для сравнения с игровой позицией

        /// <summary>
        /// Максимальное время поиска
        /// </summary>
        public static TimeSpan MaxTimeForSearching { get; set; }
        /// <summary>
        /// Секундомер, для определения - сколько времени уже прошло
        /// </summary>
        public static Stopwatch Watch { get; set; }
        /// <summary>
        /// Таймер, используемый для генерации события проверки CheckTime (вышло ли время, или изменилась ли позиция)
        /// </summary>
        public static System.Timers.Timer Timer { get; set; }

        /// <summary>
        /// Запускает таймер с событием, проверяющем вышло ли время на поиск или изменилась ли позиция
        /// </summary>
        public static void TimerStart()
        {
            gamePosition = (Position)GameLogic.GameLogic.GamePosition.DeepCopy();
            Timer = new System.Timers.Timer(100);
            Timer.Enabled = true;
            Timer.Elapsed += CheckTime;
        }
        /// <summary>
        /// Проверяет не вышло ли время на поиск, или не изменилась ли позиция. 
        /// При истине останавливает поиск
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void CheckTime(object sender, EventArgs e)
        {
            if (Watch.Elapsed > MaxTimeForSearching || !gamePosition.Equals(GameLogic.GameLogic.GamePosition))
            {
                Engine.IsStopSearch = true;
                Watch.Stop();
                Timer.Stop();
            }
        }

    }
}
