using System.Reflection;

using BookingCalendarApi.Repository;

namespace BookingCalendarApi.UnitTests.Infrastructure
{
    /// <summary>
    /// In-memory <see cref="IRepository"/> that mimics the EF Core behaviour the services rely on:
    /// <list type="bullet">
    /// <item>queried entities are tracked copies (one instance per key), changes to them are persisted only by <see cref="SaveChangesAsync"/>;</item>
    /// <item><see cref="Add{TEntity}"/>, <see cref="Update{TEntity}"/> and <see cref="Remove"/> are applied only by <see cref="SaveChangesAsync"/>;</item>
    /// <item>numeric ids are generated on save and written back to the added instance;</item>
    /// <item>everything is detached after saving, like the production repository does.</item>
    /// </list>
    /// Tenant (structure) filtering is not emulated: it is covered by the integration tests.
    /// </summary>
    public class InMemoryRepository : IRepository
    {
        private enum Operation { Add, Update, Remove }

        private static readonly MethodInfo CloneMethod = typeof(object).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic)!;

        private readonly Dictionary<Type, List<object>> _store = new();
        private readonly Dictionary<(Type, object), object> _tracked = new();
        private readonly List<(Operation Operation, object Entity)> _pending = new();

        public int SaveChangesCalls { get; private set; }

        public IQueryable<Structure> Structures => Query<Structure>();
        public IQueryable<User> Users => Query<User>();
        public IQueryable<UserRefreshToken> UserRefreshTokens => Query<UserRefreshToken>();
        public IQueryable<Nation> Nations => Query<Nation>();
        public IQueryable<Room> Rooms => Query<Room>();
        public IQueryable<RoomAssignment> RoomAssignments => Query<RoomAssignment>();
        public IQueryable<ColorAssignment> ColorAssignments => Query<ColorAssignment>();

        public IQueryable<Floor> Floors
        {
            get
            {
                var floors = Query<Floor>().ToList();
                var rooms = Query<Room>().ToList();
                foreach (var floor in floors)
                {
                    floor.Rooms = rooms.Where(room => room.FloorId == floor.Id).ToList();
                }
                return floors.AsQueryable();
            }
        }

        /// <summary>Puts an entity straight into the committed store (test arrangement).</summary>
        public TEntity Seed<TEntity>(TEntity entity) where TEntity : class
        {
            AssignIdIfMissing(entity);
            if (Find(typeof(TEntity), KeyOf(entity)) != null)
            {
                throw new InvalidOperationException($"{typeof(TEntity).Name} with key {KeyOf(entity)} already seeded");
            }
            StoreOf(typeof(TEntity)).Add(Detached(entity));
            return entity;
        }

        /// <summary>Committed state of an entity set, as copies (assertions).</summary>
        public List<TEntity> Stored<TEntity>() where TEntity : class
            => StoreOf(typeof(TEntity)).Select(e => (TEntity)Clone(e)).ToList();

        public TEntity Add<TEntity>(TEntity entity) where TEntity : class
        {
            _pending.Add((Operation.Add, entity));
            return entity;
        }

        public TEntity Update<TEntity>(TEntity entity) where TEntity : class
        {
            _pending.Add((Operation.Update, entity));
            return entity;
        }

        public void Remove(object entity)
        {
            _pending.Add((Operation.Remove, entity));
        }

        public Task SaveChangesAsync()
        {
            SaveChangesCalls++;

            foreach (var ((type, key), entity) in _tracked)
            {
                Replace(type, key, entity);
            }

            foreach (var (operation, entity) in _pending)
            {
                var type = entity.GetType();
                switch (operation)
                {
                    case Operation.Add:
                        AssignIdIfMissing(entity);
                        if (Find(type, KeyOf(entity)) != null)
                        {
                            throw new InvalidOperationException($"Duplicate key {KeyOf(entity)} for {type.Name}");
                        }
                        StoreOf(type).Add(Detached(entity));
                        break;
                    case Operation.Update:
                        Replace(type, KeyOf(entity), entity);
                        break;
                    case Operation.Remove:
                        var existing = Find(type, KeyOf(entity))
                            ?? throw new InvalidOperationException($"{type.Name} with key {KeyOf(entity)} does not exist");
                        StoreOf(type).Remove(existing);
                        break;
                }
            }

            _pending.Clear();
            _tracked.Clear();
            return Task.CompletedTask;
        }

        private IQueryable<TEntity> Query<TEntity>() where TEntity : class
        {
            return StoreOf(typeof(TEntity))
                .Select(stored =>
                {
                    var key = (typeof(TEntity), KeyOf(stored));
                    if (!_tracked.TryGetValue(key, out var tracked))
                    {
                        tracked = Clone(stored);
                        _tracked[key] = tracked;
                    }
                    return (TEntity)tracked;
                })
                .ToList()
                .AsQueryable();
        }

        private void Replace(Type type, object key, object entity)
        {
            var store = StoreOf(type);
            var existing = Find(type, key)
                ?? throw new InvalidOperationException($"{type.Name} with key {key} does not exist");
            store[store.IndexOf(existing)] = Detached(entity);
        }

        private object? Find(Type type, object key) => StoreOf(type).SingleOrDefault(e => KeyOf(e).Equals(key));

        private List<object> StoreOf(Type type)
        {
            if (!_store.TryGetValue(type, out var list))
            {
                list = new List<object>();
                _store[type] = list;
            }
            return list;
        }

        private void AssignIdIfMissing(object entity)
        {
            // behaves like an auto increment column: past the highest id in the set
            var idProperty = entity.GetType().GetProperty("Id");
            if (idProperty?.PropertyType == typeof(long) && (long)idProperty.GetValue(entity)! == 0)
            {
                var ids = StoreOf(entity.GetType()).Select(e => (long)idProperty.GetValue(e)!);
                idProperty.SetValue(entity, ids.DefaultIfEmpty(0).Max() + 1);
            }
        }

        private static object Detached(object entity)
        {
            var copy = Clone(entity);
            if (copy is Floor floor)
            {
                // rooms live in their own set
                floor.Rooms = new List<Room>();
            }
            return copy;
        }

        private static object Clone(object entity) => CloneMethod.Invoke(entity, null)!;

        private static object KeyOf(object entity) => entity switch
        {
            ColorAssignment colorAssignment => colorAssignment.BookingId,
            Nation nation => nation.Iso,
            _ => entity.GetType().GetProperty("Id")!.GetValue(entity)!,
        };
    }
}
