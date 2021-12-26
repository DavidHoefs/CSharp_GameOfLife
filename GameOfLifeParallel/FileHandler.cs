using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLifeConsole
{
    public class FileHandler
    {

        private readonly string? _inputFile;
        private string? _outputFile;
 

        public FileHandler(string inputFile,string outputFile)
        {
            _inputFile = inputFile;
            _outputFile = outputFile;
        }

        public int[,] ReadMatrix(string inputFile,int numCols,int numRows)
        {
            int i = 0;
            
            int[,] output = new int[numCols,numRows];

            string input = File.ReadAllText(path: inputFile);
            foreach (var row in input.Split("\n"))
            {
                int j = 0;
                var test = row.Replace("\r\n", null).Replace('\r',' ').ToCharArray();
                foreach (var col in row.Replace("\r",null).Replace("\n",null).ToCharArray())
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

        public int[][] ReadMatrixToJaggedArray(string inputFile,int numCols,int numRows)
        {
            string input = File.ReadAllText(@inputFile);
            int i = 0;
            int[][] output = new int[numRows][];
            foreach (var row in input.Split("\n"))
            {
                int j = 0;
                output[i] = new int[numCols];
                foreach (var col in row.Replace("\n",null).Replace("\r",null).ToCharArray())
                {
                    
                    if (col == '.')
                        output[i][j] = 0;
                    else
                        output[i][j] = 1;

                    j++;
                }
                i++;
            }

            return output;
        }

        public Dictionary<(int,int),int> ReadMatrixFaster(string inputFile)
        {
            int i = 0;
            int j = 0;
            var output = new Dictionary<(int,int),int>();

            string input = File.ReadAllText(path: inputFile);
            foreach (var row in input.Split("\n"))
            {
                j = 0;
                var test = row.Replace("\n", " ").ToCharArray();
                foreach (var col in row.Replace("\n", " ").ToCharArray())
                {
                    if (col == '.')
                        output[(i,j)] = 0;
                    else
                        output[(i, j)] = 1;

                    ++j;
                }
                ++i;
            }

            return output;
        }

        public void PrintMatrixToConsole(int[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    if(matrix[i, j] == 0)
                        Console.Write('.');
                    else
                        Console.Write('O');

                }
                Console.WriteLine();
            }
        }

        public void PrintJaggedMatrixToConsole(int[][] matrix)
        {
            for (int i = 0; i < matrix.Length; i++)
            {
                for (int j = 0; j < matrix.Length; j++)
                {
                    if (matrix[i][j] == 0)
                        Console.Write('.');
                    else
                        Console.Write('O');

                }
                Console.WriteLine();
            }
        }
        public void PrintMatrixToFile(int[,] matrix)
        {
            StreamWriter streamWriter = new StreamWriter(@"C:\Users\David Hoefs\source\repos\GameOfLifeConsole\GameOfLifeConsole\output.dat");
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    if (matrix[i, j] == 0)
                        streamWriter.Write('.');
                    else
                        streamWriter.Write('O');

                }
                streamWriter.WriteLine();
            }

            streamWriter.Close();
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
                    ++j;
                }
                cols = j;

                ++i;
            }
            rows = i;

            output[0] = cols;
            output[1] = rows ;
            return output;
        }




    }
}
