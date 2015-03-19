using System.Collections;
using System.Collections.Generic;

public interface IScreenFactory 
{
	Dictionary<int,string> getDirectoryMap();
	UICanvas getScreen( int p_screenId );
}

