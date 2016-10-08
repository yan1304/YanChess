using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YanChess.Engine
{
    /// <summary>
    /// Настройки движка
    /// </summary>
    public static class EngineOptions
    {
        /// <summary>
        /// Максимальная глубина поиска
        /// </summary>
        public static uint MaxDepth { get; set; }
        /// <summary>
        /// Истинное значение отключает позиционные критерии оценки позиции для движка
        /// </summary>
        public static bool IsUseEasyScoreOfPosition { get; set; }
        /// <summary>
        /// Истинное значение - перевод движка в мультипоточный режим
        /// </summary>
        public static bool IsMultithread { get; set; }
        /// <summary>
        /// Истинное значение - движок сохраняет оценки позиций в словарь, используя их в дальнейшем.
        /// Словарь при закрытии программы удаляется
        /// </summary>
        public static bool IsUsePositionDictionary { get; set; }
    }
}
