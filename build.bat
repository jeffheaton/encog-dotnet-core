C:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild encog-core-cs.sln /property:Configuration=Release

C:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild encog-core.shfbproj

md \stage\encog\encog-dotnet-dll-3.0.1
md \stage\encog\encog-dotnet-core-3.0.1

copy C:\Users\jheaton\dotnet\build\encog-dotnet-core\EncogCmd\bin\Release\EncogCmd.exe c:\stage\encog\encog-dotnet-dll-3.0.1
copy C:\Users\jheaton\dotnet\build\encog-dotnet-core\encog-core-cs\bin\Release\encog-core-cs.dll c:\stage\encog\encog-dotnet-dll-3.0.1
copy C:\Users\jheaton\dotnet\build\encog-dotnet-core\encog-core-cs\bin\Release\encog-core-cs.xml c:\stage\encog\encog-dotnet-dll-3.0.1
copy C:\Users\jheaton\dotnet\build\encog-dotnet-core\encog-core.chm c:\stage\encog\encog-dotnet-dll-3.0.1
copy C:\Users\jheaton\dotnet\build\encog-dotnet-core\LICENSE.TXT c:\stage\encog\encog-dotnet-dll-3.0.1
copy C:\Users\jheaton\dotnet\build\encog-dotnet-core\README.TXT c:\stage\encog\encog-dotnet-dll-3.0.1

C:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild encog-core-cs.sln /t:clean

XCOPY . c:\stage\encog\encog-dotnet-core-3.0.1\ /s /i
