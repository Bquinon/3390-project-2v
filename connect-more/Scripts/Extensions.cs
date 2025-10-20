using System.Collections.Generic;
using System.Text.Json;
using ConnectMore.Scripts.Data;
using Godot;

namespace ConnectMore.Scripts;

public static class Extensions
{
	public static bool IsBetween(this int i, int start, int end)
	{
		return i >= start && i < end;
	}

	public static bool TryReadJsonPath<T>(this string path, out T result)
	{
		result = default;
		
		if (!FileAccess.FileExists(path))
		{
			return false;
		}

		using FileAccess file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
		result = JsonSerializer.Deserialize<T>(file.GetAsText());

		return true;
	}
	
	public static void WriteJson<T>(this T obj, string path)
	{
		using FileAccess file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
		file.StoreString(JsonSerializer.Serialize(obj));
	}
}
