class FileDeleter
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.InputEncoding = System.Text.Encoding.UTF8;

        // 1. Получаем путь к папке от пользователя
        Console.WriteLine("Введите полный путь к фалу со списком:");
        string configFilePath = Console.ReadLine();
        // --- НАСТРОЙКА ---
        // Путь к файлу, в котором перечислены имена файлов для удаления.
        configFilePath = configFilePath+ "\\deleteFiles.txt";

        // Путь к папке, в которой нужно искать и удалять файлы.
        Console.WriteLine("Введите полный путь к папке с изображениямии:");
        string targetDirectoryPath = Console.ReadLine();
        // -----------------


        Console.WriteLine("Запуск программы удаления файлов...");

        // 1. Читаем список файлов для удаления из конфигурационного файла
        HashSet<string> filesToDelete;
        try
        {
            // Используем ReadAllLines для простоты, он считывает все строки в массив.
            // Затем создаем HashSet для быстрого поиска (O(1) в среднем).
            var lines = File.ReadAllLines(configFilePath);
            filesToDelete = new HashSet<string>(lines.Where(line => !string.IsNullOrWhiteSpace(line)));

            if (filesToDelete.Count == 0)
            {
                Console.WriteLine("Файл конфигурации пуст или не содержит имен файлов. Работа завершена.");
                return;
            }
            Console.WriteLine($"Загружен список из {filesToDelete.Count} файлов для удаления.");
        }
        catch (FileNotFoundException)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Ошибка: Файл конфигурации не найден по пути: {Path.GetFullPath(configFilePath)}");
            Console.ResetColor();
            Console.WriteLine("Нажмите любую клавишу для выхода.");
            Console.ReadKey();
            return;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Произошла ошибка при чтении файла конфигурации: {ex.Message}");
            Console.ResetColor();
            Console.WriteLine("Нажмите любую клавишу для выхода.");
            Console.ReadKey();
            return;
        }

        // 2. Проверяем, существует ли целевая папка
        if (!Directory.Exists(targetDirectoryPath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Ошибка: Целевая папка не найдена по пути: {targetDirectoryPath}");
            Console.ResetColor();
            Console.WriteLine("Нажмите любую клавишу для выхода.");
            Console.ReadKey();
            return;
        }

        Console.WriteLine($"Сканирование папки: {targetDirectoryPath}");

        // 3. Получаем все файлы в папке и пытаемся их удалить
        int deletedCount = 0;
        // Directory.EnumerateFiles более эффективен для больших папок, чем GetFiles
        foreach (string filePath in Directory.EnumerateFiles(targetDirectoryPath))
        {
            // Получаем только имя файла из полного пути
            string fileName = Path.GetFileNameWithoutExtension(Path.GetFileName(filePath));
            // Проверяем, есть ли это имя в нашем списке на удаление
            if (filesToDelete.Contains(fileName))
            {
                try
                {
                    File.Delete(filePath);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Удален файл: {filePath}");
                    Console.ResetColor();
                    deletedCount++;
                }
                catch (Exception ex)
                {
                    // Обрабатываем ошибки, если файл занят другим процессом или нет прав на удаление
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Не удалось удалить файл {filePath}. Причина: {ex.Message}");
                    Console.ResetColor();
                }
            }
        }

        // 4. Выводим итоговый результат
        Console.WriteLine("---------------------------------");
        if (deletedCount == 0)
        {
            Console.WriteLine("Ни одного файла для удаления не найдено в целевой папке.");
        }
        else
        {
            Console.WriteLine($"Процесс завершен. Удалено файлов: {deletedCount}.");
        }

        Console.WriteLine("Нажмите любую клавишу для выхода.");
        Console.ReadKey();
    }
}