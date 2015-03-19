using UnityEngine;
using System.Collections;

public interface IGameStateFactory
{
	int initialState { get; }

	void addStates( GameController p_gameController );
}
