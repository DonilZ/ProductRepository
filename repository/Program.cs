using System;
using System.Collections.Generic;
using System.Linq;

namespace repository
{
    class Program
    {
        static void Main(string[] args)
        {
            ILogger logger = new ConsoleLogger();
            ProductRepository currentProductRepository = ProductRepository.GetInstance(logger);

            UserShellGet currentUserShellGet = new UserShellGet(currentProductRepository);
            UserShell currentUserShell = new UserShell(currentProductRepository);
         
            while (true) {        
                Console.WriteLine("Выберите действие:");
                Console.WriteLine("1: Получить информацию обо всех хранящихся в репозитории продуктах");
                Console.WriteLine("2: Получить информацию обо всех версиях выбранного программного продукта");
                Console.WriteLine("3: Получить полную информацию о выбранной версии выбранного программного продукта");
                Console.WriteLine("4: Добавить новую версию выбранного или нового программного продукта");
                Console.WriteLine("5: Обновить информацию о выбранной версии выбранного программного продукта");
                Console.WriteLine("6: Удалить информацию о выбранной версии выбранного программного продукта");
                Console.WriteLine("Любая другая клавиша: Выход из программы");

                string command = Console.ReadLine();

                switch (command) {
                    case "1":
                        currentUserShellGet.GetProducts();
                        break;
                    case "2":
                        currentUserShellGet.GetProductVersions();
                        break;
                    case "3":
                        currentUserShellGet.GetProductConcreteVersion();
                        break;
                    case "4":
                        currentUserShell.AddVersion();
                        break;
                    case "5":
                        currentUserShell.UpdateVersion();
                        break;
                    case "6":
                        currentUserShell.RemoveVersion();
                        break;
                    default:
                        return;
                }
            }

            
            
        }
    }
}
