using System;
using ConsoleUI;

namespace PocketGranny.Commands.NecessaryProducts
{
    public class CommandsNecessaryProducts : ICommand
    {
        private Application _app;

        public CommandsNecessaryProducts(Application app)
        {
            _app = app;
        }

        public string Name => "np";

        public string Help => "Открыть список необходимых продуктов";

        public string Description => "";

        public string[] Synonyms => new[] { "NP" };

        public void Execute(params string[] parameters)
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("Открыт список необходимых продуктов");
            Console.ResetColor();
            Console.WriteLine(Line);

            _app.FindCommand("display").Execute();

            _app.Run(Console.In);

            _app.KeepRunning = true;
        }

        private const string Line = "================================================";
    }
}