using System;
using System.Collections.Generic;

namespace ConsoleUI
{
    public class ExplainCommand : ICommand
    {
        private readonly Application _app;

        public string Name => "explain";

        public string Help => "Рассказать о команде или командах";

        public string[] Synonyms => new string[] { "elaborate", "EXPLAIN" };

        public string Description
            => "Выводит всю доступную информацию по команде или командам. Параметр имя команды";

        public ExplainCommand(Application app)
        {
            _app = app;
        }

        public void Execute(params string[] parameters)
        {
            foreach (var a in parameters)
            {
                var cmd = _app.FindCommand(a);
                Console.WriteLine(Line);
                var synonyms = new List<string>(cmd.Synonyms);

                if (cmd.Name == a)
                {
                    Console.WriteLine($"{cmd.Name}: {cmd.Help}");
                }
                else
                {
                    Console.WriteLine($"{a}: {cmd.Help}");
                    synonyms.Remove(a);
                    synonyms.Add(cmd.Name);
                }

                if (synonyms.Count > 0)
                {
                    Console.WriteLine($"Синонимы: {string.Join(", ", synonyms)}");
                }

                if (cmd.Description == string.Empty) continue;

                Console.WriteLine(Line1);
                Console.WriteLine(cmd.Description);
            }

            Console.WriteLine(Line);
        }

        private const string Line = "================================================";

        private const string Line1 = "...............................................";
    }
}