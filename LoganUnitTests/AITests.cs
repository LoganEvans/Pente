using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CommonInterfaces;
using LoganPenteAI;

namespace LoganUnitTests {
  [TestClass]
  public class AITests {
    [TestMethod]
    public void TestGetMoveShouldTakeWin() {
      Assert.Fail();
      Board testBoard = new Board();
      // Default of (9, 9) for white
      testBoard.Move(9, 10);  // black
      testBoard.Move(13, 9);  // white
      testBoard.Move(9, 11);  // black
      testBoard.Move(11, 9);  // white
      testBoard.Move(9, 12);  // black
      testBoard.Move(10, 9);  // white
      testBoard.Move(9, 13);  // black

      Assert.IsTrue(testBoard.GetWinner() == Player.Neither);
      Tuple<int, int> move = new PlayerAI(Player.White, testBoard).GetMove();
      testBoard.Move(move.Item1, move.Item2);
      Assert.IsFalse(testBoard.GetWinner() == Player.Neither);
    }
  }
}
