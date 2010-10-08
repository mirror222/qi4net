﻿using System;
using System.IO.Ports;
using System.Text;
using System.Threading;
using log4net;
using Qi.Sms.Protocol;

namespace Qi.Sms.DeviceConnections
{
    public sealed class ComConnection : IDeviceConnection
    {
        private readonly object _lockItem = "";
        private readonly ILog _log = LogManager.GetLogger(typeof(ComConnection));
        private readonly SerialPort _serialPort;
        private bool _hasReturnValue;
        private StringBuilder _returnValue;

        public ComConnection(string portName, int baudRate)
        {
            if (portName == null)
                throw new ArgumentNullException("portName");
            portName = portName;
            BaudRate = baudRate;
            _serialPort = new SerialPort(portName, BaudRate, Parity.None, 8, StopBits.One)
                              {
                                  DtrEnable = true,
                                  RtsEnable = true,
                                  Handshake = Handshake.None
                              };
            _serialPort.DataReceived += SerialPortDataReceived;
        }

        public string PortName { get; private set; }

        public int BaudRate { get; private set; }

        #region IDeviceConnection Members

        public event EventHandler<DeviceCommandEventHandlerArgs> SendingEvent;
        public event EventHandler<DeviceCommandEventHandlerArgs> ReceivedEvent;

        public void Dispose()
        {
            if (_serialPort != null)
            {
                _serialPort.Close();
                _serialPort.Dispose();
            }
        }

        public void Open()
        {
            try
            {
                _serialPort.Open();
                IsConnected = true;
            }
            catch (Exception ex)
            {
                throw new DeviceConnectionException(ex.Message, ex);
            }
        }

        public void Close()
        {
            try
            {
                _serialPort.Close();
                IsConnected = false;
            }
            catch (Exception ex)
            {
                throw new DeviceConnectionException(ex.Message, ex);
            }
        }


        public bool IsConnected { get; private set; }
        public void Send(string command)
        {
            
            _log.Debug("Send:" + command);
            _serialPort.WriteLine(command + "\r\n");
        }
        public string Send(AbstractCommand command)
        {
            
            if (SendingEvent != null)
                SendingEvent(this, new DeviceCommandEventHandlerArgs(command.CompleteCommand()));
            _log.InfoFormat("send command {0}\r\n", command);
            lock (_lockItem)
            {
                ThreadPool.QueueUserWorkItem(delegate { _serialPort.WriteLine(command + "\r\n"); });

                _returnValue = new StringBuilder();
                DateTime now = DateTime.Now;


                while (true)
                {
                    if (_hasReturnValue)
                    {
                        _log.Debug("Receive data:" + _returnValue);
                        if (command.Init(_returnValue.ToString()))
                        {
                            _log.Debug("Data is correct for command =" + command.GetType().Name);
                            break;
                        }
                        _log.Debug("Receive data isn't expected, so continue to wait");
                        _returnValue = new StringBuilder();
                        _hasReturnValue = false;
                    }
                    Thread.Sleep(100);
                    TimeSpan span = DateTime.Now - now;
                    if (span.Seconds > 5)
                    {
                        _log.InfoFormat("Send command timeout,and exit");
                        return ""; //throw new TimeoutException("Timeout {0} send fail.");
                    }
                }

                string result = _returnValue.ToString();
                _hasReturnValue = false;
                _returnValue = null;
                _log.InfoFormat("receive command \r\n {0}", result);
                return result;
            }
        }

        #endregion

        private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var serialPort = (SerialPort)sender;
            var result = new StringBuilder();
            while (serialPort.BytesToRead > 0)
            {
                string re = serialPort.ReadExisting();
                result.Append(re);
                if (_returnValue != null)
                {
                    _returnValue.Append(re);
                }
                Thread.Sleep(100);
            }
            if (_returnValue != null)
            {
                _hasReturnValue = true;
            }
            OnReceiveEvent(result.ToString());
        }

        public void OnReceiveEvent(string command)
        {
            _log.Debug("Receive:" + command);
            if (ReceivedEvent != null)
            {
                ThreadPool.QueueUserWorkItem(e => ReceivedEvent(this, new DeviceCommandEventHandlerArgs(command)));
            }
        }
    }
}