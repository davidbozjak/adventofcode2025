var instructionList = new InputProvider<Instruction?>("Input.txt", GetInstruction).Where(w => w != null).Cast<Instruction>().ToList();

int current = 50;

int numAtZero = 0;
int numPassZero = 0;

foreach (var instruction in instructionList)
{
    int factor = instruction.Direction == Direction.Right ? 1 : -1;

    for (int i = instruction.Steps; i > 0; i--)
    {
        current += factor;

        if (current == -1)
        {
            current = 99;
        }
        else if (current == 100)
        {
            current = 0;
        }

        if (current == 0)
            numPassZero++;
    }

    if (current == 0)
        numAtZero++;
}

Console.WriteLine($"Part 1: {numAtZero}");
Console.WriteLine($"Part 2: {numPassZero}");

static bool GetInstruction(string? input, out Instruction? value)
{
    value = null;

    if (input == null) return false;

    Direction direction = input[0] switch
    {
        'L' => Direction.Left,
        'R' => Direction.Right,
        _ => throw new Exception()
    };

    int steps = int.Parse(input[1..]);

    value = new Instruction(direction, steps);

    return true;
}

enum Direction { Left, Right };

record Instruction(Direction Direction, int Steps);