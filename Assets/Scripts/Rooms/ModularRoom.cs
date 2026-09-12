using System.Collections.Generic;
using Anubis.Core;
using UnityEngine;

namespace Anubis.Rooms
{
    public sealed class ModularRoom : MonoBehaviour
    {
        public RoomDefinition Definition { get; private set; }
        public readonly List<DoorController> Doors = new();
        public Vector2 Size { get; private set; }

        public void Build(RoomDefinition definition)
        {
            Definition = definition;
            Size = definition.Size;
            BuildFloor();
            BuildWalls();
            BuildDoors();
        }

        public void SetDoorsOpen(bool open)
        {
            foreach (var door in Doors)
            {
                door.SetOpen(open);
            }
        }

        void BuildFloor()
        {
            var floor = new GameObject("Floor");
            floor.transform.SetParent(transform, false);
            var renderer = floor.AddComponent<SpriteRenderer>();
            renderer.sprite = RuntimeSpriteFactory.CreateRect(new Color(0.42f, 0.33f, 0.18f), 128, 128);
            renderer.sortingOrder = GameSorting.Floor;
            floor.transform.localScale = new Vector3(Size.x, Size.y, 1f);
        }

        void BuildWalls()
        {
            var thickness = 0.7f;
            CreateWall("Wall_N", new Vector2(0f, Size.y * 0.5f + thickness * 0.5f), new Vector2(Size.x + thickness * 2f, thickness));
            CreateWall("Wall_S", new Vector2(0f, -Size.y * 0.5f - thickness * 0.5f), new Vector2(Size.x + thickness * 2f, thickness));
            CreateWall("Wall_E", new Vector2(Size.x * 0.5f + thickness * 0.5f, 0f), new Vector2(thickness, Size.y));
            CreateWall("Wall_W", new Vector2(-Size.x * 0.5f - thickness * 0.5f, 0f), new Vector2(thickness, Size.y));
        }

        void CreateWall(string name, Vector2 position, Vector2 size)
        {
            var wall = new GameObject(name);
            wall.transform.SetParent(transform, false);
            wall.transform.position = position;
            wall.layer = GameLayers.Environment;
            var renderer = wall.AddComponent<SpriteRenderer>();
            renderer.sprite = RuntimeSpriteFactory.CreateRect(new Color(0.16f, 0.1f, 0.07f), 64, 64);
            renderer.sortingOrder = GameSorting.Environment;
            wall.transform.localScale = new Vector3(size.x, size.y, 1f);
            var collider = wall.AddComponent<BoxCollider2D>();
            collider.size = Vector2.one;
        }

        void BuildDoors()
        {
            CreateDoor(DoorDirection.North, new Vector2(0f, Size.y * 0.5f - 0.15f), new Vector2(2.4f, 0.55f));
            CreateDoor(DoorDirection.South, new Vector2(0f, -Size.y * 0.5f + 0.15f), new Vector2(2.4f, 0.55f));
        }

        void CreateDoor(DoorDirection direction, Vector2 position, Vector2 size)
        {
            var doorObject = new GameObject($"Door_{direction}");
            doorObject.transform.SetParent(transform, false);
            doorObject.layer = GameLayers.Environment;
            var door = doorObject.AddComponent<DoorController>();
            door.Build(direction, position, size);
            Doors.Add(door);
        }
    }
}
