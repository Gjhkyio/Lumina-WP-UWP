using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Windows.Storage;

namespace Lumina
{
    public static class SessionManager
    {
        // Key = $"{userId}_{model}"
        public static Dictionary<string, ChatSession> ActiveSessions = new Dictionary<string, ChatSession>();

        // Create or fetch the session for this userId+model
        public static ChatSession GetOrCreateSession(string userId, string selectedModel)
        {
            string key = $"{userId}_{selectedModel}";
            if (!ActiveSessions.ContainsKey(key))
            {
                var session = new ChatSession
                {
                    UserId = userId,
                    ModelUsed = selectedModel
                };
                ActiveSessions[key] = session;
            }
            return ActiveSessions[key];
        }

        public static void ClearSessions()
        {
            ActiveSessions.Clear();
        }

        // Save all sessions to ChatSessions.json
        public static async Task SaveSessionsAsync()
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            var file = await localFolder.CreateFileAsync("ChatSessions.json", CreationCollisionOption.ReplaceExisting);

            var sessionsList = new List<ChatSession>(ActiveSessions.Values);
            using (var stream = await file.OpenStreamForWriteAsync())
            {
                var serializer = new DataContractJsonSerializer(typeof(List<ChatSession>));
                serializer.WriteObject(stream, sessionsList);
            }
        }

        // Load all sessions and re-key by userId_model
        public static async Task LoadSessionsAsync()
        {
            try
            {
                var localFolder = ApplicationData.Current.LocalFolder;
                var file = await localFolder.GetFileAsync("ChatSessions.json");
                using (var stream = await file.OpenStreamForReadAsync())
                {
                    var serializer = new DataContractJsonSerializer(typeof(List<ChatSession>));
                    var sessionsList = (List<ChatSession>)serializer.ReadObject(stream);

                    ActiveSessions.Clear();
                    foreach (var session in sessionsList)
                    {
                        string key = $"{session.UserId}_{session.ModelUsed}";
                        ActiveSessions[key] = session;
                    }
                }
            }
            catch (FileNotFoundException)
            {
                // first run: no file yet
            }
        }
    }
}
