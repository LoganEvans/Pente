using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using PenteInterfaces;
using PenteAI;

namespace UnitTests {
  [TestClass]
  public class BoardTests {
    [TestMethod]
    public void TestDebugConstructor() {
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

//  [TestMethod]
//  public void TestSnapshots() {
//    var board = new Board(Player.White, 2, 4,
//      //123456789012345678
//      "W.................B" +  // 0
//      "..................." +  // 1
//      "..................." +  // 2
//      "..................." +  // 3
//      "..................." +  // 4
//      "..................." +  // 5
//      "..................." +  // 6
//      "..................." +  // 7
//      "..................." +  // 8
//      ".........WBB......." +  // 9 (center)
//      "..................." +  // 10
//      "..................." +  // 11
//      "..........W........" +  // 12
//      "..................." +  // 13
//      "............B......" +  // 14
//      "..................." +  // 15
//      "..................." +  // 16
//      "..................." +  // 17
//      "...................");  // 18
//    var copiedBoard = new Board(board);
//    Tuple<int, int, int, int> snapshotData = board.GetSnapshotData();

//    board.Move(9, 12);  // capture
//    board.Move(2, 18);
//    board.Move(9, 10);
//    board.Rollback(snapshotData);
//    Tuple<int, int, int, int> newSnapshotData = board.GetSnapshotData();
//    Assert.AreEqual(snapshotData, newSnapshotData);
//    Assert.IsTrue(board == copiedBoard);

//    board = new Board();
//    snapshotData = board.GetSnapshotData();
//    board.Move(9, 9);
//    copiedBoard = new Board(board);

//    board.Move(10, 10);
//    board.Move(9, 13);
//    board.Move(11, 11);
//    board.Move(9, 10);
//    board.Move(12, 12);
//    board.Move(9, 11);
//    board.Move(13, 13);
//    board.Move(9, 12);  // win
//    Assert.IsTrue(board.GetWinner() == Player.White);
//    board.Rollback(snapshotData);
//    board.Move(9, 9);
//    Assert.IsTrue(board == copiedBoard);
//  }

    // After some testing, the mean for the number of moves per game is quite
    // close to 154, and the variance is quite close to 
    [TestMethod]
    public void TestAvgForRandomGame() {
      var rand = new Random();
      int trials = 10000;

      int count = 0;
      for (int i = 0; i < trials; i++) {
        Board board = new Board();
        board.Move(9, 9);

        while (board.GetWinner() == Player.Neither) {
          board.Move(rand.Next(Board.ROWS), rand.Next(Board.COLS));
        }
        count += board.GetPlyNumber();
      }
      double average = count / (float)trials;
      Assert.IsTrue(152 < average);
      Assert.IsTrue(average < 156);
    }

    [TestMethod]
    public void TestWhiteSecondMoveIllegal() {
      var board = new Board(Player.White, 0, 0,
        //123456789012345678
"..................." +  // 0
"..................." +  // 1
"..................." +  // 2
"..................." +  // 3
"..................." +  // 4
"..................." +  // 5
"..................." +  // 6
"..................." +  // 7
"..................." +  // 8
".........WB........" +  // 9 (center)
"..................." +  // 10
"..................." +  // 11
"..................." +  // 12
"..................." +  // 13
"..................." +  // 14
"..................." +  // 15
"..................." +  // 16
"..................." +  // 17
"...................");  // 18
      for (int row_dex = 7; row_dex <= 11; row_dex++) {
        for (int col_dex = 7; col_dex <= 11; col_dex++) {
          Assert.IsFalse(board.Move(row_dex, col_dex));
        }
      }
    }

    [TestMethod]
    public void TestWhiteSecondMoveLegal() {
      for (int row_dex = 0; row_dex < Board.ROWS; row_dex++) {
        for (int col_dex = 0; col_dex < Board.COLS; col_dex++) {
          if (7 <= row_dex && row_dex <= 11 &&
              7 <= col_dex && col_dex <= 11) {
            continue;
          }
          var board = new Board(Player.White, 0, 0,
            //123456789012345678
    "..................." +  // 0
    "..................." +  // 1
    "..................." +  // 2
    "..................." +  // 3
    "..................." +  // 4
    "..................." +  // 5
    "..................." +  // 6
    "..................." +  // 7
    "..................." +  // 8
    ".........WB........" +  // 9 (center)
    "..................." +  // 10
    "..................." +  // 11
    "..................." +  // 12
    "..................." +  // 13
    "..................." +  // 14
    "..................." +  // 15
    "..................." +  // 16
    "..................." +  // 17
    "...................");  // 18
          Assert.IsTrue(board.Move(row_dex, col_dex));
        }
      }
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
