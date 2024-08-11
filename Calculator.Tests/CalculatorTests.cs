using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calculator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calculator;

namespace Calculator.Tests
{
    [TestClass()]
    public class CalculatorTests
    {
        [TestMethod()]
        public void IsNumberStringTrueTest()
        {
            Assert.IsTrue(Calculator.IsNumber("69"));
            Assert.IsTrue(Calculator.IsNumber("69,9999"));
            Assert.IsTrue(Calculator.IsNumber("-69"));
            Assert.IsTrue(Calculator.IsNumber("-69,9999"));
        }

        [TestMethod()]
        public void IsNumberCharTrueTest()
        {
            Assert.IsTrue(Calculator.IsNumber('9'));
        }

        public void IsNumberStringFalseTest()
        {
            Assert.IsFalse(Calculator.IsNumber("W"));
        }

        [TestMethod()]
        public void IsNumberCharFalseTest()
        {
            Assert.IsFalse(Calculator.IsNumber('L'));
        }

        [TestMethod()]
        public void GetSymbolTypeDefaultCharsTest()
        {
            Assert.AreEqual(Calculator.Symbol.Plus, Calculator.GetSymbolType('+'));
            Assert.AreEqual(Calculator.Symbol.Minus, Calculator.GetSymbolType('-'));
            Assert.AreEqual(Calculator.Symbol.Multiplication, Calculator.GetSymbolType('*'));
            Assert.AreEqual(Calculator.Symbol.Division, Calculator.GetSymbolType('/'));
            Assert.AreEqual(Calculator.Symbol.OpenParenthesis, Calculator.GetSymbolType('('));
            Assert.AreEqual(Calculator.Symbol.CloseParenthesis, Calculator.GetSymbolType(')'));
            Assert.AreEqual(Calculator.Symbol.Comma, Calculator.GetSymbolType(','));
            Assert.AreEqual(Calculator.Symbol.Power, Calculator.GetSymbolType('^'));
            Assert.AreEqual(Calculator.Symbol.Root, Calculator.GetSymbolType('#'));
            Assert.AreEqual(Calculator.Symbol.Space, Calculator.GetSymbolType(' '));
        }

        [TestMethod()]
        public void GetSymbolTypeDefaultStringsTest()
        {
            Assert.AreEqual(Calculator.Symbol.Plus, Calculator.GetSymbolType("+"));
            Assert.AreEqual(Calculator.Symbol.Minus, Calculator.GetSymbolType("-"));
            Assert.AreEqual(Calculator.Symbol.Multiplication, Calculator.GetSymbolType("*"));
            Assert.AreEqual(Calculator.Symbol.Division, Calculator.GetSymbolType("/"));
            Assert.AreEqual(Calculator.Symbol.OpenParenthesis, Calculator.GetSymbolType("("));
            Assert.AreEqual(Calculator.Symbol.CloseParenthesis, Calculator.GetSymbolType(")"));
            Assert.AreEqual(Calculator.Symbol.Comma, Calculator.GetSymbolType(","));
            Assert.AreEqual(Calculator.Symbol.Power, Calculator.GetSymbolType("^"));
            Assert.AreEqual(Calculator.Symbol.Root, Calculator.GetSymbolType("#"));
            Assert.AreEqual(Calculator.Symbol.Space, Calculator.GetSymbolType(" "));
        }

        [TestMethod()]
        public void GetSymbolTypeInvalidCharTest()
        {
            Assert.AreEqual(Calculator.Symbol.Invalid, Calculator.GetSymbolType('L'));
        }

        [TestMethod()]
        public void GetSymbolTypeInvalidStringTest()
        {
            Assert.AreEqual(Calculator.Symbol.Invalid, Calculator.GetSymbolType("Nonsense"));
            Assert.AreEqual(Calculator.Symbol.Invalid, Calculator.GetSymbolType("     "));
        }

        [TestMethod()]
        public void SeparateTest()
        {
            string rawCalculation = "10,3+ 20-30*40/ (50^2,99999+(2#40,45) ) test";

            List<string> expectedSeparation = ["10,3", "+", "20", "-", "30", "*", "40", "/", "(", "50", "^", "2,99999", "+", "(", "2", "#", "40,45", ")", ")", "test"];

            List<string> actualSeparation = Calculator.Separate(rawCalculation);

            for (int i = 0; i < expectedSeparation.Count(); i++)
            {
                if (expectedSeparation[i] != actualSeparation[i])
                {
                    Assert.Fail();
                }
            }
        }

        [TestMethod()]
        public void IsSyntaxCorrectTrueTest()
        {
            string correctSyntaxOne = "10+20";
            string correctSyntaxTwo = "10+20-30";
            string correctSyntaxThree = "10+20-30*40";
            string correctSyntaxFour = "10+20-30*40/50";
            string correctSyntaxFive = "10+20-30*40/50^2";
            string correctSyntaxSix = "10+20-30*40/50^2+(2#40)";
            string correctSyntaxSeven = "10+20-30*40/(50^2+(2#40))";
            string correctSyntaxEight = "10+20-(30*40)/(50^2+(2#40))";
            string correctSyntaxNine = "(10+20)-(30*40)^2/(50^2+(2#40))#12";
            string correctSyntaxTen = "10+(30^(23*2)/2)-(2#40/2)*30/3/(1+1*2)*(-10)";

            Assert.IsTrue(Calculator.IsSyntaxCorrect(correctSyntaxOne));
            Assert.IsTrue(Calculator.IsSyntaxCorrect(correctSyntaxTwo));
            Assert.IsTrue(Calculator.IsSyntaxCorrect(correctSyntaxThree));
            Assert.IsTrue(Calculator.IsSyntaxCorrect(correctSyntaxFour));
            Assert.IsTrue(Calculator.IsSyntaxCorrect(correctSyntaxFive));
            Assert.IsTrue(Calculator.IsSyntaxCorrect(correctSyntaxSix));
            Assert.IsTrue(Calculator.IsSyntaxCorrect(correctSyntaxSeven));
            Assert.IsTrue(Calculator.IsSyntaxCorrect(correctSyntaxEight));
            Assert.IsTrue(Calculator.IsSyntaxCorrect(correctSyntaxNine));
            Assert.IsTrue(Calculator.IsSyntaxCorrect(correctSyntaxTen));
        }

        [TestMethod()]
        public void IsSyntaxCorrectFalseTest()
        {
            string badSyntaxOne = "10++";
            string badSyntaxTwo = "+10+";
            string badSyntaxThree = "10(+10)";
            string badSyntaxFour = "10*+10";
            string badSyntaxFive = "10++";
            string badSyntaxSix = "10+30*45/(23+(1)";
            string badSyntaxUnhinged = "((15+25)-(35*45)^3/(55^2+(3#60)#15+(20/5)^2*4#16)-(100#2+(30^3-(45#3)))^2/((7*9)+5^2*3)-(((6#36)+((8^3-2)^4#81)/(11+22))-((10^3-3)+(8^3-4)))/(18#81-((9#27)^2/3)+5^3)";

            Assert.IsFalse(Calculator.IsSyntaxCorrect(badSyntaxOne));
            Assert.IsFalse(Calculator.IsSyntaxCorrect(badSyntaxTwo));
            Assert.IsFalse(Calculator.IsSyntaxCorrect(badSyntaxThree));
            Assert.IsFalse(Calculator.IsSyntaxCorrect(badSyntaxFour));
            Assert.IsFalse(Calculator.IsSyntaxCorrect(badSyntaxFive));
            Assert.IsFalse(Calculator.IsSyntaxCorrect(badSyntaxSix));
            Assert.IsFalse(Calculator.IsSyntaxCorrect(badSyntaxUnhinged));
        }

        [TestMethod()]
        public void IsArimethicSymbolCharTrueTest()
        {
            Assert.IsTrue(Calculator.IsArimethicSymbol('+'));
            Assert.IsTrue(Calculator.IsArimethicSymbol('-'));
            Assert.IsTrue(Calculator.IsArimethicSymbol('*'));
            Assert.IsTrue(Calculator.IsArimethicSymbol('/'));
            Assert.IsTrue(Calculator.IsArimethicSymbol('^'));
            Assert.IsTrue(Calculator.IsArimethicSymbol('#'));
        }

        [TestMethod()]
        public void IsArimethicSymbolStringTrueTest()
        {
            Assert.IsTrue(Calculator.IsArimethicSymbol("+"));
            Assert.IsTrue(Calculator.IsArimethicSymbol("-"));
            Assert.IsTrue(Calculator.IsArimethicSymbol("*"));
            Assert.IsTrue(Calculator.IsArimethicSymbol("/"));
            Assert.IsTrue(Calculator.IsArimethicSymbol("^"));
            Assert.IsTrue(Calculator.IsArimethicSymbol("#"));
        }

        [TestMethod()]
        public void IsArimethicSymbolTrueTest()
        {
            Assert.IsTrue(Calculator.IsArimethicSymbol(Calculator.Symbol.Plus));
            Assert.IsTrue(Calculator.IsArimethicSymbol(Calculator.Symbol.Minus));
            Assert.IsTrue(Calculator.IsArimethicSymbol(Calculator.Symbol.Multiplication));
            Assert.IsTrue(Calculator.IsArimethicSymbol(Calculator.Symbol.Division));
            Assert.IsTrue(Calculator.IsArimethicSymbol(Calculator.Symbol.Power));
            Assert.IsTrue(Calculator.IsArimethicSymbol(Calculator.Symbol.Root));
        }
    }
}