using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    private const string Value = "1";
    static List<Person> people = new List<Person>(); // Справочник людей

    static void Main()
    {
        Console.WriteLine("Привет! Введите команду или нажмите Enter для выхода.");

        while (true)
        {
            string userInput = Console.ReadLine().Trim();

            if (string.IsNullOrEmpty(userInput))
            {
                Console.WriteLine("Выход из программы.");
                break;
            }

            switch (userInput.ToLower())
            {
                case "help":
                    Console.WriteLine("Доступные команды: help - показать справку, exit - выйти из программы, addpeople - добавить людей, showpeople - показать людей.");
                    break;
                case "exit":
                    Console.WriteLine("Выход из программы.");
                    return;
                case var command when command.StartsWith(Value):
                    HandleAddPeopleCommand(userInput);
                    break;
                case var command when command.StartsWith("2"):
                    HandleShowPeopleCommand(userInput);
                    break;
                default:
                    Console.WriteLine($"Неизвестная команда: {userInput}. Введите 'help' для справки.");
                    break;
            }
        }
    }

    // Команда для добавления людей в справочник
    static void HandleAddPeopleCommand(string command)
    {
        var parts = command.Split(' ');
        if (parts.Length == 4)
        {
            if (int.TryParse(parts[1], out int count) &&
                int.TryParse(parts[2], out int minAge) &&
                int.TryParse(parts[3], out int maxAge))
            {
                AddPeople(count, minAge, maxAge);
            }
            else
            {
                Console.WriteLine("Неверный формат команды.");
            }
        }
        else
        {
            Console.WriteLine("Команда должна содержать 3 параметра: количество людей, минимальный и максимальный возраст.");
        }
    }

    // Добавляем людей в справочник
    static void AddPeople(int count, int minAge, int maxAge)
    {
        var random = new Random();
        List<string> firstNames = new List<string> { "Иван", "Алексей", "Максим", "Дмитрий", "Артур" };
        List<string> lastNames = new List<string> { "Петров", "Сидоров", "Иванов", "Кузнецов", "Попов" };
        List<string> patronymics = new List<string> { "Иванович", "Алексеевич", "Максимович", "Дмитриевич", "Артурович" };
        List<string> genders = new List<string> { "Мужской", "Женский" };

        for (int i = 0; i < count; i++)
        {
            var firstName = firstNames[random.Next(firstNames.Count)];
            var lastName = lastNames[random.Next(lastNames.Count)];
            var patronymic = patronymics[random.Next(patronymics.Count)];
            var gender = genders[random.Next(genders.Count)];

            // Генерация случайной даты рождения в указанном диапазоне
            DateTime birthDate = GenerateRandomBirthDate(minAge, maxAge);

            people.Add(new Person
            {
                FirstName = firstName,
                LastName = lastName,
                Patronymic = patronymic,
                BirthDate = birthDate,
                Gender = gender
            });
        }

        Console.WriteLine($"Добавлено {count} человек, всего {people.Count} человек.");
    }

    // Генерация случайной даты рождения
    static DateTime GenerateRandomBirthDate(int minAge, int maxAge)
    {
        Random random = new Random();
        int currentYear = DateTime.Now.Year;
        int minYear = currentYear - maxAge;
        int maxYear = currentYear - minAge;
        int year = random.Next(minYear, maxYear);
        int month = random.Next(1, 13);
        int day = random.Next(1, DateTime.DaysInMonth(year, month) + 1);

        return new DateTime(year, month, day);
    }

    // Команда для вывода людей на экран
    static void HandleShowPeopleCommand(string command)
    {
        var parts = command.Split(' ');
        if (parts.Length == 2)
        {
            if (int.TryParse(parts[1], out int count) && count > 0)
            {
                ShowPeople(count);
            }
            else
            {
                Console.WriteLine("Неверный формат команды или число меньше 1.");
            }
        }
        else
        {
            Console.WriteLine("Команда должна содержать один параметр: количество людей для отображения.");
        }
    }

    // Вывод информации о людях
    static void ShowPeople(int count)
    {
        if (people.Count == 0)
        {
            Console.WriteLine("Справочник людей пуст.");
            return;
        }

        var peopleToShow = people.Take(count).ToList();

        for (int i = 0; i < peopleToShow.Count; i++)
        {
            var person = peopleToShow[i];
            Console.WriteLine($"{i + 1}. {person.LastName} {person.FirstName} {person.Patronymic}, {person.BirthDate.ToShortDateString()}, {person.Gender.ToLower()}");
        }
    }

    // Класс для человека
    class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; }

        public override string ToString()
        {
            return $"{LastName} {FirstName} {Patronymic}, {Gender}, Дата рождения: {BirthDate.ToShortDateString()}";
        }
    }
}
