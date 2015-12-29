using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Owin;
using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

namespace aosrepo.Middleware {
    public class DebugMiddleware {
        readonly AppFunc _next;
        readonly DebugMiddlewareOptions _options;

        public DebugMiddleware(AppFunc next, DebugMiddlewareOptions options) {
            _next = next;
            _options = options;

            if (_options.OnIncomingRequest == null)
                _options.OnIncomingRequest = ctx => Debug.WriteLine("Incoming request: " + ctx.Request.Path);

            if (_options.OnOutGoingRequest == null)
                _options.OnOutGoingRequest = ctx => Debug.WriteLine("Outgoing request: " + ctx.Request.Path);
        }

        public async Task Invoke(IDictionary<string, object> environment) {
            var ctx = new OwinContext(environment);
            _options.OnIncomingRequest(ctx);
            await _next(environment);
            _options.OnOutGoingRequest(ctx);
        }
    }
}