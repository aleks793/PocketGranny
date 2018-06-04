using System;
using ConsoleUI;

namespace PocketGranny.Commands.Recipes
{
    public class CommandsRecipes : ICommand
    {
        private Application _app;

        public CommandsRecipes(Application app)
        {
            _app = app;
        }

        public string Name => "recipes";

        public string Help => "Открыть список желаемых рецептов";

        public string Description => "";

        public string[] Synonyms => new[] { "RECIPES" };

        public void Execute(params string[] parameters)
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("Открыт список желаемых продуктов");
            Console.ResetColor();
            Console.WriteLine(Line);

            _app.FindCommand("display").Execute();

            _app.Run(Console.In);

            _app.KeepRunning = true;
        }

        private const string Line = "================================================";
    }
}