namespace Calculator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Write a classic math problem:");

            Calculator calc = new();

            string? problem = Console.ReadLine();

            if (problem != null)
            {
                Console.WriteLine($"{calc.Resolve(problem)}");
            }
            else
            {
                Console.WriteLine("Nothing to calculate, see ya");
            }
        }
    }
}
