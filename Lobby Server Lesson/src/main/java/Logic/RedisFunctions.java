package Logic;
import java.io.IOException;
import java.net.InetSocketAddress;

import Globals.GlobalEnums;
import Globals.GlobalVariables;
import net.spy.memcached.MemcachedClient;
import redis.clients.jedis.Jedis;
import redis.clients.jedis.JedisShardInfo;

public class RedisFunctions 
{
	private static Boolean isInit = false;
	public static String cacheHostname = "redis-hidden.2yxfjo.ng.0001.euw1.cache.amazonaws.com";
	private static String localHostname = "localhost";
	private static int port = 6379;
	private static String cachekey = "comfy135";
	private static JedisShardInfo shardInfo;
	
	public static void RedisInit()
	{
		String _enviroment = localHostname;
		switch(GlobalVariables.getInstance().curEnviroment)
		{
			case Local:_enviroment = localHostname;break;
			case Development:_enviroment = cacheHostname;break;
			case Production:_enviroment = cacheHostname;break;
		}
		
        shardInfo = new JedisShardInfo(_enviroment, port, false);
        shardInfo.setConnectionTimeout(10000);
       // shardInfo.setPassword(cachekey); /* Use your access key. */
	}
	
	public static String RedisPing()
	{
	    String _data = "";
		try
		{
		if(isInit == false)
		{
			RedisInit();
			isInit = true;
		}
	    Jedis _jedis = new Jedis(shardInfo);      
	    _data = _jedis.ping();	
	    _jedis.close();
	    if(_data == null)
	    	return "";
		}
		catch(Exception e)
		{
			System.out.println(e.getMessage());
		}
	    return _data;
	}
	
	public static String RedisSet(String _Key,String _Value)
	{
		try
		{
			if(isInit == false)
			{
				RedisInit();
				isInit = true;
			}
		    Jedis _jedis = new Jedis(shardInfo);      
		    String _data = _jedis.set(_Key, _Value);	
		    _jedis.close();
		    if(_data == null)
		    	return "";
		    return _data;
		}
		catch(Exception e)
		{
			System.out.println(e.getMessage());
		}
		
		return "";
	}
	
	public static String RedisGet(String _Key)
	{
		if(isInit == false)
		{
			RedisInit();
			isInit = true;
		}
	    Jedis _jedis = new Jedis(shardInfo);      
	    String _data = _jedis.get(_Key);	
	    _jedis.close();
	    if(_data == null)
	    	return "";
	    return _data;
	}
	
	public static Long RedisDelete(String _Key)
	{
		if(isInit == false)
		{
			RedisInit();
			isInit = true;
		}
	    Jedis _jedis = new Jedis(shardInfo);      
	    Long _data = _jedis.del(_Key);	
	    _jedis.close();
	    return _data;
	}
	
	public static String RedisGetClientList()
	{
		if(isInit == false)
		{
			isInit = true;
			RedisInit();
			
		}
	    Jedis _jedis = new Jedis(shardInfo);      
	    String _data = _jedis.clientList();	
	    _jedis.close();
	    if(_data == null)
	    	return "";
	    return _data;
	}
}
