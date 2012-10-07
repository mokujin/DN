namespace DN
{
    class Program
    {
        static void Main(string[] args)
        {
            using (Game game = new Game())
            {
                game.Run(120.0, 120.0);
            }
        }
    }
}
