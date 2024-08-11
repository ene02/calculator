namespace Calculator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Write a classic math problem:");

            Console.WriteLine(Calculator.SolveComplex(Console.ReadLine()));
        }
    }
}
