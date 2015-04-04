using UnityEngine;
using System.Collections;

[ExecuteInEditMode()]

public class PuzzleDifficulty : MonoBehaviour 
{
	// Classes
	private PuzzleGameStart cGameStart;
	private ScreenFit cScreenFit;
	private PuzzleSound cSound;
	
	// Skins
	public GUISkin SetSkin;

	// Get Texture
	private Texture2D[] Textures = new Texture2D[4];
	// Where we load Images from
	private string ResourcesLocation = "Puzzle/";
	
	// rectangles
	private Rect rcBackground;	// Background Image
	private Rect rcDrawArea;	// where we draw GUILayout
	private Rect rcReallyQuit;
	private Rect rcYesButton;
	private Rect rcNoButton;
	
	// Screen Settings
	internal int newWidth;		// New Aspect screen width
	internal int newHeight;		// New Aspect screen height
	
	internal GameState CurrentState;
	internal DifficultyState CurrentDifficulty;

	// Game stuff	// Game stuff
	internal int GamePuzzleCorrect = 0;		// When completed puzzle piece show this
	internal int GameVictory = 0;			// When completed an entire puzzle or all of the puzzles
	internal bool bShowReallyQuit;
	internal bool bShowEndGameButton;

	// Game Animation
	internal bool bShowPuzzleFlip;
	internal bool bShowCorrectCircle;

	// Use this for initialization
	void Start ()
	{
		// Gets GameStart Class
		cGameStart = GetComponent<PuzzleGameStart>();
		// Gets ScreenFit Class
		cScreenFit = GetComponent<ScreenFit>();
		// Initialize Screen Settings
		cScreenFit.init();
		cSound = GetComponent<PuzzleSound>();
		
		// Add Images
		Object textures01 = Resources.Load(ResourcesLocation + "Difficulty/BackDifficulty", typeof(Texture2D));
		Object textures02 = Resources.Load(ResourcesLocation + "Game/BackGame", typeof(Texture2D));
		Object textures03 = Resources.Load(ResourcesLocation + "Game/QuitText", typeof(Texture2D));
		Object textures04 = Resources.Load(ResourcesLocation + "Game/WinMessage02", typeof(Texture2D));
		Textures[0] = (Texture2D)textures01;
		Textures[1] = (Texture2D)textures02;
		Textures[2] = (Texture2D)textures03;
		Textures[3] = (Texture2D)textures04;
		
		// Setting Screen sizes
		newWidth = cScreenFit.width;
		newHeight = cScreenFit.height;
		
		// Rectangle Images
		rcBackground	= new Rect(0, 0, Screen.width, Screen.height);
		rcDrawArea		= new Rect(0, 0, newWidth,	   newHeight);
		
		rcReallyQuit	= new Rect( (newWidth / 2 * 1) - (400) 			/ 2,	newHeight / 10 * 3,	400,			200			);
		rcYesButton		= new Rect( (newWidth / 4 * 1) - (360 / 10 * 9) / 2,	newHeight / 10 * 5,	360 / 10 * 9,	240 / 10 * 9);
		rcNoButton		= new Rect( (newWidth / 4 * 3) - (360 / 10 * 9) / 2,	newHeight / 10 * 5,	360 / 10 * 9,	240 / 10 * 9);
		// Everytime we open this file the game state is always choose Difficulty
		CurrentState = GameState.ChooseDifficulty;
		// By default Choose easy
		CurrentDifficulty = DifficultyState.easy;

		bShowPuzzleFlip = false;
	}

	// Will Display the menu
	void OnGUI()
	{
		if(SetSkin)
			GUI.skin = SetSkin;

		// Set Background before we scale
		if(CurrentState == GameState.ChooseDifficulty){	GUI.DrawTexture(rcBackground, Textures[0]);	}
		else if(CurrentState == GameState.PlayGame){	GUI.DrawTexture(rcBackground, Textures[1]);	}

		// Set Aspect Ratio
		cScreenFit.setScaledScreen();
		
		if(CurrentState == GameState.ChooseDifficulty)
		{
			// Background picture
			GUILayout.BeginArea(rcDrawArea);	// Start Drawing Area
			
			GetGUIChooseDifficulty();
			
			GUILayout.EndArea();	// End DrawingArea
		}
		else if(CurrentState == GameState.PlayGame)
		{
			// Background picture
			GUILayout.BeginArea(rcDrawArea);	// Start Drawing Area

			if(!bShowEndGameButton)
			{
				// Show Game Puzzle
				cGameStart.GetGUIPuzzlePlacement();
			//	GetGUIPuzzleCorrect();
				GetGUIVictory();

				GetGUIQuit();
			}
			else
			{
				GetGUIEndButton();
			}
			GUILayout.EndArea();	// End DrawingArea
		}
		
		// Debug information
	}

	/*
	 * Show difficulty select menu
	 */
	void GetGUIChooseDifficulty()
	{
		GUIStyle DifficultyEasy;
		GUIStyle DifficultyMedium;
		GUIStyle DifficultyHard;
		GUIStyle SelectReturn;
		DifficultyEasy = 	GUI.skin.GetStyle("DifficultyEasy");
		DifficultyMedium =	GUI.skin.GetStyle("DifficultyMedium");
		DifficultyHard = 	GUI.skin.GetStyle("DifficultyHard");
		SelectReturn = 		GUI.skin.GetStyle("SelectReturn");

		GUILayout.Space(250);

		// 
		GUILayout.BeginVertical("Box");
		// ----------------------------------------
		GUILayout.BeginHorizontal("Box");
		GUILayout.FlexibleSpace();


		// Game Easy Button
		if( GUILayout.Button("", DifficultyEasy, GUILayout.Width(424), GUILayout.Height(100) ) )
		{
			CurrentState = GameState.SetupGame;
			CurrentDifficulty = DifficultyState.easy;
			cGameStart.initializePuzzle();
			playForwardPage();
			cSound.stopBGMSelectDif();
			cSound.playBGMGamePlay();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		// Game Medium Button
		GUILayout.Space(30);
		GUILayout.BeginHorizontal("Box");
		GUILayout.FlexibleSpace();
		if( GUILayout.Button("", DifficultyMedium, GUILayout.Width(424), GUILayout.Height(100) ) )
		{
			CurrentState = GameState.SetupGame;
			CurrentDifficulty = DifficultyState.medium;
			cGameStart.initializePuzzle();
			playForwardPage();
			cSound.stopBGMSelectDif();
			cSound.playBGMGamePlay();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		// Game Hard Button
		GUILayout.Space(30);
		GUILayout.BeginHorizontal("Box");
		GUILayout.FlexibleSpace();
		if( GUILayout.Button("", DifficultyHard, GUILayout.Width(424), GUILayout.Height(100) ) )
		{
			CurrentState = GameState.SetupGame;
			CurrentDifficulty = DifficultyState.hard;
			cGameStart.initializePuzzle();
			playForwardPage();
			cSound.stopBGMSelectDif();
			cSound.playBGMGamePlay();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.FlexibleSpace();
		// ----------------------------------------
		GUILayout.EndVertical();
		
		
		// Go Back to Title Button
		GUILayout.FlexibleSpace();
		GUILayout.BeginHorizontal("Box");
		GUILayout.FlexibleSpace();
		if( GUILayout.Button("", SelectReturn, GUILayout.Width(345), GUILayout.Height(75) ) )
		{
			playBackPage();
			Application.LoadLevel("ConcIndex");
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.Space(120);


	}

	/*
	 * Show End menu so that we can exit or return
	 */
	void GetGUIEndButton()
	{
		GUIStyle SelectReturn;
		GUIStyle OneMoreTime;
		SelectReturn = 	GUI.skin.GetStyle("SelectReturn");
		OneMoreTime = 	GUI.skin.GetStyle("OneMoreTime");
		
		GUILayout.FlexibleSpace();

		// One More Time button
		GUILayout.BeginHorizontal("Box");
		GUILayout.FlexibleSpace();
		if( GUILayout.Button("", OneMoreTime, GUILayout.Width(345), GUILayout.Height(75) ) )
		{
			CurrentState = GameState.ChooseDifficulty;
			bShowEndGameButton = false;
			playBGMSelect();
		}
		GUILayout.Space(30);
		GUILayout.EndHorizontal();
		
		GUILayout.Space(30);

		// Go Back to Title Button
		GUILayout.BeginHorizontal("Box");
		GUILayout.FlexibleSpace();
		if( GUILayout.Button("", SelectReturn, GUILayout.Width(345), GUILayout.Height(75) ) )
		{
			playBackPage();
			Application.LoadLevel("PuzzleIndex");
		}
		
		GUILayout.Space(30);
		GUILayout.EndHorizontal();
		GUILayout.Space(120);
	}

	/*
	 * Show message if we got a single puzzle piece correct @debug
	 */
	private void GetGUIPuzzleCorrect()
	{
		int LabWi = 200;
		int LabHe = 50;

		if(GamePuzzleCorrect == 1)
		{
		//	Debug.Log("パズルピース当たり");
			GUI.Label(new Rect(getCenterPos(newWidth,  LabWi),
			                   getCenterPos(newHeight - 200,  LabHe),
			                   LabWi,
			                   LabHe), "パズルピース当たり");
		}
	}

	/*
	 * Show message if we cleared a puzzle correct or all of them @debug
	 */
	private void GetGUIVictory()
	{
		int LabWi = 200;
		int LabHe = 50;

		if(GameVictory == 1)
		{
		//	Debug.Log("パズルクリアー");
			/*
			GUI.Label(new Rect(getCenterPos(newWidth, LabWi),
			                   getCenterPos(newHeight - 200, LabHe),
			                   LabWi,
			                   LabHe), "パズルクリアー");
			*/
		}else if(GameVictory == 2)
		{
		//	Debug.Log("すべてのパズルクリアー");
			// Won Image
			GUI.DrawTexture(new Rect(getCenterPos(newWidth, Textures[3].width),
			                         getCenterPos(newHeight - 350,  Textures[3].height),
			                         Textures[3].width,
			                         Textures[3].height), Textures[3]);
		}
	}

	/*
	 * Show We can quit menu
	 */
	private void GetGUIQuit()
	{
		
		// GameCorrectOrIncorrect
		if(bShowReallyQuit)
		{
			GUIStyle GameYes;
			GUIStyle GameNo;
			GameYes = 	GUI.skin.GetStyle("GameYes");
			GameNo =	GUI.skin.GetStyle("GameNo");
			
			// Show Hontou ni yameru
			GUI.DrawTexture(rcReallyQuit, Textures[2]);
			
			// Yes Button
			if( GUI.Button(rcYesButton, "", GameYes) )
			{
				// Go back to index
				Application.LoadLevel("PuzzleIndex");
			}
			
			// No button
			if( GUI.Button(rcNoButton, "", GameNo) )
			{
				// turn off showing this
				bShowReallyQuit = false;
				
				// Turn off when we showing really quit menu
				// set bool to not movable when here
			}
		}
	}

	/*
	 * Center layout
	 */
	private float getCenterPos (float screenSize,int TargetSize)
	{
		return (screenSize / 2.0f) - (TargetSize / 2.0f);
	}


	/*
	 * Play sound if we have sound
	 */
	internal void playForwardPage(){
		cSound.playForwardPage();
	}
	
	internal void playBackPage(){
		cSound.playBackPage();
	}
	
	internal void playPuzzleFit(){
		cSound.playPuzzleFit();
	}
	
	internal void playWinSound(){
		cSound.playWinSound();
	}

	internal void stopBGMGamePlay(){
		cSound.stopBGMGamePlay();
	}

	internal void playBGMSelect(){
		cSound.playBGMSelect();
	}
}
