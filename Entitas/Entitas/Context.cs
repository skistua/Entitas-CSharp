﻿using System;
using System.Collections.Generic;
using ToolKit;

namespace Entitas {
    public partial class Context {
        readonly HashSet<Entity> _entities = new HashSet<Entity>(EntityEqualityComparer.comparer);
        readonly Dictionary<IMatcher, Group> _groups = new Dictionary<IMatcher, Group>();
        readonly List<Group>[] _groupsForIndex;
        readonly ObjectPool<Entity> _entityPool;
        readonly int _totalComponents;
        int _creationIndex;
        Entity[] _entitiesCache;

        public Context(int totalComponents) : this(totalComponents, 0) {
        }

        public Context(int totalComponents, int startCreationIndex) {
            _totalComponents = totalComponents;
            _creationIndex = startCreationIndex;
            _groupsForIndex = new List<Group>[totalComponents];
            _entityPool = new ObjectPool<Entity>(() => new Entity(_totalComponents));
        }

        public Entity CreateEntity() {
            var entity = _entityPool.Get();
            entity.creationIndex = _creationIndex++;
            _entities.Add(entity);
            _entitiesCache = null;
            entity.OnComponentAdded += onComponentAdded;
            entity.OnComponentReplaced += onComponentReplaced;
            entity.OnComponentWillBeRemoved += onComponentWillBeRemoved;
            entity.OnComponentRemoved += onComponentRemoved;
            return entity;
        }

        public void DestroyEntity(Entity entity) {
            entity.RemoveAllComponents();
            entity.OnComponentAdded -= onComponentAdded;
            entity.OnComponentReplaced -= onComponentReplaced;
            entity.OnComponentWillBeRemoved -= onComponentWillBeRemoved;
            entity.OnComponentRemoved -= onComponentRemoved;
            _entities.Remove(entity);
            _entitiesCache = null;
            _entityPool.Push(entity);
        }

        public void DestroyAllEntities() {
            var entities = GetEntities();
            foreach (var entity in entities) {
                DestroyEntity(entity);
            }
        }

        public bool HasEntity(Entity entity) {
            return _entities.Contains(entity);
        }

        public Entity[] GetEntities() {
            if (_entitiesCache == null) {
                _entitiesCache = new Entity[_entities.Count];
                _entities.CopyTo(_entitiesCache);
            }

            return _entitiesCache;
        }

        public Group GetGroup(IMatcher matcher) {
            Group group;
            if (!_groups.TryGetValue(matcher, out group)) {
                group = new Group(matcher);
                foreach (var entity in _entities) {
                    group.AddEntityIfMatching(entity);
                }
                _groups.Add(matcher, group);

                for (int i = 0, indicesLength = matcher.indices.Length; i < indicesLength; i++) {
                    var index = matcher.indices[i];
                    if (_groupsForIndex[index] == null) {
                        _groupsForIndex[index] = new List<Group>();
                    }
                    _groupsForIndex[index].Add(group);
                }
            }

            return group;
        }

        void onComponentAdded(Entity entity, int index, IComponent component) {
            var groups = _groupsForIndex[index];
            if (groups != null) {
                for (int i = 0, groupsCount = groups.Count; i < groupsCount; i++) {
                    groups[i].AddEntityIfMatching(entity);
                }
            }
        }

        void onComponentReplaced(Entity entity, int index, IComponent component) {
            var groups = _groupsForIndex[index];
            if (groups != null) {
                for (int i = 0, groupsCount = groups.Count; i < groupsCount; i++) {
                    groups[i].UpdateEntity(entity);
                }
            }
        }

        void onComponentWillBeRemoved(Entity entity, int index, IComponent component) {
            var groups = _groupsForIndex[index];
            if (groups != null) {
                for (int i = 0, groupsCount = groups.Count; i < groupsCount; i++) {
                    groups[i].WillRemoveEntity(entity);
                }
            }
        }

        void onComponentRemoved(Entity entity, int index, IComponent component) {
            var groups = _groupsForIndex[index];
            if (groups != null) {
                for (int i = 0, groupsCount = groups.Count; i < groupsCount; i++) {
                    groups[i].RemoveEntity(entity);
                }
            }
        }
    }
}
