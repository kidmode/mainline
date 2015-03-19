
class ArrayUtilityGC
{
	static function indexOf(p_array:Array, p_object:Object):int
	{
		var l_index:int = p_array.length - 1;
		
		while (l_index > -1)
		{
			if (p_array[l_index] == p_object)
			{
				return l_index;
			}
			l_index--;
		}
		
		return l_index;
	}
}