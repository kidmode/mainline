using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ErrorCanvas : UICanvas
{
	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );
	}
	
	public override void update()
	{
		base.update();
	}
	
	
	public override void enteringTransition(  )
	{
		base.enteringTransition( );
	}
	
	public override void exitingTransition( )
	{
		base.exitingTransition( );
		
	}
	
	public override void dispose( bool p_deep )
	{
		base.dispose( p_deep );
	}
}
