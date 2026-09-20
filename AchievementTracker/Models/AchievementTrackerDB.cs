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
                    Name = "Marvel's Spiderman Remastered",
                    Platform = "Steam | Epic Games",
                    ReleaseDate = new DateTime(2026, 08, 20),
                    IconFolderName = "spiderman",
                    UrlName = "",
                    CoverImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRv4dA7GQmf9Fg7ciPAS1amRnchlIkc2fLuP48nc6VWSA&s",
                    Achievements = new List<Achievement>
                    {
                        new Achievement
                        {
                            Name = "Be Greater",
                            Description = "Tüm başarımları topla.",
                            TrophyTierId = 6,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 600,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = miscTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Demons Emerge",
                            Description = "Act 1'i tamamla.",
                            TrophyTierId = 1,
                            IsHidden = true,
                            IsSpoiler = true,
                            Points = 15,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = storyTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "The Six Assemble",
                            Description = "Act 2'yi tamamla.",
                            TrophyTierId = 1,
                            IsHidden = true,
                            IsSpoiler = true,
                            Points = 15,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = storyTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "End Game",
                            Description = "Act 3'ü tamamla.",
                            TrophyTierId = 1,
                            IsHidden = true,
                            IsSpoiler = true,
                            Points = 15,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = storyTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Knocking Down Kingpin",
                            Description = "Fisk'i yen.",
                            TrophyTierId = 1,
                            IsHidden = true,
                            IsSpoiler = false,
                            Points = 15,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = storyTag },
                                new() { Tag = combatTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Staying Positive",
                            Description = "Martin Li'yi yen.",
                            TrophyTierId = 1,
                            IsHidden = true,
                            IsSpoiler = true,
                            Points = 15,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = storyTag },
                                new() { Tag = combatTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Grounded",
                            Description = "Electro ve Vulture'ı yen.",
                            TrophyTierId = 1,
                            IsHidden = true,
                            IsSpoiler = true,
                            Points = 15,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = storyTag },
                                new() { Tag = combatTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Sting and Smash",
                            Description = "Scorpion ve Rhino'yu yen.",
                            TrophyTierId = 1,
                            IsHidden = true,
                            IsSpoiler = true,
                            Points = 15,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = storyTag },
                                new() { Tag = combatTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Shock and Awe",
                            Description = "Shocker'ı yen.",
                            TrophyTierId = 1,
                            IsHidden = true,
                            IsSpoiler = true,
                            Points = 15,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = storyTag },
                                new() { Tag = combatTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Tombstone Takedown",
                            Description = "Tombstone'u yen.",
                            TrophyTierId = 1,
                            IsHidden = true,
                            IsSpoiler = true,
                            Points = 15,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = storyTag },
                                new() { Tag = combatTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Ace the Base",
                            Description = "Bir üssün (base) tüm hedeflerini tamamla.",
                            TrophyTierId = 2,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 30,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = miscTag },
                                new() { Tag = combatTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Amazing Coverage",
                            Description = "Tüm Gözetleme Kulelerini aktifleştir.",
                            TrophyTierId = 2,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 30,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = miscTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Short Fuse",
                            Description = "Bir Bomba Görevinde Spectacular veya üstü skor al.",
                            TrophyTierId = 2,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 30,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = miscTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Fists of Fury",
                            Description = "Bir Dövüş Görevinde Spectacular veya üstü skor al.",
                            TrophyTierId = 2,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 30,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = combatTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Ninja",
                            Description = "Bir Gizlilik Görevinde Spectacular veya üstü skor al.",
                            TrophyTierId = 2,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 30,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = miscTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Spy Hunter",
                            Description = "Bir Drone Görevinde Spectacular veya üstü skor al.",
                            TrophyTierId = 2,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 30,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = miscTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Challenge Finder",
                            Description = "Şehirdeki her Taskmaster görevini en az bir kez bitir.",
                            TrophyTierId = 2,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 30,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = miscTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "R&D",
                            Description = "Tüm Araştırma İstasyonlarını tamamla.",
                            TrophyTierId = 2,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 30,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = miscTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Winging It",
                            Description = "Şehir çatıları boyunca belli bir mesafe katet.",
                            TrophyTierId = 2,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 30,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = miscTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Pigeon Hunter",
                            Description = "Howard'ın tüm güvercinlerini yakala.",
                            TrophyTierId = 2,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 30,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = collectibleTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Friendly Neighbourhood Spider-Man",
                            Description = "Tüm Yan Görevleri (Side Missions) tamamla.",
                            TrophyTierId = 2,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 30,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = miscTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "The Scientific Method",
                            Description = "İlk Yükseltmeni (Upgrade) üret.",
                            TrophyTierId = 2,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 30,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = miscTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "With Great Power...",
                            Description = "Ben Parker'ın mezarını ziyaret et.",
                            TrophyTierId = 2,
                            IsHidden = true,
                            IsSpoiler = false,
                            Points = 30,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = miscTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Hero for Higher",
                            Description = "Avengers Kulesi'nin en tepesine tün.",
                            TrophyTierId = 2,
                            IsHidden = true,
                            IsSpoiler = false,
                            Points = 30,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = miscTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Sightseeing",
                            Description = "Haritadaki tüm Simgesel Yapıları (Landmarks) fotoğrafla.",
                            TrophyTierId = 2,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 30,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = collectibleTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Sticky and Tricky",
                            Description = "Yere inmeden arka arkaya 4 benzersiz hava numarası yap.",
                            TrophyTierId = 2,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 30,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = miscTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Snappy Dresser",
                            Description = "5 yeni Örümcek Kostümü giy.",
                            TrophyTierId = 2,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 30,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = miscTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Lost and Found",
                            Description = "5 Sırt Çantası topla.",
                            TrophyTierId = 2,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 30,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = collectibleTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Spider-Man About Town",
                            Description = "Sokaktaki 10 vatandaşla selamlaş.",
                            TrophyTierId = 2,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 30,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = miscTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Cat's Out of the Bag",
                            Description = "İlk Black Cat koleksiyon parçasını bul.",
                            TrophyTierId = 2,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 30,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = collectibleTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "A Bit of a Fixer-Upper",
                            Description = "Laboratuvardaki tüm isteğe bağlı projeleri tamamla.",
                            TrophyTierId = 2,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 30,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = miscTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Born to Ride",
                            Description = "Metroya (Hızlı Seyahat) 5 kez bin.",
                            TrophyTierId = 2,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 30,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = miscTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Hug It Out",
                            Description = "Trip Mine kullanarak 10 çift düşmanı birbirine çarp.",
                            TrophyTierId = 2,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 30,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = combatTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Overdrive",
                            Description = "10 Araç alt etme (Vehicle Takedown) gerçekleştir.",
                            TrophyTierId = 2,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 30,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = combatTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Spider-Sensible",
                            Description = "10 düşman saldırısından Kusursuz Kaçınma (Perfect Dodge) yap.",
                            TrophyTierId = 2,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 30,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = combatTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Master of Masters",
                            Description = "Taskmaster'ı yen.",
                            TrophyTierId = 3,
                            IsHidden = true,
                            IsSpoiler = true,
                            Points = 60,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = combatTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Backpacker",
                            Description = "Tüm sırt çantalarını topla.",
                            TrophyTierId = 3,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 60,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = collectibleTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Cat Prints",
                            Description = "Black Cat'in izini sür.",
                            TrophyTierId = 3,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 60,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = collectibleTag },
                                new() { Tag = miscTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Inner Sanctuary",
                            Description = "Tüm Demon depolarını temizle.",
                            TrophyTierId = 3,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 60,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = combatTag },
                                new() { Tag = miscTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "All the King's Men",
                            Description = "Tüm Fisk sığınaklarını temizle.",
                            TrophyTierId = 3,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 60,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = combatTag },
                                new() { Tag = miscTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Mercenary Tactics",
                            Description = "Tüm Sable karakollarını temizle.",
                            TrophyTierId = 3,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 60,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = combatTag },
                                new() { Tag = miscTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Back in the Slammer",
                            Description = "Tüm Mahkum kamplarını temizle.",
                            TrophyTierId = 3,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 60,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = combatTag },
                                new() { Tag = miscTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Schooled",
                            Description = "Tüm Corrupted Student görevlerini bitir.",
                            TrophyTierId = 3,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 60,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = miscTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Arachnophobia",
                            Description = "75 Gizli Alt Etme (Stealth Takedown) gerçekleştir.",
                            TrophyTierId = 3,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 60,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = combatTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Science FTW!",
                            Description = "15 Yükseltme (Upgrade) üret.",
                            TrophyTierId = 3,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 60,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = miscTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "King of Swing",
                            Description = "Seviye 1 Traversal Benchmark'ı tamamla.",
                            TrophyTierId = 3,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 60,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = miscTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "And Stay Down!",
                            Description = "Seviye 1 Combat Benchmark'ı tamamla.",
                            TrophyTierId = 3,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 60,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = combatTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Superior Spider-Man",
                            Description = "Tüm yetenekleri (Skills) aç.",
                            TrophyTierId = 4,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 150,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = miscTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "I Heart Manhattan",
                            Description = "Tüm bölgeleri %100 tamamla.",
                            TrophyTierId = 5,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 300,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = grindTag },
                                new() { Tag = miscTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "A Suit For All Seasons",
                            Description = "Tüm kostümleri satın al.",
                            TrophyTierId = 5,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 300,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = grindTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Neighbourhood Watch",
                            Description = "Bir bölgedeki tüm Faction suçlarını tamamla.",
                            TrophyTierId = 5,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 300,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = grindTag },
                                new() { Tag = combatTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Grinding All the Way",
                            Description = "(Remastered) Herhangi bir Benchmark türünü tamamen fulle.",
                            TrophyTierId = 5,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 300,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = grindTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Full Arsenal",
                            Description = "(Remastered) Tüm cihazları (Gadgets) en son seviyeye yükselt.",
                            TrophyTierId = 5,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 300,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = grindTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Master's Education",
                            Description = "(Remastered) Bir Taskmaster Görevinde Ultimate seviyesine ulaş.",
                            TrophyTierId = 4,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 150,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = miscTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "So Many Hits...",
                            Description = "(Remastered) 100'lük bir kombo zincirine ulaş.",
                            TrophyTierId = 4,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 150,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = combatTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "The Untouchable Spider-Man",
                            Description = "(Remastered) Herhangi bir Düşman Üssünü hiç hasar almadan tamamla.",
                            TrophyTierId = 4,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 150,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = combatTag },
                                new() { Tag = miscTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "One More Time",
                            Description = "(NG+) Oyunu New Game+ modunda tamamla.",
                            TrophyTierId = 5,
                            IsHidden = false,
                            IsSpoiler = true,
                            Points = 300,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = grindTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Power and Responsibility",
                            Description = "(NG+) Oyunu Ultimate zorluk derecesinde tamamla.",
                            TrophyTierId = 4,
                            IsHidden = false,
                            IsSpoiler = true,
                            Points = 150,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = miscTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Seduced by the City",
                            Description = "CTNS: The Heist DLC'sini %100 tamamla.",
                            TrophyTierId = 5,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 300,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = grindTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "The Cat Came Back",
                            Description = "The Maria görevini tamamla.",
                            TrophyTierId = 1,
                            IsHidden = true,
                            IsSpoiler = true,
                            Points = 15,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = storyTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Here Kitty-Kitty",
                            Description = "Black Cat kovalamacasını tamamla.",
                            TrophyTierId = 1,
                            IsHidden = true,
                            IsSpoiler = true,
                            Points = 15,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = storyTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Bye Felicia",
                            Description = "Follow the Money görevini tamamla.",
                            TrophyTierId = 1,
                            IsHidden = true,
                            IsSpoiler = true,
                            Points = 15,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = storyTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "The Long Con",
                            Description = "The Heist'teki tüm Walter Hardy tablolarını topla.",
                            TrophyTierId = 3,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 60,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = collectibleTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Disorganised Crime",
                            Description = "Bir bölgedeki tüm The Heist suçlarını tamamla.",
                            TrophyTierId = 3,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 60,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = combatTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Screwy",
                            Description = "Tüm The Heist Screwball görevlerinde Spectacular veya üstü al.",
                            TrophyTierId = 3,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 60,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = miscTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "The City is My Family",
                            Description = "CTNS: Turf Wars DLC'sini %100 tamamla.",
                            TrophyTierId = 5,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 300,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = grindTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Pulling the Trigger",
                            Description = "Blindsided görevini tamamla.",
                            TrophyTierId = 1,
                            IsHidden = true,
                            IsSpoiler = true,
                            Points = 15,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = storyTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Crossing the Thin Blue Line",
                            Description = "Lockup görevini tamamla.",
                            TrophyTierId = 1,
                            IsHidden = true,
                            IsSpoiler = true,
                            Points = 15,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = storyTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Steel Skull, Glass Jaw",
                            Description = "Bring the Hammer Down görevini tamamla.",
                            TrophyTierId = 1,
                            IsHidden = true,
                            IsSpoiler = true,
                            Points = 15,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = storyTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Prohibition",
                            Description = "Turf Wars'taki tüm Hammerhead Cephelerini çökert.",
                            TrophyTierId = 3,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 60,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = combatTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "The Gang War",
                            Description = "Bir bölgedeki tüm Turf Wars suçlarını tamamla.",
                            TrophyTierId = 3,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 60,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = combatTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Turning the Screw",
                            Description = "Tüm Turf Wars Screwball görevlerinde Spectacular veya üstü al.",
                            TrophyTierId = 3,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 60,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = miscTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "The City Sleeps",
                            Description = "CTNS: Silver Lining DLC'sini %100 tamamla.",
                            TrophyTierId = 5,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 300,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = grindTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Frenemies",
                            Description = "Old Friends görevini tamamla.",
                            TrophyTierId = 1,
                            IsHidden = true,
                            IsSpoiler = true,
                            Points = 15,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = storyTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Unplugged",
                            Description = "Screwball kovalamacasını tamamla.",
                            TrophyTierId = 1,
                            IsHidden = true,
                            IsSpoiler = true,
                            Points = 15,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = storyTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Terminated",
                            Description = "One Plus One Equals Win görevini tamamla.",
                            TrophyTierId = 1,
                            IsHidden = true,
                            IsSpoiler = true,
                            Points = 15,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = storyTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Screwballed",
                            Description = "Tüm Silver Lining Screwball görevlerinde Spectacular veya üstü al.",
                            TrophyTierId = 3,
                            IsHidden = false,
                            IsSpoiler = false,
                            Points = 60,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = miscTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "Unacceptable",
                            Description = "Scales of Justice görevini tamamla.",
                            TrophyTierId = 3,
                            IsHidden = true,
                            IsSpoiler = true,
                            Points = 60,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = storyTag },
                                new() { Tag = combatTag }
                            }
                        },
                        new Achievement
                        {
                            Name = "The Wages of War",
                            Description = "Aiding a Human görevini tamamla.",
                            TrophyTierId = 1,
                            IsHidden = true,
                            IsSpoiler = true,
                            Points = 15,
                            AchievementTags = new List<AchievementTag>
                            {
                                new() { Tag = storyTag }
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
