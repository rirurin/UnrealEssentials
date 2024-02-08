# globals
$unreal_essentials = "UnrealEssentials"
$unreal_essentials_interface = $unreal_essentials + ".Interfaces"
$emulator_parent = "UtocEmulator"
$emulator_main = "fileemu-utoc-stream-emulator"
# $emulator_tests = "toc-builder-test"
$emulator_name_csharp = "UTOC.Stream.Emulator"

# build Unreal Essentials
Push-Location "./$unreal_essentials"
dotnet build "./$unreal_essentials.csproj" -v q -c Debug 
Pop-Location
# build Unreal Essentials Interfaces
Push-Location "./$unreal_essentials_interface"
dotnet build "./$unreal_essentials_interface.csproj" -v q -c Debug 
Pop-Location
# build UTOC Emulator
Push-Location "./$emulator_parent/$emulator_main" # optimized binary from PublishAll.ps1
cargo +nightly build --release -Z build-std=std,panic_abort -Z build-std-features=panic_immediate_abort -Z unstable-options --target x86_64-pc-windows-msvc --out-dir "$env:RELOADEDIIMODS\$emulator_name_csharp\"
Pop-Location
Push-Location "./$emulator_parent/$emulator_name_csharp"
dotnet build "./$emulator_name_csharp.csproj" -v q -c Debug 
Pop-Location