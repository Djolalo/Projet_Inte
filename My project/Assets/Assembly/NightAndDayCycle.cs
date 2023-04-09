using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NightAndDayCycle : MonoBehaviour
{
    private const int maxTime = 500;

    [SerializeField] private Gradient lightColor;
    [SerializeField] private GameObject light;

    private int days;
    private float time = 50;
    private bool canChangeDay = true;
    
    //you can add this in another script like something that grows crops during the night
    public delegate void OnDayChanged();

    public OnDayChanged DayChanged;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        if(time > maxTime)
        {
            time = 0;
        }

        if( ((int) time == maxTime/2) && canChangeDay)
        {
            canChangeDay = false;
            if(DayChanged!= null) { DayChanged(); }
            days++;
        }

        if( (int) time == maxTime/2 + 5) canChangeDay = true;

        time += Time.deltaTime;

        /*evaluate method should get a value between 0 and 1
         *so we take the inverse of maxTime
         * */
        this.light.GetComponent<UnityEngine.Rendering.Universal.Light2D>().color = lightColor.Evaluate(time / maxTime);
    }

    public int getDays()
    {
        return days;
    }
}
