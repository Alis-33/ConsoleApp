using ConsoleApp;

internal class Program
{
    private static void Main(string[] args)
    {
        var connection = Database.GetConnection();

        while (true)
        {
            var choice = UIPrinter.Display();

            switch (choice)
            {
                default:
                    Console.Clear();
                    Cases.Run(connection, choice);
                    break;

                case 0:
                    Console.WriteLine("Goodbye!");
                    return;
            }
        }
    }
}