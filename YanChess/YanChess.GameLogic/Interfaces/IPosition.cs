using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YanChess.GameLogic
{
    public interface IPosition
    {
        Square[,] Board { get; set; }
        bool IsMat { get; set; }
        bool IsPat { get; set; }
        bool IsWhiteMove { get; set; }

        IPosition DeepCopy();
        void Move(MoveCoord move);
    }
}