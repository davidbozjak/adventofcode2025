using System.Text.RegularExpressions;

var points = new InputProvider<(long x, long y)?>("Input.txt", GetIntPair).Where(w => w != null).Cast<(long x, long y)>().ToList();

Console.WriteLine($"Part 1: {GetMaxArea((_, __) => true)}");

// Create a path the input outlines, to use later to check the square against
List<(long x, long y)> path = new();

for (int i = 1; i < points.Count + 1; i++)
{
    var p1 = points[i - 1];
    var p2 = points[i % points.Count];

    path.AddRange(MakePathBetweenAlignedPoints(p1, p2));
}

if (path.First() != path.Last())
    throw new Exception();

Dictionary<(double, double), bool> memCache = new();

Console.WriteLine($"Part 2: {GetMaxArea(IsCompletelyInside)}");


long GetMaxArea(Func<(long x, long y), (long x, long y), bool> AdditionalCheck)
{
    long maxArea = long.MinValue;

    for (int i = 0; i < points.Count; i++)
    {
        var p1 = points[i];

        for (int j = i + 1; j < points.Count; j++)
        {
            var p2 = points[j];

            var diffX = Math.Abs(p1.x - p2.x) + 1;
            var diffY = Math.Abs(p1.y - p2.y) + 1;

            var area = diffX * diffY;

            if (area > maxArea)
            {
                if (AdditionalCheck(p1, p2))
                {
                    maxArea = area;
                }
            }
        }
    }

    return maxArea;
}

IEnumerable<(long x, long y)> MakePathBetweenAlignedPoints((long x, long y) p1,  (long x, long y) p2)
{
    if (p1.x == p2.x)
    {
        (long min, long max) = p1.y < p2.y ? (p1.y, p2.y) : (p2.y, p2.y);
        for (long y = min; y <= max; y++)
        {
            yield return (p1.x, y);
        }
    }
    else if (p1.y == p2.y)
    {
        (long min, long max) = p1.x < p2.x ? (p1.x, p2.x) : (p2.x, p2.x);
        for (long x = min; x <= max; x++)
        {
            yield return (x, p1.y);
        }
    }
    else throw new Exception();
}

bool IsCompletelyInside((long x, long y) p1, (long x, long y) p2)
{
    var minX = Math.Min(p1.x, p2.x);
    var maxX = Math.Max(p1.x, p2.x);
    var minY = Math.Min(p1.y, p2.y);
    var maxY = Math.Max(p1.y, p2.y);

    // first check for all 4 courners, if any of the corners is outside the polygon, false
    if (!PointInPolygonInclusive(minX, minY, points))
        return false;
    if (!PointInPolygonInclusive(minX, maxY, points))
        return false;
    if (!PointInPolygonInclusive(maxX, minY, points))
        return false;
    if (!PointInPolygonInclusive(maxX, maxY, points))
        return false;

    bool allOfPathOutsideSquare = true;

    // check if any of the points of the path are INSIDE (not on edge) of the square
    foreach (var p in path)
    {
        if ((p.x > minX && p.x < maxX) &&
            (p.y > minY && p.y < maxY))
        {
            allOfPathOutsideSquare = false;
            break;
        }
    }

    return allOfPathOutsideSquare;
}

static bool GetIntPair(string? input, out (long, long)? value)
{
    value = null;

    if (input == null) return false;

    Regex numRegex = new(@"-?\d+");

    var numbers = numRegex.Matches(input).Select(w => int.Parse(w.Value)).ToList();

    if (numbers.Count != 2) throw new Exception();

    value = (numbers[0], numbers[1]);

    return true;
}

// Note: Using a known algorithm for "is point within a polygon", not my own code (except for caching and memcache)
bool PointInPolygonInclusive(double px, double py, IEnumerable<(long x, long y)> poly_int, double eps = 1e-9)
{
    if (!memCache.ContainsKey((px, py)))
    {
        (double x, double y)[] poly = poly_int.Select(w => ((double)w.x, (double)w.y)).ToArray();

        memCache[(px, py)] = PointInPolygonInclusive_Internal(px, py, poly);
    }
    
    return memCache[(px, py)];

    bool PointInPolygonInclusive_Internal(double px, double py, (double x, double y)[] poly, double eps = 1e-9)
    {
        int n = poly.Length;
        if (n < 3) return false;

        // 1) Boundary check: is P on any edge?
        for (int i = 0, j = n - 1; i < n; j = i, i++)
        {
            var (x1, y1) = poly[j];
            var (x2, y2) = poly[i];

            // Vector cross to test collinearity: (P - A) x (B - A) ≈ 0
            double cross = (px - x1) * (y2 - y1) - (py - y1) * (x2 - x1);
            if (Math.Abs(cross) <= eps)
            {
                // Within bounding box?
                double dot = (px - x1) * (px - x2) + (py - y1) * (py - y2);
                if (dot <= eps) return true; // On segment (inclusive)
            }
        }

        // 2) Even–odd ray casting (half-open y-intervals)
        bool inside = false;
        for (int i = 0, j = n - 1; i < n; j = i, i++)
        {
            var (xi, yi) = poly[i];
            var (xj, yj) = poly[j];

            // Ignore horizontal edges for crossing count
            if (Math.Abs(yi - yj) <= eps) continue;

            bool intersects = ((yi > py) != (yj > py)); // half-open: include lower, exclude upper
            if (intersects)
            {
                // Compute x coordinate of intersection with the horizontal ray at py
                double xInt = xj + (xi - xj) * (py - yj) / (yi - yj);
                if (xInt > px) inside = !inside;
            }
        }

        return inside;
    }
}
