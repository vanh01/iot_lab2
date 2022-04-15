using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace ChuongBo
{
    public class ChuongBoManager : MonoBehaviour
    {
        [SerializeField]
        private Toggle led;
        [SerializeField]
        private Toggle pump;
        [SerializeField]
        private Text temperature;
        [SerializeField]
        private Text humidity;
        [SerializeField]
        private GameObject layout1;
        [SerializeField]
        private GameObject layout2;
        [SerializeField]
        private Text time;

        private bool ledStatus = true;
        private bool pumpStatus = true;

        public void Update_Status(ChuongBoMqtt.Status_Data _status)
        {
            temperature.text = _status.temp.ToString() + "°C";
            humidity.text = _status.humi.ToString() + "%";
        }

        public void Update_Control(ChuongBoMqtt.Control_Data _control_data)
        {
            if (_control_data.device == "LED")
            {
                ledStatus = _control_data.status == "ON" ? true : false;
                led.isOn = ledStatus;
            }
            if (_control_data.device == "PUMP")
            {
                pumpStatus = _control_data.status == "ON" ? true : false;
                pump.isOn = pumpStatus;
            }
        }

        void Start()
        {
            layout2.SetActive(false);
            layout1.SetActive(true);
        }

        void Update()
        {
            if (led.isOn != ledStatus)
            {
                GetComponent<ChuongBoMqtt>().UpdateLedStatus(led.isOn);
                ledStatus = led.isOn;
            }
            if (pump.isOn != pumpStatus)
            {
                GetComponent<ChuongBoMqtt>().UpdatePumpStatus(pump.isOn);
                pumpStatus = pump.isOn;
            }

            time.text = "Time: " + DateTime.Now.ToString("HH:mm:ss dd-MM-yyyy");
        }

        public void Connect()
        {
            GetComponent<ChuongBoMqtt>().UpdateBeforeConnect();
        }

        public void Disconnect()
        {
            GetComponent<ChuongBoMqtt>().OnDestroy();
        }

        public void swtichLayout2()
        {
            layout2.SetActive(true);
            layout1.SetActive(false);
        }

        public void swtichLayout1()
        {
            layout2.SetActive(false);
            layout1.SetActive(true);
        }

    }
}