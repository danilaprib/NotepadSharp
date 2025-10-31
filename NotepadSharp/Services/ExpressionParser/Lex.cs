using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotepadSharp.Services.ExpressionParser
{
    public class Lex
    {
        public List<string> GenerateTokens(string expression)
        {
            List<string> tokens = new List<string>();

            for (int i = 0; i < expression.Length; i++)
            {
                if (expression[i] >= '0' && expression[i] <= '9')
                {
                    HashSet<string> set = new HashSet<string>();
                    string number = TokenizeNumber(expression, i);
                    tokens.Add(number);
                }
                if (expression[i] == '*' || expression[i] == '/' || expression[i] == '+' || expression[i] == '-')
                {
                    tokens.Add(expression[i].ToString());
                }
                if (expression[i] == '(')
                {
                    tokens.Add(expression[i].ToString());
                }
                if (expression[i] == ')')
                {
                    tokens.Add(expression[i].ToString());
                }
            }

            return tokens;
        }

        public string TokenizeNumber(string expression, int startNumberIndex)
        {
            StringBuilder num = new StringBuilder();
            if (expression.Length >= 2 && expression[startNumberIndex] == '0' && expression[startNumberIndex + 1] == '0')
            {
                throw new Exception("INCORRECT NUMBER FORMAT: CANNOT HAVE TRAILING ZEROES"); // crap
            }
            for (int i = startNumberIndex; i < expression.Length; i++)
            {
                if (expression[i] == '*' || expression[i] == '/' || expression[i] == '+' || expression[i] == '-' || expression[i] == '(' || expression[i] == ')')
                {
                    return num.ToString();
                }
                if (!(expression[i] >= '0' && expression[i] <= '9') && expression[i] != '.') // need to handle EOF
                {

                    throw new Exception("INCORRECT NUMBER FORMAT: CANNOT HAVE ANYTHING OTHER THAN 0...9 AND .");
                }
                else
                {
                    num.Append(expression[i]);
                }
            }

            return num.ToString();
        }
    }
}
