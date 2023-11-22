# Crunchyroll Notifier #

<br>

## Overview ##

<br>

**Crunchyroll Notifier** is a windows application written in .Net 8.<br>
It sends notifications to the user about new episodes on Cruchyroll.<br>
This app uses the RSS feed found at [Recently Added Anime Videos](http://feeds.feedburner.com/crunchyroll/rss/anime)
provided by Cruchyroll.

<br>

## How to Use ##

<br>

### FeedConfig.json ###

<br>

**Interval**: How often to check for new episodes in seconds.<br>
**Type**: Double
**Condition**: Must be greater than 10.<br>
**Required**: Yes<br>

<br>

**Visibility**: The visibility of a episode on the website.<br>
**Type**: Enum
**Values**: All, all, Free, free, Premium, premium<br>
**Required**: Yes<br>

<br>

**MaxNotifications**: The number of episodes pulled from a feed at a time.<br>
**Type**: Integer
**Condition**: Must be from 1 to 100.<br>
**Required**: Yes<br>

<br>

**ShowFirstRun**: If the first run notification should show when the app first opens.<br>
**Type**: Bool<br>
**Required**: Yes<br>

<br>

**Dubs**: The dubs to look for. (Shows all if missing).<br>
**Type**: Array of string<br>
**Required**: No<br>

<br>

**Names**: The show names to look for. (Shows all if missing).<br>
**Type**: Array of string<br>
**Required**: No<br>

<br>

#### * **For arrays if the are included they must not be empty.** ####

<br>

### FeedConfigSchema.json ###

<br>

It is the schema used to validate the FeedConfig.json
file when the app is loaded.

<br>

## Supported Platform ##

<br>

Windows 10 Version 22621.0 & above (Including all later versions of windows such as Windows 11).

<br>

&copy; 2023 Richard Whicker