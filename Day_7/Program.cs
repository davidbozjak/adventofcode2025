var map = new StringInputProvider("Input.txt").ToList();

Tile startTile = null;

var tileWorld = new TileWorld(map, true, GetTile);

var printer = new WorldPrinter();

printer.Print(tileWorld);

HashSet<Tile> beams = [startTile];
int countSplit = 0;
Dictionary<Tile, long> hits = new Dictionary<Tile, long>() { { startTile, 1 } };

while (beams.Count > 0)
{
    var newBeams = new HashSet<Tile>();

    foreach (var beamLocation in beams)
    {
        if (beamLocation.TraversibleNeighbours.FirstOrDefault(w => w.Position == beamLocation.Position.Down()) is Tile newLocation && newLocation != null)
        {
            if (newLocation is SplitTile)
            {
                var left = tileWorld.GetTileAtOrNull(newLocation.Position.Left());
                newBeams.AddIfNotNull(left);
                CountHit(left, beamLocation);

                var right = tileWorld.GetTileAtOrNull(newLocation.Position.Right());
                newBeams.AddIfNotNull(right);
                CountHit(right, beamLocation);

                countSplit++;
            }
            else
            {
                newBeams.Add(newLocation);
                CountHit(newLocation, beamLocation);
            }
        }
    }

    beams = newBeams;
}

Console.WriteLine($"Part 1: {countSplit}");
Console.WriteLine($"Part 2: {hits.Where(w => w.Key.Position.Y == tileWorld.MaxY).Sum(w => w.Value)}");


void CountHit(Tile? t, Tile? prevTile)
{
    if (t == null)
        return;

    long h = prevTile != null ? hits.ContainsKey(prevTile) ? hits[prevTile] : 1 : 1;

    if (!hits.ContainsKey(t))
    {
        hits.Add(t, h);
    }
    else
    {
        hits[t] += h;
    }
}

Tile GetTile(int x, int y, char c, Func<Tile, IEnumerable<Tile>> func)
{
    if (c == '^')
    {
        return new SplitTile(x, y, func);
    }
    else if (c == 'S')
    {
        startTile = new Tile(x, y, true, func);
        return startTile;
    }
    else
    {
        return new Tile(x, y, true, func);
    }
}

class SplitTile : Tile
{
    public override char CharRepresentation => '^';

    public SplitTile(int x, int y, Func<Tile, IEnumerable<Tile>> fillTraversibleNeighboursFunc)
        : base(x, y, true, fillTraversibleNeighboursFunc)
    {

    }
}