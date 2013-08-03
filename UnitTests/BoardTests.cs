using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using PenteInterfaces;
using PenteAI;

namespace UnitTests {
  [TestClass]
  public class BoardTests {
    [TestMethod]
    public void TestDebugConsstructor() {
      var board = new Board(Player.White, 2, 4,
        //123456789012345678
"W.................B" +  // 0
"..................." +  // 1
"..................." +  // 2
"..................." +  // 3
"..................." +  // 4
"..................." +  // 5
"..................." +  // 6
"..................." +  // 7
"..................." +  // 8
".........WBB......." +  // 9 (center)
"..................." +  // 10
"..................." +  // 11
"..........W........" +  // 12
"..................." +  // 13
"............B......" +  // 14
"..................." +  // 15
"..................." +  // 16
"..................." +  // 17
"...................");  // 18

      Assert.AreEqual(Player.White, board.GetSpot(9, 9));
      Assert.AreEqual(Player.White, board.GetSpot(12, 10));
      Assert.AreEqual(Player.White, board.GetSpot(0, 0));
      Assert.AreEqual(Player.Black, board.GetSpot(9, 10));
      Assert.AreEqual(Player.Black, board.GetSpot(9, 11));
      Assert.AreEqual(Player.Black, board.GetSpot(14, 12));
      Assert.AreEqual(Player.Black, board.GetSpot(0, 18));

      Assert.AreEqual(2, board.GetCaptures(Player.White));
      Assert.AreEqual(4, board.GetCaptures(Player.Black));
    }

    [TestMethod]
    public void TestCaptures() {
      var board = new Board(Player.White, 0, 0,
        //123456789012345678
"..................." +  // 0
"..................." +  // 1
"..................." +  // 2
"..................." +  // 3
"..................." +  // 4
"..................." +  // 5
"......W..W..W......" +  // 6
".......B.B.B......." +  // 7
"........BBB........" +  // 8
"......WBB.BBW......" +  // 9 (center)
"........BBB........" +  // 10
".......B.B.B......." +  // 11
"......W..W..W......" +  // 12
"..................." +  // 13
"..................." +  // 14
"..................." +  // 15
"..................." +  // 16
"..................." +  // 17
"...................");  // 18

      Assert.AreEqual(0, board.GetCaptures(Player.White));
      Assert.AreEqual(Player.Black, board.GetSpot(7, 7));
      Assert.AreEqual(Player.Black, board.GetSpot(8, 8));

      Assert.AreEqual(Player.Black, board.GetSpot(7, 9));
      Assert.AreEqual(Player.Black, board.GetSpot(8, 9));

      Assert.AreEqual(Player.Black, board.GetSpot(7, 11));
      Assert.AreEqual(Player.Black, board.GetSpot(8, 10));

      Assert.AreEqual(Player.Black, board.GetSpot(9, 7));
      Assert.AreEqual(Player.Black, board.GetSpot(9, 8));

      Assert.AreEqual(Player.Black, board.GetSpot(9, 10));
      Assert.AreEqual(Player.Black, board.GetSpot(9, 11));

      Assert.AreEqual(Player.Black, board.GetSpot(10, 8));
      Assert.AreEqual(Player.Black, board.GetSpot(11, 7));

      Assert.AreEqual(Player.Black, board.GetSpot(10, 9));
      Assert.AreEqual(Player.Black, board.GetSpot(11, 9));

      Assert.AreEqual(Player.Black, board.GetSpot(10, 10));
      Assert.AreEqual(Player.Black, board.GetSpot(11, 11));

      board.Move(9, 9);
      Assert.AreEqual(8, board.GetCaptures(Player.White));
      Assert.AreEqual(Player.Neither, board.GetSpot(7, 7));
      Assert.AreEqual(Player.Neither, board.GetSpot(8, 8));

      Assert.AreEqual(Player.Neither, board.GetSpot(7, 9));
      Assert.AreEqual(Player.Neither, board.GetSpot(8, 9));

      Assert.AreEqual(Player.Neither, board.GetSpot(7, 11));
      Assert.AreEqual(Player.Neither, board.GetSpot(8, 10));

      Assert.AreEqual(Player.Neither, board.GetSpot(9, 7));
      Assert.AreEqual(Player.Neither, board.GetSpot(9, 8));

      Assert.AreEqual(Player.Neither, board.GetSpot(9, 10));
      Assert.AreEqual(Player.Neither, board.GetSpot(9, 11));

      Assert.AreEqual(Player.Neither, board.GetSpot(10, 8));
      Assert.AreEqual(Player.Neither, board.GetSpot(11, 7));

      Assert.AreEqual(Player.Neither, board.GetSpot(10, 9));
      Assert.AreEqual(Player.Neither, board.GetSpot(11, 9));

      Assert.AreEqual(Player.Neither, board.GetSpot(10, 10));
      Assert.AreEqual(Player.Neither, board.GetSpot(11, 11));
    }

    [TestMethod]
    public void TestCapturesIndividualWhiteMove() {
      Board board;
      board = new Board(Player.White, 0, 0,
        //123456789012345678
"..................." +  // 0
"..................." +  // 1
"..................." +  // 2
"..................." +  // 3
"..................." +  // 4
"..................." +  // 5
"......W............" +  // 6
".......B.B.B......." +  // 7
"........BBB........" +  // 8
".......BB.BB......." +  // 9 (center)
"........BBB........" +  // 10
".......B.B.B......." +  // 11
"..................." +  // 12
"..................." +  // 13
"..................." +  // 14
"..................." +  // 15
"..................." +  // 16
"..................." +  // 17
"...................");  // 18
      Assert.AreEqual(Player.Black, board.GetSpot(7, 7));
      Assert.AreEqual(Player.Black, board.GetSpot(8, 8));
      board.Move(9, 9);
      Assert.AreEqual(Player.Neither, board.GetSpot(7, 7));
      Assert.AreEqual(Player.Neither, board.GetSpot(8, 8));

      Assert.AreEqual(Player.Black, board.GetSpot(7, 9));
      Assert.AreEqual(Player.Black, board.GetSpot(8, 9));

      Assert.AreEqual(Player.Black, board.GetSpot(7, 11));
      Assert.AreEqual(Player.Black, board.GetSpot(8, 10));

      Assert.AreEqual(Player.Black, board.GetSpot(9, 7));
      Assert.AreEqual(Player.Black, board.GetSpot(9, 8));

      Assert.AreEqual(Player.Black, board.GetSpot(9, 10));
      Assert.AreEqual(Player.Black, board.GetSpot(9, 11));

      Assert.AreEqual(Player.Black, board.GetSpot(10, 8));
      Assert.AreEqual(Player.Black, board.GetSpot(11, 7));

      Assert.AreEqual(Player.Black, board.GetSpot(10, 9));
      Assert.AreEqual(Player.Black, board.GetSpot(11, 9));

      Assert.AreEqual(Player.Black, board.GetSpot(10, 10));
      Assert.AreEqual(Player.Black, board.GetSpot(11, 11));


      board = new Board(Player.White, 0, 0,
        //123456789012345678
"..................." +  // 0
"..................." +  // 1
"..................." +  // 2
"..................." +  // 3
"..................." +  // 4
"..................." +  // 5
".........W........." +  // 6
".......B.B.B......." +  // 7
"........BBB........" +  // 8
".......BB.BB......." +  // 9 (center)
"........BBB........" +  // 10
".......B.B.B......." +  // 11
"..................." +  // 12
"..................." +  // 13
"..................." +  // 14
"..................." +  // 15
"..................." +  // 16
"..................." +  // 17
"...................");  // 18
      Assert.AreEqual(Player.Black, board.GetSpot(7, 9));
      Assert.AreEqual(Player.Black, board.GetSpot(8, 9));
      board.Move(9, 9);
      Assert.AreEqual(Player.Black, board.GetSpot(7, 7));
      Assert.AreEqual(Player.Black, board.GetSpot(8, 8));

      Assert.AreEqual(Player.Neither, board.GetSpot(7, 9));
      Assert.AreEqual(Player.Neither, board.GetSpot(8, 9));

      Assert.AreEqual(Player.Black, board.GetSpot(7, 11));
      Assert.AreEqual(Player.Black, board.GetSpot(8, 10));

      Assert.AreEqual(Player.Black, board.GetSpot(9, 7));
      Assert.AreEqual(Player.Black, board.GetSpot(9, 8));

      Assert.AreEqual(Player.Black, board.GetSpot(9, 10));
      Assert.AreEqual(Player.Black, board.GetSpot(9, 11));

      Assert.AreEqual(Player.Black, board.GetSpot(10, 8));
      Assert.AreEqual(Player.Black, board.GetSpot(11, 7));

      Assert.AreEqual(Player.Black, board.GetSpot(10, 9));
      Assert.AreEqual(Player.Black, board.GetSpot(11, 9));

      Assert.AreEqual(Player.Black, board.GetSpot(10, 10));
      Assert.AreEqual(Player.Black, board.GetSpot(11, 11));

      board = new Board(Player.White, 0, 0,
        //123456789012345678
"..................." +  // 0
"..................." +  // 1
"..................." +  // 2
"..................." +  // 3
"..................." +  // 4
"..................." +  // 5
"............W......" +  // 6
".......B.B.B......." +  // 7
"........BBB........" +  // 8
".......BB.BB......." +  // 9 (center)
"........BBB........" +  // 10
".......B.B.B......." +  // 11
"..................." +  // 12
"..................." +  // 13
"..................." +  // 14
"..................." +  // 15
"..................." +  // 16
"..................." +  // 17
"...................");  // 18
      Assert.AreEqual(Player.Black, board.GetSpot(7, 11));
      Assert.AreEqual(Player.Black, board.GetSpot(8, 10));
      board.Move(9, 9);
      Assert.AreEqual(Player.Black, board.GetSpot(7, 7));
      Assert.AreEqual(Player.Black, board.GetSpot(8, 8));

      Assert.AreEqual(Player.Black, board.GetSpot(7, 9));
      Assert.AreEqual(Player.Black, board.GetSpot(8, 9));

      Assert.AreEqual(Player.Neither, board.GetSpot(7, 11));
      Assert.AreEqual(Player.Neither, board.GetSpot(8, 10));

      Assert.AreEqual(Player.Black, board.GetSpot(9, 7));
      Assert.AreEqual(Player.Black, board.GetSpot(9, 8));

      Assert.AreEqual(Player.Black, board.GetSpot(9, 10));
      Assert.AreEqual(Player.Black, board.GetSpot(9, 11));

      Assert.AreEqual(Player.Black, board.GetSpot(10, 8));
      Assert.AreEqual(Player.Black, board.GetSpot(11, 7));

      Assert.AreEqual(Player.Black, board.GetSpot(10, 9));
      Assert.AreEqual(Player.Black, board.GetSpot(11, 9));

      Assert.AreEqual(Player.Black, board.GetSpot(10, 10));
      Assert.AreEqual(Player.Black, board.GetSpot(11, 11));
      board = new Board(Player.White, 0, 0,
        //123456789012345678
"..................." +  // 0
"..................." +  // 1
"..................." +  // 2
"..................." +  // 3
"..................." +  // 4
"..................." +  // 5
"..................." +  // 6
".......B.B.B......." +  // 7
"........BBB........" +  // 8
"......WBB.BB......." +  // 9 (center)
"........BBB........" +  // 10
".......B.B.B......." +  // 11
"..................." +  // 12
"..................." +  // 13
"..................." +  // 14
"..................." +  // 15
"..................." +  // 16
"..................." +  // 17
"...................");  // 18
      Assert.AreEqual(Player.Black, board.GetSpot(9, 7));
      Assert.AreEqual(Player.Black, board.GetSpot(9, 8));
      board.Move(9, 9);
      Assert.AreEqual(Player.Black, board.GetSpot(7, 7));
      Assert.AreEqual(Player.Black, board.GetSpot(8, 8));

      Assert.AreEqual(Player.Black, board.GetSpot(7, 9));
      Assert.AreEqual(Player.Black, board.GetSpot(8, 9));

      Assert.AreEqual(Player.Black, board.GetSpot(7, 11));
      Assert.AreEqual(Player.Black, board.GetSpot(8, 10));

      Assert.AreEqual(Player.Neither, board.GetSpot(9, 7));
      Assert.AreEqual(Player.Neither, board.GetSpot(9, 8));

      Assert.AreEqual(Player.Black, board.GetSpot(9, 10));
      Assert.AreEqual(Player.Black, board.GetSpot(9, 11));

      Assert.AreEqual(Player.Black, board.GetSpot(10, 8));
      Assert.AreEqual(Player.Black, board.GetSpot(11, 7));

      Assert.AreEqual(Player.Black, board.GetSpot(10, 9));
      Assert.AreEqual(Player.Black, board.GetSpot(11, 9));

      Assert.AreEqual(Player.Black, board.GetSpot(10, 10));
      Assert.AreEqual(Player.Black, board.GetSpot(11, 11));
      board = new Board(Player.White, 0, 0,

        //123456789012345678
"..................." +  // 0
"..................." +  // 1
"..................." +  // 2
"..................." +  // 3
"..................." +  // 4
"..................." +  // 5
"..................." +  // 6
".......B.B.B......." +  // 7
"........BBB........" +  // 8
".......BB.BBW......" +  // 9 (center)
"........BBB........" +  // 10
".......B.B.B......." +  // 11
"..................." +  // 12
"..................." +  // 13
"..................." +  // 14
"..................." +  // 15
"..................." +  // 16
"..................." +  // 17
"...................");  // 18
      Assert.AreEqual(Player.Black, board.GetSpot(9, 10));
      Assert.AreEqual(Player.Black, board.GetSpot(9, 11));
      board.Move(9, 9);
      Assert.AreEqual(Player.Black, board.GetSpot(7, 7));
      Assert.AreEqual(Player.Black, board.GetSpot(8, 8));

      Assert.AreEqual(Player.Black, board.GetSpot(7, 9));
      Assert.AreEqual(Player.Black, board.GetSpot(8, 9));

      Assert.AreEqual(Player.Black, board.GetSpot(7, 11));
      Assert.AreEqual(Player.Black, board.GetSpot(8, 10));

      Assert.AreEqual(Player.Black, board.GetSpot(9, 7));
      Assert.AreEqual(Player.Black, board.GetSpot(9, 8));

      Assert.AreEqual(Player.Neither, board.GetSpot(9, 10));
      Assert.AreEqual(Player.Neither, board.GetSpot(9, 11));

      Assert.AreEqual(Player.Black, board.GetSpot(10, 8));
      Assert.AreEqual(Player.Black, board.GetSpot(11, 7));

      Assert.AreEqual(Player.Black, board.GetSpot(10, 9));
      Assert.AreEqual(Player.Black, board.GetSpot(11, 9));

      Assert.AreEqual(Player.Black, board.GetSpot(10, 10));
      Assert.AreEqual(Player.Black, board.GetSpot(11, 11));
      board = new Board(Player.White, 0, 0,
        //123456789012345678
"..................." +  // 0
"..................." +  // 1
"..................." +  // 2
"..................." +  // 3
"..................." +  // 4
"..................." +  // 5
"..................." +  // 6
".......B.B.B......." +  // 7
"........BBB........" +  // 8
".......BB.BB......." +  // 9 (center)
"........BBB........" +  // 10
".......B.B.B......." +  // 11
"......W............" +  // 12
"..................." +  // 13
"..................." +  // 14
"..................." +  // 15
"..................." +  // 16
"..................." +  // 17
"...................");  // 18
      Assert.AreEqual(Player.Black, board.GetSpot(10, 8));
      Assert.AreEqual(Player.Black, board.GetSpot(11, 7));
      board.Move(9, 9);
      Assert.AreEqual(Player.Black, board.GetSpot(7, 7));
      Assert.AreEqual(Player.Black, board.GetSpot(8, 8));

      Assert.AreEqual(Player.Black, board.GetSpot(7, 9));
      Assert.AreEqual(Player.Black, board.GetSpot(8, 9));

      Assert.AreEqual(Player.Black, board.GetSpot(7, 11));
      Assert.AreEqual(Player.Black, board.GetSpot(8, 10));

      Assert.AreEqual(Player.Black, board.GetSpot(9, 7));
      Assert.AreEqual(Player.Black, board.GetSpot(9, 8));

      Assert.AreEqual(Player.Black, board.GetSpot(9, 10));
      Assert.AreEqual(Player.Black, board.GetSpot(9, 11));

      Assert.AreEqual(Player.Neither, board.GetSpot(10, 8));
      Assert.AreEqual(Player.Neither, board.GetSpot(11, 7));

      Assert.AreEqual(Player.Black, board.GetSpot(10, 9));
      Assert.AreEqual(Player.Black, board.GetSpot(11, 9));

      Assert.AreEqual(Player.Black, board.GetSpot(10, 10));
      Assert.AreEqual(Player.Black, board.GetSpot(11, 11));
      board = new Board(Player.White, 0, 0,
        //123456789012345678
"..................." +  // 0
"..................." +  // 1
"..................." +  // 2
"..................." +  // 3
"..................." +  // 4
"..................." +  // 5
"..................." +  // 6
".......B.B.B......." +  // 7
"........BBB........" +  // 8
".......BB.BB......." +  // 9 (center)
"........BBB........" +  // 10
".......B.B.B......." +  // 11
".........W........." +  // 12
"..................." +  // 13
"..................." +  // 14
"..................." +  // 15
"..................." +  // 16
"..................." +  // 17
"...................");  // 18
      Assert.AreEqual(Player.Black, board.GetSpot(10, 9));
      Assert.AreEqual(Player.Black, board.GetSpot(11, 9));
      board.Move(9, 9);
      Assert.AreEqual(Player.Black, board.GetSpot(7, 7));
      Assert.AreEqual(Player.Black, board.GetSpot(8, 8));

      Assert.AreEqual(Player.Black, board.GetSpot(7, 9));
      Assert.AreEqual(Player.Black, board.GetSpot(8, 9));

      Assert.AreEqual(Player.Black, board.GetSpot(7, 11));
      Assert.AreEqual(Player.Black, board.GetSpot(8, 10));

      Assert.AreEqual(Player.Black, board.GetSpot(9, 7));
      Assert.AreEqual(Player.Black, board.GetSpot(9, 8));

      Assert.AreEqual(Player.Black, board.GetSpot(9, 10));
      Assert.AreEqual(Player.Black, board.GetSpot(9, 11));

      Assert.AreEqual(Player.Black, board.GetSpot(10, 8));
      Assert.AreEqual(Player.Black, board.GetSpot(11, 7));

      Assert.AreEqual(Player.Neither, board.GetSpot(10, 9));
      Assert.AreEqual(Player.Neither, board.GetSpot(11, 9));

      Assert.AreEqual(Player.Black, board.GetSpot(10, 10));
      Assert.AreEqual(Player.Black, board.GetSpot(11, 11));
      board = new Board(Player.White, 0, 0,
        //123456789012345678
"..................." +  // 0
"..................." +  // 1
"..................." +  // 2
"..................." +  // 3
"..................." +  // 4
"..................." +  // 5
"..................." +  // 6
".......B.B.B......." +  // 7
"........BBB........" +  // 8
".......BB.BB......." +  // 9 (center)
"........BBB........" +  // 10
".......B.B.B......." +  // 11
"............W......" +  // 12
"..................." +  // 13
"..................." +  // 14
"..................." +  // 15
"..................." +  // 16
"..................." +  // 17
"...................");  // 18
      Assert.AreEqual(Player.Black, board.GetSpot(10, 10));
      Assert.AreEqual(Player.Black, board.GetSpot(11, 11));
      board.Move(9, 9);
      Assert.AreEqual(Player.Black, board.GetSpot(7, 7));
      Assert.AreEqual(Player.Black, board.GetSpot(8, 8));

      Assert.AreEqual(Player.Black, board.GetSpot(7, 9));
      Assert.AreEqual(Player.Black, board.GetSpot(8, 9));

      Assert.AreEqual(Player.Black, board.GetSpot(7, 11));
      Assert.AreEqual(Player.Black, board.GetSpot(8, 10));

      Assert.AreEqual(Player.Black, board.GetSpot(9, 7));
      Assert.AreEqual(Player.Black, board.GetSpot(9, 8));

      Assert.AreEqual(Player.Black, board.GetSpot(9, 10));
      Assert.AreEqual(Player.Black, board.GetSpot(9, 11));

      Assert.AreEqual(Player.Black, board.GetSpot(10, 8));
      Assert.AreEqual(Player.Black, board.GetSpot(11, 7));

      Assert.AreEqual(Player.Black, board.GetSpot(10, 9));
      Assert.AreEqual(Player.Black, board.GetSpot(11, 9));

      Assert.AreEqual(Player.Neither, board.GetSpot(10, 10));
      Assert.AreEqual(Player.Neither, board.GetSpot(11, 11));
    }

    [TestMethod]
    public void TestCapturesIndividualBlackMove() {
      Board board;
      board = new Board(Player.Black, 0, 0,
        //123456789012345678
"..................." +  // 0
"..................." +  // 1
"..................." +  // 2
"..................." +  // 3
"..................." +  // 4
"..................." +  // 5
"......B............" +  // 6
".......W.W.W......." +  // 7
"........WWW........" +  // 8
".......WW.WW......." +  // 9 (center)
"........WWW........" +  // 10
".......W.W.W......." +  // 11
"..................." +  // 12
"..................." +  // 13
"..................." +  // 14
"..................." +  // 15
"..................." +  // 16
"..................." +  // 17
"...................");  // 18
      Assert.AreEqual(Player.White, board.GetSpot(7, 7));
      Assert.AreEqual(Player.White, board.GetSpot(8, 8));
      board.Move(9, 9);
      Assert.AreEqual(Player.Neither, board.GetSpot(7, 7));
      Assert.AreEqual(Player.Neither, board.GetSpot(8, 8));

      Assert.AreEqual(Player.White, board.GetSpot(7, 9));
      Assert.AreEqual(Player.White, board.GetSpot(8, 9));

      Assert.AreEqual(Player.White, board.GetSpot(7, 11));
      Assert.AreEqual(Player.White, board.GetSpot(8, 10));

      Assert.AreEqual(Player.White, board.GetSpot(9, 7));
      Assert.AreEqual(Player.White, board.GetSpot(9, 8));

      Assert.AreEqual(Player.White, board.GetSpot(9, 10));
      Assert.AreEqual(Player.White, board.GetSpot(9, 11));

      Assert.AreEqual(Player.White, board.GetSpot(10, 8));
      Assert.AreEqual(Player.White, board.GetSpot(11, 7));

      Assert.AreEqual(Player.White, board.GetSpot(10, 9));
      Assert.AreEqual(Player.White, board.GetSpot(11, 9));

      Assert.AreEqual(Player.White, board.GetSpot(10, 10));
      Assert.AreEqual(Player.White, board.GetSpot(11, 11));


      board = new Board(Player.Black, 0, 0,
        //123456789012345678
"..................." +  // 0
"..................." +  // 1
"..................." +  // 2
"..................." +  // 3
"..................." +  // 4
"..................." +  // 5
".........B........." +  // 6
".......W.W.W......." +  // 7
"........WWW........" +  // 8
".......WW.WW......." +  // 9 (center)
"........WWW........" +  // 10
".......W.W.W......." +  // 11
"..................." +  // 12
"..................." +  // 13
"..................." +  // 14
"..................." +  // 15
"..................." +  // 16
"..................." +  // 17
"...................");  // 18
      Assert.AreEqual(Player.White, board.GetSpot(7, 9));
      Assert.AreEqual(Player.White, board.GetSpot(8, 9));
      board.Move(9, 9);
      Assert.AreEqual(Player.White, board.GetSpot(7, 7));
      Assert.AreEqual(Player.White, board.GetSpot(8, 8));

      Assert.AreEqual(Player.Neither, board.GetSpot(7, 9));
      Assert.AreEqual(Player.Neither, board.GetSpot(8, 9));

      Assert.AreEqual(Player.White, board.GetSpot(7, 11));
      Assert.AreEqual(Player.White, board.GetSpot(8, 10));

      Assert.AreEqual(Player.White, board.GetSpot(9, 7));
      Assert.AreEqual(Player.White, board.GetSpot(9, 8));

      Assert.AreEqual(Player.White, board.GetSpot(9, 10));
      Assert.AreEqual(Player.White, board.GetSpot(9, 11));

      Assert.AreEqual(Player.White, board.GetSpot(10, 8));
      Assert.AreEqual(Player.White, board.GetSpot(11, 7));

      Assert.AreEqual(Player.White, board.GetSpot(10, 9));
      Assert.AreEqual(Player.White, board.GetSpot(11, 9));

      Assert.AreEqual(Player.White, board.GetSpot(10, 10));
      Assert.AreEqual(Player.White, board.GetSpot(11, 11));

      board = new Board(Player.Black, 0, 0,
        //123456789012345678
"..................." +  // 0
"..................." +  // 1
"..................." +  // 2
"..................." +  // 3
"..................." +  // 4
"..................." +  // 5
"............B......" +  // 6
".......W.W.W......." +  // 7
"........WWW........" +  // 8
".......WW.WW......." +  // 9 (center)
"........WWW........" +  // 10
".......W.W.W......." +  // 11
"..................." +  // 12
"..................." +  // 13
"..................." +  // 14
"..................." +  // 15
"..................." +  // 16
"..................." +  // 17
"...................");  // 18
      Assert.AreEqual(Player.White, board.GetSpot(7, 11));
      Assert.AreEqual(Player.White, board.GetSpot(8, 10));
      board.Move(9, 9);
      Assert.AreEqual(Player.White, board.GetSpot(7, 7));
      Assert.AreEqual(Player.White, board.GetSpot(8, 8));

      Assert.AreEqual(Player.White, board.GetSpot(7, 9));
      Assert.AreEqual(Player.White, board.GetSpot(8, 9));

      Assert.AreEqual(Player.Neither, board.GetSpot(7, 11));
      Assert.AreEqual(Player.Neither, board.GetSpot(8, 10));

      Assert.AreEqual(Player.White, board.GetSpot(9, 7));
      Assert.AreEqual(Player.White, board.GetSpot(9, 8));

      Assert.AreEqual(Player.White, board.GetSpot(9, 10));
      Assert.AreEqual(Player.White, board.GetSpot(9, 11));

      Assert.AreEqual(Player.White, board.GetSpot(10, 8));
      Assert.AreEqual(Player.White, board.GetSpot(11, 7));

      Assert.AreEqual(Player.White, board.GetSpot(10, 9));
      Assert.AreEqual(Player.White, board.GetSpot(11, 9));

      Assert.AreEqual(Player.White, board.GetSpot(10, 10));
      Assert.AreEqual(Player.White, board.GetSpot(11, 11));
      board = new Board(Player.Black, 0, 0,
        //123456789012345678
"..................." +  // 0
"..................." +  // 1
"..................." +  // 2
"..................." +  // 3
"..................." +  // 4
"..................." +  // 5
"..................." +  // 6
".......W.W.W......." +  // 7
"........WWW........" +  // 8
"......BWW.WW......." +  // 9 (center)
"........WWW........" +  // 10
".......W.W.W......." +  // 11
"..................." +  // 12
"..................." +  // 13
"..................." +  // 14
"..................." +  // 15
"..................." +  // 16
"..................." +  // 17
"...................");  // 18
      Assert.AreEqual(Player.White, board.GetSpot(9, 7));
      Assert.AreEqual(Player.White, board.GetSpot(9, 8));
      board.Move(9, 9);
      Assert.AreEqual(Player.White, board.GetSpot(7, 7));
      Assert.AreEqual(Player.White, board.GetSpot(8, 8));

      Assert.AreEqual(Player.White, board.GetSpot(7, 9));
      Assert.AreEqual(Player.White, board.GetSpot(8, 9));

      Assert.AreEqual(Player.White, board.GetSpot(7, 11));
      Assert.AreEqual(Player.White, board.GetSpot(8, 10));

      Assert.AreEqual(Player.Neither, board.GetSpot(9, 7));
      Assert.AreEqual(Player.Neither, board.GetSpot(9, 8));

      Assert.AreEqual(Player.White, board.GetSpot(9, 10));
      Assert.AreEqual(Player.White, board.GetSpot(9, 11));

      Assert.AreEqual(Player.White, board.GetSpot(10, 8));
      Assert.AreEqual(Player.White, board.GetSpot(11, 7));

      Assert.AreEqual(Player.White, board.GetSpot(10, 9));
      Assert.AreEqual(Player.White, board.GetSpot(11, 9));

      Assert.AreEqual(Player.White, board.GetSpot(10, 10));
      Assert.AreEqual(Player.White, board.GetSpot(11, 11));
      board = new Board(Player.Black, 0, 0,

        //123456789012345678
"..................." +  // 0
"..................." +  // 1
"..................." +  // 2
"..................." +  // 3
"..................." +  // 4
"..................." +  // 5
"..................." +  // 6
".......W.W.W......." +  // 7
"........WWW........" +  // 8
".......WW.WWB......" +  // 9 (center)
"........WWW........" +  // 10
".......W.W.W......." +  // 11
"..................." +  // 12
"..................." +  // 13
"..................." +  // 14
"..................." +  // 15
"..................." +  // 16
"..................." +  // 17
"...................");  // 18
      Assert.AreEqual(Player.White, board.GetSpot(9, 10));
      Assert.AreEqual(Player.White, board.GetSpot(9, 11));
      board.Move(9, 9);
      Assert.AreEqual(Player.White, board.GetSpot(7, 7));
      Assert.AreEqual(Player.White, board.GetSpot(8, 8));

      Assert.AreEqual(Player.White, board.GetSpot(7, 9));
      Assert.AreEqual(Player.White, board.GetSpot(8, 9));

      Assert.AreEqual(Player.White, board.GetSpot(7, 11));
      Assert.AreEqual(Player.White, board.GetSpot(8, 10));

      Assert.AreEqual(Player.White, board.GetSpot(9, 7));
      Assert.AreEqual(Player.White, board.GetSpot(9, 8));

      Assert.AreEqual(Player.Neither, board.GetSpot(9, 10));
      Assert.AreEqual(Player.Neither, board.GetSpot(9, 11));

      Assert.AreEqual(Player.White, board.GetSpot(10, 8));
      Assert.AreEqual(Player.White, board.GetSpot(11, 7));

      Assert.AreEqual(Player.White, board.GetSpot(10, 9));
      Assert.AreEqual(Player.White, board.GetSpot(11, 9));

      Assert.AreEqual(Player.White, board.GetSpot(10, 10));
      Assert.AreEqual(Player.White, board.GetSpot(11, 11));
      board = new Board(Player.Black, 0, 0,
        //123456789012345678
"..................." +  // 0
"..................." +  // 1
"..................." +  // 2
"..................." +  // 3
"..................." +  // 4
"..................." +  // 5
"..................." +  // 6
".......W.W.W......." +  // 7
"........WWW........" +  // 8
".......WW.WW......." +  // 9 (center)
"........WWW........" +  // 10
".......W.W.W......." +  // 11
"......B............" +  // 12
"..................." +  // 13
"..................." +  // 14
"..................." +  // 15
"..................." +  // 16
"..................." +  // 17
"...................");  // 18
      Assert.AreEqual(Player.White, board.GetSpot(10, 8));
      Assert.AreEqual(Player.White, board.GetSpot(11, 7));
      board.Move(9, 9);
      Assert.AreEqual(Player.White, board.GetSpot(7, 7));
      Assert.AreEqual(Player.White, board.GetSpot(8, 8));

      Assert.AreEqual(Player.White, board.GetSpot(7, 9));
      Assert.AreEqual(Player.White, board.GetSpot(8, 9));

      Assert.AreEqual(Player.White, board.GetSpot(7, 11));
      Assert.AreEqual(Player.White, board.GetSpot(8, 10));

      Assert.AreEqual(Player.White, board.GetSpot(9, 7));
      Assert.AreEqual(Player.White, board.GetSpot(9, 8));

      Assert.AreEqual(Player.White, board.GetSpot(9, 10));
      Assert.AreEqual(Player.White, board.GetSpot(9, 11));

      Assert.AreEqual(Player.Neither, board.GetSpot(10, 8));
      Assert.AreEqual(Player.Neither, board.GetSpot(11, 7));

      Assert.AreEqual(Player.White, board.GetSpot(10, 9));
      Assert.AreEqual(Player.White, board.GetSpot(11, 9));

      Assert.AreEqual(Player.White, board.GetSpot(10, 10));
      Assert.AreEqual(Player.White, board.GetSpot(11, 11));
      board = new Board(Player.Black, 0, 0,
        //123456789012345678
"..................." +  // 0
"..................." +  // 1
"..................." +  // 2
"..................." +  // 3
"..................." +  // 4
"..................." +  // 5
"..................." +  // 6
".......W.W.W......." +  // 7
"........WWW........" +  // 8
".......WW.WW......." +  // 9 (center)
"........WWW........" +  // 10
".......W.W.W......." +  // 11
".........B........." +  // 12
"..................." +  // 13
"..................." +  // 14
"..................." +  // 15
"..................." +  // 16
"..................." +  // 17
"...................");  // 18
      Assert.AreEqual(Player.White, board.GetSpot(10, 9));
      Assert.AreEqual(Player.White, board.GetSpot(11, 9));
      board.Move(9, 9);
      Assert.AreEqual(Player.White, board.GetSpot(7, 7));
      Assert.AreEqual(Player.White, board.GetSpot(8, 8));

      Assert.AreEqual(Player.White, board.GetSpot(7, 9));
      Assert.AreEqual(Player.White, board.GetSpot(8, 9));

      Assert.AreEqual(Player.White, board.GetSpot(7, 11));
      Assert.AreEqual(Player.White, board.GetSpot(8, 10));

      Assert.AreEqual(Player.White, board.GetSpot(9, 7));
      Assert.AreEqual(Player.White, board.GetSpot(9, 8));

      Assert.AreEqual(Player.White, board.GetSpot(9, 10));
      Assert.AreEqual(Player.White, board.GetSpot(9, 11));

      Assert.AreEqual(Player.White, board.GetSpot(10, 8));
      Assert.AreEqual(Player.White, board.GetSpot(11, 7));

      Assert.AreEqual(Player.Neither, board.GetSpot(10, 9));
      Assert.AreEqual(Player.Neither, board.GetSpot(11, 9));

      Assert.AreEqual(Player.White, board.GetSpot(10, 10));
      Assert.AreEqual(Player.White, board.GetSpot(11, 11));
      board = new Board(Player.Black, 0, 0,
        //123456789012345678
"..................." +  // 0
"..................." +  // 1
"..................." +  // 2
"..................." +  // 3
"..................." +  // 4
"..................." +  // 5
"..................." +  // 6
".......W.W.W......." +  // 7
"........WWW........" +  // 8
".......WW.WW......." +  // 9 (center)
"........WWW........" +  // 10
".......W.W.W......." +  // 11
"............B......" +  // 12
"..................." +  // 13
"..................." +  // 14
"..................." +  // 15
"..................." +  // 16
"..................." +  // 17
"...................");  // 18
      Assert.AreEqual(Player.White, board.GetSpot(10, 10));
      Assert.AreEqual(Player.White, board.GetSpot(11, 11));
      board.Move(9, 9);
      Assert.AreEqual(Player.White, board.GetSpot(7, 7));
      Assert.AreEqual(Player.White, board.GetSpot(8, 8));

      Assert.AreEqual(Player.White, board.GetSpot(7, 9));
      Assert.AreEqual(Player.White, board.GetSpot(8, 9));

      Assert.AreEqual(Player.White, board.GetSpot(7, 11));
      Assert.AreEqual(Player.White, board.GetSpot(8, 10));

      Assert.AreEqual(Player.White, board.GetSpot(9, 7));
      Assert.AreEqual(Player.White, board.GetSpot(9, 8));

      Assert.AreEqual(Player.White, board.GetSpot(9, 10));
      Assert.AreEqual(Player.White, board.GetSpot(9, 11));

      Assert.AreEqual(Player.White, board.GetSpot(10, 8));
      Assert.AreEqual(Player.White, board.GetSpot(11, 7));

      Assert.AreEqual(Player.White, board.GetSpot(10, 9));
      Assert.AreEqual(Player.White, board.GetSpot(11, 9));

      Assert.AreEqual(Player.Neither, board.GetSpot(10, 10));
      Assert.AreEqual(Player.Neither, board.GetSpot(11, 11));
    }
  }
}
