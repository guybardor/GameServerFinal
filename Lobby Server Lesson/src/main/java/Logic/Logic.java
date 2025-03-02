package Logic;

import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.List;

import com.google.gson.internal.LinkedTreeMap;

import Globals.GlobalFunctions;
import Globals.GlobalVariables;

public class Logic 
{
	private static Logic instance;
	public static Logic getInstance() 
	{
      if(instance == null) {
         instance = new Logic();
      }
      return instance;
    }
	
	@SuppressWarnings("unchecked")
	public String DoLogic(String _Message)
	{
		LinkedHashMap<String, Object> _return = new LinkedHashMap<String, Object>();
		try
		{
			LinkedHashMap<String, Object> _data = GlobalFunctions.ParseJsonToDictionary(_Message);
			List<String> _services = new ArrayList<String>();
			LinkedTreeMap<String, Object> _variables = new LinkedTreeMap<String, Object>();
			if(_data.containsKey("Services"))
				_services = (ArrayList<String>)_data.get("Services");
			if(_data.containsKey("PassedVars"))
			{
				Object _o = _data.get("PassedVars");
				_variables = (LinkedTreeMap<String, Object>)_o;
			}
			
			String _playerId = "";
			if(_variables.containsKey("PlayerId"))
				_playerId = _variables.get("PlayerId").toString();
			
			if(_variables.containsKey("PlayerId") == false && _services.contains("Login") == false)
				_return.put("Error", "MissingPlayerId");
			else
			{
				for(String service : _services)
				{
					System.out.print(service);
					switch(service)
					{
						case "Login":_return.put("Login",Login(_variables)); break;
						case "UpdateCoins":
							Integer _coins = (int)(Float.parseFloat(_variables.get("Coins").toString()));
							//_return = UpdateCoins(_playerId,_coins);
							_return.put("UpdateCoins",UpdateCoins(_playerId,_coins));
							break;
						case "UpdateEnergy":
							Integer _energy = (int)(Float.parseFloat(_variables.get("Energy").toString()));
							//_return = UpdateEnergy(_playerId,_energy);
							_return.put("UpdateEnergy",UpdateEnergy(_playerId,_energy));
							break;
						case "SearchGame":
							LinkedHashMap<String, Object> _o = GlobalFunctions.ParseJsonToDictionary(_variables.get("SearchDetails").toString());
							_o.put("StartSearchDate", GlobalFunctions.GetDateString());
							RedisFunctions.RedisSet(_playerId + "/SearchGame", GlobalFunctions.SerializeToJson(_o));	
							String _ws = "ws://63.35.227.147:8080/HiddenRooms/game/";	
							//String _ws = "ws://localhost:8080/HiddenRooms/game/";	
							_o.put("WS", _ws);
							_o.put("Action", "SearchGame");
							_return.put("SearchGame",_o);
							break;
					}
				}
			}
		}
		catch(Exception e)
		{_return.put("Error", e.getMessage());}
		

		String _response = GlobalFunctions.SerializeToJson(_return);
		return _response;
	}
	
	private LinkedHashMap<String, Object> Login(LinkedTreeMap<String, Object> _PassedVariables)
	{
		LinkedHashMap<String, Object> _ret = new LinkedHashMap<String, Object>();
		
		String _deviceId = "",_ip = "",_os = "",_platform = "",_playerId = "";
		if(_PassedVariables.containsKey("DeviceId"))
			_deviceId = _PassedVariables.get("DeviceId").toString();
		if(_PassedVariables.containsKey("Ip"))
			_ip = _PassedVariables.get("Ip").toString();
		if(_PassedVariables.containsKey("Os"))
			_os = _PassedVariables.get("Os").toString();
		if(_PassedVariables.containsKey("Platform"))
			_platform = _PassedVariables.get("Platform").toString();
		if(_PassedVariables.containsKey("PlayerId"))
			_playerId = _PassedVariables.get("PlayerId").toString();
		
		if(_playerId.equals("") == false)
		{	
			_ret = RedisApi.GetLoginDetails(_playerId);
			if(_ret.size() > 0)
			{
				if(_ret.containsKey("DeviceId") == false || (_ret.get("DeviceId").toString().equals(_deviceId) == false && _deviceId.equals("") == false))
				{
					_ret.put("DeviceId", _deviceId);
					RedisApi.SaveMapToRedis(_playerId, _ret);
				}
				_ret.put("DB", "Redis");
			}
		}
		
		if(_ret.size() <= 1)
		{
			String _name = GlobalFunctions.GetName();
			String _profilImg = GlobalFunctions.GetImg();
			String _country = "Israel";
			_ret = PlayerLogin(_deviceId, _country, _name, _profilImg);
			_playerId = _ret.get("PlayerId").toString();
			_ret.put("DB", "Sql");
			_ret.put("DeviceId", _deviceId);
			RedisApi.SaveMapToRedis(_playerId, _ret);
		}
		_ret.put("Version", GetVersion(_platform));
		return _ret;
	}
	
	private LinkedHashMap<String,Object> PlayerLogin(String _DeviceId,String _Country,String _Name,String _ProfileImg)
	{
		String _tempPlayerId = GlobalFunctions.GeneratePlayerId();
		DBGlobalVars _CoinsVar = GlobalVariables.getInstance().GetDBVars().get("StartCoins");
		LinkedHashMap<String,Object> _playerData = DataBaseApi.PlayerLogin(_tempPlayerId,_DeviceId, _Country, _Name, _ProfileImg,
				_CoinsVar.varData,GlobalVariables.getInstance().startEnergy.toString(),GlobalVariables.getInstance().maxEnergy.toString());
		_playerData.put("Action", "PlayerLogin");
		return _playerData;
	}

	private LinkedHashMap<String, Object> UpdateCoins(String _PlayerId,int _Coins)
	{
		LinkedHashMap<String,Object> _ret = DataBaseApi.UpdateCoins(_PlayerId, _Coins);
		_ret.put("Action", "UpdateCoins");
		if(_ret.containsKey("Coins"))
			RedisApi.SaveToRedis(_PlayerId, "Coins", _ret.get("Coins").toString());
		return _ret;
	}
	private LinkedHashMap<String,Object> UpdateEnergy(String _PlayerId,Integer _Energy)
	{
		LinkedHashMap<String,Object> _ret = DataBaseApi.UpdateEnergy(_PlayerId, _Energy);
		_ret.put("Action", "UpdateEnergy");
		if(_ret.containsKey("Energy"))
			RedisApi.SaveToRedis(_PlayerId, "Energy", _ret.get("Energy").toString());
		return _ret;
	}
	
	public String GetVersion(String _Platform)
	{
		LinkedHashMap<String, DBGlobalVars> _vars = GlobalVariables.getInstance().GetServerGlobalVariables();
		String _id =  "";
		switch(_Platform)
		{
			case "Android":_id =  "AndroidVersion";break;
			case "IPhonePlayer":_id =  "IosVersion";break;
			default:_id =  "GlobalVersion";break;
		}
		
		DBGlobalVars _data = _vars.get(_id);
		return _data.varVersion;
	}

}
