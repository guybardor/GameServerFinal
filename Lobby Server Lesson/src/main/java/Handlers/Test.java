package Handlers;

import java.io.IOException;
import java.util.LinkedHashMap;

import javax.servlet.ServletException;
import javax.servlet.annotation.WebServlet;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import Globals.GlobalFunctions;
import Logic.RedisFunctions;

/**
 * Servlet implementation class Test
 */
@WebServlet("/Test")
public class Test extends HttpServlet {
	private static final long serialVersionUID = 1L;
       
    /**
     * @see HttpServlet#HttpServlet()
     */
    public Test() {
        super();
        // TODO Auto-generated constructor stub
    }

	/**
	 * @see HttpServlet#doGet(HttpServletRequest request, HttpServletResponse response)
	 */
	protected void doGet(HttpServletRequest request, HttpServletResponse response) throws ServletException, IOException {
		// TODO Auto-generated method stub
		String _print = "";
		try
		{
			RedisFunctions.RedisSet("Test", GlobalFunctions.GetDateString());
			_print = RedisFunctions.RedisGet("Test") + "\r\n ";
			_print += RedisFunctions.RedisGet("Keys *") + "\r\n ";
			_print += RedisFunctions.RedisPing() + "\r\n ";
			_print += RedisFunctions.cacheHostname;
			
			
		}
		catch(Exception e) {_print = e.getMessage();}
		response.getWriter().append("Served at: ").append(_print);
		//GlobalFunctions.GeneratePlayerId();
	}

	/**
	 * @see HttpServlet#doPost(HttpServletRequest request, HttpServletResponse response)
	 */
	protected void doPost(HttpServletRequest request, HttpServletResponse response) throws ServletException, IOException {
		// TODO Auto-generated method stub
		doGet(request, response);
	}

}
