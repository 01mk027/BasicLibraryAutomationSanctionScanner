<i>Bu proje Sanction Scanner þirketi tarafýndan çözülmek üzere gönderilen use case in bitmiþ halidir.</i>
<h3>Sistem Bilgisi</h3>
<b>OS: </b> Windows 10 Education<br/>
<b>IDE: </b> MicrosoftVS 2022<br/>
<b>.NET versiyonu: </b>: 6.0
<b>Veritabaný:</b>: MSSQL Server 
<b>ORM:</b> Entity Framework Core

<h3>Amaç: </h3>
Bir kütüphanede kullanýlmak üzere tasarlanmasý istenen uygulama, kitap ekleme ve ödünç münasebetlerini takip amaçlý geliþtirilmiþtir.
Harici olarak kitap bilgisi ve ödünç kaydý düzenleme özelliklerini muhteva etmektedir.

<h3>Özellikler:</h3>
<ul>
<li>Oturum Desteði, kullanýcý adý ve þifreyle eriþim</li>
<li>Kitap kaydý oluþturma, resimle beraber ekleyebilme, kayýt güncelleyebilme</li>
<li>Kitapla bire-çok iliþkili olarak ödünç kaydý oluþturabilme ve güncelleyebilme.</li>
</ul>

<h3>Çözüm Adýmlarý</h3>
<li>MSSQL + Visual Studio 2022 kurulumu</li>
<li>Modellerin oluþturulmasý, zamanla güncellemeler yapýlmasý</li>
<li>Gerekli nuget paketlerinin eklenmesi.</li>
<li>Denetleyicilerin kýsa yoldan oluþturulmasý ve amaca yönelik düzenlenmeleri</li>
<li>Resim eklenebilmesi için muhtelif yollar denenmesi ve sonuca ulaþýlmasý</li>
<li>Modellerin güncellenmeleri esnasýnda doðru olan iliþkinin (bire-çok) yapýlandýrýlmasý</li>
<li>Kayýt eklemede karþýlaþýlan zorluklarla mücadele, ve sorunsuz kayýt edebilme.</li>
<li>Login adýndaki modelin sonradan eklenmesi, oturuma yönelik iþlem yapabilme özelliðinin eklenmesi</li>
<li>Loglama özelliðinin eklenmesi. (SeriLog ile halledilmiþtir.)</li>
<li>Bireysel testlerin akabinde genel bakýþ, son gözden geçirme.</li>
<li>Deploy.</li>
<br/>
<hr/>
<b><i>Güzel bir antrenmandý, Sanction Scanner'a teþekkürler.</i></b>
