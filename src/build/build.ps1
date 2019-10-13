
param([string]$version)

if ([string]::IsNullOrEmpty($version)) {$version = "0.0.1"}

$msbuild = "C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
&$msbuild ..\main\Behaviors.Forms.Xamarin\Behaviors.Forms.Xamarin.csproj /t:Build /p:Configuration="Release"
&$msbuild ..\main\Behavior.Forms.Xamarin.Netstandard20\Behavior.Forms.Xamarin.Netstandard20.csproj /t:Build /p:Configuration="Release"


Remove-Item .\NuGet -Force -Recurse
New-Item -ItemType Directory -Force -Path .\NuGet
c:\tools\nuget\NuGet.exe pack Behaviors.Xamarin.Forms.Netstandard.nuspec -Verbosity detailed -Symbols -OutputDir "NuGet" -Version $version