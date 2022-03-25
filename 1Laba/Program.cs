using _1Laba.VM;

namespace _1Laba
{
    class Program
    {
        private static string[] _positiveAnswers =
        {
            "Да",
            "да",
            "Yes",
            "yes",
            "1",
            "+"
        };

        private static string[] _negativeAnswers =
        {
            "Нет",
            "нет",
            "No",
            "no",
            "0",
            "-"
        };

        private static string[] _menu =
        {
            "-------------------------------",
            "             МЕНЮ",
            "-------------------------------",
            "1) Вывести все страницы",
            "2) Вывести одну страницу",
            "3) Вывести элемент страницы",
            "4) Записан ли элемент",
            "5) Записать элемент",
            "6) Удалить элемент",
            "0) Выйти"
        };

        private static Action<VirtualMemory>[] _menuActions =
        {
            Exit,
            PrintAll,
            PrintPage,
            PrintElem,
            Check,
            Set,
            Delete
        };

        static void Main(string[] args)
        {
            var filePath = AskString(new[] { "Введите имя файла виртуальной памяти" });

            Message(new[] { $"Настройки по умолчанию:\n-->Количесвто страниц: 3\n-->Количество элементов на странице: 10\n" });
            var useDefaultSettings = AskBool(new[] { "Использовать настройки по-умолчанию?" }, _positiveAnswers,
                _negativeAnswers);

            var overwrite = true;
            var pageCapacity = 10;
            var pageNumber = 3;

            if (!useDefaultSettings)
            {
                overwrite = AskBool(new[] { "Перезаписать файл виртуальной памяти, если он существует?" }, _positiveAnswers,
                    _negativeAnswers);
                pageCapacity = AskInt(new[] { "Введите количество элементов в странице (1 - 1000)" }, 1, 1000);
                pageNumber = AskInt(new[] { "Введите количество страниц (от 2)" }, 2);
            }

            var vm = new VirtualMemory(filePath, pageCapacity, pageNumber, overwrite);

            Menu(vm);
        }

        static void Menu(VirtualMemory vm)
        {
            int command;

            while (true)
            {
                command = AskInt(_menu, 0, 6);
                _menuActions[command].Invoke(vm);
            }
        }

        static void PrintAll(VirtualMemory vm)
        {
            vm.PrintAllPages();
        }

        static void PrintPage(VirtualMemory vm)
        {
            var pageNumber = AskInt(new[] { $"Введите номер страницы (0 - {vm.PageNumber - 1})" }, 0, vm.PageNumber - 1);
            vm.PrintPage(pageNumber);
        }

        static void PrintElem(VirtualMemory vm)
        {
            var pageNumber = AskInt(new[] { $"Введите номер страницы (0 - {vm.PageNumber - 1})" }, 0, vm.PageNumber - 1);
            var elemNumber = AskInt(new[] { $"Введите номер элемента (0 - {vm.PageCapacity - 1})" }, 0, vm.PageCapacity - 1);
            Message(new[] { $"Page #{pageNumber} Elem #{elemNumber}: {vm[pageNumber, elemNumber]}" });
        }

        static void Check(VirtualMemory vm)
        {
            var pageNumber = AskInt(new[] { $"Введите номер страницы (0 - {vm.PageNumber - 1})" }, 0, vm.PageNumber - 1);
            var elemNumber = AskInt(new[] { $"Введите номер элемента (0 - {vm.PageCapacity - 1})" }, 0, vm.PageCapacity - 1);
            Message(new[] { $"Page #{pageNumber} Elem #{elemNumber}: { (vm.IsEmpty(pageNumber, elemNumber) ? "EMPTY" : "NOT EMPTY") }" });
        }

        static void Set(VirtualMemory vm)
        {
            var pageNumber = AskInt(new[] { $"Введите номер страницы (0 - {vm.PageNumber - 1})" }, 0, vm.PageNumber - 1);
            var elemNumber = AskInt(new[] { $"Введите номер элемента (0 - {vm.PageCapacity - 1})" }, 0, vm.PageCapacity - 1);

            if (vm.IsEmpty(pageNumber, elemNumber))
            {
                var elemValue = AskInt(new[] { $"Введите значение элемента" });
                vm[pageNumber, elemNumber] = elemValue;
                Message(new[] { $"Page #{pageNumber} Elem #{elemNumber}: {vm[pageNumber, elemNumber]}" });
            }
            else
            {
                Message(new[] { $"Данная ячейка занята элементом {vm[pageNumber, elemNumber]}" });
                var answer = AskBool(new[] { "Перезаписать элемент?" }, _positiveAnswers, _negativeAnswers);

                if (answer)
                {
                    var elemValue = AskInt(new[] { $"Введите значение элемента" });
                    vm[pageNumber, elemNumber] = elemValue;
                    Message(new[] { $"Page #{pageNumber} Elem #{elemNumber}: {vm[pageNumber, elemNumber]}" });
                }

            }
        }

        static void Delete(VirtualMemory vm)
        {
            var pageNumber = AskInt(new[] { $"Введите номер страницы (0 - {vm.PageNumber - 1})" }, 0, vm.PageNumber - 1);
            var elemNumber = AskInt(new[] { $"Введите номер элемента (0 - {vm.PageCapacity - 1})" }, 0, vm.PageCapacity - 1);

            vm.Delete(pageNumber, elemNumber);

            Message(new[] { $"Page #{pageNumber} Elem #{elemNumber}: { (vm.IsEmpty(pageNumber, elemNumber) ? "EMPTY" : "NOT EMPTY") }" });
        }

        static void Exit(VirtualMemory vm)
        {
            Environment.Exit(1);
        }

        static int AskInt(IEnumerable<string> question, int minValue = Int32.MinValue, int maxValue = Int32.MaxValue, string commandMarker = "> ")
        {
            int? result = null;

            while (result == null)
            {
                Console.WriteLine(String.Join("\n", question));
                Console.Write(commandMarker);

                var commandText = Console.ReadLine();

                Console.Clear();

                try
                {
                    var tempResult = int.Parse(commandText);

                    if (tempResult > maxValue || tempResult < minValue)
                        throw new Exception();

                    result = tempResult;
                }
                catch
                {
                    Console.Write("Некорректный ввод. Нажмите Enter и введите корректное целое число.");
                    Console.ReadLine();
                    Console.Clear();
                }
            }

            return result.Value;
        }

        static string AskString(IEnumerable<string> question, bool allowEmptyString = false, string commandMarker = "> ")
        {
            string result = null;

            while (result == null || (result == "" && !allowEmptyString))
            {
                Console.WriteLine(String.Join("\n", question));
                Console.Write(commandMarker);

                result = Console.ReadLine();

                Console.Clear();
            }

            return result;
        }

        static bool AskBool(IEnumerable<string> question, IEnumerable<string> positiveAnswers, IEnumerable<string> negativeAnswers, string commandMarker = "> ")
        {
            bool? result = null;

            while (result == null)
            {
                Console.WriteLine(String.Join("\n", question));
                Console.WriteLine($"Positive: ({String.Join(", ", positiveAnswers)})");
                Console.WriteLine($"Negative: ({String.Join(", ", negativeAnswers)})");

                Console.Write(commandMarker);

                var answer = Console.ReadLine();

                Console.Clear();

                if (positiveAnswers.Contains(answer))
                {
                    result = true;
                }
                else if (negativeAnswers.Contains(answer))
                {
                    result = false;
                }
                else
                {
                    Console.Write("Invalid value...");
                    Console.Read();
                    Console.Clear();
                }
            }

            return result.Value;
        }

        static void Message(IEnumerable<string> message, string sep = "\n")
        {
            Console.WriteLine(String.Join(sep ?? "\n", message));
        }
    }
}