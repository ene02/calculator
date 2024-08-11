using System;
using System.Buffers;
using System.Diagnostics;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Calculator
{
    /// <summary>
    /// A silly calculator that can solve math problems froms strings.
    /// </summary>
    public class Calculator
    {
        public const char ADDITION_SYMBOL = '+';
        public const char SUBSTRACTION_SYMBOL = '-';
        public const char MULTIPLICATION_SYMBOL = '*';
        public const char DIVISION_SYMBOL = '/';
        public const char OPENPAR_SYMBOL = '(';
        public const char CLOSEPAR_SYMBOL = ')';
        public const char COMMA_SYMBOL = ',';
        public const char POWER_SYMBOL = '^';
        public const char ROOT_SYMBOL = '#'; // #???
        public const char SPACE = ' ';

        public const string STRING_ADDITION_SYMBOL = "+";
        public const string STRING_SUBSTRACTION_SYMBOL = "-";
        public const string STRING_MULTIPLICATION_SYMBOL = "*";
        public const string STRING_DIVISION_SYMBOL = "/";
        public const string STRING_OPENPAR_SYMBOL = "(";
        public const string STRING_CLOSEPAR_SYMBOL = ")";
        public const string STRING_COMMA_SYMBOL = ",";
        public const string STRING_POWER_SYMBOL = "^";
        public const string STRING_ROOT_SYMBOL = "#"; // #???
        public const string STRING_SPACE = " ";

        public const string SYNTAX_ERROR_MESSAGE = "Syntax error!";
        public const string DIVZERO_ERROR_MESSAGE = "Division by zero!";

        /// <summary>
        /// Enum for symbols, used for identification.
        /// </summary>
        public enum Symbol
        {
            Plus,
            Minus,
            Multiplication,
            Division,
            Power,
            Root,
            OpenParenthesis,
            CloseParenthesis,
            Comma,
            Space,
            /// <summary>
            /// Represents an invalid symbol not in the list.
            /// </summary>
            Invalid
        }

        /// <summary>
        /// Solves a complex calculation from a string.
        /// </summary>
        /// <param name="problem"></param>
        /// <returns> A string with the awnser, returns an error string in case one is encountered</returns>
        public static string SolveComplex(string complexMathProblem)
        {
            List<string> calculation = Separate(complexMathProblem);

            if (IsSyntaxCorrect(calculation))
            {
                return CalculateExpression(calculation);
            }
            else
            {
                return SYNTAX_ERROR_MESSAGE;
            }
        }

        /// <summary>
        /// Solves a complex calculation from a string.
        /// </summary>
        /// <param name="problem"></param>
        /// <returns> A double value with the awnser, returns null in case an error is encountered</returns>
        public static double? SolveComplexDouble(string complexMathProblem)
        {
            List<string> calculation = Separate(complexMathProblem);

            if (IsSyntaxCorrect(calculation))
            {
                return CalculateExpressionDouble(calculation);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Separates each number and symbol and saves them into a List.
        /// </summary>
        /// <param name="problem"></param>
        /// <returns>A List with all the numbers and symbols separated.</returns>
        public static List<string> Separate(string problem)
        {
            StringBuilder nonSymbol = new(); // For building strings that are not symbols.

            List<string> calculation = new(); // List to store separated numbers and symbols that we later return.

            int maxIndex = problem.Count() - 1; // Get maximum lenght index of the string

            for (int i = 0; i < problem.Count(); i++)
            {
                Symbol charType = GetSymbolType(problem[i]); // Get symbol type

                if (charType == Symbol.Space) // Ignore spaces.
                {
                    continue;
                }
                else if (charType != Symbol.Invalid && charType != Symbol.Comma)
                {
                    if (nonSymbol.ToString() != string.Empty) // If we encountered a symbol but we had a string stored, then saved it.
                    {
                        calculation.Add(nonSymbol.ToString());
                    }

                    calculation.Add(problem[i].ToString()); // Save the symbol encountered.

                    nonSymbol.Clear();
                }
                else
                {
                    nonSymbol.Append(problem[i]);

                    if (i == maxIndex)
                    {
                        calculation.Add(nonSymbol.ToString());
                    }
                }
            }

            return calculation;
        }

        /// <summary>
        /// Detects if the char is a valid arimethic symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns>True if is valid, false if its not</returns>
        public static bool IsArimethicSymbol(char symbol)
        {
            Symbol type = GetSymbolType(symbol);

            if (type != Symbol.Plus && type != Symbol.Minus && type != Symbol.Multiplication && type != Symbol.Division && type != Symbol.Power && type != Symbol.Root)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Detects if the string is a valid arimethic symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns>True if is valid, false if its not</returns>
        public static bool IsArimethicSymbol(string symbol)
        {
            Symbol type = GetSymbolType(symbol);

            if (type != Symbol.Plus && type != Symbol.Minus && type != Symbol.Multiplication && type != Symbol.Division && type != Symbol.Power && type != Symbol.Root)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Detects if the Symbol type is a valid arimethic symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns>True if is valid, false if its not</returns>
        public static bool IsArimethicSymbol(Symbol type)
        {
            if (type != Symbol.Plus && type != Symbol.Minus && type != Symbol.Multiplication && type != Symbol.Division && type != Symbol.Power && type != Symbol.Root)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Analizes the entire string expression for correct syntax.
        /// </summary>
        /// <param name="calculation"></param>
        /// <returns>True if all the syntax checks pass, false if something goes wrong</returns>
        public static bool IsSyntaxCorrect(string calculation)
        {
            List<string> separatedCalculation = Separate(calculation);

            int maxIndex = separatedCalculation.Count() - 1;

            for (int i = 0; i < separatedCalculation.Count(); i++)
            {
                Symbol type = GetSymbolType(separatedCalculation[i]); // Get symbol type, can be Invalid if its just a number.

                bool isNumber = IsNumber(separatedCalculation[i]); // Is it a number?

                string nextToLeft, nextToRight;

                if (IsArimethicSymbol(type))
                {
                    // Syntax rules for symbols:
                    // Need a number or a parenthesis to their sides, anything else is a syntax error
                    // Index 0 is ignored since one can use "-10" to use negative numbers, which would cause an error if checked.
                    // Maximum Index is insta error sonce a random symbol at the end of an expression makes no sense.
                    if (i != 0)
                    {
                        nextToLeft = separatedCalculation[i - 1];

                        if (!IsNumber(nextToLeft) && GetSymbolType(nextToLeft) != Symbol.CloseParenthesis && GetSymbolType(nextToLeft) != Symbol.OpenParenthesis)
                        {
                            return false;
                        }
                    }

                    if (i != maxIndex)
                    {
                        nextToRight = separatedCalculation[i + 1];

                        if (!IsNumber(nextToRight) && GetSymbolType(nextToRight) != Symbol.OpenParenthesis && GetSymbolType(nextToRight) != Symbol.CloseParenthesis)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (type == Symbol.OpenParenthesis)
                {
                    // Syntax rules for parenthesis:
                    // Can only have symbols or other parenthesis on their sides.
                    // An open parenthesis needs to have a closed parenthesis at some point of the expression.
                    // Index 0 is ignored becuase duh.
                    // Last index is an insta error if theres an open parenthesis, because an open parenthesis at the end doesnt make sense.
                    int openParenthesisCounter = 1;

                    if (i != 0)
                    {
                        nextToLeft = separatedCalculation[i - 1];

                        if (!IsArimethicSymbol(nextToLeft))
                        {
                            return false;
                        }
                    }

                    if (i == maxIndex)
                    {
                        return false;
                    }

                    for (int x = i + 1; x <= maxIndex; x++)
                    {
                        if (GetSymbolType(separatedCalculation[x]) == Symbol.CloseParenthesis)
                        {
                            if (openParenthesisCounter == 0)
                            {
                                break;
                            }
                            else
                            {
                                openParenthesisCounter--;
                            }
                        }
                        else if (GetSymbolType(separatedCalculation[x]) == Symbol.OpenParenthesis)
                        {
                            openParenthesisCounter++;
                        }
                    }

                    if (openParenthesisCounter != 0)
                    {
                        return false;
                    }
                }
                else if (type == Symbol.CloseParenthesis)
                {
                    // All parenthesis are checked when an open one is detected, so finding this is basically useless.
                    continue;
                }
                else if (isNumber)
                {
                    // Just a number, nothing to see here.
                    continue;
                }
                else
                {
                    // A string was not a symbol, nor a number, nor a parenthesis, so it is absolutely invalid.
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Analizes the entire list of the separated expression for correct syntax.
        /// </summary>
        /// <param name="calculation"></param>
        /// <returns>True if all the syntax checks pass, false if something goes wrong</returns>
        public static bool IsSyntaxCorrect(List<string> calculation)
        {
            int maxIndex = calculation.Count() - 1;

            for (int i = 0; i < calculation.Count(); i++)
            {
                Symbol type = GetSymbolType(calculation[i]);

                bool isNumber = IsNumber(calculation[i]);

                string nextToLeft, nextToRight;

                if (IsArimethicSymbol(type))
                {
                    if (i != 0)
                    {
                        nextToLeft = calculation[i - 1];

                        if (!IsNumber(nextToLeft) && GetSymbolType(nextToLeft) != Symbol.CloseParenthesis && GetSymbolType(nextToLeft) != Symbol.OpenParenthesis)
                        {
                            return false;
                        }
                    }

                    if (i != maxIndex)
                    {
                        nextToRight = calculation[i + 1];

                        if (!IsNumber(nextToRight) && GetSymbolType(nextToRight) != Symbol.OpenParenthesis && GetSymbolType(nextToRight) != Symbol.CloseParenthesis)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (type == Symbol.OpenParenthesis)
                {
                    int openParenthesisCounter = 1;

                    if (i != 0)
                    {
                        nextToLeft = calculation[i - 1];

                        if (!IsArimethicSymbol(nextToLeft))
                        {
                            return false;
                        }
                    }

                    if (i == maxIndex)
                    {
                        return false;
                    }

                    for (int x = i + 1; x <= maxIndex; x++)
                    {
                        if (GetSymbolType(calculation[x]) == Symbol.CloseParenthesis)
                        {
                            if (openParenthesisCounter == 0)
                            {
                                break;
                            }
                            else
                            {
                                openParenthesisCounter--;
                            }
                        }
                        else if (GetSymbolType(calculation[x]) == Symbol.OpenParenthesis)
                        {
                            openParenthesisCounter++;
                        }
                    }

                    if (openParenthesisCounter != 0)
                    {
                        return false;
                    }
                }
                else if (type == Symbol.CloseParenthesis)
                {
                    continue;
                }
                else if (isNumber)
                {
                    continue;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if the character is a valid symbol.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>The symbol type, returns Invalid in case none of the valid symbols match</returns>
        public static Symbol GetSymbolType(char value)
        {
            return value switch
            {
                ADDITION_SYMBOL => Symbol.Plus,
                SUBSTRACTION_SYMBOL => Symbol.Minus,
                MULTIPLICATION_SYMBOL => Symbol.Multiplication,
                DIVISION_SYMBOL => Symbol.Division,
                OPENPAR_SYMBOL => Symbol.OpenParenthesis,
                CLOSEPAR_SYMBOL => Symbol.CloseParenthesis,
                COMMA_SYMBOL => Symbol.Comma,
                POWER_SYMBOL => Symbol.Power,
                ROOT_SYMBOL => Symbol.Root,
                SPACE => Symbol.Space,
                _ => Symbol.Invalid,
            };
        }

        /// <summary>
        /// Checks if the string is a valid symbol.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>The symbol type, returns Invalid in case none of the valid symbols match</returns>
        public static Symbol GetSymbolType(string value)
        {
            return value switch
            {
                STRING_ADDITION_SYMBOL => Symbol.Plus,
                STRING_SUBSTRACTION_SYMBOL => Symbol.Minus,
                STRING_MULTIPLICATION_SYMBOL => Symbol.Multiplication,
                STRING_DIVISION_SYMBOL => Symbol.Division,
                STRING_OPENPAR_SYMBOL => Symbol.OpenParenthesis,
                STRING_CLOSEPAR_SYMBOL => Symbol.CloseParenthesis,
                STRING_COMMA_SYMBOL => Symbol.Comma,
                STRING_POWER_SYMBOL => Symbol.Power,
                STRING_ROOT_SYMBOL => Symbol.Root,
                STRING_SPACE => Symbol.Space,
                _ => Symbol.Invalid,
            };
        }

        /// <summary>
        /// Checks if the character is a double value number.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>True if is a number, false if not</returns>
        public static bool IsNumber(char value)
        {
            try
            {
                Convert.ToDouble(value.ToString());
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the string is a double value number.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>True if is a number, false if not</returns>
        public static bool IsNumber(string value)
        {
            try
            {
                Convert.ToDouble(value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Detects parenthesis and separates calculations accordingly, uses CalculateExpression() to solve stuff inside parenthesis.
        /// </summary>
        /// <param name="calculation"></param>
        private static void ParenthesisSolver(List<string> calculation)
        {
            int? indexToReplace = null; // The index of the string that will be replaced with the result of a parenthesis in case one is found.

            List<int> indexesToRemove = []; // The strings that have to be removed when a parenthesis has been solved.

            int openParenthesisCounter = 0;

            bool canSave = false, canCheck = true; // CanSave = Can save a calculation to the list, CanCheck = Can check if it can start savig a calculation.

            List<string> parenthesisCalculation = new(); // The calculation found in the parenthesis

            for (int i = 0; i < calculation.Count; i++)
            {
                if (canCheck && calculation[i] == STRING_OPENPAR_SYMBOL) // Did we find an open parenthesis?
                {
                    indexToReplace = i; // If yes, add that open parenthesis as the index where the result will replace it.
                    openParenthesisCounter++;
                    canCheck = false;
                }

                if (canSave)
                {
                    indexesToRemove.Add(i); // If we can save, then we can start saving all indexes of the calculation inside the parenthesis as indexes that we have to remove later when we found a result.

                    // Continue to save all values on the List in case we encounter another open parenthesis, we exit in case we encounter a closed parenthesis and there isnt one open anymore.
                    if (calculation[i] == STRING_OPENPAR_SYMBOL)
                    {
                        openParenthesisCounter++;
                    }
                    else if (calculation[i] == STRING_CLOSEPAR_SYMBOL)
                    {
                        openParenthesisCounter--;

                        if (openParenthesisCounter == 0)
                        {
                            break;
                        }
                    }

                    parenthesisCalculation.Add(calculation[i]);
                }

                if (indexToReplace != null) // If index to replace now has a valid value, then that means we are in a parenthesis, so we can start saving values to the List.
                {
                    canSave = true;
                }
            }

            if (indexToReplace != null)
            {
                calculation[(int)indexToReplace] = CalculateExpression(parenthesisCalculation); // Calculate stuff inside the parenthesis.
                calculation.RemoveRange((int)indexToReplace + 1, indexesToRemove.Count); // Remove old values.

                ParenthesisSolver(calculation); // Recheck in case another parenthesis was left hanging.
            }
        }

        /// <summary>
        /// Solves a complex calculation, this method directly attempts to solve something given and assumes the expression is in good syntax.
        /// </summary>
        /// <param name="calculation"></param>
        /// <returns>A string with the anwser.</returns>
        private static string CalculateExpression(List<string> calculation)
        {
            ParenthesisSolver(calculation);

            // Since numbers and symbols are parsed separated, a negative number can be read like this: "-" "10".
            // That can cause a fake syntax error, so in case we encounter a wild symbol with no number on the left, we combine the symbol and the number as one
            // So now it can be read as "-10".
            if (GetSymbolType(calculation[0]) == Symbol.Minus)
            {
                calculation[0] = string.Concat(calculation[0], calculation[1]);
                calculation.RemoveAt(1);
            }
            else if (GetSymbolType(calculation[0]) == Symbol.Plus) // Plus symbols dont make sense so we can just remove it.
            {
                calculation.RemoveAt(0);
            }

            // Removes  used values when doing a calculation, for example: 1+2 -> 3+2 (1 got replaceed by the result '3') -> 3 ('+2' is removed since it was part of the calc.)
            void RemoveUsedValues(int index)
            {
                calculation.RemoveAt(index + 1);
                calculation.RemoveAt(index);
            }

            List<int> rootsAndPowers = [];

            List<int> multiplicationsAndDivisions = [];

            List<int> additionsAndSubstractions = [];

            bool loop = true;

            do
            {
                rootsAndPowers.Clear();
                multiplicationsAndDivisions.Clear();
                additionsAndSubstractions.Clear();

                for (int i = 0; i < calculation.Count; i++)
                {
                    if (calculation[i] == STRING_ROOT_SYMBOL || calculation[i] == STRING_POWER_SYMBOL)
                    {
                        rootsAndPowers.Add(i);
                    }
                    else if (calculation[i] == STRING_MULTIPLICATION_SYMBOL || calculation[i] == STRING_DIVISION_SYMBOL)
                    {
                        multiplicationsAndDivisions.Add(i);
                    }
                    else if (calculation[i] == STRING_ADDITION_SYMBOL || calculation[i] == STRING_SUBSTRACTION_SYMBOL)
                    {
                        additionsAndSubstractions.Add(i);
                    }
                }

                if (rootsAndPowers.Count != 0)
                {
                    double num1 = Convert.ToDouble(calculation[rootsAndPowers[0] - 1]), num2 = Convert.ToDouble(calculation[rootsAndPowers[0] + 1]);

                    if (calculation[rootsAndPowers[0]] == STRING_POWER_SYMBOL)
                    {
                        calculation[rootsAndPowers[0] - 1] = $"{Math.Pow(num1, num2)}"; // Replace first number with result.
                        RemoveUsedValues(rootsAndPowers[0]); // Remove symbol and the other number we used for calculation.
                    }
                    else if (calculation[rootsAndPowers[0]] == STRING_ROOT_SYMBOL)
                    {
                        calculation[rootsAndPowers[0] - 1] = $"{Math.Pow(num2, 1.0 / num1)}";
                        RemoveUsedValues(rootsAndPowers[0]);
                    }
                }
                else if (multiplicationsAndDivisions.Count != 0)
                {
                    double num1 = Convert.ToDouble(calculation[multiplicationsAndDivisions[0] - 1]), num2 = Convert.ToDouble(calculation[multiplicationsAndDivisions[0] + 1]);

                    if (calculation[multiplicationsAndDivisions[0]] == STRING_MULTIPLICATION_SYMBOL)
                    {
                        calculation[multiplicationsAndDivisions[0] - 1] = $"{num1 * num2}";
                        RemoveUsedValues(multiplicationsAndDivisions[0]);
                    }
                    else if (calculation[multiplicationsAndDivisions[0]] == STRING_DIVISION_SYMBOL && num2 != 0)
                    {
                        calculation[multiplicationsAndDivisions[0] - 1] = $"{num1 / num2}";
                        RemoveUsedValues(multiplicationsAndDivisions[0]);
                    }
                    else
                    {
                        return DIVZERO_ERROR_MESSAGE;
                    }
                }
                else if (additionsAndSubstractions.Count != 0)
                {
                    double num1 = Convert.ToDouble(calculation[additionsAndSubstractions[0] - 1]), num2 = Convert.ToDouble(calculation[additionsAndSubstractions[0] + 1]);

                    if (calculation[additionsAndSubstractions[0]] == STRING_ADDITION_SYMBOL)
                    {
                        calculation[additionsAndSubstractions[0] - 1] = $"{num1 + num2}";
                        RemoveUsedValues(additionsAndSubstractions[0]);
                    }
                    else if (calculation[additionsAndSubstractions[0]] == STRING_SUBSTRACTION_SYMBOL)
                    {
                        calculation[additionsAndSubstractions[0] - 1] = $"{num1 - num2}";
                        RemoveUsedValues(additionsAndSubstractions[0]);
                    }
                }

                // For some reason i have to do this because the 'do while' doesnt cancel the loop and im too lazy to figure out why.
                if (rootsAndPowers.Count == 0 && multiplicationsAndDivisions.Count == 0 && additionsAndSubstractions.Count == 0)
                {
                    loop = false;
                }

            } while (loop);

            // StringBuilder sb = new(); foreach (var item in calculation) { sb.Append(item); } return sb.ToString();

            Debug.WriteLine($"|| Expresion solved: {calculation[0]}");
            return calculation[0];
        }

        /// <summary>
        /// Solves a complex calculation, this method directly attempts to solve something given and assumes the expression is in good syntax.
        /// </summary>
        /// <param name="calculation"></param>
        /// <returns>A string with the anwser.</returns>
        private static double? CalculateExpressionDouble(List<string> calculation)
        {
            ParenthesisSolver(calculation);

            void RemoveUsedValues(int index)
            {
                calculation.RemoveAt(index + 1);
                calculation.RemoveAt(index);
            }

            List<int> rootsAndPowers = [];

            List<int> multiplicationsAndDivisions = [];

            List<int> additionsAndSubstractions = [];

            bool loop = true;

            do
            {
                rootsAndPowers.Clear();
                multiplicationsAndDivisions.Clear();
                additionsAndSubstractions.Clear();

                for (int i = 0; i < calculation.Count; i++)
                {
                    if (calculation[i] == STRING_ROOT_SYMBOL || calculation[i] == STRING_POWER_SYMBOL)
                    {
                        rootsAndPowers.Add(i);
                    }
                    else if (calculation[i] == STRING_MULTIPLICATION_SYMBOL || calculation[i] == STRING_DIVISION_SYMBOL)
                    {
                        multiplicationsAndDivisions.Add(i);
                    }
                    else if (calculation[i] == STRING_ADDITION_SYMBOL || calculation[i] == STRING_SUBSTRACTION_SYMBOL)
                    {
                        additionsAndSubstractions.Add(i);
                    }
                }

                if (rootsAndPowers.Count != 0)
                {
                    double num1 = Convert.ToDouble(calculation[rootsAndPowers[0] - 1]), num2 = Convert.ToDouble(calculation[rootsAndPowers[0] + 1]);

                    if (calculation[rootsAndPowers[0]] == STRING_POWER_SYMBOL)
                    {
                        calculation[rootsAndPowers[0] - 1] = $"{Math.Pow(num1, num2)}"; // Replace first number with result.
                        RemoveUsedValues(rootsAndPowers[0]); // Remove symbol and the other number we used for calculation.
                    }
                    else if (calculation[rootsAndPowers[0]] == STRING_ROOT_SYMBOL)
                    {
                        calculation[rootsAndPowers[0] - 1] = $"{Math.Pow(num2, 1.0 / num1)}";
                        RemoveUsedValues(rootsAndPowers[0]);
                    }
                }
                else if (multiplicationsAndDivisions.Count != 0)
                {
                    double num1 = Convert.ToDouble(calculation[multiplicationsAndDivisions[0] - 1]), num2 = Convert.ToDouble(calculation[multiplicationsAndDivisions[0] + 1]);

                    if (calculation[multiplicationsAndDivisions[0]] == STRING_MULTIPLICATION_SYMBOL)
                    {
                        calculation[multiplicationsAndDivisions[0] - 1] = $"{num1 * num2}";
                        RemoveUsedValues(multiplicationsAndDivisions[0]);
                    }
                    else if (calculation[multiplicationsAndDivisions[0]] == STRING_DIVISION_SYMBOL && num2 != 0)
                    {
                        calculation[multiplicationsAndDivisions[0] - 1] = $"{num1 / num2}";
                        RemoveUsedValues(multiplicationsAndDivisions[0]);
                    }
                    else
                    {
                        return null;
                    }
                }
                else if (additionsAndSubstractions.Count != 0)
                {
                    double num1 = Convert.ToDouble(calculation[additionsAndSubstractions[0] - 1]), num2 = Convert.ToDouble(calculation[additionsAndSubstractions[0] + 1]);

                    if (calculation[additionsAndSubstractions[0]] == STRING_ADDITION_SYMBOL)
                    {
                        calculation[additionsAndSubstractions[0] - 1] = $"{num1 + num2}";
                        RemoveUsedValues(additionsAndSubstractions[0]);
                    }
                    else if (calculation[additionsAndSubstractions[0]] == STRING_SUBSTRACTION_SYMBOL)
                    {
                        calculation[additionsAndSubstractions[0] - 1] = $"{num1 - num2}";
                        RemoveUsedValues(additionsAndSubstractions[0]);
                    }
                }

                // For some reason i have to do this because the 'do while' doesnt cancel the loop and im too lazy to figure out why.
                if (rootsAndPowers.Count == 0 && multiplicationsAndDivisions.Count == 0 && additionsAndSubstractions.Count == 0)
                {
                    loop = false;
                }

            } while (loop);

            // StringBuilder sb = new(); foreach (var item in calculation) { sb.Append(item); } return sb.ToString();

            return Convert.ToDouble(calculation[0]);
        }
    }
}
