using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;

namespace YanChess.GameLogic
{
    /// <summary>
    /// Статический класс, содержащий основную игровую позицию и основные функции проверки позиции на возможность хода, мат, пат и т.п.
    /// </summary>
    public static class GameLogic
    {
        //метод обратного вызова
        // создаем таймер
        private static TimerCallback tm;
        private static Timer timer;
        public static int MovesWithoutEating;
        public static List<Position> Positions { get; set; }
        public static List<MoveCoord> Moves { get; set; }

        /// <summary>
        /// запустить генерацию проверки времени на игру, по его истечении т.е. по истечении времени, будет сгенерировано событие
        /// </summary>
        public static void TimerStart()
        {
            tm = new TimerCallback(CheckTime);
            timer = new Timer(tm, new object(), 0, 100);
        }

        /// <summary>
        /// Начать новую игру
        /// </summary>
        /// <param name="timeWhite"></param>
        /// <param name="timeBlack"></param>
        /// <param name="funcsTime"></param>
        /// <param name="funcsEndGame"></param>
        /// <param name="p"></param>
        public static void NewGame(TimeSpan timeWhite, TimeSpan timeBlack, List<EventHandler> funcsTime,List<EventHandler> funcsEndGame, Position p=null)
        {
            Positions = new List<Position>();
            Moves = new List<MoveCoord>();
            
            MovesWithoutEating = 0;
            TimeEndedArgs = new EventArgs();
            TimerStop();
            WhiteWin = false;
            BlackWin = false;
            if (p == null)
            {
                GamePosition = new Position("start");
            }
            else GamePosition = (Position)p.DeepCopy();
            Positions.Add(GamePosition);
            MaxTimeBlack = timeBlack;
            MaxTimeWhite = timeWhite;
            TimeWhite = new System.Diagnostics.Stopwatch();
            TimeBlack = new System.Diagnostics.Stopwatch();
            if (funcsTime != null)
            {
                foreach (EventHandler f in funcsTime)
                {
                    EventTimeEnded += f;
                }
            }
            if (funcsEndGame != null)
            {
                foreach (EventHandler f in funcsEndGame)
                {
                    EventGameEnd += f;
                }
            }
            if (GamePosition.IsWhiteMove)
            {
                TimerStart();
                TimeWhite.Start();
            }
            else
            {
                TimerStart();
                TimeBlack.Start();
            }
        }
        
        /// <summary>
        /// Остановить игру (часы и т.д)
        /// </summary>
        public static void StopGame()
        {
            TimerStop();
            TimeWhite.Stop();
            TimeBlack.Stop();
            EventGameEnd?.Invoke(new object(), new EventArgs());
        }
        /// <summary>
        /// Остановить генерацию проверки времени на игру, по его истечении т.е. по истечении времени, будет сгенерировано событие
        /// </summary>
        public static void TimerStop()
        {
            if(timer!=null)timer.Dispose();
        }
        /// <summary>
        /// Когда у одной из сторон вышло время
        /// </summary>
        public static event System.EventHandler EventTimeEnded;
        /// <summary>
        /// Событие окончания игры
        /// </summary>
        public static event System.EventHandler EventGameEnd;

        /// <summary>
        /// Игровая позиция
        /// </summary>
        public static Position GamePosition { get; set; }
        public static bool WhiteWin { get; set; }
        public static bool BlackWin { get; set; }
        public static System.Diagnostics.Stopwatch TimeWhite { get; set; }
        public static System.Diagnostics.Stopwatch TimeBlack { get; set; }
        public static System.EventArgs TimeEndedArgs { get; set; }
        public static System.TimeSpan MaxTimeWhite { get; set; }
        public static System.TimeSpan MaxTimeBlack{ get; set; }

        /// <summary>
        /// Сделать ход в позиции (возвращает - возможен ли он)
        /// </summary>
        public static bool TryMoved(MoveCoord move)
        {
            if (GamePosition.Board[move.xStart, move.yStart].Figure.Type == TypeFigur.king)
            {
                if (move.yStart == 4)
                {
                    if ((move.xStart == 0 && GamePosition.IsWhiteMove) || (move.xStart == 7 && GamePosition.IsWhiteMove == false))
                    {
                        if (move.yEnd == 2 || move.yEnd == 6) move.IsCastling = true;
                    }
                }
            }
            if (GamePosition.Board[move.xStart, move.yStart].Figure.CheckMove(GamePosition, move))
            {
                Positions.Add((Position)GamePosition.DeepCopy());
                MoveCoord mc = new MoveCoord();
                mc.xStart = move.xStart;
                mc.yStart = move.yStart;
                mc.yEnd = move.yEnd;
                mc.xEnd = move.xEnd;
                mc.StartFigure = move.StartFigure;
                Moves.Add(mc);
                int coll = 0;
                GamePosition.MoveChess(move);
                if (move.EndFigure == new NotFigur() && !move.IsEnPassant) MovesWithoutEating++;
                else MovesWithoutEating = 0;
                foreach (Position p in Positions)
                {
                    if (p.Equals(GamePosition)) coll++;
                }
                if (GamePosition.IsWhiteMove)
                {
                    TimeBlack.Stop();
                    TimeWhite.Start();
                    int? i= CheckPosition(GamePosition);
                    if(i<0)
                    {
                        BlackWin = true;
                        StopGame();
                    }
                    else if(i == 0)
                    {
                        StopGame();
                    }
                    else if (coll > 2)
                    {
                        MessageBox.Show("Троекратное повторение ходов. Ничья!");
                        MovesWithoutEating = 50;
                        StopGame();
                    }
                    else if(MovesWithoutEating > 49)
                    {
                        MessageBox.Show("50 ходов без взятий. Ничья!");
                        StopGame();
                    }
                    return true;
                }
                else
                {
                    TimeWhite.Stop();
                    TimeBlack.Start();
                    int? i = CheckPosition(GamePosition);
                    if (i>0)
                    {
                        WhiteWin = true;
                        StopGame();
                    }
                    if (i == 0)
                    {
                        StopGame();
                    }
                    if (coll > 2 || MovesWithoutEating > 49) StopGame();
                    return true;
                }
            }
            move = new MoveCoord();
            return false;
        }

        /// <summary>
        /// Проверяет позицию на наличие мата или пата
        /// </summary>
        /// <returns>null при отсутствии мата или пата. Ноль при пате. Положительное число при мате черным. Отрициательное - при мате белым</returns>
        public static int? CheckPosition(Position position1)
        {
            Position position = (Position)position1.DeepCopy();
            position.IsMat = false;
            position.IsPat = false;
            bool isLegal = false;


            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if ((position.Board[i, j].Figure.Color == ColorFigur.white && position.IsWhiteMove)
                    || (position.Board[i, j].Figure.Color == ColorFigur.black && !position.IsWhiteMove))
                    {
                        if (CheckMovesForFigur(position, i, j))
                        {
                            isLegal = true;
                            goto l1;
                        }
                    }
                }
            }
            l1:
            if (!isLegal)
            {
                position.IsPat = true;
                if (position.IsWhiteMove)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            if (position.Board[i, j] == new Square(new King(ColorFigur.white)))
                            {
                                //isStop = true;
                                AttackChecker.CheckAttack(ref position, i, j, ColorFigur.black);
                                if (position.Board[i, j].IsAttackBlack)
                                {
                                    position.IsMat = true;
                                    position.IsPat = false;
                                }
                                break;
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            if (position.Board[i, j] == new Square(new King(ColorFigur.black)))
                            {
                                AttackChecker.CheckAttack(ref position, i, j, ColorFigur.white);
                                if (position.Board[i, j].IsAttackWhite)
                                {
                                    position.IsMat = true;
                                    position.IsPat = false;
                                }
                                break;
                            }
                        }
                    }
                }
                if (position.IsPat) return 0;
                if(position.IsMat)
                {
                    if (position.IsWhiteMove) return -100000;
                    else return 100000;
                }
                else return null;
            }
            else return null;
        }

        /// <summary>
        /// Проверка всех ходов для данной фигуры
        /// </summary>
        /// <param name="position"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        private static bool CheckMovesForFigur(Position position, int i, int j)
        {
            bool isLegal = false;
            switch (position.Board[i, j].Figure.Type)
            {
                case TypeFigur.king:
                    isLegal = CheckKing(position, i, j); break;
                case TypeFigur.queen:
                    isLegal = CheckQueen(position, i, j); break;
                case TypeFigur.rock:
                    isLegal = CheckRock(position, i, j); break;
                case TypeFigur.bishop:
                    isLegal = CheckBishop(position, i, j); break;
                case TypeFigur.knight:
                    isLegal = CheckKnight(position, i, j); break;
                case TypeFigur.peen:
                    isLegal = CheckPeen(position, i, j); break;
            }
            return isLegal;
        }
        private static bool CheckPeen(Position position, int i, int j)
        {
            MoveCoord mc = new MoveCoord();
            mc.xStart = i;
            mc.yStart = j;
            mc.StartFigure = position.Board[i, j].Figure;
            if(position.IsWhiteMove)
            {
                mc.yEnd = mc.yStart;
                if(i==1)
                {
                    mc.xEnd = 3;
                    if(mc.StartFigure.CheckMove(position, mc))return true;
                }
                mc.xEnd = mc.xStart + 1;
                if (mc.StartFigure.CheckMove(position, mc)) return true;
                if(j>0)
                {
                    mc.yEnd = mc.yStart - 1;
                    if (mc.StartFigure.CheckMove(position, mc)) return true;
                }
                if(j<7)
                {
                    mc.yEnd = mc.yStart + 1;
                    if (mc.StartFigure.CheckMove(position, mc)) return true;
                }
            }
            else
            {
                mc.yEnd = mc.yStart;
                if (i == 6)
                {
                    mc.xEnd = 4;
                    if (mc.StartFigure.CheckMove(position, mc)) return true;
                }
                mc.xEnd = mc.xStart - 1;
                if (mc.StartFigure.CheckMove(position, mc)) return true;
                if (j > 0)
                {
                    mc.yEnd = mc.yStart - 1;
                    if (mc.StartFigure.CheckMove(position, mc)) return true;
                }
                if (j < 7)
                {
                    mc.yEnd = mc.yStart + 1;
                    if (mc.StartFigure.CheckMove(position, mc)) return true;
                }
            }
            return false;
        }

        private static bool CheckKnight(Position position, int i, int j)
        {
            MoveCoord mc = new MoveCoord();
            mc.xStart = i;
            mc.yStart = j;
            mc.StartFigure = position.Board[i, j].Figure;
            if(i+1<8)
            {
                if(i+2<8)
                {
                    if (j + 1 < 8)
                    {
                        mc.xEnd = mc.xStart + 2;
                        mc.yEnd = mc.yStart + 1;
                        if (mc.StartFigure.CheckMove(position, mc)) return true;
                    }
                    if(j - 1 >= 0)
                    {
                        mc.xEnd = mc.xStart + 2;
                        mc.yEnd = mc.yStart - 1;
                        if (mc.StartFigure.CheckMove(position, mc)) return true;
                    }  
                }
                if (j + 2 < 8)
                {
                    mc.xEnd = mc.xStart + 1;
                    mc.yEnd = mc.yStart + 2;
                    if (mc.StartFigure.CheckMove(position, mc)) return true;
                }
                if (j - 2 >= 0)
                {
                    mc.xEnd = mc.xStart + 1;
                    mc.yEnd = mc.yStart - 2;
                    if (mc.StartFigure.CheckMove(position, mc)) return true;
                }
            }
            if (i - 1 >= 0)
            {
                if (i - 2 >= 0)
                {
                    if (j + 1 < 8)
                    {
                        mc.xEnd = mc.xStart - 2;
                        mc.yEnd = mc.yStart + 1;
                        if (mc.StartFigure.CheckMove(position, mc)) return true;
                    }
                    if (j - 1 >= 0)
                    {
                        mc.xEnd = mc.xStart - 2;
                        mc.yEnd = mc.yStart - 1;
                        if (mc.StartFigure.CheckMove(position, mc)) return true;
                    }
                }
                if (j + 2 < 8)
                {
                    mc.xEnd = mc.xStart - 1;
                    mc.yEnd = mc.yStart + 2;
                    if (mc.StartFigure.CheckMove(position, mc)) return true;
                }
                if (j - 2 >= 0)
                {
                    mc.xEnd = mc.xStart - 1;
                    mc.yEnd = mc.yStart - 2;
                    if (mc.StartFigure.CheckMove(position, mc)) return true;
                }
            }
            return false;
        }

        private static bool CheckBishop(Position position, int i, int j)
        {
            MoveCoord mc = new MoveCoord();
            mc.xStart = i;
            mc.yStart = j;
            mc.StartFigure = position.Board[i, j].Figure;
            bool isLegal = false;
            for (int x = 0; x < 8; x++)
            {
                if (mc.xStart + x < 8)
                {
                    if (mc.yStart + x < 8)
                    {
                        mc.xEnd = mc.xStart + x;
                        mc.yEnd = mc.yStart + x;
                        if (mc.StartFigure.CheckMove(position, mc))
                        {
                            isLegal = true;
                            break;
                        }
                    }
                    if (mc.yStart - x >= 0)
                    {
                        mc.xEnd = mc.xStart + x;
                        mc.yEnd = mc.yStart - x;
                        if (mc.StartFigure.CheckMove(position, mc))
                        {
                            isLegal = true;
                            break;
                        }
                    }
                }
                if (mc.xStart - x >= 0)
                {
                    if (mc.yStart + x < 8)
                    {
                        mc.xEnd = mc.xStart - x;
                        mc.yEnd = mc.yStart + x;
                        if (mc.StartFigure.CheckMove(position, mc))
                        {
                            isLegal = true;
                            break;
                        }
                    }
                    if (mc.yStart - x >= 0)
                    {
                        mc.xEnd = mc.xStart - x;
                        mc.yEnd = mc.yStart - x;
                        if (mc.StartFigure.CheckMove(position, mc))
                        {
                            isLegal = true;
                            break;
                        }
                    }
                }
            }
            return isLegal;
        }

        private static bool CheckRock(Position position, int i, int j)
        {
            MoveCoord mc = new MoveCoord();
            mc.xStart = i;
            mc.yStart = j;
            mc.StartFigure = position.Board[i, j].Figure;
            bool isLegal = false;
            for (int x = 0; x < 8; x++)
            {
                if (mc.xStart + x < 8)
                {
                    mc.xEnd = mc.xStart + x;
                    mc.yEnd = mc.yStart;
                    if (mc.StartFigure.CheckMove(position, mc))
                    {
                        isLegal = true;
                        break;
                    }
                }
                if (mc.xStart - x >= 0)
                {
                    mc.xEnd = mc.xStart - x;
                    mc.yEnd = mc.yStart;
                    if (mc.StartFigure.CheckMove(position, mc))
                    {
                        isLegal = true;
                        break;
                    }
                }
                if (mc.yStart + x < 8)
                {
                    mc.xEnd = mc.xStart;
                    mc.yEnd = mc.yStart + x;
                    if (mc.StartFigure.CheckMove(position, mc))
                    {
                        isLegal = true;
                        break;
                    }
                }
                if (mc.yStart - x >= 0)
                {
                    mc.xEnd = mc.xStart;
                    mc.yEnd = mc.yStart - x;
                    if (mc.StartFigure.CheckMove(position, mc))
                    {
                        isLegal = true;
                        break;
                    }
                }
            }
            return isLegal;
        }

        private static bool CheckQueen(Position position, int i, int j)
        {
            return (CheckBishop(position, i, j) || CheckRock(position, i, j));
        }

        private static bool CheckKing(Position position, int i, int j)
        {
            MoveCoord mc = new MoveCoord();
            mc.xStart = i;
            mc.yStart = j;
            mc.StartFigure = position.Board[i, j].Figure;
            int x = 1;
            if (position.IsWhiteMove)
            {
                if (i == 0 && j == 4 && !position.IsKingMovedWhite)
                {
                    if(!position.IsLeftRockMovedWhite)
                    {
                        mc.xEnd = i;
                        mc.yEnd = 2;
                        if (mc.StartFigure.CheckMove(position, mc)) return true;
                    }
                    if(!position.IsRightRockMovedWhite)
                    {
                        mc.xEnd = i;
                        mc.yEnd = 6;
                        if (mc.StartFigure.CheckMove(position, mc)) return true;
                    }
                }
            }
            else
            {
                if (i == 7 && j == 4 && !position.IsKingMovedBlack)
                {
                    if (!position.IsLeftRockMovedBlack)
                    {
                        mc.xEnd = i;
                        mc.yEnd = 2;
                        if (mc.StartFigure.CheckMove(position, mc)) return true;
                    }
                    if (!position.IsRightRockMovedBlack)
                    {
                        mc.xEnd = i;
                        mc.yEnd = 6;
                        if (mc.StartFigure.CheckMove(position, mc)) return true;
                    }
                }
            }
            if (mc.xStart + x < 8)
            {
                mc.xEnd = mc.xStart + x;
                mc.yEnd = mc.yStart;
                if (mc.StartFigure.CheckMove(position, mc))
                {
                    return true;
                }
            }
            if (mc.xStart - x >= 0)
            {
                mc.xEnd = mc.xStart - x;
                mc.yEnd = mc.yStart;
                if (mc.StartFigure.CheckMove(position, mc))
                {
                    return true;
                }
            }
            if (mc.yStart + x < 8)
            {
                mc.xEnd = mc.xStart;
                mc.yEnd = mc.yStart + x;
                if (mc.StartFigure.CheckMove(position, mc))
                {
                    return true;
                }
            }
            if (mc.yStart - x >= 0)
            {
                mc.xEnd = mc.xStart;
                mc.yEnd = mc.yStart - x;
                if (mc.StartFigure.CheckMove(position, mc))
                {
                    return true;
                }
            }
            if (mc.xStart + x < 8)
            {
                if (mc.yStart + x < 8)
                {
                    mc.xEnd = mc.xStart + x;
                    mc.yEnd = mc.yStart + x;
                    if (mc.StartFigure.CheckMove(position, mc))
                    {
                        return true;
                    }
                }
                if (mc.yStart - x >= 0)
                {
                    mc.xEnd = mc.xStart + x;
                    mc.yEnd = mc.yStart - x;
                    if (mc.StartFigure.CheckMove(position, mc))
                    {
                        return true;
                    }
                }
            }
            if (mc.xStart - x >= 0)
            {
                if (mc.yStart + x < 8)
                {
                    mc.xEnd = mc.xStart - x;
                    mc.yEnd = mc.yStart + x;
                    if (mc.StartFigure.CheckMove(position, mc))
                    {
                        return true;
                    }
                }
                if (mc.yStart - x >= 0)
                {
                    mc.xEnd = mc.xStart - x;
                    mc.yEnd = mc.yStart - x;
                    if (mc.StartFigure.CheckMove(position, mc))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        
        
        /// <summary>
        /// Проверяет не иссякло ли время у одной из сторон
        /// </summary>
        public static void CheckTime(object obj)
        {
            if (TimeBlack.Elapsed > MaxTimeBlack)
            {
                WhiteWin = true;
                TimeBlack.Stop();
                TimeWhite.Stop();
                StopGame();
                EventTimeEnded?.Invoke(TimeBlack,TimeEndedArgs);
            }
            else if (TimeWhite.Elapsed > MaxTimeWhite)
            {
                BlackWin = true;
                TimeBlack.Stop();
                TimeWhite.Stop();
                StopGame();
                EventTimeEnded?.Invoke(TimeWhite, TimeEndedArgs);
            }
        }
    }
}