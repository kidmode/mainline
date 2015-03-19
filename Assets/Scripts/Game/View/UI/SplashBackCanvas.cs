using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SplashBackCanvas : UICanvas
{
    const int FOREGROUND    = 0;
    const int MIDGROUND     = 1;
    const int BACKGROUND    = 2;

    const int MAX_ELEMENTS  = 3;

    
    enum TransitionState
    {
        UP,
        DOWN
    }

	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );

//        m_screenHeight = Screen.height * (1.0f / scaleFactor);
		m_screenHeight = 530;

        m_transitionState = TransitionState.UP;

        m_panelList[ FOREGROUND    ]   = getView("foreground");
        m_panelList[ MIDGROUND     ]   = getView("midground");
        m_panelList[ BACKGROUND    ]   = getView("background");

        m_startPositionList[ FOREGROUND ] = m_panelList[ FOREGROUND ].transform.localPosition;
        m_startPositionList[ MIDGROUND  ] = m_panelList[ MIDGROUND  ].transform.localPosition;
        m_startPositionList[ BACKGROUND ] = m_panelList[ BACKGROUND ].transform.localPosition;

        m_distanceList[ FOREGROUND  ]  = m_screenHeight;
//        m_distanceList[ MIDGROUND   ]  = m_screenHeight * 0.9f;
//        m_distanceList[ BACKGROUND  ]  = m_screenHeight * 0.8f;
		
		m_distanceList[ MIDGROUND   ]  = m_screenHeight;
		m_distanceList[ BACKGROUND  ]  = m_screenHeight;
	}

	public override void update()
	{
		base.update();
	}

	public override void dispose( bool p_deep )
	{
		base.dispose( p_deep );
	}


    public void transitionDown( float p_duration )
    {
        if (m_transitionState == TransitionState.DOWN)
            return;

        _tweenElementHeight( m_distanceList[ FOREGROUND ],    p_duration, m_panelList[ FOREGROUND  ]   );
        _tweenElementHeight( m_distanceList[ MIDGROUND  ],    p_duration, m_panelList[ MIDGROUND   ]   );
        _tweenElementHeight( m_distanceList[ BACKGROUND ],    p_duration, m_panelList[ BACKGROUND  ]   );

        m_transitionState = TransitionState.DOWN;
    }

    public void transitionUp(float p_duration)
    {
        if (m_transitionState == TransitionState.UP)
            return;

        _tweenElementHeight( -m_distanceList[ FOREGROUND ], p_duration, m_panelList[ FOREGROUND  ]    );
        _tweenElementHeight( -m_distanceList[ MIDGROUND  ], p_duration, m_panelList[ MIDGROUND   ]    );
        _tweenElementHeight( -m_distanceList[ BACKGROUND ], p_duration, m_panelList[ BACKGROUND  ]    );

        m_transitionState = TransitionState.UP;
    }

    public void setDown()
    {
        m_transitionState = TransitionState.DOWN;

        _setDownPosition( FOREGROUND  );
        _setDownPosition( MIDGROUND   );
        _setDownPosition( BACKGROUND  );
    }

    public void setUp()
    {
        m_transitionState = TransitionState.UP;

        _setUpPosition( FOREGROUND   );
        _setUpPosition( MIDGROUND    );
        _setUpPosition( BACKGROUND   );
    }


//------------------------ Private Implementation -------------------------

    private void _setDownPosition( int p_index )
    {
        if (p_index >= MAX_ELEMENTS || p_index < 0)
            return;

        m_panelList[p_index].transform.localPosition = m_startPositionList[p_index] 
                                                        + new Vector3(0, m_distanceList[p_index], 0);
    }

    private void _setUpPosition( int p_index )
    {
        if (p_index >= MAX_ELEMENTS || p_index < 0)
            return;

        m_panelList[p_index].transform.localPosition = m_startPositionList[p_index];
    }

    private void _tweenElementHeight( float p_distance, float p_duration, UIElement p_element )
    {
        if (p_element == null)
            return;

        Vector3 l_startPosition = p_element.transform.localPosition;
        Vector3 l_endPosition   = l_startPosition + new Vector3(0, p_distance, 0);

        List<Vector3> l_positionList = new List<Vector3>();
        l_positionList.Add(l_startPosition);
        l_positionList.Add(l_endPosition);

        p_element.tweener.addPositionTrack( l_positionList, p_duration );
    }

    private UIElement m_background;
    private UIElement m_midground;
    private UIElement m_foreground;

    private float m_screenHeight;
    private TransitionState m_transitionState;

    private Vector3[] m_startPositionList   = new Vector3[MAX_ELEMENTS];
    //private Vector3[] m_endPositionList     = new Vector3[MAX_ELEMENTS];
    private float[] m_distanceList          = new float[MAX_ELEMENTS];
    private UIElement[] m_panelList         = new UIElement[MAX_ELEMENTS];
}
