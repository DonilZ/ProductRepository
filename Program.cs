using System;
using System.Collections.Generic;

namespace repository
{
    class Program
    {
        static void Main(string[] args)
        {
            Db_manager db_manager = Db_manager.create_db();
         
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
                        User_shell_get.get_all_products(ref db_manager);
                        break;
                    case "2":
                        User_shell_get.get_concrete_product_versions(ref db_manager);
                        break;
                    case "3":
                        User_shell_get.get_concrete_product_concrete_version(ref db_manager);
                        break;
                    case "4":
                        User_shell.add_version(ref db_manager);
                        break;
                    case "5":
                        User_shell.update_version(ref db_manager);
                        break;
                    case "6":
                        User_shell.delete_version(ref db_manager);
                        break;
                    default:
                        return;
                }
            }

            
            
        }
    }
}
