using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

namespace Quiz.Model
{
    public static class QuizSerializer
    {
        public static void SaveEncrypted(string path, Quiz quiz, string password)
        {
            string json = JsonSerializer.Serialize(quiz);
            byte[] encrypted = AesEncryption.Encrypt(json, password);
            File.WriteAllBytes(path, encrypted);
        }

        public static Quiz LoadEncrypted(string path, string password)
        {
            byte[] encrypted = File.ReadAllBytes(path);
            string json = AesEncryption.Decrypt(encrypted, password);
            return JsonSerializer.Deserialize<Quiz>(json);
        }
    }
}
