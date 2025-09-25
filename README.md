# Crunchyroll Notifier

## Overview

<p>
	<strong>Crunchyroll Notifier</strong> is a windows application written in .Net 9.<br>
	It sends notifications to the user about new episodes from Crunchyroll.<br>
	This app uses the RSS feeds found at <a href="http://www.crunchyroll.com/rss/anime">Recently Added Anime Videos (Crunchyroll)</a><br>
	or <a href="http://feeds.feedburner.com/crunchyroll/rss/anime">Recently Added Anime Videos (FeedBurner)</a>.
</p>

## Configuration

### appsettings.json

#### Config

<p>
	<strong>interval</strong>: How often to check for new episodes.
	<strong>Type</strong>: <a href="https://learn.microsoft.com/en-us/dotnet/api/system.timespan">TimeSpan</a>
	<strong>Condition</strong>: Must be greater than or equal to 10 seconds.
	<strong>Default</strong>: 30 seconds
	<strong>Required</strong>: Yes
</p>

<p>
	<strong>maxNotifications</strong>: The number of episodes pulled from a feed at a time.<br>
	<strong>Type</strong>: <a href="https://learn.microsoft.com/en-us/dotnet/api/system.int32">Integer</a><br>
	<strong>Condition</strong>: Must be from 1 to 100.<br>
	<strong>Default</strong>: 30<br>
	<strong>Required</strong>: Yes
</p>

<p>
	<strong>showFirstRun</strong>: If there should be a notification when the app first opens (Sets to false after the notification is displayed).<br>
	<strong>Type</strong>: <a href="https://learn.microsoft.com/en-us/dotnet/api/system.boolean">Bool</a><br>
	<strong>Default</strong>: true<br>
	<strong>Required</strong>: Yes
</p>

<p>
	<strong>useLogging</strong>: If logs should be saved to the computer.<br>
	<strong>Type</strong>: <a href="https://learn.microsoft.com/en-us/dotnet/api/system.boolean">Bool</a><br>
	<strong>Default</strong>: false<br>
	<strong>Required</strong>: No
</p>

<p>
	<strong>logLevel</strong>: The level of logging to use (Only used if useLogging is true).<br>
	<strong>Type</strong>: <a href="https://github.com/serilog/serilog/blob/dev/src/Serilog/Events/LogEventLevel.cs">LogEventLevel</a><br>
	<strong>Values</strong>: Information, information, Warning, warning, Error, error, Fatal, fatal<br>
	<strong>Default</strong>: Information<br>
	<strong>Required</strong>: No
</p>

<p>
	<strong>visibility</strong>: The visibility of an episode on the website.<br>
	<strong>Type</strong>: <a href="https://github.com/TheDarkOrganism/Crunchyroll-Notifier/blob/WinUIPort/WinUIApp/Enums/VisibilityType.cs">VisibilityType</a><br>
	<strong>Values</strong>: Default, default, Free, free, Premium, premium<br>
	<strong>Default</strong>: Default<br>
	<strong>Required</strong>: Yes
</p>

<p>
	<strong>feedHost</strong>: The host to get the RSS feed from (Falls back to other hosts when the selected host can't be reached).<br>
	<strong>Type</strong>: <a href="https://github.com/TheDarkOrganism/Crunchyroll-Notifier/blob/WinUIPort/WinUIApp/Enums/FeedHostType.cs">FeedHostType</a><br>
	<strong>Values</strong>: Crunchyroll, crunchyroll, FeedBurner, feedburner<br>
	<strong>Default</strong>: Crunchyroll<br>
	<strong>Required</strong>: Yes
</p>

<p>
	<strong>dubs</strong>: The dubs to look for (Shows all if missing or empty).<br>
	<strong>Type</strong>: <a href="https://learn.microsoft.com/en-us/dotnet/api/system.array">Array</a> of <a href="https://learn.microsoft.com/en-us/dotnet/api/system.string">string</a><br>
	<strong>Default</strong>: Empty<br>
	<strong>Required</strong>: No
</p>

<p>
	<strong>names</strong>: The show names to look for (Shows all if missing or empty).<br>
	<strong>Type</strong>: <a href="https://learn.microsoft.com/en-us/dotnet/api/system.array">Array</a> of <a href="https://learn.microsoft.com/en-us/dotnet/api/system.string">string</a><br>
	<strong>Default</strong>: Empty<br>
	<strong>Required</strong>: No
</p>

### appsettings.Development.json

<p>
	Use this file to override settings in appsettings.json when debugging the application.
</p>

### LastUpdate.json

#### LastUpdate

<p>
	<strong>lastUpdate</strong>: The date and time of the last episode notification.<br>
	<strong>Type</strong>: <a href="https://learn.microsoft.com/en-us/dotnet/api/system.datetime">DateTime</a><br>
	<strong>Default</strong>: <a href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null">null</a><br>
	<strong>Required</strong>: No
</p>

## Supported Platform

<p>
	Windows 10 Version 17763.0 and above (Including all later versions of windows such as Windows 11).
</p>

&copy; <time>2025</time> Richard Whicker