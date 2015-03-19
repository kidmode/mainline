using UnityEngine;
using System.Collections;


public enum ViolenceRating 
{
	NoViolence  = 0,				
	ViolentInnuendos = 1, 			
	ExplosionsButNoVisibleWeapons = 2,
	VisibleWeapons = 3, 					
	SimulatedPhysicalViolence = 4, 
	RealLifeSimulatedViolence,
	Unknown,
	
	
	Default = NoViolence		     
}