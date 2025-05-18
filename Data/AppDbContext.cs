    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Vedect.Models.Domain;

    namespace Vedect.Data
    {
        public class AppDbContext : IdentityDbContext<User>
        {
            public AppDbContext(DbContextOptions<AppDbContext> options)
                : base(options)
            {
            }

            public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
            
            public DbSet<UserPlanRequests> UserPlanRequests { get; set; }

            public DbSet<Camera> Cameras { get; set; }

            public DbSet<UserCamera> UserCameras { get; set; }

            public DbSet<CameraStreamSession> CameraStreamsSessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SubscriptionPlan>()
                .Property(p => p.Id)
                .ValueGeneratedNever();

            modelBuilder.Entity<SubscriptionPlan>().HasData(
                    new SubscriptionPlan
                    {
                        Id = 0,
                        Name = "UnSubscribed",
                        EnableStreaming = false,
                        EnableFullStreamStorage = false,
                        EnableAIDetection = false,
                        EnableAIChunkStorage = false,
                        FullStreamRetentionHours = 0,
                        AIChunkRetentionHours = 0,
                        MaxTotalStorageMB = 0
                    },
                    new SubscriptionPlan
                    {
                        Id = 1,
                        Name = "Common",
                        EnableStreaming = true,
                        EnableFullStreamStorage = true,
                        EnableAIDetection = false,
                        EnableAIChunkStorage = false,
                        FullStreamRetentionHours = 24,
                        AIChunkRetentionHours = 0,
                        MaxTotalStorageMB = 1024
                    },
                    new SubscriptionPlan
                    {
                        Id = 2,
                        Name = "Plus",
                        EnableStreaming = false,
                        EnableFullStreamStorage = false,
                        EnableAIDetection = true,
                        EnableAIChunkStorage = true,
                        FullStreamRetentionHours = 0,
                        AIChunkRetentionHours = 1,
                        MaxTotalStorageMB = 512
                    },
                    new SubscriptionPlan
                    {
                        Id = 3,
                        Name = "Premium",
                        EnableStreaming = true,
                        EnableFullStreamStorage = true,
                        EnableAIDetection = true,
                        EnableAIChunkStorage = true,
                        FullStreamRetentionHours = 72,
                        AIChunkRetentionHours = 12,
                        MaxTotalStorageMB = 4096
                    }
                );
            }
        }
    }
