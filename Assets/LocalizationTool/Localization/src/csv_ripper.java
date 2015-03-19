import processing.core.*;

import org.json.*; 

import java.io.*;
import java.util.*; 

public class csv_ripper extends PApplet {
	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;

	public static String m_filePath;
	
	private ArrayList<PrintWriter> m_printWriters;
	private ArrayList<JSONObject> m_JSONObjects;
	
	private ArrayList<String> m_outputFileNames;
	
	private BufferedReader m_reader;
	private Boolean m_hasLines;
	
	public void setup()
	{
		if(m_filePath != null)
		{
			File l_file = new File(m_filePath);
			try {
				parseSelectedFile(l_file);
			} catch (IOException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			} catch (JSONException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		}
		else
		{
			selectInput("Select the .csv file to convert", "parseSelectedFile");
		}
	}
	
	public void parseSelectedFile(File p_file) throws IOException, JSONException
	{
		if (p_file == null) {
			println("Window was closed or the user hit cancel.");
			exit();
		}
		else
		{
			println("User selected " + p_file.getAbsolutePath());
			m_filePath = p_file.getAbsolutePath().toString();
		}
		
		m_reader = createReader( m_filePath );
		m_hasLines = true;
		parseFirstLine();
		parseRows();
	}
	
	private void parseFirstLine()
	{	
		String l_localLine;
		String [] l_langColumn;
	
		if ( null == m_reader )
		{
			return;
		}
		
		// The first row should represent the file name
		if ( m_hasLines )
		{
			m_printWriters 		= new ArrayList<PrintWriter>();
			m_JSONObjects		= new ArrayList<JSONObject>();
			m_outputFileNames 	= new ArrayList<String>();
			
			try
			{
				l_localLine = m_reader.readLine();
			} 
			catch(IOException e) 
			{
				e.printStackTrace();
				l_localLine = null;
			}
			if ( null != l_localLine )
			{
				println("The Line: " + l_localLine);
				l_langColumn = split( l_localLine, ";"  );
				println( "cutup: " + l_langColumn[1] );
				l_localLine = l_localLine.replace(" ", "" );
				
				for( int i = 1; i < l_langColumn.length; ++i)
				{
					String langTitle = l_langColumn[i].replace("\"", "");
					m_outputFileNames.add("output\\localization" + langTitle + ".txt");
					m_printWriters.add(createWriter(m_outputFileNames.get(i-1)));
					m_JSONObjects.add(new JSONObject());
				}
	
			}
			else if ( null == l_localLine )
			{
				exit();
			}
		}
	}
	
	private void parseRows() throws IOException, JSONException
	{
		String l_localLine;
		String [] l_langColumn;
		ArrayList<String> l_columnKeys = new ArrayList<String>();
	
		if ( null == m_reader )
		{
			return;
		}
	
		while ( m_hasLines )
		{
			try
			{
				l_localLine = m_reader.readLine();
			} 
			catch(IOException e) 
			{
				e.printStackTrace();
				l_localLine = null;
			}
			if ( null != l_localLine )
		    {
				l_langColumn = split( l_localLine, ";" );
				println( "cutup: " + l_langColumn[0] );
	      
				for( int i = 0; i < l_langColumn.length; ++i)
				{
					String langKey = l_langColumn[i].replace("\"", "");
					println("LangKey: " + langKey);
					l_columnKeys.add(langKey);
				}
				
				println( l_columnKeys.get(0) + " : " + l_columnKeys.get(1) );
	
				addRowToJSON( l_columnKeys );
				
				l_columnKeys.clear();
			}
		
			else if ( null == l_localLine )
			{
				closeFiles();
			}
		}
	}
	
	public void addRowToJSON(ArrayList<String> p_columnKeys)
	{
		p_columnKeys.set(0, p_columnKeys.get(0).replace(" ", ""));
		for(int i = 1; i < p_columnKeys.size(); ++i)
		{
			JSONObject l_jsonObj = m_JSONObjects.get(i-1);
			try {
				l_jsonObj.put(p_columnKeys.get(0), p_columnKeys.get(i));
			} catch (JSONException e) {
				e.printStackTrace();
			}
		}
	}
	
	public void closeFiles() throws IOException, JSONException
	{
		m_hasLines = false;
		for(int i = 0; i < m_printWriters.size(); ++i)
		{
			JSONObject l_jsonObj = m_JSONObjects.get(i);
			PrintWriter l_printWriteObj = m_printWriters.get(i);
			
			l_jsonObj.write(l_printWriteObj);
			
			l_printWriteObj.flush();
			l_printWriteObj.close();
			
			cleanUpFile(i);
		}
		exit();
	}
	
	private void cleanUpFile(int p_index) throws IOException
	{
		String[] l_lines = reReadInFile(p_index);
		reWriteFile(p_index, l_lines);
	}
	
	private String[] reReadInFile(int p_index) throws IOException
	{
		ArrayList<String> l_lines = new ArrayList<String>();
		
		FileReader l_file = new FileReader(m_outputFileNames.get(p_index));
		BufferedReader l_br = new BufferedReader(l_file);
		
		boolean l_hasLines = true;
		
		while(l_hasLines)
		{
			String l_line = l_br.readLine();
			if(l_line != null)
			{
				l_lines.add(l_line);
			}
			else
			{
				l_hasLines = false;
			}
		}
		
		l_file.close();
		
		String[] l_strLines = new String[l_lines.size()];
		
		for(int i = 0; i < l_lines.size(); ++i)
		{
			l_strLines[i] = l_lines.get(i);
		}
		return l_strLines;
	}
	
	private void reWriteFile(int p_index, String[] p_lines) throws IOException
	{
		FileWriter l_writer = new FileWriter(m_outputFileNames.get(p_index), false);
		PrintWriter l_printWriteObj = new PrintWriter(l_writer);
		
		for(int i = 0; i < p_lines.length; ++i)
		{
			boolean lastLine = i == p_lines.length-1;
			p_lines[i] = p_lines[i].replace("\\\\n", "\\n");
			l_printWriteObj.print(p_lines[i] + ((lastLine) ? "" : "\n"));
		}
		
		l_printWriteObj.close();
	}
	
	static public void main(String args[]) {
		if(args.length > 0)
		{
			m_filePath = args[0];
			println("File Path: " + m_filePath);
		}
		else
		{
			println("No file designated, user must select one!");
			m_filePath = null;
		}
		PApplet.main(new String[] { "--bgcolor=#F0F0F0", "csv_ripper" });
	}
}
