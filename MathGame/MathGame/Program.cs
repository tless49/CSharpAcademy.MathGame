using System.Text;
using static System.Console;

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
            Clear();
            WriteLine("This is The Math Game.");
            WriteLine($"Your current score is: {CalculateScore()}.");
            WriteLine("\nSelect an option:");
            WriteLine($"1: Play Game (Current selection: {_currentGameType}, {_numOfQuestions} quesitons)");
            WriteLine($"2: Options");
            WriteLine($"3: Game History");
            WriteLine($"4: Quit");

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
            Clear();
            Game currentGame = new(_currentGameType, _numOfQuestions);
            currentGame.RunGame();
            _history.Add(currentGame);
            MainMenuGameStart();
        }
        private void OptionsMenu()
        {
            Clear();
            WriteLine("Options:");
            WriteLine($"1: Select game type (Current: {_currentGameType})");
            WriteLine($"2: Set number of questions (Current:{_numOfQuestions})");
            WriteLine("3: Back");

            var selection = GetMenuOption("1", "2", "3");

            Clear();
            switch (selection)
            {
                case "1":
                    WriteLine("Choose Game Type");
                    WriteLine("1: Addition");
                    WriteLine("2: Subtraction");
                    WriteLine("3: Multiplication");
                    WriteLine("4: Division");
                    _currentGameType = (GameType)int.Parse(GetMenuOption("1", "2", "3", "4"));
                    break;
                case "2":
                    WriteLine("Enter the number of questions per round.");
                    string inputNum = string.Empty;
                    int num;
                    while( !int.TryParse(inputNum, out num))
                    {
                        inputNum = ReadLine();
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
            Clear();
            if (_history.Count == 0)
            {
                WriteLine("Play a game to see your game history");
            }
            else
            {
                bool firstGame = true;
                for (int i = 0; i < _history.Count; i++)
                {
                    if (!firstGame)
                    {
                        WriteLine("\n\n");
                        firstGame = false;
                    }
                    Write($"Game {i + 1}:");
                    WriteLine(_history[i].ToString());
                }
            }
            WriteLine("Press enter to continue");
            ReadLine();
            MainMenuGameStart();

        }

        private string GetMenuOption(params string[] validInputs)
        {
            bool firstRun = true;
            string input = ReadLine();
            while (!validInputs.Contains<string>(input))
            {
                ClearLinesAbove(1);
                if (firstRun)
                {
                    WriteLine("Enter valid option:");
                    firstRun = false;
                }
                input = ReadLine();
            }
            return input;
        }
        public static void ClearLinesAbove(int linesToClear)
        {
            for (int i = 0; i < linesToClear; i++)
            {
                SetCursorPosition(0, CursorTop - 1);
                ClearCurrentConsoleLine();
            }
        }
        public static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = CursorTop;
            SetCursorPosition(0, CursorTop);
            Write(new string(' ', WindowWidth));
            SetCursorPosition(0, currentLineCursor);
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
                    Clear();
                    WriteLine(question.QuestionText);
                    string answer = ReadLine();
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
