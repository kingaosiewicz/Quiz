using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quiz.Model;
using System.Collections.Generic;

namespace Quiz.Model
{
    public static class ScoreCalculator
    {
        public static int CalculateScore(List<(Question question, int selectedIndex)> answers)
        {
            int score = 0;
            foreach (var (q, index) in answers)
            {
                if (q.Answers[index].IsCorrect)
                    score++;
            }
            return score;
        }
    }
}
