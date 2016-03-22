using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boggle
{

    /**
     * A class representing a board for a game of Boggle.
     * 
     * @author Jason Allen
     */
    class Board
    {

        private Random random = new Random();
	    private readonly int STANDARD_DICE = 16;
	    private Die[,] dice;
	    private int dimension;
	    private String[,] letters;
	
	    /**
	     * Constructs a Board object
	     * 
	     * @param int: the dimensions for the board
	     */
	    public Board(int dimension)
        {
		    this.dimension = dimension;
		    dice = new Die[dimension, dimension];
		    this.letters = getLetters();
	    }
	
	    /**
	     * Retrieves the letters that are currently showing
	     * 
	     * @return the letters showing
	     */
	    public String[,] getLetters() {
            String[,] board = new String[dimension, dimension];
            getDice();
            for (int row = 0; row < dimension; row++)
            {
                for (int column = 0; column < dimension; column++)
                {
                    board[row, column] = dice[row, column].Roll();
                }
            }
            return board;
	    }
	
	    /**
	     * Returns the dice that will be used to determine the layout of the board
	     * 
	     * @return the dice for the board
	     */
	    private void getDice()
        {
		    if (dimension == 4)
            {
			    dice = standardDiceArray();
            }
		    else
            {
			    Die[,] gameDice = new Die[dimension, dimension];
			    List<Die> standardDice = standardDiceList();
			    for (int i = 0; i < dimension * dimension; i++)
                {
                    if (standardDice.Count == 0)
                    {
                        standardDice = standardDiceList();
                    }
				
				    int index = 0;
                    if (standardDice.Count > 1)
                    {
                        index = random.Next(standardDice.Count - 1);
                    }
				    Die newDie = standardDice.ElementAt(index);
				    standardDice.RemoveAt(index);
				
				    // add newDie to gameDice
				    int row = i / dimension;
				    int column = i % dimension;
				    gameDice[row, column] = newDie;
			    }
			    dice = gameDice;
		    }
	    }
	
	    /**
	     * Retrieves the standard 16 dice in the form of a 2D Array
	     * 
	     * @return the standard set of 16 dice
	     */
	    private Die[,] standardDiceArray()
        {
		    Die[,] gameDice = new Die[4, 4];
		    for (int row = 0; row < 4; row++)
            {
                for (int column = 0; column < 4; column++)
                {
                    gameDice[row, column] = new Die(column + row);
                }
		    }
		    return gameDice;
	    }
	
	    /**
	     * Retrieves the standard 16 dice in the form of an List
	     * 
	     * @return the standard set of 16 dice
	     */
	    private List<Die> standardDiceList()
        {
		    List<Die> standardDice = new List<Die>();
            for (int i = 0; i < STANDARD_DICE; i++)
            {
                standardDice.Add(new Die(i));
            }
		    return standardDice;
	    }

    }
}
