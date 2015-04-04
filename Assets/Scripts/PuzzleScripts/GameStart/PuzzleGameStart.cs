using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PuzzleGameStart : MonoBehaviour 
{
	// Classes
	private PuzzleDifficulty cDifficulty;
	private ScreenFit cScreenFit;
	
	// Screen Settings
	private int newWidth;		// New Aspect screen width
	private int newHeight;		// New Aspect screen height
	private float dpiHeight;
	private float dpiWidth;
	
	// Puzzle for Loop settings
	private int PuzzleShuffle;		// Switch Puzzle in for loop to show another Puzzle
	internal int PuzzleWinsCount;	// When 3 Wins the puzzle is cleared, increase every time we clear a puzzle
	internal int changePuzzleBoard = 0;	// Changes the Puzzle board, 0=Mount, 1=Mount2, 2=CompletePic
	
	// --------------------------------------------------------------
	// Where we load cards from
	private string ResourcesLocation = "Puzzle/Puzzle0";	// Resource location	 + int + "/" + puzzle_1_mount

	// Textures we use in a List
	private List<Texture2D[]> PzlFull = new List<Texture2D[]>();		// Puzzle Mount and Full picture 2 images
	private List<Texture2D[]> PzlPiece = new List<Texture2D[]>();	// each puzzle piece image
	private Texture2D HitAreaPic;		// Picture we need to set where we have the hit area
	private Texture2D SpawnArea;		// Where puzzle pieces spawn
	private Texture2D CorrectCircle;	// Correct Circle Image
	internal Rect rcCorrectCircle;		// Correct Circle Place
	internal float CorrectCircleSpeed;	// Correct Circle Image Speed for showing up
	private Rect rcSpawnArea;			// Where we Will Spawn a New Puzzle piece
	
	// Puzzle Piece Information
	private List<bool[]> bReachedDestination = new List<bool[]>();	// when reached turn off puzzle movement
	private List<bool[]> bShowMePuzzlePiece = new List<bool[]>();	// Show the puzzle image one at a time
	private List<bool[]> bIfDragginPiece = new List<bool[]>();		// If We are draggin the current Puzzle piece
	private List<Vector2[]> v2Loc = new List<Vector2[]>();			// Where we have each HitArea x & y
	
	// Rectangles
	internal Rect rcPicMount;	// Where we have the Mount image
	internal Rect rcPicMountBase;	// The animation flip
	private List<Rect[]> rcPieces = new List<Rect[]>();		// Each Puzzle Piece
	private List<Rect[]> rcHitArea = new List<Rect[]>();	// Hit Area we place each puzzle piece

	// The extra hit area for easier fit the puzzle piece
	private float ExtraArea;
	
	// Draggin value for Touch.Position
	private Vector2 TouchPlace;

	// --------------------------------------------------------------

	// Use this for initialization
	void Start () 
	{
		Application.targetFrameRate = 60;
		// Gets GameStart Class
		cDifficulty = GetComponent<PuzzleDifficulty>();
		// Gets ScreenFit Class
		cScreenFit = GetComponent<ScreenFit>();
		// Initialize Screen Settings
		cScreenFit.init();

		// Add Images
		Object textures01 = Resources.Load("Puzzle/HitBox", typeof(Texture2D));
		Object textures02 = Resources.Load("Puzzle/spawnArea", typeof(Texture2D));
		Object textures03 = Resources.Load("Puzzle/Game/circle", typeof(Texture2D));
		HitAreaPic = (Texture2D)textures01;
		SpawnArea = (Texture2D)textures02;
		CorrectCircle = (Texture2D)textures03;
		
		// Setting Screen sizes
		newWidth = cScreenFit.width;
		newHeight = cScreenFit.height;
		
		dpiHeight = cScreenFit.screenDpiHeight;
		dpiWidth = cScreenFit.screenDpiWidth;

		ExtraArea = 20;

		// Puzzle piece Spawn Location
		rcSpawnArea = new Rect( (newWidth / 2) - (SpawnArea.width) / 2,
								135,
								SpawnArea.width,
								SpawnArea.height);

		// Setup the Hit Area Locations for each puzzle piece maunally
		SetV2List();
	}

	/*
	 * Setup puzzle before we start playing
	 */
	public void initializePuzzle()
	{
		// Setup what level of difficulty puzzle we are using
		if(cDifficulty.CurrentDifficulty == DifficultyState.easy)
		{
			for(int i = 1; i <= 3 ; i++)
			{
				SetPuzzles(i);
			}
		}else if(cDifficulty.CurrentDifficulty == DifficultyState.medium)
		{
			for(int i = 4; i <= 6 ; i++)
			{
				SetPuzzles(i);
			}
		}else if(cDifficulty.CurrentDifficulty == DifficultyState.hard)
		{
			for(int i = 7; i <= 9 ; i++)
			{
				SetPuzzles(i);
			}
		}

		// Sets Puzzle for loop to 0
		PuzzleShuffle = 0;
		PuzzleWinsCount = 0;
		changePuzzleBoard = 0;

		// Sets it all to start playing
		cDifficulty.CurrentState = GameState.PlayGame;
	}

	/*
	 * Sets up the main 3 puzzles for images, placement values
	 * @param	PuzzleNumber	What puzzle number we use, get pictures and more
	 */
	private void SetPuzzles(int PuzzleNumber)
	{
		string PicLocation;
		Texture2D[] PuzzleFull;
		Texture2D[] PuzzlePieces;
		// Get Resources Information
		PicLocation = ResourcesLocation + PuzzleNumber.ToString();
		
		// Step 01 - Add Pictures
		// ---------------------------------------------------------------------------------

		// Add Images from Resources only to an empty object array
		Object[] GetTex = Resources.LoadAll(PicLocation, typeof(Texture2D));
		
		// Add Mount1, Mount2 & Picture to PuzzleFull
		PuzzleFull = new Texture2D[3];
		for(int i = 0 ; i < PuzzleFull.Length ; i++)
			PuzzleFull[i] = (Texture2D)GetTex[i];
		PzlFull.Add(PuzzleFull);

		// Add Pieces to the rest
		PuzzlePieces = new Texture2D[GetTex.Length - PuzzleFull.Length];
		// random values
		List<Texture2D> RandomPics = new List<Texture2D>();		// randomize theese pics
		int[] StoredRandomInt = new int[GetTex.Length - PuzzleFull.Length];		// stored random int for Puzzle nr

		// Add Pictures
		for(int i = 0 ; i < PuzzlePieces.Length; i++)
		{
			RandomPics.Add( (Texture2D)GetTex[i + PuzzleFull.Length] );
		}

		// Randomize the pictures
		for(int i = 0; i < PuzzlePieces.Length ; i++)
		{
			// Random Image and add to Array
			int thisPicture = Random.Range(0, RandomPics.Count);
			PuzzlePieces[i] = RandomPics[thisPicture];

			// Get the pictures number, by removing string name exept last and make that string(number) into an int
			int thisNumber =  int.Parse( RandomPics[thisPicture].name.Remove(0,RandomPics[thisPicture].name.Length - 1) );
			StoredRandomInt[i] = thisNumber - 1;	// -1 to get correct array location

			RandomPics.RemoveAt(thisPicture);
		}

		PzlPiece.Add(PuzzlePieces);
		
		// Step 02 - Add Rectangles
		// ---------------------------------------------------------------------------------
		Vector2[] v2HitArea;	// Where The HitArea is
		Rect[] rcPie;
		Rect[] rcHit;

		// Setup Base Mount Rectangle for all of them
		rcPicMount = new Rect((newWidth / 2) - (PzlFull[0][0].width / 2),
		                      newHeight - (110 + (PzlFull[0][0].height)), 		// set mount height here, where we want the mount to be placed on the screen
		                      PzlFull[0][0].width,
		                      PzlFull[0][0].height);
		// Set a default one
		rcPicMountBase = rcPicMount;

		// Setup HitArea, in Photoshop align picture then command+T to see where it is
		v2HitArea = new Vector2[PuzzlePieces.Length];

		for(int i = 0; i < v2HitArea.Length ; i++)
		{
			// StoredRandomInt[i] is to get the random position of puzzle number
			v2HitArea[i] = new Vector2(rcPicMount.x + ( v2Loc[PuzzleNumber - 1][StoredRandomInt[i]].x - ExtraArea / 2 ),
			                           rcPicMount.y + ( v2Loc[PuzzleNumber - 1][StoredRandomInt[i]].y - ExtraArea / 2 ) );
		}

		// Add final rectangle information
		rcPie = new Rect[PuzzlePieces.Length];	// Puzzle rect
		rcHit = new Rect[PuzzlePieces.Length];	// HitArea rect

		for(int i = 0; i < rcPie.Length ; i++)
		{
			//	rcPie[i]  = new Rect(v2StartArea.x, v2StartArea.y,	PuzzlePieces[i].width, 				PuzzlePieces[i].height);
			rcPie[i]  = new Rect(rcSpawnArea.center.x - PuzzlePieces[i].width / 2, rcSpawnArea.center.y - PuzzlePieces[i].height / 2,	PuzzlePieces[i].width, 				PuzzlePieces[i].height);
			rcHit[i] = new Rect(v2HitArea[i].x, v2HitArea[i].y,	PuzzlePieces[i].width + ExtraArea,	PuzzlePieces[i].height + ExtraArea);
		}

		rcPieces.Add(rcPie);
		rcHitArea.Add(rcHit);

		// Step 03 - Add Bools
		// ---------------------------------------------------------------------------------
		bool[] bDestination = new bool[PuzzlePieces.Length];	// The destination
		bool[] bPiecesSet = new bool[PuzzlePieces.Length];		// Each puzzle Piece
		bool[] bPDragginSet = new bool[PuzzlePieces.Length];	// Currently Draggin it

		for(int i = 0; i < bDestination.Length ; i++)
		{
			bDestination[i] = false;
			bPDragginSet[i] = false;
			// Sets first to always be true for showing first Piece.
			if(i == 0)
			{
				bPiecesSet[i] = true;
			}else
			{
				bPiecesSet[i] = false;
			}

		}
		// bool for when a puzzle piece has been reached
		bReachedDestination.Add(bDestination);
		// bool that turns of the puzzle piece 
		bShowMePuzzlePiece.Add(bPiecesSet);
		// bool that checks if we are draggin this piece
		bIfDragginPiece.Add(bPDragginSet);
	}
	
	/*
	 * Will Display the menu
	 */
	public void GetGUIPuzzlePlacement()
	{
		GUIStyle GameQuit;
		GameQuit = GUI.skin.GetStyle("GameQuit");

		// Puzzle Setup for loop
		for(int i = PuzzleShuffle ; i < PuzzleShuffle + 1 ; i++)
		{
			// Bool setup for draggin the Puzzle Piece
			for(int boolz = 0 ; boolz < rcPieces[i].Length ; boolz++)
			{
				if(!bReachedDestination[i][boolz])
				{
					// When draggin show this
					rcHitArea[i][boolz] = PlacementField(rcHitArea[i][boolz], rcPieces[i][boolz], i, boolz);
				}else
				{
					// When not draggin center the picture to the right spot
					rcPieces[i][boolz] = PlacementField(rcHitArea[i][boolz], rcPieces[i][boolz], i, boolz);
				}
			}

			// Mount Image that shows where the Puzzle Piece is going
			GUI.DrawTexture( rcPicMount, PzlFull[i][changePuzzleBoard]);

			// Hit targets, Set a texture that is invisible for future build 
			/*
			for(int hit = 0; hit < rcHitArea[i].Length ; hit++)
			{
				GUI.DrawTexture( rcHitArea[i][hit], HitAreaPic);
			}
			*/

			// Puzzle Piece to drag
			for(int drag = 0 ; drag < rcPieces[i].Length ; drag++)
			{
				// Shows 1 Puzzle piece at a time, the one we can drag
				if(bShowMePuzzlePiece[i][drag])
				{
					if(changePuzzleBoard == 0)
						GUI.DrawTexture( rcPieces[i][drag], PzlPiece[i][drag]);
					else if(changePuzzleBoard == 1){}
					//	Debug.Log("Show Complete Mount");
					else if(changePuzzleBoard == 2){}
					//	Debug.Log("Show Complete Picture");

					// If it's the correct value we set a red circle around it
					if(cDifficulty.GamePuzzleCorrect == 1)
					{
						GUI.DrawTexture( rcCorrectCircle, CorrectCircle);
					}
				}
			}

		}

		// ------------------------------------------------------------------------------
		// Bottom Logo part
		GUILayout.FlexibleSpace();
		// Go Back to Title Button
		GUILayout.BeginHorizontal("Box");
		GUILayout.FlexibleSpace();
		if(GUILayout.Button("", GameQuit, GUILayout.Width(180), GUILayout.Height(100) ) )
		{
			// When Pressed show
			cDifficulty.bShowReallyQuit = true;
		}

		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.Space(10);
	}

	/*
	 * Function that calculates if Puzzle Piece is inside the Hit area
	 * @param	ThisRect	This Rect settings
	 * @param	GoInside	What Rectangle we want it to react to
	 * @param	forIndex	What for loop index are we in
	 * @param	PzlIndex	What Index in the bool are we in
	 * @return				Where we are currently Placed
	 */
	private Rect PlacementField(Rect ThisRect, Rect GoInside, int forIndex, int PzlIndex)
	{
		if( ThisRect.x < GoInside.x && 
		   (ThisRect.x + ThisRect.width) > (GoInside.x + GoInside.width) &&
		   ThisRect.y < GoInside.y && 
		   (ThisRect.y + ThisRect.height) > (GoInside.y + GoInside.height) )
		{
			// If we are inside and not draggin
			if(!bIfDragginPiece[forIndex][PzlIndex])
			{
				// If we have reached the destination and are not draggin
				if(!bReachedDestination[forIndex][PzlIndex])
				{
					// Play clip sound
					cDifficulty.playPuzzleFit();

					// Set so that it is locked in there
					bReachedDestination[forIndex][PzlIndex] = true;
					
					// center rectangle
					Vector2 centerRectangle = ThisRect.center;
					centerRectangle.x -= GoInside.width / 2;
					centerRectangle.y -= GoInside.height / 2;
					
					GoInside.x = centerRectangle.x;
					GoInside.y = centerRectangle.y;
					
					// Check if the puzzle is done, after each piece
					StartCoroutine(GUIConfirmPuzzleState(bReachedDestination[forIndex], forIndex, PzlIndex));
					
					// If the marked Piece is inside the HitBox, return puzzle piece rectangle that shows it's set.
					return GoInside;
				}else
				{
					// return value that puts image at right spot
					return GoInside;
				}
			}
			/*

			*/
		}
		return ThisRect;
	}


	/*
	 * Sets Puzzle State
	 * @param	BoolArray	What bool List we need to change in
	 * @param	forIndex	What for loop index are we in
	 * @param	PzlIndex	What Index in the bool are we in
	 */
	private IEnumerator GUIConfirmPuzzleState(bool[] BoolArray, int forIndex, int PzlIndex)
	{
		// Get center position for animation
		rcCorrectCircle.x = rcPieces[forIndex][PzlIndex].center.x;
		rcCorrectCircle.y = rcPieces[forIndex][PzlIndex].center.y;

		// Set Animation size speed to know how much we do the animation
		if(rcPieces[forIndex][PzlIndex].width > rcPieces[forIndex][PzlIndex].height)
			CorrectCircleSpeed = (rcPieces[forIndex][PzlIndex].width + 80)     / 20;
		else
			CorrectCircleSpeed = (rcPieces[forIndex][PzlIndex].height + 80)     / 20;
		// Set Animation Circle On
		cDifficulty.bShowCorrectCircle = true;

		// Show Puzzle Piece message @REMOVE
		cDifficulty.GamePuzzleCorrect = 1;
		yield return new WaitForSeconds(2.0f);	// 120 counts
		// then turn it off message @REMOVE
		cDifficulty.GamePuzzleCorrect = 0;

		// Show Next Puzzle Piece
		if( bShowMePuzzlePiece[forIndex].Length == PzlIndex + 1 )
		{
			// if puzzle piece length is same as this then show nothing
		}else
		{
			bShowMePuzzlePiece[forIndex][PzlIndex + 1] = true;
		}

		// If All Puzzle Pieces are set change to next Mount
		if( CompareAllPuzzlePiece(BoolArray,true) )
		{
			cDifficulty.GameVictory = 1;
			cDifficulty.bShowPuzzleFlip = true;
			PuzzleWinsCount++;
			yield return new WaitForSeconds(3.99f);
			cDifficulty.GameVictory = 0;

			// If we have all Puzzle Piece Mounts complete, then we end the puzzle
			if(PuzzleWinsCount == 3)
			{
				cDifficulty.playWinSound();
				cDifficulty.stopBGMGamePlay();
				cDifficulty.GameVictory = 2;
				yield return new WaitForSeconds(3.0f);
				cDifficulty.GameVictory = 0;

				// Reset Game information
				StartCoroutine(ResetGameInformation());

			}else
			{
				PuzzleShuffle++;
			}

		}
	}

	/*
	 * Compares all the bools inside this array and if they are true
	 * @param	BoolArray	What array to check
	 * @param	BoolValue	What value we want to compare it as
	 * @return				return value
	 */
	public bool CompareAllPuzzlePiece(bool[] BoolArray, bool BoolValue)
	{
		bool Answer = true;
		for(int i = 0 ; i < BoolArray.Length ; i++)
		{
			if(BoolArray[i] != BoolValue)
			{
				Answer = false;
				break;
			}
		}
		return Answer;
	}


	private bool bMouseController;

	/*
	 * Updates the Touch or Mouse draggin of a puzzle piece
	 */
	void Update () 
	{
		// If GameState
		if(cDifficulty.CurrentState == GameState.PlayGame && !cDifficulty.bShowEndGameButton)
		{
			// For Touch based moving
			int i = 0;
			while (i < Input.touchCount) 
			{
				TouchPlace = Input.GetTouch(0).position;
				
				// Scale position
				TouchPlace.x /= dpiWidth;
				TouchPlace.y = newHeight - (TouchPlace.y / dpiHeight);
				
				// For loop Touch of all pieces
				for(int ps = PuzzleShuffle ; ps < PuzzleShuffle + 1 ; ps++)
				{

					for(int ii = 0; ii < rcPieces[ps].Length ; ii++)
					{
						if(rcPieces[ps][ii].Contains(TouchPlace))
						{
							// When Start Touching, set dragging to true
							if (Input.GetTouch(0).phase == TouchPhase.Began) 
							{
								bIfDragginPiece[ps][ii] = true;
							}

							// Move only if we haven't reached destination
							if (!bReachedDestination[ps][ii] && Input.GetTouch(0).phase == TouchPhase.Moved) 
							{
								// Move Puzzle piece
								if(bShowMePuzzlePiece[ps][ii] && bIfDragginPiece[ps][ii])
								{
									// Center the Puzzle piece
									rcPieces[ps][ii].x = TouchPlace.x - (rcPieces[ps][ii].width /2);
									rcPieces[ps][ii].y = TouchPlace.y - (rcPieces[ps][ii].height /2);
								}
							}else
							{
								//	Debug.Log("can't touch");
							}

							// When Release, set draggin to false
							if (Input.GetTouch(0).phase == TouchPhase.Ended) 
							{
								bIfDragginPiece[ps][ii] = false;
							}
						}
					}
				}
				++i;
			}

			// For computer mouse setting
			if(Input.GetMouseButton(0))
				bMouseController = true;

			if(bMouseController)
			{
				TouchPlace = Input.mousePosition;
				// Scale position
				TouchPlace.x /= dpiWidth;
				TouchPlace.y = newHeight - (TouchPlace.y / dpiHeight);

				// For loop Touch of all pieces
				for(int ps = PuzzleShuffle ; ps < PuzzleShuffle + 1 ; ps++)
				{
					for(int ii = 0; ii < rcPieces[ps].Length ; ii++)
					{
						if(rcPieces[ps][ii].Contains(TouchPlace))
						{
							// When Start Touching, set dragging to true
							if (Input.GetMouseButtonDown(0)) 
							{
								bIfDragginPiece[ps][ii] = true;
							}

							// Move only if we haven't reached destination
							if (!bReachedDestination[ps][ii] && Input.GetMouseButton(0)) 
							{
								// Move Puzzle piece
								if(bShowMePuzzlePiece[ps][ii] && bIfDragginPiece[ps][ii])
								{
									// Center the Puzzle piece
									rcPieces[ps][ii].x = TouchPlace.x - (rcPieces[ps][ii].width /2);
									rcPieces[ps][ii].y = TouchPlace.y - (rcPieces[ps][ii].height /2);
								}
							}else
							{
								//	Debug.Log("can't touch");
							}

							// When Release, set draggin to false
							if (Input.GetMouseButtonUp(0)) 
							{
								bIfDragginPiece[ps][ii] = false;
								bMouseController = false;
							}
						}
					}
				}
			}
		}
	}

	/*
	 * Removes basic information for puzzle to start a new game later and remove memory
	 */
	public IEnumerator ResetGameInformation()
	{
		// Turn off Victory message
		cDifficulty.GameVictory = 0;
		
		// Go back to choose difficulty
		cDifficulty.bShowEndGameButton = true;

		yield return new WaitForSeconds(0.1f);
		// Removes All Information so that we can make new next time
		PzlFull.RemoveRange(0,PzlFull.Count);
		PzlPiece.RemoveRange(0,PzlPiece.Count);
		bReachedDestination.RemoveRange(0,bReachedDestination.Count);
		bShowMePuzzlePiece.RemoveRange(0,bShowMePuzzlePiece.Count);
		bIfDragginPiece.RemoveRange(0,bIfDragginPiece.Count);
		rcPieces.RemoveRange(0,rcPieces.Count);
		rcHitArea.RemoveRange(0,rcHitArea.Count);

		Resources.UnloadUnusedAssets();
		
		yield return new WaitForSeconds(0.1f);
	}


	/*
	 * Sets base position for each puzzle piece
	 */
	private void SetV2List()
	{
		Vector2[] v2Puzzle01;
		v2Puzzle01 = new Vector2[2]
		{
			new Vector2(50,		53),
			new Vector2(310,	96),
		};
		v2Loc.Add(v2Puzzle01);
		
		// -----------------------------------------
		
		Vector2[] v2Puzzle02;
		v2Puzzle02 = new Vector2[2]
		{
			new Vector2(31,		141),
			new Vector2(320,	17),
		};
		v2Loc.Add(v2Puzzle02);
		
		// -----------------------------------------
		
		Vector2[] v2Puzzle03;
		v2Puzzle03 = new Vector2[3]
		{
			new Vector2(22,		20),
			new Vector2(388,	33),
			new Vector2(381,	194),
		};
		v2Loc.Add(v2Puzzle03);
		
		// -----------------------------------------
		
		Vector2[] v2Puzzle04;
		v2Puzzle04 = new Vector2[3]
		{
			new Vector2(138,	144),
			new Vector2(35,		23),
			new Vector2(300,	9),
		};
		v2Loc.Add(v2Puzzle04);
		
		// -----------------------------------------
		
		Vector2[] v2Puzzle05;
		v2Puzzle05 = new Vector2[3]
		{
			new Vector2(32,		174),
			new Vector2(210,	15),
			new Vector2(392,	173),
		};
		v2Loc.Add(v2Puzzle05);
		
		// -----------------------------------------
		
		Vector2[] v2Puzzle06;
		v2Puzzle06 = new Vector2[3]
		{
			new Vector2(32,		37),
			new Vector2(242,	38),
			new Vector2(435,	248),
		};
		v2Loc.Add(v2Puzzle06);
		
		// -----------------------------------------
		
		Vector2[] v2Puzzle07;
		v2Puzzle07 = new Vector2[4]
		{
			new Vector2(200,	21),
			new Vector2(41,		133),
			new Vector2(199,	125),
			new Vector2(362,	120),
		};
		v2Loc.Add(v2Puzzle07);
		
		// -----------------------------------------
		
		Vector2[] v2Puzzle08;
		v2Puzzle08 = new Vector2[4]
		{
			new Vector2(138,	66),
			new Vector2(94,		219),
			new Vector2(364,	2),
			new Vector2(287,	218),
		};
		v2Loc.Add(v2Puzzle08);
		
		// -----------------------------------------
		
		Vector2[] v2Puzzle09;
		v2Puzzle09 = new Vector2[4]
		{
			new Vector2(26,		13),
			new Vector2(149,	201),
			new Vector2(280,	14),
			new Vector2(396,	185),
		};
		v2Loc.Add(v2Puzzle09);
		
		// -----------------------------------------
	}
}
