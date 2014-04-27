net stop "AutoRu Crawler"
c:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild ..\AutoRu.sln /t:Rebuild /p:Configuration=Release
xcopy bin\Release c:\bin\AutoRuCrawlerSvc /E /Y /R /I
c:\bin\AutoRuCrawlerSvc\AutoRu.CrawlerSvc.exe --install
net start "AutoRu Crawler"