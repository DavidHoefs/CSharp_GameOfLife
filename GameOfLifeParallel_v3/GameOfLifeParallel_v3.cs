using GameOfLifeConsole;
using System.Diagnostics;

int ModuloOperation(int a, int b)
{
    var floatA = (float)a;
    var floatB = (float)b;
    return Convert.ToInt32(floatA - floatB * Math.Floor(floatA / floatB));
}

static T CreateJaggedArray<T>(params int[] lengths)
{
    return (T)InitializeJaggedArray(typeof(T).GetElementType(), 0, lengths);
}

int[][] CreateArray(int[] lengths)
{
    int[][] output = new int[lengths[0]][];
    for (int i = 0; i < lengths[0]; i++)
    {
        output[i] = new int[lengths[1]];
        for (int j = 0; j < lengths[1]; j++)
        {
            output[i][j] = -1;
        }
    }

    return output;
}
static object InitializeJaggedArray(Type type, int index, int[] lengths)
{
    Array array = Array.CreateInstance(type, lengths[index]);
    Type elementType = type.GetElementType();

    if (elementType != null)
    {
        for (int i = 0; i < lengths[index]; i++)
        {
            array.SetValue(
                InitializeJaggedArray(elementType, index + 1, lengths), i);
        }
    }
    else
    {
        for (int l = 0; l < lengths[index]; l++)
        {
            array.SetValue(-1, l);
        }
    }

    return array;
}

List<(int, int)> QueueCells(int[,] gameMatrix, int numCols, int numRows)
{
    List<(int, int)> cells = new();
    int counter = 0;
    foreach (var cell in gameMatrix)
    {
        for (int i = 0; i < gameMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < gameMatrix.GetLength(1); j++)
            {
                int x = i;
                int y = j;
                int size = numCols * numRows;
                int[] xCellsToCheck = new int[3];
                int[] yCellsToCheck = new int[3];

                xCellsToCheck[0] = ModuloOperation(x - 1, numCols);
                xCellsToCheck[1] = x;
                xCellsToCheck[2] = ModuloOperation(x + 1, numCols);
                yCellsToCheck[0] = ModuloOperation(y - 1, numRows);
                yCellsToCheck[1] = y;
                yCellsToCheck[2] = ModuloOperation(y + 1, numRows);
                foreach (var xCoord in xCellsToCheck)
                {
                    foreach (var yCoord in yCellsToCheck)
                    {
                        var cellToCheck = (xCoord, yCoord);

                        cells.Add(cellToCheck);
                    }
                }
            }
        }
    }

    return cells;
}

//async Task<List<List<Tuple<int, int>>>> CheckCellsAsync(int x, int[] y, int[][] matrix)
//{
//    var output = new List<List<Tuple<int, int>>>();

//    return await Task.Factory.StartNew(() =>
//    {
//        foreach (var yCoord in y)
//        {
//            output.AddRange(CheckCells(x, yCoord, matrix));
//        }

//        //output.AddRange(CheckCells(x, y, matrix));
//        return output;
//    });
//}
int[][] CheckCells(int x, int y, int[][] matrix, int[][] fullMatrix)
{
    var numCols = matrix.Length;
    var numRows = matrix.Length;

    List<int> xCellsToCheck = new();
    List<int> yCellsToCheck = new();

    xCellsToCheck.AddRange(new int[] { ModuloOperation(x - 1, numCols), x, ModuloOperation(x + 1, numCols) });
    yCellsToCheck.AddRange(new int[] { ModuloOperation(y - 1, numRows), y, ModuloOperation(y + 1, numRows) });

    int total = 0;
    int counter = 0;
    foreach (var xCoord in xCellsToCheck)
    {
        foreach (var yCoord in yCellsToCheck)
        {
            if (fullMatrix[xCoord][yCoord] == 1 && (xCoord, yCoord) != (x, y))
            {
                total++;
            }
            counter++;
        }
    }
    int[][] gameUpdate = CreateArray(new int[] { numCols, numRows }); //CreateJaggedArray<int[][]>(numRows, numCols);

    //List<Tuple<int, int>> alive = new();
    //List<Tuple<int, int>> dead = new();
    int cell = 0;

    if (matrix[x][y] == 1)
        cell = 1;
    else
        cell = 0;

    if (total % 2 == 0 && cell == 0 && total != 0)
    {
        if (matrix[x][y] == 1)
            gameUpdate[x][y] = -1;
        else
            gameUpdate[x][y] = 1;
        //var temp = Tuple.Create(x, y);
        //alive.Add(temp);
    }
    else if ((total == 2 || total == 4 || total == 3) && cell == 1)
    {
        if (matrix[x][y] == 1)
            gameUpdate[x][y] = -1;
        else
            gameUpdate[x][y] = 1;
        //var temp = Tuple.Create(x, y);
        //alive.Add(temp);
    }
    else if (cell == 1)
    {
        if (matrix[x][y] == 0)
            gameUpdate[x][y] = -1;
        else
            gameUpdate[x][y] = 0;
        //var temp = Tuple.Create(x, y);
        //dead.Add(temp);
    }
    else
    {
        if (matrix[x][y] == 0)
            gameUpdate[x][y] = -1;
        else
            gameUpdate[x][y] = 0;
        //var temp = Tuple.Create(x, y);
        //dead.Add(temp);
    }

    for (int i = 0; i < matrix.Length; i++)
    {
        for (int j = 0; j < matrix.Length; j++)
        {
            if (matrix[x][y] == gameUpdate[x][y])
                gameUpdate[x][y] = -1;
        }
    }
    return gameUpdate;
}

static int[][] Update(int[][] matrix, int[][] updates)
{
    int[][] output = CreateJaggedArray<int[][]>(matrix.Length, matrix.Length);
    for (int i = 0; i < matrix.Length; i++)
    {
        for (int j = 0; j < matrix.Length; j++)
        {
            if (matrix[i][j] != updates[i][j])
                output[i][j] = updates[i][j];
            else
                output[i][j] = -1;
        }
    }

    return output;
}

async Task<List<int[][]>> CheckCellsAsync(int[][] matrix, int index, int[][] fullMatrix)
{
    List<int[][]> cells = new List<int[][]>();

    return await Task.Factory.StartNew(() =>
    {
        for (int i = index == 0 ? 0 : (index * matrix.Length) - matrix.Length; i < matrix.Length; i++)
        {
            for (int j = 0; j < matrix.Length; j++)
            {
                cells.Add(CheckCells(i, j, matrix, fullMatrix));
            }
        }

        return cells;
    });
}
int iteration = 0;
// read in file to matrix
string inputFile = @"C:\Users\David Hoefs\source\repos\GameOfLifeConsole\GameOfLifeConsole\6x6step.dat";
FileHandler file = new("", "");

int[] gameSize = file.GetSize(inputFile);
//int[,] gameMatrix = file.ReadMatrix(inputFile, gameSize[0], gameSize[1]);

int[][] matrix = file.ReadMatrixToJaggedArray(inputFile, gameSize[0], gameSize[1]);

var watch = new Stopwatch();
watch.Start();
const int NUMPARTS = 2;
int divider = matrix.Length / NUMPARTS;
int counter = 0;


while (iteration < 1)
{
    List<int[][]>[] split = new List<int[][]>[NUMPARTS];
    var splitArray = matrix.Split(divider).ToList();
    var testing = splitArray.ToList();

    for (int count = 0; count < NUMPARTS; count++)
    {
        var d = testing.ToArray()[count].ToArray();

        outputData[count] = CheckCellsAsync(testing.ToArray()[count].ToArray(), count, matrix);
    }

    var results = await Task.WhenAll(outputData);
    foreach (var item in results)
    {
        foreach (var item2 in item)
        {
            for (int i = 0; i < item2.Length; i++)
            {
                for (int j = 0; j < item2.Length; j++)
                {
                    if (matrix[i][j] != -1)
                        matrix[i][j] = item2[i][j];
                }
            }
        }
    }

    Console.WriteLine();
    //Parallel.For(0, divider, i =>
    //{
    //    for (int x = i == 0 ? 0 : (divider * i) - divider; x < matrix.Length; x++)
    //    {
    //        if (x == (i == 1 ? divider : divider * i))
    //        {
    //            counter++;
    //            break;
    //        }
    //        for (int y = 0; y < matrix.Length; y++)
    //        {
    //            var output = CheckCells(x, y, matrix);
    //            lock (newUpdates)
    //                newUpdates.Add(output);
    //        }
    //    }
    //});

    //foreach (var update in newUpdates)
    //{
    //    for (int i = 0; i < gameSize[0]; i++)
    //    {
    //        for (int j = 0; j < gameSize[1]; j++)
    //        {
    //            if (update[i][j] != -1)
    //                matrix[i][j] = update[i][j];
    //        }
    //    }
    //}

    Console.WriteLine($"Iteration: {iteration}");
    iteration++;
}
watch.Stop();
Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
Console.WriteLine("Final Iteration:");
// print matrix
file.PrintJaggedMatrixToConsole(matrix);
//file.PrintMatrixToFile(gameMatrix);

public static class Extensions
{
    /// <summary>
    /// Splits an array into several smaller arrays.
    /// </summary>
    /// <typeparam name="T">The type of the array.</typeparam>
    /// <param name="array">The array to split.</param>
    /// <param name="size">The size of the smaller arrays.</param>
    /// <returns>An array containing smaller arrays.</returns>
    public static IEnumerable<IEnumerable<T>> Split<T>(this T[] array, int size)
    {
        for (var i = 0; i < (float)array.Length / size; i++)
        {
            yield return array.Skip(i * size).Take(size);
        }
    }

    /// <summary>
    /// Splits a given array into a two dimensional arrays of a given size.
    /// The given size must be a divisor of the initial array, otherwise the returned value is <c>null</c>,
    /// because not all the values will fit into the resulting array.
    /// </summary>
    /// <param name="array">The array to split.</param>
    /// <param name="size">The size to split the array into. The size must be a divisor of the length of the array.</param>
    /// <returns>
    /// A two dimensional array if the size is a divisor of the length of the initial array, otherwise <c>null</c>.
    /// </returns>
    //public static T[][]? ToSquare2D<T>(this T[][] array, int size)
    //{
    //    if (array.Length % size != 0) return null;

    //    var firstDimensionLength = array.Length / size;
    //    var buffer = new T[firstDimensionLength][];

    //    for (var i = 0; i < firstDimensionLength; i++)
    //    {
    //        for (var j = 0; j < size; j++)
    //        {
    //            buffer[i][j] = array[i * size + j];
    //        }
    //    }

    //    return buffer;
    //}
}