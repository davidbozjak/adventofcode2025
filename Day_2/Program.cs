using System.Text.RegularExpressions;

var intervals = new InputProvider<List<ClosedInterval>?>("Input.txt", GetIntervals).Where(w => w != null)
    .SelectMany(w => w).ToList();

long sumPart1 = 0;
long sumPart2 = 0;

foreach (var interval in intervals)
{
    for (long number = interval.Start; number <= interval.End; number++)
    {
        var str = number.ToString();

        for (int i = 1; i <= str.Length / 2; i++)
        {
            if (str.Length % i != 0)
                continue;

            var factor = str.Length / i;

            var repeatingPart = str[0..i];

            var constructedStr = string.Join("", Enumerable.Repeat(repeatingPart, factor));

            if (str == constructedStr)
            {
                //Console.WriteLine($"Found: {number}");
                sumPart2 += number;
                break;
            }
        }

        if (str.Length % 2 != 0)
            continue;

        var part1 = str[0..(str.Length / 2)];
        var part2 = str[(str.Length / 2)..];

        if (part1 == part2)
        {
            //console.writeline($"found: {number}");
            sumPart1 += number;
        }
    }
}

Console.WriteLine($"Part 1: {sumPart1}");
Console.WriteLine($"Part 2: {sumPart2}");


static bool GetIntervals(string? input, out List<ClosedInterval>? value)
{
    value = null;

    if (input == null) return false;

    Regex numRegex = new(@"(\d+)-(\d+)");

    var matches = numRegex.Matches(input).Select(w => w.Value).ToList();

    value = new List<ClosedInterval>();

    foreach (var match in matches)
    {
        var parts = match.Split('-');

        if (parts.Length != 2) throw new Exception();

        value.Add(new ClosedInterval(long.Parse(parts[0]), long.Parse(parts[1])));
    }

    return true;
}