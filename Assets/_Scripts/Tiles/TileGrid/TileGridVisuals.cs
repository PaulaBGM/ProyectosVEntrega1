using System.Collections.Generic;
using System.Linq;
using _Scripts.Core.Mediator;
using _Scripts.Occupants;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Tilemaps;
using UnityEngine.Timeline;

namespace _Scripts.Tiles.TileGrid
{
    public class TileGridVisuals : MediatorClientSystem<TileGridMediator>
    {
        [SerializeField]
        private GameObject actionMarkerPrefab;

        private ObjectPool<GameObject> _actionTileMarkerPool;

        private readonly List<GameObject> _actionTileMarkers = new();
        
        private static readonly Color OccupantColor = new Color(1f, 1f, 0f, 0.75f); 
        private static readonly Color InteractorColor = new Color(0f, 0.5f, 1f, 0.75f); 
        private static readonly Color DefaultColor = new Color(1f, 1f, 1f, 0.75f);
        
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
            
            foreach (var tile in tilesHighlighted)
            {
                var marker = _actionTileMarkerPool.Get();
                marker.transform.position = new Vector3(
                    tile.WorldPosition.x + tile.TilemapMember.cellSize.x * 0.5f,
                    tile.WorldPosition.y +tile.TilemapMember.cellSize.y * 0.5f,
                    transform.position.z);
                
                HighlightOptions(marker, tile);
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

        private void HighlightOptions(GameObject marker, LevelTile tile)
        {
            if (!marker.TryGetComponent<SpriteRenderer>(out var spriteRenderer))
            {
                Debug.LogWarning($"Marker {marker.name} no tiene SpriteRenderer");
                return;
            }

            spriteRenderer.color = tile.Occupant is IAIOccupant ? OccupantColor  :
                tile.TileInteractor != null ? InteractorColor :
                DefaultColor;
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
