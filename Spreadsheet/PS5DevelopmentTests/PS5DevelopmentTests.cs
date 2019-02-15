using SS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Formulas;

namespace DevelopmentTests
{
    /// <summary>
    /// These are grading tests for PS5
    ///</summary>
    [TestClass()]
    public class SpreadsheetTest
    {
        // EMPTY SPREADSHEETS
        [TestMethod()]
        public void Test3()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            Assert.AreEqual("", s.GetCellContents("A2"));
        }

        // SETTING CELL TO A DOUBLE
        [TestMethod()]
        public void Test6()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetCellContents("Z7", 1.5);
            Assert.AreEqual(1.5, (double)s.GetCellContents("Z7"), 1e-9);
        }

        // SETTING CELL TO A STRING
        [TestMethod()]
        public void Test10()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetCellContents("Z7", "hello");
            Assert.AreEqual("hello", s.GetCellContents("Z7"));
        }

        // SETTING CELL TO A FORMULA
        [TestMethod()]
        public void Test13()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetCellContents("Z7", new Formula("3"));
            Formula f = (Formula)s.GetCellContents("Z7");
            Assert.AreEqual(3, f.Evaluate(x => 0), 1e-6);
        }

        // CIRCULAR FORMULA DETECTION
        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void Test14()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetCellContents("A1", new Formula("A2"));
            s.SetCellContents("A2", new Formula("A1"));
        }

        //NULL NAME
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void JTest1()
        {
            string nullString = null;
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetCellContents("A1", new Formula("A2"));
            s.SetCellContents(nullString, new Formula("A1"));
        }

        //INVALID NAME
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void JTest2()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetCellContents("A1", new Formula("A2"));
            s.SetCellContents("X0", new Formula("A1"));
        }

        //INVALID NAME 2
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void JTest3()
        { 
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetCellContents("A1", new Formula("A2"));
            s.SetCellContents("17", new Formula("A1"));
        }

        //INVALID NAME 3
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void JTest5()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetCellContents("A1", new Formula("A2"));
            s.SetCellContents("Z", new Formula("A1"));
        }

        //stress test
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void JTest4()
        {
            AbstractSpreadsheet s = new Spreadsheet();

            for (int i = 0; i < 1000 / 26; i++)
            {
                for (int j = 'a'; j < 'a' + 'Z'; j++)
                {
                    char c = (char)j;
                    string name = c.ToString();
                    s.SetCellContents(name, i);
                }

            }
        }

        //check unique cells
        [TestMethod()]
        public void JTest6()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetCellContents("A1", "not hello");
            s.SetCellContents("A1", "hello");
            Assert.AreEqual("hello", s.GetCellContents("A1"));
            Assert.AreNotEqual("not hello", s.GetCellContents("A1"));
        }

        //Circ test
        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void JTest7()
        {
            AbstractSpreadsheet s = new Spreadsheet();

            s.SetCellContents("z16930", new Formula("A1+A2"));
            s.SetCellContents("A1", new Formula("46 * t19 + A2"));
            s.SetCellContents("b3", new Formula("b3"));
        }

        //Circ test 2, indirect
        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void JTest8()
        {
            AbstractSpreadsheet s = new Spreadsheet();

            s.SetCellContents("A19", new Formula("A1+A2"));
            s.SetCellContents("A1", new Formula("A3+A4"));
            s.SetCellContents("A3", new Formula("A19"));
        }

        //getnamesofallnonemptycells
        [TestMethod()]
        public void JTest9()
        {
            AbstractSpreadsheet s = new Spreadsheet();

            s.SetCellContents("A1", 1);
            s.SetCellContents("a1", 2);
            s.SetCellContents("b1", "");
            HashSet<string> names = (HashSet<string>)s.GetNamesOfAllNonemptyCells();

            foreach (string n in names)
            {
                Assert.IsTrue(n.Equals("A1") | n.Equals("a1"));
            }

        }

        //throws
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void JTest10()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.GetCellContents("");
        }


        //returned set
        [TestMethod()]
        public void JTest11()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetCellContents("A1", new Formula("B1"));
            s.SetCellContents("A2", new Formula("B1"));
            s.SetCellContents("A3", new Formula("B1"));
            s.SetCellContents("A4", new Formula("A3"));

            HashSet<string> set = new HashSet<string>() { "A1", "A2", "A3", "A4", "B1"};

            foreach(string st in s.SetCellContents("B1", 1))
            {
                Assert.IsTrue(set.Contains(st));
            }
            
        }

        //throws
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void JTest12()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.GetCellContents(null);
        }

        //throws
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void JTest13()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetCellContents(null, 11);
        }

        //throws
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void JTest14()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetCellContents(null, "hello");
        }

        //throws
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void JTest15()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetCellContents("A0", "hello");
        }

    }
}
