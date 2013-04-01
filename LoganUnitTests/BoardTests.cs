using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CommonInterfaces;
using LoganPenteAI;

namespace LoganUnitTests {
  [TestClass]
  public class BoardTests {
    [TestMethod]
    public void test_debugConsstructor() {
      var board = new Board(player_t.white, 2, 4,
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

      Assert.AreEqual(player_t.white, board.getSpot(9, 9));
      Assert.AreEqual(player_t.white, board.getSpot(12, 10));
      Assert.AreEqual(player_t.white, board.getSpot(0, 0));
      Assert.AreEqual(player_t.black, board.getSpot(9, 10));
      Assert.AreEqual(player_t.black, board.getSpot(9, 11));
      Assert.AreEqual(player_t.black, board.getSpot(14, 12));
      Assert.AreEqual(player_t.black, board.getSpot(0, 18));

      Assert.AreEqual(2, board.getCaptures(player_t.white));
      Assert.AreEqual(4, board.getCaptures(player_t.black));
    }

    [TestMethod]
    public void test_captures() {
      var board = new Board(player_t.white, 0, 0,
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

      Assert.AreEqual(0, board.getCaptures(player_t.white));
      Assert.AreEqual(player_t.black, board.getSpot(7, 7));
      Assert.AreEqual(player_t.black, board.getSpot(8, 8));

      Assert.AreEqual(player_t.black, board.getSpot(7, 9));
      Assert.AreEqual(player_t.black, board.getSpot(8, 9));

      Assert.AreEqual(player_t.black, board.getSpot(7, 11));
      Assert.AreEqual(player_t.black, board.getSpot(8, 10));

      Assert.AreEqual(player_t.black, board.getSpot(9, 7));
      Assert.AreEqual(player_t.black, board.getSpot(9, 8));

      Assert.AreEqual(player_t.black, board.getSpot(9, 10));
      Assert.AreEqual(player_t.black, board.getSpot(9, 11));

      Assert.AreEqual(player_t.black, board.getSpot(10, 8));
      Assert.AreEqual(player_t.black, board.getSpot(11, 7));

      Assert.AreEqual(player_t.black, board.getSpot(10, 9));
      Assert.AreEqual(player_t.black, board.getSpot(11, 9));

      Assert.AreEqual(player_t.black, board.getSpot(10, 10));
      Assert.AreEqual(player_t.black, board.getSpot(11, 11));

      board.move(9, 9);
      Assert.AreEqual(8, board.getCaptures(player_t.white));
      Assert.AreEqual(player_t.neither, board.getSpot(7, 7));
      Assert.AreEqual(player_t.neither, board.getSpot(8, 8));

      Assert.AreEqual(player_t.neither, board.getSpot(7, 9));
      Assert.AreEqual(player_t.neither, board.getSpot(8, 9));

      Assert.AreEqual(player_t.neither, board.getSpot(7, 11));
      Assert.AreEqual(player_t.neither, board.getSpot(8, 10));

      Assert.AreEqual(player_t.neither, board.getSpot(9, 7));
      Assert.AreEqual(player_t.neither, board.getSpot(9, 8));

      Assert.AreEqual(player_t.neither, board.getSpot(9, 10));
      Assert.AreEqual(player_t.neither, board.getSpot(9, 11));

      Assert.AreEqual(player_t.neither, board.getSpot(10, 8));
      Assert.AreEqual(player_t.neither, board.getSpot(11, 7));

      Assert.AreEqual(player_t.neither, board.getSpot(10, 9));
      Assert.AreEqual(player_t.neither, board.getSpot(11, 9));

      Assert.AreEqual(player_t.neither, board.getSpot(10, 10));
      Assert.AreEqual(player_t.neither, board.getSpot(11, 11));
    }

    [TestMethod]
    public void test_capturesIndividualWhiteMove() {
      Board board;
      board = new Board(player_t.white, 0, 0,
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
      Assert.AreEqual(player_t.black, board.getSpot(7, 7));
      Assert.AreEqual(player_t.black, board.getSpot(8, 8));
      board.move(9, 9);
      Assert.AreEqual(player_t.neither, board.getSpot(7, 7));
      Assert.AreEqual(player_t.neither, board.getSpot(8, 8));

      Assert.AreEqual(player_t.black, board.getSpot(7, 9));
      Assert.AreEqual(player_t.black, board.getSpot(8, 9));

      Assert.AreEqual(player_t.black, board.getSpot(7, 11));
      Assert.AreEqual(player_t.black, board.getSpot(8, 10));

      Assert.AreEqual(player_t.black, board.getSpot(9, 7));
      Assert.AreEqual(player_t.black, board.getSpot(9, 8));

      Assert.AreEqual(player_t.black, board.getSpot(9, 10));
      Assert.AreEqual(player_t.black, board.getSpot(9, 11));

      Assert.AreEqual(player_t.black, board.getSpot(10, 8));
      Assert.AreEqual(player_t.black, board.getSpot(11, 7));

      Assert.AreEqual(player_t.black, board.getSpot(10, 9));
      Assert.AreEqual(player_t.black, board.getSpot(11, 9));

      Assert.AreEqual(player_t.black, board.getSpot(10, 10));
      Assert.AreEqual(player_t.black, board.getSpot(11, 11));


      board = new Board(player_t.white, 0, 0,
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
      Assert.AreEqual(player_t.black, board.getSpot(7, 9));
      Assert.AreEqual(player_t.black, board.getSpot(8, 9));
      board.move(9, 9);
      Assert.AreEqual(player_t.black, board.getSpot(7, 7));
      Assert.AreEqual(player_t.black, board.getSpot(8, 8));

      Assert.AreEqual(player_t.neither, board.getSpot(7, 9));
      Assert.AreEqual(player_t.neither, board.getSpot(8, 9));

      Assert.AreEqual(player_t.black, board.getSpot(7, 11));
      Assert.AreEqual(player_t.black, board.getSpot(8, 10));

      Assert.AreEqual(player_t.black, board.getSpot(9, 7));
      Assert.AreEqual(player_t.black, board.getSpot(9, 8));

      Assert.AreEqual(player_t.black, board.getSpot(9, 10));
      Assert.AreEqual(player_t.black, board.getSpot(9, 11));

      Assert.AreEqual(player_t.black, board.getSpot(10, 8));
      Assert.AreEqual(player_t.black, board.getSpot(11, 7));

      Assert.AreEqual(player_t.black, board.getSpot(10, 9));
      Assert.AreEqual(player_t.black, board.getSpot(11, 9));

      Assert.AreEqual(player_t.black, board.getSpot(10, 10));
      Assert.AreEqual(player_t.black, board.getSpot(11, 11));

      board = new Board(player_t.white, 0, 0,
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
      Assert.AreEqual(player_t.black, board.getSpot(7, 11));
      Assert.AreEqual(player_t.black, board.getSpot(8, 10));
      board.move(9, 9);
      Assert.AreEqual(player_t.black, board.getSpot(7, 7));
      Assert.AreEqual(player_t.black, board.getSpot(8, 8));

      Assert.AreEqual(player_t.black, board.getSpot(7, 9));
      Assert.AreEqual(player_t.black, board.getSpot(8, 9));

      Assert.AreEqual(player_t.neither, board.getSpot(7, 11));
      Assert.AreEqual(player_t.neither, board.getSpot(8, 10));

      Assert.AreEqual(player_t.black, board.getSpot(9, 7));
      Assert.AreEqual(player_t.black, board.getSpot(9, 8));

      Assert.AreEqual(player_t.black, board.getSpot(9, 10));
      Assert.AreEqual(player_t.black, board.getSpot(9, 11));

      Assert.AreEqual(player_t.black, board.getSpot(10, 8));
      Assert.AreEqual(player_t.black, board.getSpot(11, 7));

      Assert.AreEqual(player_t.black, board.getSpot(10, 9));
      Assert.AreEqual(player_t.black, board.getSpot(11, 9));

      Assert.AreEqual(player_t.black, board.getSpot(10, 10));
      Assert.AreEqual(player_t.black, board.getSpot(11, 11));
      board = new Board(player_t.white, 0, 0,
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
      Assert.AreEqual(player_t.black, board.getSpot(9, 7));
      Assert.AreEqual(player_t.black, board.getSpot(9, 8));
      board.move(9, 9);
      Assert.AreEqual(player_t.black, board.getSpot(7, 7));
      Assert.AreEqual(player_t.black, board.getSpot(8, 8));

      Assert.AreEqual(player_t.black, board.getSpot(7, 9));
      Assert.AreEqual(player_t.black, board.getSpot(8, 9));

      Assert.AreEqual(player_t.black, board.getSpot(7, 11));
      Assert.AreEqual(player_t.black, board.getSpot(8, 10));

      Assert.AreEqual(player_t.neither, board.getSpot(9, 7));
      Assert.AreEqual(player_t.neither, board.getSpot(9, 8));

      Assert.AreEqual(player_t.black, board.getSpot(9, 10));
      Assert.AreEqual(player_t.black, board.getSpot(9, 11));

      Assert.AreEqual(player_t.black, board.getSpot(10, 8));
      Assert.AreEqual(player_t.black, board.getSpot(11, 7));

      Assert.AreEqual(player_t.black, board.getSpot(10, 9));
      Assert.AreEqual(player_t.black, board.getSpot(11, 9));

      Assert.AreEqual(player_t.black, board.getSpot(10, 10));
      Assert.AreEqual(player_t.black, board.getSpot(11, 11));
      board = new Board(player_t.white, 0, 0,

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
      Assert.AreEqual(player_t.black, board.getSpot(9, 10));
      Assert.AreEqual(player_t.black, board.getSpot(9, 11));
      board.move(9, 9);
      Assert.AreEqual(player_t.black, board.getSpot(7, 7));
      Assert.AreEqual(player_t.black, board.getSpot(8, 8));

      Assert.AreEqual(player_t.black, board.getSpot(7, 9));
      Assert.AreEqual(player_t.black, board.getSpot(8, 9));

      Assert.AreEqual(player_t.black, board.getSpot(7, 11));
      Assert.AreEqual(player_t.black, board.getSpot(8, 10));

      Assert.AreEqual(player_t.black, board.getSpot(9, 7));
      Assert.AreEqual(player_t.black, board.getSpot(9, 8));

      Assert.AreEqual(player_t.neither, board.getSpot(9, 10));
      Assert.AreEqual(player_t.neither, board.getSpot(9, 11));

      Assert.AreEqual(player_t.black, board.getSpot(10, 8));
      Assert.AreEqual(player_t.black, board.getSpot(11, 7));

      Assert.AreEqual(player_t.black, board.getSpot(10, 9));
      Assert.AreEqual(player_t.black, board.getSpot(11, 9));

      Assert.AreEqual(player_t.black, board.getSpot(10, 10));
      Assert.AreEqual(player_t.black, board.getSpot(11, 11));
      board = new Board(player_t.white, 0, 0,
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
      Assert.AreEqual(player_t.black, board.getSpot(10, 8));
      Assert.AreEqual(player_t.black, board.getSpot(11, 7));
      board.move(9, 9);
      Assert.AreEqual(player_t.black, board.getSpot(7, 7));
      Assert.AreEqual(player_t.black, board.getSpot(8, 8));

      Assert.AreEqual(player_t.black, board.getSpot(7, 9));
      Assert.AreEqual(player_t.black, board.getSpot(8, 9));

      Assert.AreEqual(player_t.black, board.getSpot(7, 11));
      Assert.AreEqual(player_t.black, board.getSpot(8, 10));

      Assert.AreEqual(player_t.black, board.getSpot(9, 7));
      Assert.AreEqual(player_t.black, board.getSpot(9, 8));

      Assert.AreEqual(player_t.black, board.getSpot(9, 10));
      Assert.AreEqual(player_t.black, board.getSpot(9, 11));

      Assert.AreEqual(player_t.neither, board.getSpot(10, 8));
      Assert.AreEqual(player_t.neither, board.getSpot(11, 7));

      Assert.AreEqual(player_t.black, board.getSpot(10, 9));
      Assert.AreEqual(player_t.black, board.getSpot(11, 9));

      Assert.AreEqual(player_t.black, board.getSpot(10, 10));
      Assert.AreEqual(player_t.black, board.getSpot(11, 11));
      board = new Board(player_t.white, 0, 0,
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
      Assert.AreEqual(player_t.black, board.getSpot(10, 9));
      Assert.AreEqual(player_t.black, board.getSpot(11, 9));
      board.move(9, 9);
      Assert.AreEqual(player_t.black, board.getSpot(7, 7));
      Assert.AreEqual(player_t.black, board.getSpot(8, 8));

      Assert.AreEqual(player_t.black, board.getSpot(7, 9));
      Assert.AreEqual(player_t.black, board.getSpot(8, 9));

      Assert.AreEqual(player_t.black, board.getSpot(7, 11));
      Assert.AreEqual(player_t.black, board.getSpot(8, 10));

      Assert.AreEqual(player_t.black, board.getSpot(9, 7));
      Assert.AreEqual(player_t.black, board.getSpot(9, 8));

      Assert.AreEqual(player_t.black, board.getSpot(9, 10));
      Assert.AreEqual(player_t.black, board.getSpot(9, 11));

      Assert.AreEqual(player_t.black, board.getSpot(10, 8));
      Assert.AreEqual(player_t.black, board.getSpot(11, 7));

      Assert.AreEqual(player_t.neither, board.getSpot(10, 9));
      Assert.AreEqual(player_t.neither, board.getSpot(11, 9));

      Assert.AreEqual(player_t.black, board.getSpot(10, 10));
      Assert.AreEqual(player_t.black, board.getSpot(11, 11));
      board = new Board(player_t.white, 0, 0,
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
      Assert.AreEqual(player_t.black, board.getSpot(10, 10));
      Assert.AreEqual(player_t.black, board.getSpot(11, 11));
      board.move(9, 9);
      Assert.AreEqual(player_t.black, board.getSpot(7, 7));
      Assert.AreEqual(player_t.black, board.getSpot(8, 8));

      Assert.AreEqual(player_t.black, board.getSpot(7, 9));
      Assert.AreEqual(player_t.black, board.getSpot(8, 9));

      Assert.AreEqual(player_t.black, board.getSpot(7, 11));
      Assert.AreEqual(player_t.black, board.getSpot(8, 10));

      Assert.AreEqual(player_t.black, board.getSpot(9, 7));
      Assert.AreEqual(player_t.black, board.getSpot(9, 8));

      Assert.AreEqual(player_t.black, board.getSpot(9, 10));
      Assert.AreEqual(player_t.black, board.getSpot(9, 11));

      Assert.AreEqual(player_t.black, board.getSpot(10, 8));
      Assert.AreEqual(player_t.black, board.getSpot(11, 7));

      Assert.AreEqual(player_t.black, board.getSpot(10, 9));
      Assert.AreEqual(player_t.black, board.getSpot(11, 9));

      Assert.AreEqual(player_t.neither, board.getSpot(10, 10));
      Assert.AreEqual(player_t.neither, board.getSpot(11, 11));
    }

    [TestMethod]
    public void test_capturesIndividualBlackMove() {
      Board board;
      board = new Board(player_t.black, 0, 0,
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
      Assert.AreEqual(player_t.white, board.getSpot(7, 7));
      Assert.AreEqual(player_t.white, board.getSpot(8, 8));
      board.move(9, 9);
      Assert.AreEqual(player_t.neither, board.getSpot(7, 7));
      Assert.AreEqual(player_t.neither, board.getSpot(8, 8));

      Assert.AreEqual(player_t.white, board.getSpot(7, 9));
      Assert.AreEqual(player_t.white, board.getSpot(8, 9));

      Assert.AreEqual(player_t.white, board.getSpot(7, 11));
      Assert.AreEqual(player_t.white, board.getSpot(8, 10));

      Assert.AreEqual(player_t.white, board.getSpot(9, 7));
      Assert.AreEqual(player_t.white, board.getSpot(9, 8));

      Assert.AreEqual(player_t.white, board.getSpot(9, 10));
      Assert.AreEqual(player_t.white, board.getSpot(9, 11));

      Assert.AreEqual(player_t.white, board.getSpot(10, 8));
      Assert.AreEqual(player_t.white, board.getSpot(11, 7));

      Assert.AreEqual(player_t.white, board.getSpot(10, 9));
      Assert.AreEqual(player_t.white, board.getSpot(11, 9));

      Assert.AreEqual(player_t.white, board.getSpot(10, 10));
      Assert.AreEqual(player_t.white, board.getSpot(11, 11));


      board = new Board(player_t.black, 0, 0,
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
      Assert.AreEqual(player_t.white, board.getSpot(7, 9));
      Assert.AreEqual(player_t.white, board.getSpot(8, 9));
      board.move(9, 9);
      Assert.AreEqual(player_t.white, board.getSpot(7, 7));
      Assert.AreEqual(player_t.white, board.getSpot(8, 8));

      Assert.AreEqual(player_t.neither, board.getSpot(7, 9));
      Assert.AreEqual(player_t.neither, board.getSpot(8, 9));

      Assert.AreEqual(player_t.white, board.getSpot(7, 11));
      Assert.AreEqual(player_t.white, board.getSpot(8, 10));

      Assert.AreEqual(player_t.white, board.getSpot(9, 7));
      Assert.AreEqual(player_t.white, board.getSpot(9, 8));

      Assert.AreEqual(player_t.white, board.getSpot(9, 10));
      Assert.AreEqual(player_t.white, board.getSpot(9, 11));

      Assert.AreEqual(player_t.white, board.getSpot(10, 8));
      Assert.AreEqual(player_t.white, board.getSpot(11, 7));

      Assert.AreEqual(player_t.white, board.getSpot(10, 9));
      Assert.AreEqual(player_t.white, board.getSpot(11, 9));

      Assert.AreEqual(player_t.white, board.getSpot(10, 10));
      Assert.AreEqual(player_t.white, board.getSpot(11, 11));

      board = new Board(player_t.black, 0, 0,
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
      Assert.AreEqual(player_t.white, board.getSpot(7, 11));
      Assert.AreEqual(player_t.white, board.getSpot(8, 10));
      board.move(9, 9);
      Assert.AreEqual(player_t.white, board.getSpot(7, 7));
      Assert.AreEqual(player_t.white, board.getSpot(8, 8));

      Assert.AreEqual(player_t.white, board.getSpot(7, 9));
      Assert.AreEqual(player_t.white, board.getSpot(8, 9));

      Assert.AreEqual(player_t.neither, board.getSpot(7, 11));
      Assert.AreEqual(player_t.neither, board.getSpot(8, 10));

      Assert.AreEqual(player_t.white, board.getSpot(9, 7));
      Assert.AreEqual(player_t.white, board.getSpot(9, 8));

      Assert.AreEqual(player_t.white, board.getSpot(9, 10));
      Assert.AreEqual(player_t.white, board.getSpot(9, 11));

      Assert.AreEqual(player_t.white, board.getSpot(10, 8));
      Assert.AreEqual(player_t.white, board.getSpot(11, 7));

      Assert.AreEqual(player_t.white, board.getSpot(10, 9));
      Assert.AreEqual(player_t.white, board.getSpot(11, 9));

      Assert.AreEqual(player_t.white, board.getSpot(10, 10));
      Assert.AreEqual(player_t.white, board.getSpot(11, 11));
      board = new Board(player_t.black, 0, 0,
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
      Assert.AreEqual(player_t.white, board.getSpot(9, 7));
      Assert.AreEqual(player_t.white, board.getSpot(9, 8));
      board.move(9, 9);
      Assert.AreEqual(player_t.white, board.getSpot(7, 7));
      Assert.AreEqual(player_t.white, board.getSpot(8, 8));

      Assert.AreEqual(player_t.white, board.getSpot(7, 9));
      Assert.AreEqual(player_t.white, board.getSpot(8, 9));

      Assert.AreEqual(player_t.white, board.getSpot(7, 11));
      Assert.AreEqual(player_t.white, board.getSpot(8, 10));

      Assert.AreEqual(player_t.neither, board.getSpot(9, 7));
      Assert.AreEqual(player_t.neither, board.getSpot(9, 8));

      Assert.AreEqual(player_t.white, board.getSpot(9, 10));
      Assert.AreEqual(player_t.white, board.getSpot(9, 11));

      Assert.AreEqual(player_t.white, board.getSpot(10, 8));
      Assert.AreEqual(player_t.white, board.getSpot(11, 7));

      Assert.AreEqual(player_t.white, board.getSpot(10, 9));
      Assert.AreEqual(player_t.white, board.getSpot(11, 9));

      Assert.AreEqual(player_t.white, board.getSpot(10, 10));
      Assert.AreEqual(player_t.white, board.getSpot(11, 11));
      board = new Board(player_t.black, 0, 0,

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
      Assert.AreEqual(player_t.white, board.getSpot(9, 10));
      Assert.AreEqual(player_t.white, board.getSpot(9, 11));
      board.move(9, 9);
      Assert.AreEqual(player_t.white, board.getSpot(7, 7));
      Assert.AreEqual(player_t.white, board.getSpot(8, 8));

      Assert.AreEqual(player_t.white, board.getSpot(7, 9));
      Assert.AreEqual(player_t.white, board.getSpot(8, 9));

      Assert.AreEqual(player_t.white, board.getSpot(7, 11));
      Assert.AreEqual(player_t.white, board.getSpot(8, 10));

      Assert.AreEqual(player_t.white, board.getSpot(9, 7));
      Assert.AreEqual(player_t.white, board.getSpot(9, 8));

      Assert.AreEqual(player_t.neither, board.getSpot(9, 10));
      Assert.AreEqual(player_t.neither, board.getSpot(9, 11));

      Assert.AreEqual(player_t.white, board.getSpot(10, 8));
      Assert.AreEqual(player_t.white, board.getSpot(11, 7));

      Assert.AreEqual(player_t.white, board.getSpot(10, 9));
      Assert.AreEqual(player_t.white, board.getSpot(11, 9));

      Assert.AreEqual(player_t.white, board.getSpot(10, 10));
      Assert.AreEqual(player_t.white, board.getSpot(11, 11));
      board = new Board(player_t.black, 0, 0,
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
      Assert.AreEqual(player_t.white, board.getSpot(10, 8));
      Assert.AreEqual(player_t.white, board.getSpot(11, 7));
      board.move(9, 9);
      Assert.AreEqual(player_t.white, board.getSpot(7, 7));
      Assert.AreEqual(player_t.white, board.getSpot(8, 8));

      Assert.AreEqual(player_t.white, board.getSpot(7, 9));
      Assert.AreEqual(player_t.white, board.getSpot(8, 9));

      Assert.AreEqual(player_t.white, board.getSpot(7, 11));
      Assert.AreEqual(player_t.white, board.getSpot(8, 10));

      Assert.AreEqual(player_t.white, board.getSpot(9, 7));
      Assert.AreEqual(player_t.white, board.getSpot(9, 8));

      Assert.AreEqual(player_t.white, board.getSpot(9, 10));
      Assert.AreEqual(player_t.white, board.getSpot(9, 11));

      Assert.AreEqual(player_t.neither, board.getSpot(10, 8));
      Assert.AreEqual(player_t.neither, board.getSpot(11, 7));

      Assert.AreEqual(player_t.white, board.getSpot(10, 9));
      Assert.AreEqual(player_t.white, board.getSpot(11, 9));

      Assert.AreEqual(player_t.white, board.getSpot(10, 10));
      Assert.AreEqual(player_t.white, board.getSpot(11, 11));
      board = new Board(player_t.black, 0, 0,
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
      Assert.AreEqual(player_t.white, board.getSpot(10, 9));
      Assert.AreEqual(player_t.white, board.getSpot(11, 9));
      board.move(9, 9);
      Assert.AreEqual(player_t.white, board.getSpot(7, 7));
      Assert.AreEqual(player_t.white, board.getSpot(8, 8));

      Assert.AreEqual(player_t.white, board.getSpot(7, 9));
      Assert.AreEqual(player_t.white, board.getSpot(8, 9));

      Assert.AreEqual(player_t.white, board.getSpot(7, 11));
      Assert.AreEqual(player_t.white, board.getSpot(8, 10));

      Assert.AreEqual(player_t.white, board.getSpot(9, 7));
      Assert.AreEqual(player_t.white, board.getSpot(9, 8));

      Assert.AreEqual(player_t.white, board.getSpot(9, 10));
      Assert.AreEqual(player_t.white, board.getSpot(9, 11));

      Assert.AreEqual(player_t.white, board.getSpot(10, 8));
      Assert.AreEqual(player_t.white, board.getSpot(11, 7));

      Assert.AreEqual(player_t.neither, board.getSpot(10, 9));
      Assert.AreEqual(player_t.neither, board.getSpot(11, 9));

      Assert.AreEqual(player_t.white, board.getSpot(10, 10));
      Assert.AreEqual(player_t.white, board.getSpot(11, 11));
      board = new Board(player_t.black, 0, 0,
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
      Assert.AreEqual(player_t.white, board.getSpot(10, 10));
      Assert.AreEqual(player_t.white, board.getSpot(11, 11));
      board.move(9, 9);
      Assert.AreEqual(player_t.white, board.getSpot(7, 7));
      Assert.AreEqual(player_t.white, board.getSpot(8, 8));

      Assert.AreEqual(player_t.white, board.getSpot(7, 9));
      Assert.AreEqual(player_t.white, board.getSpot(8, 9));

      Assert.AreEqual(player_t.white, board.getSpot(7, 11));
      Assert.AreEqual(player_t.white, board.getSpot(8, 10));

      Assert.AreEqual(player_t.white, board.getSpot(9, 7));
      Assert.AreEqual(player_t.white, board.getSpot(9, 8));

      Assert.AreEqual(player_t.white, board.getSpot(9, 10));
      Assert.AreEqual(player_t.white, board.getSpot(9, 11));

      Assert.AreEqual(player_t.white, board.getSpot(10, 8));
      Assert.AreEqual(player_t.white, board.getSpot(11, 7));

      Assert.AreEqual(player_t.white, board.getSpot(10, 9));
      Assert.AreEqual(player_t.white, board.getSpot(11, 9));

      Assert.AreEqual(player_t.neither, board.getSpot(10, 10));
      Assert.AreEqual(player_t.neither, board.getSpot(11, 11));
    }
  }
}
