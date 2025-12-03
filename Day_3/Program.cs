var inputLines = new StringInputProvider("Input.txt").ToList();

Dictionary<(string, int), long> memcache = new();

Console.WriteLine("Part 1:" + inputLines.Select(w => GetMaxValue(w, 2)).Sum());
Console.WriteLine("Part 2:" + inputLines.Select(w => GetMaxValue(w, 12)).Sum());


long GetMaxValue(string remainingInput, int remainingCharacters)
{
    if (!memcache.ContainsKey((remainingInput, remainingCharacters)))
    {
        memcache[(remainingInput, remainingCharacters)] = GetValues(remainingInput, remainingCharacters).Max();
    }

    return memcache[(remainingInput, remainingCharacters)];

    IEnumerable<long> GetValues(string remainingInput, int remainingCharacters)
    {
        if (remainingCharacters == 0)
        {
            throw new Exception();
        }
        if (remainingInput.Length < remainingCharacters)
        {
            throw new Exception();
        }
        
        if (remainingInput.Length == remainingCharacters)
        {
            yield return long.Parse(remainingInput);
            yield break;
        }

        //don't include the current character
        yield return GetMaxValue(remainingInput[1..], remainingCharacters);

        //include the current character - either on it's own or in addition with the recursive step
        if (remainingCharacters > 1)
        {
            yield return long.Parse(remainingInput[0] + GetMaxValue(remainingInput[1..], remainingCharacters - 1).ToString());
        }
        else
        {
            yield return long.Parse(remainingInput[0].ToString());
        }
    }
}