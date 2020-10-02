using System.IO;

namespace IDHandler
{
    class Program
    {  
        static void Main()
        {
            WorkWithStrings p = new WorkWithStrings();
            File.WriteAllText("outputt.txt", "");                   //чистим файл с результатами
            string[] inputText = File.ReadAllLines("inputt.txt");   //читаем строки из входного файла
            p.ConvertArrayToList(inputText);                        //массив строк вх. файла конвертируем в лист
            p.CleanGarbageInList(p.ListWithGarbage  );              //чистим из него строки с мусором
            
            foreach (string word in p.detectedWords)
            {
                p.AppendRecords(p.inputTextList, word, "dbo.");     //парсим данные по ключевым строкам
            }
            p.WritelnRecords(p.ready4Output);                    //записываем в файл обработанные строки
            p.ConsoleOut();                                         //выводим результаты на консоль
        }  
    }
}