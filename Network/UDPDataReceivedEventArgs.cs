using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skyfire.Network
{
    /// <summary>
    /// 接收到网络数据时的事件参数。参数中能够保存的数据内容长度为1024个字节
    /// @author mmCAtE
    /// @version 0.1
    /// @date 2015-03-06
    /// </summary>
    public class NetworkDataReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// 接收到的数据长度
        /// @since 0.1
        /// </summary>
        public int ReceivedLength { get; set; }
        /// <summary>
        /// 接收到的数据内容
        /// @since 0.1
        /// </summary>
        public byte[] ReceivedData { get; set; }

        /// <summary>
        /// 构造方法
        /// @since 0.1
        /// </summary>
        public NetworkDataReceivedEventArgs()
        {
            // 默认数据内容长度为1024个字节
            ReceivedData = new byte[1024];
        }
    }
}
