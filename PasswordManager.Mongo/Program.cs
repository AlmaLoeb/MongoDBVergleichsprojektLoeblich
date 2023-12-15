namespace PasswordManager.Mongo
{
    class Program
    {
        static void Main(string[] args)
        {
            var tester = new CrudPerformanceTester();
            tester.RunTests();
        }
    }
}