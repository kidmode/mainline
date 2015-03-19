/*
 * File:			GAFMovieClip.cs
 * Version:			3.7.1
 * Last changed:	2014/7/25 14:13
 * Author:			Nikolay_Nikitin
 * Copyright:		© Catalyst Apps
 * Project:			UnityVS.UnityProject.CSharp
 */

using UnityEngine;
using GAF;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

[System.Serializable]
[AddComponentMenu("GAF/GAFMovieClip")]
[ExecuteInEditMode]
[RequireComponent(typeof(GAFObjectsManager))]
#if UNITY_4_5
[DisallowMultipleComponent]
#endif
public class GAFMovieClip :
	  MonoBehaviour
	, GAFMovieClipInterface
{
	#region Movie clip interface

	public void play()
	{
		setPlaying (true);
	}
	
	public void pause()
	{
		setPlaying (false);
	}

	public void stop()
	{
		updateToFrame (currentSequence.startFrame, true);
		setPlaying (false);
	}

	public void gotoAndStop(uint _FrameNumber)
	{
		_FrameNumber = (uint)Mathf.Clamp (
			  (int)_FrameNumber
			, (int)currentSequence.startFrame
			, (int)currentSequence.endFrame);

		updateToFrame (_FrameNumber, true);

		if (on_goto != null)
			on_goto (this);

		setPlaying (false);
	}

	public void gotoAndPlay(uint _FrameNumber)
	{
		_FrameNumber = (uint)Mathf.Clamp(
			  (int)_FrameNumber
			, (int)currentSequence.startFrame
			, (int)currentSequence.endFrame);
		
		updateToFrame (_FrameNumber, true);

		if (on_goto != null)
			on_goto (this);
		
		setPlaying (true);
	}

	public string sequenceIndexToName(uint _Index)
	{
		if (asset != null && asset.isLoaded)
		{
			var sequences = asset.getSequences (timelineID);
			return sequences[Mathf.Clamp((int)_Index, 0, sequences.Count - 1)].name;
		}
		else
		{
			return string.Empty;
		}
	}

	public uint sequenceNameToIndex(string _Name)
	{
		if (asset != null && asset.isLoaded)
		{
			int index = asset.getSequences(timelineID).FindIndex(__sequence => __sequence.name == _Name);
			return index < 0 ? uint.MaxValue : (uint)index;
		}
		else
		{
			return uint.MaxValue;
		}
	}

	public void setSequence(string _SequenceName, bool _PlayImmediately = false)
	{
		uint sequenceIndex = sequenceNameToIndex(_SequenceName);
		if (sequenceIndex != uint.MaxValue &&
		    m_SequenceIndex != sequenceIndex)
		{
			m_SequenceIndex = (int)sequenceIndex;

			updateToFrame (currentSequence.startFrame, true);

			if (on_sequence_change != null)
				on_sequence_change (this);

			setPlaying (_PlayImmediately);
		}
	}

	public void setDefaultSequence(bool _PlayImmediately = false)
	{
		setSequence ("Default", _PlayImmediately);
	}

	public uint getCurrentSequenceIndex()
	{
		return (uint)m_SequenceIndex;
	}

	public uint getCurrentFrameNumber()
	{
		return (uint)m_CurrentFrameNumber;
	}
	
	public uint getFramesCount()
	{
		return asset.getFramesCount(timelineID);
	}

	public GAFWrapMode getAnimationWrapMode()
	{
		return settings.wrapMode;
	}

	public void setAnimationWrapMode(GAFWrapMode _Mode)
	{
		settings.wrapMode = _Mode;
	}
	
	public bool isPlaying()
	{
		return m_IsPlaying;
	}

	public float duration()
	{
		return (currentSequence.endFrame - currentSequence.startFrame) * settings.targetSPF;
	}

	public string addTrigger(uint _FrameNumber, System.Action<GAFMovieClip> _Callback)
	{
		if (_FrameNumber < getFramesCount())
		{
			GAFFrameEvent triggerEvent = new GAFFrameEvent (_Callback);
			if (m_FrameEvents.ContainsKey(_FrameNumber))
			{
				m_FrameEvents [_FrameNumber].Add (triggerEvent);
			}
			else
			{
				m_FrameEvents.Add(_FrameNumber, new List<GAFFrameEvent>());
				m_FrameEvents[_FrameNumber].Add(triggerEvent);
			}
			
			return triggerEvent.id;
		}
		
		return string.Empty;
	}
	
	public void removeTrigger(string _ID)
	{
		foreach(KeyValuePair<uint, List<GAFFrameEvent>> pair in m_FrameEvents)
		{
			pair.Value.RemoveAll(delegate(GAFFrameEvent _event) 
			{
				return _event.id == _ID;
			});
		}
	}
	
	public void removeAllTriggers(uint _FrameNumber)
	{
		if (_FrameNumber < getFramesCount())
		{
			if (m_FrameEvents.ContainsKey(_FrameNumber))
			{
				m_FrameEvents[_FrameNumber].Clear();
			}
		}
	}
	
	public void removeAllTriggers()
	{
		m_FrameEvents.Clear ();
	}

    public IGAFObject getObject(uint _ID)
	{
		return manager.objectsDict.ContainsKey((int)_ID) ? manager.objectsDict[(int)_ID] : null;
	}

	public IGAFObject getObject(string _PartName)
	{
		return getObject (partNameToObjectID (_PartName));
	}

	public string objectIDToPartName(uint _ID)
	{
		if (asset != null &&
		    asset.isLoaded)
		{
			var data = asset.getNamedParts(timelineID).Find(part => part.objectID == _ID);
			return data != null ? data.name : string.Empty;
		}
		
		return string.Empty;
	}

	public uint partNameToObjectID(string _PartName)
	{
		if (asset != null &&
		    asset.isLoaded)
		{
			var data = asset.getNamedParts(timelineID).Find(part => part.name == _PartName);
			return data != null ? data.objectID : uint.MaxValue;
		}

		return uint.MaxValue;
	}

	#endregion // Movie clip interface

	#region Events

	public event System.Action<GAFMovieClip> on_start_play;
	public event System.Action<GAFMovieClip> on_stop_play;
	public event System.Action<GAFMovieClip> on_goto;
	public event System.Action<GAFMovieClip> on_sequence_change;
	public event System.Action<GAFMovieClip> on_clear;
	
	#endregion // Events

	#region Behavior interface

	public void initialize(GAFAnimationAsset _Asset, int _TimelineID)
	{
		if (!isInitialized)
		{
			resource = _Asset.getResource(settings.scale, settings.csf);

			if (resource != null &&
				resource.isReady)
			{
				m_IsInitialized = true;

				m_MovieClipVersion	= GAFSystem.MovieClipVersion;
				m_GAFAsset			= _Asset;
				m_TimelineID		= _TimelineID;

				manager.initialize(this);
			}
		}
	}

	public void clear(bool destroyChildren = false)
	{
		if (on_clear != null)
			on_clear(this);

		manager.clear(destroyChildren);
		
		m_FrameEvents.Clear ();

		m_GAFAsset	 			= null;
		m_Resource				= null;
		m_Settings 				= new GAFAnimationPlayerSettings ();
		m_SequenceIndex 		= 0;
		m_CurrentFrameNumber 	= 1;
		m_Stopwatch 			= 0.0f;

		m_IsInitialized = false;
	}

	public void reload()
	{
		if (!System.Object.Equals(asset, null) &&
		    isInitialized)
		{
			if (!asset.isLoaded)
				asset.load();

			if (asset.isLoaded)
			{
				resource = asset.getResource(settings.scale, settings.csf);

				if (resource != null &&
				    resource.isReady)
				{
#if UNITY_EDITOR
					if (m_MovieClipVersion < GAFSystem.MovieClipVersion)
					{
						upgrade();
					}
#endif // UNITY_EDITOR

					manager.reload();

					updateToFrame (getCurrentFrameNumber(), true);
				}
			}
		}
	}

	#endregion // Behavior interface

	#region Properties

	public GAFAnimationAsset asset
	{
		get
		{
			return m_GAFAsset;
		}
	}

	public int timelineID
	{
		get
		{
			return m_TimelineID;
		}
	}

	public GAFObjectsManager manager
	{
		get
		{
			if (m_ObjectsManager == null)
			{
				m_ObjectsManager = GetComponent<GAFObjectsManager>();

				if (m_ObjectsManager == null)
				{
					m_ObjectsManager = gameObject.AddComponent<GAFObjectsManager>();
				}
			}

			return m_ObjectsManager;
		}
	}

	public GAFTexturesResource resource
	{
		get
		{
			return m_Resource;
		}

		set
		{
			m_Resource = value;
		}
	}

	public GAFAnimationPlayerSettings settings
	{
		get
		{
			return m_Settings;
		}
	}

	public bool isInitialized
	{
		get
		{
			return m_IsInitialized;
		}
	}

	public GAFSequenceData currentSequence
	{
		get
		{
			if (asset != null &&
			    asset.isLoaded)
			{
				return asset.getSequences(timelineID)[(int)getCurrentSequenceIndex()];
			}

			return null;
		}
	}

	#endregion // Properties

	#region MonoBehaviour

	private void FixedUpdate()
	{
		if (asset != null &&
		    asset.isLoaded &&
		    isPlaying() &&
		   !settings.ignoreTimeScale)
		{
			OnUpdate(Time.deltaTime);
		}
	}

	private void Update()
	{
		if (asset != null &&
		    asset.isLoaded &&
		    isPlaying() &&
		    settings.ignoreTimeScale)
		{
			OnUpdate(Mathf.Clamp(Time.realtimeSinceStartup - m_PreviouseUpdateTime, 0f, Time.maximumDeltaTime));
			m_PreviouseUpdateTime = Time.realtimeSinceStartup;
		}
	}

	private void Awake()
	{
#if UNITY_EDITOR
		GAFPostprocessorHelper.instance.on_resource_become_ready += delegate(GAFTexturesResource _Resource) 
		{
			if (resource == _Resource)
				reload();
		};
#endif // UNITY_EDITOR
	}

	private void OnEnable()
	{
		reload ();
	}

	private void Start()
	{
		if (Application.isPlaying)
			setPlaying (settings.playAutomatically);
	}

	private void OnDestroy()
	{
		if (null != resource)
		{
			resource.dispose();
		}

		clear (true);
	}

	private void OnApplicationFocus(bool _FocusStatus) 
	{
		if (!settings.playInBackground)
		{
			setPlaying(_FocusStatus);
		}
	}

	private void OnApplicationPause(bool _PauseStatus) 
	{
		if (!settings.playInBackground)
		{
			setPlaying(_PauseStatus);
		}
	}

	#endregion // MonoBehaviour

	#region Implementation

	private void OnUpdate(float _TimeDelta)
	{
		m_Stopwatch += _TimeDelta;

		if (m_Stopwatch >= settings.targetSPF)
		{
			int framesCount = 1;
			if (settings.perfectTiming)
			{
				m_StoredTime += m_Stopwatch - settings.targetSPF;
				if (m_StoredTime > settings.targetSPF)
				{
					int additionalFrames = Mathf.FloorToInt(m_StoredTime / settings.targetSPF);
					m_StoredTime = m_StoredTime - (additionalFrames * settings.targetSPF);
					framesCount += additionalFrames;
				}
			}

			m_Stopwatch = 0f;

			if (getCurrentFrameNumber() + framesCount > currentSequence.endFrame)
			{
				switch(settings.wrapMode)
				{
				case GAFWrapMode.Once:
					updateToFrame(currentSequence.endFrame, true);
					setPlaying (false);
					break;

				case GAFWrapMode.Loop:
					updateToFrame(currentSequence.startFrame, true);
					
					if (on_stop_play != null)
						on_stop_play(this);

					if (on_start_play != null)
						on_start_play(this);

					break;

				default:
					setPlaying (false);
					break;
				}
			}
			else
			{ 
				updateToFrame(getCurrentFrameNumber() + (uint)framesCount, false);
			}

			for (uint i = getCurrentFrameNumber() + 1; i <= getCurrentFrameNumber() + (uint)framesCount; i++)
			{
				if (m_FrameEvents.ContainsKey(i))
				{
					foreach (GAFFrameEvent _event in m_FrameEvents[i])
					{
						_event.trigger(this);
					}
				}
			}
		}
	}

	private void updateToFrame(uint _FrameNumber, bool _RefreshStates)
	{
		if (getCurrentFrameNumber() != _FrameNumber || _RefreshStates)
		{
			var states = getStates (_FrameNumber, _RefreshStates);
			if (states != null)
			{
				manager.updateToFrame(states, _RefreshStates);
			}

			m_CurrentFrameNumber = (int)_FrameNumber;
		}
	}

	private List<GAFObjectStateData> getStates(uint _FrameNumber, bool _RefreshStates)
	{
		if (!_RefreshStates)
		{
			_RefreshStates = _FrameNumber < getCurrentFrameNumber();
		}

		if (_RefreshStates)
		{
			var frame 	= new GAFFrameData(_FrameNumber);
			var objects = asset.getObjects(timelineID);
			var frames	= asset.getFrames(timelineID);

			foreach(var _obj in objects)
			{
				frame.addState(new GAFObjectStateData(_obj.id));
			}
			
			foreach(var _frame in frames)
			{
				if (_frame.Key > _FrameNumber)
					break;
				
				foreach(var _state in _frame.Value.states)
				{
					frame.states[_state.Key] = _state.Value;
				}
			}

			return frame.states.Values.ToList();
		}
		else
		{
			var frames = asset.getFrames(timelineID);
			if (_FrameNumber - getCurrentFrameNumber() == 1)
			{
				if (frames.ContainsKey(_FrameNumber))
				{
					return frames[_FrameNumber].states.Values.ToList();
				}
			}
			else
			{
				var frame = new GAFFrameData(_FrameNumber);
				foreach(var _frame in frames)
				{
					if (_frame.Key > _FrameNumber)
						break;
					
					if (_frame.Key < getCurrentFrameNumber())
						continue;
					
					foreach(var _state in _frame.Value.states)
					{
						frame.states[_state.Key] = _state.Value;
					}
				}

				return frame.states.Values.ToList();
			}

			return null;
		}
	}

	private void setPlaying(bool _IsPlay)
	{
		if (m_IsPlaying != _IsPlay)
		{
			m_IsPlaying = _IsPlay;

			if (m_IsPlaying)
			{
				if (on_start_play != null)
					on_start_play(this);

				m_Stopwatch = 0.0f;
				m_PreviouseUpdateTime = 0f;
			}
			else
			{
				if (on_stop_play != null)
					on_stop_play(this);

				m_Stopwatch = 0.0f;
				m_PreviouseUpdateTime = 0f;
			}
		}
	}

	private void upgrade()
	{
		GAFAnimationAsset			_asset				= asset;
		int							_timelineID			= timelineID;
		GAFAnimationPlayerSettings	_settings			= settings;
		int							_sequenceIndex		= m_SequenceIndex;
		int							_currentFrameNumber = m_CurrentFrameNumber;

		clear(true);

		m_Settings				= _settings;
		m_SequenceIndex			= _sequenceIndex;
		m_CurrentFrameNumber	= _currentFrameNumber;

		initialize(_asset, _timelineID);
	}

	#endregion // Implementation

	#region Members

	private int suppressMovieClipVersion() { return m_MovieClipVersion; }

	[HideInInspector][SerializeField] private int						 m_MovieClipVersion		= 0;
	[HideInInspector][SerializeField] private GAFObjectsManager			 m_ObjectsManager		= null;
	[HideInInspector][SerializeField] private GAFAnimationAsset			 m_GAFAsset				= null;
	[HideInInspector][SerializeField] private int						 m_TimelineID			= 0;
	[HideInInspector][SerializeField] private GAFAnimationPlayerSettings m_Settings				= new GAFAnimationPlayerSettings ();
	[HideInInspector][SerializeField] private int 						 m_SequenceIndex		= 0;	
	[HideInInspector][SerializeField] private int 						 m_CurrentFrameNumber 	= 1;
	[HideInInspector][SerializeField] private bool 						 m_IsInitialized		= false;
	[HideInInspector][SerializeField] private GAFTexturesResource		 m_Resource				= null;

	private Dictionary<uint, List<GAFFrameEvent>> m_FrameEvents = new Dictionary<uint, List<GAFFrameEvent>>();
	
	private bool 	m_IsPlaying 	= false;
	private float 	m_Stopwatch 	= 0f;
	private float 	m_StoredTime 	= 0f;

	private float 	m_PreviouseUpdateTime = 0f;

	#endregion // Members
}
