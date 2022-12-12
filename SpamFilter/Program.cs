
string[] words_freq;

using (var reader = new StreamReader("../../../../wordfreq.txt", System.Text.Encoding.Default))
{
    string? line;
    string[] words;
    char[] separators = new char[] { ';', '\n' };

    line = await reader.ReadToEndAsync();
    words_freq = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
}


/*using (var reader = new StreamReader("email.txt", System.Text.Encoding.Default))
{
    string? line;
    string[] words;
    char[] separators = new char[] { ' ', ',', '.', '-', '(', ')', '/', ':', ';', '!', '?', '*', '"', '>', '<', '\'', '`' };

    line = await reader.ReadToEndAsync();
    if (line != null)
        words = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
    else throw new Exception(); // это можно будет сделать, как свой эксепшон, и не пропускать юзера дальше, пока не будет не пустого файла
    


}*/

void Filter(Stemmer stemmer, string[] words_in_mes)
{
    
}




delegate string Stemmer(string value);




