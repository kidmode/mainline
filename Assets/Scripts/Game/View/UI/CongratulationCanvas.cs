﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CongratulationCanvas : UICanvas
{

	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );

        tweener.addAlphaTrack(0.0f, 1.0f, ZoodlesScreenFactory.FADE_SPEED);

        m_starParticles = GameObject.Find("starParticles").particleSystem;
        DebugUtils.Assert(m_starParticles != null);
		setupLocalization ();
	}
	
	public void setupLocalization()
	{
		UILabel l_title = getView ("congraturationLabel") as UILabel;
		l_title.text = Localization.getString (Localization.TXT_105_LABEL_TITLE);
	}

	public override void update()
	{
		base.update();

        if (m_inTransition)
            _updateParticleAlpha();
	}



	public override void dispose( bool p_deep )
	{
		base.dispose( p_deep );
	}

	public override void enteringTransition()
	{
		base.enteringTransition();

        float l_screenHeight = Screen.height * ( 1.0f / scaleFactor );

        List<Vector3> l_positionList = new List<Vector3>();
        l_positionList.Add( transform.localPosition );
        l_positionList.Add( transform.localPosition + new Vector3(0, l_screenHeight, 0) );

		tweener.addPositionTrack( l_positionList, 3.0f, onFadeFinish );
        tweener.addAlphaTrack( 1.0f, 0.0f, 0.5f );

		m_inTransition = true;

        if( m_starParticles != null )
            m_starParticles.enableEmission = false;
	}



	private void _updateParticleAlpha()
	{
		if(null != m_starParticles)
		{
			Material l_material = m_starParticles.renderer.material;
			Color l_col = l_material.GetColor( "_TintColor" );
			l_col.a = alpha;
			l_material.SetColor( "_TintColor", l_col );
		}
	}

	//-- Private Implementation --
	private void onFadeFinish( UIElement p_element, Tweener.TargetVar p_targetVariable )
	{
		UICanvas l_canvas = p_element as UICanvas;
		l_canvas.isTransitioning = false;
	}




    private bool m_inTransition = false;
    private ParticleSystem m_starParticles;

}
