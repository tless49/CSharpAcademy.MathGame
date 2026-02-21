using System.Text;

namespace MathGame
{
    internal class Program
    {
        Random _randGenerator = new();
        List<Game> _history = new();
        int _numOfQuestions = 5;
        GameType _currentGameType = GameType.Addition;

        static void Main(string[] args)
        {
            Program program = new Program();
            program.MainMenuGameStart();
        }

        private void MainMenuGameStart()
        {
            Console.Clear();
            Console.WriteLine("This is The Math Game.");
            Console.WriteLine($"Your current score is: {CalculateScore()}.");
            Console.WriteLine("\nSelect an option:");
            Console.WriteLine($"1: Play Game (Current selection: {_currentGameType}, {_numOfQuestions} quesitons)");
            Console.WriteLine($"2: Options");
            Console.WriteLine($"3: Game History");
            Console.WriteLine($"4: Quit");

            string selection = GetMenuOption("1", "2", "3", "4");

            switch (selection)
            {
                case "1":
                    //PlayGame();
                    MainMenuGameStart();
                    break;
                case "2":
                    OptionsMenu();
                    break;
                case "3":
                    ShowGameHistory();
                    break;
                case "4":
                    return;
            }
        }
        private void OptionsMenu()
        {
            Console.Clear();
            Console.WriteLine("Options:");
            Console.WriteLine($"1: Select game type (Current: {_currentGameType})");
            Console.WriteLine($"2: Set number of questions (Current:{_numOfQuestions})");
            Console.WriteLine("3: Back");

            var selection = GetMenuOption("1", "2", "3");

            Console.Clear();
            switch (selection)
            {
                case "1":
                    Console.WriteLine("Choose Game Type");
                    Console.WriteLine("1: Addition");
                    Console.WriteLine("2: Subtraction");
                    Console.WriteLine("3: Multiplication");
                    Console.WriteLine("4: Division");
                    _currentGameType = (GameType)int.Parse(GetMenuOption("1", "2", "3", "4"));
                    break;
                case "2":
                    Console.WriteLine("Enter the number of questions per round.");
                    string inputNum = string.Empty;
                    int num;
                    while( !int.TryParse(selection, out num))
                    {
                        inputNum = Console.ReadLine();
                    }
                    _numOfQuestions = num;
                    break;
                case "3":
                    MainMenuGameStart();
                    break;
            }
            OptionsMenu();
        }
        private void ShowGameHistory()
        {
            Console.Clear();
            if (_history.Count == 0)
            {
                Console.WriteLine("Play a game to see your game history");
            }
            else
            {
                foreach (Game run in _history)
                    run.ToString();
            }
            Console.WriteLine("Press enter to continue");
            Console.ReadLine();
            MainMenuGameStart();

        }

        private string GetMenuOption(params string[] validInputs)
        {
            bool firstRun = true;
            string input = Console.ReadLine();
            while (!validInputs.Contains<string>(input))
            {
                ClearLinesAbove(1);
                if (firstRun)
                {
                    Console.WriteLine("Enter valid option:");
                    firstRun = false;
                }
                input = Console.ReadLine();
            }
            return input;
        }
        public static void ClearLinesAbove(int linesToClear)
        {
            for (int i = 0; i < linesToClear; i++)
            {
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                ClearCurrentConsoleLine();
            }
        }
        public static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }
        private string CalculateScore()
        {
            int score = 0;
            foreach(Game run in _history)
                score += run.Score;
            return score.ToString();
        }

        record Game()
        {
            public int Score;
            public string GameType;
            public List<Question> Questions;

            public override string ToString()
            {
                StringBuilder sb = new();

                sb.AppendLine($"{GameType} game with {Questions.Count} questions.");
                for ( int i = 0; i < Questions.Count; i++)
                {
                    sb.AppendLine($"\nQuesiton {i + 1}");
                    sb.AppendLine( Questions[i].ToString() );
                }
                return sb.ToString();
            }
        }

        record Question(string questionText)
        {
            public string QuestionText = questionText;
            public string Answer = "\"question not answered\"";
            public bool AnswerCorrect = false;

            public override string ToString() => QuestionText + $"\nUser's Answer: {Answer} was {(AnswerCorrect ? "correct" : "incorrect")}.";
        }

        enum GameType
        {
            Addition = 1,
            Subtraction,
            Multiplication,
            Division
        }
    }
}
