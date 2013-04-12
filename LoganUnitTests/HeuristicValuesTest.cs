using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CommonInterfaces;
using LoganPenteAI;

namespace LoganUnitTests {
  [TestClass]
  public class HeuristicValuesTest {
    [TestMethod]
    public void TestDictionary() {
      // Note: the text file for this dictionary is NOT in the same location as the one for the main release.
      Dictionary<Pattern, Tuple<double, int>> uut = HeuristicValues.GetHeuristicDict();

      Assert.AreEqual(65280, uut.Count);

      Pattern pattern = new Pattern();
      pattern.SetPattern(0, Convert.ToInt32("111101000", 2));
      Assert.IsTrue(uut.ContainsKey(pattern));
      pattern.SetPattern(Convert.ToInt32("111101000", 2), 0);
      Assert.IsTrue(uut.ContainsKey(pattern));
    }
  }
}
