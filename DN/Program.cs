namespace DN
{
    class Program
    {
        static void Main(string[] args)
        {
            using (Game game = new Game())
            {
                game.Run(200.0,200.0);
            }
        }
    }
}
