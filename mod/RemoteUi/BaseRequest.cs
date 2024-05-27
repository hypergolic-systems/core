using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EdyCommonTools;

namespace Hgs.RemoteUi;

public abstract class BaseRequest {

  private HttpListenerContext context;
  public BaseRequest(HttpListenerContext context) {
    this.context = context;
  }

  public void Run() {
    var data = Handle();
    _ = Task.Run(async () => {
      var res = context.Response;
      res.ContentType = "application/json";
      res.AddHeader("Access-Control-Allow-Origin", "*");
      try {
        var obj = Handle();
        var data = MiniJSON.jsonEncode(obj);
        var buffer = Encoding.UTF8.GetBytes(data);
        res.ContentLength64 = buffer.Length;
        await res.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        res.Close();
      } catch (Exception e) {
        Util.Log(e.ToString());
        res.StatusCode = 500;
        res.Close();
      }
    });
  }

  protected abstract object Handle();
}
