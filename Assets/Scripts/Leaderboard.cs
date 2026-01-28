using System.Collections.Generic;
using UnityEngine;

public struct LeaderboardEntry
{
    public string name;
    public float time;

    public LeaderboardEntry(string n, float t)
    {
        name = n;
        time = t;
    }
}

public static class Leaderboard
{
    private const int MaxEntries = 10;
    private const string CountKey = "LB_Count";
    private const string NameKeyPrefix = "LB_Name_";
    private const string TimeKeyPrefix = "LB_Time_";

    public static void SaveScore(string name, float time)
    {
        List<LeaderboardEntry> entries = LoadScores();

        // Add new entry
        entries.Add(new LeaderboardEntry(name, time));

        // Sort by time ascending (smaller = better)
        entries.Sort((a, b) => a.time.CompareTo(b.time));

        // Keep only top MaxEntries
        if (entries.Count > MaxEntries)
            entries = entries.GetRange(0, MaxEntries);

        SaveScores(entries);
    }

    public static List<LeaderboardEntry> LoadScores()
    {
        List<LeaderboardEntry> entries = new List<LeaderboardEntry>();

        int count = PlayerPrefs.GetInt(CountKey, 0);

        for (int i = 0; i < count; i++)
        {
            string name = PlayerPrefs.GetString(NameKeyPrefix + i, "---");
            float time = PlayerPrefs.GetFloat(TimeKeyPrefix + i, 9999f);
            entries.Add(new LeaderboardEntry(name, time));
        }

        return entries;
    }

    private static void SaveScores(List<LeaderboardEntry> entries)
    {
        PlayerPrefs.SetInt(CountKey, entries.Count);

        for (int i = 0; i < entries.Count; i++)
        {
            PlayerPrefs.SetString(NameKeyPrefix + i, entries[i].name);
            PlayerPrefs.SetFloat(TimeKeyPrefix + i, entries[i].time);
        }

        PlayerPrefs.Save();
    }
}
