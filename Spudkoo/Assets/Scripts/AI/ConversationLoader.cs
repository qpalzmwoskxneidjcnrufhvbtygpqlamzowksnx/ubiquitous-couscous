using UnityEngine;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using OpenAI.Chat;



public class ConversationLoader : MonoBehaviour
{
    
    [SerializeField] private string originalFileName = "conversation.txt";
    [SerializeField] private string compressedFileName = "conversation.gz";
    private List<Message> testing = new();

    private void CompressFile()
    {
        // using FileStream originalFileStream = File.Open(OriginalFileName, FileMode.Open);
        using FileStream compressedFileStream = File.Create(compressedFileName);
        using var compressor = new GZipStream(compressedFileStream, CompressionMode.Compress);
        //compressor.Write(testing, 0, 0); // <----- ERROR HERE
    }

}
