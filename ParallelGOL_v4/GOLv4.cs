using GameOfLifeConsole;
using GameOfLifeParallel_v3;
using ParallelGOL_v4;
using System.Diagnostics;

int ModuloOperation(int a, int b)
{
    var floatA = (float)a;
    var floatB = (float)b;
    return Convert.ToInt32(floatA - floatB * Math.Floor(floatA / floatB));
}

bool CheckCells(int x, int y, int[][] fullMatrix)
{
    var numCols = fullMatrix.Length;
    var numRows = fullMatrix.Length;

    List<int> xCellsToCheck = new() { ModuloOperation(x - 1, numCols), x, ModuloOperation(x + 1, numCols) };
    List<int> yCellsToCheck = new() { ModuloOperation(y - 1, numRows), y, ModuloOperation(y + 1, numRows) };

    //xCellsToCheck.AddRange(new int[] { ModuloOperation(x - 1, numCols), x, ModuloOperation(x + 1, numCols) });
    //yCellsToCheck.AddRange(new int[] { ModuloOperation(y - 1, numRows), y, ModuloOperation(y + 1, numRows) });

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

    bool output = false;

    int cell = 0;

    if (fullMatrix[x][y] == 1)
        cell = 1;
    else
        cell = 0;

    if (total % 2 == 0 && cell == 0 && total != 0)
    {
        output = true;
    }
    else if ((total == 2 || total == 4 || total == 3) && cell == 1)
    {
        output = true;
    }
    else if (cell == 1)
    {
        output = false;
    }
    else
    {
        output = false;
    }

    return output;
}

async Task<List<Coord>> CheckCellsAsync(List<Coord> cells, int index, int[][] fullMatrix)
{
    List<Coord> cellsUpdate = new List<Coord>();

    return await Task.Factory.StartNew(() =>
    {
        foreach (var cell in cells)
        {
            if (CheckCells(cell.X, cell.Y, fullMatrix))
            {
                cellsUpdate.Add(new Coord(cell.X, cell.Y, 1));
            }
            else
            {
                cellsUpdate.Add(new Coord(cell.X, cell.Y, 0));
            }
        }

        return cellsUpdate;
    });
}

int iteration = 0;

// read in file to matrix
string inputFile = @"C:\Users\David Hoefs\source\repos\GameOfLifeConsole\GameOfLifeConsole\1000x1000step.dat";
FileHandler file = new("", "");
int numCols = 8000;
int numRows = 8000;
int[][] matrix = file.GenerateRandomBoard(numRows, numCols);

int[] gameSize = file.GetSize(inputFile);
//int[,] gameMatrix = file.ReadMatrix(inputFile, gameSize[0], gameSize[1]);
gameSize[0] = numRows;
gameSize[1] = numCols;
//int[][] matrix = file.ReadMatrixToJaggedArray(inputFile, gameSize[0], gameSize[1]);
const int NUM_THREADS = 36;
int divider = matrix.Length / NUM_THREADS;
int splitNum = (gameSize[0] * gameSize[1]) / divider;

if (ModuloOperation(splitNum, gameSize[0]) != 0)
{
    splitNum = ((gameSize[0] * gameSize[1] ) / divider) + 1;
}

List<Coord> testingCoord = file.ReadMatrixToCoord(matrix, gameSize[0], gameSize[1]);
List<bool> again = new List<bool>();
Stopwatch timer = new Stopwatch();
timer.Start();
while (iteration < 100)
{
    Task<List<Coord>>[] finalOutput = new Task<List<Coord>>[divider];
    List<Coord> update = new List<Coord>();
    var split = testingCoord.Split(splitNum);

    if (split is not null)
    {
        int count = 0;
        var splitArray = split.ToArray();
        foreach (var chunk in split)
        {
            var output =  CheckCellsAsync(splitArray[count].ToList(), 1, matrix);
            finalOutput[count] = output;
            count++;
        }
    }

    var completeOutput = await Task.WhenAll(finalOutput);
    foreach (var updateArray in completeOutput)
    {
        foreach (var updatedCell in updateArray)
        {
            matrix[updatedCell.X][updatedCell.Y] = updatedCell.Value;
        }
    }

    Console.WriteLine($"Iteration: {iteration}");
    //file.PrintJaggedMatrixToConsole(matrix);
    iteration++;
}

timer.Stop();
Console.WriteLine($"Elapsed Time: {timer.ElapsedMilliseconds}");
file.PrintJaggedMatrixToFile(matrix);
//file.PrintJaggedMatrixToConsole(matrix);
