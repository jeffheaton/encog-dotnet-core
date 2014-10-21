set version=3.3.0
md \stage\encog\encog-dotnet-dll-%version%
md \stage\encog\encog-dotnet-core-%version%

C:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild encog-core-cs.sln /property:Configuration=Release

C:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild encog-core.shfbproj

copy .\encog-core-cs\bin\Release\encog-core-cs.dll c:\stage\encog\encog-dotnet-dll-%version%
copy .\encog-core-cs\bin\Release\encog-core-cs.xml c:\stage\encog\encog-dotnet-dll-%version%
copy .\encog-dotnet-core\encog-core.chm c:\stage\encog\encog-dotnet-dll-%version%
copy .\LICENSE.TXT c:\stage\encog\encog-dotnet-dll-%version%
copy .\README.TXT c:\stage\encog\encog-dotnet-dll-%version%

C:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild encog-core-cs.sln /t:clean

XCOPY . c:\stage\encog\encog-dotnet-core-%version%\ /s /i

rd /s /q "C:\stage\encog\encog-dotnet-core-%version%\ConsoleExamples\bin"
rd /s /q "C:\stage\encog\encog-dotnet-core-%version%\encog-core-cs\bin"
rd /s /q "C:\stage\encog\encog-dotnet-core-%version%\encog-core-test\bin"
rd /s /q "C:\stage\encog\encog-dotnet-core-%version%\ConsoleExamples\obj"
rd /s /q "C:\stage\encog\encog-dotnet-core-%version%\encog-core-cs\obj"
rd /s /q "C:\stage\encog\encog-dotnet-core-%version%\encog-core-test\obj"

