using Microsoft.EntityFrameworkCore;

namespace AchievementTracker.Models
{
    public class AchievementTrackerDB : DbContext
    {
        public AchievementTrackerDB(DbContextOptions<AchievementTrackerDB> options) : base(options) { }
        

        public DbSet<Game> Games { get; set; } 
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<TrophyTier> TrophyTiers { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<AchievementTag> AchievementTags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AchievementTag>().HasKey(at => new { at.AchievementId, at.TagId });

            // Achievement -> AchievementTag Bağlantısı
            modelBuilder.Entity<AchievementTag>()
                .HasOne(at => at.Achievement)
                .WithMany(a => a.AchievementTags)
                .HasForeignKey(at => at.AchievementId);

            // Tag -> AchievementTag Bağlantısı
            modelBuilder.Entity<AchievementTag>()
                .HasOne(at => at.Tag)
                .WithMany(t => t.AchievementTags)
                .HasForeignKey(at => at.TagId);
        }
}

    public class DbCreator
    {
        
        public static void Init(AchievementTrackerDB context)
        {
            // Veritabanı SQL'de henüz yoksa, tablolarla birlikte sıfırdan oluşturur. (Yoksa yarat şalteri)
            context.Database.Migrate();

            if (!context.TrophyTiers.Any())
            {
                var tiers = new TrophyTier[]
                {
                    // IconPath sadece base (varsayılan) ikonların yolunu tutuyor
                new TrophyTier { Name = "Tier 1", IconPath = "/TrophyIcons/base/tier-1.png" },
                new TrophyTier { Name = "Tier 2", IconPath = "/TrophyIcons/base/tier-2.png" },
                new TrophyTier { Name = "Tier 3", IconPath = "/TrophyIcons/base/tier-3.png" },
                new TrophyTier { Name = "Tier 4", IconPath = "/TrophyIcons/base/tier-4.png" },
                new TrophyTier { Name = "Tier 5", IconPath = "/TrophyIcons/base/tier-5.png" },
                new TrophyTier { Name = "Tier 6", IconPath = "/TrophyIcons/base/tier-6.png" }
                };
                context.TrophyTiers.AddRange(tiers);
            }


            var storyTag = new Tag { Name = "Hikaye" };
            var missableTag = new Tag { Name = "Kaçırılabilir" };
            var collectibleTag = new Tag { Name = "Toplanabilir" };
            var grindTag = new Tag { Name = "Grind" };
            var onlineTag = new Tag { Name = "Online/Çok Oyunculu" };
            var chapterSpecTag = new Tag { Name = "Bölüme Özel" };
            var coopTag = new Tag { Name = "Co-op/Eşli" };
            var combatTag = new Tag { Name = "Combat" };
            var miscTag = new Tag { Name = "Diğer" };

            // Eğer veritabanında halihazırda oyun varsa işlemi durdurur ve aynı oyunların tekrar eklenmesini önler.
            if (context.Games.Any())
            {
                return;
            }
            var game = new Game[]
            {
                new Game
                {
                    Name = "Detroit Become Human",
                    Platform = "Steam",
                    ReleaseDate = new DateTime(2018,05,25),
                    IconFolderName = "detroit-become-human",
                    UrlName = "detroit-become-human",
                    CoverImageUrl = "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1222140/library_600x900_2x.jpg",

                    Achievements = new List<Achievement>
                    {
                        new Achievement
                        {
                            Name = "BU BENİM HİKAYEM",
                            Description = "Oyunu bir kez bitir",
                            TrophyTierId = 1,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = storyTag }
                            }
                        },
                        new Achievement 
                        {
                            Name = "TEŞEKKÜRLER",
                            Description = "İlk bölümü oyna",
                            TrophyTierId = 1,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = storyTag },
                                new() { Tag = chapterSpecTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "KİTAP KURDU",
                            Description = "Oyundaki her dergiyi bul",
                            TrophyTierId = 5,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = collectibleTag },
                                new() { Tag = missableTag },
                                new() { Tag = grindTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "SIRADAN BİR MAKİNE",
                            Description = "Hank, Connor'ı öldürdü",
                            TrophyTierId = 3,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = chapterSpecTag },
                                new() { Tag = missableTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "NAMAĞLUP",
                            Description = "Sona ulaşana kadar hiçbir mücadeleyi kaybetme",
                            TrophyTierId = 4,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = combatTag },
                                new() { Tag = missableTag }
                            }
                        },
                    }
                },
                new Game
                {
                    Name = "The Elder Scrolls V: Skyrim",
                    Platform = "Steam",
                    ReleaseDate = new DateTime(2011, 11, 11),
                    IconFolderName = "skyrim",
                    UrlName = "the-elder-scrolls-v-skyrim",
                    CoverImageUrl = "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/489830/library_600x900_2x.jpg",
                    Achievements = new List<Achievement>
                    {
                        new Achievement
                        {
                            Name = "Unbound",
                            Description = "Unbound görevini tamamla",
                            TrophyTierId = 1,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = storyTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Oblivion Walker",
                            Description = "15 Daedric Artifact (Daedrik Eser) topla",
                            TrophyTierId = 3,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = collectibleTag },
                                new() { Tag = missableTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Legend",
                            Description = "Bir Efsanevi Ejderha (Legendary Dragon) yen",
                            TrophyTierId = 5,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = combatTag },
                                new() { Tag = grindTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Master Criminal",
                            Description = "Tüm 9 şehirde aynı anda 1000 altın ödüle (Bounty) ulaş",
                            TrophyTierId = 2,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = miscTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Thief",
                            Description = "50 kilit aç ve 50 kişinin cebinden eşya çal (Pickpocket)",
                            TrophyTierId = 5,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = grindTag },
                                new() { Tag = miscTag }
                            }
                        }
                    }
                },
                new Game
                {
                    Name = "Test Game",
                    Platform = "Any",
                    ReleaseDate = new DateTime(2026, 08, 20),
                    IconFolderName = "test-game",
                    UrlName = "test-game",
                    CoverImageUrl = "https://yt3.googleusercontent.com/KI9pQTv4fCFVj7GMLa0QDgHf8obRtHOGg7hLEGhurPS6C6DgBtIWSmx173mWVoGVCXA45d1eMw=s900-c-k-c0x00ffffff-no-rj",
                    Achievements = new List<Achievement>
                    {
                        new Achievement
                        {
                            Name = "Achievement 1",
                            Description = "First Achievement",
                            TrophyTierId = 1,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = storyTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Achievement 2",
                            Description = "Second Achievement",
                            TrophyTierId = 2,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = collectibleTag },
                                new() { Tag = missableTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Achievement 3",
                            Description = "Third Achievement",
                            TrophyTierId = 3,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = combatTag },
                                new() { Tag = grindTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Achievement 4",
                            Description = "Fourth Achievement",
                            TrophyTierId = 4,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = miscTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Achievement 5",
                            Description = "Fifth Achievement",
                            TrophyTierId = 5,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = grindTag },
                                new() { Tag = miscTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Achievement 6",
                            Description = "Sixth Achievement",
                            TrophyTierId = 6,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = grindTag },
                                new() { Tag = miscTag }
                            }
                        }
                    }
                },
                new Game
                {
                    Name = "Test Game 2",
                    Platform = "Any",
                    ReleaseDate = new DateTime(2026, 08, 20),
                    IconFolderName = "test-game",
                    UrlName = "test-game-2",
                    CoverImageUrl = "https://yt3.googleusercontent.com/KI9pQTv4fCFVj7GMLa0QDgHf8obRtHOGg7hLEGhurPS6C6DgBtIWSmx173mWVoGVCXA45d1eMw=s900-c-k-c0x00ffffff-no-rj",
                    Achievements = new List<Achievement>
                    {
                        new Achievement
                        {
                            Name = "Achievement 1",
                            Description = "First Achievement",
                            TrophyTierId = 1,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = storyTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Achievement 2",
                            Description = "Second Achievement",
                            TrophyTierId = 2,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = collectibleTag },
                                new() { Tag = missableTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Achievement 3",
                            Description = "Third Achievement",
                            TrophyTierId = 3,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = combatTag },
                                new() { Tag = grindTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Achievement 4",
                            Description = "Fourth Achievement",
                            TrophyTierId = 4,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = miscTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Achievement 5",
                            Description = "Fifth Achievement",
                            TrophyTierId = 5,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = grindTag },
                                new() { Tag = miscTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Achievement 6",
                            Description = "Sixth Achievement",
                            TrophyTierId = 6,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = grindTag },
                                new() { Tag = miscTag }
                            }
                        }
                    }
                }

            };
            context.Games.AddRange(game);
            context.SaveChanges();
            List<Game> AllGames = context.Games.ToList();
        }
    }
}
