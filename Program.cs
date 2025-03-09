using System;
using System.Collections.Generic;
using System.Linq;
using static Program;

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
                    else if (command.StartsWith("15"))
                    {
                        HandleAddResidentsToCityCommand(command);
                    }
                    else
                    {
                        HandleAddPeopleCommand(userInput); // Остальные команды, начинающиеся с 1
                    }
                    break;
                case var command when command.StartsWith("2"):
                    if (command == "20")
                    {
                        HandleDistributeResidentsCommand();
                    }
                    else
                    {
                        HandleShowPeopleCommand(userInput);
                    }
                    break;
                case var command when command.StartsWith("3"):
                    HandleSearchPeopleCommand(userInput);
                    break;
                case var command when command.StartsWith("4"):
                    HandleAgeStatisticsCommand();
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
                case "15":
                    HandleAddResidentsToCityCommand("15 ");
                    break;
                case "20":
                    HandleDistributeResidentsCommand();
                    break;
                default:
                    Console.WriteLine($"Неизвестная команда: {userInput}. Введите 'help' для справки.");
                    break;
            }
        }
    }

    // Команда для добавления людей в справочник
    static void HandleAddPeopleCommand(string userInput)
    {
        string[] parts = userInput.Split(' ');

        if (parts.Length < 2)
        {
            Console.WriteLine("Неверный формат команды. Используйте: 1 [количество людей] [минимальный возраст] [максимальный возраст] [опционально: ID города]");
            return;
        }

        if (!int.TryParse(parts[1], out int count) || count <= 0)
        {
            Console.WriteLine("Неверный формат количества людей. Введите положительное целое число.");
            return;
        }

        if (!int.TryParse(parts.Length > 2 ? parts[2] : "18", out int minAge) || minAge < 0)
        {
            Console.WriteLine("Неверный формат минимального возраста. Введите неотрицательное целое число.");
            return;
        }

        if (!int.TryParse(parts.Length > 3 ? parts[3] : "60", out int maxAge) || maxAge < minAge)
        {
            Console.WriteLine("Неверный формат максимального возраста. Введите целое число, большее или равное минимальному возрасту.");
            return;
        }

        City? city = null;
        if (parts.Length > 4)
        {
            if (!int.TryParse(parts[4], out int cityId))
            {
                Console.WriteLine("Неверный формат ID города. Введите целое число.");
                return;
            }
            city = CityDatabase.GetCityById(cityId);
            if (city == null)
            {
                Console.WriteLine($"Город с ID {cityId} не найден.");
                return;
            }
            if (city.CurrentPopulation + count > city.MaxPopulation)
            {
                Console.WriteLine($"Невозможно добавить {count} жителей в город {city.Name}. Максимальное количество жителей будет превышено.");
                return;
            }
        }
        // Вызовите фактическую логику добавления людей с указанным городом или без него
        AddPeople(count, minAge, maxAge, city);
        Console.WriteLine($"Добавлено {count} человек" + (city != null ? $" в город {city.Name}" : ""));

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
                Console.WriteLine(person);  // Используйте toString() из класса Person
                residentsFound = true;
            }
        }
        if (!residentsFound)
        {
            Console.WriteLine("Жители не найдены.");
        }
    }

    // Добавляем людей в справочник
    static void AddPeople(int count, int minAge, int maxAge, City city)
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

            // Случайным образом генерируется пол
            if (random.Next(2) == 0) // 0 для мужчин, 1 для женщин
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


            // Сгенерируйте случайную дату рождения
            DateTime birthDate = GenerateRandomBirthDate(minAge, maxAge);

            Person person = new Person
            {
                FirstName = firstName,
                LastName = lastName,
                Patronymic = patronymic,
                BirthDate = birthDate,
                Gender = gender,
                CityOfResidence = city
            };
            people.Add(person);
            city.CurrentPopulation++;
        }

        Console.WriteLine($"Добавлено {count} человек в город {city.Name}, всего {people.Count} человек.");
    }

    // Генерация случайной даты рождения
    static DateTime GenerateRandomBirthDate(int minAge, int maxAge)
    {
        var random = new Random();
        int age = random.Next(minAge, maxAge + 1);
        DateTime now = DateTime.Now;
        DateTime startOfYear = new DateTime(now.Year - age, 1, 1);
        int range = (DateTime.Today - startOfYear).Days;
        return startOfYear.AddDays(random.Next(range));
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

    static void HandleDistributeResidentsCommand()
    {
        var random = new Random();
        List<City> availableCities = CityDatabase.GetCities();

        if (availableCities.Count == 0)
        {
            Console.WriteLine("Нет доступных городов для распределения жителей.");
            return;
        }

        // Сбросьте текущее население всех городов перед распределением
        CityDatabase.ResetCityPopulations();

        // Распределите каждого человека по городу случайным образом, учитывая максимальную численность населения
        foreach (Person person in people)
        {
            // Попробуйте присвоить этому человеку город, пока не будет найден тот, который не был переполнен
            int attempts = 0;
            while (attempts < availableCities.Count * 2)
            {

                City chosenCity = availableCities[random.Next(availableCities.Count)]; // Выберите город случайным образом
                if (chosenCity.CurrentPopulation < chosenCity.MaxPopulation)
                {
                    person.CityOfResidence = chosenCity;
                    chosenCity.CurrentPopulation++;
                    break; // Человек успешно назначен
                }
                attempts++;

            }
            if (person.CityOfResidence == null)
            {
                person.CityOfResidence = null;
            }
        }

        Console.WriteLine("Жители перераспределены между городами.");
        foreach (City city in availableCities)
        {
            Console.WriteLine(city);  // Перечислите города.
        }
    }

    static void HandleAddResidentsToCityCommand(string command)
    {
        string[] parts = command.Split(' ');

        if (parts.Length < 5)
        {
            Console.WriteLine("Неверный формат команды. Используйте: 15 <Количество жителей> <Минимальный возраст> <Максимальный возраст> <Порядковый номер города>");
            return;
        }

        if (!int.TryParse(parts[1], out int residentCount))
        {
            Console.WriteLine("Неверный формат количества жителей. Введите целое число.");
            return;
        }

        if (!int.TryParse(parts[2], out int minAge))
        {
            Console.WriteLine("Неверный формат минимального возраста. Введите целое число.");
            return;
        }

        if (!int.TryParse(parts[3], out int maxAge))
        {
            Console.WriteLine("Неверный формат максимального возраста. Введите целое число.");
            return;
        }

        if (!int.TryParse(parts[4], out int cityId))
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

        if (city.CurrentPopulation + residentCount > city.MaxPopulation)
        {
            Console.WriteLine($"Невозможно добавить {residentCount} жителей в город {city.Name}. Максимальное количество жителей будет превышено.");
            return;
        }

        // Добавьте жителей в город
        AddPeople(residentCount, minAge, maxAge, city);
        Console.WriteLine($"В город {city.Name} добавлено {residentCount} жителей.");
    }

    static void HandleAgeStatisticsCommand()
    {
        // Определите возрастные диапазоны
        List<(int minAge, int maxAge)> ageRanges = new List<(int, int)>
        {
            (10, 15),
            (16, 20),
            (21, 30),
            (31, 40),
            (41, 50),
            (51, 60),
            (61, 70),
            (71, 150)  // До разумного максимального возраста
        };

        Console.WriteLine("Статистика по возрасту:");

        foreach (var range in ageRanges)
        {
            int count = Program.people.Count(p => p.GetAge() >= range.minAge && p.GetAge() <= range.maxAge);
            Console.WriteLine($"От {range.minAge} до {range.maxAge} лет - {count} человек");
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
            Console.WriteLine($"ID: {city.Id}, Name: {city.Name}, Max Population: {city.MaxPopulation:N0}");
        }

        // Вызовите AddPeople, но укажите город по умолчанию (или null)
        AddPeople(10, 20, 30, null); // Изначально добавьте несколько человек *за пределами* какого-либо конкретного города

        foreach (var person in Program.people) // Используйте Program.people, а не локальную переменную
        {
            string cityInfo = person.CityOfResidence != null ? person.CityOfResidence.Name : "Вне города";
            Console.WriteLine($"{person.FirstName} {person.LastName} {person.Patronymic}, {person.Gender}, {person.BirthDate.ToShortDateString()}, City: {cityInfo}");
        }
    }

    // Класс для человека
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; }
        public City? CityOfResidence { get; set; }

        public int GetAge()
        {
            DateTime now = DateTime.Today;
            int age = now.Year - BirthDate.Year;
            if (BirthDate > now.AddYears(-age))
            {
                age--;
            }
            return age;
        }


        public override string ToString()
        {
            string cityInfo = CityOfResidence != null ? CityOfResidence.Name : "Вне города";
            return $"{FirstName} {LastName} {Patronymic}, {Gender}, {BirthDate.ToShortDateString()}, City: {cityInfo}";
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

        public static void ResetCityPopulations()
        {
            foreach (City city in cities)
            {
                city.CurrentPopulation = 0;
            }
        }

    }
}
