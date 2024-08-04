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
            string numberBuffer = "";

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
                    if ((mathProblem[i] == '+' || mathProblem[i] == '-' || mathProblem[i] == '*' || mathProblem[i] == '/') && numberBuffer != "")
                    {
                        // If it was, then solve last pending operation.
                        SolvePendingOperations();

                        // Save a new pending operation for next time.
                        pendingOperation = mathProblem[i];

                        // Empty for a new number to be added.
                        numberBuffer = "";

                        // Skip so we dont add a symbol to the number string.
                        continue;
                    }

                    // If we encounter a parenthesis, then we expect the numberBuffer to be empty
                    // 10(10+10) <-- In this example numberBuffer is not cleared because we do not encounter a new symbol
                    // this is a syntax error and we cancel everything if this is encountered
                    if (mathProblem[i] == '(' && numberBuffer == "")
                    {
                        int currentIndex = i, endSkipsAllowed = 0;

                        string tempProblem = "";

                        bool wasSuccessful = false;

                        while (true)
                        {
                            currentIndex++;

                            // If we reached the end of the line and the loop is still running, then theres a ')' missing, we cancel everything obviously.
                            if (currentIndex > mathProblem.Length - 1)
                            {
                                break;
                            }

                            // Encountering a ')' would mean the parenthesis ended, but if the parenthesis has more parenthesis nested, then we would reach
                            // a false ending since we reached the ending of the nested parenthesis instead of the original one, this counter avoids that by counting
                            // the amount of opened nested parenthesis.
                            if (mathProblem[currentIndex] == '(')
                            {
                                endSkipsAllowed++;
                            }

                            if (mathProblem[currentIndex] == ')')
                            {
                                if (endSkipsAllowed == 0) // If there was no more nested parenthesis, then we can finally end the loop.
                                {
                                    wasSuccessful = true;
                                    i = currentIndex;
                                    break;
                                }
                                else // If the ending was from a nested parenthesis, then we keep going until reaching a new ending.
                                {
                                    endSkipsAllowed--;
                                }
                            }

                            // Problem string builder.
                            tempProblem += mathProblem[currentIndex];
                        }

                        if (wasSuccessful)
                        {
                            // If everything went alright, then we use recursion to send the problem to the parenthesis to the Resolve() method and save it.
                            numberBuffer = Resolve(tempProblem).ToString();

                            // Now that we have the solution, we can resume the calculations by just doing the pending operation with the anwser from the parenthesis problem.
                            SolvePendingOperations();
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
                numberBuffer += mathProblem[i].ToString();

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
                        anwser += Convert.ToDouble(numberBuffer);
                        break;
                    case '-':
                        anwser -= Convert.ToDouble(numberBuffer);
                        break;
                    case '*':
                        anwser *= Convert.ToDouble(numberBuffer);
                        break;
                    case '/':
                        anwser /= Convert.ToDouble(numberBuffer);
                        break;
                    case ' ':
                        anwser += Convert.ToDouble(numberBuffer); // Empty char means this is the first numbers read so we just add it.
                        break;
                }
            }

            return anwser;
        }
    }
}
