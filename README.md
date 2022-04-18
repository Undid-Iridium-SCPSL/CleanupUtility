

![CleanupUtility ISSUES](https://img.shields.io/github/issues/Undid-Iridium/CleanupUtility)
![CleanupUtility FORKS](https://img.shields.io/github/forks/Undid-Iridium/CleanupUtility)
![CleanupUtility LICENSE](https://img.shields.io/github/license/Undid-Iridium/CleanupUtility)


![CleanupUtility LATEST](https://img.shields.io/github/v/release/Undid-Iridium/CleanupUtility?include_prereleases&style=flat-square)
![CleanupUtility LINES](https://img.shields.io/tokei/lines/github/Undid-Iridium/CleanupUtility)
![CleanupUtility DOWNLOADS](https://img.shields.io/github/downloads/Undid-Iridium/CleanupUtility/total?style=flat-square)


# CleanupUtility

Ability to control what items get cleaned up, and when.

This solution Hijacks the methods that instantiate the ServerDropping of items, and the ServerPickup adding of ammo. 


# Installation

**[EXILED](https://github.com/galaxy119/EXILED) must be installed for this to work.**

Current plugin version: V1.0.0

## REQUIREMENTS
* Exiled: V5.1.3
* SCP:SL Server: V12.0.0


Example configuration
```
cleanup_utility:
# Whether to enable or disable plugin
  is_enabled: true
  # The message to show most debug messages.
  debug_enabled: true
  # Debug filter for logging levels.
  debug_filters:
    All: true
    Fine: true
    Finer: true
    Finest: true
  # Amount of time a thread will try to remove items before breaking until the next set of data is available. This is to prevent thread for constantly spinning with items it can't remove yet
  spinout_time: 00:00:30
  # Item filter. If you want an item to be removed, add it here with a time associated
  item_filter:
    GrenadeHE: 00:00:15
 ```
 
![NVIDIA_Share_YmibdG6PY2](https://user-images.githubusercontent.com/24619207/163738277-e2a80193-5ae2-497e-99fd-181468e7742f.png)
![NVIDIA_Share_5ZWKPjTGmo](https://user-images.githubusercontent.com/24619207/163738279-76834f94-42ee-4bc6-845a-6eca3a60d577.png)
![NVIDIA_Share_2TTTAws7Dt](https://user-images.githubusercontent.com/24619207/163738278-5dc8afe0-9dbe-4e02-92ca-c9056e57c369.png)

This also requires https://github.com/Undid-Iridium/SharedLogicOrchestrator as I refuse to reuse logic and I believe Exiled/NW needs a better logging level management. 
