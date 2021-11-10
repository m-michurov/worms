using Microsoft.EntityFrameworkCore;

namespace Worms.Database {
    public class WorldBehaviorContext : DbContext {
        public WorldBehaviorContext(DbContextOptions<WorldBehaviorContext> options) : base(options) { }
        
        public DbSet<WorldBehavior>? WorldBehaviors { get; set; }
        
        // ReSharper disable once UnusedMember.Global
        public DbSet<FoodPosition>? FoodPositions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<FoodPosition>()
                .Property(position => position.WorldBehaviorId)
                .HasColumnName("WORLD_BEHAVIOR_ID");
            
            modelBuilder.Entity<FoodPosition>()
                .HasOne<WorldBehavior>()
                .WithMany(behavior => behavior.FoodPositions)
                .HasForeignKey(position => position.WorldBehaviorId);
            
            modelBuilder.Entity<FoodPosition>()
                .HasKey(position => new {Id = position.WorldBehaviorId, position.Step});
        }
    }
}