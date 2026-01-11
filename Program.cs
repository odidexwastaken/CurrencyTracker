using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace CurrencyTracker
{
    // gerekli Sınıflar
    class CurrencyResponse
    {
        public string Base { get; set; }
        public Dictionary<string, decimal> Rates { get; set; }
    }

    class Currency
    {
        public string Code { get; set; }
        public decimal Rate { get; set; }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            // listemizi en başta oluşturuyoruz
            List<Currency> currencies = new List<Currency>();

            // VERİ ÇEKME İŞLEMİ 

            // using bloğu içinde basit client kullanımı
            using (HttpClient client = new HttpClient())
            {
                Console.WriteLine("Veriler internetten çekiliyor...");
                string url = "https://api.frankfurter.app/latest?from=TRY";

                // veriyi string olarak alıyoruz
                string jsonVerisi = await client.GetStringAsync(url);

                // JSON ayarları (büyük küçük harf sorunu olmasın diye)
                var ayarlar = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                // gelen veriyi sınıfa çeviriyoruz
                CurrencyResponse gelenVeri = JsonSerializer.Deserialize<CurrencyResponse>(jsonVerisi, ayarlar);

                // dictionary olan veriyi listeye çeviriyoruz
                currencies = gelenVeri.Rates.Select(x => new Currency { Code = x.Key, Rate = x.Value }).ToList();
            }

            // MENÜ DÖNGÜSÜ
            while (true)
            {
                Console.Clear(); // ekranı temizle
                Console.WriteLine("===== CurrencyTracker =====");
                Console.WriteLine("1. Tüm dövizleri listele");
                Console.WriteLine("2. Koda göre döviz ara");
                Console.WriteLine("3. Belirli bir değerden büyük dövizleri listele");
                Console.WriteLine("4. Dövizleri değere göre sırala");
                Console.WriteLine("5. İstatistiksel özet göster");
                Console.WriteLine("0. Çıkış");
                Console.Write("Seçiminiz: ");

                string secim = Console.ReadLine();

                Console.WriteLine("\n--- SONUÇLAR ---\n");

                if (secim == "1")
                {
                    // 1. tümünü Listele
                    // LINQ Select ile sadece yazı haline getirip sonra yazdırıyoruz
                    var liste = currencies.Select(x => x.Code + ": " + x.Rate).ToList();

                    foreach (var item in liste)
                    {
                        Console.WriteLine(item);
                    }
                }
                else if (secim == "2")
                {
                    // 2. arama Yapma
                    Console.Write("Döviz Kodu Girin (Örn: USD): ");
                    string aranan = Console.ReadLine();

                    // hepsini küçük harfe çevirip kontrol ediyoruz (ToLower)
                    var bulunanlar = currencies.Where(x => x.Code.ToLower() == aranan.ToLower()).ToList();

                    if (bulunanlar.Count > 0)
                    {
                        foreach (var item in bulunanlar)
                        {
                            Console.WriteLine("Bulundu: " + item.Code + " - Değer: " + item.Rate);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Böyle bir döviz bulamadım.");
                    }
                }
                else if (secim == "3")
                {
                    // 3. ... değerinden büyük olanlar
                    Console.Write("Bir değer girin (Örn: 0,5): ");
                    string degerString = Console.ReadLine();

                    // girilen yazıyı sayıya (decimal) çevirmeyi deniyoruz
                    decimal limit;
                    bool sayiMi = decimal.TryParse(degerString, out limit);

                    if (sayiMi)
                    {
                        var buyukler = currencies.Where(x => x.Rate > limit).ToList();

                        if (buyukler.Count > 0)
                        {
                            foreach (var item in buyukler)
                            {
                                Console.WriteLine(item.Code + ": " + item.Rate);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Bu değerden büyük bir döviz yok.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Lütfen geçerli bir sayı girin.");
                    }
                }
                else if (secim == "4")
                {
                    // 4. sıralama
                    Console.Write("1-Artan Sıralama, 2-Azalan Sıralama: ");
                    string siralamaSecimi = Console.ReadLine();

                    List<Currency> siraliListe;

                    if (siralamaSecimi == "2")
                    {
                        // azalan (büyükten küçüğe)
                        siraliListe = currencies.OrderByDescending(x => x.Rate).ToList();
                    }
                    else
                    {
                        // artan (küçükten büyüğe)
                        siraliListe = currencies.OrderBy(x => x.Rate).ToList();
                    }

                    foreach (var item in siraliListe)
                    {
                        Console.WriteLine(item.Code + ": " + item.Rate);
                    }
                }
                else if (secim == "5")
                {
                    // 5. istatistikler
                    // LINQ metodları: Count, Max, Min, Average
                    int toplamSayi = currencies.Count();
                    decimal enYuksek = currencies.Max(x => x.Rate);
                    decimal enDusuk = currencies.Min(x => x.Rate);
                    decimal ortalama = currencies.Average(x => x.Rate);

                    Console.WriteLine("Toplam Döviz Sayısı: " + toplamSayi);
                    Console.WriteLine("En Yüksek Değer: " + enYuksek);
                    Console.WriteLine("En Düşük Değer: " + enDusuk);
                    Console.WriteLine("Ortalama Değer: " + ortalama);
                }
                else if (secim == "0")
                {
                    Console.WriteLine("Çıkış yapılıyor...");
                    break;
                }
                else
                {
                    Console.WriteLine("Geçerli bir değer giriniz.");
                }

                Console.WriteLine("\nDevam etmek için bir tuşa basın...");
                Console.ReadKey();
            }
        }
    }
}