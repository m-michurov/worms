using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Worms.Database {
    internal sealed class WorldBehaviorsRepository {
        private readonly WorldBehaviorContext context;

        public WorldBehaviorsRepository(WorldBehaviorContext context_) => context = context_;

        private IEnumerable<WorldBehavior> WorldBehaviors =>
            context.WorldBehaviors!.Include(behaviour => behaviour.FoodPositions);

        internal WorldBehavior? GetByName(string name) {
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

        internal void Add(WorldBehavior worldBehavior) {
            context.Add(worldBehavior);
            context.SaveChanges();
        }
    }
}