namespace GameOfLifeConsole
{
    public class FileHandler
    {
        private readonly string? _inputFile;
        private string? _outputFile;

        public FileHandler(string inputFile, string outputFile)
        {
            _inputFile = inputFile;
            _outputFile = outputFile;
        }

        public int[,] ReadMatrix(string inputFile)
        {
            int i = 0;
            int j = 0;
            int[,] output = new int[6, 6];

            string input = File.ReadAllText(path: inputFile);
            foreach (var row in input.Split("\n"))
            {
                j = 0;
                //var test = row.Replace("\n", " ").ToCharArray();
                foreach (var col in row.Replace("\n", " ").ToCharArray())
                {
                    if (col == '.')
                        output[i, j] = 0;
                    else
                        output[i, j] = 1;

                    ++j;
                }
                ++i;
            }

            return output;
        }

        public Dictionary<(int, int), int> ReadMatrixFaster(string inputFile)
        {
            int i = 0;
            
            var output = new Dictionary<(int, int), int>();

            string input = File.ReadAllText(path: inputFile);
            foreach (var row in input.Split("\n"))
            {
                int j = 0;

                foreach (var col in row.Replace("\n", null).Replace("\r",null).ToCharArray())
                {
                    if (col == 'O')
                    {
                        output[(i,j)] = 1;
                    }
                    
                    
          

                    j++;
                }
                i++;
            }

            return output;
        }

        public void PrintMatrixToConsole(int[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    if (matrix[i, j] == 0)
                        Console.Write('.');
                    else
                        Console.Write('O');
                }
                Console.WriteLine();
            }
        }

        public void PrintDictionaryToConsole(Dictionary<(int, int), int> dict,int numCols,int numRows)
        {
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    if (dict.ContainsKey((i, j)))
                        Console.Write('O');
                    else
                        Console.Write('.');
                }

                Console.WriteLine();
            }
        }

        public int[] GetSize(string inputFile)
        {
            int i = 0;
            int j = 0;
            int cols = 0;
            int rows = 0;
            int[] output = new int[2];

            string input = File.ReadAllText(path: inputFile);
            foreach (var row in input.Split("\n"))
            {
                j = 0;

                foreach (var col in row.Replace("\n", null).ToCharArray())
                {
                    j++;
                }
                cols = j;

                i++;
            }
            rows = i;

            output[0] = cols;
            output[1] = rows;
            return output;
        }
    }
}