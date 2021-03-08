using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace WordSearch
{

    public class TestingBranch
    {
        public int id {get; set;}
    }

    public class WordCoordinates
    {
        public string Word { get; set; }
        public string Coorinates { get; set; }

    }
    class Program
    {
        public static int WordsInGrid { get; set; }
        public static int Rows { get; set; }
        public static int Cols { get; set; }
        public static List<String> gridWords = new List<String>();
        public static void addGridWord(string word) => gridWords.Add(word);
        public static List<String> userFoundWords = new List<String>();
        public static void addUserFoundWord(string word) => userFoundWords.Add(word);
        public static List<String> systemFoundWords = new List<String>();
        public static void addSystemFoundWord(string word) => systemFoundWords.Add(word);
        public static void Main(string[] args)
        {      
            UserInput(); //Gets grid and word parameters from the user.

            char[,] grid = NewCharGrid( Rows , Cols );
            bool[,] reservedGrid = NewBoolGrid( Rows , Cols );
            char[,] userFoundGrid = NewCharGrid( Rows , Cols );
            char[,] systemFoundGrid = NewCharGrid( Rows , Cols );

            List<String> randomWords = ImportedWords();

            PlaceWords( randomWords , grid , reservedGrid );
            RandomCharacterFill( grid , reservedGrid );
            UserSubmissions( grid , systemFoundGrid , userFoundGrid );
            Finish( grid , systemFoundGrid , userFoundGrid );
        }
        public static void SystemWordsFinder( List<String> words , char[,] letterGrid , char[,] systemFoundGrid )
        {
            for ( int i = 0 ; i < words.Count ; i++ )
            {
                NewSystemWordFinder( words[i] , letterGrid , systemFoundGrid );
            }
        }
        public static void UserInput()
        {
            Console.WriteLine( "Welcome to WordSearch!" );
            
            do
            {
                Console.WriteLine( "How many words do you want to look for? 3 minimum." );
                WordsInGrid = Convert.ToInt32(Console.ReadLine());
            }
            while ( WordsInGrid < 3 );

            do
            {
                Console.WriteLine( "How many rows do you want? 3 minimum." );
                Rows = Convert.ToInt32(Console.ReadLine());
            }
            while ( Rows < 3 );

            do
            {
                Console.WriteLine( "How many columns do you want? Minimum 3." );
                Cols = Convert.ToInt32(Console.ReadLine());
            }
            while ( Cols < 3 );
        }
        static bool UserWordFinder( char[,] grid , char[,] userFoundGrid )
        {
            bool wordFound;
            string wordSubmitted = SubmitWordForCheck();
            int rowOfFirstLetter = FirstLetterRow();
            int colOfFirstLetter = FirstLetterColumn();
            int rowOfLastLetter = LastLetterRow();
            int colOfLastLetter = LastLetterColumn();

            WriteUserWordSubmission(wordSubmitted, rowOfFirstLetter, colOfFirstLetter, rowOfLastLetter, colOfLastLetter);
            wordFound = SubmittedWordMatch(grid, wordSubmitted, rowOfFirstLetter, colOfFirstLetter, rowOfLastLetter, colOfLastLetter);

            if( wordFound == true )
            {
                int direction = DirectionCalculation( rowOfFirstLetter , colOfFirstLetter , rowOfLastLetter , colOfLastLetter );
                PlaceWordNoAudit( wordSubmitted , userFoundGrid , rowOfFirstLetter - 1 , colOfFirstLetter - 1 , direction );
            }
            return wordFound;
        }
        static bool NewSystemWordFinder( string word , char[,] grid , char[,] systemFoundGrid )
        {
            bool wordFound = false;
            int wordLength = word.Length;
            int i = 0;
            int j = 0;
            int d = 0;
            string rowRule;
            string colRule;

            while( wordFound == false & i < Rows )
            {
                j = 0;
                while( wordFound == false & j < Cols )
                {
                    d = 0;
                    while( wordFound == false & d < 8 )
                    {
                        rowRule = RowDirectionRules( d );
                        colRule = ColDirectionRules( d );
                        if( InBounds( wordLength , i , j , d ) == true )
                        {
                            wordFound = systemWordMatch( grid , word , i , j , RowCalculation( i , wordLength , rowRule ), ColCalculation( j , wordLength , colRule ));
                        }
                        
                        if( wordFound == true )
                        {
                            PlaceWordNoAudit( word , systemFoundGrid , i , j , d );
                        }
                        d++;
                    }
                    j++;
                }
                i++;
            }
            
            if( wordFound == true )
            {
                addSystemFoundWord( word );
            }
            
            return wordFound;
        }
        public static void UserSubmissions( char[,] charArray , char[,] systemFoundGrid , char[,] userFoundGrid )
        { 
            bool userStuck = false;
            bool wordFound;
            while ( userStuck == false & gridWords.Count != ( userFoundWords.Count + systemFoundWords.Count ) )
            {
                for ( int i = 0 ; i < gridWords.Count ; i++ )
                {
                    if( userStuck == false )
                    {
                    PrintCharGrid( charArray , systemFoundGrid , userFoundGrid );
                    PrintWords( gridWords );
                    }

                    wordFound = false;
                    while ( wordFound == false & userStuck == false )
                    {
                        if ( userStuck == false )
                        {
                            userStuck = UserStuck();
                        }
                        if ( userStuck == false )
                        {
                            wordFound = UserWordFinder( charArray , userFoundGrid );
                        }
                        else
                        {
                            SystemWordsFinder( RemainingWords() , charArray , systemFoundGrid );
                        }
                    }
                }
            }
        }
        static List<string> RemainingWords()
        {
            List<string> remainingWords = new List<string>();
            foreach ( string word in gridWords )
            {
                remainingWords.Add( word );
            }
            foreach ( string uWord in userFoundWords )
            {
                remainingWords.Remove( uWord );
            }
            return remainingWords;
        }
        static void Finish( char[,] grid , char[,] systemFoundGrid , char[,] userFoundGrid )
        {
            PrintCharGrid( grid , systemFoundGrid , userFoundGrid );
            
            PrintWords( gridWords );
            if ( userFoundWords.Count == gridWords.Count )
            {
                Console.WriteLine( "You found all of the words. Congratulations!" );
            }
            else
            {
                Console.WriteLine( "You found " + userFoundWords.Count + " of " + gridWords.Count + " words!" );
            }
        }
        static bool UserStuck()
        {
            bool userStuck = false;
            bool validResponce = false;
            string responce = "";
            string yes = "yes";
            string no = "no";

            while( validResponce == false )
            {
                Console.WriteLine( "Let the system find the words? ( yes / no )" );
                responce = Console.ReadLine();
                if ( responce == yes || responce == no )
                {
                    validResponce = true;
                }
                else
                {
                    Console.WriteLine( "I didn't recognise your answer, please use 'yes' or 'no'.");
                }
            }
            if ( responce == yes )
            {
                userStuck = true;
            }
            return userStuck;
        }
        static string SubmitWordForCheck()
        {
            Console.WriteLine( "To submit a found word, enter the word." );
            string submittedWord = Console.ReadLine();
            return submittedWord;
        }
        static int FirstLetterRow()
        {
            Console.WriteLine( "Enter the first row of the first letter. ( E.g. 1 )" );
            string input = Console.ReadLine();
            int i;

            var isNumeric = int.TryParse( input , out i );

            return i;
        }
        static int FirstLetterColumn()
        {
            Console.WriteLine( "Enter the first column of the first letter. ( E.g. 1 )" );
            string input = Console.ReadLine();
            int i;

            var isNumeric = int.TryParse( input , out i );

            return i;
        }
        static int LastLetterRow()
        {
            Console.WriteLine( "Enter the last row of the last letter. ( E.g. 1 )" );
            string input = Console.ReadLine();
            int i;

            var isNumeric = int.TryParse( input , out i );

            return i;
        }
        static int LastLetterColumn()
        {
            Console.WriteLine( "Enter the last column of the last letter. ( E.g. 3 )" );
            string input = Console.ReadLine();
            int i;

            var isNumeric = int.TryParse( input , out i );

            return i;
        }
        static void WriteUserWordSubmission( string word , int firstRow , int firstCol , int lastRow , int lastCol )
        {
            Console.WriteLine( "Word submitted: " + word + "\t" );
            Console.WriteLine( "at grid reference: " + "R" + firstRow + "C" + firstCol + " to R" + lastRow + "C" + lastCol );
        }
        static bool SubmittedWordMatch( char[,] charArray , string word , int firstRow , int firstCol , int lastRow , int lastCol )
        {
            bool wordFound = UserFoundWord( charArray , word , firstRow -1 , firstCol -1 , lastRow -1 , lastCol -1 );

            if ( wordFound == true )
            {
                Console.WriteLine("That's right!");
                addUserFoundWord(word);
            }
            else
            {
                Console.WriteLine("Sorry that's not quite right. Please try again.");
            }
            return wordFound;
        }
        static bool systemWordMatch( char[,] charArray , string word , int firstRow , int firstCol , int lastRow , int lastCol )
        {
            bool wordFound = UserFoundWord( charArray , word , firstRow , firstCol , lastRow , lastCol );

            if ( wordFound == true )
            {
                addSystemFoundWord(word);
            }

            return wordFound;
        }
        public static char[,] NewCharGrid(int r, int c) //Creates the character grid array.
        {
            char[,] blankGrid = new char[ r , c ];
            return blankGrid;
        }
        public static bool[,] NewBoolGrid(int r, int c) //Creates the reserved grid array.
        {
            bool[,] reserved;
            reserved = new bool[ r , c ];

            for ( int i = 0 ; i < r ; i++ )
            {
                for ( int j = 0 ; j < c ; j++ )
                {
                    reserved[ i , j ] = false;
                }
            }
            return reserved;
        }
        static void PrintCharGrid(char[,] grid , char[,] systemFoundGrid , char[,] userFoundGrid ) //Prints the charracter grid array.
        {
            for ( int i = 0 ; i <= Rows ; i++ )
            {
                for ( int j = 0 ; j <= Cols ; j++ )
                {
                    if ( i == 0 && j == 0 )
                    {
                        Console.Write( "\t" );
                    }
                    else
                    {
                        if (i == 0)
                        {
                            Console.Write( "C" + j + "\t" );
                        }
                        else
                        {
                            if (j == 0)
                            {
                                Console.Write( "R" + i + "\t" );
                            }
                            else
                            {
                                if( systemFoundGrid[ i - 1 , j - 1 ] == grid[ i - 1 , j - 1 ] )
                                {
                                    Console.BackgroundColor = ConsoleColor.Red;
                                    Console.ForegroundColor = ConsoleColor.Black;
                                    Console.Write( grid[ i - 1 , j - 1 ] );
                                    Console.BackgroundColor = ConsoleColor.Black;
                                    Console.ForegroundColor = ConsoleColor.White;
                                    Console.Write( "\t" );
                                }
                                else
                                {
                                    if( userFoundGrid[ i - 1 , j - 1 ] == grid[ i - 1 , j - 1 ])
                                    {
                                        Console.BackgroundColor = ConsoleColor.Green;
                                        Console.ForegroundColor = ConsoleColor.Black;
                                        Console.Write( grid[ i - 1 , j - 1 ] );
                                        Console.BackgroundColor = ConsoleColor.Black;
                                        Console.ForegroundColor = ConsoleColor.White;
                                        Console.Write( "\t" );
                                    }
                                    else
                                    {
                                        Console.Write( grid[ i - 1 , j - 1 ] );
                                        Console.Write( "\t" );
                                    }
                                }
                            }
                        }
                    }
                }
                Console.WriteLine( "\n" );
            }
        }

        static void PrintWords(List<String> words) //Prints the random words list.
        {
            WordCoordinates wc = new WordCoordinates();
            foreach (string word in words)
            {
                if ( userFoundWords.Contains( word ) == true )
                {
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write( word );
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    if ( RemainingWords().Contains ( word ) == true )
                    {
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write( word );
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.Write(word);
                    }
                }
                Console.WriteLine("\n");
            }
        }
        static void RandomCharacterFill(char[,] charArray, bool[,] boolArray) //Fills all null values in the grid with a random letter.
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++ )
                {
                    if (boolArray[i,j] == false)
                    {
                        charArray[i,j] = GetRandomLetter();
                    }
                }
            }
        } 
        static void PlaceWords(List<String> words, char[,] charArray, bool[,] boolArray) //Performs checks and Places all random words.
        {
            int placeRow = 0;
            int placeCol = 0;
            List<int> directionList;
            int wordIndex = 0;
            string word;
            int w = 0;
            bool nextWord;

            while ( gridWords.Count < WordsInGrid & w != words.Count )
            {
                word = words[w];
                placeRow = GetRandomRow();
                placeCol = GetRandomCol();
                directionList = Shuffle(DirectionIndexes());
                nextWord = false;
                int i = 0;
                while ( nextWord == false && i < directionList.Count )
                {
                    if ( PlacementClear( charArray , boolArray , word , word.Length , placeRow , placeCol , directionList[i] ) == true )
                    {
                        PlaceWord(word, charArray, boolArray, placeRow, placeCol, directionList[i] );
                        gridWords.Add(word);
                        nextWord = true;
                        wordIndex = wordIndex +1;
                    }
                    i++;
                }
                w++;
            }
        }
        static bool PlacementClear( char[,] charArray , bool[,] boolArray , string word , int wordLength , int row , int col , int direction )
        {
            bool boundsCheck = false;
            bool reservedCheck = false;
            bool placementClear = false;

            if ( InBounds( wordLength , row , col , direction ) == true )
            {
                boundsCheck = true;
            }

            if ( boundsCheck == true )
            {
                if ( Reserved( charArray , boolArray , word , wordLength , row , col , direction ) == false )
                {
                    reservedCheck = true;
                }
            }
            if ( boundsCheck == true && reservedCheck == true )
            {
                placementClear = true;
            }
            return placementClear;
        }
        static bool Reserved( char[,] charArray , bool[,] boolArray , string word , int wordLength , int row , int col , int direction )
        {
            bool reserved;
            bool taken;
            int reservedCount = 0;
            char[] letters = word.ToCharArray();

            for ( int i = 0 ; i < wordLength ; i++ )
            {
                taken = ReservedCheck( charArray , boolArray , letters[i] , RowCalculation( row , i , RowDirectionRules(direction) ) , ColCalculation( col , i , ColDirectionRules(direction) ) );
                if (taken == false)
                {
                    reservedCount++;
                }
            }
            if ( reservedCount == wordLength )
            {
                reserved = false;
            }
            else
            {
                reserved = true;
            }
            return reserved;
        }
        static bool ReservedCheck( char[,] charArray , bool[,] boolArray , char letter , int row, int col )
        {
            bool reserved = true;
            if ( charArray[ row , col ] == letter || boolArray[ row , col ] == false )
            {
                reserved = false;
            }
            return reserved;
        }
        static bool InBounds( int wordLength , int row , int col , int direction )
        {
            bool inBounds = false;
            int inBoundsCount = 0;
            
            for ( int i = 0; i < wordLength; i++ )
            {
                if ( InBoundsCheck( RowCalculation( row , i , RowDirectionRules( direction ) ) , ColCalculation( col , i , ColDirectionRules( direction ) ) ) == true )
                {
                    inBoundsCount++;
                }
            }
            if ( inBoundsCount == wordLength )
            {
                inBounds = true;
            }
            return inBounds;
        }
        static bool InBoundsCheck( int row , int col )
        {
            bool inBoundsCheck;
            if ( row >= 0 && col >= 0 && row < Rows && col < Cols )
            {
                inBoundsCheck = true;
            }
            else
            {
                inBoundsCheck = false;
            }
            return inBoundsCheck;
        }
        static void PlaceWord( string word , char[,] grid , bool[,] reserved , int row , int col , int directionIndex )  //Places each letter of the word in the array, follows direction placement rules.
        {
            int wordLength = word.Length;
            string rowRule = RowDirectionRules( directionIndex );
            string colRule = ColDirectionRules( directionIndex );
            char[] letters = word.ToCharArray();
            for ( int i = 0 ; i < wordLength ; i++ )
            {
                grid[ RowCalculation( row , i , rowRule ) , ColCalculation( col , i , colRule ) ] = letters[i];
                reserved[ RowCalculation( row , i , rowRule ) , ColCalculation( col , i , colRule ) ] = true;
            }
        }
        static void PlaceWordNoAudit(string word, char[,] grid, int row, int col, int directionIndex) //Places each letter of the word in the array, follows direction placement rules.
        {
            int wordLength = word.Length;
            string rowRule = RowDirectionRules( directionIndex );
            string colRule = ColDirectionRules( directionIndex );
            char[] letters = word.ToCharArray();
            for ( int i = 0 ; i < wordLength ; i++ )
            {
                grid[ RowCalculation( row , i , rowRule ) , ColCalculation( col , i , colRule ) ] = letters[i];
            }
        }
        static Random rand = new Random();
        public static int GetRandomRow()  //Returns a random row in the bounds of the array.
        {
            int randomNumber = rand.Next(0, Rows -1);
            return randomNumber;
        }
        public static int GetRandomCol() //Returns a random column in the bounds of the array.
        {
            int randomNumber = rand.Next(0, Cols -1);
            return randomNumber;
        }
        public static char GetRandomLetter() //Returns a random letter from a to z.
        {
            int randomNumber = rand.Next(0, 26);
            char randomLetter = (char)('a' + randomNumber);
            return randomLetter;
        }
        public static List<int> DirectionIndexes() //Array of direction indexes.
        {
            List<int> directionIndexes = new List<int>();
            directionIndexes.Add(0);
            directionIndexes.Add(1);
            directionIndexes.Add(2);                        
            directionIndexes.Add(3);
            directionIndexes.Add(4);
            directionIndexes.Add(5);
            directionIndexes.Add(6);
            directionIndexes.Add(7);
            return directionIndexes;
        }
        public static string[,] forwardDirectionRules = new string[,] { {"N","-",""}, {"E","","+"}, {"S","+",""}, {"W","","-"}, {"NE","-","+"}, {"SE","+","+"}, {"SW","+","-"}, {"NW","-","-"} }; //Array of directions and respective placement rules.
        public static int RowCalculation(int row, int letter, string rowRule) //Returns the relative row position. Derrived by the current position, nth letter and direction.
        {
            DataTable dt = new DataTable();
            int rowValue = (int)dt.Compute(row + (rowRule + letter), "");
            return rowValue;
        }
        public static int ColCalculation(int col, int letter, string colRule) //Returns the relative column position. Derrived by the current position, nth letter and direction.
        {
            DataTable dt = new DataTable();
            int colValue = (int)dt.Compute(col + (colRule + letter), "");
            return colValue;
        }
        public static string RowDirectionRules(int directionIndex) //Returns a row placement modifier based on the directionIndex. E.g. North is -, South is + and West and East is 0 *. 
        {
            string rowRule = forwardDirectionRules.GetValue(directionIndex,1).ToString();
            return rowRule;
        }
        public static string ColDirectionRules(int directionIndex) //Returns a column placement modifier based on the directionIndex. E.g. West is -, East is + and North and South is 0 *. 
        {
            string colRule = forwardDirectionRules.GetValue(directionIndex,2).ToString();
            return colRule; 
        }
        public static List<String> ImportedWords() //Imports a string[] from a .txt file, creates a List<String>. Adds strings to List<String> if the string[].Length is <= the grid size.
        {
            string fileName = "CommonWords.txt";
            string path = Path.Combine(Environment.CurrentDirectory, @"Data\", fileName);
            string[] importedWords = File.ReadAllLines( path, Encoding.UTF8);
            
            List<String> list = new List<String>();

            for (int i = 0; i < importedWords.Length; i++)
            {
                if (importedWords[i].Length <= Rows || importedWords[i].Length <= Cols)
                {
                    list.Add(importedWords[i]);
                }
            }
            list = Shuffle(list);

            return list;
        }
        static List<T> Shuffle<T>(List<T> list) //Randomises a list of values.
        {
            for (var i = 0; i < list.Count; i++)
            {
                int k = rand.Next(0, i);
                T value = list[k];
                list[k] = list[i];
                list[i] = value;
            }
            return list;
        }
        static bool UserFoundWord(char[,] grid, string word, int rowOfFirstLetter, int colOfFirstLetter, int rowOfLastLetter, int colOfLastLetter)
        {
            bool wordFound = false;
            int wordLength = word.Length;
            char[] letters = word.ToCharArray();
            int lettersMatched = 0;
            int directionIndex = DirectionCalculation(rowOfFirstLetter, colOfFirstLetter, rowOfLastLetter, colOfLastLetter);
            string rowRule = RowDirectionRules(directionIndex);
            string colRule = ColDirectionRules(directionIndex);

            if ( InBounds( wordLength , rowOfFirstLetter , colOfFirstLetter , directionIndex ) == true && directionIndex != 99 )
            {
                for (int i = 0; i < wordLength; i++)
                {
                char gridLetter = grid[RowCalculation(rowOfFirstLetter, i, rowRule), ColCalculation(colOfFirstLetter, i, colRule)];
                if (gridLetter == letters[i])
                {
                    lettersMatched ++;
                }
                }
                if (lettersMatched == wordLength)
                {
                    wordFound = true;
                }
            }
            return wordFound;
        }   
        static int DirectionCalculation(int rowOfFirstLetter, int colOfFirstLetter, int rowOfLastLetter, int colOfLastLetter)
        {
            bool north = false;
            bool south = false;
            bool west = false;
            bool east = false;
            int directionIndex = 99;

            if( rowOfFirstLetter > rowOfLastLetter )
            {
                north = true;
            }
            if( rowOfFirstLetter < rowOfLastLetter )
            {
                south = true;
            }
            if( colOfFirstLetter > colOfLastLetter )
            {
                west = true;
            }
            if( colOfFirstLetter < colOfLastLetter )
            {
                east = true;
            }

            if( north == true && west == false && east == false )
            {
                directionIndex = 0; //north
            }
            if( east == true && north == false && south == false )
            {
                directionIndex = 1; //east
            }
            if( south == true && west == false && east == false )
            {
                directionIndex = 2; //south
            }
            if ( west == true && north == false && south == false )
            {
                directionIndex = 3; //west
            }  
            if( north == true && east == true )
            {
                directionIndex = 4; //north east
            }
            if( south == true & east == true )
            {
                directionIndex = 5; //south east
            }
            if ( south == true && west == true )
            {
                directionIndex = 6; //south west
            }
            if ( north == true && west == true )
            {
                directionIndex = 7; //north west
            }   
            return directionIndex;
        }
        static void PrintBasicCharGrid( char[,] grid ) //Prints the charracter grid array.
        {
            for ( int i = 0 ; i <= Rows ; i++ )
            {
                for ( int j = 0 ; j <= Cols ; j++ )
                {
                    if ( i == 0 && j == 0 )
                    {
                        Console.Write( "\t" );
                    }
                    else
                    {
                        if (i == 0)
                        {
                            Console.Write( "C" + j + "\t" );
                        }
                        else
                        {
                            if (j == 0)
                            {
                                Console.Write( "R" + i + "\t" );
                            }
                            else
                            {
                                Console.Write( grid[ i - 1 , j - 1 ] );
                                Console.Write( "\t" );
                            }
                        }
                    }
                }
                Console.WriteLine( "\n" );
            }
        }
        static void PrintBoolGrid(bool[,] grid) //Prints the reserved grid array.
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    Console.Write(grid[i,j] + "\t");
                }
                Console.WriteLine("\n");
            }
        }
    }
}
