

![CleanupUtility ISSUES](https://img.shields.io/github/issues/Undid-Iridium/CleanupUtility)
![CleanupUtility FORKS](https://img.shields.io/github/forks/Undid-Iridium/CleanupUtility)
![CleanupUtility LICENSE](https://img.shields.io/github/license/Undid-Iridium/CleanupUtility)


![CleanupUtility LATEST](https://img.shields.io/github/v/release/Undid-Iridium/CleanupUtility?include_prereleases&style=flat-square)
![CleanupUtility LINES](https://img.shields.io/tokei/lines/github/Undid-Iridium/CleanupUtility)
![CleanupUtility DOWNLOADS](https://img.shields.io/github/downloads/Undid-Iridium/CleanupUtility/total?style=flat-square)


# CleanupUtility

Ability to control what items get cleaned up, and when.

This solution Hijacks the methods that instantiate the ServerDropping of items, and the ServerPickup adding of ammo. I went down this route to allow the ability to lock in individual patch's per type, and force the ItemPickupBase to be grabbed after it was done being touched by NW's instantiation code (or at least right after). If you have any suggestions please don't hesitate to open an issue. Thanks

*Update* I will probably modify how the code behaves. I had originally used an internal queue system but then I allowed filtering on items making that efficiency useless since I'll have to iterate all items based on config. Additionally, My reasoning for two transpilers that were after NW's instantiation seem extreme. Supposedly, or at least from raw testing it seems to be fine to just modify ServerCallPickup which I may do later but it shouldn't change the behavior. Furthermore, I want to add debug layer's and actually cleanup the way debug is handeled.

* Add hot swappable config settings for debug
* Add Log.Batch, Log.BatchDebug
* Add Probability of item to be cleaned up 
* Add zone choices
* Add documentation in code. Also justification for queue is that I could use it for more than just an escape method (No longer true based on granular filtering, so swapping to dictionary system). Also justification for two transpilers was for after-NW touching (Not needed after some testing, reference object is delayed long enough it is a non-relevant factor. If I was cleaning items up immediately then maybe; however, behavior will remain the same in essence)

# Installation

**[EXILED](https://github.com/galaxy119/EXILED) must be installed for this to work.**

Current plugin version: V1.0.0

## REQUIREMENTS
* Exiled: V5.1.3
* SCP:SL Server: V12.0.0
* SharedLogicOrchestrator https://github.com/Undid-Iridium/SharedLogicOrchestrator (I do not like copy pasting the same configuration logic/enums/etc.)


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
