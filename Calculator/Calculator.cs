using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Calculator
{
    public class Calculator
    {
        public double Resolve(string mathProblem)
        {
            string number = "";

            char pendingOperation = ' ';

            double anwser = 0;

            for (int i = 0; i < mathProblem.Count(); i++)
            {
                try
                {
                    // Try to convert char to a double.
                    Convert.ToDouble(mathProblem[i].ToString());
                }
                catch
                {
                    // If the char was not a number, then we can check if it was a math symbol.
                    if ((mathProblem[i] == '+' || mathProblem[i] == '-' || mathProblem[i] == '*' || mathProblem[i] == '/') && number != "")
                    {
                        // If it was, then solve last pending operation.
                        SolvePendingOperations();

                        // Save a new pending operation for next time.
                        pendingOperation = mathProblem[i];

                        // Empty for a new number to be added.
                        number = "";

                        // Skip so we dont add a symbol to the number string.
                        continue;
                    }

                    if (mathProblem[i] == '(' && number == "")
                    {
                        Debug.WriteLine("== '(' detected");

                        int currentIndex = i, endSkipsAllowed = 0;

                        string tempProblem = "";

                        bool wasSuccessful = false;

                        while (true)
                        {
                            currentIndex++;

                            if (currentIndex > mathProblem.Length - 1)
                            {
                                Debug.WriteLine("== End of line reached before problem was solved, ending loop");
                                break;
                            }

                            if (mathProblem[currentIndex] == '(')
                            {
                                Debug.WriteLine("== There is another '(' opened on this problem, skipping the next ')' to avoid errors");
                                endSkipsAllowed++;
                            }

                            if (mathProblem[currentIndex] == ')')
                            {
                                Debug.WriteLine("== ')' detected");
                                if (endSkipsAllowed == 0)
                                {
                                    wasSuccessful = true;

                                    Debug.WriteLine($"== Success parsing, last index is {currentIndex}");
                                    i = currentIndex;
                                    break;
                                }
                                else
                                {
                                    Debug.WriteLine("== Skipping one end");
                                    endSkipsAllowed--;
                                }
                            }

                            tempProblem += mathProblem[currentIndex];
                        }

                        if (wasSuccessful)
                        {
                            number = Resolve(tempProblem).ToString();
                            SolvePendingOperations();
                            Debug.WriteLine($"== Result from parenthesis was {number}");
                            continue;
                        }
                    }

                    if (mathProblem[i] != ',')
                    {
                        anwser = 0;
                        break;
                    }
                }

                // If everything went well and the char was successfully converted to a double, then that means the char is valid for future operations so we save it.
                number += mathProblem[i].ToString();

                // If its the last number, then we can finish any pending calculation.
                if (i == mathProblem.Count() - 1)
                {
                    SolvePendingOperations();
                }
            }

            // Updates the anwser depending on the pending operation.
            void SolvePendingOperations()
            {
                switch (pendingOperation)
                {
                    case '+':
                        anwser += Convert.ToDouble(number);
                        break;
                    case '-':
                        anwser -= Convert.ToDouble(number);
                        break;
                    case '*':
                        anwser *= Convert.ToDouble(number);
                        break;
                    case '/':
                        anwser /= Convert.ToDouble(number);
                        break;
                    case ' ':
                        anwser += Convert.ToDouble(number); // Empty char means this is the first numbers read so we just add it.
                        break;
                }
            }

            return anwser;
        }
    }
}
