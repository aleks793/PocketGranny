namespace ConsoleUI
{
    public class ExitCommand : ICommand
    {
        private readonly Application _app;

        public string Name => "exit";

        public string Help => "Выход из программы(листа покупок)";

        public string[] Synonyms => new string[] { "quit", "bye", "EXIT" };

        public string Description => "Длинное и подробное описание команды выхода";

        public ExitCommand(Application app)
        {
            _app = app;
        }

        public void Execute(params string[] parameters)
        {
            _app.Exit();
        }
    }
}