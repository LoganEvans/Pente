using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CommonInterfaces;
using LoganPenteAI;

namespace LoganUnitTests {
  [TestClass]
  public class GameStateTests {
    [TestMethod]
    public void TestGetWindows() {
      Assert.AreEqual(Pattern.ROW_PATTERN, 0);
      Assert.AreEqual(Pattern.COL_PATTERN, 1);
      Assert.AreEqual(Pattern.UP_DIAG_PATTERN, 2);
      Assert.AreEqual(Pattern.DOWN_DIAG_PATTERN, 3);

      GameState uut = new GameState(Player.White, 2, 4,
//123456789012345678
"..................." +  // 0
"..................." +  // 1
"..................." +  // 2
"..................." +  // 3
"..................." +  // 4
".........W...W....." +  // 5
".........W..B......" +  // 6
".......W.B........." +  // 7
".........BW........" +  // 8
".....BBBB.BBBB....." +  // 9 (center)
"........WBB........" +  // 10
".........B.B......." +  // 11
"......W..W........." +  // 12
".....W...W........." +  // 13
"..................." +  // 14
"..................." +  // 15
"..................." +  // 16
"..................." +  // 17
"...................");  // 18

      int row = 9;
      int col = 9;
      List<Tuple<int, int>> windows = uut.GetWindows(row, col);
      List<Tuple<int, int>> windowsB = uut.GetWindows(row + 1, col + 1);
      Assert.AreNotEqual(windows[0].Item1, windowsB[0].Item1);

      int expectedWhiteRow = Convert.ToInt32("000000000", 2);
      int expectedBlackRow = Convert.ToInt32("111101111", 2);
      Assert.AreEqual(expectedWhiteRow, windows[0].Item1);
      Assert.AreEqual(expectedBlackRow, windows[0].Item2);

      int expectedWhiteCol = Convert.ToInt32("110000011", 2);
      int expectedBlackCol = Convert.ToInt32("001101100", 2);
      Assert.AreEqual(expectedWhiteCol, windows[1].Item1);
      Assert.AreEqual(expectedBlackCol, windows[1].Item2);

      int expectedWhiteUpDiag = Convert.ToInt32("100101011", 2);
      int expectedBlackUpDiag = Convert.ToInt32("010000000", 2);
      Assert.AreEqual(expectedWhiteUpDiag, windows[2].Item1);
      Assert.AreEqual(expectedBlackUpDiag, windows[2].Item2);

      int expectedWhiteDownDiag = Convert.ToInt32("000000100", 2);
      int expectedBlackDownDiag = Convert.ToInt32("001100000", 2);
      Assert.AreEqual(expectedWhiteDownDiag, windows[3].Item1);
      Assert.AreEqual(expectedBlackDownDiag, windows[3].Item2);
    }

    [TestMethod]
    public void TestGetHeuristicValueWhiteTurn() {
      GameState uut = new GameState(Player.White, 2, 4,
//123456789012345678
".......B..........." +  // 0
"........B.........." +  // 1
"..................." +  // 2
"..........B..W....." +  // 3
"...........BW......" +  // 4
"...........W......." +  // 5
"..........W........" +  // 6
"..................." +  // 7
"........W.........." +  // 8
"..........WWWWB...." +  // 9 (center)
"..........BBBBW...." +  // 10
".......WB.WWWWB...." +  // 11
"....WBBBB.........." +  // 12
"..................." +  // 13
"..................." +  // 14
"..................." +  // 15
"..................." +  // 16
"..................." +  // 17
"...................");  // 18

      // Can win: priority == 0
      // Must block win: priority == 1
      //

      List<Tuple<int, int>> windows;
      Heuristic heuristic;
      windows = uut.GetWindows(2, 9);
      int expectedBlackDownDiag = Convert.ToInt32("001101100", 2);
      Assert.AreEqual(expectedBlackDownDiag, windows[3].Item2);
      heuristic = uut.GetHeuristicValue(2, 9);
      Assert.AreEqual(1, heuristic.GetPriority());

      windows = uut.GetWindows(7, 9);
      int expectedWhiteUpDiag = Convert.ToInt32("111101000", 2);
      Assert.AreEqual(expectedWhiteUpDiag, windows[2].Item1);
      Assert.AreEqual(0, uut.GetHeuristicValue(7, 9).GetPriority());
      Assert.AreEqual(0, uut.GetHeuristicValue(9, 9).GetPriority());
      Assert.AreEqual(1, uut.GetHeuristicValue(10, 9).GetPriority());
      Assert.AreEqual(0, uut.GetHeuristicValue(11, 9).GetPriority());
      Assert.AreEqual(1, uut.GetHeuristicValue(12, 9).GetPriority());
    }

    [TestMethod]
    public void TestGetHeuristicValueBlackTurn() {
      GameState uut = new GameState(Player.Black, 2, 4,
//123456789012345678
".......B..........." +  // 0
"........B.........." +  // 1
"..................." +  // 2
"..........B..W....." +  // 3
"...........BW......" +  // 4
"...........W......." +  // 5
"..........W........" +  // 6
"..................." +  // 7
"........W.........." +  // 8
"..........WWWWB...." +  // 9 (center)
"..........BBBBW...." +  // 10
".......WB.WWWWB...." +  // 11
"....WBBBB.........." +  // 12
"..................." +  // 13
"..................." +  // 14
"..................." +  // 15
"..................." +  // 16
"..................." +  // 17
"...................");  // 18

      // Can win: priority == 0
      // Must block win: priority == 1
      //

      List<Tuple<int, int>> windows;
      Heuristic heuristic;
      windows = uut.GetWindows(2, 9);
      int expectedBlackDownDiag = Convert.ToInt32("001101100", 2);
      Assert.AreEqual(windows[3].Item2, expectedBlackDownDiag);
      heuristic = uut.GetHeuristicValue(2, 9);
      Assert.AreEqual(0, heuristic.GetPriority());

      windows = uut.GetWindows(7, 9);
      int expectedWhiteUpDiag = Convert.ToInt32("111101000", 2);
      Assert.AreEqual(windows[2].Item1, expectedWhiteUpDiag);
      Assert.AreEqual(1, uut.GetHeuristicValue(7, 9).GetPriority());
      Assert.AreEqual(1, uut.GetHeuristicValue(9, 9).GetPriority());
      Assert.AreEqual(0, uut.GetHeuristicValue(10, 9).GetPriority());
      Assert.AreEqual(1, uut.GetHeuristicValue(11, 9).GetPriority());
      Assert.AreEqual(0, uut.GetHeuristicValue(12, 9).GetPriority());
    }
  }
}
