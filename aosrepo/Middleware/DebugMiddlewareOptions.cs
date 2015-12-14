using System;
using Microsoft.Owin;

namespace aosrepo.Middleware {
    public class DebugMiddlewareOptions {
        public Action<IOwinContext> OnIncomingRequest { get; set; }
        public Action<IOwinContext> OnOutGoingRequest { get; set; }
    }
}