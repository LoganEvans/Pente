using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CommonInterfaces;
using LoganPenteAI;

namespace LoganUnitTests {
  [TestClass]
  public class AITests {
    [TestMethod]
    public void test_getMove_shouldTakeWin() {
      Assert.Fail();
      Board testBoard = new Board();
      // Default of (9, 9) for white
      testBoard.move(9, 10);  // black
      testBoard.move(13, 9);  // white
      testBoard.move(9, 11);  // black
      testBoard.move(11, 9);  // white
      testBoard.move(9, 12);  // black
      testBoard.move(10, 9);  // white
      testBoard.move(9, 13);  // black

      Assert.IsTrue(testBoard.getWinner() == player_t.neither);
      Tuple<int, int> move = new PlayerAI(player_t.white, testBoard).getMove();
      testBoard.move(move.Item1, move.Item2);
      Assert.IsFalse(testBoard.getWinner() == player_t.neither);
    }
  }
}
