using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextureManager : System.Object
{
	public const int MAX_UNDO_DEPTH = 10;

	public TextureManager(GameController p_gameController, Texture2D p_texture)
	{
		m_texture = p_texture;
		m_undoStack = new List<Color[]>();
	}

	public void save()
	{
		byte[] l_bytes = m_texture.EncodeToPNG();
		RequestQueue l_queue = new RequestQueue();
		if (SessionHandler.getInstance().currentDrawing == null)
			l_queue.add(new NewDrawingRequest(l_bytes));
		else
			l_queue.add(new SaveDrawingRequest(l_bytes));
		l_queue.request(RequestType.RUSH);
	}

	public void load()
	{
		string l_path = getPath();
		m_www = new WWW("file://" + l_path);
	}

	public void reset()
	{
		Color[] l_pixels = m_texture.GetPixels();//Get all pixels

		int l_numPixels = l_pixels.Length;
		Color l_white = Color.white;
		for (int i = 0; i < l_numPixels; ++i)
		{
			l_pixels[i] = l_white;
		}

		m_texture.SetPixels(l_pixels);
		m_texture.Apply();
	}

	public void revertToUndoPoint()
	{
		if (m_pixels != null)
		{
			m_texture.SetPixels(m_pixels);
			m_texture.Apply();
		}
	}

	public void setUndoPoint()
	{
		m_pixels = m_texture.GetPixels();
	}

	public void pushUndoPoint()
	{
		Color[] l_pixels = m_texture.GetPixels();
		m_undoStack.Add(l_pixels);

		if (MAX_UNDO_DEPTH < m_undoStack.Count)
		{
			m_undoStack.RemoveAt(0);
		}
	}

	public void undo()
	{
		int l_undoIndex = m_undoStack.Count - 1;
		if (l_undoIndex >= 0)
		{
			Color[] l_undoPixels = m_undoStack[l_undoIndex];
			m_undoStack.RemoveAt(l_undoIndex);
			m_texture.SetPixels(l_undoPixels);
			m_texture.Apply();
		}
	}

	public void update()
	{
		if (null != m_www)
		{
			if (m_www.isDone)
			{
				m_www.LoadImageIntoTexture(m_texture);
				m_www.Dispose();
				m_www = null;
			}
		}
	}

	public void dispose()
	{
		m_undoStack.Clear();
		m_undoStack = null;
		m_pixels = null;
		GameObject.Destroy(m_texture);
		m_texture = null;
	}

	private string getPath()
	{
		return Application.persistentDataPath + "/zoodles_image.png";
	}

	private WWW m_www;
	private Color[] m_pixels;
	private Texture2D m_texture;
	private List<Color[]> m_undoStack;
}





