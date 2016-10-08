using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YanChess.GameLogic;
using System.Threading;

namespace YanChess.UnitTests
{
    [TestClass]
    public class UnitTestLogic
    {
        /// <summary>
        /// Создание клетки и фигур
        /// </summary>
        [TestMethod]
        public void CreateSquare()
        {
            var p1 = new Square(new Peen(ColorFigur.black));
            var p2 = new Square(new Peen(ColorFigur.black));
            var k1 = new Square(new King(ColorFigur.white));
            var empt1 = new Square();
            empt1.IsAttackBlack = true;


            Assert.AreEqual(p1.Figure.Type,TypeFigur.peen);
            Assert.AreEqual(p1.Figure.Color, ColorFigur.black);
            Assert.AreEqual(p1.IsAttackBlack, false);
            Assert.AreEqual(p1.IsAttackWhite, false);

            Assert.AreEqual(k1.Figure.Type, TypeFigur.king);
            Assert.AreEqual(k1.Figure.Color, ColorFigur.white);
            Assert.AreEqual(k1.IsAttackBlack, false);
            Assert.AreEqual(k1.IsAttackWhite, false);

            Assert.AreEqual(empt1.Figure.Type, TypeFigur.none);
            Assert.AreEqual(empt1.Figure.Color, ColorFigur.none);
            Assert.AreEqual(empt1.IsAttackBlack, true);
            Assert.AreEqual(empt1.IsAttackWhite, false);
            Figure f = new Peen(ColorFigur.white);
            Figure f1 = new Peen(ColorFigur.white);
            Assert.AreEqual(f.Equals(f1), true);
            Assert.AreEqual(p1.Equals(p2), true);
            Assert.AreEqual(p1.Equals(k1), false);
        }

        /// <summary>
        /// Создание позиции
        /// </summary>
        [TestMethod]
        public void CreatePosition()
        {
            Position pos = new Position();
            Position startPos = new Position("start");
            Position startPos1 = new Position("start");
            Position p = new Position("gdfg");
            int i = 0;
            bool bul = false;
            bool bulS = false;
            foreach (Square s in pos.Board)
            {
                if (s.Figure.Type != TypeFigur.none) bul = true;
                i++;
            }
            int coll = 0;
            foreach (Square s in startPos.Board)
            {
                if (s.Figure.Type != TypeFigur.none)
                {
                    bulS = true;
                    coll++;
                }
            }

            int coll1 = 0;
            foreach (Square s in p.Board)
            {
                if (s.Figure.Type != TypeFigur.none)
                {
                    coll1++;
                }
            }
            Assert.AreEqual(i, 64);
            Assert.AreEqual(coll1, 0);
            Assert.AreEqual(bul,false);
            Assert.AreEqual(bulS, true);
            Assert.AreEqual(coll, 32);
            Assert.AreEqual(startPos.IsKingMovedBlack, false);
            Assert.AreEqual(startPos.IsKingMovedWhite, false);
            Assert.AreEqual(startPos.Board[0, 4].Figure.Type, TypeFigur.king);
            Assert.AreEqual(startPos.Board[0, 4].Figure.Color, ColorFigur.white);
            Assert.AreEqual(startPos.IsWhiteMove, true);
            Assert.AreEqual(startPos.IsMat, false);
            Assert.AreEqual(startPos.IsPat, false);


            Assert.AreEqual(startPos.Equals(p), false);
            Assert.AreEqual(startPos.Equals(startPos1), true);
        }

        /// <summary>
        /// Полное копирование позиции
        /// </summary>
        [TestMethod]
        public void DeepCopyPosition()
        {
            Position p = new Position("start");
            Position p1 = (Position)p.DeepCopy();
            Position p2 = (Position)p.DeepCopy();
            p1.Board[0, 0].Figure.Type = TypeFigur.queen;

            Assert.AreNotEqual(p1.Board[0, 0].Figure.Type, p.Board[0, 0].Figure);
            Assert.AreEqual(p.Equals(p2),true);
            Assert.AreEqual(p1.Board[0, 4].Figure.Type, TypeFigur.king);
            Assert.AreEqual(p1.Board[0, 4].Figure.Color, ColorFigur.white);
        }

        /// <summary>
        /// Перемещение фигуры
        /// </summary>
        [TestMethod]
        public void MovePosition()
        {
            Position p = new Position();
            p.Board[2, 2] = new Square(new Bishop(ColorFigur.white));
            MoveCoord mc = new MoveCoord();
            mc.xStart = 2;
            mc.yStart = 2;
            mc.yEnd = 6;
            mc.xEnd = 3;
            mc.IsCastling = false;
            mc.StartFigure = p.Board[2, 2].Figure;
            mc.EndFigure = new NotFigur();
            p.Move(mc);
            Assert.AreEqual(p.Board[2, 2], new Square(new NotFigur()));
            Assert.AreEqual(p.Board[3, 6], new Square(new Bishop(ColorFigur.white)));
        }

        /// <summary>
        /// Проверка хода с изменением переменных позиции
        /// </summary>
        [TestMethod]
        public void MoveChess()
        {
            Position p = new Position();
            p.IsWhiteMove = true;
            p.Board[0, 4] = new Square(new King(ColorFigur.white));
            p.Board[0, 7] = new Square(new Rock(ColorFigur.white));
            MoveCoord mc = new MoveCoord();
            mc.StartFigure = new King(ColorFigur.white);
            mc.xStart = 0;
            mc.yStart = 4;
            mc.EndFigure = new NotFigur();
            mc.xEnd = 0;
            mc.yEnd = 6;
            mc.IsCastling = true;
            p.MoveChess(mc);

            Position p1 = new Position();
            p1.Board[4, 4] = new Square(new Peen(ColorFigur.white));
            p1.Board[4, 5] = new Square(new Peen(ColorFigur.black));
            ((Peen)p1.Board[4, 5].Figure).IsEnPassant = true;
            MoveCoord mc1 = new MoveCoord();
            mc1.StartFigure = new Peen(ColorFigur.white);
            mc1.xStart = 4;
            mc1.yStart = 4;
            mc1.xEnd = 5;
            mc1.yEnd = 5;
            mc1.IsEnPassant = true;
            p1.MoveChess(mc1);

            Position p2 = new Position();
            p2.IsWhiteMove = true;
            p2.Board[1, 1] = new Square(new Peen(ColorFigur.white));
            MoveCoord mc2 = new MoveCoord();
            mc2.StartFigure = new Peen(ColorFigur.white);
            mc2.xStart = 1;
            mc2.yStart = 1;
            mc2.xEnd = 3;
            mc2.yEnd = 1;
            p2.MoveChess(mc2);

            Assert.AreEqual(((Peen)p2.Board[3, 1].Figure).IsEnPassant, true);
            Assert.AreEqual(p.IsWhiteMove,false);
            Assert.AreEqual(p.IsKingMovedWhite, true);
            Assert.AreEqual(p.Board[0, 7], new Square(new NotFigur()));
            Assert.AreEqual(p.Board[0, 6], new Square(new King(ColorFigur.white)));
            Assert.AreEqual(p.Board[0, 5], new Square(new Rock(ColorFigur.white)));
            Assert.AreEqual(p.Board[0, 4], new Square(new NotFigur()));

            Assert.AreEqual(p1.Board[5, 5], new Square(new Peen(ColorFigur.white)));
            Assert.AreEqual(p1.Board[4, 5], new Square(new NotFigur()));
            Assert.AreEqual(p1.Board[4, 4], new Square(new NotFigur()));
        }
        /// <summary>
        /// Проверка наличия пата или мата
        /// </summary>
        [TestMethod]
        public void CheckPateAndMate()
        {
            int? i1, i2, i3, iMate;
            Position p = new Position("start");//ничего нет
            Position p1 = new Position();//пустая позиция - пат
            Position p2 = new Position();//патовая позиция
            p2.IsWhiteMove = true;
            p2.Board[0, 0] = new Square(new King(ColorFigur.white));
            p2.Board[1, 2] = new Square(new Queen(ColorFigur.black));
            i1=GameLogic.GameLogic.CheckPosition(p);
            i2=GameLogic.GameLogic.CheckPosition(p1);
            i3=GameLogic.GameLogic.CheckPosition(p2);
            
            Position pMate = new Position();//матовая позиция
            pMate.Board[0, 0] = new Square(new King(ColorFigur.white));
            pMate.Board[0, 2] = new Square(new King(ColorFigur.black));
            pMate.Board[0, 1] = new Square(new Queen(ColorFigur.black));
            pMate.IsWhiteMove = true;
            iMate=GameLogic.GameLogic.CheckPosition(pMate);
            var p4 = new Position("start");
            Assert.AreEqual(iMate < 0, true);
            Assert.AreEqual(i1 == null, true);
            Assert.AreEqual(i2 == 0, true);
            Assert.AreEqual(i3 == 0, true);
            Assert.AreEqual(p.Equals(p4),true);
        }
        
        /// <summary>
        /// Проверка рокировки
        /// </summary>
        [TestMethod]
        public void CheckKingRook()
        {
            //Создаем позицию с возможными рокировками в обе стороны
            Position p = new Position();
            p.IsWhiteMove = true;
            p.IsKingMovedWhite = false;
            p.IsLeftRockMovedWhite = false;
            p.IsRightRockMovedWhite = false;
            p.Board[0, 4] = new Square(new King(ColorFigur.white));
            p.Board[0, 0] = new Square(new Rock(ColorFigur.white));
            p.Board[0, 7] = new Square(new Rock(ColorFigur.white));
            for(int i=0;i<8;i++)
            {
                for(int j=0;j<=7;j++)
                {
                    p.Board[i, j].IsAttackBlack = false;//ни одна из клеток не атакована противником
                }
            }
            MoveCoord mcL = new MoveCoord();//Длинная рокировка
            MoveCoord mcR = new MoveCoord();//Короткая рокировка
            mcL.xStart = mcR.xStart = 0;
            mcL.yStart = mcR.yStart = 4;
            mcL.xEnd = mcR.xEnd = 0;
            mcL.yEnd = 2;
            mcR.yEnd = 6;
            mcL.StartFigure = new King(ColorFigur.white);
            mcR.StartFigure = new King(ColorFigur.white);
            mcL.NewFigure = new King(ColorFigur.white);
            mcR.NewFigure = new King(ColorFigur.white);

            Assert.AreEqual(mcL.StartFigure.CheckMove(p, mcL),true);
            Assert.AreEqual(mcR.StartFigure.CheckMove(p, mcR), true);
            //Варианты при которых рокировка невозможна
            //атакованы промежуточные клетки
            p.Board[4, 3] = new Square(new Queen(ColorFigur.black));
            Assert.AreEqual(mcL.StartFigure.CheckMove(p, mcL), false);
            p.Board[4, 3] = new Square(new NotFigur());
            p.Board[4, 5] = new Square(new Queen(ColorFigur.black));
            Assert.AreEqual(mcR.StartFigure.CheckMove(p, mcR), false);
            p.Board[4, 4] = new Square(new NotFigur());
            //король под шахом
            p.Board[4, 4] = new Square(new Queen(ColorFigur.black));
            Assert.AreEqual(mcL.StartFigure.CheckMove(p, mcL), false);
            Assert.AreEqual(mcR.StartFigure.CheckMove(p, mcR), false);
            p.Board[4, 4] = new Square(new NotFigur());
            //атакованы конечные клетки
            p.Board[4, 6] = new Square(new Queen(ColorFigur.black));
            p.Board[4, 2] = new Square(new Queen(ColorFigur.black));
            Assert.AreEqual(mcL.StartFigure.CheckMove(p, mcL), false);
            Assert.AreEqual(mcR.StartFigure.CheckMove(p, mcR), false);
            p.Board[4, 6] = new Square(new NotFigur());
            p.Board[4, 2] = new Square(new NotFigur());
            //король уже ходил
            p.IsKingMovedWhite = true;
            Assert.AreEqual(mcL.StartFigure.CheckMove(p, mcL), false);
            Assert.AreEqual(mcR.StartFigure.CheckMove(p, mcR), false);
            p.IsKingMovedWhite = false;
            //ладья уже ходила
            p.IsLeftRockMovedWhite = true;
            p.IsRightRockMovedWhite = true;
            Assert.AreEqual(mcL.StartFigure.CheckMove(p, mcL), false);
            Assert.AreEqual(mcR.StartFigure.CheckMove(p, mcR), false);
            p.IsLeftRockMovedWhite = false;
            p.IsRightRockMovedWhite = false;
            //отсутствуют ладьи
            p.Board[0, 0] = new Square(new NotFigur());
            p.Board[0, 7] = new Square(new NotFigur());
            Assert.AreEqual(mcL.StartFigure.CheckMove(p, mcL), false);
            Assert.AreEqual(mcR.StartFigure.CheckMove(p, mcR), false);
        }

        /// <summary>
        /// Проверка правильности ходов короля
        /// </summary>
        [TestMethod]
        public void CheckKing()
        {
            Position p = new Position("start");//король не имеет ходов
            MoveCoord mc = new MoveCoord();
            mc.xStart = 0;
            mc.yStart = 4;
            mc.StartFigure = new King(ColorFigur.white);
            //ни один ход не будет возможен;
            mc.xEnd = 1;
            mc.yEnd = 3;
            Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), false);
            mc.xEnd = 1;
            mc.yEnd = 4;
            Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), false);
            mc.xEnd = 1;
            mc.yEnd = 5;
            Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), false);
            mc.xEnd = 0;
            mc.yEnd = 5;
            Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), false);
            mc.xEnd = 0;
            mc.yEnd = 3;
            Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), false);

            p = new Position();
            p.Board[4, 4] = new Square(new King(ColorFigur.white));
            p.IsWhiteMove = true;
            //все ходы возможны
            mc.xStart = 4;
            mc.yStart = 4;
            mc.StartFigure = new King(ColorFigur.white);
            mc.xEnd = 4;
            mc.yEnd = 3;
            Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), true);
            mc.xEnd = 4;
            mc.yEnd = 5;
            Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), true);
            mc.xEnd = 3;
            mc.yEnd = 5;
            Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), true);
            mc.xEnd = 3;
            mc.yEnd = 4;
            Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), true);
            mc.xEnd = 3;
            mc.yEnd = 3;
            Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), true);
            mc.xEnd = 5;
            mc.yEnd = 5;
            Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), true);
            mc.xEnd = 5;
            mc.yEnd = 4;
            Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), true);
            //атакованная клетка
            mc.xEnd = 5;
            mc.yEnd = 3;
            p.Board[5, 6] = new Square(new Queen(ColorFigur.black));
            Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), false);
            p.Board[5, 6] = new Square(new NotFigur());
            //вне диапазона ходов
            mc.xEnd = 5;
            mc.yEnd = 0;
            Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), false);
        }

        /// <summary>
        /// Проверка правильности ходов коня
        /// </summary>
        [TestMethod]
        public void CheckKnight()
        {
            Position p = new Position();
            p.IsWhiteMove = true;
            p.Board[4, 4] = new Square(new Knight(ColorFigur.white));
            p.Board[5, 2] = new Square(new Peen(ColorFigur.white));
            MoveCoord mc = new MoveCoord();
            mc.xStart = 4;
            mc.yStart = 4;
            mc.StartFigure = new Knight(ColorFigur.white);
            //ход не будет возможен т.к. на клетке пешка;
            mc.xEnd = 5;
            mc.yEnd = 2;
            Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), false);
            //Остальные ходы возможны
            mc.xEnd = 5;
            mc.yEnd = 6;
            Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), true);
            mc.xEnd = 6;
            mc.yEnd = 3;
            Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), true);
            mc.xEnd = 6;
            mc.yEnd = 5;
            Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), true);
            mc.xEnd = 3;
            mc.yEnd = 6;
            Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), true);
            mc.xEnd = 3;
            mc.yEnd = 2;
            Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), true);
            mc.xEnd = 2;
            mc.yEnd = 3;
            Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), true);
            mc.xEnd = 2;
            mc.yEnd = 3;
            Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), true);
            //Король атакован после хода
            p.Board[0, 0] = new Square(new King(ColorFigur.white));
            p.Board[0, 1] = new Square(new Queen(ColorFigur.black));
            Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), false);
            p.Board[0, 1] = new Square(new NotFigur());
            //Вне диапозона
            mc.xEnd = 7;
            mc.yEnd = 7;
            Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), false);
        }

        /// <summary>
        /// Проверка правильности ходов пешки
        /// </summary>
        [TestMethod]
        public void CheckPeen()
        {
            Position p = new Position();
            p.IsWhiteMove = true;
            p.Board[1, 4] = new Square(new Peen(ColorFigur.white));
            MoveCoord mc = new MoveCoord();
            mc.xStart = 1;
            mc.yStart = 4;
            mc.yEnd = 4;
            mc.StartFigure = new Peen(ColorFigur.white);

            //возможные ходы
            mc.xEnd = 2;
            Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), true);
            mc.xEnd = 3;
            Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), true);
            //возможно взятие на проходе
            p.Board[4, 4] = new Square(new Peen(ColorFigur.white));
            p.Board[4, 5] = new Square(new Peen(ColorFigur.black));
            ((Peen)p.Board[4, 5].Figure).IsEnPassant = true;
            p.Board[5, 3] = new Square(new Peen(ColorFigur.black));
            mc.xStart = 4;
            mc.yStart = 4;
            mc.xEnd = 5;
            mc.yEnd = 5;
            Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), true);
            //обычное взятие
            mc.yEnd = 3;
            Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), true);
            //ход вперед, когда там фигура
            p.Board[5, 4] = new Square(new Peen(ColorFigur.black));
            mc.yEnd = 4;
            Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), false);
            //нельзя взять на проходе
            ((Peen)p.Board[4, 5].Figure).IsEnPassant = false;
            mc.yEnd = 5;
            Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), false);
            //ход, когда король под шахом
            p.Board[0, 0] = new Square(new King(ColorFigur.white));
            p.Board[0, 1] = new Square(new Queen(ColorFigur.black));
            mc.yEnd = 3;
            Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), false);
            p.Board[0, 1] = new Square(new NotFigur());
            //вне диапозона
            mc.yEnd = 4;
            mc.xEnd = 6;
            Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), false);
        }

        /// <summary>
        /// Проверка правильности ходов королевы
        /// </summary>
        [TestMethod]
        public void CheckQueen()
        {
            Position p = new Position();
            p.IsWhiteMove = true;
            p.Board[4, 4] = new Square(new Queen(ColorFigur.white));
            MoveCoord mc = new MoveCoord();
            mc.xStart = 4;
            mc.yStart = 4;
            mc.StartFigure = new Queen(ColorFigur.white);
            mc.NewFigure = mc.StartFigure;

            //Возможные ходы
            mc.xEnd = mc.xStart;
            for (int i = 1; (i + mc.yStart) <= 7; i++)
            {
                mc.yEnd = mc.yStart + i;
                Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), true);
            }
            for (int i = 1; (mc.yStart - i) >= 0; i++)
            {
                mc.yEnd = mc.yStart - i;
                Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), true);
            }
            mc.yEnd = mc.yStart;
            for (int i = 1; (i + mc.xStart) <= 7; i++)
            {
                mc.xEnd = mc.xStart + i;
                Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), true);
            }
            for (int i = 1; (mc.xStart - i) >= 0; i++)
            {
                mc.xEnd = mc.xStart - i;
                Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), true);
            }

            for (int i = 1; ((mc.xStart + i <= 7) && (mc.yStart + i <= 7)) || ((mc.xStart - i >= 0) && (mc.yStart + i <= 7))
                || ((mc.xStart + i <= 7) && (mc.yStart - i >= 0)) || ((mc.xStart - i >= 0) && (mc.yStart - i >= 0)); i++)
            {
                if ((mc.xStart + i <= 7) && (mc.yStart + i <= 7))
                {
                    mc.xEnd = mc.xStart + i;
                    mc.yEnd = mc.yStart + i;
                    Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), true);
                }
                if ((mc.xStart - i >= 0) && (mc.yStart + i <= 7))
                {
                    mc.xEnd = mc.xStart - i;
                    mc.yEnd = mc.yStart + i;
                    Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), true);
                }
                if ((mc.xStart + i <= 7) && (mc.yStart - i >= 0))
                {
                    mc.xEnd = mc.xStart + i;
                    mc.yEnd = mc.yStart - i;
                    Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), true);
                }
                if ((mc.xStart - i >= 0) && (mc.yStart - i >= 0))
                {
                    mc.xEnd = mc.xStart - i;
                    mc.yEnd = mc.yStart - i;
                    Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), true);
                }
            }
            //король под шахом
            p.Board[1, 0] = new Square(new King(ColorFigur.white));
            p.Board[2, 1] = new Square(new Queen(ColorFigur.black));
            Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), false);
            p.Board[2, 1] = new Square(new NotFigur());
            //препятствие
            p.Board[1, 1] = new Square(new Peen(ColorFigur.white));
            Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), false);
            //вне диапозона
            mc.xEnd = 7;
            mc.yEnd = 6;
            Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), false);
        }

        /// <summary>
        /// Проверка правильности ходов ладьи
        /// </summary>
        [TestMethod]
        public void CheckRock()
        {
            Position p = new Position();
            p.IsWhiteMove = true;
            p.Board[4, 4] = new Square(new Rock(ColorFigur.white));
            MoveCoord mc = new MoveCoord();
            mc.xStart = 4;
            mc.yStart = 4;
            mc.StartFigure = new Rock(ColorFigur.white);

            //Возможные ходы
            mc.xEnd = mc.xStart;
            for(int i = 1; (i+mc.yStart)<=7;i++)
            {
                mc.yEnd = mc.yStart + i;
                Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), true);
            }
            for (int i = 1; (mc.yStart-i) >= 0; i++)
            {
                mc.yEnd = mc.yStart - i;
                Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), true);
            }
            mc.yEnd = mc.yStart;
            for (int i = 1; (i + mc.xStart) <= 7; i++)
            {
                mc.xEnd = mc.xStart + i;
                Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), true);
            }
            for (int i = 1; (mc.xStart - i) >= 0; i++)
            {
                mc.xEnd = mc.xStart - i;
                Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), true);
            }
            //король под шахом
            p.Board[0, 0] = new Square(new King(ColorFigur.white));
            p.Board[0, 1] = new Square(new Queen(ColorFigur.black));
            Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), false);
            p.Board[0, 1] = new Square(new NotFigur());
            //путь перегорожден
            p.Board[1, mc.yStart] = new Square(new Peen(ColorFigur.white));
            Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), false);
            //вне диапозона
            mc.yEnd = 7;
            mc.xEnd = 7;
            Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), false);
        }

        /// <summary>
        /// Проверка правильности ходов слона
        /// </summary>
        [TestMethod]
        public void CheckBishop()
        {
            Position p = new Position();
            p.IsWhiteMove = true;
            p.Board[4, 4] = new Square(new Bishop(ColorFigur.white));
            MoveCoord mc = new MoveCoord();
            mc.xStart = 4;
            mc.yStart = 4;
            mc.StartFigure = new Bishop(ColorFigur.white);
            //возможные ходы
            for(int i=1; ((mc.xStart+i<=7)&&(mc.yStart+i<=7))|| ((mc.xStart - i >= 0) && (mc.yStart + i <= 7))
                || ((mc.xStart + i <= 7) && (mc.yStart - i >= 0)) || ((mc.xStart - i >= 0) && (mc.yStart - i >= 0)); i++)
            {
                if((mc.xStart + i <= 7) && (mc.yStart + i <= 7))
                {
                    mc.xEnd = mc.xStart + i;
                    mc.yEnd = mc.yStart + i;
                    Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), true);
                }
                if((mc.xStart - i >= 0) && (mc.yStart + i <= 7))
                {
                    mc.xEnd = mc.xStart - i;
                    mc.yEnd = mc.yStart + i;
                    Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), true);
                }
                if((mc.xStart + i <= 7) && (mc.yStart - i >= 0))
                {
                    mc.xEnd = mc.xStart + i;
                    mc.yEnd = mc.yStart - i;
                    Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), true);
                }
                if((mc.xStart - i >= 0) && (mc.yStart - i >= 0))
                {
                    mc.xEnd = mc.xStart - i;
                    mc.yEnd = mc.yStart - i;
                    Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), true);
                }
            }
            //король под шахом
            p.Board[1, 0] = new Square(new King(ColorFigur.white));
            p.Board[2, 1] = new Square(new Queen(ColorFigur.black));
            Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), false);
            p.Board[2, 1] = new Square(new NotFigur());
            //препятствие
            p.Board[1, 1] = new Square(new Peen(ColorFigur.white));
            Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), false);
            //вне диапозона
            mc.xEnd = 7;
            mc.yEnd = 6;
            Assert.AreEqual(mc.StartFigure.CheckMove(p, mc), false);
        }
        
        /// <summary>
        /// Общая проверка атак
        /// </summary>
        [TestMethod]
        public void CheckAttack()
        {
            Position p = new Position("start");
            GameLogic.AttackChecker.CheckAttack(ref p);
            Assert.AreEqual(p.Board[3, 0].IsAttackWhite, false);
            Assert.AreEqual(p.Board[2, 3].IsAttackWhite, true);

        }
        /// <summary>
        /// Проверка аттаки для одной клетки 
        /// </summary>
        [TestMethod]
        public void CheckAttackForOne()
        {
            Position p = new Position();
            p.Board[4, 4] = new Square(new Queen(ColorFigur.white));
            p.Board[7, 7] = new Square(new Bishop(ColorFigur.black));
            GameLogic.AttackChecker.CheckAttack(ref p, 6, 6, ColorFigur.black);
            Assert.AreEqual(p.Board[6, 6].IsAttackBlack, true);
            Assert.AreEqual(p.Board[6, 6].IsAttackWhite, false);
            GameLogic.AttackChecker.CheckAttackClear(ref p);
            GameLogic.AttackChecker.CheckAttack(ref p, 6, 6, ColorFigur.white);
            Assert.AreEqual(p.Board[6, 6].IsAttackWhite, true);
            Assert.AreEqual(p.Board[6, 6].IsAttackBlack, false);
            GameLogic.AttackChecker.CheckAttackClear(ref p);
            GameLogic.AttackChecker.CheckAttack(ref p, 6, 6, ColorFigur.none);
            Assert.AreEqual(p.Board[6, 6].IsAttackWhite, true);
            Assert.AreEqual(p.Board[6, 6].IsAttackBlack, true);
            GameLogic.AttackChecker.CheckAttack(ref p, 0, 6, ColorFigur.none);
            Assert.AreEqual(p.Board[0, 6].IsAttackWhite, false);
            Assert.AreEqual(p.Board[0, 6].IsAttackBlack, false);
        }
        /// <summary>
        /// Проверка возврата хода
        /// </summary>
        [TestMethod]
        public void BackMove()
        {
            Position p = new Position();
            p.Board[0, 4] = new Square(new King(ColorFigur.white));
            p.Board[0, 7] = new Square(new Rock(ColorFigur.white));
            p.IsWhiteMove = true;
            Position p1 = (Position)p.DeepCopy();
            MoveCoord mc = new MoveCoord();
            mc.xStart = mc.xEnd = 0;
            mc.yStart = 4;
            mc.yEnd = 6;
            mc.IsCastling = true;
            p1.MoveChess(mc);
            Assert.AreEqual(p1.Board[0, 5] == new Square(new Rock(ColorFigur.white)), true);
            p1.MoveBack(mc);
            Assert.AreEqual(p.Equals(p1), true);

            p = new Position();
            p.Board[4, 4] = new Square(new Peen(ColorFigur.white));
            p.Board[4, 5] = new Square(new Peen(ColorFigur.black));
            ((Peen)p.Board[4, 5].Figure).IsEnPassant = true;
            mc = new MoveCoord();
            mc.IsEnPassant = true;
            mc.xStart = 4;
            mc.xEnd = 5;
            mc.yEnd = 5;
            mc.yStart = 4;
            p.IsWhiteMove = true;
            p1 = (Position)p.DeepCopy();
            p1.MoveChess(mc);
            Assert.AreEqual(p1.Board[5, 5] == new Square(new Peen(ColorFigur.white)), true);
            Assert.AreEqual(p1.Board[4, 5] == new Square(new NotFigur()), true);
            p1.MoveBack(mc);
            Assert.AreEqual(p.Equals(p1), true);
        }
    }
}
