using System.Collections.Generic;
using Anubis.AI;
using Anubis.Characters;
using Anubis.Combat;
using Anubis.Core;
using UnityEngine;

namespace Anubis.Rooms
{
    public sealed class EncounterDirector : MonoBehaviour
    {
        readonly List<EnemyController> _alive = new();
        RoomDefinition _room;
        PlayerController _player;
        GameSignals _signals;
        ObjectPool _pool;
        int _waveIndex;
        bool _running;

        public bool IsCleared { get; private set; }
        public int AliveCount => _alive.Count;
        public int WaveIndex => _waveIndex;

        public void Configure(RoomDefinition room, PlayerController player, GameSignals signals, ObjectPool pool)
        {
            _room = room;
            _player = player;
            _signals = signals;
            _pool = pool;
            _signals.EnemyKilled.Subscribe(OnEnemyKilled);
        }

        public void StartEncounter()
        {
            _running = true;
            IsCleared = false;
            _waveIndex = 0;
            _signals.EncounterStarted.Raise();
            SpawnWave(0);
        }

        void OnDestroy()
        {
            _signals?.EnemyKilled.Unsubscribe(OnEnemyKilled);
        }

        void OnEnemyKilled(EnemyKillInfo _)
        {
            _alive.RemoveAll(enemy => enemy == null || !enemy.Health.IsAlive);
            if (!_running || _alive.Count > 0)
            {
                return;
            }

            _waveIndex++;
            if (_room.Waves != null && _waveIndex < _room.Waves.Length)
            {
                SpawnWave(_waveIndex);
                return;
            }

            _running = false;
            IsCleared = true;
            _signals.EncounterCleared.Raise();
        }

        void SpawnWave(int index)
        {
            if (_room.Waves == null || index >= _room.Waves.Length)
            {
                return;
            }

            foreach (var spawn in _room.Waves[index].Spawns)
            {
                if (spawn?.Enemy == null)
                {
                    continue;
                }

                var enemyObject = new GameObject(spawn.Enemy.DisplayName);
                enemyObject.transform.SetParent(transform, false);
                enemyObject.transform.localPosition = spawn.LocalPosition;
                var enemy = enemyObject.AddComponent<EnemyController>();
                enemy.Bind(spawn.Enemy, _player.Health, _signals, _pool);
                _alive.Add(enemy);
            }
        }
    }
}
