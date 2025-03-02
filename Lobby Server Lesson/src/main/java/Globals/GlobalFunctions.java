package Globals;

import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.URL;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.Date;
import java.util.LinkedHashMap;
import java.util.Locale;
import java.util.Random;
import java.util.TimeZone;
import java.util.regex.Pattern;

import javax.websocket.Session;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import com.google.gson.reflect.TypeToken;

public class GlobalFunctions
{
	public static LinkedHashMap<String, Object> ParseJsonToDictionary(String _Message)
	{
		 Gson _gson = new GsonBuilder().create();
	     java.lang.reflect.Type typeOfHashMap = new TypeToken<LinkedHashMap<String, Object>>() { }.getType();
	     LinkedHashMap<String, Object> newMap = _gson.fromJson(_Message, typeOfHashMap); // This type must match TypeToken	
		 return newMap;
	}
	
	public static String getHTML(String urlToRead) throws Exception 
	{
	      StringBuilder result = new StringBuilder();
	      URL url = new URL(urlToRead);
	      HttpURLConnection conn = (HttpURLConnection) url.openConnection();
	      conn.setRequestMethod("GET");
	      BufferedReader rd = new BufferedReader(new InputStreamReader(conn.getInputStream()));
	      String line;
	      while ((line = rd.readLine()) != null) {
	         result.append(line);
	      }
	      rd.close();
	      return result.toString();
	}	
	public static String SerializeToJson(Object _Value)
	{
		Gson _gson = new GsonBuilder().create();
	    String _result = _gson.toJson(_Value);
	    return _result;
	}
	
	public static String PrepairGameName(String _GameName)
	{
		_GameName = _GameName.replace(" ", "%20");
		return _GameName;
	}
	

	public static Boolean CheckIfUserIsValid(String _DeviceId)
	{
		if(_DeviceId != null && _DeviceId.equals(""))
			return false;
		return true;
	}	
	
	
	public static Integer GetRandomNumber(int aStart, int aEnd)
	{
		Random aRandom = new Random();
		long range = (long)aEnd - (long)aStart + 1;
		long fraction = (long)(range * aRandom.nextDouble());
		int randomNumber =  (int)(fraction + aStart);
		return randomNumber;
	}
	
	public static Date GetDate()
	{
		return ParseStringDate(GetDateString());
	}

	public static String GetDateString()
	{
		TimeZone.setDefault(TimeZone.getTimeZone("UTC"));
		TimeZone timeZone = TimeZone.getTimeZone("UTC");
		Calendar calendar = Calendar.getInstance(timeZone);
		SimpleDateFormat simpleDateFormat = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss", Locale.US);
		simpleDateFormat.setTimeZone(timeZone);
		
		String _returnValue = calendar.get(Calendar.YEAR) + "-" + (calendar.get(Calendar.MONTH) + 1) + "-" + calendar.get(Calendar.DAY_OF_MONTH) + " ";
		_returnValue += calendar.get(Calendar.HOUR_OF_DAY) + ":" + calendar.get(Calendar.MINUTE) + ":" + calendar.get(Calendar.SECOND);
	
		return _returnValue;
	}
	public static Date ParseStringDate(String _DateToParse)
	{
		try
		{
			String[] _tokens = _DateToParse.split( "[ ]");
		
			String[] _tokens2 = _tokens[0].split("[-]");
			String[] _tokens3 = _tokens[1].split("[:]");
			int _year = Integer.parseInt(_tokens2[0]);
			
			int _month = Integer.parseInt(_tokens2[1]) - 1;
			int _days = Integer.parseInt(_tokens2[2]);
			int _hours = Integer.parseInt(_tokens3[0]);
			int _minutes = Integer.parseInt(_tokens3[1]);
			
			String _t = _tokens3[2];
			String[] _sec = _t.split(Pattern.quote("."));
			int _seconds = Integer.parseInt(_sec[0]);

			Calendar cal = Calendar.getInstance();
			cal.set(_year,_month, _days,_hours,_minutes,_seconds); 
			return cal.getTime();
		}
		catch(Exception e)
		{
			String _e = e.getMessage();
		}
		
		Calendar cal = Calendar.getInstance();
		return cal.getTime();
	}
	
	public static String GeneratePlayerId()
	{
		TimeZone.setDefault(TimeZone.getTimeZone("UTC"));
		TimeZone timeZone = TimeZone.getTimeZone("UTC");
		Calendar calendar = Calendar.getInstance(timeZone);
//		SimpleDateFormat simpleDateFormat = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss", Locale.US);
//		simpleDateFormat.setTimeZone(timeZone);

		Integer _YEARInt = calendar.get(Calendar.YEAR) + 138;
		Integer _MONTHInt = calendar.get(Calendar.MONTH);
		if(_MONTHInt + 1 < 13)
			_MONTHInt += 1;
		else _MONTHInt = 1;
		
		Integer _DAY_OF_MONTHInt = calendar.get(Calendar.DAY_OF_MONTH);
		Integer _HOUR_OF_DAYInt = calendar.get(Calendar.HOUR_OF_DAY);
		Integer _MINUTEInt = calendar.get(Calendar.MINUTE);
		Integer _SECONDInt = calendar.get(Calendar.SECOND);
		Integer _MILLISECONDInt = calendar.get(Calendar.MILLISECOND);

		String _year = _YEARInt.toString();
		
		String _month = _MONTHInt.toString();
		if(_MONTHInt < 10)
			_month = "0" + _MONTHInt.toString();
		
		String _day = _DAY_OF_MONTHInt.toString();
		if(_DAY_OF_MONTHInt < 10)
			_day = "0" + _DAY_OF_MONTHInt.toString();
		
		String _hour = _HOUR_OF_DAYInt.toString();
		if(_HOUR_OF_DAYInt < 10)
			_hour = "0" + _HOUR_OF_DAYInt.toString();
		
		String _minute = _MINUTEInt.toString();
		if(_MINUTEInt < 10)
			_minute = "0" + _MINUTEInt.toString();
		
		String _second = _SECONDInt.toString();
		if(_SECONDInt < 10)
			_second = "0" + _SECONDInt.toString();
		
		String _milisecond = _MILLISECONDInt.toString();
		if(_MILLISECONDInt < 100)
		{
			if(_MILLISECONDInt < 10)
				_milisecond = "00" + _MILLISECONDInt.toString();
			else _milisecond = "0" + _MILLISECONDInt.toString();
		}

		_year =  new StringBuilder(_year).reverse().toString();
		_month =  new StringBuilder(_month).reverse().toString();
		_minute =  new StringBuilder(_minute).reverse().toString();
	//	String _returnValue = new StringBuilder(_year + _month + _day + _hour + _minute + _second + _milisecond).reverse().toString();
		String _returnValue = _year + _month + _day + _hour + _minute + _second + _milisecond;
		System.out.println(_returnValue + " " + _returnValue.length());
		
		return _returnValue;
	}
	
	public static Date ParseStringCalender(String _DateToParse) throws ParseException
	{
		Calendar cal = Calendar.getInstance();
		SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss", Locale.ENGLISH);
		cal.setTime(sdf.parse(_DateToParse));// all done
		return cal.getTime();
	}
	
	public static String GetName()
	{
		return "arm";
	}
	
	public static String GetImg()
	{
		return "http://Img";
	}

	
}
