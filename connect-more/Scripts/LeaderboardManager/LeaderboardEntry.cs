using Godot; 
using Godot.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ConnectMore.Scripts.Data;

public class LeaderboardEntry
{
	public string Name { get; set; }
	public int Score { get; set; }
	
	public LeaderboardEntry() {}
	
	public LeaderboardEntry(string name, int score)
	{
		this.Name = name;
		this.Score = score;
	}
	
	public Dictionary ToGodotDict()
	{
		return new Dictionary
		{
			{ "Name", this.Name },
			{ "Score", this.Score }
		};
	}
	
	public static LeaderboardEntry FromGodotDict(Dictionary dict)
	{
		return new LeaderboardEntry
		{
			Name = dict["Name"].AsString(),
			Score = dict["Score"].AsInt32(),
		};
	}	
}

public static class LeaderboardManager
{
	private const string SavePath = "user://leaderboard.json";
	
	public static void AddScore(LeaderboardEntry newEntry)
	{
		var scores = LoadScores();
		scores.Add(newEntry);
		var sortedScores = scores.OrderByDescending(entry => entry.Score).ToList();
		SaveScores(sortedScores);
	}
	
	public static List<LeaderboardEntry> LoadScores()
	{
		var scores = new List<LeaderboardEntry>();
		
		if (!FileAccess.FileExists(SavePath))
		{
			return scores;
		}
		
		using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Read);
		string content = file.GetAsText();
		
		var parseResult = Json.ParseString(content);
		
		if (parseResult.VariantType != Variant.Type.Array)
		{
			GD.PrintErr("Leaderboard JSON is corrupted or not an array.");
			return scores;
		}
		
		foreach (var entryData in parseResult.AsGodotArray())
		{
			if (entryData.VariantType == Variant.Type.Dictionary)
			{
				scores.Add(LeaderboardEntry.FromGodotDict(entryData.AsGodotDictionary()));
			}
		}
		return scores;
	}
	
	public static void SaveScores(List<LeaderboardEntry> scores)
	{
		var dataToSave = new Array<Dictionary>();
		foreach (var entry in scores)
		{
			dataToSave.Add(entry.ToGodotDict());
		}
		
		using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Write);
		string jsonString = Json.Stringify(dataToSave, "\t");
		file.StoreString(jsonString);
	}
	
	public static void DeleteScore(int index)
	{
		var scores = LoadScores();
		if (index >= 0 && index < scores.Count)
		{
			scores.RemoveAt(index);
			SaveScores(scores);
		}
	}	
}
