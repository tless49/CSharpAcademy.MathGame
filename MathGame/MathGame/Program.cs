using System.Text;

namespace MathGame
{
    internal class Program
    {
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
                    PlayGame();
                    break;
                case "2":
                    OptionsMenu();
                    break;
                case "3":
                    ShowGameHistory();
                    break;
                case "4":
                    Environment.Exit(0);
                    return;
            }
        }
        private void PlayGame()
        {
            Console.Clear();
            Game currentGame = new(_currentGameType, _numOfQuestions);
            currentGame.RunGame();
            _history.Add(currentGame);
            MainMenuGameStart();
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
                    while( !int.TryParse(inputNum, out num))
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
                bool firstGame = true;
                for (int i = 0; i < _history.Count; i++)
                {
                    if (!firstGame)
                    {
                        Console.WriteLine("\n\n");
                        firstGame = false;
                    }
                    Console.Write($"Game {i + 1}:");
                    Console.WriteLine(_history[i].ToString());
                }
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

        record Game(GameType gameType, int numOfQuestions)
        {
            public int Score { get; private set; }
            GameType _gameType = gameType;
            int _numOfQuestions = numOfQuestions;
            public List<Question> Questions = new(numOfQuestions);

            // Debated making this private vs public. Rationale for private: no params or return value, so there's no outside user input allowed, so why expose it
            private void BuildGame()
            {
                Random rand = new();
                while (_numOfQuestions > 0)
                {
                    int one = rand.Next(0, 101);
                    int two = rand.Next(0,101);
                    int answer = 0;
                    string operation = string.Empty;
                    switch (_gameType)
                    {
                        case GameType.Addition:
                            operation = "+";
                            answer = one + two;
                            break;
                        case GameType.Subtraction:
                            operation = "-"; 
                            answer = one - two;
                            break;
                        case GameType.Multiplication:
                            operation = "*";
                            answer = one * two;
                            break;
                        case GameType.Division:
                            List<int> factors = new();
                            if (one%two != 0)
                            {
                                for (two = 1; two < one; two++)
                                {
                                    if(one%two == 0) factors.Add(two);
                                }
                            }
                            var index = rand.Next(0, factors.Count);
                            two = factors[index];
                            operation = "/";
                            answer = one / two;
                            break;
                    }
                    Questions.Add(new Question($"{one} {operation} {two}", answer));
                    _numOfQuestions--;
                }
            }
            public void RunGame()
            {
                BuildGame();
                foreach (Question question in Questions)
                {
                    Console.Clear();
                    Console.WriteLine(question.QuestionText);
                    string answer = Console.ReadLine();
                    bool answerValid = int.TryParse(answer, out int intAnswer);
                    if (!answerValid)
                    {
                        question.Answer = "Invalid Input";
                        question.AnswerCorrect = false;
                    }
                    question.Answer = answer;
                    question.AnswerCorrect = intAnswer == question.correctAnswer;
                    if (question.AnswerCorrect) Score++;
                }
            }
            public override string ToString()
            {
                StringBuilder sb = new();

                sb.AppendLine($"{_gameType} game with {Questions.Count} questions.");
                sb.AppendLine($"User scored {Score} out of {Questions.Count}.");
                for ( int i = 0; i < Questions.Count; i++)
                {
                    sb.AppendLine($"\nQuesiton {i + 1}");
                    sb.AppendLine( Questions[i].ToString() );
                }
                return sb.ToString();
            }
        }

        record Question(string questionText, int correctAnswer)
        {
            public string QuestionText { get; init; } = questionText;
            public string Answer { get; set; } = "\"question not answered\"";
            public int CorrectAnswer { get; init; } = correctAnswer;
            public bool AnswerCorrect { get; set; } = false;

            public override string ToString() 
            {
                string ans = QuestionText + $"\nUser's Answer: {Answer} was {(AnswerCorrect ? "correct" : "incorrect")}.";
                if (!AnswerCorrect) ans += $"\nCorrect answer: {CorrectAnswer.ToString()}";
                return ans;
            }

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
