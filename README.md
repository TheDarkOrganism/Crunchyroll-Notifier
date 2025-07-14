# Crunchyroll Notifier #

<br>

## Overview ##

<br>

**Crunchyroll Notifier** is a windows application written in .Net 9.<br>
It sends notifications to the user about new episodes from Crunchyroll.<br>
This app uses the RSS feeds found at [Recently Added Anime Videos (Crunchyroll)](http://www.crunchyroll.com/rss/anime)<br>
or [Recently Added Anime Videos (FeedBurner)](http://feeds.feedburner.com/crunchyroll/rss/anime).

<br>

## How to Use ##

<br>

### Config.json ###

<br>

**interval**: How often to check for new episodes.<br>
**Type**: [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br>
**Condition**: Must be greater than or equal to 10 seconds.<br>
**Default**: 30 seconds<br>
**Required**: Yes<br>

<br>

**maxNotifications**: The number of episodes pulled from a feed at a time.<br>
**Type**: [Integer](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>
**Condition**: Must be from 1 to 100.<br>
**Default**: 30<br>
**Required**: Yes<br>

<br>

**showFirstRun**: If there should be a notification when the app first opens (Sets to false after the notification is displayed).<br>
**Type**: [Bool](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>
**Default**: true<br>
**Required**: Yes<br>

<br>

**useLogging**: If logs should be saved to the computer.<br>
**Type**: [Bool](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>
**Default**: false<br>
**Required**: No<br>

<br>

**visibility**: The visibility of an episode on the website.<br>
**Type**: [VisibilityType](https://github.com/TheDarkOrganism/Crunchyroll-Notifier/blob/WinUIPort/WinUIApp/Enums/VisibilityType.cs)<br>
**Values**: Default, default, Free, free, Premium, premium<br>
**Default**: Default<br>
**Required**: Yes<br>

<br>

**feedHost**: The host to get the RSS feed from (Falls back to other hosts when the selected host can't be reached).<br>
**Type**: [FeedHostType](https://github.com/TheDarkOrganism/Crunchyroll-Notifier/blob/WinUIPort/WinUIApp/Enums/FeedHostType.cs)<br>
**Values**: Crunchyroll, crunchyroll, FeedBurner, feedburner<br>
**Default**: Crunchyroll<br>
**Required**: Yes<br>

<br>

**dubs**: The dubs to look for (Shows all if missing or empty).<br>
**Type**: [Array](https://learn.microsoft.com/en-us/dotnet/api/system.array) of [string](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>
**Default**: Empty<br>
**Required**: No<br>

<br>

**names**: The show names to look for (Shows all if missing or empty).<br>
**Type**: [Array](https://learn.microsoft.com/en-us/dotnet/api/system.array) of [string](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>
**Default**: Empty<br>
**Required**: No<br>

<br>

## Supported Platform ##

<br>

Windows 10 Version 17763.0 and above (Including all later versions of windows such as Windows 11).

<br>

&copy; 2025 Richard Whicker