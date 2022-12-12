
Dictionary<string, SpamHamFreq> wordfreq = new Dictionary<string, SpamHamFreq>(); // словарь слов с их частотами появления в спаме и хаме

using (var reader = new StreamReader("../../../../spamdb.txt", System.Text.Encoding.Default)) // файл с письмами
{
    using (var writer = new StreamWriter("../../../../wordfreq.txt", false, System.Text.Encoding.Default)) // конечный файл со словами и их повторениями
    {
        string? line;
        string[] words;
        char[] separators = new char[] { ' ', ',', '.', '-', '(', ')', '/', ':', ';', '!', '?', '*', '"', '>', '<', '\'', '`' };
        bool isnotrussian = false; // для определения, является ли буква русской

        while ((line = await reader.ReadLineAsync()) != null) // считываем строчки, у которых начало либо "Мэри,...", либо "спам,..."
        { 
            if (line[0] == 'М') // если начинается на М, то это не спам
            {
                SpamHamFreq.isspam = false;
                SpamHamFreq.num_of_ham++;
            }
            else
            {
                SpamHamFreq.isspam = true;
                SpamHamFreq.num_of_spam++;
            }

            line = line.Remove(0, 5).ToLower(); // удаляем начало из 5 букв и приводим всё в нижний регистр
            words = line.Split(separators, StringSplitOptions.RemoveEmptyEntries); // разделяем строчку на слова, удаляя пустые строки

            foreach (string word in words) // перебираем слова в текущей строке
            {
                foreach (char letter in word)
                {
                    if (!(letter >= 'а' && letter <= 'я')) // оставляем только слова с русскими буквами
                    {
                        isnotrussian = true;
                        break;
                    }
                    isnotrussian = false;
                }
                if (isnotrussian)
                    continue;

                //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                // вот тут нужно провести стемминг слова !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                if (wordfreq.ContainsKey(word)) // если такое слово уже есть в словаре, то вызываем метод NewEntrance()
                {
                    wordfreq[word].NewEntrance();
                }
                else // если нет, то вызываем для него конструктор
                {
                    wordfreq[word] = new SpamHamFreq();
                    wordfreq[word].NewEntrance();
                }
            }
        }

        var select = wordfreq.OrderBy(a => a.Key);

        double spamfreq, hamfreq;
        foreach (var word in select) // считаем относитульную вероятность встречи слова для каждого типа сообщений
        {
            spamfreq = (((double)word.Value.met_in_spam + 1) / ((double)SpamHamFreq.num_of_spam + 2)); // добавляем 1 в числитель и 2 в знаменатель
            hamfreq = (((double)word.Value.met_in_ham + 1) / ((double)SpamHamFreq.num_of_ham + 2));    // чтобы не было нулевых вероятностей
            await writer.WriteLineAsync($"{word.Key};{spamfreq};{hamfreq}");
        }
    }
    
}

class SpamHamFreq
{
    public static bool isspam = true;
    public static int num_of_spam = 0;
    public static int num_of_ham = 0;
    public int met_in_spam { get; set; }
    public int met_in_ham { get; set; }

    public SpamHamFreq()
    {
        met_in_spam = 0;
        met_in_ham = 0;
    }

    public void NewEntrance()
    {
        if (isspam)
            met_in_spam++;
        else
            met_in_ham++;
    }
}

