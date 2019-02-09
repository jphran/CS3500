using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dependencies;
using System.Collections.Generic;
using System;

namespace PS4DevelopmentTests
{
    [TestClass]
    public class DevelopmentTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Null1()
        {
            DependencyGraph d = new DependencyGraph();
            d.AddDependency("a", null);
        }

        [TestMethod]
        public void Copy1()
        {
            var d1 = new DependencyGraph();
            var d2 = new DependencyGraph(d1);
            Assert.AreEqual(0, d1.Size);
            Assert.AreEqual(0, d2.Size);
        }

        [TestMethod]
        public void Copy5()
        {
            var d1 = new DependencyGraph();
            d1.AddDependency("a", "b");
            d1.AddDependency("d", "e");
            var d2 = new DependencyGraph(d1);
            d1.AddDependency("a", "c");
            d2.AddDependency("d", "f");
            Assert.AreEqual(2, new List<string>(d1.GetDependents("a")).Count);
            Assert.AreEqual(1, new List<string>(d1.GetDependents("d")).Count);
            Assert.AreEqual(2, new List<string>(d2.GetDependents("d")).Count);
            Assert.AreEqual(1, new List<string>(d2.GetDependents("a")).Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullMethods1()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency(null, "x");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullMethods2()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.GetDependees(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullMethods3()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.GetDependents(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullMethods4()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.HasDependees(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullMethods5()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.HasDependents(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullMethods6()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.RemoveDependency(null, "x");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullMethods7()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.RemoveDependency("x", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullMethods8()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.ReplaceDependees(null, new HashSet<string>());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullMethods9()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.ReplaceDependees("x", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullMethods10()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.ReplaceDependents("x", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullMethods11()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.ReplaceDependents(null, new HashSet<string>());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullMethods12()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("x", "a");
            dg.AddDependency("a", "x");
            HashSet<string> hs = new HashSet<string>();
            hs.Add("q");
            hs.Add(null);
            dg.ReplaceDependents("x", hs);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullMethods13()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("x", "a");
            dg.AddDependency("a", "x");

            HashSet<string> hs = new HashSet<string>();
            hs.Add("q");
            hs.Add(null);
            dg.ReplaceDependees("a", hs);
        }

        [TestMethod]
        public void ConstructorDGParam()
        {
            DependencyGraph dg1 = new DependencyGraph();
            dg1.AddDependency("a", "z");
            dg1.AddDependency("a", "y");
            dg1.AddDependency("a", "x");
            dg1.AddDependency("b", "a");
            DependencyGraph dg2 = new DependencyGraph(dg1);
            dg1.RemoveDependency("a", "z");
            dg1.RemoveDependency("a", "y");
            dg1.RemoveDependency("a", "x");
            dg1.RemoveDependency("b", "a");

            Assert.AreEqual(0, dg1.Size);
            Assert.AreEqual(4, dg2.Size);
            Assert.IsTrue(dg2.HasDependees("a"));
            Assert.IsTrue(dg2.HasDependents("a"));
        }

        [TestMethod]
        public void ConstructorDGParam1()
        {
            DependencyGraph dg1 = new DependencyGraph();
            dg1.AddDependency("a", "z");
            dg1.AddDependency("a", "y");
            dg1.AddDependency("a", "x");
            dg1.AddDependency("b", "a");
            DependencyGraph dg2 = new DependencyGraph(dg1);
            dg2.AddDependency("c", "k");

            Assert.AreEqual(4, dg1.Size);
            Assert.AreEqual(5, dg2.Size);
            Assert.IsTrue(dg2.HasDependees("k"));
            Assert.IsFalse(dg1.HasDependents("c"));
        }

        [TestMethod]
        public void TimingConstructorDGParam1()
        {
            DependencyGraph dg1 = new DependencyGraph();
            for(int i = 0; i < 10000; i++)
            {
                dg1.AddDependency(i.ToString(), "a");
            }
            DependencyGraph dg2 = new DependencyGraph(dg1);

            Assert.AreEqual(10000, dg2.Size);
            Assert.IsTrue(dg2.HasDependees("a"));
            Assert.IsTrue(dg2.HasDependents("559"));
        }

        [TestMethod]
        public void TimingConstructorDGParam2()
        {
            DependencyGraph dg1 = new DependencyGraph();
            for (int i = 0; i < 1000; i++)
            {
                for (int j = 0; j < 1000; j++)
                {
                    dg1.AddDependency(i.ToString(), j.ToString());
                }
            }
            DependencyGraph dg2 = new DependencyGraph(dg1);

            Assert.AreEqual(1e6, dg2.Size);
            Assert.AreEqual(1e6, dg1.Size);
            Assert.IsTrue(dg2.HasDependees("973"));
            Assert.IsTrue(dg2.HasDependents("7"));

            dg1.RemoveDependency("1", "1");
            Assert.AreEqual(1e6 - 1, dg1.Size);
            Assert.AreEqual(1e6, dg2.Size);
        }
    }
}
