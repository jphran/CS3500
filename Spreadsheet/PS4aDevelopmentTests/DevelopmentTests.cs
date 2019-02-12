using Microsoft.VisualStudio.TestTools.UnitTesting;
using Formulas;
using System.Collections.Generic;
using System;

namespace PS4aDevelopmentTests
{
    [TestClass]
    public class DevelopmentTests
    {
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ThreeArg4()
        {
            Formula f = new Formula("x+y", s => s == "x" ? "z" : s, s => s != "z");
        }
        [TestMethod]
        public void ThreeArg7()
        {
            Formula f = new Formula("y", s => "x", s => true);
            Assert.AreEqual(1.0, f.Evaluate(s => (s == "x") ? 1 : 0), 1e-6);
        }

        [TestMethod]
        public void GetVars3()
        {
            Formula f = new Formula("a * b - c + d / e * 2.5e6");
            var expected = new HashSet<string>();
            expected.Add("a");
            expected.Add("b");
            expected.Add("c");
            expected.Add("d");
            expected.Add("e");
            var actual = f.GetVariables();
            Assert.IsTrue(expected.SetEquals(actual));
        }

        [TestMethod]
        public void ToString4()
        {
            Formula f1 = new Formula("a+b*(c-15)/2");
            Formula f2 = new Formula(f1.ToString());
            Assert.AreEqual(24.0, f2.Evaluate(s => char.IsLower(s[0]) ? 16 : 0), 1e-6);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullConstructorParamsFormula()
        {
            Formula f0 = new Formula(null, s => s, s => true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullConstructorParamNorm()
        {
            Formula f1 = new Formula("a+b*(c-15)/2", null, s => true);
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullConstructorParamsValidator()
        {
            Formula f2 = new Formula("a+b*(c-15)/2", s => s, null);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void InvalidNormalizer()
        {
            Formula f1 = new Formula("a+b*(c-15)/2", s => (s = "!"), s => true);
        }

        [TestMethod]
        public void CheckNormalizedList()
        {
            Formula f1 = new Formula("a+b*(c-15)/2", s => s.ToUpper(), s => (s == s.ToUpper()));
            Assert.AreEqual("A+B*(C-15)/2", f1.ToString());
        }

        [TestMethod]
        public void CheckNormalizedVarSet()
        {
            string[] vars = new string[3];
            Formula f1 = new Formula("a+b*(c-15)/2", s => s.ToUpper(), s => (s == s.ToUpper()));
            f1.GetVariables().CopyTo(vars,0);
            Assert.IsTrue(vars[0] == "A");
            Assert.IsTrue(vars[1] == "B");
            Assert.IsTrue(vars[2] == "C");
        }

        [TestMethod]
        public void CheckNormalizedVarSetSingleParamConstructor()
        {
            string[] vars = new string[3];
            Formula f1 = new Formula("a+b*(c-15)/2");
            f1.GetVariables().CopyTo(vars, 0);
            Assert.IsTrue(vars[0] == "a");
            Assert.IsTrue(vars[1] == "b");
            Assert.IsTrue(vars[2] == "c");
            Assert.AreEqual("a+b*(c-15)/2", f1.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CheckNullMethods1()
        {
            Formula f1 = new Formula(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CheckNullMethods2()
        {
            Formula f1 = new Formula("x2 * y6 /  j8", null, s => true);
            
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CheckNullMethods3()
        {
            Formula f1 = new Formula("x2 * y6 /  j8", s => s, null);

        }

        //[TestMethod]
        //[ExpectedException(typeof(ArgumentNullException))]
        //public void CheckNullMethods4()
        //{
        //    Formula f1 = null;
        //    f1.ToString();

        //}
    }
}
