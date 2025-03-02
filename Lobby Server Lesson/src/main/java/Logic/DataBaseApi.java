package Logic;
import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.util.Date;
import java.util.LinkedHashMap;

public class DataBaseApi 
{
	static String DBConnectionString = "jdbc:sqlserver://armetisdb.cil5cqlcozam.eu-west-1.rds.amazonaws.com:1433;databaseName=HiddenObjects;user=mindblast6;password=qweR1234";
	
	public static LinkedHashMap<String,Object> PlayerLogin(String _PlayerId,String _DeviceId,String _Country,String _NickName,String _ProfileImg,String _CoinsAmount,String _Energy,String _MaxEnergy)
	{
		LinkedHashMap<String,Object> _result = new LinkedHashMap<String,Object>();
		Connection _connection = null;  // For making the connection
		Statement _statement = null;    // For the SQL statement
		ResultSet _resultSet = null;    // For the result set, if applicable
		
		try
		{
		    Class.forName("com.microsoft.sqlserver.jdbc.SQLServerDriver");
		    _connection = DriverManager.getConnection(DBConnectionString);

		     String _sqlString = "EXEC CreatePlayer '" + _PlayerId + "','"+ _DeviceId + "','"+ _Country + "','"+ _NickName + "','"+ _ProfileImg + "'," +
		     _CoinsAmount + "," + _Energy + "," + _MaxEnergy;
		    
		    _statement = _connection.createStatement();
		    _resultSet = _statement.executeQuery(_sqlString);
		    while (_resultSet.next())
		    {
		    	_result.put("PlayerId", _resultSet.getString("PlayerId").toString());
		    	//_result.put("DeviceId", _resultSet.getString("DeviceId").toString());
		    	_result.put("Country", _resultSet.getString("Country").toString());
		    	_result.put("NickName", _resultSet.getString("NickName").toString());
		    	_result.put("ProfileImg", _resultSet.getString("ProfileImg").toString());
		    	_result.put("Coins", _resultSet.getString("Coins").toString());
		    	_result.put("Xp", _resultSet.getString("Xp").toString());
		    	_result.put("XpLevel", _resultSet.getString("XpLevel").toString());
		    	_result.put("Crm", _resultSet.getString("Crm").toString());
		    	_result.put("CrmLevel", _resultSet.getString("CrmLevel").toString());
		    	_result.put("Energy", _resultSet.getString("Energy").toString());
		    	_result.put("MaxEnergy", _resultSet.getString("MaxEnergy").toString());
		    	_result.put("LastEnergyCollectionDate", _resultSet.getString("LastEnergyCollectionDate").toString());
		    	_result.put("LastDailyBonusDate", _resultSet.getString("LastDailyBonusDate").toString());
		    } 
		}
		catch (ClassNotFoundException cnfe)  { }
		catch (SQLException se) {}
        catch (Exception e){}
        finally
        {
            try
            {
                // Close resources.
                if (null != _connection) _connection.close();
                if (null != _statement) _statement.close();
                if (null != _resultSet) _resultSet.close();
            }
            catch (SQLException sqlException){}
        }
		
		return _result;
	}

	public static LinkedHashMap<String,Object> GetDbVariables()
	{
		LinkedHashMap<String,Object> _result = new LinkedHashMap<String,Object>();
		LinkedHashMap<String,Object> _data = new LinkedHashMap<String,Object>();
		Connection _connection = null;  // For making the connection
		Statement _statement = null;    // For the SQL statement
		ResultSet _resultSet = null;    // For the result set, if applicable
		
		try
		{
		    Class.forName("com.microsoft.sqlserver.jdbc.SQLServerDriver");
		    _connection = DriverManager.getConnection(DBConnectionString);

		     String _sqlString = "Select * FROM GlobalVariables";
		    
		    _statement = _connection.createStatement();
		    _resultSet = _statement.executeQuery(_sqlString);
		    while (_resultSet.next())
		    {
		    	_data = new LinkedHashMap<String, Object>();
		    	_data.put("Id", _resultSet.getString("Id").toString());
		    	_data.put("VarName", _resultSet.getString("VarName").toString());
		    	_data.put("VarVersion", _resultSet.getString("VarVersion").toString());
		    	_data.put("VarData", _resultSet.getString("VarData").toString());
		    	_result.put(_resultSet.getString("VarName").toString(), _data);
		    } 
		}
		catch (ClassNotFoundException cnfe)  { }
		catch (SQLException se) {}
        catch (Exception e){}
        finally
        {
            try
            {
                // Close resources.
                if (null != _connection) _connection.close();
                if (null != _statement) _statement.close();
                if (null != _resultSet) _resultSet.close();
            }
            catch (SQLException sqlException){}
        }
		
		return _result;
	}

	public static LinkedHashMap<String,Object> CreateMatch(String _FirstPlayerId,String _SecondPlayerId,Integer _Bet,Integer _Energy,String _Log)
	{
		LinkedHashMap<String,Object> _result = new LinkedHashMap<String,Object>();
		Connection _connection = null;  // For making the connection
		Statement _statement = null;    // For the SQL statement
		ResultSet _resultSet = null;    // For the result set, if applicable
		
		try
		{
		    Class.forName("com.microsoft.sqlserver.jdbc.SQLServerDriver");
		    _connection = DriverManager.getConnection(DBConnectionString);

		     String _sqlString = "EXEC CreateMatch '" + _FirstPlayerId + "','"+ _SecondPlayerId + "',"+ _Bet + ","+ _Energy + ",'"+ _Log + "'";
		    
		    _statement = _connection.createStatement();
		    _resultSet = _statement.executeQuery(_sqlString);
		    while (_resultSet.next())
		    {
		    	_result.put("Id", _resultSet.getString("Id").toString());
		    	_result.put("EnergyP1", _resultSet.getString("EnergyP1").toString());
		    	_result.put("Player1", _resultSet.getString("Player1").toString());
		    	_result.put("EnergyP2", _resultSet.getString("EnergyP2").toString());
		    	_result.put("Player2", _resultSet.getString("Player2").toString());
		    } 
		}
		catch (ClassNotFoundException cnfe)  { }
		catch (SQLException se) {}
        catch (Exception e){}
        finally
        {
            try
            {
                // Close resources.
                if (null != _connection) _connection.close();
                if (null != _statement) _statement.close();
                if (null != _resultSet) _resultSet.close();
            }
            catch (SQLException sqlException){}
        }
		
		return _result;
	}
	
	public static LinkedHashMap<String,Object> FinishMatch(Integer MatchId,String _Winner,Integer _Bet,String _Log)
	{
		LinkedHashMap<String,Object> _result = new LinkedHashMap<String,Object>();
		Connection _connection = null;  // For making the connection
		Statement _statement = null;    // For the SQL statement
		ResultSet _resultSet = null;    // For the result set, if applicable
		
		try
		{
		    Class.forName("com.microsoft.sqlserver.jdbc.SQLServerDriver");
		    _connection = DriverManager.getConnection(DBConnectionString);

		     String _sqlString = "EXEC FinishMatch " + MatchId + ",'"+ _Winner + "',"+ _Bet + ",'"+ _Log + "'";
		    
		    _statement = _connection.createStatement();
		    _resultSet = _statement.executeQuery(_sqlString);
		    while (_resultSet.next())
		    {
		    	_result.put("Coins", _resultSet.getString("Coins").toString());
		    } 
		}
		catch (ClassNotFoundException cnfe)  { }
		catch (SQLException se) {}
        catch (Exception e){}
        finally
        {
            try
            {
                // Close resources.
                if (null != _connection) _connection.close();
                if (null != _statement) _statement.close();
                if (null != _resultSet) _resultSet.close();
            }
            catch (SQLException sqlException){}
        }
		
		return _result;
	}

	public static LinkedHashMap<String,Object> GetMatchInfo(Integer  _MatchId)
	{
		LinkedHashMap<String,Object> _result = new LinkedHashMap<String,Object>();
		Connection _connection = null;  // For making the connection
		Statement _statement = null;    // For the SQL statement
		ResultSet _resultSet = null;    // For the result set, if applicable
		
		try
		{
		    Class.forName("com.microsoft.sqlserver.jdbc.SQLServerDriver");
		    _connection = DriverManager.getConnection(DBConnectionString);

		     String _sqlString = "SELECT * FROM Matches WHERE id = " + _MatchId;
		    
		    _statement = _connection.createStatement();
		    _resultSet = _statement.executeQuery(_sqlString);
		    while (_resultSet.next())
		    {
		    	_result.put("MatchId", _resultSet.getString("Id").toString());
		    	_result.put("FirstPlayerId", _resultSet.getString("FirstPlayerId").toString());
		    	_result.put("SecondPlayerId", _resultSet.getString("SecondPlayerId").toString());
		    }
		}
		catch (ClassNotFoundException cnfe)  { }
		catch (SQLException se) {}
        catch (Exception e){}
        finally
        {
            try
            {
                // Close resources.
                if (null != _connection) _connection.close();
                if (null != _statement) _statement.close();
                if (null != _resultSet) _resultSet.close();
            }
            catch (SQLException sqlException){}
        }
		
		return _result;
	}

	public static LinkedHashMap<String,Object> UpdateCoins(String _PlayerId,Integer _Coins)
	{
		LinkedHashMap<String,Object> _result = new LinkedHashMap<String,Object>();
		Connection _connection = null;  // For making the connection
		Statement _statement = null;    // For the SQL statement
		ResultSet _resultSet = null;    // For the result set, if applicable
		
		try
		{
		    Class.forName("com.microsoft.sqlserver.jdbc.SQLServerDriver");
		    _connection = DriverManager.getConnection(DBConnectionString);

		     String _sqlString = "EXEC UpdateCoins '" + _PlayerId + "',"+ _Coins;
		    _statement = _connection.createStatement();
		    _resultSet = _statement.executeQuery(_sqlString);
		    while (_resultSet.next())
		    {
		    	_result.put("Coins", _resultSet.getString("Coins").toString());
		    } 
		}
		catch (ClassNotFoundException cnfe)  { }
		catch (SQLException se) {}
        catch (Exception e){}
        finally
        {
            try
            {
                // Close resources.
                if (null != _connection) _connection.close();
                if (null != _statement) _statement.close();
                if (null != _resultSet) _resultSet.close();
            }
            catch (SQLException sqlException){}
        }
		
		return _result;
	}
	public static LinkedHashMap<String,Object> UpdateEnergy(String _PlayerId,Integer _Energy)
	{
		LinkedHashMap<String,Object> _result = new LinkedHashMap<String,Object>();
		Connection _connection = null;  // For making the connection
		Statement _statement = null;    // For the SQL statement
		ResultSet _resultSet = null;    // For the result set, if applicable
		
		try
		{
		    Class.forName("com.microsoft.sqlserver.jdbc.SQLServerDriver");
		    _connection = DriverManager.getConnection(DBConnectionString);

		     String _sqlString = "EXEC UpdateEnergy '" + _PlayerId + "',"+ _Energy;
		    _statement = _connection.createStatement();
		    _resultSet = _statement.executeQuery(_sqlString);
		    while (_resultSet.next())
		    {
		    	_result.put("Energy", _resultSet.getString("Energy").toString());
		    } 
		}
		catch (ClassNotFoundException cnfe)  { }
		catch (SQLException se) {}
        catch (Exception e){}
        finally
        {
            try
            {
                // Close resources.
                if (null != _connection) _connection.close();
                if (null != _statement) _statement.close();
                if (null != _resultSet) _resultSet.close();
            }
            catch (SQLException sqlException){}
        }
		
		return _result;
	}
	
	public static void UpdateLastEnergyCollectionDate(String _PlayerId,java.sql.Timestamp _NewDate)
	{
		LinkedHashMap<String,Object> _result = new LinkedHashMap<String,Object>();
		Connection _connection = null;  // For making the connection
		Statement _statement = null;    // For the SQL statement
		
		try
		{
		    Class.forName("com.microsoft.sqlserver.jdbc.SQLServerDriver");
		    _connection = DriverManager.getConnection(DBConnectionString);

		     String _sqlString = "EXEC UpdateLastEnergyCollectionDate '" + _PlayerId + "','"+ _NewDate + "'";
		    _statement = _connection.createStatement();
		    _statement.executeUpdate(_sqlString);
		}
		catch (ClassNotFoundException cnfe)  { }
		catch (SQLException se) {}
        catch (Exception e){}
        finally
        {
            try
            {
                // Close resources.
                if (null != _connection) _connection.close();
                if (null != _statement) _statement.close();
            }
            catch (SQLException sqlException){}
        }
	}
	
}
