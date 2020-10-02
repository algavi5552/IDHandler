using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IDHandler
{
    public class WorkWithStrings
    {



        public List<string> detectedWords = new List<string>() { "id = ", "VALUES(", "VALUES (", "nomk_ls = " };    //лист строк, по которым парсим
        public List<string> ListWithGarbage = new List<string>();                                                   //лист на входе в программу,содержит мусор и данные
        public List<string> inputTextList = new List<string>();                                                     //входной параметр метода AppendRecords
        public List<string> handledRecords = new List<string>();                                                    //лист с обработанными строками
        public List<string> outStringsWithDoubles = new List<string>();                                             //лист с обработанными строками,но в нем есть дубли
        public List<string> ready4Output = new List<string>();                                                      //лист с готовыми в выводу строками
        public string resultString;                                                                                 //результат,возвращаемый методом GetWordFromSentence()
        public string shortstr;                                                                                     //строка для вывода в текстовый файл
        public int numOExceptions = 0;
        public int tableID;
        
        int endIndex0;
        int endIndex1;
        int endIndex2;
        int endIndex3;
        int endIndex;

        FileInfo fi = new FileInfo(@"outputt.txt");


        public void WritelnRecords(List<string> input)
        {
            foreach (string element in outStringsWithDoubles.Distinct())
            {
                ready4Output.Add(element);
            }
            using (StreamWriter sw = File.AppendText("outputt.txt"))
            {
                foreach (string ss in input)
                {
                    sw.Write(ss);
                }
            }
        }


                    /// <summary>
                    /// Блок вывода на консоль
                    /// </summary>
        internal void ConsoleOut()                                                                                  
        {
            Check (inputTextList);

            if (numOExceptions > 0)
            {
            Console.WriteLine("Некоторые данные не удалось распарсить");
            }
            Console.WriteLine("Число исходных  строк " + ListWithGarbage.Count );
            Console.WriteLine("Число строк для обработки " + inputTextList.Count);
            Console.WriteLine("Число исключений " + numOExceptions);
            Console.WriteLine("Размер выходного файла " + fi.Length / 1024 + " КБ");
            
            Console.ReadKey();
        }

        public List<string> CleanGarbageInList(List<string> ListWithGarbage) //из списка необработанных строк отсеиваем фильтром бесполезные для нас записи
        {                                                        
            foreach (string s in ListWithGarbage )
            {
                if (
                    (s.Contains("GO")   
                    || s.Contains("IF") 
                    || s.Contains("ALTER")      
                    || s.Contains("ADD")    
                    || s.Contains("NO ACTION")
                    || s.Contains("--") 
                    || s.Contains("NOEXEC") 
                    )==false
                   )
                    inputTextList.Add(s);
            }
            return inputTextList;
        }
        /// <summary>
        /// Вывод на консоль необработанных строк
        /// </summary>
        /// <param name="inputTextList"></param>
        private void Check(List<string> inputTextList)
        {
            foreach (string item in inputTextList.Except(handledRecords))
            {
                    Console.WriteLine(item);
            }
        }
          
        /// <summary>
        /// Переводим прочитанные строки из массива в лист, он нужен как вх. параметр в метод AppendRecords()
        /// </summary>
        /// <param name="inputText"></param>
        public List<string> ConvertArrayToList(string[] inputText)
        {
            foreach (string item in inputText)                                                     
            {
                ListWithGarbage.Add(item);
            }
            return ListWithGarbage;
        }
        /// <summary>
        /// Находит ID лекарства и имя таблицы в строках листа inputText, выделяет их и экспортирует в текстовый SQL-файл, необработанные записи возвращает в виде листа inputText для возможности повторной обработки таким же методом с другими ключевыми словами
        /// </summary>
        /// <param name="inputText"></param>
        /// <param name="idStartsWith"></param>
        /// <param name="dboStartsWith"></param>
        /// <returns></returns>
        public void AppendRecords(List<string> inputTextList, string idStartsWith, string dboStartsWith)
        {
            using (StreamWriter sw = File.AppendText("outputt.txt"))
            {
                foreach (string str in inputTextList)
                {
                    int indexStartID = str.IndexOf(idStartsWith);
                    int indexStartDBO = str.IndexOf(dboStartsWith);

                    try 
                    {
                        if (indexStartDBO > 0 && indexStartID > 0)                                                  //если в строке нашелся и id и имя таблицы, то берем ее в работу
                        {
                            string stringWithID = GetWordFromSentence(str, idStartsWith);
                            string tableName    = GetWordFromSentence(str, dboStartsWith);

                            tableID = GetTableIDbyName(tableName);                                                  //меняем имя таблицы на ее код из словаря
                            if ((stringWithID == "1") == false)
                            {
                                shortstr = "INSERT INTO [nomen].[dbo].[EN_SynTablesLog]" + "\n";
                                shortstr += "(WorkTableId, PositionId, EventDate, EventTypeId, HostName, SessionGuid, HostSynch)" + "\n";
                                shortstr += "VALUES(" + tableID + ", " + stringWithID + ", GETDATE(), 3, HOST_NAME(), NULL, HOST_NAME())" + "\n" + "\n";
                                //sw.Write(shortstr);
                                outStringsWithDoubles.Add(shortstr);
                                handledRecords.Add(str);
                            }
                        }
                    } 
                    catch
                    {
                        numOExceptions++;
                    }
                }
            }
        }

    Dictionary<string, int> TableNameAndID = new Dictionary<string, int>
        {
        ["ROZN_S_TN"]               = 3,
        ["ROZN_S_NOMK"]             = 4,
        ["ROZN_S_PREP"]             = 5,
        ["ROZN_S_MNN"]              = 6,
        ["ROZN_S_LF"]               = 7,
        ["ROZN_S_LFACT"]            = 8,
        ["ROZN_S_LFCHR"]            = 9,
        ["ROZN_S_LFCONC"]           = 10,
        ["ROZN_S_LFMASS"]           = 11,
        ["ROZN_S_LFSIZE"]           = 12,
        ["ROZN_S_P_USE"]            = 13,
        ["ROZN_S_TMC_PREP"]         = 14,
        ["PARUS_TMC"]               = 15,
        ["ROZN_S_VOL"]              = 16,
        ["ROZN_S_SET"]              = 17,
        ["ROZN_S_FIRMS"]            = 18,
        ["ROZN_S_CNTRS"]            = 19,
        ["ROZN_S_HAR"]              = 20,
        ["s_ed"]                    = 21,
        ["ROZN_S_SIGNS"]            = 22,
        ["ROZN_S_GROUP"]            = 23,
        ["ROZN_S_NOMK_SPEC_names"]  = 24,
        ["ROZN_S_FV_DOZ"]           = 25,
        ["MONITOR_LS"]              = 31,
        ["MZ_JNVLS_LS"]             = 32,
        ["rozn_sps_gr_mnn_lf"]      = 34,
        ["ROZN_S_GR"]               = 35,
        };

        /// <summary>
        /// Возвращает ID  таблицы из WorkTables по ее названию
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public int GetTableIDbyName(string tableName)
        {
            tableID = TableNameAndID[tableName];
            return tableID ;
        }

        /// <summary>
        /// Метод достает из строки слово, которое начинается с заданных символов
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="startsWith"></param>
        /// <returns></returns>
        public string GetWordFromSentence( string sentence, string startsWith)                                                                          
        {
            int inputWordLength = startsWith.Length;
            int firstIndex = sentence.IndexOf(startsWith) + inputWordLength;
            try
            {
               
            string StringStartsWithSearched = sentence.Substring(firstIndex);

            endIndex0 = StringStartsWithSearched.IndexOf(",");
            endIndex1 = StringStartsWithSearched.IndexOf(" ");
            endIndex2 = StringStartsWithSearched.IndexOf("(");
            endIndex3 = StringStartsWithSearched.Length ;
            endIndex = endIndex3;
            List<int> endOfWordIndex = new List<int> { endIndex0, endIndex1, endIndex2, endIndex3 };

                foreach (int index in endOfWordIndex)                                                   //сортировка индексов, ищем наименьший положительный, это и будет конец слова
                {
                    if (index > 0 && endIndex>index)
                    {
                        endIndex = index;
                    }
                }
                string shortWord = sentence.Substring(firstIndex, endIndex);
                resultString = shortWord;
            }
            catch (Exception ex)
            {
                Console.WriteLine("метод GetWordFromSentence отработал нештатно " + ex.Message);
                numOExceptions++;
            }
            
            return resultString;
        }
    }
}

//SET rcfi_name = CONVERT(varchar(43), 'Мочеприемник педиатрический 100 мл №100 
//' COLLATE SQL_Latin1_General_CP1251_CI_AS), sait_name = CONVERT(varchar(39), 'Мочеприемник педиатрический 100 мл №100' COLLATE SQL_Latin1_General_CP1251_CI_AS), vgr_kod = 20200320130728 WHERE nomk_ls = 192749 AND otdel_id = 1