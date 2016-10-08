using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YanChess.GameLogic
{
    [Serializable]
    public class NotFigur : Figure
    {
        /// <summary>
        /// Создать без фигуры
        /// </summary>
        public NotFigur():base()
        {
        }
        /// <summary>
        /// проверка возможности хода для фигуры
        /// </summary>
        public override bool CheckMove(Position position, MoveCoord mc)
        {
            return false;
        }
    }
}