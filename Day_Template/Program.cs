using System.Text.RegularExpressions;

Regex numRegex = new(@"-?\d+");
Regex hexColorRegex = new(@"#[0-9a-z][0-9a-z][0-9a-z][0-9a-z][0-9a-z][0-9a-z]");

var singleDigitIntParser = new SingleLineStringInputParser<int>(int.TryParse, str => str.ToCharArray().Select(w => w.ToString()).ToArray());
var singleDigitIntInput = new InputProvider<int>("Input.txt", singleDigitIntParser.GetValue).ToList();

var wholeStringConvertInput = new InputProvider<string?>("Input.txt", GetString).Where(w => w != null).Cast<string>().ToList();
var wholeStringInput = new StringInputProvider("Input.txt");

var intPairInput = new InputProvider<(int, int)?>("Input.txt", GetIntPair).Where(w => w != null).Cast<(int, int)>().ToList();

var commaSeperatedSingleLineIntParser = new SingleLineStringInputParser<int>(int.TryParse, str => str.Split(",", StringSplitOptions.RemoveEmptyEntries));
var commaSeperatedSingleLineStringParser = new SingleLineStringInputParser<string>(StringInputProvider.GetString, str => str.Split(",", StringSplitOptions.RemoveEmptyEntries));
var commaSeperatedIntsInput = new InputProvider<int>("Input.txt", commaSeperatedSingleLineIntParser.GetValue).ToList();


static bool GetString(string? input, out string? value)
{
    value = null;

    if (input == null) return false;

    value = input ?? string.Empty;

    return true;
}

static bool GetIntPair(string? input, out (int, int)? value)
{
    value = null;

    if (input == null) return false;

    Regex numRegex = new(@"-?\d+");

    var numbers = numRegex.Matches(input).Select(w => int.Parse(w.Value)).ToList();

    if (numbers.Count != 2) throw new Exception();

    value = (numbers[0], numbers[1]);

    return true;
}