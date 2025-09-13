using System;
using System.Linq;
using Bogus;
using Microsoft.EntityFrameworkCore;

namespace Cs_kt3
{
    class Program
    {
        static void Main()
        {
            InitializeDatabase();
            var users = GenerateUsers(10);
            DisplayGeneratedUsers(users);
            SaveUsersToDatabase(users);
            DisplayDatabaseUsers();

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        static void InitializeDatabase()
        {
            using (var context = new AppDbContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
        }

        static User[] GenerateUsers(int count)
        {
            var userFaker = new Faker<User>()
                .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                .RuleFor(u => u.LastName, f => f.Name.LastName())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.DateOfBirth, f => f.Date.Past(40));

            return userFaker.Generate(count).ToArray();
        }

        static void DisplayGeneratedUsers(User[] users)
        {
            Console.WriteLine("Сгенерированные пользователи:");
            foreach (var user in users)
            {
                Console.WriteLine($"{user.FirstName} {user.LastName}, {user.DateOfBirth:yyyy-MM-dd}, Возраст: {user.Age}");
            }
        }

        static void SaveUsersToDatabase(User[] users)
        {
            using (var context = new AppDbContext())
            {
                Console.WriteLine("\nСохранение в базу данных:");
                foreach (var user in users)
                {
                    var today = DateTime.Today;
                    var age = today.Year - user.DateOfBirth.Year;
                    if (user.DateOfBirth.Date > today.AddYears(-age)) age--;

                    if (age < 14)
                    {
                        Console.WriteLine($"Ошибка: {user.FirstName} {user.LastName} младше 14 лет. Регистрация запрещена.");
                        continue;
                    }

                    try
                    {
                        context.Users.Add(user);
                        context.SaveChanges();
                        Console.WriteLine($"Успешно сохранен: {user.FirstName} {user.LastName}");
                    }
                    catch (DbUpdateException ex)
                    {
                        Console.WriteLine($"Ошибка при сохранении {user.FirstName} {user.LastName}: {ex.InnerException?.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Общая ошибка при сохранении {user.FirstName} {user.LastName}: {ex.Message}");
                    }
                }
            }
        }

        static void DisplayDatabaseUsers()
        {
            using (var context = new AppDbContext())
            {
                Console.WriteLine("\nПользователи в базе данных:");
                var dbUsers = context.Users.ToList();

                if (!dbUsers.Any())
                {
                    Console.WriteLine("В базе данных нет пользователей.");
                    return;
                }

                foreach (var user in dbUsers)
                {
                    var today = DateTime.Today;
                    var age = today.Year - user.DateOfBirth.Year;
                    if (user.DateOfBirth.Date > today.AddYears(-age)) age--;

                    Console.WriteLine($"{user.Id}: {user.FirstName} {user.LastName}, {user.DateOfBirth:yyyy-MM-dd}, Возраст: {age}");
                }
            }
        }
    }
}