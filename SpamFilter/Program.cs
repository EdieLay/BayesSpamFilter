
Dictionary<string, SpamHam> wordfreq = new();
Stemmer StemmerDelegate = new(stemmer1);

Thread filldictionary = new(FillDictionary); 
filldictionary.Start();


using (var reader = new StreamReader("../../../../email.txt", System.Text.Encoding.Default))
{
    string? line;
    string[] words;
    char[] separators = new char[] { ' ', ',', '.', '-', '(', ')', '/', ':', ';', '!', '?', '*', '"', '>', '<', '\'', '`' }; // массив разделителей
    
    line = await reader.ReadToEndAsync();
    if (line != null)
        words = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
    else throw new Exception(); // это можно будет сделать, как свой эксепшон, и не пропускать юзера дальше, пока не будет не пустого файла
    if (words == null)
        throw new Exception();

    for (int i = 0; i < words.Length; i++)
    {
        words[i] = StemmerDelegate(words[i]); // каждое слово прогоняем через стемминг
    }
    var stemmedwords = words.Distinct().ToList(); // убираем все повторения
    
}

filldictionary.Join(); // где-то это должно быть, но пока не знаю

double ProbOfSpam(string[] words_in_mes) // можно сделать сразу bool и в конце возвращать сравнение с 0.5 или другим числом
{
    double spam_prob = 0, ham_prob = 0; // вероятности, что сообщение спам или хэм
    foreach (var word in wordfreq)
    {
        if (words_in_mes.Contains(word.Key)) // если слово из словаря есть в сообщении, то прямое событие, иначе обратное
        {
            spam_prob += Math.Log(word.Value.spam_freq); // считаем сумму логарифмов вероятностей, т.к. это аналогично умножению вероятностей
            ham_prob += Math.Log(word.Value.ham_freq);   // потом просто экспоненту возведём в эту степень
        }
        else
        {
            spam_prob += Math.Log(1.0 - word.Value.spam_freq);
            ham_prob += Math.Log(1.0 - word.Value.ham_freq);
        }
    }
    double e_spam_prob = Math.Exp(spam_prob);
    double e_ham_prob = Math.Exp(ham_prob);

    return e_spam_prob / (e_spam_prob + e_ham_prob);
}

string stemmer1(string word) // условный стеммер
{
    return word;
}

void FillDictionary() // заполнение слов со стеммингом и их частот из файла
{
    using (var reader = new StreamReader("../../../../wordfreq.txt", System.Text.Encoding.Default))
    {
        string? line;
        string[] words;
        double spam, ham;

        while ((line = reader.ReadLine()) != null)
        {
            words = line.Split(';', StringSplitOptions.RemoveEmptyEntries);

            words[0] = StemmerDelegate(words[0]); // в делегате определённый стеммер
            spam = double.Parse(words[1]);
            ham = double.Parse(words[2]);

            if (!wordfreq.ContainsKey(words[0])) // если после стеммера слова нет в словаре, то создаём новый элемент, иначе прибавляем частоту
                wordfreq[words[0]] = new SpamHam(spam, ham);
            else
                wordfreq[words[0]].IncreaseFreq(spam, ham);
        }

    }
}

class SpamHam // класс для хранения условных вероятностей появления в спаме и хэме, учитывая разный стемминг
{
    public double spam_freq { get; private set; }
    public double ham_freq { get; private set; }

    public SpamHam(double spam_freq, double ham_freq)
    {
        this.spam_freq = spam_freq;
        this.ham_freq = ham_freq;
    }

    public void IncreaseFreq(double spam, double ham)
    {
        spam_freq += spam;
        ham_freq += ham;
    }
}


delegate string Stemmer(string value); // делегат, куда будут передаваться стеммеры




