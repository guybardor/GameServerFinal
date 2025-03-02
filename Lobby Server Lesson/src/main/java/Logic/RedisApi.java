package Logic;

import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.List;

public class RedisApi 
{
	public static void SaveMapToRedis(String _PlayerId,LinkedHashMap<String, Object> _Data)
	{
		for(String str : _Data.keySet())
		{
			RedisFunctions.RedisSet(_PlayerId + "/" + str, _Data.get(str).toString());
			System.out.println(_PlayerId + "/" + str + " , "+ _Data.get(str).toString());
		}
	}
	
	public static void SaveToRedis(String _PlayerId,String _Key, String _Value)
	{
		RedisFunctions.RedisSet(_PlayerId + "/" + _Key, _Value);
	}
	
	public static String GetFromRedis(String _PlayerId,String _Key)
	{
		return RedisFunctions.RedisGet(_PlayerId + "/" + _Key);
	}
	
	public static LinkedHashMap<String, Object> GetLoginDetails(String _PlayerId)
	{
		LinkedHashMap<String, Object> _ret = new LinkedHashMap<String, Object>();
		_ret.put("PlayerId", _PlayerId);
		
		List<String> _keys = new ArrayList<String>();
		_keys.add("Country");
		_keys.add("NickName");
		_keys.add("ProfileImg");
		_keys.add("Coins");
		_keys.add("Xp");
		_keys.add("XpLevel");
		_keys.add("Crm");
		_keys.add("CrmLevel");
		_keys.add("Energy");
		_keys.add("MaxEnergy");
		_keys.add("LastEnergyCollectionDate");
		_keys.add("LastDailyBonusDate");
		
		Boolean _bringDB = false;
		String _temp = "";
		for(String s : _keys)
		{
			_temp = GetFromRedis(_PlayerId,s);
			if(_temp.equals(""))
				return new LinkedHashMap<String, Object>();
			else _ret.put(s, _temp);
			System.out.println(_temp);
		}
		return _ret;
	}
}
