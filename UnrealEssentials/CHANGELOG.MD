# Unreal Essentials Changelog
## 2.0.0
- Added Unreal Engine 5 support
- Added file logging for games that use UTOC
## 1.2.0
- Added IUnrealMemory to the API to help working with memory using the native UE allocator (@TheBestAstroNOT)

## 1.1.5
- Added support for The Callisto Protocol

## 1.1.4
- Added support for Hogwarts Legacy

## 1.1.3
- Fix support for Hi-Fi RUSH

## 1.1.2
- Improved clarity of error message when the Unreal Engine version cannot be found.
- Error now is just a logged message instead of a thrown exception as people regularly enable this on non Unreal Engine games, preventing them from working at all.

## 1.1.1
- Added support for Shin Megami Tensei V Vengeance

## 1.1.0
- Added an API to allow mods to add files from code. Checkout [the documentation](https://github.com/AnimatedSwine37/UnrealEssentials/tree/master/UnrealEssentials.Interfaces/README.md) for more information.

## 1.0.2
- Files are now accepted from any mod, not just those that directly depend on Unreal Essentials (done so stuff works with P3R Essentials)