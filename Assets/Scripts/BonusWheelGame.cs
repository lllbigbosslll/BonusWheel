using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class BonusWheelGame : MonoBehaviour
{
    public enum RollState 
    {
        None, SpeedUp, SpeedDown, End
    }

    public Transform RollPanel;
    public Button startBtn;

    RollState curState; 
    int sectorID; // resultId

    float totalTime = 0;
    float endAngle;

    float MaxSpeed = 1500;
    float factor;
    float accelerateTime = 1;
    float speedUpTime = 3;

    float tempAngle;
    float k = 2f;

    // Start is called before the first frame update
    void Start()
    {
        // unit test, write to file ""
        UnitTest();

        sectorID = 0;
        startBtn.onClick.AddListener(StartTurnWheel);
    }

    // Update is called once per frame
    void Update()
    {
        totalTime += Time.deltaTime;
        // accelerate
        if (curState == RollState.SpeedUp) 
        {
            factor = totalTime / accelerateTime;
            factor = factor > 1 ? 1 : factor;
            RollPanel.Rotate(new Vector3(0, 0, -1) * factor * MaxSpeed * Time.deltaTime, Space.Self);
        }
        // decelerate
        if (totalTime >= speedUpTime && curState == RollState.SpeedUp) 
        {
            curState = RollState.SpeedDown;
            tempAngle = GetTempAngle();
        }
        if (curState == RollState.SpeedDown) 
        {
            tempAngle = Mathf.Lerp(tempAngle, endAngle, Time.deltaTime * k);
            RollPanel.rotation = Quaternion.Euler(0, 0, tempAngle);
            
            // end spinning
            if (Mathf.Abs(tempAngle - endAngle) <= 1)
            {
                curState = RollState.None;
            }
        }
    }

    public void StartTurnWheel()
    {
        if (curState != RollState.None)
        {
            return;
        }
        totalTime = 0;
        tempAngle = 0;
        sectorID = GetRandomID();
        Debug.Log("Sector: " + sectorID);
        endAngle = (-1) * (9 - sectorID) * 45;
        curState = RollState.SpeedUp;
    }

    // get the current wheel angle
    private float GetTempAngle() 
    {
        return (360 - RollPanel.eulerAngles.z) % 360;
    }

    //(0,45)(45,90)(90,135)(135,180)(180,225)(225,279)(270.315)(315,360)
    private int GetRandomID()
    {
        int id = 0;
        int a = Random.Range(0, 100);
        if (80 < a && a <= 100) // 20%
        {
            id = 8;
        }
        else if (75 < a && a <= 80) // 10%
        {
            id = 7;
        }
        else if (55 < a && a <= 75) // 10%
        {
            id = 6;
        }
        else if (50 < a && a <= 55) // 10%
        {
            id = 5;
        }
        else if (40 < a && a <= 50) // 5%
        {
            id = 4;
        }
        else if (30 < a && a <= 40) // 20%
        {
            id = 3;
        }
        else if (20 < a && a <= 30) // 5%
        {
            id = 2;
        }
        else {                      // 20%
            id = 1;
        }
        return id;
    }

    private void UnitTest()
    {
        int[] result = new int[8];
        for (int i = 0; i < 1000; i++) 
        {
            int sector = GetRandomID();
            result[sector - 1]++;
        }
        string[] lines = 
        {   "Life 30 min: " + result[0] + " times\n",
            "Brush 3X: " + result[1] + " times\n",
            "Gems 35: " + result[2] + " times\n",
            "Hammer 3X: " + result[3] + " times\n",
            "Coins 750: " + result[4] + " times\n",
            "Brush 1x: " + result[5] + " times\n",
            "Gems 75: " + result[6] + " times\n",
            "Hammer 1X: " + result[7] + " times"
        };
        using (StreamWriter sw = new StreamWriter("Assets/UnitTest/UnitTest.txt")) 
        {
            foreach(string line in lines) 
            {
                sw.WriteLine(line);
            }
        }
    }
}
