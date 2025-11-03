using System.Collections.Generic;
using System.Linq;
using _Scripts.Core.Mediator;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Tilemaps;

namespace _Scripts.Tiles.TileGrid
{
    public class TileGridVisuals : MediatorClientSystem<TileGridMediator>
    {
        [SerializeField]
        private GameObject actionMarkerPrefab;
        
        private IEnumerable<LevelTile> _tilesHighlighted = new List<LevelTile>();

        private ObjectPool<GameObject> _actionTileMarkerPool;

        private readonly List<GameObject> _actionTileMarkers = new();
        
        protected override void Awake()
        {
            base.Awake();
            _actionTileMarkerPool = new ObjectPool<GameObject>(OnCreateMarker, OnGetMarker, OnReleaseMarker);
        }

        private GameObject OnCreateMarker()
        {
            var markerCopy = Instantiate(actionMarkerPrefab, Vector3.zero, Quaternion.identity);
            _actionTileMarkers.Add(markerCopy);
            return markerCopy;
        }
        
        private void OnGetMarker(GameObject marker)
        {
            marker.gameObject.SetActive(true);
        }
        
        private void OnReleaseMarker(GameObject marker)
        {
            marker.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            mediator.OnMovementTilesSet += HighlightMovementTiles;
            mediator.OnTileClicked += HandleOnTileClicked;
        }

        private void HighlightMovementTiles(IEnumerable<LevelTile> movementTiles)
        {
            HideHighlightTiles();
            var tilesHighlighted = movementTiles as LevelTile[] ?? movementTiles.ToArray();
            _tilesHighlighted = tilesHighlighted;
            
            foreach (var tile in tilesHighlighted)
            {
                Debug.Log("AAA");
                var marker = _actionTileMarkerPool.Get();
                marker.transform.position = new Vector3(
                    tile.WorldPosition.x + tile.TilemapMember.cellSize.x * 0.5f,
                    tile.WorldPosition.y +tile.TilemapMember.cellSize.y * 0.5f,
                    transform.position.z);
            }
        }
        private void HandleOnTileClicked(Vector3 _)
        {
            HideHighlightTiles();
        }

        private void HideHighlightTiles()
        {
            foreach (var marker in _actionTileMarkers.Where(m => m.activeSelf))
            {
                _actionTileMarkerPool.Release(marker);
            }
        }
        
        // private void HighlightMovementTiles(IEnumerable<LevelTile> movementTiles)
        // {
        //     var tilesHighlighted = movementTiles as LevelTile[] ?? movementTiles.ToArray();
        //     _tilesHighlighted = tilesHighlighted;
        //     
        //     foreach (var tile in tilesHighlighted)
        //     {
        //         tile.TilemapMember.SetTileFlags(tile.LocalPosition, TileFlags.LockTransform);
        //         tile.TilemapMember.SetColor(tile.LocalPosition, Color.blue);
        //     }
        // }
        //
        // private void HideHighlightMovementTiles(Vector3 _)
        // {
        //     foreach (var tile in _tilesHighlighted)
        //     {
        //         tile.TilemapMember.SetTileFlags(tile.LocalPosition, TileFlags.LockTransform);
        //         tile.TilemapMember.SetColor(tile.LocalPosition, Color.white);
        //     }
        // }

        private void OnDisable()
        {
            mediator.OnMovementTilesSet -= HighlightMovementTiles;
            mediator.OnTileClicked -= HandleOnTileClicked;
        }
    }
}
