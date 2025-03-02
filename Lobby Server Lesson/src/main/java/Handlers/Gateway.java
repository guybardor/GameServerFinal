package Handlers;

import java.io.IOException;
import java.io.PrintWriter;
import java.util.LinkedHashMap;

import javax.servlet.ServletException;
import javax.servlet.annotation.WebServlet;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import Globals.GlobalFunctions;
import Logic.Logic;


/**
 * Servlet implementation class Gateway
 */
@WebServlet("/Gateway")
public class Gateway extends HttpServlet {
	private static final long serialVersionUID = 1L;
       
    /**
     * @see HttpServlet#HttpServlet()
     */
    public Gateway() {
        super();
        // TODO Auto-generated constructor stub
    }

	/**
	 * @see HttpServlet#doGet(HttpServletRequest request, HttpServletResponse response)
	 */
	protected void doGet(HttpServletRequest request, HttpServletResponse response) throws ServletException, IOException 
	{
		String _return = "";
		try
		{
			String _message = request.getParameter("JSON");
			_return = Logic.getInstance().DoLogic(_message);
		}
		catch(Exception e){}
		PrintWriter out = response.getWriter();
		out.println(_return);
	}

	/**
	 * @see HttpServlet#doPost(HttpServletRequest request, HttpServletResponse response)
	 */
	protected void doPost(HttpServletRequest request, HttpServletResponse response) throws ServletException, IOException {
		// TODO Auto-generated method stub
		doGet(request, response);
	}

}
