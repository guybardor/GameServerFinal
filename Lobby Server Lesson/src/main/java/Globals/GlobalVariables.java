package Globals;

import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.List;

import Globals.GlobalEnums.serverEnvironment;
import Logic.DBGlobalVars;
import Logic.DataBaseApi;

public class GlobalVariables 
{
	private static GlobalVariables instance = null;
	private LinkedHashMap<String,DBGlobalVars> dbVars;
	
	public LinkedHashMap<String,Integer> botsData;
	public List<String> rmvSearchList;

	public GlobalEnums.serverEnvironment curEnviroment = serverEnvironment.Production;
	public Integer maxGameTime = 180;
	public Integer matchId = 1;
	public Integer maxItemsToFind = 6;
	public Integer cancelMatchedPlayersTime = 10000;
	public Integer startEnergy = 30;
	public Integer maxEnergy = 100;
	
    public static GlobalVariables getInstance()
    {
	  if(instance == null) 
	     instance = new GlobalVariables();
	      
	  return instance;
    }
    
    public GlobalVariables()
	{
		rmvSearchList = new ArrayList<String>();
		
		dbVars = GetDBVars();
		InitBotsData();
	}
    
    private void InitBotsData()
    {
		if(dbVars.containsKey("Bots"))
		{
			DBGlobalVars _bots = dbVars.get("Bots");
			botsData =  new LinkedHashMap<String, Integer>();
			LinkedHashMap<String,Object> _botsData = GlobalFunctions.ParseJsonToDictionary(_bots.varData);
			for(String s : _botsData.keySet())
			{
				int _num = (int)Float.parseFloat(_botsData.get(s).toString());
				botsData.put(s, _num);
			}
		}
    }
    

	public LinkedHashMap<String, DBGlobalVars> GetDBVars()
	{
		LinkedHashMap<String, DBGlobalVars> _ret = new LinkedHashMap<String, DBGlobalVars>();
		
		LinkedHashMap<String, Object> _data = DataBaseApi.GetDbVariables();
		for(String s : _data.keySet())
		{
			LinkedHashMap<String, Object> _d = (LinkedHashMap<String, Object>)_data.get(s);
			String _id = _d.get("Id").toString();
			String _varName = _d.get("VarName").toString();
			String _varVersion = _d.get("VarVersion").toString();
			String _varData = _d.get("VarData").toString();
			DBGlobalVars _vars = new DBGlobalVars(_id,_varName, _varVersion,_varData);
			_ret.put(_varName, _vars);
		}
		return _ret;
	}
	
	public LinkedHashMap<String, DBGlobalVars> GetServerGlobalVariables()
	{
		return dbVars;
	}
	
	
}
