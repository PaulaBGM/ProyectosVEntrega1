using _Scripts.Tiles;

namespace _Scripts.Pathfinding
{
    public class PathNode
    {
        public LevelTile LevelTile { get; }
    
        public PathNode Parent { get; set; }
    
        public int GCost { get; set; }
    
        public int HCost { get; set; }
    
        public int FCost => GCost + HCost;

        // Constructor
        public PathNode(LevelTile levelTile, PathNode parent, int gCost, int hCost)
        {
            LevelTile = levelTile;
            Parent = parent;
            GCost = gCost;
            HCost = hCost;
        }
    }
}
