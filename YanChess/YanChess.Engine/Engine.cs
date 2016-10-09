using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YanChess.GameLogic;
using System.Diagnostics;

namespace YanChess.Engine
{
    /// <summary>
    /// Основной класс для компьютерного игрока.
    /// Он оценивает позицию, выбирает лучший ход за компьютер.
    /// </summary>
    public class Engine
    {
        private int flexDepth; // для вычисления максимального значения, которое может принять глубина 
                               // во время увелечения глубины перебора в методе SearchInGlub

        private int alpha, beta;
        private Mutex alphaMutex, moveMutex; //Мьютексы для синхронизации при мультипоточном поиске
        private static Dictionary<Position, ScoreDepth> bufferPositions; //Словарь уже вычесленных позиций

        /// <summary>
        /// Изменение в true остановит поиск
        /// </summary>
        public static bool IsStopSearch { get; set; }

        /// <summary>
        /// Ходы, возможные в основной позиции 
        /// </summary>
        public List<Move> MovesWithScore { get; set; }
        
        /// <summary>
        /// Глубина перебора
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// Основной класс для компьютерного игрока.
        /// Он оценивает позицию, выбирает лучший ход за компьютер.
        /// </summary>
        public Engine()
        {
            MovesWithScore = new List<Move>();
            if(bufferPositions==null)bufferPositions = new Dictionary<Position, ScoreDepth>();
        }

        /// <summary>
        /// Вычисляет вес позиции, переданной в параметре
        /// </summary>
        /// <param name="position">Позиция</param>
        /// <returns>Вес позиции</returns>
        public int CheckScoreForPosition(Position position)
        {
            int collWB = 0;
            int collBB = 0;
            // Возвращает ценность расположения фигуры к центру ()
            Func<int, int, int, int> ScoreCenter = ((i, j, coof) =>
            {
                return coof * (Math.Min(i, 7 - i) + Math.Min(j, 7 - j));
            });

            // Вернуть вес активности фигуры
            Func<Position, int, int, int, int> ScoreFigurActivity = (p, i, j, coof) =>
            {
                int sc = 0;
                if (p.Board[i, j].Figure.Color == ColorFigur.white && !EngineOptions.IsUseEasyScoreOfPosition)
                {
                    p.IsWhiteMove = true;
                    sc += coof * CheckMovesForFigur(p, i, j).Count;
                }
                else if (!EngineOptions.IsUseEasyScoreOfPosition)
                {
                    p.IsWhiteMove = false;
                    sc -= coof * CheckMovesForFigur(p, i, j).Count;
                }
                return sc;
            };
            // Вычисляет вес фигуры, учитывая ее ценность, близость к центру, активность
            Func<Position, int, int, int> ScoreFigur = ((p, i, j) =>
            {
                int s = 0;
                int scoreForFigur = 0;
                switch (p.Board[i, j].Figure.Type)
                {
                    case TypeFigur.bishop:
                        if (p.Board[i, j].Figure.Color == ColorFigur.white)
                        {
                            collWB++;
                        }
                        else
                        {
                            collBB++;
                        }
                        scoreForFigur = 3000;
                        if (!EngineOptions.IsUseEasyScoreOfPosition)
                        {
                            scoreForFigur += ScoreCenter(i, j, 30);
                        }
                        s += ScoreFigurActivity(p, i, j, 25);
                        break;
                    case TypeFigur.knight:
                        s += ScoreFigurActivity(p, i, j, 35);
                        scoreForFigur = 3000;
                        if (!EngineOptions.IsUseEasyScoreOfPosition)
                        {
                            scoreForFigur += ScoreCenter(i, j, 50);
                        }
                        break;
                    case TypeFigur.peen:
                        s += ScoreFigurActivity(p, i, j, 5);
                        scoreForFigur = 1000;
                        if (!EngineOptions.IsUseEasyScoreOfPosition)
                        {
                            scoreForFigur += ScoreCenter(i, j, 30);
                            //Проверка на сдвоенность пешек
                            for (int i1 = 1; i1 < 6; i1++)
                            {
                                if (i1 == i) continue;
                                if (p.Board[i1, j].Figure.Type == TypeFigur.peen && p.Board[i1, j].Figure.Color == p.Board[i, j].Figure.Color)
                                    scoreForFigur -= 200;
                            }
                        }
                        break;
                    case TypeFigur.queen:
                        s += ScoreFigurActivity(p, i, j, 7);
                        scoreForFigur = 9000;
                        if (!EngineOptions.IsUseEasyScoreOfPosition)
                        {
                            scoreForFigur += ScoreCenter(i, j, 10);
                        }
                        break;
                    case TypeFigur.rock:
                        s += ScoreFigurActivity(p, i, j, 20);
                        scoreForFigur = 5000;
                        if (!EngineOptions.IsUseEasyScoreOfPosition)
                        {
                            scoreForFigur += ScoreCenter(i, j, 50);
                        }
                        break;
                }
                //Если была черная фигура, то ее вес - отрицателен
                if (p.Board[i, j].Figure.Color == ColorFigur.black)
                {
                    scoreForFigur = -scoreForFigur;
                }
                s += scoreForFigur;
                return s;
            });

            int score = 0;
            bool IsWhite = position.IsWhiteMove;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (position.Board[i, j].Figure.Type != TypeFigur.none)
                    {
                        score += ScoreFigur(position, i, j);
                    }
                }
            }
            if (collBB >= 2) score -= 250;
            if (collWB >= 2) score += 250;
            position.IsWhiteMove = IsWhite;
            return score;
        }
        
        /// <summary>
        /// Вычисляет все возможные ходы для фигуры
        /// </summary>
        /// <param name="position">Позиция</param>
        /// <param name="i">Координата по вертикали</param>
        /// <param name="j">Координата по горизонтали</param>
        /// <returns></returns>
        private static List<MoveCoord> CheckMovesForFigur(Position position, int i, int j)
        {
            List<MoveCoord> mc = new List<MoveCoord>();
            //Для каждой фигуры выбираем свою функцию поиска ходов
            switch (position.Board[i, j].Figure.Type)
            {
                case TypeFigur.king:
                    mc.AddRange(CheckKing(position, i, j));break;
                case TypeFigur.queen:
                    mc.AddRange(CheckQueen(position, i, j)); break;
                case TypeFigur.rock:
                    mc.AddRange(CheckRock(position, i, j)); break;
                case TypeFigur.bishop:
                    mc.AddRange(CheckBishop(position, i, j)); break;
                case TypeFigur.knight:
                    mc.AddRange(CheckKnight(position, i, j)); break;
                case TypeFigur.peen:
                    mc.AddRange(CheckPeen(position, i, j)); break;
            }
            return mc;
        }

        #region //функции посика ходов для фигур
        private static IEnumerable<MoveCoord> CheckPeen(Position position, int i, int j)
        {
            MoveCoord move = new MoveCoord();
            List<MoveCoord> moves = new List<MoveCoord>();
            if (position.IsWhiteMove)
            {
                if(i==1)
                {
                    move = new MoveCoord();
                    move.xStart = i;
                    move.yStart = j;
                    move.xEnd = 3;
                    move.yEnd = j;
                    move.EndFigure = position.Board[3, j].Figure;
                    move.StartFigure = position.Board[i, j].Figure;
                    if (position.Board[move.xStart, move.yStart].Figure.CheckMove(position, move))
                    {
                        moves.Add(move);
                    }
                }
                move = new MoveCoord();
                move.xStart = i;
                move.yStart = j;
                move.xEnd = i+1;
                move.yEnd = j;
                move.EndFigure = position.Board[i+1, j].Figure;
                move.StartFigure = position.Board[i, j].Figure;
                if (move.StartFigure.Color == ColorFigur.white && move.xEnd == 7)
                {
                    move.NewFigure = new Queen(ColorFigur.white);
                }
                if (position.Board[move.xStart, move.yStart].Figure.CheckMove(position, move))
                {
                    moves.Add(move);
                }
                if (j < 7)
                {
                    if (position.Board[i + 1, j + 1].Figure.Color == ColorFigur.black||
                        position.Board[i, j + 1].Figure.Color==ColorFigur.black&& position.Board[i, j + 1].Figure.Type==TypeFigur.peen)
                    {
                        move = new MoveCoord();
                        move.xStart = i;
                        move.yStart = j;
                        move.xEnd = i + 1;
                        move.yEnd = j + 1;
                        move.EndFigure = position.Board[i + 1, j+1].Figure;
                        move.StartFigure = position.Board[i, j].Figure;
                        if (move.StartFigure.Color == ColorFigur.white && move.xEnd == 7)
                        {
                            move.NewFigure = new Queen(ColorFigur.white);
                        }
                        if (position.Board[move.xStart, move.yStart].Figure.CheckMove(position, move))
                        {
                            moves.Add(move);
                        }
                    }
                }
                if (j > 0)
                {
                    if (position.Board[i + 1, j - 1].Figure.Color == ColorFigur.black ||
                        position.Board[i, j - 1].Figure.Color == ColorFigur.black && position.Board[i, j - 1].Figure.Type == TypeFigur.peen)
                    {
                        move = new MoveCoord();
                        move.xStart = i;
                        move.yStart = j;
                        move.xEnd = i + 1;
                        move.yEnd = j - 1;
                        move.EndFigure = position.Board[i + 1, j-1].Figure;
                        move.StartFigure = position.Board[i, j].Figure;
                        if (move.StartFigure.Color == ColorFigur.white && move.xEnd == 7)
                        {
                            move.NewFigure = new Queen(ColorFigur.white);
                        }
                        if (position.Board[move.xStart, move.yStart].Figure.CheckMove(position, move))
                        {
                            moves.Add(move);
                        }
                    }
                }
            }
            else
            {
                if (i == 6)
                {
                    move = new MoveCoord();
                    move.xStart = i;
                    move.yStart = j;
                    move.xEnd = 4;
                    move.yEnd = j;
                    move.EndFigure = position.Board[4, j].Figure;
                    move.StartFigure = position.Board[i, j].Figure;
                    if (position.Board[move.xStart, move.yStart].Figure.CheckMove(position, move))
                    {
                        moves.Add(move);
                    }
                }
                move = new MoveCoord();
                move.xStart = i;
                move.yStart = j;
                move.xEnd = i - 1;
                move.yEnd = j;
                move.EndFigure = position.Board[i - 1, j].Figure;
                move.StartFigure = position.Board[i, j].Figure;
                if (move.StartFigure.Color == ColorFigur.black && move.xEnd == 7)
                {
                    move.NewFigure = new Queen(ColorFigur.black);
                }
                if (position.Board[move.xStart, move.yStart].Figure.CheckMove(position, move))
                {
                    moves.Add(move);
                }
                if (j < 7)
                {
                    if (position.Board[i - 1, j + 1].Figure.Color == ColorFigur.white ||
                        position.Board[i, j + 1].Figure.Color == ColorFigur.white && position.Board[i, j + 1].Figure.Type == TypeFigur.peen)
                    {
                        move = new MoveCoord();
                        move.xStart = i;
                        move.yStart = j;
                        move.xEnd = i - 1;
                        move.yEnd = j + 1;
                        move.EndFigure = position.Board[i - 1, j + 1].Figure;
                        move.StartFigure = position.Board[i, j].Figure;
                        if (move.StartFigure.Color == ColorFigur.black && move.xEnd == 7)
                        {
                            move.NewFigure = new Queen(ColorFigur.black);
                        }
                        if (position.Board[move.xStart, move.yStart].Figure.CheckMove(position, move))
                        {
                            moves.Add(move);
                        }
                    }
                }
                if (j > 0)
                {
                    if (position.Board[i - 1, j - 1].Figure.Color == ColorFigur.white ||
                        position.Board[i, j - 1].Figure.Color == ColorFigur.white && position.Board[i, j - 1].Figure.Type == TypeFigur.peen)
                    {
                        move = new MoveCoord();
                        move.xStart = i;
                        move.yStart = j;
                        move.xEnd = i - 1;
                        move.yEnd = j - 1;
                        move.EndFigure = position.Board[i - 1, j - 1].Figure;
                        move.StartFigure = position.Board[i, j].Figure;
                        if (move.StartFigure.Color == ColorFigur.black && move.xEnd == 7)
                        {
                            move.NewFigure = new Queen(ColorFigur.black);
                        }
                        if (position.Board[move.xStart, move.yStart].Figure.CheckMove(position, move))
                        {
                            moves.Add(move);
                        }
                    }
                }
            }
            return moves;
        }

        private static IEnumerable<MoveCoord> CheckKnight(Position position, int i, int j)
        {
            MoveCoord move = new MoveCoord();
            List<MoveCoord> moves = new List<MoveCoord>();
            if(i>0)
            {
                if(i>1)
                {
                    if(j<7)
                    {
                        move = new MoveCoord();
                        move.xStart = i;
                        move.yStart = j;
                        move.xEnd = i -2;
                        move.yEnd = j +1;
                        move.EndFigure = position.Board[3, j].Figure;
                        move.StartFigure = position.Board[i, j].Figure;
                        if (position.Board[move.xStart, move.yStart].Figure.CheckMove(position, move))
                        {
                            moves.Add(move);
                        }
                    }
                    if(j>0)
                    {
                        move = new MoveCoord();
                        move.xStart = i;
                        move.yStart = j;
                        move.xEnd = i - 2;
                        move.yEnd = j - 1;
                        move.EndFigure = position.Board[3, j].Figure;
                        move.StartFigure = position.Board[i, j].Figure;
                        if (position.Board[move.xStart, move.yStart].Figure.CheckMove(position, move))
                        {
                            moves.Add(move);
                        }
                    }
                }
                if (j < 6)
                {
                    move = new MoveCoord();
                    move.xStart = i;
                    move.yStart = j;
                    move.xEnd = i - 1;
                    move.yEnd = j + 2;
                    move.EndFigure = position.Board[3, j].Figure;
                    move.StartFigure = position.Board[i, j].Figure;
                    if (position.Board[move.xStart, move.yStart].Figure.CheckMove(position, move))
                    {
                        moves.Add(move);
                    }
                }
                if (j > 1)
                {
                    move = new MoveCoord();
                    move.xStart = i;
                    move.yStart = j;
                    move.xEnd = i - 1;
                    move.yEnd = j - 2;
                    move.EndFigure = position.Board[3, j].Figure;
                    move.StartFigure = position.Board[i, j].Figure;
                    if (position.Board[move.xStart, move.yStart].Figure.CheckMove(position, move))
                    {
                        moves.Add(move);
                    }
                }
            }
            if(i<7)
            {
                if(i<6)
                {
                    if (j < 7)
                    {
                        move = new MoveCoord();
                        move.xStart = i;
                        move.yStart = j;
                        move.xEnd = i + 2;
                        move.yEnd = j + 1;
                        move.EndFigure = position.Board[3, j].Figure;
                        move.StartFigure = position.Board[i, j].Figure;
                        if (position.Board[move.xStart, move.yStart].Figure.CheckMove(position, move))
                        {
                            moves.Add(move);
                        }
                    }
                    if (j > 0)
                    {
                        move = new MoveCoord();
                        move.xStart = i;
                        move.yStart = j;
                        move.xEnd = i + 2;
                        move.yEnd = j - 1;
                        move.EndFigure = position.Board[3, j].Figure;
                        move.StartFigure = position.Board[i, j].Figure;
                        if (position.Board[move.xStart, move.yStart].Figure.CheckMove(position, move))
                        {
                            moves.Add(move);
                        }
                    }
                }
                if (j < 6)
                {
                    move = new MoveCoord();
                    move.xStart = i;
                    move.yStart = j;
                    move.xEnd = i + 1;
                    move.yEnd = j + 2;
                    move.EndFigure = position.Board[3, j].Figure;
                    move.StartFigure = position.Board[i, j].Figure;
                    if (position.Board[move.xStart, move.yStart].Figure.CheckMove(position, move))
                    {
                        moves.Add(move);
                    }
                }
                if (j > 1)
                {
                    move = new MoveCoord();
                    move.xStart = i;
                    move.yStart = j;
                    move.xEnd = i + 1;
                    move.yEnd = j - 2;
                    move.EndFigure = position.Board[3, j].Figure;
                    move.StartFigure = position.Board[i, j].Figure;
                    if (position.Board[move.xStart, move.yStart].Figure.CheckMove(position, move))
                    {
                        moves.Add(move);
                    }
                }
            }
            return moves;
        }

        private static IEnumerable<MoveCoord> CheckBishop(Position position, int i, int j)
        {
            MoveCoord move = new MoveCoord();
            List<MoveCoord> moves = new List<MoveCoord>();

            bool b1, b2, b3, b4;
            b1 = b2 = b3 = b4 = true;
            for (int x = 1; x < 8; x++)
            {
                if ((i + x > 7) || (j + x > 7)) b1 = false;
                if ((i + x > 7) || (j - x < 0)) b2 = false;
                if ((i - x < 0) || (j + x > 7)) b3 = false;
                if ((i - x < 0) || (j - x < 0)) b4 = false;
                if (!b1 && !b2 && !b3 && !b4) break;

                if(b1)
                {
                    if (position.Board[i, j].Figure.Color == position.Board[i + x, j + x].Figure.Color)
                    {
                        b1 = false;
                    }
                    else
                    {
                        move = new MoveCoord();
                        move.xStart = i;
                        move.yStart = j;
                        move.xEnd = i + x;
                        move.yEnd = j + x;
                        move.EndFigure = position.Board[move.xEnd, move.yEnd].Figure;
                        move.StartFigure = position.Board[i, j].Figure;
                        if (position.Board[move.xStart, move.yStart].Figure.CheckMove(position, move))
                        {
                            moves.Add(move);
                        }
                    }
                }

                if(b2)
                {
                    if (position.Board[i, j].Figure.Color == position.Board[i + x, j - x].Figure.Color)
                    {
                        b2 = false;
                    }
                    else
                    {
                        move = new MoveCoord();
                        move.xStart = i;
                        move.yStart = j;
                        move.xEnd = i + x;
                        move.yEnd = j - x;
                        move.EndFigure = position.Board[move.xEnd, move.yEnd].Figure;
                        move.StartFigure = position.Board[i, j].Figure;
                        if (position.Board[move.xStart, move.yStart].Figure.CheckMove(position, move))
                        {
                            moves.Add(move);
                        }
                    }
                }

                if(b3)
                {
                    if (position.Board[i, j].Figure.Color == position.Board[i - x, j + x].Figure.Color)
                    {
                        b3 = false;
                    }
                    else
                    {
                        move = new MoveCoord();
                        move.xStart = i;
                        move.yStart = j;
                        move.xEnd = i - x;
                        move.yEnd = j + x;
                        move.EndFigure = position.Board[move.xEnd, move.yEnd].Figure;
                        move.StartFigure = position.Board[i, j].Figure;
                        if (position.Board[move.xStart, move.yStart].Figure.CheckMove(position, move))
                        {
                            moves.Add(move);
                        }
                    }
                }

                if(b4)
                {
                    if (position.Board[i, j].Figure.Color == position.Board[i - x, j - x].Figure.Color)
                    {
                        b4 = false;
                    }
                    else
                    {
                        move = new MoveCoord();
                        move.xStart = i;
                        move.yStart = j;
                        move.xEnd = i - x;
                        move.yEnd = j - x;
                        move.EndFigure = position.Board[move.xEnd, move.yEnd].Figure;
                        move.StartFigure = position.Board[i, j].Figure;
                        if (position.Board[move.xStart, move.yStart].Figure.CheckMove(position, move))
                        {
                            moves.Add(move);
                        }
                    }
                }
            }

            return moves;
        }

        private static IEnumerable<MoveCoord> CheckRock(Position position, int i, int j)
        {
            MoveCoord move = new MoveCoord();
            List<MoveCoord> moves = new List<MoveCoord>();

            bool b1, b2, b3, b4;
            b1 = b2 = b3 = b4 = true;
            for (int x = 1; x<8; x++)
            {
                if (i + x > 7) b1 = false;
                if (i - x < 0) b2 = false;
                if (j + x > 7) b3 = false;
                if (j - x < 0) b4 = false;
                if (!b1 && !b2 && !b3 && !b4) break;

                if (b1)
                {
                    if (position.Board[i, j].Figure.Color == position.Board[i + x, j].Figure.Color)
                    {
                        b1 = false;
                    }
                    else
                    {
                        move = new MoveCoord();
                        move.xStart = i;
                        move.yStart = j;
                        move.xEnd = i + x;
                        move.yEnd = j;
                        move.EndFigure = position.Board[move.xEnd, move.yEnd].Figure;
                        move.StartFigure = position.Board[i, j].Figure;
                        if (position.Board[move.xStart, move.yStart].Figure.CheckMove(position, move))
                        {
                            moves.Add(move);
                        }
                    }
                }

                if (b2)
                {
                    if (position.Board[i, j].Figure.Color == position.Board[i - x, j].Figure.Color)
                    {
                        b2 = false;
                    }
                    else
                    {
                        move = new MoveCoord();
                        move.xStart = i;
                        move.yStart = j;
                        move.xEnd = i - x;
                        move.yEnd = j;
                        move.EndFigure = position.Board[move.xEnd, move.yEnd].Figure;
                        move.StartFigure = position.Board[i, j].Figure;
                        if (position.Board[move.xStart, move.yStart].Figure.CheckMove(position, move))
                        {
                            moves.Add(move);
                        }
                    }
                }

                if (b3)
                {
                    if (position.Board[i, j].Figure.Color == position.Board[i, j + x].Figure.Color)
                    {
                        b3 = false;
                    }
                    else
                    {
                        move = new MoveCoord();
                        move.xStart = i;
                        move.yStart = j;
                        move.xEnd = i;
                        move.yEnd = j + x;
                        move.EndFigure = position.Board[move.xEnd, move.yEnd].Figure;
                        move.StartFigure = position.Board[i, j].Figure;
                        if (position.Board[move.xStart, move.yStart].Figure.CheckMove(position, move))
                        {
                            moves.Add(move);
                        }
                    }
                }

                if (b4)
                {
                    if (position.Board[i, j].Figure.Color == position.Board[i, j - x].Figure.Color)
                    {
                        b4 = false;
                    }
                    else
                    {
                        move = new MoveCoord();
                        move.xStart = i;
                        move.yStart = j;
                        move.xEnd = i;
                        move.yEnd = j - x;
                        move.EndFigure = position.Board[move.xEnd, move.yEnd].Figure;
                        move.StartFigure = position.Board[i, j].Figure;
                        if (position.Board[move.xStart, move.yStart].Figure.CheckMove(position, move))
                        {
                            moves.Add(move);
                        }
                    }
                }
            }

            return moves;
        }

        private static IEnumerable<MoveCoord> CheckQueen(Position position, int i, int j)
        {
            MoveCoord move = new MoveCoord();
            List<MoveCoord> moves = new List<MoveCoord>();
            bool b1, b2, b3, b4;
            b1 = b2 = b3 = b4 = true;
            for (int x = 1; x < 8; x++)
            {
                if ((i + x > 7) || (j + x > 7)) b1 = false;
                if ((i + x > 7) || (j - x < 0)) b2 = false;
                if ((i - x < 0) || (j + x > 7)) b3 = false;
                if ((i - x < 0) || (j - x < 0)) b4 = false;
                if (!b1 && !b2 && !b3 && !b4) break;

                if (b1)
                {
                    if (position.Board[i, j].Figure.Color == position.Board[i + x, j + x].Figure.Color)
                    {
                        b1 = false;
                    }
                    else
                    {
                        move = new MoveCoord();
                        move.xStart = i;
                        move.yStart = j;
                        move.xEnd = i + x;
                        move.yEnd = j + x;
                        move.EndFigure = position.Board[move.xEnd, move.yEnd].Figure;
                        move.StartFigure = position.Board[i, j].Figure;
                        if (position.Board[move.xStart, move.yStart].Figure.CheckMove(position, move))
                        {
                            moves.Add(move);
                        }
                    }
                }

                if (b2)
                {
                    if (position.Board[i, j].Figure.Color == position.Board[i + x, j - x].Figure.Color)
                    {
                        b2 = false;
                    }
                    else
                    {
                        move = new MoveCoord();
                        move.xStart = i;
                        move.yStart = j;
                        move.xEnd = i + x;
                        move.yEnd = j - x;
                        move.EndFigure = position.Board[move.xEnd, move.yEnd].Figure;
                        move.StartFigure = position.Board[i, j].Figure;
                        if (position.Board[move.xStart, move.yStart].Figure.CheckMove(position, move))
                        {
                            moves.Add(move);
                        }
                    }
                }

                if (b3)
                {
                    if (position.Board[i, j].Figure.Color == position.Board[i - x, j + x].Figure.Color)
                    {
                        b3 = false;
                    }
                    else
                    {
                        move = new MoveCoord();
                        move.xStart = i;
                        move.yStart = j;
                        move.xEnd = i - x;
                        move.yEnd = j + x;
                        move.EndFigure = position.Board[move.xEnd, move.yEnd].Figure;
                        move.StartFigure = position.Board[i, j].Figure;
                        if (position.Board[move.xStart, move.yStart].Figure.CheckMove(position, move))
                        {
                            moves.Add(move);
                        }
                    }
                }

                if (b4)
                {
                    if (position.Board[i, j].Figure.Color == position.Board[i - x, j - x].Figure.Color)
                    {
                        b4 = false;
                    }
                    else
                    {
                        move = new MoveCoord();
                        move.xStart = i;
                        move.yStart = j;
                        move.xEnd = i - x;
                        move.yEnd = j - x;
                        move.EndFigure = position.Board[move.xEnd, move.yEnd].Figure;
                        move.StartFigure = position.Board[i, j].Figure;
                        if (position.Board[move.xStart, move.yStart].Figure.CheckMove(position, move))
                        {
                            moves.Add(move);
                        }
                    }
                }
            }

            b1 = b2 = b3 = b4 = true;
            for (int x = 1; x < 8; x++)
            {
                if (i + x > 7) b1 = false;
                if (i - x < 0) b2 = false;
                if (j + x > 7) b3 = false;
                if (j - x < 0) b4 = false;
                if (!b1 && !b2 && !b3 && !b4) break;

                if (b1)
                {
                    if (position.Board[i, j].Figure.Color == position.Board[i + x, j].Figure.Color)
                    {
                        b1 = false;
                    }
                    else
                    {
                        move = new MoveCoord();
                        move.xStart = i;
                        move.yStart = j;
                        move.xEnd = i + x;
                        move.yEnd = j;
                        move.EndFigure = position.Board[move.xEnd, move.yEnd].Figure;
                        move.StartFigure = position.Board[i, j].Figure;
                        if (position.Board[move.xStart, move.yStart].Figure.CheckMove(position, move))
                        {
                            moves.Add(move);
                        }
                    }
                }

                if (b2)
                {
                    if (position.Board[i, j].Figure.Color == position.Board[i - x, j].Figure.Color)
                    {
                        b2 = false;
                    }
                    else
                    {
                        move = new MoveCoord();
                        move.xStart = i;
                        move.yStart = j;
                        move.xEnd = i - x;
                        move.yEnd = j;
                        move.EndFigure = position.Board[move.xEnd, move.yEnd].Figure;
                        move.StartFigure = position.Board[i, j].Figure;
                        if (position.Board[move.xStart, move.yStart].Figure.CheckMove(position, move))
                        {
                            moves.Add(move);
                        }
                    }
                }

                if (b3)
                {
                    if (position.Board[i, j].Figure.Color == position.Board[i, j + x].Figure.Color)
                    {
                        b3 = false;
                    }
                    else
                    {
                        move = new MoveCoord();
                        move.xStart = i;
                        move.yStart = j;
                        move.xEnd = i;
                        move.yEnd = j + x;
                        move.EndFigure = position.Board[move.xEnd, move.yEnd].Figure;
                        move.StartFigure = position.Board[i, j].Figure;
                        if (position.Board[move.xStart, move.yStart].Figure.CheckMove(position, move))
                        {
                            moves.Add(move);
                        }
                    }
                }

                if (b4)
                {
                    if (position.Board[i, j].Figure.Color == position.Board[i, j - x].Figure.Color)
                    {
                        b4 = false;
                    }
                    else
                    {
                        move = new MoveCoord();
                        move.xStart = i;
                        move.yStart = j;
                        move.xEnd = i;
                        move.yEnd = j - x;
                        move.EndFigure = position.Board[move.xEnd, move.yEnd].Figure;
                        move.StartFigure = position.Board[i, j].Figure;
                        if (position.Board[move.xStart, move.yStart].Figure.CheckMove(position, move))
                        {
                            moves.Add(move);
                        }
                    }
                }
            }

            return moves;
        }

        private static IEnumerable<MoveCoord> CheckKing(Position position, int i, int j)
        {
            MoveCoord move = new MoveCoord();
            List<MoveCoord> moves = new List<MoveCoord>();
            if((position.IsWhiteMove && i == 0 || !position.IsWhiteMove && i == 7) && j == 4)
            {
                move = new MoveCoord();
                move.xStart = i;
                move.yStart = j;
                move.xEnd = i;
                move.yEnd = j + 2;
                move.EndFigure = position.Board[move.xEnd, move.yEnd].Figure;
                move.StartFigure = position.Board[i, j].Figure;
                move.IsCastling = true;
                if (position.Board[move.xStart, move.yStart].Figure.CheckMove(position, move))
                {
                    moves.Add(move);
                }
                move = new MoveCoord();
                move.xStart = i;
                move.yStart = j;
                move.xEnd = i;
                move.yEnd = j - 2;
                move.EndFigure = position.Board[move.xEnd, move.yEnd].Figure;
                move.StartFigure = position.Board[i, j].Figure;
                move.IsCastling = true;
                if (position.Board[move.xStart, move.yStart].Figure.CheckMove(position, move))
                {
                    moves.Add(move);
                }
            }
            if(i + 1 < 8)
            {
                if (j + 1 < 8)
                {
                    move = new MoveCoord();
                    move.xStart = i;
                    move.yStart = j;
                    move.xEnd = i + 1;
                    move.yEnd = j + 1;
                    move.EndFigure = position.Board[move.xEnd, move.yEnd].Figure;
                    move.StartFigure = position.Board[i, j].Figure;
                    if (position.Board[move.xStart, move.yStart].Figure.CheckMove(position, move))
                    {
                        moves.Add(move);
                    }
                }
                if (j - 1 >= 0)
                {
                    move = new MoveCoord();
                    move.xStart = i;
                    move.yStart = j;
                    move.xEnd = i + 1;
                    move.yEnd = j - 1;
                    move.EndFigure = position.Board[move.xEnd, move.yEnd].Figure;
                    move.StartFigure = position.Board[i, j].Figure;
                    if (position.Board[move.xStart, move.yStart].Figure.CheckMove(position, move))
                    {
                        moves.Add(move);
                    }
                }
                move = new MoveCoord();
                move.xStart = i;
                move.yStart = j;
                move.xEnd = i + 1;
                move.yEnd = j;
                move.EndFigure = position.Board[move.xEnd, move.yEnd].Figure;
                move.StartFigure = position.Board[i, j].Figure;
                if (position.Board[move.xStart, move.yStart].Figure.CheckMove(position, move))
                {
                    moves.Add(move);
                }
            }
            if(i - 1 >=0)
            {
                if (j + 1 < 8)
                {
                    move = new MoveCoord();
                    move.xStart = i;
                    move.yStart = j;
                    move.xEnd = i - 1;
                    move.yEnd = j + 1;
                    move.EndFigure = position.Board[move.xEnd, move.yEnd].Figure;
                    move.StartFigure = position.Board[i, j].Figure;
                    if (position.Board[move.xStart, move.yStart].Figure.CheckMove(position, move))
                    {
                        moves.Add(move);
                    }
                }
                if (j - 1 >= 0)
                {
                    move = new MoveCoord();
                    move.xStart = i;
                    move.yStart = j;
                    move.xEnd = i - 1;
                    move.yEnd = j - 1;
                    move.EndFigure = position.Board[move.xEnd, move.yEnd].Figure;
                    move.StartFigure = position.Board[i, j].Figure;
                    if (position.Board[move.xStart, move.yStart].Figure.CheckMove(position, move))
                    {
                        moves.Add(move);
                    }
                }
                move = new MoveCoord();
                move.xStart = i;
                move.yStart = j;
                move.xEnd = i - 1;
                move.yEnd = j;
                move.EndFigure = position.Board[move.xEnd, move.yEnd].Figure;
                move.StartFigure = position.Board[i, j].Figure;
                if (position.Board[move.xStart, move.yStart].Figure.CheckMove(position, move))
                {
                    moves.Add(move);
                }
            }
            if (j + 1 < 8)
            {
                move = new MoveCoord();
                move.xStart = i;
                move.yStart = j;
                move.xEnd = i;
                move.yEnd = j + 1;
                move.EndFigure = position.Board[move.xEnd, move.yEnd].Figure;
                move.StartFigure = position.Board[i, j].Figure;
                if (position.Board[move.xStart, move.yStart].Figure.CheckMove(position, move))
                {
                    moves.Add(move);
                }
            }
            if (j - 1 >= 0)
            {
                move = new MoveCoord();
                move.xStart = i;
                move.yStart = j;
                move.xEnd = i;
                move.yEnd = j - 1;
                move.EndFigure = position.Board[move.xEnd, move.yEnd].Figure;
                move.StartFigure = position.Board[i, j].Figure;
                if (position.Board[move.xStart, move.yStart].Figure.CheckMove(position, move))
                {
                    moves.Add(move);
                }
            }
            return moves;
        }
        #endregion

        /// <summary>
        /// Выдать список всех ходов
        /// </summary>
        public List<MoveCoord> SearchAllLegalMoves(Position position)
        {
            List<MoveCoord> moves = new List<MoveCoord>();
            //Ищем все фигуры нужного цвета и добавляем к списку moves их возможные ходы
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (position.Board[i, j].Figure.Type != TypeFigur.none)
                    {
                        if (position.IsWhiteMove && position.Board[i, j].Figure.Color == ColorFigur.white
                            || !position.IsWhiteMove && position.Board[i, j].Figure.Color == ColorFigur.black)
                        {
                            moves.AddRange(CheckMovesForFigur(position, i, j));
                        }
                    }
                }
            }
            if (position.IsWhiteMove) moves.Reverse();
            return moves;
        }

        /// <summary>
        /// Проверяет есть ли опасные взятия после хода 
        /// </summary>
        ///  /// <param name="p">Позиция</param>
        /// <param name="mc">Ход</param>
        private bool IsCheckThisEating(Position p, MoveCoord mc)
        {
            // При ходе на не атакованные противником или защищенные поля не представляют интереса
            if (p.Board[mc.xEnd, mc.yEnd].IsAttackBlack && p.Board[mc.xEnd, mc.yEnd].IsAttackWhite) return false;
            if (p.IsWhiteMove && !p.Board[mc.xEnd, mc.yEnd].IsAttackWhite) return false;
            if (!p.IsWhiteMove && !p.Board[mc.xEnd, mc.yEnd].IsAttackBlack) return false;
            if (mc.StartFigure.Type == TypeFigur.peen)
            {
                if (p.IsWhiteMove && !p.Board[mc.xEnd, mc.yEnd].IsAttackBlack) return false;
                if (!p.IsWhiteMove && !p.Board[mc.xEnd, mc.yEnd].IsAttackWhite) return false;
            }
            //Задаем начальные веса атакованых фигур
            int maxAttackWhite = 0;
            int maxAttackBlack = 0;
            if(mc.EndFigure.Type!=TypeFigur.none)
            {
                switch(mc.EndFigure.Type)
                {
                    case TypeFigur.king:
                        return true;
                    case TypeFigur.queen:
                        return true;
                    case TypeFigur.rock:
                        if (mc.EndFigure.Color == ColorFigur.white)
                        {
                            maxAttackBlack = 5;
                        }
                        else
                        {
                            maxAttackWhite = 5;
                        }
                        break;
                    case TypeFigur.bishop:
                        if (mc.EndFigure.Color == ColorFigur.white)
                        {
                            maxAttackBlack = 3;
                        }
                        else
                        {
                            maxAttackWhite = 3;
                        }
                        break;
                    case TypeFigur.knight:
                        if (mc.EndFigure.Color == ColorFigur.white)
                        {
                            maxAttackBlack = 3;
                        }
                        else
                        {
                            maxAttackWhite = 3;
                        }
                        break;
                    case TypeFigur.peen:
                        if (mc.EndFigure.Color == ColorFigur.white)
                        {
                            maxAttackBlack = 1;
                        }
                        else
                        {
                            maxAttackWhite = 1;
                        }
                        break;
                }
            }
            // В зависимости от типа сильнейшей атакованой фигуры
            // делаем вывод об опасных взятиях
            foreach (Square s in p.Board)
            {
                if(s.Figure.Color!=ColorFigur.none)
                {
                    switch (s.Figure.Type)
                    {
                        case TypeFigur.king:
                            return true;
                        case TypeFigur.queen:
                            return true;
                        case TypeFigur.rock:
                            if (s.Figure.Color == ColorFigur.white && s.IsAttackBlack)
                                maxAttackBlack = Math.Max(5, maxAttackBlack);
                            else if (s.Figure.Color == ColorFigur.black && s.IsAttackWhite)
                                maxAttackWhite = Math.Max(5, maxAttackWhite);
                            break;
                        case TypeFigur.bishop:
                            if (s.Figure.Color == ColorFigur.white && s.IsAttackBlack)
                                maxAttackBlack = Math.Max(3, maxAttackBlack);
                            else if (s.Figure.Color == ColorFigur.black && s.IsAttackWhite)
                                maxAttackWhite = Math.Max(3, maxAttackWhite);
                            break;
                        case TypeFigur.knight:
                            if (s.Figure.Color == ColorFigur.white && s.IsAttackBlack)
                                maxAttackBlack = Math.Max(3, maxAttackBlack);
                            else if (s.Figure.Color == ColorFigur.black && s.IsAttackWhite)
                                maxAttackWhite = Math.Max(3, maxAttackWhite);
                            break;
                        case TypeFigur.peen:
                            if (s.Figure.Color == ColorFigur.white && s.IsAttackBlack)
                                maxAttackBlack = Math.Max(1, maxAttackBlack);
                            else if (s.Figure.Color == ColorFigur.black && s.IsAttackWhite)
                                maxAttackWhite = Math.Max(1, maxAttackWhite);
                            break;
                    }
                }
            }
            if (p.IsWhiteMove && maxAttackBlack >= maxAttackWhite) return true;
            if (!p.IsWhiteMove && maxAttackWhite <= maxAttackBlack) return true;
            return false;
        }

        
        /// <summary>
        /// Alpha-beta поиск лучшего хода на заданную глубину
        /// </summary>
        /// <param name="p">Позиция</param>
        /// <param name="depth">Глубина поиска (по умолчю равна 1)</param>
        /// <param name="thisDepth">Текущая глубина (по умолч. равна нулю)</param>
        /// <param name="valPerMoves">Список ходов с весами (первыми будут рассматриваться ходы с лучшим весом). По умольчанию равен null</param>
        /// <returns></returns>
        public int? SearchInDepth(Position p, int depth = 1, int thisDepth = 0, List<Move> valPerMoves = null)
        {
            // При мате или пате вернуть вес позиции сразу
            int? mat = GameLogic.GameLogic.CheckPosition(p);
            if (mat == 0) return 0;
            if (mat > 0) return 1000000;
            if (mat != null) return -1000000;
            // При достижении требуемой глубины поиска - вернуть вес позиции
            if (thisDepth >= depth || thisDepth>=EngineOptions.MaxDepth) return CheckScoreForPosition(p);
            
            List<MoveCoord> myL = new List<MoveCoord>();
            // Если нет базового списка ходов с весами,
            // то получение списка ходов, для которых будет выполняться перебор
            if (valPerMoves == null)
            {
                valPerMoves = new List<Move>();
                myL = SearchAllLegalMoves(p);
                if (myL != null)
                {
                    myL.Sort(); //Сортировка ходов для базового приоритета
                }
            }
            else
            {
                valPerMoves.Sort(); //Сортировка ходов по весам
                if (p.IsWhiteMove) valPerMoves.Reverse(); //Для белых первыми рассматриваются ходы с максимальным весом
                for(int i=0;i<valPerMoves.Count;i++)
                {
                    myL.Add(valPerMoves[i].MC); 
                }
            }

            //При текущей глубине == 0 установка начальных значений, запуск мультипотока (в зависимости от опций), сохранение результатов для передаче другим компонентам.
            if (thisDepth == 0)
            {
                //Мutex нужны для синхронизации доступа потоков к переменным веса ходов
                moveMutex = new Mutex();
                alphaMutex = new Mutex();

                //Если возможен лишь один ход, то вернуть его, и не важно с каким весом 
                if (myL.Count == 1)
                {
                    MovesWithScore.Add(new Move(0, myL[0]));
                    return MovesWithScore[0].Score;
                }
                // Задаем начальные параметры
                valPerMoves = new List<Move>();
                flexDepth = depth;
                alpha = -100000000;
                beta = 100000000;

                // Выполняем перебор ходов (мультипоточно или однопоточно)
                if (EngineOptions.IsMultithread)
                {
                    #region // Мультипоточно
                    CancellationTokenSource cts = new CancellationTokenSource();
                    ParallelOptions po = new ParallelOptions();
                    po.CancellationToken = cts.Token;
                    po.MaxDegreeOfParallelism = System.Environment.ProcessorCount;
                    Parallel.ForEach(myL, po, (mc,pls) =>
                    {
                        // Копируем позицию, чтобы избежать изменений
                        var p1 = (Position)p.DeepCopy();
                        mc.EndFigure = p1.Board[mc.xEnd, mc.yEnd].Figure;
                        p1.MoveChess(mc);
                        if (p1.Board[mc.xEnd, mc.yEnd].Figure.Type == TypeFigur.peen && (mc.xEnd == 7 || mc.xEnd == 0))
                        {
                            p1.Board[mc.xEnd, mc.yEnd] = new Square(new Queen(p1.Board[mc.xEnd, mc.yEnd].Figure.Color));
                        }
                        // вычисляем вес хода 
                        int? score = SearchInDepth(p1, depth, thisDepth + 1);
                        // Добавляем ход и его вес в список ходов
                        if (score != null)
                        {
                            moveMutex.WaitOne();
                            valPerMoves.Add(new Move((int)score, mc));
                            moveMutex.ReleaseMutex();
                            if (p.IsWhiteMove && score > 100000 || !p.IsWhiteMove && (int)score < -100000) pls.Break();
                        }
                    });
                    #endregion
                }
                else
                {
                    #region // Однопоточно
                    foreach (MoveCoord mc in myL)
                    {
                        mc.EndFigure = p.Board[mc.xEnd, mc.yEnd].Figure;
                        p.MoveChess(mc);
                        if (p.Board[mc.xEnd, mc.yEnd].Figure.Type == TypeFigur.peen && (mc.xEnd == 7 || mc.xEnd == 0))
                        {
                            p.Board[mc.xEnd, mc.yEnd] = new Square(new Queen(p.Board[mc.xEnd, mc.yEnd].Figure.Color));
                        }
                        // вычисляем вес хода 
                        int? score = SearchInDepth(p, depth, thisDepth + 1);
                        // Добавляем ход и его вес в список ходов
                        if (score != null)
                        {
                            valPerMoves.Add(new Move((int)score, mc));
                        }
                        // Отменяем изменения
                        p.MoveBack(mc);
                    }
                    #endregion
                }

                // В случае генерации прерывания поиска и наличии записи ходов в основном списке,
                // Заменяем все ходы, которые успели рассчитать
                if (IsStopSearch && MovesWithScore != null)
                {
                    if (valPerMoves != null)
                    {
                        foreach (Move oneMove in valPerMoves)
                        {
                            for (int i = 0; i < MovesWithScore.Count; i++)
                            {
                                if (MovesWithScore[i].MC.Equals(oneMove.MC))
                                {
                                    MovesWithScore[i].Score = oneMove.Score;
                                    break;
                                }
                            }
                        }
                    }
                }
                // Если прерывания не было, заменяем весь результат поиска
                else
                {
                    MovesWithScore = new List<Move>();
                    MovesWithScore = valPerMoves;
                }
            }
            // Если текущая глубина больше нуля, то поиск без добавления ходов в основной список.
            // Также добавление позиций в словарь, или изввлечение веса позиций оттуда (если позиция там уже есть и глубина ее рассчета больше)
            else
            {
                //Проверяем, есть ли уже позиция в словаре
                //Если есть - извлекаем
                if(EngineOptions.IsUsePositionDictionary)
                { 
                    ScoreDepth sd;
                    if (bufferPositions.TryGetValue(p, out sd))
                    {
                        if (sd.Depth > depth - thisDepth)
                        {
                            if (p.IsWhiteMove)
                            {
                                //Изменяем значение бета
                                alphaMutex.WaitOne();
                                if (beta > sd.Score && thisDepth == 1) beta = sd.Score;
                                alphaMutex.ReleaseMutex();
                            }
                            else
                            {
                                // Изменяем значение альфа
                                alphaMutex.WaitOne();
                                if (alpha < sd.Score && thisDepth == 1) alpha = sd.Score;
                                alphaMutex.ReleaseMutex();
                            }
                            return sd.Score;
                        }
                    }
                }

                // Выполняем перебор ходов 
                foreach (MoveCoord mc in myL)
                {
                    #region
                    mc.EndFigure = p.Board[mc.xEnd, mc.yEnd].Figure;
                    p.MoveChess(mc);
                    if (p.Board[mc.xEnd, mc.yEnd].Figure.Type == TypeFigur.peen && (mc.xEnd == 7 || mc.xEnd == 0))
                    {
                        p.Board[mc.xEnd, mc.yEnd] = new Square(new Queen(p.Board[mc.xEnd, mc.yEnd].Figure.Color));
                    }

                    int? score = 0;
                    // В случае прерывания поиска вернем null
                    if (IsStopSearch) return null;

                    // При почти максимальной глубине поиска нужно убедиться, что нет ненувиденного мата или проигрыша фигуры
                    if (thisDepth == depth - 1)
                    {
                        AttackChecker.CheckAttack(ref p);
                        // В случае если позиция опасна, увеличиваем максимальную глубину поиска (но не более чем на 2)
                        if (IsCheckThisEating(p, mc) && depth - 2 < flexDepth)
                        {
                            AttackChecker.CheckAttackClear(ref p);
                            depth += 1;
                            score = SearchInDepth(p, depth, thisDepth + 1);
                            depth -= 1;
                        }
                        //Иначе выясняем вес позиции в обычную глубину
                        else
                        {
                            AttackChecker.CheckAttackClear(ref p);
                            score = SearchInDepth(p, depth, thisDepth + 1);
                        }
                    }
                    //Иначе выясняем вес позиции в обычную глубину
                    else
                    {
                        score = SearchInDepth(p, depth, thisDepth + 1);
                    }
                    #endregion

                    #region//Проверяем, что вес позиции входит в диапозон alpha-beta, иначе прерываем поиск
                    if (score != null)
                    {
                        valPerMoves.Add(new Move((int)score, mc));
                        if (!p.IsWhiteMove)
                        {
                            if (score > beta)
                            {
                                p.MoveBack(mc);
                                return score;
                            }
                        }
                        else
                        {
                            if (score < alpha)
                            {
                                p.MoveBack(mc);
                                return score;
                            }
                        }
                    }
                    #endregion

                    p.MoveBack(mc);
                }
            }

            #region// Сохраняем результаты поиска
            if (p.IsWhiteMove)
            {
                // Изменяем бета
                if (valPerMoves.Max() != null)
                {
                    alphaMutex.WaitOne();
                    int beta1 = valPerMoves.Max().Score;
                    if (beta > beta1 && thisDepth == 1)
                    {
                        beta = beta1;
                    }
                    alphaMutex.ReleaseMutex();

                    // Если позиции нет в словаре - добавляем
                    if (EngineOptions.IsUsePositionDictionary)
                    {
                        ScoreDepth sd;
                        if (bufferPositions.TryGetValue(p, out sd))
                        {
                            if (sd.Depth < depth - thisDepth)
                            {
                                moveMutex.WaitOne();
                                bufferPositions[p].Depth = depth - thisDepth;
                                bufferPositions[p].Score = beta1;
                                moveMutex.ReleaseMutex();
                            }
                        }
                        else
                        {
                            moveMutex.WaitOne();
                            try
                            {
                                bufferPositions.Add(p, new ScoreDepth(beta1, depth - thisDepth));
                            }
                            catch
                            {
                                if (bufferPositions.TryGetValue(p, out sd))
                                {
                                    if (sd.Depth < depth - thisDepth)
                                    {
                                        moveMutex.WaitOne();
                                        bufferPositions[p].Depth = depth - thisDepth;
                                        bufferPositions[p].Score = beta1;
                                        moveMutex.ReleaseMutex();
                                    }
                                }
                            }
                            moveMutex.ReleaseMutex();
                        }
                    }
                    return beta1;
                }
                else return null;
            }
            else
            {
                //Изменяем альфа
                if (valPerMoves.Min() != null)
                {
                    alphaMutex.WaitOne();
                    int alpha1 = valPerMoves.Min().Score;
                    if (alpha < alpha1 && thisDepth == 1)
                    {
                        alpha = alpha1;
                    }
                    alphaMutex.ReleaseMutex();

                    // Если позиции нет в словаре - добавляем
                    if (EngineOptions.IsUsePositionDictionary)
                    {
                        ScoreDepth sd;
                        if (bufferPositions.TryGetValue(p, out sd))
                        {
                            if (sd.Depth < depth - thisDepth)
                            {
                                moveMutex.WaitOne();
                                bufferPositions[p].Depth = depth - thisDepth;
                                bufferPositions[p].Score = alpha1;
                                moveMutex.ReleaseMutex();
                            }
                        }
                        else
                        {
                            moveMutex.WaitOne();
                            try
                            {
                                bufferPositions.Add(p, new ScoreDepth(alpha1, depth - thisDepth));
                            }
                            catch
                            {
                                if (bufferPositions.TryGetValue(p, out sd))
                                {
                                    if (sd.Depth < depth - thisDepth)
                                    {
                                        moveMutex.WaitOne();
                                        bufferPositions[p].Depth = depth - thisDepth;
                                        bufferPositions[p].Score = alpha1;
                                        moveMutex.ReleaseMutex();
                                    }
                                }
                            }
                            moveMutex.ReleaseMutex();
                        }
                    }
                    return alpha1;
                }
                else return null;
            }
            #endregion
        }

        /// <summary>
        /// Ищет лучший ход, учитывая время на ход т.е. при запасе времени увеличивает глубину поиска.
        /// </summary>
        /// <param name="p">Позиция для поиска</param>
        /// <param name="depth">Максимальная глубина поиска (по умолч. = 1)</param>
        /// <param name="isPlayerMove">Ходит ли игрок</param>
        /// <returns>Вес лучшего хода</returns>
        public int? SearchInTime(Position p,  int depth = 1, bool isPlayerMove = false)
        {
            MovesWithScore = new List<Move>();
            EngineTime.Watch = new Stopwatch();
            int? score = 0;
            long milliseconds = 0;
            //Вычисляем время на игру для движка
            if(p.IsWhiteMove)
            {
                milliseconds=(int)((GameLogic.GameLogic.MaxTimeWhite - GameLogic.GameLogic.TimeWhite.Elapsed).TotalMilliseconds / 15);
                //В начале партии время на обдумывания уменьшаем
                if (GameLogic.GameLogic.MaxTimeWhite > new TimeSpan(0, 10, 0)
                    && GameLogic.GameLogic.Moves.Count < 8
                    && GameLogic.GameLogic.Positions[0] == new Position("start"))
                {
                    milliseconds /= 2;
                }
            }
            else
            {
                milliseconds = (int)((GameLogic.GameLogic.MaxTimeBlack - GameLogic.GameLogic.TimeBlack.Elapsed).TotalMilliseconds / 15);
                //В начале партии время на обдумывания уменьшаем
                if (GameLogic.GameLogic.MaxTimeBlack > new TimeSpan(0, 10, 0)
                    && GameLogic.GameLogic.Moves.Count < 8
                    && GameLogic.GameLogic.Positions[0] == new Position("start"))
                {
                    milliseconds /= 2;
                }
            }

            int hours = (int)(milliseconds / (1000 * 3600));
            int minutes = (int)((milliseconds / (1000 * 60)) % 60);
            int seconds = (int)((milliseconds / 1000) % 60);

            EngineTime.MaxTimeForSearching = new TimeSpan(hours, minutes, seconds);

            //При ходе компьютера активируем секундомер
            if (!isPlayerMove)
            {
                EngineTime.Watch.Start();
            }
            //Запускаем проверку, что время не вышло, а позиция не изменилась
            EngineTime.TimerStart();

            // Пока поиск не остановлен из за срабатывания события или пока не привысим максимальную глубину поиска
            // ищем лучший ход, постепенно увеличивая глубину 
            for (int thisDepth = 1; thisDepth <= EngineOptions.MaxDepth && !IsStopSearch ; thisDepth++)
            {
                Position p1 = (Position)p.DeepCopy();
                if (MovesWithScore.Count != 0)
                {
                    score = SearchInDepth(p1, thisDepth);
                }
                else
                {
                    score = SearchInDepth(p1, thisDepth);
                }
                if (IsStopSearch)
                {
                    if (p1.IsWhiteMove) score = MovesWithScore.Max().Score;
                    else score = MovesWithScore.Min().Score;
                    break;
                }
            }

            //Останавливаем таймер и секундомер
            IsStopSearch = false;
            EngineTime.Watch.Stop();
            EngineTime.Timer.Stop();
            return score;
        }
    }
}