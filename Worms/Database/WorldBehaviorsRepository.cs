using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Worms.Database {
    internal sealed class WorldBehaviorsRepository : IWorldBehaviorsRepository {
        private readonly MainContext context;

        public WorldBehaviorsRepository(MainContext context_) => context = context_;

        private IEnumerable<WorldBehavior> WorldBehaviors =>
            context.WorldBehaviors!.Include(behaviour => behaviour.FoodPositions);

        public WorldBehavior? GetByName(string name) {
            var behaviours =
                from behaviour in WorldBehaviors
                where name == behaviour.Name
                select behaviour;
            try {
                return behaviours.First();
            } catch (InvalidOperationException) {
                return null;
            }
        }

        public void Add(WorldBehavior worldBehavior) {
            context.Add(worldBehavior);
            context.SaveChanges();
        }
    }
}