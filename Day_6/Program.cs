var lines = new StringInputProvider("Input.txt");

List<long>[] columns = null;

char[] operands = null;

foreach (var line in lines)
{
    var parts = line.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(w => w.Trim()).ToList();

    if (columns == null)
    {
        columns = new List<long>[parts.Count];

        for (int i = 0; i < parts.Count; i++)
        {
            columns[i] = new List<long>();
        }
    }

    if (((string[])["+", "*"]).Contains(parts[0]))
    {
        operands = parts.Select(w => w[0]).ToArray();
        break;
    }

    for (int i = 0; i < parts.Count; i++)
    {
        columns[i].Add(long.Parse(parts[i]));
    }
}

if (operands!.Length != columns.Length)
    throw new Exception();

Console.WriteLine($"Part 1: {AggergateColumns()}");

lines = new StringInputProvider("Input.txt");

foreach (var c in columns)
    c.Clear();

var rows = lines.Reverse().Skip(1).Reverse().ToArray();
int currentColumn = 0;

for (int i = 0; i < rows[0].Length; i++)
{
    var rowStr = new string(rows.Select(w => w[i]).ToArray());

    if (string.IsNullOrWhiteSpace(rowStr))
    {
        currentColumn++;
        continue;
    }

    columns[currentColumn].Add(long.Parse(rowStr));
}

Console.WriteLine($"Part 2: {AggergateColumns()}");

long AggergateColumns()
{
    long sum = 0;

    for (int i = 0; i < columns.Length; i++)
    {
        sum += operands[i] switch
        {
            '+' => columns[i].Sum(),
            '*' => columns[i].Aggregate((w1, w2) => w1 * w2),
            _ => throw new Exception()
        };
    }

    return sum;
}