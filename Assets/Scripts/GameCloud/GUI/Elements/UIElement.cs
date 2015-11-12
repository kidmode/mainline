using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class UIElement : System.Object
{
	public string name;
	public UIElement parent;
	public Transform transform;
	public GameObject gameObject;
	
	public Tweener tweener;
	
	public static UIElement attachToGameObject(GameObject p_gameObject)
	{
		//Create the current element wrapper and initialize it.
		UIElement l_element = getUIElement(p_gameObject);
		l_element.init(p_gameObject);
		
		//Add children if the element doesn't do it in its own init.
		setupChildren( l_element );
		
		return l_element;
	}
	
	public static void setupChildren( UIElement p_element )
	{
		//Add children if the element doesn't do it in its own init.
		if( p_element.m_children.Count == 0 )
		{
			foreach (Transform l_transform in p_element.gameObject.transform)
			{
				if( l_transform.tag == "IgnoreUI")
					continue;
				
				GameObject l_childObject = l_transform.gameObject;
				UIElement l_child = attachToGameObject(l_childObject);
				p_element.addChild(l_child);
			}
		}
	}
	
	
	public virtual void init(GameObject p_gameObject)
	{
		name 		= p_gameObject.name;
		transform 	= p_gameObject.transform;
		gameObject 	= p_gameObject;
		
		tweener 	= new Tweener( this );
		m_children 	= new List<UIElement>();

		m_alpha		= 1.0f;

		setupChildren( this );
	}
	
	public virtual float alpha
	{
		get 
		{ 
			return m_alpha; 
		}
		set 
		{
			m_alpha = value;
			if (null != m_children)
			{
				int l_numChildren = m_children.Count;
				for (int i = 0; i < l_numChildren; ++i)
				{
					UIElement l_child = m_children[i];
					l_child.alpha = value;
				}
			}
		}
	}
	
	public System.Object userData
	{
		get;
		set;
	}
	
	public bool active
	{
		get { return gameObject.activeSelf; 	}
		set 
		{
			if (gameObject != null)
			{
				if (gameObject.activeSelf != value)
				{
					gameObject.SetActive(value); 	
				}
			}
		}
	}
	
	public float u
	{
		set
		{
			Vector3 l_position = transform.localPosition;
			l_position.x = (value - 0.5f) * Screen.width;
			transform.localPosition = l_position;
		}
	}
	
	public float v
	{
		set
		{
			Vector3 l_position = transform.localPosition;
			l_position.y = (value - 0.5f) * Screen.height;
			transform.localPosition = l_position;
		}
	}
	
	public float x
	{
		set
		{
			if (transform == null) return;
			
			Vector3 l_position = transform.localPosition;
			l_position.x = value;
			transform.localPosition = l_position;
		}
		get
		{
			Vector3 l_position = transform.localPosition;
			return l_position.x;	
		}
	}
	
	public float y
	{
		set
		{
			if (transform == null) return;
			
			Vector3 l_position = transform.localPosition;
			l_position.y = value;
			transform.localPosition = l_position;
		}
		get		
		{
			Vector3 l_position = transform.localPosition;
			return l_position.y;
		}
	}
	
	public float z
	{
		set
		{
			if (transform == null) return;
			
			Vector3 l_position = transform.localPosition;
			l_position.z = value;
			transform.localPosition = l_position;
		}
		get
		{
			Vector3 l_position = transform.localPosition;
			return l_position.z;
		}
	}

	public UIEventListener listener
	{
		get
		{
			return UIEventListener.attach(this);
		}
	}
	
	public UICanvas canvas
	{
		get
		{
			UIElement l_parent = this;
			while (l_parent != null && false == (l_parent is UICanvas))
			{
				l_parent = l_parent.parent;
			}

			return (l_parent as UICanvas);
		}
	}
	
	public virtual void update()
	{
		DebugUtils.Assert(null != m_children);
		
		tweener.update( Time.deltaTime );
		
		if (null != m_children)
		{
			int i;
			int l_numChildren;
			
			l_numChildren = m_children.Count;
			for (i = l_numChildren - 1; i >= 0; --i)
			{
				UIElement l_child = m_children[i];
				l_child.update();
			}
		}
	}
	
	public virtual void dispose(bool p_deep)
	{
		clearChildren(p_deep);
		removeSelf();
		
		m_children = null;
		parent = null;
		transform = null;
		name = null;
	}
	
	public void removeSelf()
	{
		if (null != parent)
		{
			parent.removeChild(this);
			parent = null;
		}
	}
	
	public void clearChildren(bool p_deep)
	{
		DebugUtils.Assert(null != m_children);
		
		if (null != m_children)
		{
			int i;
			int l_numChildren;
			
			l_numChildren = m_children.Count;
			for (i = l_numChildren - 1; i >= 0; --i)
			{
				UIElement l_child = m_children[i];
				l_child.dispose(p_deep);
			}
			
			m_children.Clear();
		}
	}
	
	public void addChild(UIElement p_child)
	{
		DebugUtils.Assert(null != p_child);
		DebugUtils.Assert(null == p_child.parent);
		DebugUtils.Assert(null != m_children);
		
		m_children.Add(p_child);
		p_child.parent = this;
	}
	
	public void addChildAt(UIElement p_child, int p_index)
	{
		DebugUtils.Assert(null != p_child);
		DebugUtils.Assert(null == p_child.parent);
		DebugUtils.Assert(null != m_children);
		
		m_children.Insert(p_index, p_child);
		p_child.parent = this;
	}
	
	public void removeChild(UIElement p_child)
	{
		DebugUtils.Assert(null != p_child);
		DebugUtils.Assert(null != p_child.parent);
		DebugUtils.Assert(null != m_children);
		DebugUtils.Assert(m_children.Contains(p_child));
		
		p_child.parent = null;
		m_children.Remove(p_child);
	}
	
	public void removeChildAt(int p_index)
	{
		DebugUtils.Assert(p_index >= 0);
		DebugUtils.Assert(p_index < m_children.Count);
		DebugUtils.Assert(null != m_children);
		DebugUtils.Assert(null != m_children[p_index].parent);
		
		UIElement l_element = m_children[p_index];
		l_element.parent = null;
		m_children.RemoveAt(p_index);
	}
	
	public int getChildCount()
	{
		DebugUtils.Assert(null != m_children);
		return m_children.Count;
	}
	
	public UIElement getChildAt(int p_index)
	{
		DebugUtils.Assert(p_index >= 0);
		DebugUtils.Assert(null != m_children);
		DebugUtils.Assert(p_index < m_children.Count);
		
		return m_children[p_index];
	}
	
	public UIElement getView(string p_name)
	{
		//If this is the view, return this.
		if (p_name.Equals(name))
		{
			return this;
		}
		
		//Iterate through children to see if the view is a decendant.
		UIElement l_element = null;
		if(null != m_children)
		{
			int l_numChildren = m_children.Count;
			for (int i = 0; i < l_numChildren; ++i)
			{
				UIElement l_child = m_children[i];
				l_element = l_child.getView(p_name);
				
				//If view was found, exit
				if (null != l_element)
				{
					break;
				}
			}
		}
		
		return l_element;
	}	
	
	public ArrayList getViewsByTag(string p_tagName)
	{
		ArrayList l_list = new ArrayList ();
		
		int l_numChildren = m_children.Count;
		for (int i = 0; i < l_numChildren; ++i)
		{
			UIElement l_child = m_children[i];
			if(p_tagName.Equals(l_child.gameObject.transform.tag))
			{
				l_list.Add(l_child);
			}
		}
		return l_list;
	}

	public void getViewsByTag(string p_tagName,ArrayList p_list)
	{
		if(p_tagName.Equals(gameObject.transform.tag))
		{
			p_list.Add(this);
		}

		int l_numChildren = m_children.Count;
		for (int i = 0; i < l_numChildren; ++i)
		{
			UIElement l_child = m_children[i];
			l_child.getViewsByTag(p_tagName,p_list);
		}
	}
	
	
	public Behaviour baseElement
	{
		get { return m_baseElement; 	}
		protected set { m_baseElement = value;	}
	}
	
	private static UIElement getUIElement(GameObject p_gameObject)
	{
		UIType l_type = getUITypeFromGameObject(p_gameObject);
		UIElement l_element = createUIElementOfType(l_type);
		return l_element;
	}
	
	private static UIType getUITypeFromGameObject(GameObject p_gameObject)
	{
		UIType l_type = UIType.element;
		
		if( p_gameObject.GetComponents<Canvas>().Length > 0 )
		{
			l_type = UIType.canvas;
		}
		else if( p_gameObject.GetComponents<GAFMovieClip>().Length > 0 )
		{
			l_type = UIType.movieclip;
		}
		else if( p_gameObject.GetComponents<InputField>().Length > 0 )
		{
			l_type = UIType.inputField;
		}
		else if (p_gameObject.GetComponents<Button>().Length > 0)
		{
			l_type = UIType.button;
		}
		else if (p_gameObject.GetComponents<ScrollRect>().Length > 0)
		{
			if(!p_gameObject.transform.tag.Equals("ignoreScrollRect"))
				l_type = UIType.scrollView;
		}
		else if (p_gameObject.GetComponents<Slider>().Length > 0 )
		{
			l_type = UIType.slider;
		}
		else if (p_gameObject.GetComponents<Text>().Length > 0)
		{
			l_type = UIType.label;
		}
		else if (p_gameObject.GetComponents<Image>().Length > 0)
		{
			l_type = UIType.image;
		}
		else if (p_gameObject.GetComponents<RawImage>().Length > 0)
		{
			l_type = UIType.rawImage;
		}
		else if (p_gameObject.GetComponents<Mask>().Length > 0)
		{
			l_type = UIType.mask;
		}
		else if( p_gameObject.GetComponents<Toggle>().Length > 0 )
		{
			l_type = UIType.toggle;
		}
		else if( p_gameObject.GetComponents<ToggleGroup>().Length > 0 )
		{
			l_type = UIType.toggleGroup;
		}

		if(p_gameObject.tag.Equals("ComboBox"))
		{
			l_type = UIType.comboBox;
		}

		return l_type;
	}
	
	private static UIElement createUIElementOfType(UIType p_type)
	{
		UIElement l_element = null;
		
		switch (p_type)
		{
		case UIType.canvas:
			l_element = new UICanvas();
			break;
		case UIType.scrollView:
			l_element = new UISwipeList();
			break;
		case UIType.image:
			l_element = new UIImage();
			break;
		case UIType.rawImage:
			l_element = new UIRawImage();
			break;
		case UIType.button:
			l_element = new UIButton();
			break;
		case UIType.label:
			l_element = new UILabel();
			break;
		case UIType.slider:
			l_element = new UISlider();
			break;
		case UIType.toggle:
			l_element = new UIToggle();
			break;
		case UIType.toggleGroup:
			l_element = new UIToggleGroup();
			break;
		case UIType.movieclip:
			l_element = new UIMovieClip();
			break;
		case UIType.inputField:
			l_element = new UIInputField();
			break;
		case UIType.comboBox:
			l_element = new UIComboBox();
			break;
		default:
			l_element = new UIElement();
			break;
		}
		
		return l_element;
	}
	
	
	private Behaviour m_baseElement; 
	private List<UIElement> m_children;
	protected float m_alpha;
}

public enum UIType
{
	element,
	label,
	image,
	rawImage,
	button,
	scrollView,
	mask,
	toggle,
	toggleGroup,
	slider,
	canvas,
	movieclip,
	inputField,
	comboBox
}
