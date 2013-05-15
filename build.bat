md \stage\encog\encog-dotnet-dll-3.2.0
md \stage\encog\encog-dotnet-core-3.2.0

C:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild encog-core-cs.sln /property:Configuration=Release

C:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild encog-core.shfbproj


copy .\encog-core-cs\bin\Release\encog-core-cs.dll c:\stage\encog\encog-dotnet-dll-3.2.0
copy .\encog-core-cs\bin\Release\encog-core-cs.xml c:\stage\encog\encog-dotnet-dll-3.2.0
copy .\encog-dotnet-core\encog-core.chm c:\stage\encog\encog-dotnet-dll-3.2.0
copy .\LICENSE.TXT c:\stage\encog\encog-dotnet-dll-3.2.0
copy .\README.TXT c:\stage\encog\encog-dotnet-dll-3.2.0

C:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild encog-core-cs.sln /t:clean

XCOPY . c:\stage\encog\encog-dotnet-core-3.2.0\ /s /i
