using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using M2MqttUnity;
using Newtonsoft.Json.Linq;
using System.Linq;
using Newtonsoft.Json;

namespace ChuongBo
{
    public class ChuongBoMqtt : M2MqttUnityClient
    {
        public List<string> topics = new List<string>();

        public InputField uriInputField;
        public InputField userInputField;
        public InputField passwordInputField;

        public class Status_Data
        {
            public int temp { get; set; }
            public int humi { get; set; }
        }

        public class Control_Data
        {
            public string device { get; set; }
            public string status { get; set; }
        }

        public string msg_received_from_topic_status = "";
        public string msg_received_from_topic_control = "";

        private List<string> eventMessages = new List<string>();
        [SerializeField]
        public Status_Data _status_data;
        [SerializeField]
        public Control_Data _control_data;

        public void UpdateBeforeConnect()
        {
            this.brokerAddress = uriInputField.text;
            this.brokerPort = 1883;
            this.mqttUserName = userInputField.text;
            this.mqttPassword = passwordInputField.text;
            this.Connect();
        }

        public void SetEncrypted(bool isEncrypted)
        {
            this.isEncrypted = isEncrypted;
        }

        protected override void OnConnecting()
        {
            try
            {
                base.OnConnecting();
            }
            catch { }
        }

        protected override void OnConnected()
        {
            GetComponent<ChuongBoManager>().swtichLayout2();
            base.OnConnected();
            SubscribeTopics();
        }

        protected override void SubscribeTopics()
        {

            foreach (string topic in topics)
            {
                if (topic != "")
                {
                    client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
                }
            }
        }

        protected override void UnsubscribeTopics()
        {
            foreach (string topic in topics)
            {
                if (topic != "")
                {
                    client.Unsubscribe(new string[] { topic });
                }
            }
        }
        protected override void OnConnectionFailed(string errorMessage)
        {
            Debug.Log("CONNECTION FAILED! " + errorMessage);
            Disconnect();
        }

        protected override void OnDisconnected()
        {
            Debug.Log("Disconnected.");
        }

        protected override void OnConnectionLost()
        {
            Debug.Log("CONNECTION LOST!");
            Disconnect();
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void DecodeMessage(string topic, byte[] message)
        {
            string msg = System.Text.Encoding.UTF8.GetString(message);
            Debug.Log("Received: " + msg);
            if (topic == topics[0])
                ProcessMessageStatus(msg);
            else
                ProcessMessageControl(msg);
        }

        private void ProcessMessageStatus(string msg)
        {
            _status_data = JsonConvert.DeserializeObject<Status_Data>(msg);
            msg_received_from_topic_status = msg;
            GetComponent<ChuongBoManager>().Update_Status(_status_data);
        }

        private void ProcessMessageControl(string msg)
        {
            _control_data = JsonConvert.DeserializeObject<Control_Data>(msg);
            msg_received_from_topic_control = msg;
            GetComponent<ChuongBoManager>().Update_Control(_control_data);
        }

        public void UpdateLedStatus(bool status)
        {
            Control_Data control_Data = new Control_Data() { device = "LED", status = status ? "ON" : "OFF" };
            var temp = JsonConvert.SerializeObject(control_Data);
            client.Publish("/bkiot/1912602/led", System.Text.Encoding.UTF8.GetBytes(temp), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
        }

        public void UpdatePumpStatus(bool status)
        {
            Control_Data control_Data = new Control_Data() { device = "PUMP", status = status ? "ON" : "OFF" };
            var temp = JsonConvert.SerializeObject(control_Data);
            client.Publish("/bkiot/1912602/pump", System.Text.Encoding.UTF8.GetBytes(temp), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
        }

        public void OnDestroy()
        {
            Disconnect();
            GetComponent<ChuongBoManager>().swtichLayout1();
        }
    }
}