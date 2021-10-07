using Java.Nio.Channels;
using Org.Java_websocket;
using Org.Java_websocket.Drafts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Java_websocket
{
    internal partial class IWebSocketServerFactoryInvoker
    {
        //public void Close()
        //{
        //    throw new NotImplementedException();
        //}

        //public WebSocketImpl CreateWebSocket(WebSocketAdapter p0, IList<Draft> p1)
        //{
        //    throw new NotImplementedException();
        //}

        //public WebSocketImpl CreateWebSocket(WebSocketAdapter p0, Draft p1)
        //{
        //    throw new NotImplementedException();
        //}

        IByteChannel IWebSocketServerFactory.WrapChannel(SocketChannel p0, SelectionKey p1)
        {
            throw new NotImplementedException();
        }

        IWebSocket IWebSocketFactory.CreateWebSocket(WebSocketAdapter p0, IList<Draft> p1)
        {
            throw new NotImplementedException();
        }

        IWebSocket IWebSocketFactory.CreateWebSocket(WebSocketAdapter p0, Draft p1)
        {
            throw new NotImplementedException();
        }
    }
}
