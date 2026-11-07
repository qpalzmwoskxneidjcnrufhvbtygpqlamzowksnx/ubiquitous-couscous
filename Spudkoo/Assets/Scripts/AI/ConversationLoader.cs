using UnityEngine;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using OpenAI;
using OpenAI.Chat;
using System;

public class ConversationLoader : MonoBehaviour
{
    [SerializeField] private static string archiveFileName = "conversation.gz";
    [SerializeField] private static string logFileName = "conversation.log";

    private static string ArchivePath => Path.Combine(Application.persistentDataPath, archiveFileName);
    private static string LogPath => Path.Combine(Application.persistentDataPath, logFileName);

    // Call once in Start: returns full history and folds the log into the archive.
    public static List<Message> LoadAndCompact()
    {
        var saved = new List<SavedMessage>();
        saved.AddRange(ReadArchive());
        saved.AddRange(ReadLog());

        saved = saved.Where(s => s.role != Role.System && s.role != Role.Developer).ToList();

        WriteArchive(saved);
        if (File.Exists(LogPath)) File.Delete(LogPath);

        return SavedMessage.ToMessages(saved);
    }

    // Call every time a message is added (user message and AI reply).
    public static void Append(Message message)
    {
        if (message.Role == Role.System || message.Role == Role.Developer) return;
        string line = JsonConvert.SerializeObject(new SavedMessage(message));
        File.AppendAllText(LogPath, line + "\n", Encoding.UTF8);
    }

    private static IEnumerable<SavedMessage> ReadArchive()
    {
        if (!File.Exists(ArchivePath)) yield break;
        using var file = File.OpenRead(ArchivePath);
        using var gzip = new GZipStream(file, CompressionMode.Decompress);
        using var reader = new StreamReader(gzip, Encoding.UTF8);
        foreach (var msg in ReadLines(reader)) yield return msg;
    }

    private static IEnumerable<SavedMessage> ReadLog()
    {
        if (!File.Exists(LogPath)) yield break;
        using var reader = new StreamReader(LogPath, Encoding.UTF8);
        foreach (var msg in ReadLines(reader)) yield return msg;
    }

    private static IEnumerable<SavedMessage> ReadLines(StreamReader reader)
    {
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            SavedMessage msg = null;
            try { msg = JsonConvert.DeserializeObject<SavedMessage>(line); }
            catch (JsonException) { Debug.LogWarning("Skipping corrupt line in conversation file."); }
            if (msg != null) yield return msg;
        }
    }

    private static void WriteArchive(List<SavedMessage> messages)
    {
        // Write to a temp file first so a crash mid-write can't destroy the old archive.
        string tempPath = ArchivePath + ".tmp";
        using (var file = File.Create(tempPath))
        using (var gzip = new GZipStream(file, CompressionMode.Compress))
        using (var writer = new StreamWriter(gzip, Encoding.UTF8))
        {
            foreach (var msg in messages)
                writer.WriteLine(JsonConvert.SerializeObject(msg));
        }

        if (File.Exists(ArchivePath)) File.Delete(ArchivePath);
        File.Move(tempPath, ArchivePath);
    }
}

[Serializable]
public class SavedMessage
{
    public Role role;
    public string content;

    public SavedMessage() { } // needed for deserialization

    public SavedMessage(Message message)
    {
        role = message.Role;
        content = message.Content?.ToString() ?? "";
    }

    public Message ToMessage() => new Message(role, content);

    public static List<SavedMessage> FromMessages(IEnumerable<Message> messages) =>
        messages.Select(m => new SavedMessage(m)).ToList();

    public static List<Message> ToMessages(IEnumerable<SavedMessage> saved) =>
        saved.Select(s => s.ToMessage()).ToList();
}
