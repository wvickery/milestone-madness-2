﻿#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Networking;

public class CloudConnectorEditor
{
	// -- Complete the following fields. --
	private static string webServiceUrl = "";
	private static string spreadsheetId = ""; // If this is a fixed value could also be setup on the webservice to save POST request size.
	private static string servicePassword = "passcode";
	private static float timeOutLimit = 30f;
	private static bool usePOST = true;
	// --

	private static UnityWebRequest www;
	private static double elapsedTime = 0.0f;
	private static double startTime = 0.0f;
	
	public static void CreateRequest(Dictionary<string, string> form)
	{
		form.Add("ssid", spreadsheetId);
		form.Add("pass", servicePassword);
		
		EditorApplication.update += EditorUpdate;

		if (usePOST)
		{
			CloudConnectorCore.UpdateStatus("Establishing Connection at URL " + webServiceUrl);
			www = UnityWebRequest.Post(webServiceUrl, form);
		}
		else // Use GET.
		{
			string urlParams = "?";
			foreach (KeyValuePair<string, string> item in form)
			{
				urlParams += item.Key + "=" + item.Value + "&";
			}
			CloudConnectorCore.UpdateStatus("Establishing Connection at URL " + webServiceUrl + urlParams);
			www = UnityWebRequest.Get(webServiceUrl + urlParams);
		}
		
		startTime = EditorApplication.timeSinceStartup;
		www.Send();
	}

	static void EditorUpdate()
	{
		while (!www.isDone)
		{
			elapsedTime = EditorApplication.timeSinceStartup - startTime;
			if (elapsedTime >= timeOutLimit)
			{
				CloudConnectorCore.ProcessResponse("TIME_OUT", (float)elapsedTime);
				EditorApplication.update -= EditorUpdate;
			}
			return;
		}
		
		if (www.isNetworkError)
		{
			CloudConnectorCore.ProcessResponse(CloudConnectorCore.MSG_CONN_ERR + "Connection error after " + elapsedTime.ToString() + " seconds: " + www.error, (float)elapsedTime);
			return;
		}
		
		CloudConnectorCore.ProcessResponse(www.downloadHandler.text, (float)elapsedTime);
		
		EditorApplication.update -= EditorUpdate;
	}
}
#endif