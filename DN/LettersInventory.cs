using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DN
{
    public class LettersInventory
    {
        private readonly Dictionary<char, int> _letters;

        public LettersInventory()
        {
            _letters = new Dictionary<char, int>();

            for (int i = 97; i < 122; i++)
            {
                _letters.Add((char)i, 0);
            }
        }

        public void Add(char ch)
        {
            _letters[ch]++;
        }
        public void Remove(char ch)
        {
            _letters[ch]--;
        }
    }
}
