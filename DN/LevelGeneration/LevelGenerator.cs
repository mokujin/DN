using System.Collections.Generic;

namespace DN.LevelGeneration
{
    public partial class LevelGenerator
    {
        internal TileMap TileMap;
        internal ResourseMap ResourseMap;
        private List<Miner> _miners;
       


        public LevelGenerator()
        {

            _miners = new List<Miner>();
        }

        public void Generate(GameWorld gameWorld)
        {

            TileMap = gameWorld.TileMap;

            ResourseMap = new ResourseMap(TileMap.Width, TileMap.Height);
            TileMap.FillWith(CellType.Wall);



            _miners.Add(new Miner(this, TileMap.Width / 2, TileMap.Height -1));
            while(_miners.Count > 0)
            {
                foreach (var miner in _miners)
                {
                    miner.Step();
                }
                for (int i = 0; i < _miners.Count; i++)
                {
                    if (_miners[i].Cell.Y <= 0)
                    {
                        _miners.Remove(_miners[i]);
                        i--;
                        continue;
                    }
                }
            }
        }
    }
}
