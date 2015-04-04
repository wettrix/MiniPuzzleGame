using UnityEngine;
using System.Collections;

public class PuzzleTimer : MonoBehaviour 
{
	// Classes
	private PuzzleDifficulty cDifficulty;
	private PuzzleGameStart cGameStart;

	// Rectangles
	private Rect rcPuzzlePair;
	private Rect rcPuzzleClear;
	private Rect rcCountDown;
	
	// Animation
	private int animationCounter = 0;
	private int animationCounterCorrect = 0;
	private float alpha = 0;
	private int countNum = 0;


	// Use this for initialization
	void Start () 
	{
		Application.targetFrameRate = 60;
		// Gets GamessStart Class
		cDifficulty = GetComponent<PuzzleDifficulty>();

		cGameStart = GetComponent<PuzzleGameStart>();

		// Rectangle setups
		rcCountDown = new Rect( getCenterPos(cDifficulty.newWidth,256),  getCenterPos(cDifficulty.newHeight,256),  256, 256);	// height was 320.0f
		rcPuzzlePair = new Rect( getCenterPos(cDifficulty.newWidth,640),  -240,  640, 240);	// height was 200.0f
		rcPuzzleClear = new Rect( getCenterPos(cDifficulty.newWidth,640),  250,  640, 240);	

		// Animation values
		animationCounter = 0;	//counter that is used for update to animate
		animationCounterCorrect = 0;
		alpha = 0;				// alpha we use on pictures
		countNum = 3;			// Countdown order number
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(cDifficulty.bShowPuzzleFlip)
			PlayPuzzleFlip();
		if(cDifficulty.bShowCorrectCircle)
			PlayCorrectCircle();

	}

	/*
	 * Play Animation Flipping the Puzzle deck when a Mount is completed
	 */
	private void PlayPuzzleFlip()
	{
		++animationCounter;
		if (animationCounter <= 1)
		{
			Time.timeScale = 1;
			cGameStart.changePuzzleBoard = 1;
			// Sound play
		}
		else if (animationCounter <= 60)		
		{
			// nothing
		}
		else if (animationCounter <= 100)	// from 60 - 100
		{
			cGameStart.rcPicMount.y += 5.0f;
			cGameStart.rcPicMount.height -= 10.0f;

			cGameStart.rcPicMount.x  -= 2.5f;
			cGameStart.rcPicMount.width += 5.0f;
		}
		else if (animationCounter <= 120)	// from 100 to 140
		{
			cGameStart.changePuzzleBoard = 2;
			cGameStart.rcPicMount.y -= 10.0f;
			cGameStart.rcPicMount.height += 20.0f;
			
			cGameStart.rcPicMount.x  += 5f;
			cGameStart.rcPicMount.width -= 10.0f;
		}
		else if(animationCounter <= 130)
		{
			cGameStart.rcPicMount.y += 3.0f;
			cGameStart.rcPicMount.height -= 6.0f;
			
			cGameStart.rcPicMount.x  -= 1.5f;
			cGameStart.rcPicMount.width += 3.0f;
		}
		else if(animationCounter <= 140)
		{
			cGameStart.rcPicMount.y -= 3.0f;
			cGameStart.rcPicMount.height += 6.0f;
			
			cGameStart.rcPicMount.x  += 1.5f;
			cGameStart.rcPicMount.width -= 3.0f;
		}
		else if (animationCounter <= 230)	// 290
		{
			// stop and set it back to basic one
		}
		else if (animationCounter <= 239)	// 299
		{
		}
		else if (animationCounter <= 240)	// 300
		{
			if(cGameStart.PuzzleWinsCount == 3)
				cGameStart.changePuzzleBoard = 2;
			else
				cGameStart.changePuzzleBoard = 0;
			cGameStart.rcPicMount = cGameStart.rcPicMountBase;
			animationCounter = 0;
			cDifficulty.bShowPuzzleFlip = false;
		}
	}

	/*
	 * Play Animation Show a Red Circle when it's correct
	 */
	private void PlayCorrectCircle()
	{
		++animationCounterCorrect;
		if (animationCounterCorrect <= 1)
		{
			Time.timeScale = 1;

			// Sound play
		}
		else if (animationCounterCorrect <= 23) // 1 to 30
		{
			cGameStart.rcCorrectCircle.x -= cGameStart.CorrectCircleSpeed / 2;
			cGameStart.rcCorrectCircle.y -= cGameStart.CorrectCircleSpeed / 2;
			cGameStart.rcCorrectCircle.width  += cGameStart.CorrectCircleSpeed;
			cGameStart.rcCorrectCircle.height += cGameStart.CorrectCircleSpeed;
			// nothing
		}
		else if (animationCounterCorrect <= 29)		
		{
			cGameStart.rcCorrectCircle.x += cGameStart.CorrectCircleSpeed / 4;
			cGameStart.rcCorrectCircle.y += cGameStart.CorrectCircleSpeed / 4;
			cGameStart.rcCorrectCircle.width  -= cGameStart.CorrectCircleSpeed / 2;
			cGameStart.rcCorrectCircle.height -= cGameStart.CorrectCircleSpeed / 2;
			// nothing
		}
		else if (animationCounterCorrect <= 119)		
		{
			// nothing
		}
		else if (animationCounterCorrect <= 120)		
		{
			// End
			// reset information
			animationCounterCorrect = 0;
			cGameStart.rcCorrectCircle.x = 0;
			cGameStart.rcCorrectCircle.y = 0;
			cGameStart.rcCorrectCircle.width  = 0;
			cGameStart.rcCorrectCircle.height = 0;
			cDifficulty.bShowCorrectCircle = false;
//			Debug.Log("Over Timer");
		}
	}
	
	
	private float getCenterPos (float screenSize,int TargetSize)
	{
		return (screenSize / 2.0f) - (TargetSize / 2.0f);
	}
}
