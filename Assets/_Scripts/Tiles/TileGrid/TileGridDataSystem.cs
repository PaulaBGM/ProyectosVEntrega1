using System.Collections.Generic;
using System.Linq;
using _Scripts.Events;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Scripts.Tiles.TileGrid
{
    public class TileGridDataSystem : MediatorClientSystem<TileGridMediator>
    {
        [SerializeField]
        private Tilemap[] _tilemapsFromTopToBottom;

        public Dictionary<Vector3, LevelTile> Tiles { get; private set; } = new();

        protected override void Awake()
        {
            base.Awake();
            SetWorldTiles();
        }

        private void OnEnable()
        {
            mediator.OnTileClicked += GetTileFromPlayerInput;
        }

        public void SetWorldTiles()
        {
            Tiles.Clear();
            int layerCounter = 0;

            foreach (var tilemap in _tilemapsFromTopToBottom)
            {
                foreach (Vector3Int localPosition in tilemap.cellBounds.allPositionsWithin)
                {
                    if ((!tilemap.HasTile(localPosition)) ||
                        Tiles.ContainsKey(tilemap.CellToWorld(localPosition)))
                        continue;

                    var tile = new LevelTile(
                        localPosition: localPosition,
                        worldPosition: tilemap.CellToWorld(localPosition),
                        tileBase: tilemap.GetTile(localPosition),
                        tilemapMember: tilemap,
                        heightLayer: layerCounter);

                    Tiles.Add(tile.WorldPosition, tile);
                }

                layerCounter++;
            }

            AssignNeighbours();
            mediator.WorldTilesSet(Tiles);
        }

        private void AssignNeighbours()
        {
            foreach (var tile in Tiles.Values)
            {
                tile.TileNeighbours = new LevelTileNeighbours(
                    upTile: GetWorldTile(tile.WorldPosition + new Vector3(0, tile.HeightSize, 0)),
                    downTile: GetWorldTile(tile.WorldPosition + new Vector3(0, -tile.HeightSize, 0)),
                    rightTile: GetWorldTile(tile.WorldPosition + new Vector3(tile.WidthSize, 0, 0)),
                    leftTile: GetWorldTile(tile.WorldPosition + new Vector3(-tile.WidthSize, 0, 0))
                );
            }
        }

        private LevelTile GetWorldTile(Vector3 position) =>
            Tiles.GetValueOrDefault(_tilemapsFromTopToBottom.First().WorldToCell(position));

        private void GetTileFromPlayerInput(OnTileClicked eventData)
        {
            var tileSelected = GetWorldTile(eventData.Point);

            mediator.TileExecuteAction(tileSelected);
        }

        private void OnDisable()
        {
            mediator.OnTileClicked -= GetTileFromPlayerInput;
        }
    }
}