﻿using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;



namespace System.Net.Sockets.Kcp.Simple
{
    /// <summary>
    /// 简单例子
    /// </summary>
    public class SimpleKcpClient11 : IKcpCallback
    {
        UdpClient client;

        public SimpleKcpClient11(int port)
            : this(port, null)
        {

        }

        public SimpleKcpClient11(int port, IPEndPoint endPoint)
        {
            client = new UdpClient(port);
            kcp = new Kcp(2001, this);
            this.EndPoint = endPoint;
            BeginRecv();
        }

        public Kcp kcp { get; }
        public IPEndPoint EndPoint { get; set; }

        public void Output(IMemoryOwner<byte> buffer, int avalidLength)
        {
            var s = buffer.Memory.Span.Slice(0, avalidLength).ToArray();
            client.SendAsync(s, s.Length, EndPoint);
            client.SendAsync(buffer.Memory.Span.Slice(0, avalidLength).ToArray(), avalidLength);
            buffer.Dispose();

            Console.WriteLine(" Send avalidLength " + avalidLength);
        }

        public async void SendAsync(byte[] datagram, int bytes)
        {
            kcp.Send(datagram.AsSpan().Slice(0, bytes));
        }

        public async ValueTask<byte[]> ReceiveAsync()
        {
            var (buffer, avalidLength) = kcp.TryRecv();
            if (buffer == null)
            {
                await Task.Delay(10);
                return await ReceiveAsync();
            }
            else
            {
                var s = buffer.Memory.Span.Slice(0, avalidLength).ToArray();
                return s;
            }
        }

        private async void BeginRecv()
        {
            var res = await client.ReceiveAsync();
            EndPoint = res.RemoteEndPoint;
            kcp.Input(res.Buffer);
            BeginRecv();

            Console.WriteLine(" Recv  " + res.Buffer.Length);
        }
    }
}




