using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Core.Mediator;
using _Scripts.Events;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Scripts.Tiles.TileGrid
{
    public class TileGridDataSystem : MediatorClientSystem<TileGridMediator>
    {
        [SerializeField] private Tilemap[] _tilemapsFromTopToBottom;

        private readonly Dictionary<Vector3, LevelTile> _tiles = new();

        private void OnEnable()
        {
            mediator.OnTileClicked += GetTileFromPlayerInput;
        }

        private void Start()
        {
            SetWorldTiles();
        }

        public void SetWorldTiles()
        {
            _tiles.Clear();
            int layerCounter = 0;

            foreach (var tilemap in _tilemapsFromTopToBottom)
            {
                foreach (Vector3Int localPosition in tilemap.cellBounds.allPositionsWithin)
                {
                    if ((!tilemap.HasTile(localPosition)) ||
                        _tiles.ContainsKey(tilemap.CellToWorld(localPosition)))
                        continue;

                    var tile = new LevelTile(
                        localPosition: localPosition,
                        worldPosition: tilemap.CellToWorld(localPosition),
                        tileBase: tilemap.GetTile(localPosition),
                        tilemapMember: tilemap,
                        heightLayer: layerCounter);

                    _tiles.Add(tile.WorldPosition, tile);
                }

                layerCounter++;
            }

            AssignNeighbours();
            mediator.WorldTilesSet(_tiles);
        }

        private void AssignNeighbours()
        {
            foreach (var tile in _tiles.Values)
            {
                LevelTile TryGetSameHeightNeighbour(Vector3 offset)
                {
                    var neighbour = GetWorldTile(tile.WorldPosition + offset);
                    return (neighbour != null && neighbour.HeightLayer == tile.HeightLayer) ? neighbour : null;
                }

                tile.TileNeighbours = new LevelTileNeighbours(
                    upTile:    TryGetSameHeightNeighbour(new Vector3(0, tile.HeightSize, 0)),
                    downTile:  TryGetSameHeightNeighbour(new Vector3(0, -tile.HeightSize, 0)),
                    rightTile: TryGetSameHeightNeighbour(new Vector3(tile.WidthSize, 0, 0)),
                    leftTile:  TryGetSameHeightNeighbour(new Vector3(-tile.WidthSize, 0, 0))
                );
            }
        }


        private LevelTile GetWorldTile(Vector3 position) =>
            _tiles.GetValueOrDefault(_tilemapsFromTopToBottom.First().WorldToCell(position));

        private void GetTileFromPlayerInput(Vector3 pointInWorld)
        {
            var tileSelected = GetWorldTile(pointInWorld);

            mediator.TileExecuteAction(tileSelected);
        }

        private void OnDisable()
        {
            mediator.OnTileClicked -= GetTileFromPlayerInput;
        }
    }
}