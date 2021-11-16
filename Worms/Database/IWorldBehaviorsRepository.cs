namespace Worms.Database {
    internal interface IWorldBehaviorsRepository {
        WorldBehavior? GetByName(string name);
        void Add(WorldBehavior worldBehavior);
    }
}