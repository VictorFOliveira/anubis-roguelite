using System;
using System.Collections.Generic;
using Anubis.Core;
using UnityEngine;

namespace Anubis.Runner
{
    public enum RunnerBiome
    {
        Giza,
        Luxor,
        AbuSimbel,
        Ur
    }

    public sealed class RunnerWorldGenerator : MonoBehaviour
    {
        const float ChunkWidth = 24f;
        const float GroundY = -3.25f;
        const float GroundHeight = 1.2f;
        const int BiomeSpanChunks = 4;

        readonly Queue<ChunkRecord> _chunks = new();
        Transform _player;
        RunnerBackdrop _backdrop;
        Sprite _unit;
        int _nextChunkIndex;
        RunnerBiome _currentBiome;
        bool _hasBiome;

        public event Action<RunnerBiome> BiomeChanged;
        public RunnerBiome CurrentBiome => _currentBiome;

        public void Configure(RunnerBackdrop backdrop)
        {
            _backdrop = backdrop;
            _unit = RuntimeSpriteFactory.CreateRect(Color.white, 64, 64);
        }

        public void BuildInitial()
        {
            _nextChunkIndex = 0;
            for (var i = 0; i < 8; i++)
            {
                SpawnChunk(_nextChunkIndex++);
            }

            SetBiome(RunnerBiome.Giza);
        }

        public void Track(Transform player)
        {
            _player = player;
        }

        void Update()
        {
            if (_player == null)
            {
                return;
            }

            while (_nextChunkIndex * ChunkWidth < _player.position.x + 150f)
            {
                SpawnChunk(_nextChunkIndex++);
            }

            while (_chunks.Count > 0 && _chunks.Peek().EndX < _player.position.x - 45f)
            {
                var old = _chunks.Dequeue();
                if (old.Root != null)
                {
                    Destroy(old.Root);
                }
            }

            var chunkIndex = Mathf.Max(0, Mathf.FloorToInt(_player.position.x / ChunkWidth));
            var biome = (RunnerBiome)((chunkIndex / BiomeSpanChunks) % 4);
            SetBiome(biome);
        }

        void SetBiome(RunnerBiome biome)
        {
            if (_hasBiome && _currentBiome == biome)
            {
                return;
            }

            _hasBiome = true;
            _currentBiome = biome;
            _backdrop?.SetBiome(biome);
            BiomeChanged?.Invoke(biome);
        }

        void SpawnChunk(int index)
        {
            var startX = index * ChunkWidth;
            var biome = (RunnerBiome)((index / BiomeSpanChunks) % 4);
            var root = new GameObject($"Chunk_{index:000}_{biome}");
            root.transform.SetParent(transform, false);

            var pattern = index == 0 ? 0 : Mathf.Abs((index * 17 + index / 3) % 4);
            switch (pattern)
            {
                case 0:
                    CreateGround(root.transform, startX, ChunkWidth, biome);
                    if (index > 0) SpawnEnemy(root.transform, startX + 11f, index);
                    break;
                case 1:
                    CreateGround(root.transform, startX, 8.5f, biome);
                    CreateGround(root.transform, startX + 12f, 12f, biome);
                    SpawnEnemy(root.transform, startX + 17f, index);
                    break;
                case 2:
                    CreateGround(root.transform, startX, ChunkWidth, biome);
                    CreateObstacle(root.transform, startX + 9f, 1.2f, 1.35f, biome);
                    CreateObstacle(root.transform, startX + 16f, 1.5f, 1.8f, biome);
                    break;
                default:
                    CreateGround(root.transform, startX, ChunkWidth, biome);
                    SpawnEnemy(root.transform, startX + 8f, index);
                    SpawnEnemy(root.transform, startX + 17f, index + 1);
                    break;
            }

            if (index > 0 && index % 2 == 0)
            {
                var upgrade = (RunnerUpgradeType)((index / 2) % 3);
                SpawnUpgrade(root.transform, startX + 19.5f, upgrade);
            }

            _chunks.Enqueue(new ChunkRecord(root, startX + ChunkWidth));
        }

        void CreateGround(Transform parent, float startX, float width, RunnerBiome biome)
        {
            var go = new GameObject("Ground");
            go.layer = GameLayers.Environment;
            go.transform.SetParent(parent, false);
            go.transform.position = new Vector3(startX + width * 0.5f, GroundY, 0f);
            go.transform.localScale = new Vector3(width, GroundHeight, 1f);

            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = _unit;
            renderer.color = GroundColor(biome);
            renderer.sortingOrder = GameSorting.Floor;

            var collider = go.AddComponent<BoxCollider2D>();
            collider.size = Vector2.one;
        }

        void CreateObstacle(Transform parent, float x, float width, float height, RunnerBiome biome)
        {
            var go = new GameObject("RuinedBlock");
            go.layer = GameLayers.Environment;
            go.transform.SetParent(parent, false);
            go.transform.position = new Vector3(x, GroundY + GroundHeight * 0.5f + height * 0.5f, 0f);
            go.transform.localScale = new Vector3(width, height, 1f);

            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = _unit;
            renderer.color = GroundColor(biome) * 0.82f;
            renderer.sortingOrder = GameSorting.Environment;

            go.AddComponent<BoxCollider2D>().size = Vector2.one;
        }

        static void SpawnEnemy(Transform parent, float x, int seed)
        {
            var go = new GameObject(seed % 2 == 0 ? "ScarabRunner" : "DesertGuard");
            go.transform.SetParent(parent, false);
            go.transform.position = new Vector3(x, -2.08f, 0f);
            var enemy = go.AddComponent<RunnerEnemy>();
            enemy.Configure(seed % 2 == 0 ? RunnerEnemyStyle.Scarab : RunnerEnemyStyle.Guard);
        }

        static void SpawnUpgrade(Transform parent, float x, RunnerUpgradeType type)
        {
            var go = new GameObject($"Upgrade_{type}");
            go.transform.SetParent(parent, false);
            go.transform.position = new Vector3(x, -0.8f, 0f);
            go.AddComponent<RunnerUpgradePickup>().Configure(type);
        }

        static Color GroundColor(RunnerBiome biome)
        {
            return biome switch
            {
                RunnerBiome.Giza => new Color(0.66f, 0.43f, 0.21f),
                RunnerBiome.Luxor => new Color(0.55f, 0.34f, 0.17f),
                RunnerBiome.AbuSimbel => new Color(0.48f, 0.29f, 0.16f),
                RunnerBiome.Ur => new Color(0.46f, 0.31f, 0.19f),
                _ => new Color(0.6f, 0.4f, 0.2f)
            };
        }

        readonly struct ChunkRecord
        {
            public readonly GameObject Root;
            public readonly float EndX;

            public ChunkRecord(GameObject root, float endX)
            {
                Root = root;
                EndX = endX;
            }
        }
    }
}
