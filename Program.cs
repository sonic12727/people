using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    private const string Value = "1";
    static List<Person> people = new List<Person>(); // Справочник людей

    public static City? CityOfResidence { get; private set; }

    static void Main()
    {
        CityDatabase.Initialize(); // Инициализируем базу городов в начале

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
                    Console.WriteLine("Доступные команды: help - показать справку, exit - выйти из программы, 1 - добавить людей, 2 - показать людей, 3 - поиск по фамилии, 11 - список городов.");
                    break;
                case "exit":
                    Console.WriteLine("Выход из программы.");
                    return;
                case var command when command.StartsWith("1"):
                    if (command == "11")
                    {
                        CityDatabase.ListCities();
                    }
                    else if (command.StartsWith("12"))
                    {
                        HandleAddCityCommand(command);
                    }
                    else if (command.StartsWith("13"))
                    {
                        HandleRemoveCityCommand(command);
                    }
                    else if (command.StartsWith("14"))
                    {
                        HandleListCityResidentsCommand(command);
                    }
                    else
                    {
                        HandleAddPeopleCommand(userInput); // Остальные команды, начинающиеся с 1
                    }
                    break;
                case var command when command.StartsWith("2"):
                    HandleShowPeopleCommand(userInput);
                    break;
                case var command when command.StartsWith("3"):
                    HandleSearchPeopleCommand(userInput);
                    break;
                case "11": // Добавляю на всякий случай
                    CityDatabase.ListCities();
                    break;
                case "12":
                    HandleAddCityCommand("12 "); //Вызовите его с пробелом, чтобы при синтаксическом анализе не возникало ошибок
                    break;
                case "13":
                    HandleRemoveCityCommand("13 ");
                    break;
                case "14":
                    HandleListCityResidentsCommand("14 ");
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
                // Проверка, что минимальный возраст не больше максимального
                if (minAge > maxAge)
                {
                    Console.WriteLine("Ошибка: минимальный возраст не может быть больше максимального.");
                }
                else
                {
                    AddPeople(count, minAge, maxAge);
                }
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

    static void HandleAddCityCommand(string command)
    {
        // Разделите команду пробелами
        string[] parts = command.Split(' ');

        // Проверьте, достаточно ли параметров у команды
        if (parts.Length < 3)
        {
            Console.WriteLine("Неверный формат команды. Используйте: 12 <Название города> <Максимальное количество жителей>");
            return;
        }

        // Получите название города и максимальную численность населения из командных частей
        string cityName = parts[1];
        if (!int.TryParse(parts[2], out int maxPopulation))
        {
            Console.WriteLine("Неверный формат числа жителей. Введите целое число.");
            return;
        }

        // Добавьте город в базу данных
        CityDatabase.AddCity(cityName, maxPopulation);
    }

    static void HandleRemoveCityCommand(string command)
    {
        string[] parts = command.Split(' ');

        if (parts.Length < 2)
        {
            Console.WriteLine("Неверный формат команды. Используйте: 13 <Порядковый номер города>");
            return;
        }

        if (!int.TryParse(parts[1], out int cityId))
        {
            Console.WriteLine("Неверный формат порядкового номера города. Введите целое число.");
            return;
        }

        CityDatabase.RemoveCity(cityId);
    }

    static void HandleListCityResidentsCommand(string command)
    {
        string[] parts = command.Split(' ');

        if (parts.Length < 2)
        {
            Console.WriteLine("Неверный формат команды. Используйте: 14 <Порядковый номер города>");
            return;
        }

        if (!int.TryParse(parts[1], out int cityId))
        {
            Console.WriteLine("Неверный формат порядкового номера города. Введите целое число.");
            return;
        }

        City? city = CityDatabase.GetCityById(cityId);
        if (city == null)
        {
            Console.WriteLine($"Город с ID {cityId} не найден.");
            return;
        }

        Console.WriteLine($"Жители города {city.Name}:");
        bool residentsFound = false;
        foreach (Person person in people)
        {
            if (person.CityOfResidence?.Id == cityId)
            {
                Console.WriteLine(person);  // Use ToString() from Person class
                residentsFound = true;
            }
        }
        if (!residentsFound)
        {
            Console.WriteLine("Жители не найдены.");
        }
    }

    // Добавляем людей в справочник
    static void AddPeople(int count, int minAge, int maxAge)
    {
        var random = new Random();
        List<string> firstNamesMale = new List<string> { "Иван", "Алексей", "Максим", "Дмитрий", "Артур" };
        List<string> lastNamesMale = new List<string> { "Петров", "Сидоров", "Иванов", "Кузнецов", "Попов" };

        List<string> patronymicsMale = new List<string> { "Иванович", "Алексеевич", "Максимович", "Дмитриевич", "Артурович" };

        List<string> firstNamesFemale = new List<string> { "Анна", "Елена", "Ольга", "Татьяна", "Наталья" };
        List<string> lastNamesFemale = new List<string> { "Петрова", "Сидорова", "Иванова", "Кузнецова", "Попова" };  
        List<string> patronymicsFemale = new List<string> { "Ивановна", "Алексеевна", "Максимовна", "Дмитриевна", "Артуровна" };

        List<string> genders = new List<string> { "Мужской", "Женский" };


        for (int i = 0; i < count; i++)
        {
            string firstName, lastName, patronymic, gender;

            // Рандомно генерируется гендер
            if (random.Next(2) == 0) // 0 для Мужчин, 1 для Женщин
            {
                firstName = firstNamesMale[random.Next(firstNamesMale.Count)];
                lastName = lastNamesMale[random.Next(lastNamesMale.Count)];
                patronymic = patronymicsMale[random.Next(patronymicsMale.Count)];
                gender = "Мужской";
            }
            else
            {
                firstName = firstNamesFemale[random.Next(firstNamesFemale.Count)];
                lastName = lastNamesFemale[random.Next(lastNamesFemale.Count)]; // Используйте список женских фамилий
                // Убедитесь, что женские отчества используются в женском списке, если имена женские
                if (firstNamesFemale.Contains(firstName))
                {
                    patronymic = patronymicsFemale[random.Next(patronymicsFemale.Count)];
                }
                else
                {
                    patronymic = patronymicsMale[random.Next(patronymicsMale.Count)]; // По умолчанию используется мужское, если не женское имя
                }
                gender = "Женский";
            }

            // Генерация случайной даты рождения в указанном диапазоне
            DateTime birthDate = GenerateRandomBirthDate(minAge, maxAge);

            // Назначить город проживания - или НЕТ
            City cityOfResidence = null; // По умолчанию: нет города
            if (random.Next(2) == 0) // 50%-ная вероятность проживания в городе
            {
                cityOfResidence = CityDatabase.GetCities()[random.Next(CityDatabase.GetCities().Count)]; // Выберите случайный город
            }

            people.Add(new Person
            {
                FirstName = firstName,
                LastName = lastName,
                Patronymic = patronymic,
                BirthDate = birthDate,
                Gender = gender,
                CityOfResidence = cityOfResidence // Назначьте человеку город (или значение null)
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

    // Команда для поиска людей по фамилии
    static void HandleSearchPeopleCommand(string command)
    {
        var parts = command.Split(' ');
        if (parts.Length == 2)
        {
            string searchTerm = parts[1].ToLower();  // Приводим строку поиска к нижнему регистру
            SearchPeople(searchTerm);
        }
        else
        {
            Console.WriteLine("Команда должна содержать один параметр: часть фамилии для поиска.");
        }
    }

    // Поиск людей по фамилии
    static void SearchPeople(string searchTerm)
    {
        var foundPeople = people.Where(p => p.LastName.ToLower().Contains(searchTerm)).ToList();

        if (foundPeople.Any())
        {
            for (int i = 0; i < foundPeople.Count; i++)
            {
                var person = foundPeople[i];
                Console.WriteLine($"{i + 1}. {person.LastName} {person.FirstName} {person.Patronymic}, {person.BirthDate.ToShortDateString()}, {person.Gender.ToLower()}");
            }
        }
        else
        {
            Console.WriteLine("Не найдено людей с такой фамилией.");
        }
    }

    public static void RunSomeCode(string[] args)
    {
        CityDatabase.Initialize();

        // Пример использования базы данных
        Console.WriteLine("Города в базе данных:");
        foreach (var city in CityDatabase.GetCities())
        {
            Console.WriteLine($"ID: {city.Id}, Name: {city.Name}, Max Population: {city.MaxPopulation:N0}"); // :N0 форматирует число с помощью разделителей в тысячах
        }

        AddPeople(10, 20, 30); // Добавьте несколько человек

        foreach (var person in people)
        {
            string cityInfo = person.CityOfResidence != null ? person.CityOfResidence.Name : "Вне города";
            Console.WriteLine($"{person.FirstName} {person.LastName} {person.Patronymic}, {person.Gender}, {person.BirthDate.ToShortDateString()}, City: {cityInfo}");
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
        public City? CityOfResidence { get; set; } // Сейчас это можно обнулить

        public override string ToString()
        {
            string cityInfo = CityOfResidence != null ? CityOfResidence.Name : "Вне города";
            return $"{LastName} {FirstName} {Patronymic}, {Gender}, Дата рождения: {BirthDate.ToShortDateString()}";
        }
    }


    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MaxPopulation { get; set; }
        public int CurrentPopulation { get; set; } = 0;  // Инициализировать до 0

        public City(int id, string name, int maxPopulation)
        {
            Id = id;
            Name = name;
            MaxPopulation = maxPopulation;
        }

        public override string ToString()
        {
            return $"{Id}. {Name}, макс. число жителей - {MaxPopulation:N0}, текущее число жителей - {CurrentPopulation:N0}";
        }
    }

    public static class CityDatabase
    {
        private static List<City> cities = new List<City>();

        public static List<City> GetCities()
        {
            return cities;
        }

        public static void Initialize()
        {
            // Заранее определенное население города (приблизительное, в миллионах)
            // Они используются для определения максимального числа жителей. Измените их в соответствии с вашими предположениями относительно численности населения каждого города.
            Dictionary<string, double> cityPopulations = new Dictionary<string, double>
        {
            { "Москва", 13.0 },      // ~13 млн
            { "Санкт-Петербург", 5.6 }, // ~5.6 млн
            { "Новосибирск", 1.6 },  // ~1.6 млн
            { "Астана", 1.4 },        // ~1.4 млн
            { "Караганда", 0.5 }      // ~0.5 млн
        };

            int idCounter = 1; // Начните вводить идентификаторы городов с 1

            foreach (var cityData in cityPopulations)
            {
                string cityName = cityData.Key;
                double populationMillions = cityData.Value;
                int maxPopulation = (int)(populationMillions * 1000); // Конвертируйте миллионы в тысячи

                cities.Add(new City(idCounter++, cityName, maxPopulation));
            }
        }

        public static City GetCityById(int id)
        {
            return cities.FirstOrDefault(c => c.Id == id);
        }

        public static City GetCityByName(string name)
        {
            return cities.FirstOrDefault(c => c.Name == name);
        }

        public static void AddCity(string name, int maxPopulation)
        {
            if (GetCityByName(name) != null)
            {
                Console.WriteLine($"Город с таким названием '{name}' уже существует.");
                return;
            }

            int nextId = cities.Count > 0 ? cities.Max(c => c.Id) + 1 : 1; // Определите следующий доступный идентификатор

            cities.Add(new City(cities.Count + 1, name, maxPopulation));
            Console.WriteLine($"Город '{name}' добавлен.");
        }

        public static bool RemoveCity(int cityId)
        {
            City cityToRemove = cities.FirstOrDefault(c => c.Id == cityId);
            if (cityToRemove != null)
            {
                cities.Remove(cityToRemove);
                Console.WriteLine($"City with ID {cityId} removed.");
                return true;
            }
            else
            {
                Console.WriteLine($"City with ID {cityId} not found.");
                return false;
            }
        }

        public static void ListCities()
        {
            foreach (City city in cities)
            {
                Console.WriteLine(city);
            }
        }

    }
}
