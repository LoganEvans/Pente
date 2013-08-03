using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using PenteInterfaces;
using PenteAI;

namespace UnitTests {
  [TestClass]
  public class AITests {
    [TestMethod]
    public void TestGetMoveShouldTakeWin() {
      GameState uut = new GameState(Player.White, 2, 4,
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
".........WBBBB....." +  // 9 (center)
".........W........." +  // 10
".........W........." +  // 11
"..................." +  // 12
".........W........." +  // 13
"..................." +  // 14
"..................." +  // 15
"..................." +  // 16
"..................." +  // 17
"...................");  // 18

      Tuple<int, int> expectedMove = Tuple.Create(12, 9);
      Tuple<int, int> move;
      move = uut.GetBestMove(0);
      Assert.AreEqual(expectedMove, move);
      move = uut.GetBestMove(1);
      Assert.AreEqual(expectedMove, move);
      move = uut.GetBestMove(2);
      Assert.AreEqual(expectedMove, move);
    }
  }
}
