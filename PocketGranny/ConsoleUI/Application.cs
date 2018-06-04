using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleUI
{
    public class Application
    {
        private bool _keepRunning = true;

        NotFoundCommand notFound = new NotFoundCommand();
        List<ICommand> commands = new List<ICommand>();
        Dictionary<string, ICommand> commandMap = new Dictionary<string, ICommand>();

        public IList<ICommand> Commands
        {
            get
            {
                return commands;
            }
        }

        public bool KeepRunning
        {
            get => _keepRunning;
            set => _keepRunning = value;
        }

        public void Exit()
        {
            _keepRunning = false;
        }

        public ICommand FindCommand(string name)
        {
            if (commandMap.ContainsKey(name))
            {
                return commandMap[name];
            }

            notFound.Name = name;
            return notFound;
        }
        
        public void AddCommand(ICommand cmd)
        {
            commands.Add(cmd);

            if (commandMap.ContainsKey(cmd.Name))
            {
                throw new Exception(String.Format($"Команда {cmd.Name} уже добавлена"));
            }

            commandMap.Add(cmd.Name, cmd);

            foreach (var s in cmd.Synonyms)
            {
                if (commandMap.ContainsKey(s))
                {
                    Console.WriteLine($"ERROR: Игнорирую синоним {s} для команды {cmd.Name}, поскольку имя {s}  уже использовано");
                    continue;
                }

                commandMap.Add(s, cmd);
            }
        }

        public void Run(TextReader reader)
        {
            string[] cmdline, parameters;

            while (_keepRunning)
            {
                Console.Write("> ");
                var cmd = reader.ReadLine();

                if (cmd == null)
                {
                    break;
                }

                cmdline = cmd.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                if (cmdline.Length == 0)
                {
                    continue;
                }

                parameters = new string[cmdline.Length - 1];
                Array.Copy(cmdline, 1, parameters, 0, cmdline.Length - 1);
                FindCommand(cmdline[0]).Execute(parameters);
            }
        }
    }
}