using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

//Function prototype for drawing an element in a list
public delegate void ListDrawFunction(UIElement p_element, System.Object p_data, int p_index);

//Function prototype for a button callback that is a part of the list.
public delegate void ListClickCallback(UISwipeList p_list, UIButton p_listElement, System.Object p_data, int p_index);

public class UISwipeList : UIElement
{
	public override void init (GameObject p_gameObject)
	{
		base.init (p_gameObject);
		m_scrollRect = p_gameObject.GetComponent<ScrollRect>() as ScrollRect;
		initScrollView();
		m_scale = 1.0f;
		m_increasLenght = 0f;
		m_isFirstFrame = true;
		m_buttonPrototypeMap = new Dictionary<UIButton, UIElement>();
		m_callbacks = new Dictionary<UIButton, List<ListClickCallback>>();
	}
	
	public void setData(List<System.Object> p_data)
	{
		m_data = p_data;

		if (!m_isFirstFrame)
		{
			adjustScrollSize();
			redrawPrototypes();
			layoutElements();

			m_numElements = m_data.Count;

	        calculateNumDrawElements();
	        createMissingElements();

	        applyCallListeners();
		}
		else
		{
			m_numElements = 0;
		}
	}

	public void setDataWithoutMove(List<System.Object> p_data)
	{
		m_data = p_data;
		
		if (!m_isFirstFrame)
		{
			staticAdjustScrollSize();
			redrawPrototypes();
			layoutElements();
			
			m_numElements = m_data.Count;
			
			calculateNumDrawElements();
			createMissingElements();
			
			applyCallListeners();
		}
		else
		{
			m_numElements = 0;
		}
	}

	public List<UIElement> getListElements()
	{
		return m_listElements;
	}

    public List<System.Object> getData()
    {
        return m_data;
    }
	
	public void setDrawFunction(ListDrawFunction p_drawFunction)
	{
		m_drawFunction = p_drawFunction;
	}
	
	public void addClickListener(string p_buttonName, ListClickCallback p_callback)
	{
        m_lastButtonName = p_buttonName;
        m_callback = p_callback;

        applyCallListeners();	

//		if( null != m_data )
//		{
//			setData( m_data );
//		}
	}
	

    public void applyCallListeners()
    {
        if (m_lastButtonName == null || m_callback == null)
            return;

        int l_numElements = m_listElements.Count;
        for (int i = 0; i < l_numElements; ++i)
        {
            UIElement l_prototype = m_listElements[i];
            UIButton l_button = getButton(m_lastButtonName, l_prototype);
            registerCallbackForButton(l_button, l_prototype, m_callback);
            registerButtonListener(l_button);
        }
    }

	public void removeClickListener(string p_buttonName, ListClickCallback p_callback)
	{
		int l_numElements = m_listElements.Count;
		for (int i = 0; i < l_numElements; ++i)
		{
			UIElement l_prototype = m_listElements[i];
			UIButton l_button = getButton(p_buttonName, l_prototype);
			removeCallbackForFunction(l_button, l_prototype, p_callback);
			removeButtonListener(l_button);
		}	
	}

	public void removeDrawFunction()
	{
		m_drawFunction = null;
	}

	public override void update()
	{
		base.update();
		if (!m_isFirstFrame)
		{
			checkDataChange();
			checkScrolling();
			redraw();
		}
		else
		{
			checkInitialScrollPosition();
			m_isFirstFrame = false;
		}
	}
	
	public override void dispose(bool p_deep)
	{
		base.dispose(p_deep); 
	}


    public override float alpha
    {
        get { return base.alpha; }

        set
        {
            base.alpha = value;
            foreach (UIElement e in m_listElements)
                e.alpha = value;
        }
    }

	public void setEnabled(bool p_enabled)
	{
		gameObject.SetActive(p_enabled);
	}
	
	//Doesn't work unless the element size is recalculated and prototypes are initialized to fill the new space.

	public void setScale(float p_scale)
	{
		DebugUtils.Assert( false == float.IsInfinity(p_scale) );
		DebugUtils.Assert( false == float.IsNaN(p_scale) );
		
		Vector3 l_scale = m_scrollPanel.transform.localScale;
		l_scale.Set(p_scale, p_scale, p_scale);
		m_scrollPanel.transform.localScale = l_scale;
				
		float l_inverseScale = 1f/p_scale;
		Text[] l_textObjects = gameObject.GetComponentsInChildren<Text>();
		
		int l_numTextObjects = l_textObjects.Length;
		for (int i = 0; i < l_numTextObjects; ++i)
		{
			Text l_text = l_textObjects[i];
			l_scale.Set(l_inverseScale, l_inverseScale, l_inverseScale);
			l_text.transform.localScale = l_scale;
		}
		
		m_scale = p_scale;
		
		//Re calculate layout
		calculateElementSize();
		calculateScrollSize();
		m_scrollSize = m_scrollSize * l_inverseScale;
		calculateNumDrawElements(); 
		createMissingElements();
		layoutElements();
		redrawPrototypes();
	}
	
	public void redraw()
	{
		redrawPrototypes();
	}

	public void addValueChangeListener( UnityEngine.Events.UnityAction<Vector2> p_lisentener)
	{
		m_scrollRect.onValueChanged.AddListener( p_lisentener );
	}

	public void removeValueChangeListener( UnityEngine.Events.UnityAction<Vector2> p_lisentener)
	{
		m_scrollRect.onValueChanged.RemoveListener( p_lisentener );
	}

	private void checkInitialScrollPosition()
	{
		m_scrollPanel.transform.localPosition = m_initialScrollPosition;
	}
	
	private void checkDataChange()
	{
		//Check if the list size has changed. if it has, then redraw the list.
		if( m_data == null )
			return;

		if (m_data.Count != m_numElements)
		{
			setData(m_data);
		}		
	}
	
	private void checkScrolling()
	{
		int l_currentDrawGroup = getCurrentDrawGroup();
		shiftElementsToDrawGroup(l_currentDrawGroup);		
	}
	
	private void shiftElementsToDrawGroup(int p_drawGroup)
	{
		if (m_drawGroup != p_drawGroup
		    && p_drawGroup >= 0
		    && p_drawGroup <= (m_data.Count - m_numVisibleElements))
		{
			//Shift to end of list as necessary.
			while (p_drawGroup > m_drawGroup)
			{
				//rearrange list
				UIElement l_element = m_listElements[0];
				m_listElements.RemoveAt(0);
				m_listElements.Add(l_element);
				
				//Update draw group counter
				m_drawGroup++;	
				
				//Physically move element to the end
				layoutElements();
						
				//Redraw current element
				drawElement(l_element);
			}
			
			//Shift to beginning of list as neccessary.
			while (p_drawGroup < m_drawGroup)
			{
				//rearrange list
				UIElement l_element = m_listElements[m_listElements.Count - 1];
				m_listElements.RemoveAt(m_listElements.Count - 1);
				m_listElements.Insert(0, l_element);
				
				//Update draw group counter
				m_drawGroup--;	
				
				//Physically move element to the end
				layoutElements();
				
				//Redraw current element
				drawElement(l_element);
			}
			
			if (m_drawGroup == 3)
				redraw();
			
		}
	}	

	private int getCurrentDrawGroup()
	{
		int l_currentDrawGroup = 0;
		float l_currentOffset = getCurrentScrollOffset();
		
		if (m_scrollRect.horizontal)
		{
			l_currentDrawGroup = (int)Mathf.Floor((l_currentOffset / (m_elementSize.x * m_scale)));
		}
		else
		{
			l_currentDrawGroup = (int)Mathf.Floor((l_currentOffset / (m_elementSize.y * m_scale)));			
		}						
		
		return l_currentDrawGroup;
	}	
	
	private float getCurrentScrollOffset()
	{
		float l_totalOffset = 0;
		Vector3 l_currentScrollPosition = m_scrollPanel.transform.localPosition;
		
		if (m_scrollRect.horizontal)
		{
			l_totalOffset = m_initialScrollPosition.x- l_currentScrollPosition.x;
		}
		else
		{
			l_totalOffset = l_currentScrollPosition.y - m_initialScrollPosition.y;
		}
		
		return l_totalOffset;
	}
	
	private void initScrollView()
	{
		initPrototype();
		adjustScrollSize();
	}
	
	private void initPrototype()
	{
		//Initialize draw group
		m_drawGroup = 0;
		
		//Get scroll panel that the prototypes go on.
		m_scrollPanel = gameObject.transform.FindChild("Content").gameObject;
		DebugUtils.Assert(m_scrollPanel != null);

		m_gridLayout = m_scrollPanel.GetComponent< GridLayoutGroup >();

		//Get prototype.
		m_prototype = m_scrollPanel.transform.FindChild("Prototype").gameObject;
		DebugUtils.Assert(m_prototype != null);
		
		//Get prototype size
		calculateElementSize();
		
		//Get the size of the viewable scroll area.
		calculateScrollSize();
		
		//Figure out how many prototypes need to be created
		calculateNumDrawElements();
		
		//Create list elements, using prototype as the first
		createListElements();
		
		//Lay out prototypes 
		layoutElements();
		
		getInitialScrollPosition();
		setInitialScrollPosition();

	}
	
	private void calculateElementSize()
	{
		RectTransform l_transform = getFirstRectTransform(m_prototype);
		m_elementSize = getSize(l_transform);
	}

	public void changeContentHeight(float p_height)
	{
		m_increasLenght = p_height;
	}
	
	private void calculateScrollSize()
	{
		m_scrollSize = getSize(transform);
	}
	
	private Vector2 getSize(Transform p_transform)
	{
		Vector2 l_size = new Vector2(0f, 0f);
		RectTransform l_transform = p_transform as RectTransform;	
		
		if (null != l_transform)
		{
			Rect l_rect = l_transform.rect;
			l_size.x = l_rect.width;
			l_size.y = l_rect.height;
		}
		
		return l_size;
	}
	
	private void calculateNumDrawElements()
	{
		if( m_gridLayout )
		{
            m_numVisibleElements = m_numElements;
                                    //(int)(Mathf.Ceil(m_scrollSize.y / 
			                        //( m_elementSize.y + m_gridLayout.padding.vertical) + 1) 
			                        //* m_gridLayout.constraintCount );

		} 
		else if (m_scrollRect.horizontal)
		{
			m_numVisibleElements = (int)(Mathf.Ceil(m_scrollSize.x / m_elementSize.x) + 1);
		}
		else
		{
			m_numVisibleElements = (int)(Mathf.Ceil(m_scrollSize.y / m_elementSize.y) + 1);
		}
	}
	
	private RectTransform getFirstRectTransform(GameObject p_gameObject)
	{
		RectTransform l_transform = null;
		
		if (m_prototype.transform is RectTransform)
		{
			l_transform = m_prototype.transform as RectTransform;
		}
		else
		{
			foreach (Transform l_child in p_gameObject.transform)
			{
				l_transform = getFirstRectTransform(l_child.gameObject);
			}
		}
		
		return l_transform;
	}
	
	private void layoutElements()
	{
		if( m_gridLayout )
			return;

		if (m_scrollRect.horizontal)
		{
			layoutElementsHorizontal();
		}
		else
		{
			layoutElementsVertical();
		}
	}
	
	private void layoutElementsHorizontal()
	{
		int l_numElements = m_listElements.Count;
		for (int i = 0; i < l_numElements; ++i)
		{
			UIElement l_element = m_listElements[i];
			int l_index = getIndexOfElement(l_element);
			l_element.x = ((l_index + 0.5f) * m_elementSize.x);
			l_element.y = (0);
			
//			_Debug.log(l_index + " layout");
		}
	}
	
	private void layoutElementsVertical()
	{
		int l_numElements = m_listElements.Count;
		for (int i = 0; i < l_numElements; ++i)
		{
			UIElement l_element = m_listElements[i];
			int l_index = getIndexOfElement(l_element);
			l_element.y = ((-l_index - 0.5f) * m_elementSize.y);
			l_element.x = (0);
		}		
	}		
	
	private void createListElements()
	{
		m_listElements = new List<UIElement>();
		UIElement l_element = attachToGameObject(m_prototype);
		m_listElements.Add(l_element);
		
		createMissingElements();
	}
	
	private void createMissingElements()
	{		
		while (m_listElements.Count < m_numVisibleElements)
		{
			GameObject l_newElement = GameObject.Instantiate(m_prototype) as GameObject;
			Transform l_newTransform = l_newElement.transform;
			Transform l_prototypeTransform = m_prototype.transform;

			l_newTransform.SetParent(l_prototypeTransform.parent);
			l_newTransform.localScale = l_prototypeTransform.localScale;//new Vector3(1, 1, 1);
			l_newTransform.localPosition = l_prototypeTransform.localPosition;

			UIElement l_element = attachToGameObject(l_newElement);
			m_listElements.Add(l_element);			
		}		
	}
	
	private void adjustScrollSize()
	{
		if( m_gridLayout )
			return;

        if (m_scrollPanel == null)
            return;

		RectTransform l_transform = m_scrollPanel.transform as RectTransform;
        

		if (null != m_data)
		{
			if (m_scrollRect.horizontal)
			{
				l_transform.pivot = new Vector2(0, l_transform.pivot.y);
				l_transform.offsetMin = new Vector2(0, -m_elementSize.y * 0.5f);
				l_transform.offsetMax = new Vector2(m_elementSize.x * m_data.Count, m_elementSize.y * 0.5f);
			}
			else
			{
				l_transform.pivot = new Vector2(l_transform.pivot.x, 1);
				l_transform.offsetMin = new Vector2(-m_elementSize.x * 0.5f, -m_elementSize.y * m_data.Count - m_increasLenght);
				l_transform.offsetMax = new Vector2(m_elementSize.x * 0.5f, 0);
			}
		}
	}

	private void staticAdjustScrollSize()
	{
		if( m_gridLayout )
			return;
		
		if (m_scrollPanel == null)
			return;
		
		RectTransform l_transform = m_scrollPanel.transform as RectTransform;

		Vector3 l_position = m_scrollPanel.transform.position;
		
		if (null != m_data)
		{
			if (m_scrollRect.horizontal)
			{
				l_transform.pivot = new Vector2(0, l_transform.pivot.y);
				l_transform.offsetMin = new Vector2(0, -m_elementSize.y * 0.5f);
				l_transform.offsetMax = new Vector2(m_elementSize.x * m_data.Count, m_elementSize.y * 0.5f);
			}
			else
			{
				l_transform.pivot = new Vector2(l_transform.pivot.x, 1);
				l_transform.offsetMin = new Vector2(-m_elementSize.x * 0.5f, -m_elementSize.y * m_data.Count - m_increasLenght);
				l_transform.offsetMax = new Vector2(m_elementSize.x * 0.5f, 0);
			}
			m_scrollPanel.transform.position = l_position;
		}
	}
	
	private void redrawPrototypes()
	{
		int l_numElements = m_listElements.Count;
		for (int i = 0; i < l_numElements; ++i)
		{
			UIElement l_element = m_listElements[i];
			drawElement(l_element);
		}
	}
	
	private void drawElement(UIElement p_element)
	{
		try {

        if (m_data == null || p_element.gameObject == null )
            return;

		if (null != m_drawFunction)
		{
			int l_index = getIndexOfElement(p_element);
			if( l_index >= m_data.Count )
			{
				p_element.gameObject.SetActive( false );
			}
			else 
			{
				p_element.gameObject.SetActive( true );
				System.Object l_data = m_data[l_index];
				m_drawFunction(p_element, l_data, l_index);
			}
		}

		}
		catch (Exception e) 
		{
			Debug.Log(e);
		}

	}
	
	private void getInitialScrollPosition()
	{
		m_initialScrollPosition = m_scrollPanel.transform.localPosition;
		RectTransform l_scrollTransform = m_scrollRect.transform as RectTransform;		
		Vector2 l_pivot = l_scrollTransform.pivot;
		Rect l_scrollRect = l_scrollTransform.rect;		

		if( m_gridLayout )
		{
			m_initialScrollPosition.x = 0.0f;
			m_initialScrollPosition.y = 0.0f;
		}
		else if (m_scrollRect.horizontal)
		{
			m_initialScrollPosition.x = -l_scrollRect.width * l_pivot.x;
			m_initialScrollPosition.y = 0;
		}
		else
		{
			m_initialScrollPosition.x = 0;
			m_initialScrollPosition.y = l_scrollRect.height * l_pivot.y;
		}
	}
	
	private void setInitialScrollPosition()
	{
		m_scrollPanel.transform.localPosition = m_initialScrollPosition;
	}
	
	private int getIndexOfElement(UIElement p_element)
	{
		int l_index = 0;
	
		int l_listPosition = m_listElements.IndexOf(p_element);
		l_index = l_listPosition + m_drawGroup;
		
		return l_index;
	}
	
	/*
	/ Button listener functions
	*/
	private UIButton getButton(string p_buttonName, UIElement p_prototype)
	{
		UIButton l_button = p_prototype.getView(p_buttonName) as UIButton;	
		
		if (null == l_button)
		{
			if (p_prototype.name.Contains(p_buttonName))
			{
				l_button = p_prototype as UIButton;
			}
		}
		
		return l_button;
	}
	
	private void registerCallbackForButton(UIButton p_button, UIElement p_prototype, ListClickCallback p_callback)
	{
		//If the key doesn't exist, create it
		if (!m_callbacks.ContainsKey(p_button))
		{
			m_callbacks[p_button] = new List<ListClickCallback>();
			m_buttonPrototypeMap[p_button] = p_prototype;
		}
		
		//Add the callback to the list
		if( !m_callbacks[p_button].Contains(p_callback) )
		{
			m_callbacks[p_button].Add(p_callback);
		}
	}
	
	private void registerButtonListener(UIButton p_button)
	{	
		if (m_callbacks[p_button].Count > 0)
		{
			p_button.removeAllCallbacks();
			p_button.addClickCallback(buttonCallback);	
		}
	}
	
	private void removeButtonListener(UIButton p_button)
	{
		if (m_callbacks.ContainsKey(p_button) == false)
		{
			p_button.removeClickCallback(buttonCallback);		
		}
	}	
	
	private void removeCallbackForFunction(UIButton p_button, UIElement p_prototype, ListClickCallback p_callback)
	{
		if (m_callbacks.ContainsKey(p_button))
		{
			//Find callback
			List<ListClickCallback> l_callbacks = m_callbacks[p_button];			
			int l_callbackIndex = l_callbacks.IndexOf(p_callback);
			
			//Try to remove callback
			if (-1 != l_callbackIndex)
			{
				l_callbacks.RemoveAt(l_callbackIndex);
			}
			else
			{
				_Debug.log("Callback not found: " + p_callback + " for button " + p_button.name);
			}	
			
			//If the list is empty remove it
			if (l_callbacks.Count == 0)
			{
				l_callbacks.Remove(p_callback);
				m_buttonPrototypeMap.Remove(p_button);
			}						
		}
	}	
	
	private int getButtonIndex(UIButton p_button)
	{
		int l_index = -1;
		
		if (m_buttonPrototypeMap.ContainsKey(p_button))
		{
			UIElement l_prototype = m_buttonPrototypeMap[p_button];
			l_index = getIndexOfElement(l_prototype);			
		}	
		
		return l_index;
	}
	
	public void buttonCallback(UIButton p_button)
	{
		//Get callback
		List<ListClickCallback> l_callbacks = null;
		if (m_callbacks.ContainsKey(p_button))
		{
			l_callbacks = m_callbacks[p_button];
		}
		
		//Get data
		UIButton l_view = p_button;
		int l_index = getButtonIndex(p_button);
		UISwipeList l_swipeList = this;
		System.Object l_data = m_data[l_index];
		
		//Issue callbacks
		int l_numCallbacks = l_callbacks.Count;
		for (int i = 0; i < l_numCallbacks; ++i)
		{
			ListClickCallback l_callback = l_callbacks[i];
			l_callback(l_swipeList, l_view, l_data, l_index);
		}				
	}
	
	//The Unity objects this class is wrapping
	private ScrollRect m_scrollRect;
	private GameObject m_scrollPanel;
	
	//A gameobject representing an individual list item, derived from the lists initial contents
	private GameObject m_prototype;
	
	//Callback to draw an element
	private ListDrawFunction m_drawFunction;
	
	//The ui elements this scroll list created
	private List<UIElement>	m_listElements;
	
	//The data assigned to the scroll list for the elements
	private List<System.Object> m_data;
	
	//The size of each list element in the current screens coordinates
	private Vector2 m_elementSize;
	
	//The size of the scroll rect in current screen coordinates
	private Vector2 m_scrollSize;
	
	//The max number of elements that are visible at any time
	private int m_numVisibleElements;
	
	//Position when the scroll list was initialized. 
	private Vector3 m_initialScrollPosition;
	
	//Indicates how many times an element has been shifted to the front of the list.
	private int m_drawGroup;
		
	//State information to keep track of last known size of the list.
	private int m_numElements;
	
	//Current scale factor
	private float m_scale;
	
	//Mapping of button to prototype for list callbacks.
	private Dictionary<UIButton, UIElement> m_buttonPrototypeMap;
	
	//The callbacks shared between buttons
	private Dictionary<UIButton, List<ListClickCallback>> m_callbacks;	
	
	//Fix for first frame weirdness on scroll position
	private bool m_isFirstFrame;

	//Has grid layout component
	private GridLayoutGroup m_gridLayout;

    //Last callback set
    private string m_lastButtonName;

	//Increasing the length of the content.
	private float m_increasLenght;
    private ListClickCallback m_callback = null;
}
