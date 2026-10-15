namespace RagApp.Application.Persistence
{ 
    using Microsoft.EntityFrameworkCore;  

    using RagApp.Domain.Entities;
     
    public class RagDbContext : DbContext
    { 
        public RagDbContext(DbContextOptions<RagDbContext> options) : base(options) { }
         
        public DbSet<User> Users => Set<User>();
        public DbSet<Document> Documents => Set<Document>();
        public DbSet<DocumentChunk> Chunks => Set<DocumentChunk>();
         
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { 
            modelBuilder.HasPostgresExtension("vector");
             
            modelBuilder.Entity<User>(e =>
            {
                e.HasKey(x => x.Id); 
                e.HasIndex(x => x.Nickname).IsUnique();
                e.HasIndex(x => x.Email).IsUnique(); 
                e.Property(x => x.Nickname).HasMaxLength(50).IsRequired();
                e.Property(x => x.Email).HasMaxLength(200).IsRequired();
                e.Property(x => x.AvatarUrl).HasMaxLength(500);
                e.Property(x => x.FirstName).HasMaxLength(100);
                e.Property(x => x.LastName).HasMaxLength(100);
                e.Property(x => x.Country).HasMaxLength(100);
                e.Property(x => x.City).HasMaxLength(100);
            });
             
            modelBuilder.Entity<Document>(e =>
            {
                e.HasKey(x => x.Id); 
                e.HasOne(x => x.User)
                 .WithMany(u => u.Documents)
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Cascade);
            });
             
            modelBuilder.Entity<DocumentChunk>(e =>
            {
                e.HasKey(x => x.Id); 
                e.Property(x => x.Embedding).HasColumnType("vector(768)");
                 
                e.HasOne(x => x.Document)
                 .WithMany(d => d.Chunks)
                 .HasForeignKey(x => x.DocumentId)
                 .OnDelete(DeleteBehavior.Cascade); 
                e.HasIndex(x => x.Embedding)
                 .HasMethod("hnsw")
                 .HasOperators("vector_cosine_ops");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
