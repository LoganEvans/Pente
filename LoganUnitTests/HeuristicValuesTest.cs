using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CommonInterfaces;
using LoganPenteAI;

namespace LoganUnitTests {
  [TestClass]
  public class HeuristicValuesTest {
    [TestMethod]
    public void TestHeuristicComparison() {
      Heuristic uutA, uutB;
      uutA = new Heuristic(1.0, 0);
      uutB = new Heuristic(1.0, 1);
      Boolean x;
      Assert.IsTrue(uutA > uutB);
      Assert.IsTrue(uutA != uutB);
      Assert.IsTrue(uutA >= uutB);
      x = uutB < uutA;
      Assert.IsTrue(uutB < uutA);
      Assert.IsTrue(uutB != uutA);
      Assert.IsTrue(uutB <= uutA);
      Assert.IsTrue(x);
      Assert.IsTrue(uutA != null);
      Assert.IsTrue(uutB != null);

      uutA = new Heuristic(1.0, 2);
      uutB = new Heuristic(0.0, 2);
      Assert.IsTrue(uutA > uutB);
      Assert.IsTrue(uutA != uutB);
      Assert.IsTrue(uutA >= uutB);
      Assert.IsTrue(uutB < uutA);
      Assert.IsTrue(uutB != uutA);
      Assert.IsTrue(uutB <= uutA);
      Assert.IsTrue(uutA != null);
      Assert.IsTrue(uutB != null);

      uutA = new Heuristic(10.0, 5);
      uutB = new Heuristic(10.0, 5);
      Assert.IsTrue(uutA == uutB);
      Assert.IsTrue(uutA <= uutB);
      Assert.IsTrue(uutA >= uutB);
      Assert.IsTrue(uutB <= uutA);
      Assert.IsTrue(uutB >= uutA);
      Assert.IsTrue(uutA != null);
      Assert.IsTrue(uutB != null);

      uutA = null;
      uutB = null;
      Assert.IsTrue(uutA == null);
      Assert.IsTrue(uutB == null);
    }

    [TestMethod]
    public void TestDictionary() {
      // Note: the text file for this dictionary is NOT in the same location as the one for the main release.
      Dictionary<Pattern, Heuristic> uut = HeuristicValues.GetHeuristicDict();

      Assert.AreEqual(65280, uut.Count);

      Pattern pattern;
      pattern = new Pattern(0, Convert.ToInt32("111101000", 2));
      Assert.IsTrue(uut.ContainsKey(pattern));
      pattern = new Pattern(Convert.ToInt32("111101000", 2), 0);
      Assert.IsTrue(uut.ContainsKey(pattern));
    }
  }
}
