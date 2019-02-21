using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dependencies;
using Formulas;

namespace SS
{
    /// <summary>
    /// 
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        private Dictionary<string, Cell> table; //holds cell location and contents/value
        private DependencyGraph dg = new DependencyGraph(); //holds dependencies
        private static Regex IsValid = new Regex(@"[a-zA-Z]+[1-9]\d*", RegexOptions.IgnorePatternWhitespace); //regex to detect invalid names
        private bool IsChanged; //var to tell if ss has been changed since last save

        /// <summary>
        /// An AbstractSpreadsheet object represents the state of a simple spreadsheet.  A 
        /// spreadsheet consists of a regular expression (called IsValid below) and an infinite 
        /// number of named cells.
        /// 
        /// A string is a valid cell name if and only if (1) s consists of one or more letters, 
        /// followed by a non-zero digit, followed by zero or more digits AND (2) the C#
        /// expression IsValid.IsMatch(s.ToUpper()) is true.
        /// 
        /// For example, "A15", "a15", "XY32", and "BC7" are valid cell names, so long as they also
        /// are accepted by IsValid.  On the other hand, "Z", "X07", and "hello" are not valid cell 
        /// names, regardless of IsValid.
        /// 
        /// Any valid incoming cell name, whether passed as a parameter or embedded in a formula,
        /// must be normalized by converting all letters to upper case before it is used by this 
        /// this spreadsheet.  For example, the Formula "x3+a5" should be normalize to "X3+A5" before 
        /// use.  Similarly, all cell names and Formulas that are returned or written to a file must also
        /// be normalized.
        /// 
        /// A spreadsheet contains a unique cell corresponding to every possible cell name.  
        /// In addition to a name, each cell has a contents and a value.  The distinction is
        /// important, and it is important that you understand the distinction and use
        /// the right term when writing code, writing comments, and asking questions.
        /// 
        /// The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
        /// contents is an empty string, we say that the cell is empty.  (By analogy, the contents
        /// of a cell in Excel is what is displayed on the editing line when the cell is selected.)
        /// 
        /// In an empty spreadsheet, the contents of every cell is the empty string.
        ///  
        /// The value of a cell can be (1) a string, (2) a double, or (3) a FormulaError.  
        /// (By analogy, the value of an Excel cell is what is displayed in that cell's position
        /// in the grid.)
        /// 
        /// If a cell's contents is a string, its value is that string.
        /// 
        /// If a cell's contents is a double, its value is that double.
        /// 
        /// If a cell's contents is a Formula, its value is either a double or a FormulaError.
        /// The value of a Formula, of course, can depend on the values of variables.  The value 
        /// of a Formula variable is the value of the spreadsheet cell it names (if that cell's 
        /// value is a double) or is undefined (otherwise).  If a Formula depends on an undefined
        /// variable or on a division by zero, its value is a FormulaError.  Otherwise, its value
        /// is a double, as specified in Formula.Evaluate.
        /// 
        /// Spreadsheets are never allowed to contain a combination of Formulas that establish
        /// a circular dependency.  A circular dependency exists when a cell depends on itself.
        /// For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
        /// A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
        /// dependency.
        /// </summary>
        public Spreadsheet() => table = new Dictionary<string, Cell>();

        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved
        /// (whichever happened most recently); false otherwise.
        /// </summary>
        public override bool Changed { get => IsChanged; protected set => IsChanged = value; }


        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
        public override object GetCellContents(string name)
        {
            name = NormalizedName(name);
            if (table.TryGetValue(name, out Cell cell)) //pull appropriate cell from spreadsheet
            {
                return cell.contents;
            }

            return "";
        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            Changed = true;
            IEnumerable<string> keys = table.Keys; //pull all cells
            HashSet<string> nonEmptyCells = new HashSet<string>(); //container for nonempty cells names

            foreach (string s in keys) //find nonempty cells
            {
                if (table.TryGetValue(s, out Cell cell))
                {
                    if (!cell.contents.Equals(""))
                    {
                        nonEmptyCells.Add(s);
                    }
                }

            }
            return nonEmptyCells;
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes numbeIsValid.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, double number)
        {
            name = NormalizedName(name);

            table[name] = new Cell("numbeIsValid", number); //reset value in dictionary


            // create iset full of dependencies both direct and indirect
            HashSet<string> dependents = new HashSet<string>(dg.GetDependents(name));
            IEnumerable<string> indirectDependents = GetCellsToRecalculate(dependents);

            foreach (string s in indirectDependents)
            {
                dependents.Add(s);
            }

            foreach(string s in dependents)
            {
                table.TryGetValue(s, out Cell cell);
                SetCellContents(s, (Formula) cell.contents);
            }

            return dependents;
        }

        /// <summary>
        /// If text is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes text.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, string text)
        {

            if (text == null)
            {
                throw new ArgumentNullException("Text is null, please revise");
            }
            name = NormalizedName(name);

            table[name] = new Cell(text, text); //update value


            //crete iset of dependencies both direct and indirect
            HashSet<string> dependents = new HashSet<string>(dg.GetDependents(name));
            IEnumerable<string> indirectDependents = GetCellsToRecalculate(dependents);

            foreach (string s in indirectDependents)
            {
                dependents.Add(s);
            }

            return dependents;
        }

        /// <summary>
        /// Requires that all of the variables in formula are valid cell names.
        /// 
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException.
        /// 
        /// Otherwise, the contents of the named cell becomes formula.  The method returns a
        /// Set consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, Formula formula)
        {
            name = NormalizedName(name);

            foreach (string var in formula.GetVariables()) //add dependencies to dictionary
            {
                dg.AddDependency(var, name);
            }

            //create set of dependencies both indirect and direct
            HashSet<string> dependents = new HashSet<string>(dg.GetDependents(name));
            IEnumerable<string> indirectDependents = GetCellsToRecalculate(dependents);

            foreach (string s in indirectDependents)
            {
                dependents.Add(s);
            }
     
            table[name] = new Cell(new Formula(formula.ToString(), s => s = s.ToUpper(), v => true), formula.Evaluate(s => (double) this.GetCellValue(s))); //update dictionary

            return dependents;
        }

        /// <summary>
        /// If name is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name isn't a valid cell name, throws an InvalidNameException.
        /// 
        /// Otherwise, returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// 
        /// For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents of A1 are B1 and C1
        /// </summary>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            Changed = true;
            if (name == null)
            {
                throw new ArgumentNullException();
            }
            if (!IsValid.IsMatch(name.ToUpper()))
            {
                throw new InvalidNameException();
            }

            return dg.GetDependees(name.ToUpper());
        }

        /// <summary>
        /// Writes the contents of this spreadsheet to dest using an XML format.
        /// The XML elements should be structured as follows:
        ///
        /// <spreadsheet IsValid="IsValid regex goes here">
        ///   <cell name="cell name goes here" contents="cell contents go here"></cell>
        ///   <cell name="cell name goes here" contents="cell contents go here"></cell>
        ///   <cell name="cell name goes here" contents="cell contents go here"></cell>
        /// </spreadsheet>
        ///
        /// The value of the IsValid attribute should be IsValid.ToString()
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.
        /// If the cell contains a string, the string (without surrounding double quotes) should be written as the contents.
        /// If the cell contains a double d, d.ToString() should be written as the contents.
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
        ///
        /// If there are any problems writing to dest, the method should throw an IOException.
        /// </summary>
        /// <param name="dest"></param>
        public override void Save(TextWriter dest)
        {
            Changed = true;
            throw new NotImplementedException();
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        ///
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a FormulaError.
        /// </summary>
        public override object GetCellValue(string name)
        {


            name = NormalizedName(name);
            if (table.TryGetValue(name, out Cell cell))
            {
                return cell.value;
            }

            return ""; 
        }

        /// <summary>
        /// If content is null, throws an ArgumentNullException.
        ///
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        ///
        /// Otherwise, if content parses as a double, the contents of the named
        /// cell becomes that double.
        ///
        /// Otherwise, if content begins with the character '=', an attempt is made
        /// to parse the remainder of content into a Formula f using the Formula
        /// constructor with s => s.ToUpper() as the normalizer and a validator that
        /// checks that s is a valid cell name as defined in the AbstractSpreadsheet
        /// class comment.  There are then three possibilities:
        ///
        ///   (1) If the remainder of content cannot be parsed into a Formula, a
        ///       Formulas.FormulaFormatException is thrown.
        ///
        ///   (2) Otherwise, if changing the contents of the named cell to be f
        ///       would cause a circular dependency, a CircularException is thrown.
        ///
        ///   (3) Otherwise, the contents of the named cell becomes f.
        ///
        /// Otherwise, the contents of the named cell becomes content.
        ///
        /// If an exception is not thrown, the method returns a set consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell.
        ///
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        public override ISet<string> SetContentsOfCell(string name, string content)
        {
            Changed = true;
            throw new NotImplementedException();
        }

        /// <summary>
        /// return normalized version of string name if name is a valid name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string NormalizedName(string name)
        {
            Changed = true;
            if (name == null || !IsValid.IsMatch(name.ToUpper())) //check for inproper name
            {
                throw new InvalidNameException();
            }

            return name.ToUpper();
        }

        
    }

    /// <summary>
    /// individual container for contents and value of spreadsheets
    /// </summary>
    public struct Cell
    {
        /// <summary>
        /// Contents within the cell
        /// </summary>
        public object contents;
        /// <summary>
        /// Value of cell in spreadsheet
        /// </summary>
        public object value; //needed for PS6

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_contents"></param>
        /// <param name="_value"></param>
        public Cell(object _contents, object _value)
        {
            this.contents = _contents;
            this.value = _value;
        }
    }



}

