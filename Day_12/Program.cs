using System.Text.RegularExpressions;

var regions = new InputProvider<Region?>("Regions.txt", GetRegion).Where(w => w != null).Cast<Region>().ToList();

var parser = new MultiLineParser<Shape>(() => new Shape(), (shape, line) => shape.AddLine(line));

var inputProvider = new InputProvider<Shape?>("Presents.txt", parser.AddLine)
{
    CheckBufferFirst = false,
    EndAtEmptyLine = false
};

var shapes = inputProvider.Where(w => w != null).ToDictionary(w => w.Id, w => w);

int count = 0;

foreach (var region in regions)
{
    List<Shape> toFit = new List<Shape>();

    for (int i = 0; i < region.Presents.Length; i++)
    {
        toFit.AddRange(Enumerable.Repeat(shapes[i], region.Presents[i]));
    }

    var minFillRequirement = toFit.Sum(w => w.TotalFill);

    if (region.TotalSize > minFillRequirement)
    {
        count++;
    }
}

Console.WriteLine($"Part 1: {count}");

static bool GetRegion(string? input, out Region? value)
{
    value = null;

    if (input == null) return false;

    var parts = input.Split(':');

    if (parts.Length != 2)
        throw new Exception();

    Regex numRegex = new(@"-?\d+");

    var sizes = numRegex.Matches(parts[0]).Select(w => int.Parse(w.Value)).ToList();

    if (sizes.Count != 2) 
        throw new Exception();

    var numbers = numRegex.Matches(parts[1]).Select(w => int.Parse(w.Value)).ToArray();

    if (numbers.Length < 1)
        throw new Exception();

    value = new Region(sizes[0], sizes[1], numbers);

    return true;
}

record Region(int SizeX, int SizeY, int[] Presents)
{
    public int TotalSize => SizeX * SizeY;
}

class Shape
{
    private Cached<int> totalFill;
    public int TotalFill => totalFill.Value;

    public int Id { get; private set; } = -1;

    public List<string> Lines { get; }

    public Shape()
    {
        this.Lines = new List<string>();
        
        totalFill = new Cached<int>(CalcTotalFill);
    }

    public void AddLine(string line)
    {
        if (Id == -1)
        {
            Id = int.Parse(line[..^1]);
        }
        else
        {
            this.Lines.Add(line);
        }
    }

    private int CalcTotalFill()
    {
        return Lines.Sum(w => w.Sum(w => w == '#' ? 1 : 0));
    }
}