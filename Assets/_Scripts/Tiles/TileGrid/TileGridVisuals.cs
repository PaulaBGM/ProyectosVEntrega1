using System.Collections.Generic;
using System.Linq;
using _Scripts.Core.Mediator;
using _Scripts.Occupants;
using UnityEngine;
using UnityEngine.Pool;

namespace _Scripts.Tiles.TileGrid
{
    public class TileGridVisuals : MediatorClientSystem<TileGridMediator>
    {
        [SerializeField]
        private GameObject actionMarkerPrefab;

        private ObjectPool<GameObject> _actionTileMarkerPool;

        private readonly List<GameObject> _actionTileMarkers = new();

        private static readonly Color OccupantColor = new(1f, 1f, 0f, 0.75f);
        private static readonly Color InteractorColor = new(0f, 0.5f, 1f, 0.75f);
        private static readonly Color DefaultColor = new(1f, 1f, 1f, 0.75f);

        protected override void Awake()
        {
            base.Awake();

            _actionTileMarkerPool = new ObjectPool<GameObject>(OnCreateMarker, OnGetMarker, OnReleaseMarker);
        }

        private GameObject OnCreateMarker()
        {
            Debug.Log("CREATE");

            var marker = Instantiate(actionMarkerPrefab, Vector3.zero, Quaternion.identity);

            _actionTileMarkers.Add(marker);

            Debug.Log($"CREATE -> {marker.GetInstanceID()} Pos:{marker.transform.position}");

            return marker;
        }

        private void OnGetMarker(GameObject marker)
        {
            Debug.Log($"GET BEFORE -> {marker.GetInstanceID()} Pos:{marker.transform.position}");

            marker.SetActive(true);

            Debug.Log($"GET AFTER -> {marker.GetInstanceID()} Pos:{marker.transform.position}");
        }

        private void OnReleaseMarker(GameObject marker)
        {
            Debug.Log($"RELEASE -> {marker.GetInstanceID()} Pos:{marker.transform.position}");

            marker.SetActive(false);
        }

        private void OnEnable()
        {
            mediator.OnMovementTilesSet += HighlightMovementTiles;
            mediator.OnTileClicked += HandleOnTileClicked;
        }

        private void OnDisable()
        {
            mediator.OnMovementTilesSet -= HighlightMovementTiles;
            mediator.OnTileClicked -= HandleOnTileClicked;
        }

        private void HighlightMovementTiles(IEnumerable<LevelTile> movementTiles)
        {
            Debug.Log("===== HighlightMovementTiles =====");

            HideHighlightTiles();

            foreach (var tile in movementTiles)
            {
                Debug.Log($"Tile: {tile.WorldPosition}");

                var marker = _actionTileMarkerPool.Get();

                Debug.Log($"AFTER GET -> {marker.GetInstanceID()} Pos:{marker.transform.position}");

                marker.transform.position = new Vector3(tile.WorldPosition.x + tile.TilemapMember.cellSize.x * 0.5f, tile.WorldPosition.y + tile.TilemapMember.cellSize.y * 0.5f, 0f);

                Debug.Log($"AFTER MOVE -> {marker.GetInstanceID()} Pos:{marker.transform.position}");

                HighlightOptions(marker, tile);
            }
        }

        private void HandleOnTileClicked(Vector3 _)
        {
            Debug.Log("Tile Clicked");

            HideHighlightTiles();
        }

        private void HideHighlightTiles()
        {
            Debug.Log($"HideHighlightTiles ({_actionTileMarkers.Count})");

            foreach (var marker in _actionTileMarkers.Where(m => m.activeSelf))
            {
                _actionTileMarkerPool.Release(marker);
            }
        }

        private void HighlightOptions(GameObject marker, LevelTile tile)
        {
            if (!marker.TryGetComponent(out SpriteRenderer spriteRenderer))
            {
                Debug.LogError("SpriteRenderer no encontrado");
                return;
            }

            spriteRenderer.color = tile.Occupant is IAIOccupant ? OccupantColor : tile.TileInteractor != null ? InteractorColor : DefaultColor;
        }
    }
}
