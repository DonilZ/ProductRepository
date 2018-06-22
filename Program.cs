using System;
using System.Collections.Generic;

namespace repository
{
    class Program
    {
        static void Main(string[] args)
        {
            DatabaseManager CurrentDatabaseManager = DatabaseManager.CreateDatabase();
         
            while (true) {        
                Console.WriteLine("Выберите действие:");
                Console.WriteLine("1: Получить информацию обо всех хранящихся в репозитории продуктах");
                Console.WriteLine("2: Получить информацию обо всех версиях выбранного программного продукта");
                Console.WriteLine("3: Получить полную информацию о выбранной версии выбранного программного продукта");
                Console.WriteLine("4: Добавить новую версию выбранного или нового программного продукта");
                Console.WriteLine("5: Обновить информацию о выбранной версии выбранного программного продукта");
                Console.WriteLine("6: Удалить информацию о выбранной версии выбранного программного продукта");
                Console.WriteLine("Любая другая клавиша: Выход из программы");

                string Command = Console.ReadLine();

                switch (Command) {
                    case "1":
                        UserShellGet.GetInformationProducts(CurrentDatabaseManager);
                        break;
                    case "2":
                        UserShellGet.GetInformationProductVersions(CurrentDatabaseManager);
                        break;
                    case "3":
                        UserShellGet.GetInformationProductConcreteVersion(CurrentDatabaseManager);
                        break;
                    case "4":
                        UserShell.AddVersion(CurrentDatabaseManager);
                        break;
                    case "5":
                        UserShell.UpdateVersion(CurrentDatabaseManager);
                        break;
                    case "6":
                        UserShell.RemoveVersion(CurrentDatabaseManager);
                        break;
                    default:
                        return;
                }
            }

            
            
        }
    }
}
