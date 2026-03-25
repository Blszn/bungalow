@echo off
title Bungalov Web Projesi
echo =======================================================
echo Bungalov Web Projesi Baslatiliyor...
echo Lutfen bekleyin, ilk acilista derleme yapilabilir.
echo =======================================================

:: Dosyanin bulundugu dizine gidelim
cd /d "%~dp0"

:: Projeyi calistir
dotnet run --project Bungalov.WebUI\Bungalov.WebUI.csproj

:: Eger uygulama kapanirsa veya hata verirse ekranda kalmasi icin
pause
