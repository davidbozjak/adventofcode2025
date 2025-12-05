using System.Text.RegularExpressions;

var qToVerify = new StringInputProvider("Input_Q.txt").Where(w => w != null).Select(long.Parse).ToList();

var intervals = new InputProvider<ClosedInterval?>("Input_Fresh.txt", GetIntervals).Where(w => w != null).Cast<ClosedInterval>()
    .ToList();

int count = 0;

foreach (var q in qToVerify)
{
    if (intervals.Any(w => w.ContainsPoint(q)))
        count++;
}

Console.WriteLine($"Part 1: {count}");

var working = intervals.ToList();

bool anyChanges = false;

do
{
    anyChanges = false;

    for (int i = 0; !anyChanges && i < working.Count; i++)
    {
        var current = working[i];

        for (int j = i + 1; !anyChanges && j < working.Count; j++)
        {
            var other = working[j];

            if (other.CoversWholeInterval(current))
            {
                working.Remove(current);
                anyChanges = true;
                break;
            }

            if (other.HasIntersect(current))
            {
                var newInterval = other.Union(current);
                working.Remove(current);
                working.Remove(other);
                working.Add(newInterval);
                anyChanges = true;
                break;
            }
        }
    }

} while (anyChanges);

Console.WriteLine($"Part 2: {working.Sum(w => w.Length)}");


static bool GetIntervals(string? input, out ClosedInterval? value)
{
    value = null;

    if (input == null) return false;

    Regex numRegex = new(@"(\d+)");

    var matches = numRegex.Matches(input).Select(w => long.Parse(w.Value)).ToList();

    value = new ClosedInterval(matches[0], matches[1]);

    return true;
}