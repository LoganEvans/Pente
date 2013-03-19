using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CommonInterfaces;
using LoganPenteAI;

namespace LoganUnitTests {
  [TestClass]
  public class UnitTest1 {
    [TestMethod]
    public void test_getMove_shouldTakeWin() {
      Board testBoard = new Board();
      // Default of (9, 9) for white
      testBoard.move(9, 10);  // black
      testBoard.move(13, 9);  // white
      testBoard.move(9, 11);  // black
      testBoard.move(11, 9);  // white
      testBoard.move(9, 12);  // black
      testBoard.move(10, 9);  // white
      testBoard.move(9, 13);  // black

      Assert.AreEqual<Tuple<int, int>>(Tuple.Create(10, 9), new PlayerAI(player_t.white, testBoard).getMove());
    }
  }
}
