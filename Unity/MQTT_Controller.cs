using M2MqttUnity;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

public class MQTT_Controller : M2MqttUnityClient
{
    private List<Message> eventMessages = new List<Message>();

    public GameObject buttonTarget;
    public bool buttonTargetFoundATM = false;
    public PressableButton currentlySelectedButton = null;

    public GameObject sliderTarget;
    public bool sliderTargetFoundATM = false;
    public PinchSlider currentlySelectedSlider = null;

    public GameObject drehknopfTarget;
    public bool drehknopfTargetFoundATM = false;
    public RotaryKnob currentlySelectedDrehknopf = null;

    private bool subscribed = false;

    private static MQTT_Controller mMQTTController;


    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if(client != null && client.IsConnected && subscribed == false)
        {
            client.Subscribe(new string[] { "esp/Button" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            client.Subscribe(new string[] { "esp/Slider" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            client.Subscribe(new string[] { "esp/Drehknopf" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            subscribed = true;
        }
        base.Update();

        if(eventMessages.Count > 0)
        {
            foreach(Message msg in eventMessages)
            {
                ProcessMessage(msg);
            }
            eventMessages.Clear();
        }
    }

    protected override void OnConnecting()
    {
        base.OnConnecting();
        Debug.Log("Connecting to broker on " + brokerAddress + ":" + brokerPort.ToString() + "...\n");
    }

    protected override void OnConnectionFailed(string errorMessage)
    {
        //base.OnConnectionFailed(errorMessage);
        Debug.Log("Connection failed");
    }

    protected override void DecodeMessage(string topic, byte[] message)
    {
        string msg = System.Text.Encoding.UTF8.GetString(message);
        Debug.Log("Received: " + msg);
        StoreMessage(msg, topic);
    }

    private void StoreMessage(string eventMsg, string topic)
    {
        eventMessages.Add(new Message(eventMsg, topic));
    }

    private void ProcessMessage(Message msg)
    {
        if (msg.topic.Equals("esp/Button") && currentlySelectedButton != null && drehknopfTargetFoundATM == false && sliderTargetFoundATM == false)
        {
            if (msg.messageText.Equals("pressed") )
                currentlySelectedButton.ButtonPressed.Invoke();
            else if (msg.messageText.Equals("released"))
                currentlySelectedButton.ButtonReleased.Invoke();

        }
        else if (msg.topic.Equals("esp/Slider") && currentlySelectedSlider != null && drehknopfTargetFoundATM == false && buttonTargetFoundATM == false)
        {
            float value = float.Parse(msg.messageText);
            //value = value /100;
            currentlySelectedSlider.SliderValue = value;
            //currentlySelectedSlider.OnValueUpdated.Invoke(); //Muss es �ber die Events gehen oder reicht Slidervalue setzen?
            currentlySelectedSlider.OnInteractionEnded.Invoke(new SliderEventData(0,value,null, currentlySelectedSlider)); //das Event wird anscheinend sonst nicht getriggert
        }
        else if (msg.topic.Equals("esp/Drehknopf") && currentlySelectedDrehknopf != null && buttonTargetFoundATM == false && sliderTargetFoundATM == false)
        {
            float value = float.Parse(msg.messageText);
            //value = value /100;
            currentlySelectedDrehknopf.setValue(value);
            //currentlySelectedSlider.OnValueUpdated.Invoke(); //Muss es �ber die Events gehen oder reicht Slidervalue setzen?
            currentlySelectedDrehknopf.gameObject.GetComponent<ObjectManipulator>().OnManipulationEnded.Invoke(new ManipulationEventData());
        }
    }

    public void setCurrentlySelectedButton(PressableButton pButton)
    {

        if (pButton != null && pButton != currentlySelectedButton)
            pButton.GetComponent<Interactable>().HasFocus = true;//pButton. TouchBegin.Invoke();
        if ((pButton == null && currentlySelectedButton != null) || (pButton != currentlySelectedButton && currentlySelectedButton != null))
        {
            currentlySelectedButton.GetComponent<Interactable>().HasFocus = false; //currentlySelectedButton.TouchEnd.Invoke();
            currentlySelectedButton.ButtonReleased.Invoke();
        }
        currentlySelectedButton = pButton;
        Debug.Log("Currently Selected Button Set: " + pButton);
    }

    public void buttonTargetFound()
    {
        buttonTargetFoundATM = true;
        Debug.Log("Button Target Found");
        currentlySelectedDrehknopf = null;
        currentlySelectedSlider = null;
    }
    public void buttonTargetLost()
    {
        buttonTargetFoundATM = false;
        if (currentlySelectedButton != null && currentlySelectedButton.IsPressing)
            currentlySelectedButton.ButtonReleased.Invoke();
        Debug.Log("Button Target Lost");

    }


    public void setCurrentlySelectedSlider(PinchSlider pSlider)
    {
        if (pSlider != null && pSlider != currentlySelectedSlider)
            pSlider.OnInteractionStarted.Invoke(new SliderEventData(pSlider.SliderValue, pSlider.SliderValue, null, pSlider));
        if ((pSlider == null && currentlySelectedSlider != null) || (pSlider != currentlySelectedSlider && currentlySelectedSlider != null))
            currentlySelectedSlider.OnInteractionEnded.Invoke(new SliderEventData(currentlySelectedSlider.SliderValue, currentlySelectedSlider.SliderValue, null, currentlySelectedSlider));
        currentlySelectedSlider = pSlider;

        Debug.Log("Currently Selected Slider Set: " + pSlider);
    }

    public void sliderTargetFound()
    {
        sliderTargetFoundATM = true;
        Debug.Log("Slider Target Found");
        currentlySelectedDrehknopf = null;
        currentlySelectedButton = null;
    }
    public void sliderTargetLost()
    {
        sliderTargetFoundATM = false;
        Debug.Log("Slider Target Lost");

    }

    public void setCurrentlySelectedDrehknopf(RotaryKnob pDrehknopf)
    {
        if (pDrehknopf != null && pDrehknopf != currentlySelectedDrehknopf)
            pDrehknopf.gameObject.GetComponent<ObjectManipulator>().OnHoverEntered.Invoke(new ManipulationEventData());
        if ((pDrehknopf == null && currentlySelectedDrehknopf != null) || (pDrehknopf != currentlySelectedDrehknopf && currentlySelectedDrehknopf != null))
            currentlySelectedDrehknopf.gameObject.GetComponent<ObjectManipulator>().OnHoverExited.Invoke(new ManipulationEventData());
        currentlySelectedDrehknopf = pDrehknopf;
        Debug.Log("Currently Selected Drehknopf Set: " + pDrehknopf);
    }

    public void drehknopfTargetFound()
    {
        drehknopfTargetFoundATM = true;
        Debug.Log("Drehknopf Target Found");
        currentlySelectedButton = null;
        currentlySelectedSlider = null;
    }
    public void drehknopfTargetLost()
    {
        drehknopfTargetFoundATM = false;
        Debug.Log("Drehknopf Target Lost");

    }

    public static MQTT_Controller GetMQTT_Controller()
    {
        if (mMQTTController == null)
        {
            GameObject GOMQTTController = GameObject.Find("MQTTController");
            mMQTTController = GOMQTTController.GetComponent<MQTT_Controller>();
        }
        return mMQTTController;
    }
}

public class Message
{
    public string messageText;
    public string topic;

    public Message(string pmessageText, string ptopic)
    {
        messageText = pmessageText;
        topic = ptopic;
    }
}
