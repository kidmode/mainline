using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UpsellCongraturationsState : GameState 
{
	//Public variables

	//Standard state flow	
	public override void enter( GameController p_gameController )
	{
		base.enter( p_gameController );
		
		m_game = p_gameController.game;

		_setupScreen( p_gameController.getUI() );
		p_gameController.game.StartCoroutine( _tweenFillBar( 1.0f, 1.25f ) );

	}
	
	public override void update( GameController p_gameController, int p_time )
	{
		base.update( p_gameController, p_time );
	}
	
	public override void exit( GameController p_gameController )
	{
		base.exit( p_gameController );
		p_gameController.getUI().removeScreen( m_congraturationCanvas );
	}

	//---------------- Private Implementation ----------------------
	
	private void _setupScreen( UIManager p_uiManager )
	{	
		m_congraturationCanvas = p_uiManager.createScreen( UIScreen.UPSELL_CONGRATURATION, true, 1 );
		m_loadingBarImg     = m_congraturationCanvas.getView( "loadingBarSprite" ) as UIImage;

		m_continuedButton = m_congraturationCanvas.getView ("continueButton") as UIButton;
		m_continuedButton.addClickCallback (onContinue);
	}

	private void goBack( UIButton p_button )
	{
		m_game.gameController.changeState (ZoodleState.PAYMENT);
	}

	private void onContinue( UIButton p_button )
	{
		if(null != SessionHandler.getInstance().kidList && SessionHandler.getInstance().kidList.Count > 0)
			m_game.gameController.changeState (ZoodleState.PROFILE_SELECTION);
		else
			m_game.gameController.changeState (ZoodleState.CREATE_CHILD_NEW);
	}

	private IEnumerator _tweenFillBar( float p_filledAmount, float p_duration )
	{
		float l_time = 0;
		while( l_time < p_duration )
		{
			float l_fillAmount = Mathf.Lerp( 0, p_filledAmount, l_time / p_duration );
			
			m_loadingBarImg.fillAmount = l_fillAmount;
			l_time += Time.deltaTime;
			
			yield return new WaitForEndOfFrame();
		}
		
		m_loadingBarImg.fillAmount = 1.0f;
		
		yield return null;
	}


	//Private variables
	
	private UICanvas    m_congraturationCanvas;

	private UIButton 	m_backButton;
	private UIButton 	m_continuedButton;
	private UIImage 	m_loadingBarImg;

	private Game 		m_game;
}
