using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boggle
{
    /**
     * A class representing a die from the game of Boggle.
     * 
     * @author Jason Allen
     */
    class Die
    {
        public String[] die;
        private Random random = new Random();
        private readonly int NUM_SIDES = 6;

        /**
         * Constructs a Die object.
         * 
         * @param dieNumber: indicates which of the 16 standard die should be constructed
         */
        public Die(int dieNumber)
        {
            die = GetDie(dieNumber);
        }

        /**
         * Retrieves the letters on the die
         * 
         * @param dieNumber: indicates which of the 16 standard die should be represented
         * @return the six letters on the die
         */
        private String[] GetDie(int dieNumber)
        {
            if (dieNumber == 0)
            {
                String[] die = { "R", "Y", "T", "T", "E", "L" };
                return die;
            }
            if (dieNumber == 1)
            {
                String[] die = { "A", "N", "A", "E", "E", "G" };
                return die;
            }
            if (dieNumber == 2)
            {
                String[] die = { "A", "F", "P", "K", "F", "S" };
                return die;
            }
            if (dieNumber == 3)
            {
                String[] die = { "Y", "L", "D", "E", "V", "R" };
                return die;
            }
            if (dieNumber == 4)
            {
                String[] die = { "V", "T", "H", "R", "W", "E" };
                return die;
            }
            if (dieNumber == 5)
            {
                String[] die = { "I", "D", "S", "Y", "T", "T" };
                return die;
            }
            if (dieNumber == 6)
            {
                String[] die = { "X", "L", "D", "E", "R", "I" };
                return die;
            }
            if (dieNumber == 7)
            {
                String[] die = { "Z", "N", "R", "N", "H", "L" };
                return die;
            }
            if (dieNumber == 8)
            {
                String[] die = { "R", "Y", "T", "T", "E", "L" };
                return die;
            }
            if (dieNumber == 9)
            {
                String[] die = { "O", "A", "T", "T", "O", "W" };
                return die;
            }
            if (dieNumber == 10)
            {
                String[] die = { "H", "C", "P", "O", "A", "S" };
                return die;
            }
            if (dieNumber == 11)
            {
                String[] die = { "N", "M", "I", "QU", "H", "U" };
                return die;
            }
            if (dieNumber == 12)
            {
                String[] die = { "S", "E", "O", "T", "I", "S" };
                return die;
            }
            if (dieNumber == 13)
            {
                String[] die = { "M", "T", "O", "I", "C", "U" };
                return die;
            }
            if (dieNumber == 14)
            {
                String[] die = { "E", "N", "S", "I", "E", "U" };
                return die;
            }
            if (dieNumber == 15)
            {
                String[] die = { "O", "B", "B", "A", "O", "J" };
                return die;
            }
            return null;
        }

        /**
         * Rolls the die and returns the letter to display
         * 
         * @return the letter to display
         */
        public String Roll()
        {
            return die[random.Next(NUM_SIDES)];
        }

    }
}
