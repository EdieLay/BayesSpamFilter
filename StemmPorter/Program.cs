char[] VowelLetters = {
        'а', 'е', 'и', 'о', 'у', 'ы', 'э', 'ю', 'я'
};

string[] PerfectiveGerund = {
        "ав", "авши", "авшись", "яв", "явши", "явшись", "ив", "ивши", "ившись", "ыв", "ывши", "ывшись"
};

string[] Adjective = {
        "ее", "ие", "ые", "ое", "ими", "ыми", "ей", "ий", "ый", "ой", "ем", "им", "ым",
        "ом", "его", "ого", "ему", "ому", "их", "ых", "ую", "юю", "ая", "яя", "ою", "ею"
};

string[] Participle = {
        "аем", "анн", "авш", "ающ", "ащ", "яем", "янн", "явш", "яющ", "ящ", "ивш", "ывш", "ующ"
};

var select0 = Participle
    .Join(Adjective,
    p => 1,
    a => 1,
    (p, a) => new { p, a })
    .ToArray();

string[] Adjectival = new string[Adjective.Length + select0.Length + Participle.Length];
for (int i = 0; i < Adjective.Length; i++)
    Adjectival[i] = Adjective[i];
for (int i = 0; i < select0.Length; i++)
    Adjectival[Adjective.Length + i] = select0[i].p + select0[i].a;
for (int i = 0; i < Participle.Length; i++)
    Adjectival[Adjective.Length + select0.Length + i] = Participle[i];


string[] Reflexive = {
        "ся", "сь"
};

string[] Verb = {
         "ала", "ана", "аете", "айте", "али", "ай", "ал", "аем", "ан", "ало", "ано", "ает", "ают", "аны", "ать", "аешь", "анно",
         "яла", "яна", "яете", "яйте", "яли", "яй", "ял", "яем", "ян", "яло", "яно", "яет", "яют", "яны", "ять", "яешь", "янно",
         "ила", "ыла", "ена", "ейте", "уйте", "ите", "или", "ыли", "ей", "уй", "ил", "ыл", "им", "ым", "ен", "ило", "ыло", "ено",
         "ят", "ует", "уют", "ит", "ыт", "ены", "ить", "ыть", "ишь", "ую", "ю"
};

string[] Noun = {
        "а", "ев", "ов", "ие", "ье", "е", "иями", "ями", "ами", "еи", "ии", "и", "ией", "ей", "ой", "ий", "й", "иям",
        "ям", "ием", "ем", "ам", "ом", "о", "у", "ах", "иях", "ях", "ы", "ь", "ию", "ью", "ю", "ия", "ья", "я"
};

string[] Superlative = {
        "ейш", "ейше"
};

string[] Derivational = {
        "ост", "ость"
};

string s1 = "красивейшая";
Console.WriteLine(PorterStemmer(s1));

string PorterStemmer(string word)
{
    if (!GetRV(word, out string RV)) return word;
    int RVlength = RV.Length;

    Console.WriteLine(RV);

    if (!DeleteEnding(ref RV, PerfectiveGerund))            // 1 шаг
    {
        DeleteEnding(ref RV, Reflexive);
        if (!DeleteEnding(ref RV, Adjectival))
            if (!DeleteEnding(ref RV, Verb))
                if (!DeleteEnding(ref RV, Noun))
                {
                    Console.WriteLine($"RV {RV}");
                    return word[..(word.Length - RVlength + RV.Length)];
                }
    }
    if (RV.Length > 1 & RV[^1] == 'и') RV = RV[..^1];       // 2 шаг



    if (GetR1fromRV(RV, out string R1) & GetRV(R1, out string R2) & GetR1fromRV(R2, out R2)) // GetR1fromRV(R2, out R2) - область R1 после первого сочетания "гласная - согласная"
    {
        Console.WriteLine($"RV {RV}");
        Console.WriteLine($"R1 {R1}");
        Console.WriteLine($"R2 {R2}");
        int R2length = R2.Length;
        if (DeleteEnding(ref R2, Derivational))             // 3 шаг
            RV = RV[..(RV.Length - R2length + R2.Length)];
    }
    else return word[..(word.Length - RVlength + RV.Length)];

    Console.WriteLine($"идем на 4 шаг");

    if (RV.Length > 2 & RV[..^2] == "нн") RV = RV[..^1];    // 4 шаг
    DeleteEnding(ref RV, Superlative);
    if (RV.Length > 2 & RV[..^2] == "нн") RV = RV[..^1];
    if (RV.Length > 1 & RV[^1] == 'ь') RV = RV[..^1];
    return word[..(word.Length - RVlength + RV.Length)];
}

bool GetRV(string word, out string RV) // область слова после первой гласной
{
    int ind = 0;
    bool flagVowelLetter = false;
    while (ind < word.Length)
    {
        for (int j = 0; j < VowelLetters.Length; j++)
        {
            if (word[ind] == VowelLetters[j])
            {
                flagVowelLetter = true;
                break;
            }
        }

        if (flagVowelLetter) break;
        else ind++;
    }
    if (++ind < word.Length)
    {
        RV = word[ind..];
        return true;
    }
    RV = word;
    return false;
}

bool GetR1fromRV(string RV, out string R1) // область слова после первого сочетания "гласная-согласная"
{
    int ind = 0;
    bool flagVowelLetter = false;
    while (ind < RV.Length)
    {
        for (int j = 0; j < VowelLetters.Length; j++)
        {
            if (RV[ind] == VowelLetters[j])
            {
                flagVowelLetter = true;
                break;
            }
        }
        if (flagVowelLetter)
        {
            ind++;
            flagVowelLetter = false;
        }
        else break;
    }
    if (++ind < RV.Length)
    {
        R1 = RV[ind..];
        return true;
    }
    R1 = RV;
    return false;
}

bool DeleteEnding(ref string RV, string[] ending)
{
    int max = 0;
    for (int i = 0; i < ending.Length; i++) // итерации по массиву окончаний
    {
        if (ending[i].Length < RV.Length)
        {
            bool flagEqually = true;
            for (int j = ending[i].Length - 1; j >= 0; j--) // итерации по концу слова
                if (ending[i][j] != RV[RV.Length - ending[i].Length + j])
                {
                    flagEqually = false;
                    break;
                }
            if (flagEqually & ending[i].Length > max) max = ending[i].Length;
        }
    }
    RV = RV[..^max];
    Console.WriteLine($"DeleteEnding {ending[0]}: max {max} | RV[^max..] {RV}");

    if (max > 0) return true; return false;
}