using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Boggle
{
     /**
     * A class that stores a dictionary containing words that can be used in a
     * Boggle game.
     * 
     * @author Jason Allen
     */
    class BoggleDictionary
    {
        public SortedSet<String> dictionary;

        /**
        * Create the BoggleDictionary from the file dictionary.dat
        */
        public BoggleDictionary()
        {
            dictionary = new SortedSet<String>();
            StreamReader file = new StreamReader("dictionary.txt");
            String line;
            while ((line = file.ReadLine()) != null)
            {
                dictionary.Add(line);
            }
        }

        /**
         * Check if a specific sequence of letters appears in the dictionary.
         *
         * @param possibleWord: The string whose existence in the Dictionary is being queried.
         * @return true if this is a known word, otherwise false.
         */
        public Boolean Contains(String possibleWord)
        {
            return dictionary.Contains(possibleWord.ToUpper());
        }

        /**
         * Find all of the words in the Dictionary which start with a given sequence of letters.
         *
         * @param prefix: the start of a possible word
         * @return A Dictionary which contains only those known words which start with the supplied prefix.
         */
        public SortedSet<String> GetChildWords(String prefix)
        {
            return dictionary.GetViewBetween(prefix, prefix + "zzzz");
        }

    }
}
