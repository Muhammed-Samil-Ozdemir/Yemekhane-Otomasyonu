# Yemekhane Otomasyonu · Dönem Ödevi Raporu

| | |
|---|---|
| Ders | Web Teknolojileri |
| Öğrenci | Muhammed Şamil Özdemir, 241229001 |
| Depo | https://github.com/Muhammed-Samil-Ozdemir/Yemekhane-Otomasyonu |
| Seçilen yol | **Web API + HTML5/CSS3 arayüz** (ASP.NET Core 10, arayüz JavaScript `fetch` ile) |
| Etiketler | `asama-1`, `asama-2`, `asama-3`, `asama-4` |

## Aşama 1 · Altyapı

Başlangıç paketi değiştirilmeden çalıştırıldı (`docker compose up -d`).

```
$ docker compose ps -a
NAME                        SERVICE         STATUS
yemekhane-frontend-1        frontend        Up 25 seconds (healthy)
yemekhane-mysql-primary-1   mysql-primary   Up 25 seconds (healthy)
yemekhane-mysql-replica-1   mysql-replica   Up 26 seconds (healthy)
yemekhane-redis-1           redis           Up 25 seconds (healthy)
yemekhane-replica-setup-1   replica-setup   Exited (0) 3 seconds ago

$ docker compose logs replica-setup
[replica-setup] Replikasyon çalışıyor (IO=Yes, SQL=Yes). Replika salt okunur.

$ docker compose exec mysql-replica mysql -uroot -pyemekhane_kok -e "SHOW REPLICA STATUS\G"
           Replica_IO_Running: Yes
          Replica_SQL_Running: Yes
        Seconds_Behind_Source: 0
                Auto_Position: 1
```

**Kaynakta yapılan bakiye yüklemesi replikada görünüyor**

```
$ docker compose exec mysql-primary mysql -uyemekhane_app -pUyg_2026 yemekhane -e "CALL sp_bakiye_yukle(7, 100)"
hareket_id  bakiye
455         455.00

$ docker compose exec mysql-replica mysql -uyemekhane_app -pUyg_2026 yemekhane -e "SELECT id, bakiye FROM kullanicilar WHERE id = 7"
id  bakiye
7   455.00
```

**Replikaya yazma denemesi read-only hatasıyla reddediliyor**

```
$ docker compose exec mysql-replica mysql -uyemekhane_app -pUyg_2026 yemekhane -e "CALL sp_bakiye_yukle(7, 50)"
ERROR 1290 (HY000) at line 1: The MySQL server is running with the --super-read-only option so it cannot execute this statement
```

**Replika durdurulup başlatıldığında aradaki değişikliği yakalıyor**

```
$ docker compose stop mysql-replica
$ docker compose exec mysql-primary mysql -uyemekhane_app -pUyg_2026 yemekhane -e "CALL sp_bakiye_yukle(7, 200)"
hareket_id  bakiye
456         655.00
$ docker compose start mysql-replica
$ docker compose exec mysql-replica mysql -uyemekhane_app -pUyg_2026 yemekhane -e "SELECT id, bakiye FROM kullanicilar WHERE id = 7"
id  bakiye
7   655.00
           Retrieved_Gtid_Set: 83613c04-e6b5-11f1-9381-da8b70113a24:43-44
```

Replika kapalıyken yapılan işlem 44 numaralı GTID olarak geldi; replika açılınca `SOURCE_AUTO_POSITION = 1` sayesinde kaldığı yerden devam etti. Redis `PONG` döndü. Yordamların hata sözleşmesi de denendi: 12,50 TL bakiyeli kullanıcıyla `CALL sp_rezervasyon_yap(6, 80)` → `ERROR 1644 (45000): Bakiye yetersiz.`
