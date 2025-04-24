using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;

namespace Quiz.Model
{
    public static class QuizSerializer
    {
        public static void Save(string path, Quiz quiz)
        {
            var json = JsonSerializer.Serialize(quiz);
            File.WriteAllText(path, json);
        }

        public static Quiz Load(string path)
        {
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<Quiz>(json);
        }
    }
}
