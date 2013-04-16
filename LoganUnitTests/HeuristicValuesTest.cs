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
      var uutList = new List<Tuple<Heuristic, Heuristic>>();
      // Note: First must be bettern than second. 
      uutList.Add(Tuple.Create(new Heuristic(1.0, 0), new Heuristic(1.0, 1)));
      uutList.Add(Tuple.Create(new Heuristic(1.0, 0), new Heuristic(2.0, 1)));
      uutList.Add(Tuple.Create(new Heuristic(0.0001, 3), new Heuristic(1.0, 5)));
      uutList.Add(Tuple.Create(new Heuristic(1.1, 0), new Heuristic(1.0, 0)));
      uutList.Add(Tuple.Create(new Heuristic(-1.0, 1), new Heuristic(-2.0, 1)));
      uutList.Add(Tuple.Create(new Heuristic(-50.0, 0), new Heuristic(1.0, 5)));

      foreach (Tuple<Heuristic, Heuristic> uutTuple in uutList) {
        Assert.IsTrue(uutTuple.Item2 < uutTuple.Item1);
        Assert.IsTrue(uutTuple.Item1 != uutTuple.Item2);
        Assert.IsTrue(uutTuple.Item2 != uutTuple.Item1);
        Assert.IsTrue(uutTuple.Item2 <= uutTuple.Item1);
        Assert.IsTrue(uutTuple.Item1 >= uutTuple.Item2);
        Assert.IsTrue(uutTuple.Item1 > uutTuple.Item2);
        Assert.IsTrue(uutTuple.Item1 != null);
        Assert.IsTrue(uutTuple.Item2 != null);
      }

      uutList.Clear();
      // Note: Must be equal
      uutList.Add(Tuple.Create(new Heuristic(0.0, 0), new Heuristic(0.0, 0)));
      uutList.Add(Tuple.Create(new Heuristic(1.0, 3), new Heuristic(1.0, 3)));
      uutList.Add(Tuple.Create(new Heuristic(0.0001, 2), new Heuristic(0.0001, 2)));
      uutList.Add(new Tuple<Heuristic, Heuristic>(null, null));

      foreach (Tuple<Heuristic, Heuristic> uutTuple in uutList) {
        Assert.IsTrue(uutTuple.Item1 == uutTuple.Item2);
        Assert.IsTrue(uutTuple.Item1 >= uutTuple.Item2);
        Assert.IsTrue(uutTuple.Item1 <= uutTuple.Item2);
      }

      Heuristic uutA = null;
      Heuristic uutB = null;
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
