# CleanupUtility

Ability to control what items get cleaned up, and when.

# Installation

**[EXILED](https://github.com/Exiled-Team/EXILED) must be installed for this to work.**

Example configuration
```
cleanup_utility:
  is_enabled: true
  # Whether debug logs should be shown.
  debug: true
  # The time, in seconds, between each check of the list of items to delete.
  check_interval: 2
  # The item filter. If you want an item to be removed, add it here with the time in seconds.
  item_filter:
    GrenadeHE: 15
 ```
 
![NVIDIA_Share_YmibdG6PY2](https://user-images.githubusercontent.com/24619207/163738277-e2a80193-5ae2-497e-99fd-181468e7742f.png)
![NVIDIA_Share_5ZWKPjTGmo](https://user-images.githubusercontent.com/24619207/163738279-76834f94-42ee-4bc6-845a-6eca3a60d577.png)
![NVIDIA_Share_2TTTAws7Dt](https://user-images.githubusercontent.com/24619207/163738278-5dc8afe0-9dbe-4e02-92ca-c9056e57c369.png)
