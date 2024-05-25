using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Hgs.Core.RemoteUi;

public class RemoteUiServer {
  private HttpListener listener = new HttpListener();

  public static RemoteUiServer Instance {get; private set;} = null;

  public ConcurrentQueue<BaseRequest> Requests {get; private set; } = new ConcurrentQueue<BaseRequest>();

  private Dictionary<string, Func<HttpListenerContext, BaseRequest>> handlers = new Dictionary<string, Func<HttpListenerContext, BaseRequest>>();

  private RemoteUiServer() {
    listener.Prefixes.Add("http://localhost:3456/");
    listener.Start();
    _ = Task.Run(Run);
  }

  public static void Initialize() {
    Instance = new RemoteUiServer();
  }

  public static void RegisterHandler(string path, Func<HttpListenerContext, BaseRequest> handler) {
    Instance.handlers[path] = handler;
  }

  private void Run() {
    while (true) {
      var context = listener.GetContext();
      _ = Task.Run(() => Accept(context));
    }
  }

  private void Accept(HttpListenerContext context) {
    var req = context.Request;
    // Handle CORS requests.
    if (req.HttpMethod == "OPTIONS") {
      var res = context.Response;
      res.AddHeader("Access-Control-Allow-Origin", "*");
      res.AddHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS, DELETE");
      res.StatusCode = 204;
      res.Close();
      return;
    }
    
    // Handle unknown requests.
    if (!handlers.ContainsKey(req.Url.PathAndQuery)) {
      context.Response.StatusCode = 404;
      context.Response.Close();
      return;
    }

    Requests.Enqueue(handlers[req.Url.PathAndQuery](context));
  }
}