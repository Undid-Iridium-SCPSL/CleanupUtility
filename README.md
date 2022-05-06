

![CleanupUtility ISSUES](https://img.shields.io/github/issues/Undid-Iridium/CleanupUtility)
![CleanupUtility FORKS](https://img.shields.io/github/forks/Undid-Iridium/CleanupUtility)
![CleanupUtility LICENSE](https://img.shields.io/github/license/Undid-Iridium/CleanupUtility)


![CleanupUtility LATEST](https://img.shields.io/github/v/release/Undid-Iridium/CleanupUtility?include_prereleases&style=flat-square)
![CleanupUtility LINES](https://img.shields.io/tokei/lines/github/Undid-Iridium/CleanupUtility)
![CleanupUtility DOWNLOADS](https://img.shields.io/github/downloads/Undid-Iridium/CleanupUtility/total?style=flat-square)


# CleanupUtility

Ability to control what items get cleaned up, and when.

This solution Hijacks the methods that instantiate the ServerCreatePickup of items, and of ammo. I went down this route to allow the ability to lock in individual patch's per type, and force the ItemPickupBase to be grabbed after it was done being touched by NW's instantiation code (or at least right after). If you have any suggestions please don't hesitate to open an issue. This also includes the zone the pickup was created in, which allows you specify where items can be deleted from. Thanks

* Add Probability of item to be cleaned up (Feature, may not do unless desire for)
* ~~Add zone choices~~ Done V1.1.2
* ~~Add documentation in code. Also justification for queue is that I could use it for more than just an escape method (No longer true based on granular filtering, so swapping to dictionary system). Also justification for two transpilers was for after-NW touching (Not needed after some testing, reference object is delayed long enough it is a non-relevant factor. If I was cleaning items up immediately then maybe; however, behavior will remain the same in essence)~~ Done

# Installation

**[EXILED](https://github.com/Exiled-Team/EXILED) must be installed for this to work.**

Emergency Bug Fix V1.1.6

Current plugin version: V1.1.5 (If you do not have items being cleaned up fast in 914, this will work but I would recommend V1.1.6)

## REQUIREMENTS
* Exiled: V5.2.0
* SCP:SL Server: V11.2


Example configuration
```
cleanup_utility:
  is_enabled: true
  # Whether debug logs should be shown.
  debug: true
  # The time, in seconds, between each check of the list of items to delete.
  check_interval: 2
  # A collection of items that should be deleted paired with the time, in seconds, to wait before deleting them.
  item_filter:
    KeycardJanitor: 10
    KeycardScientist: 10
    KeycardResearchCoordinator: 10
    KeycardZoneManager: 10
    KeycardGuard: 10
    KeycardNTFOfficer: 10
    KeycardContainmentEngineer: 10
    KeycardNTFLieutenant: 10
    KeycardNTFCommander: 10
    KeycardFacilityManager: 10
    KeycardChaosInsurgency: 10
    KeycardO5: 10
    Radio: 10
    GunCOM15: 10
    Medkit: 10
    Flashlight: 10
    SCP500: 10
    SCP207: 10
    Ammo12gauge: 10
    GunE11SR: 10
    GunCrossvec: 10
    Ammo556x45: 10
    GunFSP9: 10
    GunLogicer: 10
    GrenadeHE: 10
    GrenadeFlash: 10
    Ammo44cal: 10
    Ammo762x39: 10
    Ammo9x19: 10
    GunCOM18: 10
    SCP018: 10
    SCP268: 10
    Adrenaline: 10
    Painkillers: 10
    Coin: 10
    ArmorLight: 10
    ArmorCombat: 10
    ArmorHeavy: 10
    GunRevolver: 10
    GunAK: 10
    GunShotgun: 10
    SCP330: 10
    SCP2176: 10
    SCP1853: 10
  # Filter on what zone item type can be cleared from.
  zone_filter:
    KeycardJanitor:
    - Surface
    KeycardScientist:
    - Unspecified
    KeycardResearchCoordinator:
    - Unspecified
    KeycardZoneManager:
    - Unspecified
    KeycardGuard:
    - Unspecified
    KeycardNTFOfficer:
    - Unspecified
    KeycardContainmentEngineer:
    - Unspecified
    KeycardNTFLieutenant:
    - Unspecified
    KeycardNTFCommander:
    - Unspecified
    KeycardFacilityManager:
    - Unspecified
    KeycardChaosInsurgency:
    - Unspecified
    KeycardO5:
    - Unspecified
    Radio:
    - Unspecified
    GunCOM15:
    - Unspecified
    Medkit:
    - Unspecified
    Flashlight:
    - Unspecified
    SCP500:
    - Unspecified
    SCP207:
    - Unspecified
    Ammo12gauge:
    - Unspecified
    GunE11SR:
    - Unspecified
    GunCrossvec:
    - Unspecified
    Ammo556x45:
    - Unspecified
    GunFSP9:
    - Unspecified
    GunLogicer:
    - Unspecified
    GrenadeHE:
    - Unspecified
    GrenadeFlash:
    - Unspecified
    Ammo44cal:
    - Unspecified
    Ammo762x39:
    - Unspecified
    Ammo9x19:
    - Unspecified
    GunCOM18:
    - Unspecified
    SCP018:
    - Unspecified
    SCP268:
    - Unspecified
    Adrenaline:
    - Unspecified
    Painkillers:
    - Unspecified
    Coin:
    - Unspecified
    ArmorLight:
    - Unspecified
    ArmorCombat:
    - Unspecified
    ArmorHeavy:
    - Unspecified
    GunRevolver:
    - Unspecified
    GunAK:
    - Unspecified
    GunShotgun:
    - Unspecified
    SCP330:
    - Unspecified
    SCP2176:
    - Unspecified
    SCP1853:
    - Unspecified
 ```
 
![NVIDIA_Share_YmibdG6PY2](https://user-images.githubusercontent.com/24619207/163738277-e2a80193-5ae2-497e-99fd-181468e7742f.png)
![NVIDIA_Share_5ZWKPjTGmo](https://user-images.githubusercontent.com/24619207/163738279-76834f94-42ee-4bc6-845a-6eca3a60d577.png)
![NVIDIA_Share_2TTTAws7Dt](https://user-images.githubusercontent.com/24619207/163738278-5dc8afe0-9dbe-4e02-92ca-c9056e57c369.png)

(The gap in time is ridtp to surface and spawning the items in)

![image](https://user-images.githubusercontent.com/24619207/163898085-097de715-450f-47b9-adc1-ed5d019f789a.png)
