// ==========================================
// DARK/LIGHT TEMA DEĞİŞTİRİCİ
// ==========================================
document.addEventListener("DOMContentLoaded", function () {
    const themeSwitch = document.getElementById('themeSwitch');

    // Daha önce seçilmiş temayı hafızadan al (Yoksa varsayılan dark)
    const currentTheme = localStorage.getItem('theme') || 'dark';

    // Sayfa açılır açılmaz temayı uygula
    document.documentElement.setAttribute('data-bs-theme', currentTheme);

    // Eğer tema light ise butonu "Güneş" tarafına çek
    if (currentTheme === 'light') {
        themeSwitch.checked = true;
    }

    // Butona tıklandığında çalışacak olay
    themeSwitch.addEventListener('change', function (e) {
        if (e.target.checked) {
            document.documentElement.setAttribute('data-bs-theme', 'light');
            localStorage.setItem('theme', 'light');
        } else {
            document.documentElement.setAttribute('data-bs-theme', 'dark');
            localStorage.setItem('theme', 'dark');
        }
    });
});
// ========================================
// YILDIZ (FAVORİ) BUTONU TIKLANMA İŞLEMİ
// ========================================
$(document).on('click', '.toggle-favorite-btn', function (e) {
    e.preventDefault(); // Butonun sayfayı en üste kaydırmasını engeller

    var btn = $(this);
    var gameId = btn.data('game-id');
    var icon = btn.find('i'); // Tıklanan butonun içindeki <i> (yıldız) etiketini yakala

    $.ajax({
        url: '/Home/ToggleFavorite',
        type: 'POST',
        data: { id: gameId },
        success: function (response) {
            if (response.success) {
                if (response.isFavorite) {
                    // Yıldıza tıklandı ve Favoriye eklendi: İçi boş yıldızı sil, dolu yıldız (sarı) yap
                    icon.removeClass('bi-star').addClass('bi-star-fill');
                    console.log("Oyun %100 listesine eklendi!");
                } else {
                    // Yıldıza tekrar tıklandı ve Favoriden çıkarıldı: İçi dolu yıldızı sil, boş yıldız yap
                    icon.removeClass('bi-star-fill').addClass('bi-star');
                    console.log("Oyun %100 listesinden çıkarıldı!");

                    // 🌟 UX SİHRİ: Favoriler sayfasındaysak ve yıldızı kaldırdıysa...
                    if (window.location.pathname.toLowerCase().includes("hundredlist")) {
                        var cardCol = btn.closest('.col'); // Silinen kart
                        var gamesContainer = $('#gamesContainer'); // Kartların durduğu ana satır

                        cardCol.fadeOut(400, function () {
                            $(this).remove(); // Kartı HTML'den tamamen sil
                            var counterElement = $('#totalGameCounter');
                            var currentCount = parseInt(counterElement.text().replace(/[^0-9]/g, ''));
                            if (!isNaN(currentCount) && currentCount > 0) {
                                counterElement.text("Toplam Oyun: " + (currentCount - 1));
                            }
                            // EĞER SİLİNEN KARTTAN SONRA İÇERİDE HİÇ KART KALMADIYSA:
                            if (gamesContainer.children('.col').length === 0) {
                                gamesContainer.hide(); // Boş satırı gizle
                                $('#emptyFavoritesState').fadeIn(600); // Boş ekran şovunu başlat!
                            }
                        });
                    }
                }
            } else {
                alert('@Html.Raw(Localizer["OperationFailed"].Value: "' + response.message);
            }
        },
        error: function () {
            alert('@Html.Raw(Localizer["ServerUnreachable"].Value)');
        }
    });
});
// ==========================================
// AKILLI NAVBAR ARAMA SİSTEMİ (GLOBAL SEARCH)
// ==========================================
$(function () {
    $("#globalSearchInput").on("keyup", function () {
        var value = $(this).val().toLowerCase();

        // Sayfada "gamesContainer" (oyunların listelendiği ana kutu) var mı diye bakıyoruz
        if ($("#gamesContainer").length > 0) {

            // Eğer varsa, anında o sayfanın (Ana sayfa, %100 Kulübü veya Favoriler) içindeki oyunları filtrele
            $("#gamesContainer > .col").filter(function () {
                var gameTitle = $(this).find('h5.card-title').text().toLowerCase();
                $(this).toggle(gameTitle.indexOf(value) > -1);
            });

        } else {
            // EKSTRA VİZYON: Eğer kullanıcı ayarlardayken (oyun listesi olmayan bir sayfada) 
            // arama yaparsa, onu yazdığı kelimeyle birlikte ana sayfaya yönlendirebilirsin.
            // (Şimdilik boş bırakıyoruz, sadece oyun olan sayfalarda çalışacak)
        }
    });
});


