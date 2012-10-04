using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DN
{
    public class LettersInventory
    {
        private List<char> _letters;

        public LettersInventory()
        {
            _letters = new List<char>();
        }

        public void Add()
        { }
        public void Remove()
        { }

        public bool IsIn(char l)
        {
            return _letters.Any(p => p == l);
        }
    }
}
