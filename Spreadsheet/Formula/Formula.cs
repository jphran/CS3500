// Skeleton written by Joe Zachary for CS 3500, January 2019
// Edited by Justin Francis, Jan 2019 v1.0+ ready for release (grade)
// Updated by Justin Francis, Feb 2019 v2.0.1 ready for release (grade)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using static Formulas.TokenType;

namespace Formulas
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  Provides a means to evaluate Formulas.  Formulas can be composed of
    /// non-negative floating-point numbers, variables, left and right parentheses, and
    /// the four binary operator symbols +, -, *, and /.  (The unary operators + and -
    /// are not allowed.)
    /// </summary>
    public class Formula
    {
        private string formula;
        private List<Token> tokenList;
        private HashSet<string> variableSet = new HashSet<string>();

        /// <summary>
        /// Creates a Formula from a string that consists of a standard infix expression composed
        /// from non-negative floating-point numbers (using C#-like syntax for double/int literals), 
        /// variable symbols (a letter followed by zero or more letters and/or digits), left and right
        /// parentheses, and the four binary operator symbols +, -, *, and /.  White space is
        /// permitted between tokens, but is not required.
        /// 
        /// Examples of a valid parameter to this constructor are:
        ///     "2.5e9 + x5 / 17"
        ///     "(5 * 2) + 8"
        ///     "x*y-2+35/9"
        ///     
        /// Examples of invalid parameters are:
        ///     "_"
        ///     "-5.3"
        ///     "2 5 + 3"
        /// 
        /// If the formula is syntacticaly invalid, throws a FormulaFormatException with an 
        /// explanatory Message.
        /// </summary>
        public Formula(String formula)
        {
            //check if null argument 
            if (formula == null)
            {
                throw new ArgumentNullException("Null formula, please revise");
            }

            this.formula = formula; //idk how else to pass this to Evaluate()
            this.tokenList = new List<Token>(); //list to pass to other constructor/evaluate
            TokenType? previous = null; //make TokenType an nullable type and set to null to check first token's TokenType
            Stack lParen = new Stack(); //opening paren stack 
            Stack rParen = new Stack(); //closing paren stack

            IEnumerable<Token> tokenEnum = GetTokens(formula);

            //check and make sure there is at least one token (req 2)
            if (tokenEnum == null || !tokenEnum.GetEnumerator().MoveNext())
            {
                throw new FormulaFormatException("Empty formula");
            }

            //consume enum and check inputted formula for syntatical correctness as defined in PS2 and the summary abovve
            foreach (Token t in tokenEnum)
            {

                //When reading tokens from left to right, at no point should the
                //number of closing parentheses seen so far be greater than the number
                //of opening parentheses seen so far (req 3)
                if (t.Type.Equals(LParen))
                {
                    lParen.Push(t.Text);
                }
                else if (t.Type.Equals(RParen))
                {
                    rParen.Push(t.Text);
                }
                //check parens 
                if (rParen.Count > lParen.Count)
                {
                    throw new FormulaFormatException("Formula contains too many closing parenthesis");
                }


                //check if first token is open paren (req 5)
                if (t.Type.Equals(RParen) && previous.Equals(null))
                {
                    throw new FormulaFormatException("Formula starts with closing paren, please revise");
                }

                //check if first token is Oper (req 5)
                if (t.Type.Equals(Oper) && previous.Equals(null))
                {
                    throw new FormulaFormatException("Formula starts with operator, please revise");
                }

                //Any token that immediately follows an opening parenthesis or an operator must be either a number, a variable, or an opening parenthesis (req 7)
                if ((t.Type.Equals(Oper) && (previous.Equals(Oper) || previous.Equals(LParen))) || (t.Type.Equals(RParen) && (previous.Equals(Oper) || previous.Equals(LParen))))
                {
                    throw new FormulaFormatException("Forumula contains back to back operators, please revise");
                }

                //token that immediately follows an opening parenthesis or an operator must be either a number, a variable, or an opening parenthesis (req 8)
                if ((previous.Equals(Number) && (!t.Type.Equals(Oper) && !t.Type.Equals(RParen))) || (previous.Equals(Var) && (!t.Type.Equals(Oper) && !t.Type.Equals(RParen))) || (previous.Equals(RParen) && (!t.Type.Equals(Oper) && !t.Type.Equals(RParen))))
                {
                    throw new FormulaFormatException("Forumula contains back to back vars, numbers, or , please revise");
                }

                // no invalid tokens (req 1)
                if (t.Type.Equals(Invalid))
                {
                    throw new FormulaFormatException("Forumula contains invalid syntax, please revise");
                }

                //add vars to ISet to return in GetVariables()
                if (t.Type.Equals(Var))
                {
                    variableSet.Add(t.Text);
                }

                previous = t.Type;
                tokenList.Add(t);
            }

            //check ending of formula for completion (req 6)
            if (previous.Equals(Oper) || previous.Equals(LParen))
            {
                throw new FormulaFormatException("Forumula ends with invalid syntax, please revise");
            }

            //check same number of open/close parens (req 4)
            if (rParen.Count != lParen.Count)
            {
                throw new FormulaFormatException("Formula contains mismatched parenthesis");
            }

        }

        /// <summary>
        /// Creates a Formula from a string that consists of a standard infix expression composed
        /// from non-negative floating-point numbers (using C#-like syntax for double/int literals), 
        /// variable symbols (a letter followed by zero or more letters and/or digits), left and right
        /// parentheses, and the four binary operator symbols +, -, *, and /.  White space is
        /// permitted between tokens, but is not required.
        /// Uses a Normalizer and Validator on all variables
        /// Examples of a valid parameter to this constructor are:
        ///     "2.5e9 + x5 / 17"
        ///     "(5 * 2) + 8"
        ///     "x*y-2+35/9"
        ///     
        /// Examples of invalid parameters are:
        ///     "_"
        ///     "-5.3"
        ///     "2 5 + 3"
        /// 
        /// If the formula is syntacticaly invalid, throws a FormulaFormatException with an 
        /// explanatory Message. 
        /// </summary>
        /// <param name="formula"></param>
        /// <param name="norm"></param>
        /// <param name="valid"></param>
        public Formula(string formula, Normalizer norm, Validator valid) : this(formula)
        {
            //delete vars from variableSet if previously ran in single param constructor
            variableSet.Clear();

            //check if null param
            if (formula == null || norm == null || valid == null)
            {
                throw new ArgumentNullException("Null parameter, please revise");
            }

            Regex r = new Regex(@"[a-zA-Z][0-9a-zA-Z]*", RegexOptions.IgnorePatternWhitespace);
            List<Token> normalizedTokenList = new List<Token>();

            foreach (Token x in tokenList)
            {
                //if the token is a variable, add the token to the normalized list and the var to variable set
                if (x.Type.Equals(Var))
                {
                    string normalizedVar = norm(x.Text);

                    if (!r.IsMatch(normalizedVar)) //check if N(x) is not a legal variable
                    {
                        throw new FormulaFormatException("Invalid variable via Normalizer, please revise");
                    }
                    if (valid(normalizedVar) != true) //check if V(N(x)) is false aka invalid
                    {
                        throw new FormulaFormatException("Invalid variable via Validator, please revise");
                    }


                    normalizedTokenList.Add(new Token(normalizedVar, Var));
                    variableSet.Add(normalizedVar);



                }

                else
                {
                    normalizedTokenList.Add(x); //only add token to normalized list
                }
            }

            tokenList = normalizedTokenList;


        }

        /// <summary>
        /// Returns ISet<string> of all normalized variables</string>
        /// </summary>
        /// <returns></returns>
        public ISet<string> GetVariables()
        {
            if(this == null)
            {
                throw new ArgumentNullException("Cannot get variables from null formula, please revise");
            }

            return variableSet;
        }

        /// <summary>
        /// Evaluates this Formula, using the Lookup delegate to determine the values of variables.  (The
        /// delegate takes a variable name as a parameter and returns its value (if it has one) or throws
        /// an UndefinedVariableException (otherwise).  Uses the standard precedence rules when doing the evaluation.
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, its value is returned.  Otherwise, throws a FormulaEvaluationException  
        /// with an explanatory Message.
        /// </summary>
        public double Evaluate(Lookup lookup)
        {
            //check if null param
            if (lookup == null)
            {
                throw new ArgumentNullException("Null Lookup delegate, please revise");
            }

            Stack<double> values = new Stack<double>();
            Stack<string> operators = new Stack<string>();

            //consume enum and evaluate as appropriate
            foreach (Token t in tokenList)
            {
                TokenType tokenType = t.Type; //short hand
                string token = t.Text; //short hand

                //handles TokenType [Number] as defined in PS2
                if (tokenType.Equals(Number))
                {
                    if (OpPeek(operators, "*")) //checks operators stack for multiplication oper
                    {
                        operators.Pop(); //pull op off stack
                        values.Push(values.Pop() * double.Parse(token)); //evaluate expression and push to values stack
                    }
                    else if (OpPeek(operators, "/")) //check operator stack for division oper
                    {
                        if (double.Parse(token) == 0.0) //check for divide by 0
                        {
                            throw new FormulaEvaluationException("Cannot divide by zero, please revise");
                        }
                        operators.Pop(); //remove oper from stack
                        values.Push(values.Pop() / double.Parse(token)); //evaluate and push to val stack
                    }
                    else //otherwise push num to val stack
                    {
                        values.Push(double.Parse(token));
                    }
                }

                //handles TokenType [Var] as defined in PS2
                else if (tokenType.Equals(Var))
                {
                    try
                    {
                        if (OpPeek(operators, "*")) //check top of oper stack for multi oper
                        {
                            operators.Pop(); //remove oper from stack
                            values.Push(values.Pop() * lookup(token)); //evaluate experssion and push to val stack
                        }
                        else if (OpPeek(operators, "/")) //check top of oper stack for divide oper
                        {
                            if (values.Peek() == 0.0 || lookup(token) == 0.0) //check against division by zero
                            {
                                throw new FormulaEvaluationException("Cannot divide by zero, please revise");
                            }
                            operators.Pop(); //remove oper from top of oper stack
                            values.Push(values.Pop() / lookup(token)); //evaluate expression and push to val stack
                        }
                        else //push variable to val stack
                        {
                            values.Push(lookup(token));
                        }
                    }
                    catch //catch undefinedVariableException
                    {
                        throw new FormulaEvaluationException("Variables are non-double values");
                    }
                }

                //handles TokenType [Oper] as defined in PS2
                else if (tokenType.Equals(Oper))
                {
                    if (token.Equals("*") || token.Equals("/")) //check token string for mult or divide oper
                    {
                        operators.Push(token); //push oper to op stack
                    }
                    else //for + and - opers
                    {

                        if (OpPeek(operators, "+")) //check op stack for +
                        {
                            operators.Pop(); //remove top oper
                            values.Push(values.Pop() + values.Pop()); //eval and push to val stack
                        }
                        else if (OpPeek(operators, "-")) //check op stack for -
                        {
                            operators.Pop(); //remove top oper
                            double secondOperand = values.Pop(); //pull top num off val (both of these are necessary for correct evaluation with respect to original formula)
                            double firstOperand = values.Pop(); //pull second num off val
                            values.Push(firstOperand - secondOperand); //evaluate and push to val stack
                        }
                        operators.Push(token); //no matter what push the +/- oper to the oper stack
                    }
                }

                //handles TokenType [LParen] as defined in PS2
                else if (tokenType.Equals(LParen))
                {
                    operators.Push(token);
                }

                //handles TokenType [RParen} as defined in PS2
                else if (tokenType.Equals(RParen))
                {
                    //redundency of (req 3)
                    if (operators.Count == 0)
                    {
                        throw new FormulaFormatException("No opening paren");
                    }

                    if (OpPeek(operators, "+")) //check oper stack for +
                    {
                        operators.Pop(); //remove + from oper stack
                        values.Push(values.Pop() + values.Pop()); //evaluate expression and push to val stack
                    }
                    else if (OpPeek(operators, "-")) //check top of oper for -
                    {
                        operators.Pop(); //remove top oper
                        double secondOperand = values.Pop(); //pull top num off val (both of these are necessary for correct evaluation with respect to original formula)
                        double firstOperand = values.Pop(); //pull second num off val
                        values.Push(firstOperand - secondOperand); //evaluate and push to val stack
                    }

                    operators.Pop(); //no matter what remove ( from oper stack

                    if (OpPeek(operators, "*")) //check top of opers for *
                    {
                        operators.Pop(); //remove * from stack
                        values.Push(values.Pop() * values.Pop()); //evaluate and push to val stack
                    }
                    else if (OpPeek(operators, "/")) //check top of oper for /
                    {
                        if (values.Peek() == 0.0) //check against division by zero
                        {
                            throw new FormulaEvaluationException("Cannot divide by zero, please revise");
                        }
                        operators.Pop(); //otherwise remove / from op stack
                        values.Push(values.Pop() / values.Pop()); //evaluate division expression and push to stack
                    }
                }

                //handles TokenType [Invalid] as defined in PS2
                else if (tokenType.Equals(Invalid))
                {
                    throw new FormulaEvaluationException("Formula contains invalid sytax, please try again");
                }
            }

            //finish evaluating rest expression if there is still objects in oper stack
            if (operators.Count != 0)
            {
                if (operators.Peek().Equals("+")) //check top of oper stack for +
                {
                    operators.Pop(); //remove +
                    values.Push(values.Pop() + values.Pop()); //evaluate addition and push to val stack
                }
                else if (operators.Peek().Equals("-")) //check top of oper stack for -
                {
                    operators.Pop(); //same proceedure as above to get correct evaluation for subtraction expression
                    double secondOperand = values.Pop();
                    double firstOperand = values.Pop();
                    values.Push(firstOperand - secondOperand);
                }
            }

            return values.Pop(); //return only value left on val stack (if there is more than 1, something went terribly wrong)
        }

        /// <summary>
        /// Used to simplify peek command for operatiors stack. This is an ultra specific method for a Stack<double></double>
        /// </summary>
        /// <param name="stack"></param>
        /// <param name="oper"></param>
        /// <returns></returns>
        private static bool OpPeek(Stack<string> stack, string oper = "default")
        {
            bool returnVal = false; //pretty straight forward, if serious questions... please seek help

            if (stack.Count != 0)
            {
                if (stack.Peek().Equals(oper))
                {
                    returnVal = true;
                }

            }

            return returnVal;
        }


        /// <summary>
        /// Returns string version of normalized formula
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            //most likely (almost 100% uncessary) but caution gets A's
            if (this == null)
            {
                throw new ArgumentNullException("Null formula, please revise");
            }

            StringBuilder sb = new StringBuilder();
            foreach (Token t in tokenList)
            {
                sb.Append(t.Text);
            }

            return sb.ToString();
        }





        /// <summary>
        /// Given a formula, enumerates the tokens that compose it.  Each token is described by a
        /// Tuple containing the token's text and TokenType.  There are no empty tokens, and no
        /// token contains white space.
        /// </summary>
        private static IEnumerable<Token> GetTokens(String formula)
        {
            // Patterns for individual tokens.
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z][0-9a-zA-Z]*";

            // NOTE:  I have added white space to this regex to make it more readable.
            // When the regex is used, it is necessary to include a parameter that says
            // embedded white space should be ignored.  See below for an example of this.
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: e[\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall token pattern.  It contains embedded white space that must be ignored when
            // it is used.  See below for an example of this.
            String tokenPattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5}) | (.)",
                                            spacePattern, lpPattern, rpPattern, opPattern, varPattern, doublePattern);

            // Create a Regex for matching tokens.  Notice the second parameter to Split says 
            // to ignore embedded white space in the pattern.
            Regex r = new Regex(tokenPattern, RegexOptions.IgnorePatternWhitespace);

            // Look for the first match
            Match match = r.Match(formula);

            // Start enumerating tokens
            while (match.Success)
            {
                // Ignore spaces
                if (!match.Groups[1].Success)
                {
                    // Holds the token's type
                    TokenType type;

                    if (match.Groups[2].Success)
                    {
                        type = LParen;
                    }
                    else if (match.Groups[3].Success)
                    {
                        type = RParen;
                    }
                    else if (match.Groups[4].Success)
                    {
                        type = Oper;
                    }
                    else if (match.Groups[5].Success)
                    {
                        type = Var;
                    }
                    else if (match.Groups[6].Success)
                    {
                        type = Number;
                    }
                    else if (match.Groups[7].Success)
                    {
                        type = Invalid;
                    }
                    else
                    {
                        // We shouldn't get here
                        throw new InvalidOperationException("Regular exception failed in GetTokens");
                    }

                    // Yield the token
                    yield return new Token(match.Value, type);
                }

                // Look for the next match
                match = match.NextMatch();
            }
        }
    }


    /// <summary>
    /// Identifies the type of a token.
    /// </summary>
    public enum TokenType
    {
        /// <summary>
        /// Left parenthesis
        /// </summary>
        LParen,

        /// <summary>
        /// Right parenthesis
        /// </summary>
        RParen,

        /// <summary>
        /// Operator symbol
        /// </summary>
        Oper,

        /// <summary>
        /// Variable
        /// </summary>
        Var,

        /// <summary>
        /// Double literal
        /// </summary>
        Number,

        /// <summary>
        /// Invalid token
        /// </summary>
        Invalid
    };

    /// <summary>
    /// A Lookup method is one that maps some strings to double values.  Given a string,
    /// such a function can either return a double (meaning that the string maps to the
    /// double) or throw an UndefinedVariableException (meaning that the string is unmapped 
    /// to a value. Exactly how a Lookup method decides which strings map to doubles and which
    /// don't is up to the implementation of the method.
    /// </summary>
    public delegate double Lookup(string var);
    public delegate string Normalizer(string s);
    public delegate bool Validator(string s);

    /// <summary>
    /// Used to report that a Lookup delegate is unable to determine the value
    /// of a variable.
    /// </summary>
    [Serializable]
    public class UndefinedVariableException : Exception
    {
        /// <summary>
        /// Constructs an UndefinedVariableException containing whose message is the
        /// undefined variable.
        /// </summary>
        /// <param name="variable"></param>
        public UndefinedVariableException(String variable)
            : base(variable)
        {
        }
    }

    /// <summary>
    /// Used to report syntactic errors in the parameter to the Formula constructor.
    /// </summary>
    [Serializable]
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message) : base(message)
        {
        }
    }

    /// <summary>
    /// Used to report errors that occur when evaluating a Formula.
    /// </summary>
    [Serializable]
    public class FormulaEvaluationException : Exception
    {
        /// <summary>
        /// Constructs a FormulaEvaluationException containing the explanatory message.
        /// </summary>
        public FormulaEvaluationException(String message) : base(message)
        {
        }
    }

    /// <summary>
    /// Used to store the value of a token as Text and the TokenType as Type to save heap space
    /// </summary>
    public struct Token
    {
        public string Text;
        public TokenType Type;

        public Token(string text, TokenType type)
        {
            Text = text;
            Type = type;
        }
    }
}
