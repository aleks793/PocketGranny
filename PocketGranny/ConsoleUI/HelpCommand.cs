using System;

namespace ConsoleUI
{
    public class HelpCommand : ICommand
    {
        private readonly Application _app;

        public string Name => "help";

        public string Help => "Краткая помощь по всем командам";

        public string[] Synonyms => new string[] { "?", "HELP" };

        public string Description => "Выводит список  команд с краткой помощью";

        public HelpCommand(Application app)
        {
            _app = app;
        }

        public void Execute(params string[] parameters)
        {
            Console.WriteLine(Line);

            foreach (var cmd in _app.Commands)
            {
                Console.WriteLine($"{cmd.Name}: {cmd.Help}");
            }

            Console.WriteLine(Line);
        }

        private const string Line = "================================================";
    }
}