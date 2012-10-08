using System;

namespace DN
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {


            using (Game game = new Game())
            {
                game.Run(120.0, 120.0);
            }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
