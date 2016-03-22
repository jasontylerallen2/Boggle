using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Boggle
{
    /**
    * A class containing methods to solve a Boggle board
    * 
    * @author Jason Allen
    */
    class Solver
    {

        private static String[,] letters;

        /**
        * Constructs a Board for a set of letters
        * 
        * @param boardLetters: letters to make the board out of
        */
        public Solver(String[,] boardLetters)
        {
            letters = boardLetters;
        }

        /**
        * Solves the board and returns the solutions and paths
        * 
        * @param dictionary: dictionary to check against when testing words
        * 
        * @return: A dictionary of all the solutions mapping to a 2D Boolean array indicating the path
        */
        public SortedDictionary<String, Boolean[,]> Solve(BoggleDictionary dictionary)
        {
            SortedDictionary<String, Boolean[,]> solutionsAndPaths = new SortedDictionary<String, Boolean[,]>();

            // start at every position on the grid
            for (int row = 0; row < letters.GetLength(0); row++)
            {
                for (int column = 0; column < letters.GetLength(0); column++)
                {
                    Stack<WorldState> stack = new Stack<WorldState>();
                    stack.Push(new WorldState(row, column));

                    while (stack.Count() != 0)
                    {
                        WorldState lastWS = stack.Pop();

                        // for every neighbor row and column
                        for (int r = lastWS.row - 1; r <= lastWS.row + 1; r++)
                        {
                            for (int c = lastWS.column - 1; c <= lastWS.column + 1; c++)
                            {
                                // if neighbor position is a valid grid square and not previously visited
                                if (r >= 0 && c >= 0 && r < letters.GetLength(0) && c < letters.GetLength(0) && !lastWS.visited[r, c])
                                {
                                    WorldState newWS = new WorldState(r, c, lastWS);
                                    String possibleWord = newWS.word;

                                    // check if the word is a valid prefix
                                    SortedSet<String> childWords = dictionary.GetChildWords(possibleWord);
                                    if (childWords.Count() != 0)
                                    {
                                        stack.Push(newWS);
                                        if (dictionary.Contains(possibleWord) && !solutionsAndPaths.ContainsKey(possibleWord))
                                        {
                                            solutionsAndPaths.Add(possibleWord, newWS.visited);
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
            }
            return solutionsAndPaths;
        }


        /**
         * Indicates the current state of the board as it is being solved, including the visited squares,
         * the current position, and the word so far
         */
        public class WorldState
        {

            public int row;
            public int column;
            public String word;
            public Boolean[,] visited;

            /**
             * Constructs a WorldState based on the previous WorldState
             * 
             * @param row: the current row
             * @param column: the current column
             * @param parent: the WorldState prior to the one being constructed
             */
            public WorldState(int row, int column, WorldState parent)
            {
                this.row = row;
                this.column = column;
                this.visited = new Boolean[letters.GetLength(0), letters.GetLength(0)];
                // add to the word being built
                this.word = parent.word + letters[row, column];
                // copy the visited squares
                for (int r = 0; r < parent.visited.GetLength(0); r++)
                {
                    for (int c = 0; c < parent.visited.GetLength(0); c++)
                    {
                        this.visited[r, c] = parent.visited[r, c];
                    }
                }
                this.visited[row, column] = true;
            }

            /**
             * Constructs the first WorldState for each letter in the grid
             * 
             * @param row: the current row
             * @param column: the current column
             */
            public WorldState(int row, int column)
            {
                this.row = row;
                this.column = column;
                this.word = letters[row, column];
                this.visited = new Boolean[letters.GetLength(0), letters.GetLength(0)];
                this.visited[row, column] = true;
            }
        }

    }
}
